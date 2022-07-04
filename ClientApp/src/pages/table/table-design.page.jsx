import React from "react";
import { Link } from "react-router-dom";
import { connect } from "react-redux";
import { tablesActions } from "../../actions";

import { history } from "../../helpers";
import { Loading, Page, TableColumns } from "../../components";
import { CreateTableColumnModal, AddTableColumnModal } from "../../modals";
import { tablesService } from "../../services";

const TableDesignPage = ({ schema, match, location, dispatch, loggedIn }) => {
    const query = new URLSearchParams(location.search);
    const id = query.get("id") ? parseInt(query.get("id")) : 0;
    const mode = query.get("mode") ? query.get("mode").toLowerCase() : "show";

    const [table, setTable] = React.useState(null);

    const [tableColumns, setTableColumns] = React.useState(null);
    const [loadingTableColums, setLoadingTableColumns] = React.useState(null);

    const [predefinedColumns, setPredefinedColumns] = React.useState(null);
    const [loadingPredefinedColums, setLoadingPredefinedColumns] =
        React.useState(null);

    const [isCreateTableColumnModalVisible, setCreateTableColumnModalVisible] =
        React.useState(false);

    const handleColumnSubmit = (columnProperties) => {
        // e.preventDefault();
        // this.setState({ submitted: true });
        // const { dispatch } = this.props;
        console.log(columnProperties);

        // this.setState({addTableColumn: true})
    };

    const handleCreateTableModalToggle = (isVisible) => {
        setCreateTableColumnModalVisible(isVisible);
    };

    const handleOnCreateColumnComplete = (result) => {
        if (result === true) {
            dispatch(tablesActions.getTableColumns(schema, table));
        }
        setCreateTableColumnModalVisible(false);
    };

    const showCreateColumnModal = () => {
        setCreateTableColumnModalVisible(true);
    };

    const handleOnCreateComplete = (result) => {
        if (result === true) {
            dispatch(tablesActions.getTableColumns(schema, table));
        }
        setCreateTableColumnModalVisible(false);
    };

    React.useEffect(() => {
        if (table != match.params.table_name) {
            setTable(match.params.table_name);
            setLoadingTableColumns(true);
            tablesService
                .getTableColumns(schema, match.params.table_name)
                .then((response) => {
                    setLoadingTableColumns(false);
                    if (response.Success) {
                        setTableColumns(response.Data);
                    } else {
                        setTableColumns(null);
                    }
                });
            tablesService
                .getPredefinedColumns(match.params.table_name)
                .then((response) => {
                    setLoadingPredefinedColumns(false);
                    if (response.Success) {
                        setPredefinedColumns(response.Data);
                    } else {
                        setPredefinedColumns(null);
                    }
                });
        }

        if (!loggedIn) {
            history.push("/");
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
            {(loadingTableColums || loadingPredefinedColums) && (
                <Loading></Loading>
            )}
            {!loadingTableColums && !loadingPredefinedColums && !tableColumns && (
                <div className="row">
                    <div className="col-sm-1 col-md-3"></div>
                    <div
                        align="center"
                        className="col-sm-10 col-md-6 col-md-offset-3"
                        style={{ marginTop: "25vh" }}
                    >
                        <p>Nothing to show...</p>
                    </div>
                    <div className="col-sm-1 col-md-3"></div>
                </div>
            )}
            {!loadingTableColums &&
                tableColumns &&
                !loadingPredefinedColums &&
                predefinedColumns && (
                    <div>
                        <div align="right" style={{ marginBottom: "16px" }}>
                            <button
                                className="btn btn-primary"
                                onClick={showCreateColumnModal}
                            >
                                Create a new Column
                            </button>
                        </div>

                        <TableColumns table={table} />
                        {isCreateTableColumnModalVisible && (
                            <AddTableColumnModal
                                table={table}
                                tableColumns={tableColumns}
                                predefinedColumns={predefinedColumns}
                                headerLabel="Create Column"
                                handleOnClose={handleOnCreateColumnComplete}
                            />
                        )}
                    </div>
                )}
        </React.Fragment>
    );
};

// render() {
//         const { loggedIn, loadingCurrentTable, currentTableColumns, dispatch } = this.props;

//         if (!loggedIn) {
//             history.push('/');
//         }

//         if (!loadingCurrentTable && (!currentTableColumns || currentTableColumns.name !== table)) {
//             dispatch(tablesActions.getTableColumns(schema, table));
//         }
//         return (
//             <div>
//                 <div align="right" style={{ marginBottom: "16px" }}>
//                     <button className="btn btn-primary" onClick={this.showCreateColumnModal}>Create a new Column</button>
//                 </div>

//                 <TableColumns table={table} />
//                 {this.state.isCreateTableColumnModalVisible &&
//                     <AddTableColumnModal table={table} headerLabel="Create Column" handleOnClose={this.handleOnCreateColumnComplete} />

//                 }
//             </div>
//         );
//     }
// }

function mapStateToProps(state) {
    const { loggedIn } = state.authentication;
    const { loadingCurrentTable, currentTableColumns } = state.currentTableInfo;
    const { loadingPredefinedColumns, predefinedColumns } =
        state.predefinedColumnsInfo;
    return {
        loggedIn,
        loadingCurrentTable,
        currentTableColumns,
        loadingPredefinedColumns,
        predefinedColumns,
    };
}

const connectedTable = connect(mapStateToProps)(TableDesignPage);
export { connectedTable as TableDesignPage };
