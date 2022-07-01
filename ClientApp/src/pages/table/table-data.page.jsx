import React from 'react';
import { Link } from 'react-router-dom';
import { connect } from 'react-redux';
import { tablesActions } from '../../actions';

import { history } from '../../helpers';
import { Loading, Page, TableRecords } from '../../components';
import { CreateEditTableRecordModal } from '../../modals';
import { tablesService } from '../../services';

const TableDataPage = ({match, location, dispatch, loggedIn}) => {
    const query = new URLSearchParams(location.search);
    const id = query.get('id') ? parseInt(query.get('id')) : 0;
    const mode = query.get('mode') ? query.get('mode').toLowerCase() : 'show';

    const [table, setTable] = React.useState(null);
    const [tableColumns, setTableColumns] = React.useState(null);
    const [loadingTableColums, setLoadingTableColumns] = React.useState(null);

    const [isCreateTableRecordModalVisible, setCreateTableRecordModalVisible] = React.useState(false);
    
    const showCreateRecordModal = () => {
        setCreateTableRecordModalVisible(true);
    }

    const handleOnCreateComplete = (result) => {
        if(result === true) {
            dispatch(tablesActions.getTableRows(table));
        }
        setCreateTableRecordModalVisible(false);
    }

    React.useEffect(() => {
        if(table != match.params.table_name) {
            setTable(match.params.table_name);
            setLoadingTableColumns(true);
            tablesService.getApplicationTableColumns(match.params.table_name).then( response => {
                setLoadingTableColumns(false);
                if(response.Success) {
                    setTableColumns(response.Data);
                }
                else {
                    setTableColumns(null);
                }
            })            
        }

        if (!loggedIn) {
            history.push('/');
        }
    }, [match, loggedIn]);


    // static getDerivedStateFromProps(props, state) {
    //     if (state.table !== props.match.params.table_name) {
    //         const query = new URLSearchParams(props.location.search);
    //         const id = query.get('id') ? parseInt(query.get('id')) : 0;
    //         const mode = query.get('mode') ? query.get('mode').toLowerCase() : 'show';
    //         return { table: props.match.params.table_name, id, mode };
    //     }
    //     return null;
    // }

    return (
        <React.Fragment>
        {
            (loadingTableColums) &&
            <Loading></Loading>
        }
        {
            (!loadingTableColums && !tableColumns) &&
            <div className="row">
            <div className="col-sm-1 col-md-3"></div>
            <div align="center" className="col-sm-10 col-md-6 col-md-offset-3" style={{ "marginTop": "25vh" }}>
                <p>Nothing to show...
                </p>
            </div>
            <div className="col-sm-1 col-md-3"></div>
        </div>
        }
        {
            (!loadingTableColums && tableColumns) &&
            <div>
                <div align="right" style={{marginBottom: "16px"}}>
                    <button className="btn btn-primary" onClick={showCreateRecordModal}>Create a new row</button>
                </div>
                <TableRecords key={table + 'TableRecords'} table={table} tableColumns={tableColumns}/>
            </div>
        }
        {                
            isCreateTableRecordModalVisible && tableColumns &&

            <CreateEditTableRecordModal
                key={table + 'CreateEditTableRecordModal'}
                table={table}
                tableColumns={tableColumns}
                row={{}}
                handleOnClose={handleOnCreateComplete}
                mode="create"
                buttonLabel="Create a new"
            />
        }
        </React.Fragment>
    );
}

function mapStateToProps(state) {
    const { loggedIn } = state.authentication;
    return {
        loggedIn,
    };
}

const connectedTable = connect(mapStateToProps)(TableDataPage);
export { connectedTable as TableDataPage };