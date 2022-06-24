import React from 'react';
import { Link } from 'react-router-dom';
import { connect } from 'react-redux';
import { tablesActions } from '../actions';
import { Table } from 'reactstrap';
import { ButtonIcon } from '../components/button-icon.component';
import { history } from '../helpers';
import { Loading, Page, User } from '../components';
import { CreateEditModal } from '../modals';
import { userService, rolesService} from '../services';
import { DialogModal } from '../modals'
import { ApiraiserTable } from '../components';
import { alertActions, toastActions } from '../actions';

const UsersPage = ({ match, location, dispatch, user, loggedIn }) => {
    // console.log(user.Role);
    // let currentUser= user;
    // if(currentUser.Role.Name || currentUser.Role.Id){
    //     currentUser.Role={ label: currentUser.Role.Name, value: currentUser.Role.Id };
    // }
    
    const [loadingSettingColumns, setLoadingSettingColumns] = React.useState(true);
    const [isCreateSettingModalVisible, setCreateSettingModalVisible] = React.useState(false);
    const [currentRow, setCurrentRow] = React.useState(null);
    const [isDeleteModalVisible, setDeleteModalVisible] = React.useState(false);
    const [isEditModalVisible, setEditModalVisible] = React.useState(false);
    const [deleteSuccess, setDeleteSuccess] = React.useState(false);
    const [addSuccess, setAddSuccess] = React.useState(false);
    const [updateSuccess, setUpdateSuccess] = React.useState(false);
    const [users, setUsers] = React.useState([]);
    const [selectOptionsList, setSelectOptionsList] = React.useState({
        Role: []
    });
    
    const isSelectList = {
        Role: true
    };
    
    const columns = [
        { Name: 'Id', value: 'Id', hidden: false, needParse: false, IsSelectList: false},
        { Name: 'Username', value: 'Username', hidden: false, needParse: false, IsSelectList: false},
        { Name: 'Email', value: 'Email', hidden: false, needParse: false, IsSelectList: false},
        { Name: 'Fullname', value: 'Fullname', hidden: false, needParse: false, IsSelectList: false},
        { Name: 'Password', value: 'Password', hidden: true, needParse: false, IsSelectList: false },
        { Name: 'CreatedOn', value: 'CreatedOn', hidden: false, needParse: false, IsSelectList: false },
        { Name: 'LastUpdatedOn', value: 'LastUpdatedOn', hidden: false, needParse: false, IsSelectList: false },
        { Name: 'Role', value: 'Role', hidden: false, needParse: true, IsSelectList: true },
    ]

    // const columnNames = columns.map(x => x.Name);
    let filteredHiddenColumns = columns.filter((item) => {
      return !item.hidden;
    });
    const displayedColumns = filteredHiddenColumns.map(x => x.Name);
    const showCreateModal = () => {
        setCreateSettingModalVisible(true);
    }
    const handleOnCreateComplete = (values) => {
        if (values) {
            // values.Role= parseInt(values.Role.value);
            userService.addUser(values).then(response => {
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
        setCreateSettingModalVisible(false);
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
            userService.updateUser(currentRow.Id, values).then(response => {
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
            userService.deleteUser(currentRow.Id).then(response => {
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
        let rolesPromise = rolesService.getRoles();
        Promise.all([rolesPromise]).then( data => {
            let selectOptionsListUpdate = {};
            selectOptionsListUpdate['Role'] = (
                data[0].Data.map(x => {
                    return { label: x.Name, value: x.Id }
                })
            );
            setSelectOptionsList(selectOptionsListUpdate);
        });
    }
    React.useEffect(()=>{
        if(!selectOptionsList.Role || !selectOptionsList.Role.length ){
            loadSelectOptions();
        }
    });
    React.useEffect(() => {

        userService.getAllUsers().then( response => {
            if(response && response.Success) {
                setUsers(response.Data);        
            }
            else {
                setUsers(null);
            }
        })       
        // setUsers(null);
        setLoadingSettingColumns(false);
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
                <button className="btn btn-primary" onClick={showCreateModal}>Create a new User</button>
            </div>
            {
                (loadingSettingColumns) &&
                <Loading></Loading>
            }
            {
                (!loadingSettingColumns && (!users || users.length == 0)) &&
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
                (!loadingSettingColumns && users && users.length > 0) &&
                <div>
                    <ApiraiserTable headers={displayedColumns}>
                        {users &&
                            users.map((row, i) => (
                                <tr key={'row_' + (i + 1)}>

                                    <td key={'data_' + i + '_#'} scope="row">
                                        <div style={{ display: 'flex', flexDirection: 'row' }}>
                                            <ButtonIcon icon="edit" color="#007bff" onClick={() => showEditModal(row)} />
                                            <ButtonIcon icon="trash" color="#dc3545" onClick={() => showDeleteConfirmationModal(row)} />
                                        </div>
                                    </td>
                                    {
                                        displayedColumns.map(key => {
                                            let tableData
                                            if(!key.hidden){

                                            }
                                            if (key == "Role") {
                                                tableData = row[key] != null ? '' + row[key]['label'] : ''
                                            }
                                            else {
                                                tableData = row[key] != null ? '' + row[key] : ''
                                            }
                                            return (
                                                <td key={'data_' + i + key} >{tableData}</td>
                                            )
                                        })
                                    }
                                </tr>
                            ))
                        }
                    </ApiraiserTable>
                    {isEditModalVisible &&
                        <CreateEditModal
                            isSelectList={isSelectList}
                            selectOptions={selectOptionsList}
                            columns={filteredHiddenColumns}
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
                isCreateSettingModalVisible &&
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
    const { loggedIn, user } = state.authentication;
    return {
        loggedIn,
        user
    };
}

const connectedUser = connect(mapStateToProps)(UsersPage);
export { connectedUser as UsersPage };