import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import { connect } from 'react-redux';
import { withRouter } from "react-router-dom";
import { tablesActions } from '../actions';
import { history, GetElementTypeFromDataType } from '../helpers';
import { Page } from './page.component';
import { Form, Field } from 'react-final-form'
import { Columns } from "../enums"
import { DynamicElementAdapter } from '../adapters';

const CreateEditTableRecord = ({mode, table, dispatch, insertedRowSuccess, insertingRow, updatedRowSuccess, updatingRow, tableColumns, row, handleOnComplete}) => {
    const [isJsonVisible, setJsonVisible] = React.useState(true);

    const getIsColumnReadOnly = (column) => {
        switch (column) {
            case "Id": case "CreatedOn": case "CreatedBy": case "LastUpdatedOn": case "LastUpdatedBy": return true;
            default: return false;
        }
    }

    const handleOnSubmit = (data, event) => {
        tableColumns.filter((column)=> column.Datatype== "Integer").map((column)=>{
            data[column.Name]= parseInt(data[column.Name]);
        });
        if(event && event.preventDefault) {
            event.preventDefault();
        }
        if (mode === 'create') {
            dispatch(tablesActions.insertRow(table, data));
        }
        else if (mode === 'edit') {
            dispatch(tablesActions.updateRow(table, data));
        }
    }

    const handleValidate = (values) => {
        // console.log(values);        
    }

    React.useEffect(() => {
        if(insertingRow === false) {
            dispatch(tablesActions.acknowledgeInsertRow());
            handleOnComplete(insertedRowSuccess);
        }
        
        if(updatingRow === false) {
            dispatch(tablesActions.acknowledgeUpdateRow());
            handleOnComplete(updatedRowSuccess);
        }

    }, [insertingRow, insertedRowSuccess, updatingRow, updatedRowSuccess]);

    return (
        <React.Fragment>
            {
                (!tableColumns) &&            
                <div className="row">
                    <div className="col-sm-1 col-md-3"></div>
                    <div align="center" className="col-sm-10 col-md-6 col-md-offset-3" style={{ "marginTop": "25vh" }}>
                        <p>Table columns were not found!                            
                        </p>
                    </div>
                    <div className="col-sm-1 col-md-3"></div>
                </div>            
            }
            {
                (tableColumns) &&
                <div>
                    {/* <div>
                        {mode === 'create' && <p>Add new Record:</p>}
                        {mode === 'edit' && <p>Update Record:</p>}
                    </div>
                        */}
                    <Form

                        onSubmit={handleOnSubmit}
                        initialValues={{
                            ...row,
                            }}
                        validate={handleValidate}
                        render={({ handleSubmit, form, submitting, pristine, values }) => (
                        
                        <form name="form" className="row" id="createEditTableForm" onSubmit={(e) => handleOnSubmit(values, e)}>
                            { 
                                tableColumns.filter(column => !getIsColumnReadOnly(column.Name) || mode == 'edit').map(column => (
                                    <Field isWorking={insertingRow ||  updatingRow || false} type={GetElementTypeFromDataType(column.Datatype)} key={column.Name} name={column.Name} required={column.IsRequired} component={DynamicElementAdapter} column={column}/>
                                ))
                            }
                            {
                                isJsonVisible && <pre>{JSON.stringify(values, 0, 2)}</pre>
                            }
                            <div className="col-md-12">
                                <button type="button" form="createEditTableForm" value="Submit" className="btn btn-primary m-2"
                                    onClick={() =>
                                        // { cancelable: true } required for Firefox
                                        // https://github.com/facebook/react/issues/12639#issuecomment-382519193
                                        // document
                                        // .getElementById('createEditTableForm')
                                        // .dispatchEvent(new Event('submit', { cancelable: true }))
                                        handleOnSubmit(values)
                                    }
                                >
                                    {mode === 'create' ? 'Add' : 'Update'}
                                </button>
                                <button
                                    type="button" className="btn btn-info m-2"
                                    onClick={form.reset}
                                    disabled={submitting || pristine}
                                    >
                                    Reset
                                </button>
                            </div>
                        </form>
                
                        )}
                    />
                </div>
            }
        </React.Fragment>
    );
}


function mapStateToProps(state) {
    const { loggedIn } = state.authentication;
    const { insertedRowSuccess, insertingRow, updatedRowSuccess, updatingRow } = state.currentTableData;

    return {
        insertedRowSuccess,
        insertingRow,
        updatedRowSuccess,
        updatingRow,
        loggedIn,
    };

}

const connectedCreateEditTableRecord = connect(mapStateToProps)(CreateEditTableRecord);
export { connectedCreateEditTableRecord as CreateEditTableRecord };
