import React, { Component } from 'react';
import { connect } from 'react-redux';
import { RiPencilFill, RiDeleteRow } from 'react-icons/ri';
import { tablesActions } from '../actions';
import { CreateEditTableRecordModal } from '../modals'
import { DialogModal } from '../modals'
import { ButtonIcon } from './button-icon.component';
import { Table } from 'reactstrap';
import { Loading } from './loading.component';
import { ApiraiserTable } from '.';

const TableRecords = ({table, tableColumns, loadingCurrentTableRows, currentTableRows, dispatch, deletedRowSuccess, deletingRow}) => {
    const [currentTable, setCurrentTable] = React.useState(null);
    const [currentRow, setCurrentRow] = React.useState(null);
    const [isDeleteModalVisible, setDeleteModalVisible] = React.useState(false);
    const [isEditModalVisible, setEditModalVisible] = React.useState(false);
            
    const buttonsConfig = [
        {
            label: "Delete",
            class: "btn-info",
            handle: () => handleOnDeleteComplete(true),
            closeOnClick: true
        },
        {
            label: "Cancel",
            class: "btn-secondary",
            handle: () => handleOnDeleteComplete(null),
            closeOnClick: true
        },
    ];
    
    const showEditModal = (row) => {
        setEditModalVisible(true);
        setCurrentRow(row);
    }

    const showDeleteConfirmationModal = (row) => {
        setDeleteModalVisible(true);
        setCurrentRow(row);        
    }

    const handleOnEditComplete = (result) => {        
        console.log(result);
        if(result === true) {
            console.log('getTableRows');
            dispatch(tablesActions.getTableRows(table));
        }
        setEditModalVisible(false);
    }

    const handleOnDeleteComplete = (result) => {
        console.log(result);
        if(result === true) {
            console.log(currentRow);
            console.log(currentRow.Id);
            dispatch(tablesActions.deleteRow(table, currentRow.Id));    
        }

        setDeleteModalVisible(false);
    }

    React.useEffect(() => {
        if(table != currentTable) {
            setCurrentTable(table);
            dispatch(tablesActions.getApplicationTableColumns(table));
            dispatch(tablesActions.getTableRows(table));
        }
        if(deletingRow === false) {
            dispatch(tablesActions.acknowledgeDeleteRow());
            if(deletedRowSuccess === true) {
                dispatch(tablesActions.getTableRows(table));
            }
        }
        if(loadingCurrentTableRows === false) {
            dispatch(tablesActions.acknowledgeGetRows());
        }
    }, [table, loadingCurrentTableRows, deletingRow, deletedRowSuccess]);

    return (
        <React.Fragment>
            {
                (loadingCurrentTableRows || deletingRow) &&
                <Loading></Loading>
            }
            {
                (loadingCurrentTableRows === null && deletingRow === null && (!currentTableRows || currentTableRows.length < 1)) &&
                <div>
                    <p>No records found!</p>
                </div>
            }
            {   
                (loadingCurrentTableRows === null && deletingRow === null && currentTableRows) &&          
                <div>
                    <ApiraiserTable headers={Object.keys(currentTableRows[0])}>
                        {currentTableRows &&
                            currentTableRows.map((row, i) => (
                                <tr key={'row_' + (i + 1)}>

                                    <td key={'data_' + i + '_#'} scope="row">
                                        <div style={{display: 'flex', flexDirection: 'row'}}>
                                            <ButtonIcon icon="edit" color="#007bff" onClick={() => showEditModal(row)}/>
                                            <ButtonIcon icon="trash" color="#dc3545" onClick={() => showDeleteConfirmationModal(row)}/>
                                        </div>
                                    </td>
                                    {
                                        Object.keys(currentTableRows[0]).map(key => (
                                            <td key={'data_' + i + key} >{row[key] != null ? '' + row[key]: ''}</td>
                                        ))
                                    }
                                </tr>
                            ))
                        }
                    </ApiraiserTable>                    
                    {isEditModalVisible &&
                        <CreateEditTableRecordModal
                            key={table}
                            table={table}
                            tableColumns={tableColumns}
                            row={currentRow}
                            handleOnClose={handleOnEditComplete}
                            mode="edit"
                            label="Edit" />
                    }
                    {isDeleteModalVisible &&

                        <DialogModal
                            headerLabel="Delete"
                            handleOnClose={handleOnDeleteComplete}
                            text="Do you really want to delete?"
                            buttonsConfig={buttonsConfig}
                        />

                    }

                </div>
            }
        </React.Fragment>
    )
}


function mapStateToProps(state) {
    const { loggedIn } = state.authentication;
    const { loadingCurrentTableRows, currentTableRows } = state.currentTableData;
    const { deletedRowSuccess, deletingRow } = state.currentTableData;

    return {
        deletedRowSuccess,
        deletingRow,
        loadingCurrentTableRows,
        currentTableRows,
        loggedIn,

    };

}

const connectedTableRecords = connect(mapStateToProps)(TableRecords);
export { connectedTableRecords as TableRecords };
