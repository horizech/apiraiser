import React from 'react';
import { Link } from 'react-router-dom';
import { connect } from 'react-redux';
import { tablesActions } from '../actions';
import { Table } from 'reactstrap';
import { ButtonIcon } from '../components/button-icon.component';
import { history } from '../helpers';
import { Loading, Page, Permissions } from '../components';
import { CreateEditModal } from '../modals';
import { permissionsService } from '../services';
import { DialogModal } from '../modals'
import { ApiraiserTable } from '../components';
import { alertActions, toastActions } from '../actions';

const PermissionsPage = ({ match, location, dispatch, loggedIn }) => {

    const [permissions, setPermissions] = React.useState(null);
    const [loadingPermissionColumns, setLoadingPermissionColumns] = React.useState(true);
    const [isCreatePermissionModalVisible, setCreatePermissionModalVisible] = React.useState(false);
    const [currentRow, setCurrentRow] = React.useState(null);
    const [isDeleteModalVisible, setDeleteModalVisible] = React.useState(false);
    const [isEditModalVisible, setEditModalVisible] = React.useState(false);
    const [deleteSuccess, setDeleteSuccess] = React.useState(false);
    const [addSuccess, setAddSuccess] = React.useState(false);
    const [updateSuccess, setUpdateSuccess] = React.useState(false);

    const columns = [
        { Name: 'Id', value: 'Id' },
        { Name: 'Name', value: 'Name' },
        { Name: 'Description', value: 'Description' },
        { Name: 'CreatedOn', value: 'CreatedOn' },
        { Name: 'CreatedBy', value: 'CreatedBy' },
        { Name: 'LastUpdatedOn', value: 'LastUpdatedOn' },
        { Name: 'LastUpdatedBy', value: 'LastUpdatedBy' }
    ]

    const showCreateModal = () => {
        setCreatePermissionModalVisible(true);
    }

    const handleOnCreateComplete = (values) => {
        if (values) {
            permissionsService.addPermissions(values).then(response => {
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
        setCreatePermissionModalVisible(false);
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
        if (values) {
            permissionsService.updatePermissions(currentRow.Name, values).then(response => {
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
            permissionsService.deletePermissions(currentRow).then(response => {
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

    React.useEffect(() => {

        permissionsService.getPermissions().then(response => {
            if (response.Success) {
                setPermissions(response.Data);
            }
            else {
                setPermissions(null);
            }
        })
        setLoadingPermissionColumns(false);
        if (!loggedIn) {
            history.push('/');
        }
        setAddSuccess(false);
        setDeleteSuccess(false);
        setUpdateSuccess(false);
    }, [match, loggedIn, deleteSuccess, addSuccess, updateSuccess]);

    return (
        <React.Fragment>
            <div align="right" style={{ marginBottom: "16px" }}>
                <button className="btn btn-primary" onClick={showCreateModal}>Create a new permission</button>
            </div>
            {
                (loadingPermissionColumns) &&
                <Loading></Loading>
            }
            {
                (!loadingPermissionColumns && (!permissions || permissions["length"] == 0)) &&
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
                (!loadingPermissionColumns && permissions && permissions["length"] != 0) &&
                <div>
                    <ApiraiserTable headers={Object.keys(permissions[0])}>
                    {permissions &&
                                permissions.map((row, i) => (
                                    <tr key={'row_' + (i + 1)}>

                                        <td key={'data_' + i + '_#'} scope="row">
                                            <div style={{ display: 'flex', flexDirection: 'row' }}>
                                                <ButtonIcon icon="edit" color="#007bff" onClick={() => showEditModal(row)} />
                                                <ButtonIcon icon="trash" color="#dc3545" onClick={() => showDeleteConfirmationModal(row)} />
                                            </div>
                                        </td>
                                        {
                                            Object.keys(permissions[0]).map(key => (
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
                                {permissions &&
                                    Object.keys(permissions[0]).map(key => (
                                        <th key={'header_' + key} scope="col">{key}</th>
                                    ))
                                }
                            </tr>
                        </thead>
                        <tbody>
                            {permissions &&
                                permissions.map((row, i) => (
                                    <tr key={'row_' + (i + 1)}>

                                        <td key={'data_' + i + '_#'} scope="row">
                                            <div style={{ display: 'flex', flexDirection: 'row' }}>
                                                <ButtonIcon icon="edit" color="#007bff" onClick={() => showEditModal(row)} />
                                                <ButtonIcon icon="trash" color="#dc3545" onClick={() => showDeleteConfirmationModal(row)} />
                                            </div>
                                        </td>
                                        {
                                            Object.keys(permissions[0]).map(key => (
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
                            isSelectList={[]}
                            selectOptionsList={[]}
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
                isCreatePermissionModalVisible && permissions &&

                <CreateEditModal
                    isSelectList={[]}
                    selectOptions={[]}
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

const connectedPermissions = connect(mapStateToProps)(PermissionsPage);
export { connectedPermissions as PermissionsPage };