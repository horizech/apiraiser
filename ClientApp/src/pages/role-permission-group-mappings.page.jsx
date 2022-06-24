import React from 'react';
import { connect } from 'react-redux';
import { ButtonIcon } from '../components/button-icon.component';
import { history } from '../helpers';
import { Loading } from '../components';
import { CreateEditModal } from '../modals';
import { rolePermissionGroupMappingsService, permissionGroupsService, rolesService, userAccessLevelsService } from '../services';
import { DialogModal } from '../modals';
import { ApiraiserTable } from '../components';
import { alertActions, toastActions } from '../actions';

const RolePermissionGroupMappingsPage = ({ dispatch,match, loggedIn }) => {

    const [rolePermissionGroupMappings, setRolePermissionGroupMappings] = React.useState(null);
    const [loadingRolePermissionGroupMappingColumns, setLoadingRolePermissionGroupMappingColumns] = React.useState(true);
    const [isCreateRolePermissionGroupMappingModalVisible, setCreateRolePermissionGroupMappingModalVisible] = React.useState(false);
    const [currentRow, setCurrentRow] = React.useState(null);
    const [isDeleteModalVisible, setDeleteModalVisible] = React.useState(false);
    const [isEditModalVisible, setEditModalVisible] = React.useState(false);
    const [deleteSuccess, setDeleteSuccess] = React.useState(false);
    const [addSuccess, setAddSuccess] = React.useState(false);
    const [updateSuccess, setUpdateSuccess] = React.useState(false);
    const [selectOptionsList, setSelectOptionsList] = React.useState({
        Role: [],
        PermissionGroup: [],
        UserAccessLevel: []
    });

    const columns = [
        { Name: 'Id', value: 'Id', Datatype: 'Integer', needParse: false, IsSelectList: false},
        { Name: 'Role', value: 'Role', Datatype: 'Integer', needParse: true, IsSelectList: true  },
        { Name: 'PermissionGroup', value: 'PermissionGroup', Datatype: 'Integer', needParse: true, IsSelectList: true },
        { Name: 'UserAccessLevel', value: 'UserAccessLevel', Datatype: 'Integer', needParse: true, IsSelectList: true  },
        { Name: 'IsSystem', value: 'IsSystem', Datatype: 'Boolean', needParse: false, IsSelectList: false  },
        { Name: 'CreatedOn', value: 'CreatedOn', Datatype: 'DateTime', needParse: false, IsSelectList: false },
        { Name: 'CreatedBy', value: 'CreatedBy', Datatype: 'ShortText', needParse: false, IsSelectList: false },
        { Name: 'LastUpdatedOn', value: 'LastUpdatedOn', Datatype: 'DateTime', needParse: false, IsSelectList: false },
        { Name: 'LastUpdatedBy', value: 'LastUpdatedBy', Datatype: 'ShortText', needParse: false, IsSelectList: false }
    ]

    const isSelectList =  {
        Role: true,
        PermissionGroup: true,
        UserAccessLevel: true
    };
    
    const showCreateModal = () => {
        setCreateRolePermissionGroupMappingModalVisible(true);
    }

    const handleOnCreateComplete = (values) => {
        if (values) {
            console.log(values);
            // values.Role = values.Role.value;
            // values.PermissionGroup = values.PermissionGroup.value;
            rolePermissionGroupMappingsService.addRolePermissionGroupMapping(values).then(response => {
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
        setCreateRolePermissionGroupMappingModalVisible(false);
    }
    const showEditModal = (row) => {
        setEditModalVisible(true);
        if(row) {
            let editRow = JSON.parse(JSON.stringify(row));
            if(selectOptionsList['Role'] && editRow['Role']) {
                editRow['Role'] = selectOptionsList['Role'].filter(x => x.value == editRow['Role'])[0];                             
            }
            if(selectOptionsList['PermissionGroup'] && editRow['PermissionGroup']) {
                editRow['PermissionGroup'] = selectOptionsList['PermissionGroup'].filter(x => x.value == editRow['PermissionGroup'])[0];                             
            }
            if(selectOptionsList['UserAccessLevel'] && editRow['UserAccessLevel']) {
                editRow['UserAccessLevel'] = selectOptionsList['UserAccessLevel'].filter(x => x.value == editRow['UserAccessLevel'])[0];                             
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
            // values.Role = values.Role.value;
            // values.PermissionGroup = values.PermissionGroup.value;
            rolePermissionGroupMappingsService.updateRolePermissionGroupMapping(values).then(response => {
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
            rolePermissionGroupMappingsService.deleteRolePermissionGroupMapping(currentRow).then(response => {
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
        let rolesPromise = rolesService.getRoles();
        let userAccessLevelPromise = userAccessLevelsService.getUserAccessLevels();
        Promise.all([permissionGroupsPromise, userAccessLevelPromise, rolesPromise]).then( data => {
            let selectOptionsListUpdate = {};
            selectOptionsListUpdate['Role'] = (
                data[2].Data.map(x => {
                    return { label: x.Name, value: x.Id }
                })
            );
            selectOptionsListUpdate['PermissionGroup'] = (
                data[0].Data.map(x => {
                    return { label: x.Name, value: x.Id }
                })
            );
            selectOptionsListUpdate['UserAccessLevel'] = (
                data[1].Data.map(x => {
                    return { label: x.Name, value: x.Id }
                })
            );
            setSelectOptionsList(selectOptionsListUpdate);
        })        
        // const foreignSchema = (values.ForeignSchema.value);        
    }


    React.useEffect( () => {
        if (!selectOptionsList.PermissionGroup || !selectOptionsList.PermissionGroup.length || !selectOptionsList.Role || !selectOptionsList.Role.length || !selectOptionsList.UserAccessLevel || !selectOptionsList.UserAccessLevel.length) {
            loadSelectOptions();
        }
    }, []);

    React.useEffect(() => {

        rolePermissionGroupMappingsService.getRolePermissionGroupMappings().then(response => {
            if (response.Success) {
                setRolePermissionGroupMappings(response.Data);
            }
            else {
                setRolePermissionGroupMappings(null);
            }
        })
        setLoadingRolePermissionGroupMappingColumns(false);
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
                <button className="btn btn-primary" onClick={showCreateModal}>Create a new rolePermissionGroupMapping</button>
            </div>
            {
                (loadingRolePermissionGroupMappingColumns) &&
                <Loading></Loading>
            }
            {
                (!loadingRolePermissionGroupMappingColumns && (!rolePermissionGroupMappings || rolePermissionGroupMappings["length"] == 0)) &&
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
                (!loadingRolePermissionGroupMappingColumns && rolePermissionGroupMappings && rolePermissionGroupMappings["length"] != 0) &&
                <div>
                    <ApiraiserTable headers={Object.keys(rolePermissionGroupMappings[0])}>
                    {rolePermissionGroupMappings &&
                                rolePermissionGroupMappings.map((row, i) => (
                                    <tr key={'row_' + (i + 1)}>

                                        <td key={'data_' + i + '_#'} scope="row">
                                            <div style={{ display: 'flex', flexDirection: 'row' }}>
                                                <ButtonIcon icon="edit" color="#007bff" onClick={() => showEditModal(row)} />
                                                <ButtonIcon icon="trash" color="#dc3545" onClick={() => showDeleteConfirmationModal(row)} />
                                            </div>
                                        </td>
                                        {
                                            Object.keys(rolePermissionGroupMappings[0]).map(key => (
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
                                {rolePermissionGroupMappings &&
                                    Object.keys(rolePermissionGroupMappings[0]).map(key => (
                                        <th key={'header_' + key} scope="col">{key}</th>
                                    ))
                                }
                            </tr>
                        </thead>
                        <tbody>
                            {rolePermissionGroupMappings &&
                                rolePermissionGroupMappings.map((row, i) => (
                                    <tr key={'row_' + (i + 1)}>

                                        <td key={'data_' + i + '_#'} scope="row">
                                            <div style={{ display: 'flex', flexDirection: 'row' }}>
                                                <ButtonIcon icon="edit" color="#007bff" onClick={() => showEditModal(row)} />
                                                <ButtonIcon icon="trash" color="#dc3545" onClick={() => showDeleteConfirmationModal(row)} />
                                            </div>
                                        </td>
                                        {
                                            Object.keys(rolePermissionGroupMappings[0]).map(key => (
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
                isCreateRolePermissionGroupMappingModalVisible && rolePermissionGroupMappings &&

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

const connectedRolePermissionGroupMappings = connect(mapStateToProps)(RolePermissionGroupMappingsPage);
export { connectedRolePermissionGroupMappings as RolePermissionGroupMappingsPage };
