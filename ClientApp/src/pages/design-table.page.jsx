import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import { connect } from 'react-redux';
import { withRouter } from "react-router-dom";
import { tablesActions } from '../actions';
import { history } from '../helpers';
import { Page } from '../components';
import { AddTableColumnModal } from '../modals';


class designTablePage extends Page {
    constructor(props) {
        super(props);
         this.handleClick = this.handleClick.bind(this);
        // this.handleChange = this.handleChange.bind(this);
        // this.handleSubmit = this.handleSubmit.bind(this);
        // this.showCreateTableModal = this.showCreateTableModal.bind(this);
        this.handleCreateTableModalToggle = this.handleCreateTableModalToggle.bind(this);

        this.state = {
            // isCreateTableModalVisible: false,
            
            addTableColumn: false,
        }
    }
    handleColumnSubmit(columnProperties) {
        // e.preventDefault();
        // this.setState({ submitted: true });
        // const { dispatch } = this.props;
        console.log(columnProperties);

        // this.setState({addTableColumn: true})
    }
    handleCreateTableModalToggle(isVisible){
        this.setState({addTableColumn: isVisible})
    }
    handleClick(){
        this.setState({addTableColumn: true});
    }

        render() {

            return (
                <div>
                
                <button onClick = {this.handleClick}>Add Column</button>
                {
                    this.state.addTableColumn &&

                        <AddTableColumnModal headerLabel="Create Column" handleModalToggle={this.handleCreateTableModalToggle} data={this.state.data} handleSubmit={this.handleColumnSubmit} handleChange={this.handleColumnChange} />

                }
                </div>
            );
        }
    }
// export  {designTable};
function mapStateToProps(state) {
    const { loggedIn } = state.authentication;
    const { loadingCurrentTable, currentTableColumns } = state.currentTableInfo;
    return {
        loadingCurrentTable,
        currentTableColumns,
        loggedIn,
    };

}

const connectedDesignTable = connect(mapStateToProps)(designTablePage);
export { connectedDesignTable as designTablePage };