import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import { connect } from 'react-redux';
import { withRouter } from "react-router-dom";
import { tablesActions } from '../../actions';
import { history } from '../../helpers';
import { Page } from '../../components';
import { AddTableColumnModal } from '../../modals';
import {tablesService} from '../../services';


const TableCreatePage = ({dispatch}) => {
    const [columns, setColumns] = React.useState([]);
    const [table, setTable] = React.useState(null);
    const [creatingTable, setCreatingTable] = React.useState(false);

    const createTable = () => {
        setCreatingTable(true);

        tablesService.createTable(table, columns).then( response => {
            if(response.Success){
                dispatch(tablesActions.getApplicationTables());
                console.log('Table Created!');
                history.push(`/table/design/${table}`);
            }
            else {
                setCreatingTable(false);
            }
        });
    }

    const handleTableNameChange = (e) => {
        const { name, value } = e.target;
        if(name === 'table') {
            setTable(value);
        }
    }

    const handleSubmit = (e) => {
        e.preventDefault();
        createTable();
        // dispatch(tablesActions.createTable(form["table"], columns));
    }
     
    return (
        <React.Fragment>            
        {
            (creatingTable)  &&
            <div className="row">
                <div className="col-sm-1 col-md-3"></div>
                <div align="center" className="col-sm-10 col-md-6 col-md-offset-3" style={{ "marginTop": "25vh" }}>
                    <p>Creating table. Please wait...
                        <span style={{ marginLeft: '16px' }}>
                            <img src="data:image/gif;base64,R0lGODlhEAAQAPIAAP///wAAAMLCwkJCQgAAAGJiYoKCgpKSkiH/C05FVFNDQVBFMi4wAwEAAAAh/hpDcmVhdGVkIHdpdGggYWpheGxvYWQuaW5mbwAh+QQJCgAAACwAAAAAEAAQAAADMwi63P4wyklrE2MIOggZnAdOmGYJRbExwroUmcG2LmDEwnHQLVsYOd2mBzkYDAdKa+dIAAAh+QQJCgAAACwAAAAAEAAQAAADNAi63P5OjCEgG4QMu7DmikRxQlFUYDEZIGBMRVsaqHwctXXf7WEYB4Ag1xjihkMZsiUkKhIAIfkECQoAAAAsAAAAABAAEAAAAzYIujIjK8pByJDMlFYvBoVjHA70GU7xSUJhmKtwHPAKzLO9HMaoKwJZ7Rf8AYPDDzKpZBqfvwQAIfkECQoAAAAsAAAAABAAEAAAAzMIumIlK8oyhpHsnFZfhYumCYUhDAQxRIdhHBGqRoKw0R8DYlJd8z0fMDgsGo/IpHI5TAAAIfkECQoAAAAsAAAAABAAEAAAAzIIunInK0rnZBTwGPNMgQwmdsNgXGJUlIWEuR5oWUIpz8pAEAMe6TwfwyYsGo/IpFKSAAAh+QQJCgAAACwAAAAAEAAQAAADMwi6IMKQORfjdOe82p4wGccc4CEuQradylesojEMBgsUc2G7sDX3lQGBMLAJibufbSlKAAAh+QQJCgAAACwAAAAAEAAQAAADMgi63P7wCRHZnFVdmgHu2nFwlWCI3WGc3TSWhUFGxTAUkGCbtgENBMJAEJsxgMLWzpEAACH5BAkKAAAALAAAAAAQABAAAAMyCLrc/jDKSatlQtScKdceCAjDII7HcQ4EMTCpyrCuUBjCYRgHVtqlAiB1YhiCnlsRkAAAOwAAAAAAAAAAAA==" />
                        </span>
                    </p>
                </div>
                <div className="col-sm-1 col-md-3"></div>
            </div>
        }
        {
            (!creatingTable)  &&
            <div text-align="center" className="col-sm-10 col-md-6 col-md-offset-3" style={{ "marginTop": "25vh", "marginLeft": "50vh"}}>
                <form name="form" className="row"  onSubmit={handleSubmit}>
                <div className={'form-group'}  >
                    <label htmlFor="table">Table name</label>
                    <input type="text" required={true} className="form-control" name="table" placeholder="Enter table name"  onChange= {handleTableNameChange}/>
                </div>
                <div className="col-md-12">
                        <button className="btn btn-primary">Create Table</button>
                    </div>
                </form>
                
            </div>
        }
        </React.Fragment>
    );
}

function mapStateToProps(state) {
    const { loggedIn } = state.authentication;
    const { tableslist, loadingTablesList, loadedTablesSuccess, createdTable, creatingTable, createdTableSuccess} = state.tables;
    return {
        loggedIn,
        tableslist, 
        loadingTablesList,
        loadedTablesSuccess,
        createdTable, 
        creatingTable, 
        createdTableSuccess
    };

}

const connectedCreateTable = connect(mapStateToProps)(TableCreatePage);
export { connectedCreateTable as TableCreatePage };
