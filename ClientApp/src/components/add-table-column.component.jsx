import React, { Component, useRef } from 'react';
import { Link } from 'react-router-dom';
import { connect } from 'react-redux';
import { withRouter } from "react-router-dom";
import { tablesActions } from '../actions';
import { history, GetElementTypeFromDataType } from '../helpers';
import { Page } from './page.component';
import { Form, Field } from 'react-final-form'
import { Columns } from "../enums"
import { ReactSelectAdapter } from '../adapters';
import { DynamicElementAdapter } from '../adapters';
import { TablesApiProvider } from "../api_providers/tables.api";
import { tablesService } from '../services';


const AddTableColumn = ({ table, loggedIn, loadingCurrentTable,
    dispatch, addingColumn, tableColumns, loadingCurrentTableRows,
    currentTableRows,  deletedRowSuccess, deletingRow, addedColumnSuccess,  mode, row, handleOnComplete }) => {
    
    let formRef = null;
    
    const [ showJson, setShowJson ] = React.useState(true);
        
    const formData = {
        Name: null,
        Datatype: { label: 'Nothing', value: -1 },
        IsRequired: false,
        IsUnique: false,
        IsForeignKey: false,
        ForeignSchema: { label: 'Not Selected', value: 'Not Selected' },
        ForeignTable: null,
        ForeignName: null,
        DefaultValue: null
    };
    
    // const [ isForeignKey, setForeignKey ] = React.useState(true);
    // const [ foreignSchema, setForeignSchema ] = React.useState(null);
    // const [ foreignTable, setForeignTable ] = React.useState(null);
    // const [ foreignName, setForeignName ] = React.useState(null);
    
    const foreignSchemaOptions = [
        { label: 'System', value: "System" },
        { label: 'Application', value: "Application" }
    ];

    let curForeignSchema = null;
    let curForeignTable = null;


    let foreignTableOptions = [];
    let foreignNameOptions = [];
    
    let clearForeignTableOnNextRender = false;
    let clearForeignNameOnNextRender = false;
    

    // const [ foreignTableOptions, setForeignTableOptions ] = React.useState(null);
    // const [ foreignNameOptions, setForeignNameOptions ] = React.useState(null);
    
    const labels = {
        Datatype: 'Data type',
        IsRequired: 'Is required?',
        IsUnique: 'Is unique?',
        IsForeignKey: 'Is foreign key?',
        ForeignSchema: 'Foreign schema',
        ForeignTable: 'Foreign table',
        ForeignName: 'Foreign column',
        DefaultValue: 'Default value'
    };

    const columns = [
        {
            "Name": "Name",
            "Datatype": "LongText",
            "IsRequired": true
        },
        {
            "Name": "Datatype",
            "Datatype": "Integer",
            "IsRequired": true
        },
        {
            "Name": "IsRequired",
            "Datatype": "Boolean",
            "IsRequired": false
        },
        {
            "Name": "IsUnique",
            "Datatype": "Boolean",
            "IsRequired": false
        },
        {
            "Name": "IsForeignKey",
            "Datatype": "Boolean",
            "IsRequired": false
        },
        {
            "Name": "ForeignSchema",
            "Datatype": "ShortText",
            "IsRequired": false
        },
        {
            "Name": "ForeignTable",
            "Datatype": "ShortText",
            "IsRequired": false
        },
        {
            "Name": "ForeignName",
            "Datatype": "ShortText",
            "IsRequired": false
        },
        {
            "Name": "DefaultValue",
            "Datatype": "ShortText",
            "IsRequired": false
        }
    ]
   
    const prepareFormData = (data) => {
        let result = {
            Name: data.Name ? data.Name : null,
            IsRequired: data.IsRequired ? data.IsRequired : false,
            IsUnique: data.IsUnique ? data.IsUnique : false,
            IsForeignKey: data.IsForeignKey ? data.IsForeignKey : false,
            Datatype: data.Datatype ? data.Datatype.value : null,
            ForeignSchema: data.IsForeignKey && data.ForeignSchema ? data.ForeignSchema.value : null,
            ForeignTable: data.IsForeignKey && data.ForeignTable ? data.ForeignTable.value : null,
            ForeignName: data.IsForeignKey && data.ForeignName ? data.ForeignName.value : null,
            DefaultValue: data.DefaultValue ? data.DefaultValue : null
        };

        return result;
    }

    // const handleValidate = (data) => {
    //     // console.log(prepareFormData(data));
    // }

    const handleOnSubmit = (columnInfo, event) => {

        columnInfo = prepareFormData(columnInfo);

        if (event && event.preventDefault) {
            event.preventDefault();
        }

        dispatch(tablesActions.addColumn(table, columnInfo));

    }

    const handleValidate = (values) => {
        console.log(values);
        clearForeignTableOnNextRender = false;
        clearForeignNameOnNextRender = false;

        if(values.ForeignSchema != null && values.ForeignSchema.value != curForeignSchema) {
            curForeignSchema = values.ForeignSchema.value;
            clearForeignTableOnNextRender = true;            
            clearForeignNameOnNextRender = true;
            loadForeignTableOptions(values);
        }
        if(values.ForeignTable != null && values.ForeignTable.value != curForeignTable) {
            curForeignTable = values.ForeignTable.value;
            clearForeignNameOnNextRender = true;
            loadForeignNameOptions(values);
        }
        // if(values.IsForeignKey != null && values.IsForeignKey != isForeignKey) {
        //     // setForeignKey(values.IsForeignKey);
        //     // formData.IsForeignKey = values.IsForeignKey;
        // }
    }
    
    const loadForeignTableOptions = (values) => {
        const foreignSchema = (values.ForeignSchema.value);
        if(foreignSchema) {
            tablesService.getTables(foreignSchema).then(
                result => {
                    if (result.Success) {
                        console.log(result.Data);                        
                        foreignTableOptions = (
                            result.Data.map(x => {
                                    return { label: x, value: x }
                                }
                            )
                        );                            
                    }
                }
            );
        }
    }

    const loadForeignNameOptions = (values) => {
        console.log(values);
        const foreignSchema = (values.ForeignSchema.value);
        const foreignTable = (values.ForeignTable.value);
        if (foreignSchema && foreignTable) {
            tablesService.getTableColumns(foreignSchema, foreignTable).then(
                result => {
                    if (result.Success) {                
                        foreignNameOptions = (
                            result.Data.map(x => {
                                    return { label: x['Name'], value: x['Name'] }
                                }
                            )
                        );
                    }
                }
            );
        }
    }


    // React.useEffect(() => {
    //     console.log('-> foreignSchema');
    //     console.log(`foreignSchema: ${foreignSchema}, foreignTable: ${foreignTable}, foreignName: ${foreignName}`);
    //     loadForeignTableOptions();
    // }, [foreignSchema]);

    // React.useEffect(() => {       
    //     console.log('-> foreignTable');
    //     console.log(`foreignSchema: ${foreignSchema}, foreignTable: ${foreignTable}, foreignName: ${foreignName}`);
    //     loadForeignNameOptions();      
    // },[ foreignTable ]);


    React.useEffect(() => {               
        console.log('formData: ', formData);
        if (!loggedIn) {
            history.push('/');
        }
    },[ ]);

    const getColumns = (values) => {
            
        let filteredColumns = [];
        let isSelectList = {};
        let selectOptionsList = {};
        // console.log(filteredColumns);
        columns.forEach((column, i) => {
            switch (column.Name) {
                case 'Name': case 'IsRequired': case 'IsUnique': case 'IsForeignKey':
                    isSelectList[column.Name] = false;
                    filteredColumns.push(column);
                    break;
                case 'Datatype':
                    isSelectList[column.Name] = true;
                    selectOptionsList[column.Name] = Columns.DataTypesOptions;
                    filteredColumns.push(column);
                    break;
                case 'ForeignSchema':
                    isSelectList[column.Name] = true;
                    selectOptionsList[column.Name] = foreignSchemaOptions;
                    if (values.IsForeignKey) {
                        filteredColumns.push(column);
                    }
                    break;
                case 'ForeignTable':
                    isSelectList[column.Name] = true;
                    selectOptionsList[column.Name] = foreignTableOptions;
                    if (values.IsForeignKey) {
                        filteredColumns.push(column);
                    }
                    break;
                case 'ForeignName':
                    isSelectList[column.Name] = true;
                    selectOptionsList[column.Name] = foreignNameOptions;
                    console.log(selectOptionsList);
                    if (values.IsForeignKey) {
                        filteredColumns.push(column);
                    }
                    break;
                case 'DefaultValue':
                    const dataTypesKeys = Object.keys(Columns.DataTypes);
                    const dataTypeSelectedValue = values['Datatype'] ? values['Datatype'].value : 0;
                    dataTypesKeys.forEach(key => {
                        if (Columns.DataTypes[key] == dataTypeSelectedValue) {
                            column.Datatype = key;
                        }
                    });
                    filteredColumns.push(column);
                    break;

            }
        });

        return filteredColumns.map(column => (
            <Field
                isWorking={addingColumn || false}
                isSelect={isSelectList[column.Name]}
                label={labels[column.Name]}
                selectOptions={selectOptionsList[column.Name]}
                type={GetElementTypeFromDataType(column.Datatype)}
                key={column.Name} name={column.Name} required={true}
                component={DynamicElementAdapter} column={column}

            />
        ))
    }

    if (addedColumnSuccess === true && addingColumn === false && handleOnComplete) {
        dispatch(tablesActions.acknowledgeAddColumn());
        handleOnComplete(true);
    }

    if (addingColumn) {
        return (
            <div className="row">
                <div className="col-sm-1 col-md-3"></div>
                <div align="center" className="col-sm-10 col-md-6 col-md-offset-3" style={{ "marginTop": "25vh" }}>
                    <p>Loading...
                        <span style={{ marginLeft: '16px' }}>
                            <img src="data:image/gif;base64,R0lGODlhEAAQAPIAAP///wAAAMLCwkJCQgAAAGJiYoKCgpKSkiH/C05FVFNDQVBFMi4wAwEAAAAh/hpDcmVhdGVkIHdpdGggYWpheGxvYWQuaW5mbwAh+QQJCgAAACwAAAAAEAAQAAADMwi63P4wyklrE2MIOggZnAdOmGYJRbExwroUmcG2LmDEwnHQLVsYOd2mBzkYDAdKa+dIAAAh+QQJCgAAACwAAAAAEAAQAAADNAi63P5OjCEgG4QMu7DmikRxQlFUYDEZIGBMRVsaqHwctXXf7WEYB4Ag1xjihkMZsiUkKhIAIfkECQoAAAAsAAAAABAAEAAAAzYIujIjK8pByJDMlFYvBoVjHA70GU7xSUJhmKtwHPAKzLO9HMaoKwJZ7Rf8AYPDDzKpZBqfvwQAIfkECQoAAAAsAAAAABAAEAAAAzMIumIlK8oyhpHsnFZfhYumCYUhDAQxRIdhHBGqRoKw0R8DYlJd8z0fMDgsGo/IpHI5TAAAIfkECQoAAAAsAAAAABAAEAAAAzIIunInK0rnZBTwGPNMgQwmdsNgXGJUlIWEuR5oWUIpz8pAEAMe6TwfwyYsGo/IpFKSAAAh+QQJCgAAACwAAAAAEAAQAAADMwi6IMKQORfjdOe82p4wGccc4CEuQradylesojEMBgsUc2G7sDX3lQGBMLAJibufbSlKAAAh+QQJCgAAACwAAAAAEAAQAAADMgi63P7wCRHZnFVdmgHu2nFwlWCI3WGc3TSWhUFGxTAUkGCbtgENBMJAEJsxgMLWzpEAACH5BAkKAAAALAAAAAAQABAAAAMyCLrc/jDKSatlQtScKdceCAjDII7HcQ4EMTCpyrCuUBjCYRgHVtqlAiB1YhiCnlsRkAAAOwAAAAAAAAAAAA==" />
                        </span>
                    </p>
                </div>
                <div className="col-sm-1 col-md-3"></div>
            </div>
        )
    }
    else {
        console.log('formData: ', formData);
        return (
            <div>

                <Form
                    mutators={{
                        clear: ([address], state, { changeValue }) => {
                        changeValue(state, "address", () => undefined);
                        }
                    }}
                    onSubmit={handleOnSubmit}
                    initialValues={{
                        ...formData,
                    }}
                    validate={handleValidate}
                    render={({ handleSubmit, form, submitting, pristine, values }) => (
                        <React.Fragment>
                            {
                                console.log(clearForeignTableOnNextRender, clearForeignNameOnNextRender)
                            }
                            {
                                clearForeignTableOnNextRender && form.change('ForeignTable', undefined)
                            }
                            {
                                clearForeignNameOnNextRender && form.change('ForeignName', undefined)
                            }
                            <form name="form" className="row" id="addColumnForm" onSubmit={(e) => handleOnSubmit(values, e)}>
                            {
                                getColumns(values)
                            }

                            <br />
                            {
                                showJson && <p>{JSON.stringify(prepareFormData(values), 0, 2)}</p>
                            }
                            <div className="col-md-12">
                                <button type="button"
                                    form="addColumnForm"
                                    value="Submit"
                                    className="btn btn-primary m-2"
                                    onClick={() => handleOnSubmit(values)}>
                                    Add
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
                        </React.Fragment>


                    )}
                />
            </div>
        );
    }

}


function mapStateToProps(state) {
    const { loggedIn } = state.authentication;
    const { addedColumnSuccess, addingColumn, currentTableColumns, loadingCurrentTable } = state.currentTableInfo;
    const { tableslist, loadingTablesList } = state.tables
    return {
        addedColumnSuccess,
        addingColumn,
        loggedIn,
        tableslist,
        loadingTablesList,
        currentTableColumns,
        loadingCurrentTable
    };

}

const connectedAddTableColumn = connect(mapStateToProps)(AddTableColumn);
export { connectedAddTableColumn as AddTableColumn };
