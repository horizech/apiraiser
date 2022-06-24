import React, { Component } from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from './nav-menu.component';
import { Sidebar } from './sidebar.component';

import '../styles/Layout.css';

export const Layout = ({children}) => {
  // const displayName = Layout.name;
  const screenHeight = window.innerHeight;
  const navbarOffset = 76;

  return (
    <div>
      {/* <div className="layout"> */}
        {/* <Sidebar /> */}
        <div style={{width: "100%"}}>
          <NavMenu />
          <Container className='layout-container' style={{height: screenHeight - navbarOffset}}>
            {children}
          </Container>        
        </div>
      {/* </div> */}
    </div>
  );
}