import React from 'react';
import { Link } from 'react-router-dom';
import { connect } from 'react-redux';
import { tablesActions } from '../actions';
import { Table } from 'reactstrap';
import { ButtonIcon } from '../components/button-icon.component';
import { history, SwalAlert } from '../helpers';
import { Loading, Page, Roles } from '../components';
import { CreateEditModal } from '../modals';
import { rolesService } from '../services';
import { DialogModal } from '../modals'
import { ApiraiserTable } from '../components';
import { alertActions, toastActions } from '../actions';

const RolesPage = ({match, location, dispatch, loggedIn}) => {
 
    const [roles, setRoles] = React.useState(null);
    const [loadingRoleColumns, setLoadingRoleColumns] = React.useState(true);
    const [isCreateRoleModalVisible, setCreateRoleModalVisible] = React.useState(false);
    const [currentRow, setCurrentRow] = React.useState(null);
    const [isDeleteModalVisible, setDeleteModalVisible] = React.useState(false);
    const [isEditModalVisible, setEditModalVisible] = React.useState(false);
    const [deleteSuccess, setDeleteSuccess] = React.useState(false);
    const [addSuccess, setAddSuccess] = React.useState(false);
    const [updateSuccess, setUpdateSuccess] = React.useState(false);
    
    const columns = [
        { Name: 'Id', value: 'Id', valueDataType: 'Integer', needParse: false  },
        { Name: 'Name', value: 'Name', valueDataType: 'Integer'  },
        { Name: 'Description', value: 'Description', valueDataType: 'Integer', needParse: false  },
        { Name: 'Level', value: 'Level', valueDataType: 'Integer', needParse: true  },
        { Name: 'CreatedOn', value: 'CreatedOn', valueDataType: 'Integer', needParse: false  },
        { Name: 'CreatedBy', value: 'CreatedBy', valueDataType: 'Integer', needParse: false  },
        { Name: 'LastUpdatedOn', value: 'LastUpdatedOn', valueDataType: 'Integer', needParse: false  },
        { Name: 'LastUpdatedBy', value: 'LastUpdatedBy', valueDataType: 'Integer', needParse: false  }
    ]

    const showCreateModal = () => {
        setCreateRoleModalVisible(true);
    }

    const handleOnCreateComplete = (values) => {
        if(values) {
            rolesService.addRoles(values).then( response => {
                if(response.Success) {
                    setAddSuccess(true);
                    dispatch(toastActions.success(response.Message));
                }
                else {
                    setAddSuccess(false);
                    dispatch(alertActions.error("Error", response.Message));
                }
            });  
        }
        setCreateRoleModalVisible(false);
    }
    const showEditModal = (row) => {
        setEditModalVisible(true);
        setCurrentRow(row);
    }

    const showDeleteConfirmationModal = (row) => {
        setDeleteModalVisible(true);
        setCurrentRow(row);        
    }
    
    const handleOnEditComplete = (values) => {        
        if(values) {
            rolesService.updateRoles(currentRow.Name, values).then( response => {
                if(response.Success) {
                    setUpdateSuccess(true);
                    dispatch(toastActions.success(response.Message));
                }
                else {
                    setUpdateSuccess(false);
                    dispatch(alertActions.error("Error", response.Message));
                }
            });  
        }
        setEditModalVisible(false);
    }

    const handleOnDeleteComplete = (result) => {
        if(result === true) {
            rolesService.deleteRoles(currentRow).then( response => {
                if(response.Success) {
                    setDeleteSuccess(true);
                    dispatch(toastActions.success(response.Message));
                }
                else {
                    setDeleteSuccess(false);
                    dispatch(alertActions.error("Error", response.Message));
                }
            });  
        }

        setDeleteModalVisible(false);
    }
        
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

    React.useEffect(() => {
       
        rolesService.getRoles().then( response => {
            if(response.Success) {
                console.log(response.Data);
                setRoles(response.Data);   
                   
            }
            else {
               
                setRoles(null);
            }
        })       
        setLoadingRoleColumns(false);
        if (!loggedIn) {
            history.push('/');
        }
        setAddSuccess(false);
        setDeleteSuccess(false);
        setUpdateSuccess(false);
    }, [match, loggedIn, deleteSuccess, addSuccess, updateSuccess]);

    return (
        <React.Fragment>
            <div align="right" style={{marginBottom: "16px"}}>
                    <button className="btn btn-primary" onClick={showCreateModal}>Create a new role</button>
                </div>
        {
            (loadingRoleColumns) &&
            <Loading></Loading>
        }
        {
            (!loadingRoleColumns  && (!roles || roles["length"] == 0) ) &&
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
            (!loadingRoleColumns && roles && roles["length"] != 0) &&
            <div>
                <ApiraiserTable headers={Object.keys(roles[0])}>
                {roles &&
                                roles.map((row, i) => (
                                    <tr key={'row_' + (i + 1)}>

                                        <td key={'data_' + i + '_#'} scope="row">
                                            <div style={{display: 'flex', flexDirection: 'row'}}>
                                                <ButtonIcon icon="edit" color="#007bff" onClick={() => showEditModal(row)}/>
                                                <ButtonIcon icon="trash" color="#dc3545" onClick={() => showDeleteConfirmationModal(row)}/>
                                            </div>
                                        </td>
                                        {
                                            Object.keys(roles[0]).map(key => (
                                                <td key={'data_' + i + key} >{row[key] != null ? '' + row[key]: ''}</td>
                                            ))
                                        }
                                    </tr>
                                ))
                            }
                </ApiraiserTable>
                {/* <Table responsive bordered striped size="sm">
                        <thead>
                            <tr key={'header'}>
                                <th key={'header_#'} scope="col"></th>
                                {roles &&
                                    Object.keys(roles[0]).map(key => (
                                        <th key={'header_' + key} scope="col">{key}</th>
                                    ))
                                }
                            </tr>
                        </thead>
                        <tbody>
                            {roles &&
                                roles.map((row, i) => (
                                    <tr key={'row_' + (i + 1)}>

                                        <td key={'data_' + i + '_#'} scope="row">
                                            <div style={{display: 'flex', flexDirection: 'row'}}>
                                                <ButtonIcon icon="edit" color="#007bff" onClick={() => showEditModal(row)}/>
                                                <ButtonIcon icon="trash" color="#dc3545" onClick={() => showDeleteConfirmationModal(row)}/>
                                            </div>
                                        </td>
                                        {
                                            Object.keys(roles[0]).map(key => (
                                                <td key={'data_' + i + key} >{row[key] != null ? '' + row[key]: ''}</td>
                                            ))
                                        }
                                    </tr>
                                ))
                            }
                        </tbody>
                    </Table> */}
                    {isEditModalVisible &&
                        <CreateEditModal
                            columns={columns}
                            isSelectList={[]}
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
        {                
            isCreateRoleModalVisible && roles &&

            <CreateEditModal
                columns={columns}
                isSelectList={[]}
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

const connectedRoles = connect(mapStateToProps)(RolesPage);
export { connectedRoles as RolesPage };