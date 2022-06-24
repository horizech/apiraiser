import React, { useEffect, useState } from 'react';
import { Button, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
// import { CreateEditSettingRecord } from '../components';
import { Form, Field } from 'react-final-form';
import { DynamicElementAdapter } from '../adapters';
import { history, GetElementTypeFromDataType } from '../helpers';

const CreateEditModal = ({ columns, row, label, mode, handleOnClose, isSelectList, selectOptions}) => {
    const [isVisible, setIsVisible] = useState(true);
    const [isJsonVisible, setJsonVisible] = React.useState(true);
    // const [columns, setColumns] = React.useState(null);
    console.log(selectOptions);
    const toggle = () => {
        const newIsVisible = !isVisible;
        setIsVisible(newIsVisible);
        if (!newIsVisible) {
            handleOnClose(null);
        }
    }
  
 console.log(columns);
    const handleOnSubmit = (values) => {
        columns.filter((column)=> column.IsSelectList).map((column)=>{
            values[column.Name]= values[column.Name].value;
        });
        columns.filter((column)=> column.needParse).map((column)=>{
            values[column.Name]= parseInt(values[column.Name]);
        });
        columns.filter((column)=> column.Datatype== "Boolean" && values[column.Name]== null).map((column)=>{
            values[column.Name]= false;
        });
        console.log("values",values);
        const newIsVisible = !isVisible;
        
        setIsVisible(newIsVisible);
        handleOnClose(values);
    }
    
    const getIsColumnReadOnly = (column) => {
        switch (column.Name) {
            case "Id" :case "CreatedOn": case "CreatedBy": case "LastUpdatedOn": case "LastUpdatedBy": return true;
            default: return false;
        }
    }
    const getIsSelectColumn = (column) => {
        console.log(column);
        const selectColumns = [];
        if(selectColumns && selectColumns.length) {
            const selectColumnSearch = selectColumns.filter(c => c.Name == column.Name);
            if(selectColumnSearch && selectColumnSearch.length) {
                return true;
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }
    }
  
    console.log(row);
    console.log(isSelectList);
    console.log(selectOptions);
    
    return (
        <div>
            <Modal isOpen={isVisible} toggle={toggle} title={label}>
                <ModalHeader toggle={toggle}>{label} Row</ModalHeader>
                <ModalBody>

                    {(columns && columns[0].Name) &&
                        <div>

                            <Form

                                onSubmit={handleOnSubmit}
                                initialValues={{
                                    ...row,
                                }}
                                // validate={handleValidate}
                                render={({ handleSubmit, form, submitting, pristine, values }) => (

                                    <form name="form" className="row" id="createEditSettingForm" onSubmit={(e) => handleOnSubmit(values, e)}>
                                        {columns &&
                                            columns.filter(column => !getIsColumnReadOnly(column) || mode == 'edit').map(column => (
                                                <Field isWorking={false} type={GetElementTypeFromDataType(column.Datatype)} key={column.Name} name={column.Name} required="true" component={DynamicElementAdapter} selectOptions={selectOptions ? selectOptions[column.Name]: null} isSelect={isSelectList? isSelectList[column.Name]: false} column={column} />
                                            ))
                                        }
                                        {
                                            isJsonVisible && <pre>{JSON.stringify(values, 0, 2)}</pre>
                                        }
                                        <div className="col-md-12">
                                            <button type="button" form="createEditTableForm" value="Submit" className="btn btn-primary m-2"
                                                onClick={() =>
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

                </ModalBody>
            </Modal>
        </div>
    );
}

export default CreateEditModal;