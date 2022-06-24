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
     <div className={"configuration-button"} onClick={ () => history.push("/permissions")}>
        <Card body>
          <center>    
            <i className={"fas fa-user-lock fa-3x"}></i>
            <p>Permisssions</p>
          </center>
          <CardText></CardText>
        </Card>
      </div>
      <div className={"configuration-button"} onClick={ () => history.push("/permission-groups")}>
        <Card body>
          <center>    
            <i className={"fas fa-layer-group fa-3x"}></i>
            <p>Permisssion Groups</p>
          </center>
          <CardText></CardText>
        </Card>
      </div>
      <div className={"configuration-button"} onClick={ () => history.push("/permission-group-mappings")}>
        <Card body>
          <center>    
            <i className={"fas fa-users-cog fa-3x"}></i>
            <p>Permisssion Group Mappings</p>
          </center>
          <CardText></CardText>
        </Card>
      </div>
      <div className={"configuration-button"} onClick={ () => history.push("/role-permission-group-mappings")}>
        <Card body>
          <center>    
            <i className={"fas fa-sitemap fa-3x"}></i>
            <p>Role Permisssion Group Mappings</p>
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
