import React from 'react';
import { Router, Route } from 'react-router-dom';
import { connect } from 'react-redux';

import { history } from './helpers';
import { alertActions } from './actions';
import { toastActions } from './actions';
// import { PrivateRoute } from './components';

import { StartPage, TableDesignPage, TableDataPage, PageCreatorPage, HomePage, LoginPage, InitializePage, CounterPage, AdminPage, FetchDataPage, LogoutPage, TableCreatePage, designTablePage, SettingsPage, PermissionsPage, RolesPage, RolePermissionsPage, UsersPage, UserPage, PermissionGroupsPage, PermissionGroupMappingsPage, RolePermissionGroupMappingsPage, RegisterPage } from './pages';

import { Layout, UserGuardedRoute } from './components';
import toast, { Toaster } from 'react-hot-toast';

import Swal from 'sweetalert2'
import withReactContent from 'sweetalert2-react-content'

import './styles/App.scss';
import { ConfigurationPage } from './pages/configuration.page';
import { UserAccessLevelsPage } from './pages/user-access-levels.page';


const MySwal = withReactContent(Swal)

const showToast = (type, message) => {
    if(type === 'error') {
        toast.error(message);
    }
    else {
        toast.success(message);
    }
}

class App extends React.Component {
    constructor(props) {
        super(props);

        const { dispatch } = this.props;
        history.listen((location, action) => {
            // clear alert on location change
            // dispatch(alertActions.clear());
        });
    }

    render() {
        const { alert, toast, dispatch } = this.props;
        if(toast && toast.message) {
            dispatch(toastActions.clear());
            showToast(toast.type, toast.message);
        }

        if(alert && alert.message) {
            dispatch(alertActions.clear());
            MySwal.fire({
                title: <p>{alert.message}</p>,
                // footer: 'Copyright 2018',
                didOpen: () => {
                  // `MySwal` is a subclass of `Swal`
                  //   with all the same instance & static methods
                  MySwal.clickConfirm()
                }
              }).then(() => {
                return MySwal.fire({
                    title: <strong>{alert.title || ""}</strong>,
                    html: <i>{alert.message}</i>,
                    icon: alert.type || 'success'
                })
              })
        }

        return (
            <div>
                <Toaster />

                {/* <div className="container alert-container">
                    <div className="col-sm-8 col-sm-offset-2">
                        {alert.message &&
                            <div className={`alert ${alert.type}`}>{alert.message}</div>
                        }
                    </div>
                </div> */}
                <Router history={history}>
                    <Layout>
                        <UserGuardedRoute exact path='/' component={HomePage}/>
                        <UserGuardedRoute path='/home' component={HomePage}/>
                        <Route path='/start' component={StartPage} />
                        <Route path='/login' component={LoginPage} />
                        <Route path='/register' component={RegisterPage} />
                        <Route path='/initialize' component={InitializePage} />
                        
                        <UserGuardedRoute path='/logout' component={LogoutPage} />
                        <UserGuardedRoute path='/counter' component={CounterPage} />
                        <UserGuardedRoute path='/admin' component={AdminPage} />
                        <UserGuardedRoute path='/table/create' component={TableCreatePage} />
                        <UserGuardedRoute path='/table/data/:table_name' component={TableDataPage} />
                        <UserGuardedRoute path='/table/design/:table_name' component={TableDesignPage} />
                        <UserGuardedRoute path='/page/create' component={PageCreatorPage} />
                        <UserGuardedRoute path='/fetch-data' component={FetchDataPage} />
                        <UserGuardedRoute path='/design-table/:table_name' component={designTablePage} />
                        <UserGuardedRoute path='/settings' component={SettingsPage} />
                        <UserGuardedRoute path='/permissions' component={PermissionsPage} />
                        <UserGuardedRoute path='/roles' component={RolesPage} />
                        <UserGuardedRoute path='/role-permissions' component={RolePermissionsPage} />
                        <UserGuardedRoute path='/users' component={UsersPage} />
                        <UserGuardedRoute path='/user' component={UserPage} />
                        <UserGuardedRoute path='/permission-groups' component={PermissionGroupsPage} />
                        <UserGuardedRoute path='/role-permission-group-mappings' component={RolePermissionGroupMappingsPage} />
                        <UserGuardedRoute path='/permission-group-mappings' component={PermissionGroupMappingsPage} />
                        <UserGuardedRoute path='/user-access-levels' component={UserAccessLevelsPage} />
                        <UserGuardedRoute path='/configuration' component={ConfigurationPage} />
                        
                        
                    </Layout>
                </Router>
            </div>
        );
    }
}

function mapStateToProps(state) {
    const { alert, toast } = state;
    return {
        alert, toast
    };
}

const connectedApp = connect(mapStateToProps)(App);
export { connectedApp as App }; 