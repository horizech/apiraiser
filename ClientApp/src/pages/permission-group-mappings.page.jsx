import React from 'react';
import { Link } from 'react-router-dom';
import { connect } from 'react-redux';
import { tablesActions } from '../actions';
import { Table } from 'reactstrap';
import { ButtonIcon } from '../components/button-icon.component';
import { history } from '../helpers';
import { Loading, Page, PermissionGroups } from '../components';
import { CreateEditModal } from '../modals';
import { permissionGroupMappingsService, permissionGroupsService, permissionsService } from '../services';
import { DialogModal } from '../modals';
import { ApiraiserTable } from '../components';
import { alertActions, toastActions } from '../actions';

const PermissionGroupMappingsPage = ({ match, location, dispatch, loggedIn }) => {

    const [permissionGroupMappings, setPermissionGroupMappings] = React.useState(null);
    const [loadingPermissionGroupMappingColumns, setLoadingPermissionGroupMappingColumns] = React.useState(true);
    const [isCreatePermissionGroupMappingModalVisible, setCreatePermissionGroupMappingModalVisible] = React.useState(false);
    const [currentRow, setCurrentRow] = React.useState(null);
    const [isDeleteModalVisible, setDeleteModalVisible] = React.useState(false);
    const [isEditModalVisible, setEditModalVisible] = React.useState(false);
    const [deleteSuccess, setDeleteSuccess] = React.useState(false);
    const [addSuccess, setAddSuccess] = React.useState(false);
    const [updateSuccess, setUpdateSuccess] = React.useState(false);
    const [selectOptionsList, setSelectOptionsList] = React.useState({
        Permission: [],
        PermissionGroup: []
    });

    const columns = [
        { Name: 'Id', value: 'Id', Datatype: 'Integer', needParse: false, IsSelectList: false },
        { Name: 'Permission', value: 'Permission', Datatype: 'LongText', needParse: true, IsSelectList: true },
        { Name: 'PermissionGroup', value: 'PermissionGroup', Datatype: 'LongText', needParse: true, IsSelectList: true },
        { Name: 'IsSystem', value: 'IsSystem', Datatype: 'Boolean', needParse: false, IsSelectList: false },
        { Name: 'CreatedOn', value: 'CreatedOn', Datatype: 'DateTime', needParse: false, IsSelectList: false },
        { Name: 'CreatedBy', value: 'CreatedBy', Datatype: 'ShortText', needParse: false, IsSelectList: false },
        { Name: 'LastUpdatedOn', value: 'LastUpdatedOn', Datatype: 'DateTime', needParse: false, IsSelectList: false },
        { Name: 'LastUpdatedBy', value: 'LastUpdatedBy', Datatype: 'ShortText', needParse: false, IsSelectList: false }
    ]

    const isSelectList =  {
        Permission: true,
        PermissionGroup: true
    };
    
    const showCreateModal = () => {
        setCreatePermissionGroupMappingModalVisible(true);
    }

    const handleOnCreateComplete = (values) => {
        if (values) {
            // values.Permission = values.Permission.value;
            // values.PermissionGroup = values.PermissionGroup.value;
            permissionGroupMappingsService.addPermissionGroupMapping(values).then(response => {
                if (response.Success) {
                    setAddSuccess(true);
                    dispatch(toastActions.success(response.Message));
                }
                else {
                    setAddSuccess(false);
                    dispatch(alertActions.error("Error", response.Message));
                }
            });
        }
        setCreatePermissionGroupMappingModalVisible(false);
    }
    const showEditModal = (row) => {
        setEditModalVisible(true);
        if(row) {
            let editRow = JSON.parse(JSON.stringify(row));
            if(selectOptionsList['Permission'] && editRow['Permission']) {
                editRow['Permission'] = selectOptionsList['Permission'].filter(x => x.value == editRow['Permission'])[0];                             
            }
            if(selectOptionsList['PermissionGroup'] && editRow['PermissionGroup']) {
                editRow['PermissionGroup'] = selectOptionsList['PermissionGroup'].filter(x => x.value == editRow['PermissionGroup'])[0];                             
            }
            setCurrentRow(editRow);
        }
    }

    const showDeleteConfirmationModal = (row) => {
        setDeleteModalVisible(true);
        setCurrentRow(row);
    }
    
    const handleOnEditComplete = (values) => {
        if (values) {
            // values.Permission = values.Permission.value;
            // values.PermissionGroup = values.PermissionGroup.value;
            permissionGroupMappingsService.updatePermissionGroupMapping(values).then(response => {
                if (response.Success) {
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
        if (result === true) {
            permissionGroupMappingsService.deletePermissionGroupMapping(currentRow).then(response => {
                if (response.Success) {
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
    const loadSelectOptions = () => {
        let permissionGroupsPromise = permissionGroupsService.getPermissionGroups();
        let permissionsPromise = permissionsService.getPermissions();
        Promise.all([permissionGroupsPromise, permissionsPromise]).then( data => {
            let selectOptionsListUpdate = {};
            selectOptionsListUpdate['PermissionGroup'] = (
                data[0].Data.map(x => {
                    return { label: x.Name, value: x.Id }
                })
            );
            selectOptionsListUpdate['Permission'] = (
                data[1].Data.map(x => {
                    return { label: x.Name, value: x.Id }
                })
            );
            setSelectOptionsList(selectOptionsListUpdate);
        })        
        // const foreignSchema = (values.ForeignSchema.value);        
    }


    React.useEffect( () => {
        if (!selectOptionsList.PermissionGroup || !selectOptionsList.PermissionGroup.length || !selectOptionsList.Permission || !selectOptionsList.Permission.length) {
            loadSelectOptions();
        }
    }, []);
    React.useEffect(() => {

    
        permissionGroupMappingsService.getPermissionGroupMappings().then(response => {
            if (response.Success) {
                setPermissionGroupMappings(response.Data);
            }
            else {
                setPermissionGroupMappings(null);
            }
        })
        setLoadingPermissionGroupMappingColumns(false);
        if (!loggedIn) {
            history.push('/');
        }
        setAddSuccess(false);
        setDeleteSuccess(false);
        setUpdateSuccess(false);
    }, [match, loggedIn, deleteSuccess, addSuccess, updateSuccess, selectOptionsList]);
    return (
        <React.Fragment>
            <div align="right" style={{ marginBottom: "16px" }}>
                <button className="btn btn-primary" onClick={showCreateModal}>Create a new permissionGroupMapping</button>
            </div>
            {
                (loadingPermissionGroupMappingColumns) &&
                <Loading></Loading>
            }
            {
                (!loadingPermissionGroupMappingColumns && (!permissionGroupMappings || permissionGroupMappings["length"] == 0)) &&
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
                (!loadingPermissionGroupMappingColumns && permissionGroupMappings && permissionGroupMappings["length"] != 0) &&
                <div>
                    <ApiraiserTable headers={Object.keys(permissionGroupMappings[0])}>
                    {permissionGroupMappings &&
                                permissionGroupMappings.map((row, i) => (
                                    <tr key={'row_' + (i + 1)}>

                                        <td key={'data_' + i + '_#'} scope="row">
                                            <div style={{ display: 'flex', flexDirection: 'row' }}>
                                                <ButtonIcon icon="edit" color="#007bff" onClick={() => showEditModal(row)} />
                                                <ButtonIcon icon="trash" color="#dc3545" onClick={() => showDeleteConfirmationModal(row)} />
                                            </div>
                                        </td>
                                        {
                                            Object.keys(permissionGroupMappings[0]).map(key => (
                                                <td key={'data_' + i + key} >{row[key] != null ? '' + row[key] : ''}</td>
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
                                {permissionGroupMappings &&
                                    Object.keys(permissionGroupMappings[0]).map(key => (
                                        <th key={'header_' + key} scope="col">{key}</th>
                                    ))
                                }
                            </tr>
                        </thead>
                        <tbody>
                            {permissionGroupMappings &&
                                permissionGroupMappings.map((row, i) => (
                                    <tr key={'row_' + (i + 1)}>

                                        <td key={'data_' + i + '_#'} scope="row">
                                            <div style={{ display: 'flex', flexDirection: 'row' }}>
                                                <ButtonIcon icon="edit" color="#007bff" onClick={() => showEditModal(row)} />
                                                <ButtonIcon icon="trash" color="#dc3545" onClick={() => showDeleteConfirmationModal(row)} />
                                            </div>
                                        </td>
                                        {
                                            Object.keys(permissionGroupMappings[0]).map(key => (
                                                <td key={'data_' + i + key} >{row[key] != null ? '' + row[key] : ''}</td>
                                            ))
                                        }
                                    </tr>
                                ))
                            }
                        </tbody>
                    </Table> */}
                    {isEditModalVisible &&
                        <CreateEditModal
                            isSelectList={isSelectList}
                            selectOptions={selectOptionsList}
                            columns={columns}
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
                isCreatePermissionGroupMappingModalVisible && permissionGroupMappings &&

                <CreateEditModal
                    isSelectList={isSelectList}
                    selectOptions={selectOptionsList}
                    columns={columns}
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

const connectedPermissionGroupMappings = connect(mapStateToProps)(PermissionGroupMappingsPage);
export { connectedPermissionGroupMappings as PermissionGroupMappingsPage };
