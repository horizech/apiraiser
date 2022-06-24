import React, { Component } from "react";
import {
  ProSidebar,
  SidebarHeader,
  Menu,
  MenuItem,
  SubMenu,
  SidebarContent,
  SidebarFooter,
} from "react-pro-sidebar";
import { Link } from 'react-router-dom';
import { connect } from "react-redux";
import { history } from '../helpers';
import { ButtonIcon } from './button-icon.component';
import "react-pro-sidebar/dist/css/styles.css";
import "../styles/Sidebar.css";
import { sidebarSubMenus } from "../constants";
// import { TablesApiProvider } from "../api_providers/tables.api";
import { tablesActions } from '../actions';

const Sidebar = ({loggedIn, tableslist, loadingTablesList, dispatch, user })=>{

 const displayName = Sidebar.name;
 
 const getParams = () => {
  let location = window.location.pathname.split("/");
  let result = {
    subMenuActive: null,
    menuItemActive: null
  };

  if(location.length > 1) {
    result.subMenuActive = location[1];
    if(location.length > 2) {
      result.menuItemActive = location[2];
    }
  }
  return result;
}  

 let params = getParams();
 const [subMenuActive, setSubMenuActive] = React.useState(params.subMenuActive);
 const [menuItemActive, setMenuItemActive] = React.useState(params.menuItemActive);
 const [toggled, setToggled] = React.useState();
 

 React.useEffect(()=>{
  if(loggedIn && tableslist === null) {
    getTables();
  }
 });
 
 const getTables= () => {
  dispatch(tablesActions.getTables());
}
const renderTables = (tables) => {
  return (
    <SubMenu onOpenChange={(open) => {setSubMenu(open, 'table')}} open={subMenuActive === 'table'} title="Tables">
      <Link to="/table/create" className="btn btn-link">Create New Table</Link>
      {tables&&
        tables.map((table) => (
        <MenuItem 
        active={subMenuActive === 'table' && menuItemActive === table ? true: null} 
         
        key={table}>
        <div style={{display: 'flex', flexDirection: 'row', justifyContent: 'space-between'}}>
          <span onClick={() => gotoTableDataPage(table)}>{table}</span> 
          <ButtonIcon className="ml-3" icon="table" color="#fffc" onClick={() => gotoTableDesignPage(table)}/>
        </div>
        </MenuItem>
      ))}  
    </SubMenu>
  );
}



const toggleSidebar = () =>  {

  setToggled(!toggled);

}

const setSubMenu = (isOpen, subMenu) => {
  if(!isOpen) {
    setSubMenuActive(subMenu);
  }
  else {
    if(subMenu === subMenuActive) {
      setSubMenuActive(null);
    }
    else {
      setSubMenuActive(subMenu);
    }
  }
}

const setMenuItem = (menuItem) =>  {
  setMenuItemActive(menuItem);
}

const gotoTableDataPage = (table) => {
  history.push(`/table/data/${table}`);    
}

const gotoTableDesignPage = (table) =>  {
  setMenuItem(table);
  history.push(`/table/design/${table}`);
}

const gotoPath = (menuPath) => {
  history.push(`${menuPath}`);    
}

const renderLoading = () =>  {
  return (
    <MenuItem>Loading...</MenuItem>
  );
}

// const renderMenu = () =>  {
//   return (
//     sidebarSubMenus.map((subMenu, index) => {
//       return (
//         <SubMenu key={index} onOpenChange={(open) => {setSubMenu(open, subMenu.subMenuPath)}} open={subMenuActive === subMenu.subMenuPath} title={subMenu.title}>
//           {
//             subMenu.menuItems && subMenu.menuItems.map((item, index) => <MenuItem key={index} onClick={() => gotoPath(item.menuItemPath)}>{item.title}</MenuItem>)
//           }
//         </SubMenu>
//       );
//     })
//   );
// }
let contents = (loadingTablesList === false && tableslist !== null)? renderTables(tableslist):renderLoading();

return (
  <React.Fragment>
    { 
      loggedIn &&
      <ProSidebar key={'prosidebar'} toggled={toggled}>
        <SidebarHeader>
          <div className="sidebar-header">Apiraiser</div>
        </SidebarHeader>
        <SidebarContent>
          <Menu iconShape="square">
            <MenuItem>Apiraiser</MenuItem>
            {/* {renderMenu()} */}
            <MenuItem onClick={() => gotoPath('/user')}>Current User</MenuItem>
            <MenuItem onClick={() => gotoPath('/users')}>Users</MenuItem>
            <MenuItem onClick={() => gotoPath('/settings')}>Settings</MenuItem>
            <MenuItem onClick={() => gotoPath('/permissions')}>Permissions</MenuItem>
            <MenuItem onClick={() => gotoPath('/roles')}>Roles</MenuItem>
            <MenuItem onClick={() => gotoPath('/permission-groups')}>Permission Groups</MenuItem>
            <MenuItem onClick={() => gotoPath('/permission-group-mappings')}>Permission Group Mappings</MenuItem>
            <MenuItem onClick={() => gotoPath('/role-permission-group-mappings')}>Role Permission Group Mappings</MenuItem>
            
            
            {contents}
            <SubMenu onOpenChange={(open) => {setSubMenu(open, 'Components')}} open={subMenuActive === 'Components'} title="Components">
              <MenuItem>Component 1</MenuItem>
              <MenuItem>Component 2</MenuItem>
            </SubMenu>
          </Menu>
        </SidebarContent>
        <SidebarFooter>
          <p>Copyright 2021 Horizech</p>
        </SidebarFooter>
      </ProSidebar>
    }
    
    </React.Fragment>
);

}

function mapStateToProps(state) {
  const { user, loggedIn } = state.authentication;
  const { tableslist, loadingTablesList} = state.tables

  return {
    user,
    loggedIn,
    tableslist, 
    loadingTablesList
  };
}

const connectedSidebar = connect(mapStateToProps)(Sidebar);
export { connectedSidebar as Sidebar };
