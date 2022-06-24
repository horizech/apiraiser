import React, { Component } from 'react';
import { Container, Card, CardBody, CardHeader, CardTitle, CardSubtitle, CardText } from 'reactstrap';
import { NavMenu } from './nav-menu.component';
import { Sidebar } from './sidebar.component';

import '../styles/Collapsible.css';

export const Collapsible = ({title, subTitle, icon, defaultOpen, style, children}) => {
  
  const [isOpen, setOpen] = React.useState(defaultOpen || false);

  const toggle = () => {
    setOpen(!isOpen);
  }

  return (
    <React.Fragment>
      <Card style={style} className="collapsible-card border-0">
        <CardHeader className={'collapsible-header' + (isOpen? ' collapsible-header-open': '')} onClick={toggle}>
          <div style={{display: 'flex', alignItems: 'center'}}>
            {
              icon &&
            <i style={{marginRight: '16px'}} className={"fas fa-" + icon}>
            </i>
            }
            <div style={{flex: '1'}}>
              <CardTitle>{title}</CardTitle>
              <CardSubtitle className="mb-2 text-muted">{subTitle}</CardSubtitle>        
            </div>
            <div style={{marginLeft: '16px', display: 'inline-block'}}>
            <span style={{display: "inline-block"}}>
              <i style={{marginRight: '16px'}} className={"collapsible-arrow fas fa-chevron-right" + (isOpen? ' collapsible-arrow-open': '')} ></i>
            </span>
            </div>
          </div>
        </CardHeader>        
            <CardText className={'collapsible-body' + (isOpen? ' collapsible-body-open': '')}>
            {              
              children
            }
          </CardText>
        </Card>      
    </React.Fragment>
  );
}
