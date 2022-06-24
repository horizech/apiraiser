/*
import React, { Component } from 'react';
import { Router, Route } from 'react-router-dom';
import { Login } from './Login';

export function UserGuardedRoute(loggedIn) {
  return function({ component: Component, path: Path, exact: IsExact }) {
    alert(`${IsExact},${Path}`);

    if(IsExact){
      return (
        <Route exact path={Path}
          render={props => (loggedIn ? <Component /> : <Login />)}
        />
      );
    }
    else{
      return (
        <Route path={Path}
          render={props => (loggedIn ? <Component /> : <Login />)}
        />
      );
    }
    
  };
}
*/









import React from 'react';
import { Router, Route } from 'react-router-dom';
import { connect } from 'react-redux';

import { StartPage } from '../pages';

class UserGuardedRoute extends React.Component {
    constructor(props) {
        super(props);
        // alert(JSON.stringify(props));
    }

    render() {
        // <Route path={this.props.path}
        //     render={props => (loggedIn ? <Component /> : <Login />)}
        // />
        if(this.props.loggedIn){
            if(this.props.exact){
                return <Route exact path={this.props.path} component={this.props.component}/>
            }
            else{
                return <Route path={this.props.path} component={this.props.component}/>
            }
        }
        else{
            if(this.props.exact){
                return <Route exact path={this.props.path} component={StartPage}/>
            }
            else{
                return <Route path={this.props.path} component={StartPage}/>
            }
        }
    }
}

function mapStateToProps(state) {
    const { loggedIn } = state.authentication;
    return {
        loggedIn
    };
}

const connectedUserGuardedRoute = connect(mapStateToProps)(UserGuardedRoute);
export { connectedUserGuardedRoute as UserGuardedRoute }; 