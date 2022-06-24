import React from 'react';
import { Link } from 'react-router-dom';
import { connect } from 'react-redux';
import { withRouter } from "react-router-dom";

import { userActions } from '../actions';
import { history } from '../helpers';
import { Page } from '../components';

class LogoutPage extends Page {
    constructor(props) {
        super(props);
    }

    componentDidMount() {
        this.props.dispatch(userActions.logout());
        history.push("/");
    }

    render() {
        return (
            <p>Logging out...</p>
        );
    }
}

function mapStateToProps(state) {
    const { loggingIn, loggedIn } = state.authentication;
    return {
        loggedIn,
        loggingIn
    };
}

const connectedLogout = connect(mapStateToProps)(LogoutPage);
export { connectedLogout as LogoutPage };
