import React, { Component } from "react";
import { connect } from "react-redux";
import { tablesActions } from "../actions";
import { CreateTableColumnModal, AddTableColumnModal } from "../modals";
import { DialogModal } from "../modals";
import { ButtonIcon } from "./button-icon.component";
import { Table } from "reactstrap";
import { isColumnPredefined } from "../helpers";
import { ApiraiserTable } from ".";
import { Schemas } from "../constants";

const TableColumns = ({
    dispatch,
    deletedColumnSuccess,
    deletingColumn,
    addedColumnSuccess,
    addingColumn,
    loadingCurrentTable,
    currentTableColumns,
    table,
    loadingPredefinedColumns,
    predefinedColumns,
}) => {
    const [isDeleteModalVisible, setIsDeleteModalVisible] =
        React.useState(false);
    const [currentRow, setCurrentRow] = React.useState(null);

    React.useEffect(() => {
        dispatch(tablesActions.getTableColumns(Schemas.Data, table));
    }, [table]);

    const showDeleteConfirmationModal = (row) => {
        setIsDeleteModalVisible(true);
        setCurrentRow(row);
    };
    const buttonsConfig = [
        {
            label: "Delete",
            class: "btn-info",
            handle: () => handleOnDeleteComplete(true),
            closeOnClick: true,
        },
        {
            label: "Cancel",
            class: "btn-secondary",
            handle: () => handleOnDeleteComplete(null),
            closeOnClick: true,
        },
    ];

    const handleOnDeleteComplete = (result) => {
        const row = currentRow;

        if (result === true) {
            console.log(row);
            console.log(row.Name);
            dispatch(tablesActions.deleteColumn(table, row.Name));
        }
        setIsDeleteModalVisible(false);
    };

    if (deletedColumnSuccess === true && deletingColumn === false) {
        dispatch(tablesActions.acknowledgeDeleteColumn());
        dispatch(tablesActions.getTableColumns(Schemas.Data, table));
    }
    if (addedColumnSuccess === true && addingColumn === false) {
        dispatch(tablesActions.getTableColumns(Schemas.Data, table));
    }
    if (loadingCurrentTable || deletingColumn || loadingPredefinedColumns) {
        return (
            <div className="row">
                <div className="col-sm-1 col-md-3"></div>
                <div
                    align="center"
                    className="col-sm-10 col-md-6 col-md-offset-3"
                    style={{ marginTop: "25vh" }}
                >
                    <p>
                        Loading...
                        <span style={{ marginLeft: "16px" }}>
                            <img src="data:image/gif;base64,R0lGODlhEAAQAPIAAP///wAAAMLCwkJCQgAAAGJiYoKCgpKSkiH/C05FVFNDQVBFMi4wAwEAAAAh/hpDcmVhdGVkIHdpdGggYWpheGxvYWQuaW5mbwAh+QQJCgAAACwAAAAAEAAQAAADMwi63P4wyklrE2MIOggZnAdOmGYJRbExwroUmcG2LmDEwnHQLVsYOd2mBzkYDAdKa+dIAAAh+QQJCgAAACwAAAAAEAAQAAADNAi63P5OjCEgG4QMu7DmikRxQlFUYDEZIGBMRVsaqHwctXXf7WEYB4Ag1xjihkMZsiUkKhIAIfkECQoAAAAsAAAAABAAEAAAAzYIujIjK8pByJDMlFYvBoVjHA70GU7xSUJhmKtwHPAKzLO9HMaoKwJZ7Rf8AYPDDzKpZBqfvwQAIfkECQoAAAAsAAAAABAAEAAAAzMIumIlK8oyhpHsnFZfhYumCYUhDAQxRIdhHBGqRoKw0R8DYlJd8z0fMDgsGo/IpHI5TAAAIfkECQoAAAAsAAAAABAAEAAAAzIIunInK0rnZBTwGPNMgQwmdsNgXGJUlIWEuR5oWUIpz8pAEAMe6TwfwyYsGo/IpFKSAAAh+QQJCgAAACwAAAAAEAAQAAADMwi6IMKQORfjdOe82p4wGccc4CEuQradylesojEMBgsUc2G7sDX3lQGBMLAJibufbSlKAAAh+QQJCgAAACwAAAAAEAAQAAADMgi63P7wCRHZnFVdmgHu2nFwlWCI3WGc3TSWhUFGxTAUkGCbtgENBMJAEJsxgMLWzpEAACH5BAkKAAAALAAAAAAQABAAAAMyCLrc/jDKSatlQtScKdceCAjDII7HcQ4EMTCpyrCuUBjCYRgHVtqlAiB1YhiCnlsRkAAAOwAAAAAAAAAAAA==" />
                        </span>
                    </p>
                </div>
                <div className="col-sm-1 col-md-3"></div>
            </div>
        );
    } else {
        if (
            !currentTableColumns ||
            currentTableColumns.length < 1 ||
            !currentTableColumns.columns ||
            currentTableColumns.columns.length < 1
        ) {
            return (
                <div>
                    <p>No columns found!</p>
                </div>
            );
        } else {
            // console.log(predefinedColumns);
            const keys = Object.keys(currentTableColumns.columns[0]);
            return (
                <React.Fragment>
                    <ApiraiserTable headers={keys}>
                        {currentTableColumns.columns &&
                            currentTableColumns.columns.map((column, i) => (
                                <tr key={"col_" + (i + 1)}>
                                    <td key={"data_" + i + "_#"} scope="row">
                                        <div
                                            style={{
                                                display: "flex",
                                                flexDirection: "row",
                                            }}
                                        >
                                            {/* <ButtonIcon icon="edit" color="#007bff" onClick={() => this.showEditModal(row)}/> */}
                                            {
                                                <ButtonIcon
                                                    disabled={isColumnPredefined(
                                                        column
                                                    )}
                                                    icon="trash"
                                                    color="#dc3545"
                                                    onClick={() =>
                                                        showDeleteConfirmationModal(
                                                            column
                                                        )
                                                    }
                                                />
                                            }
                                        </div>
                                    </td>
                                    {keys.map((key) => (
                                        <td key={"data_" + i + key}>
                                            {column[key] != null
                                                ? "" + column[key]
                                                : ""}
                                        </td>
                                    ))}
                                </tr>
                            ))}
                    </ApiraiserTable>

                    {isDeleteModalVisible && (
                        <DialogModal
                            headerLabel="Delete"
                            handleOnClose={handleOnDeleteComplete}
                            text="Do you really want to delete?"
                            buttonsConfig={buttonsConfig}
                        />
                    )}
                </React.Fragment>
            );
        }
    }
};

function mapStateToProps(state) {
    const { loggedIn } = state.authentication;
    const { loadingPredefinedColumns, predefinedColumns } =
        state.predefinedColumnsInfo;
    const {
        loadingCurrentTable,
        deletedColumnSuccess,
        deletingColumn,
        currentTableColumns,
        addedColumnSuccess,
        addingColumn,
    } = state.currentTableInfo;
    const { deletedRowSuccess, deletingRow } = state.currentTableData;

    return {
        deletedRowSuccess,
        deletingRow,
        loadingCurrentTable,
        currentTableColumns,
        loggedIn,
        loadingPredefinedColumns,
        predefinedColumns,
        deletedColumnSuccess,
        deletingColumn,
        addedColumnSuccess,
        addingColumn,
    };
}

const connectedTableColumns = connect(mapStateToProps)(TableColumns);
export { connectedTableColumns as TableColumns };
