import React from 'react';
import { connect } from 'react-redux';
import { Card, Button, CardTitle, CardText, Row, Col,CardImg } from 'reactstrap';
import { history } from '../helpers';

import '../styles/ConfigurationPage.scss';

const ConfigurationPage = (props) => {
  return (
    <div className={'configuration-page'}>
      <div className={"configuration-button"} onClick={ () => history.push("/roles")}>
        <Card body>
          <center>    
            <i className={"fas fa-user-tag fa-3x"}></i>
            <p>Roles</p>
          </center>
          <CardText></CardText>
        </Card>
      </div>
      <div className={"configuration-button"} onClick={ () => history.push("/user-access-levels")}>
        <Card body>
          <center>    
            <i className={"fas fa-user-alt-slash fa-3x"}></i>
            <p>User Access Levels</p>
          </center>
          <CardText></CardText>
        </Card>
      </div>
    </div>
  );
};                        

function mapStateToProps(state) {
  const { loggedIn } = state.authentication;
  return {
      loggedIn
  };
}
const connectedHome = connect(mapStateToProps)(ConfigurationPage);
 export { connectedHome as ConfigurationPage }; 
