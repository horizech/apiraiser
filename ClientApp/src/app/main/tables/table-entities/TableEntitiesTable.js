import FuseLoading from "@fuse/core/FuseLoading";
import FuseScrollbars from "@fuse/core/FuseScrollbars";
import withRouter from "@fuse/core/withRouter";
import FuseUtils from "@fuse/utils";
import _ from "@lodash";
import { Button, Icon, IconButton } from "@mui/material";
import Checkbox from "@mui/material/Checkbox";
import Table from "@mui/material/Table";
import TableBody from "@mui/material/TableBody";
import TableCell from "@mui/material/TableCell";
import TablePagination from "@mui/material/TablePagination";
import TableRow from "@mui/material/TableRow";
import Typography from "@mui/material/Typography";
import { motion } from "framer-motion";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { useDispatch, useSelector } from "react-redux";
import { CreateEditTableEntityDialog } from "./CreateEditTableEntityDialog";
import { DeleteTableEntityDialog } from "./DeleteTableEntityDialog";
import TableEntitiesTableHead from "./TableEntitiesTableHead";
import { DynamicRowElement } from "app/shared-components/dynamic-row-element/DynamicRowElement";
import { DataUtils } from "app/utils/data";
import { TableService } from "../TableService";
import { selectAll } from "../TableSlice";
import { Link } from "react-router-dom";

const _TableEntitiesTable = (props) => {
    const dispatch = useDispatch();
    const schema = props.schema;
    const table = props.table;

    const statePath = `${schema}_${table}`;
    const tableSlice = useSelector(
        ({ apiraiser }) => apiraiser.slices[statePath]
    );
    // const [tableSlice , setTableSlice] = useState(null);

    const [response, setResponse] = useState([]);

    const { t } = useTranslation("apiraiserTranslations");

    const ids = useSelector((state) => state[statePath].ids);

    const usersState = useSelector((state) => state["Administration_Users"]);
    const [users, setUsers] = useState({});

    const entities = useSelector((state) => state[statePath].entities);

    const columns = useSelector((state) => state[statePath].columns);

    const columnsState = useSelector((state) => state[statePath].columnsState);

    const searchText = useSelector((state) => state[statePath].searchText);

    const entitiesState = useSelector(
        (state) => state[statePath].entitiesState
    );

    const errors = useSelector((state) => state[statePath].errors);

    const [loading, setLoading] = useState(true);
    const [selected, setSelected] = useState([]);
    const [data, setTableEntity] = useState(response || []);
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(10);
    const [sorting, setSorting] = useState({
        direction: "asc",
        id: null,
    });

    const [isCreateEditDialogOpen, setCreateEditDialogOpen] = useState(false);
    const [curEditRow, setCurEditRow] = useState(null);
    const [isDeleteDialogOpen, setDeleteDialogOpen] = useState(false);
    const [curDeleteId, setCurDeleteId] = useState(null);

    const showCreateDialog = () => {
        setCurEditRow(null);
        setCreateEditDialogOpen(true);
    };
    const showEditDialog = (row) => {
        setCurEditRow(DataUtils.DecodeData(row, columns));
        setCreateEditDialogOpen(true);
    };
    const handleCreateEditDialogClose = () => {
        setCreateEditDialogOpen(false);
    };

    const showDeleteDialog = (id) => {
        setCurDeleteId(id);
        setDeleteDialogOpen(true);
    };
    const handleDeleteDialogClose = () => {
        setDeleteDialogOpen(false);
    };

    useEffect(() => {
        dispatch(tableSlice.actions.initialize({ schema, table }));
    }, [dispatch]);

    useEffect(() => {
        if (table && dispatch) {
            if (
                Object.keys(entities).length == 0 &&
                entitiesState !== "loading" &&
                entitiesState !== "success"
            ) {
                dispatch(tableSlice.thunks.getEntities());
            }
        }
    }, [table, dispatch, entities, entitiesState]);

    useEffect(() => {
        if (table && dispatch) {
            if (
                (!columns || !columns.length) &&
                columnsState !== "loading" &&
                columnsState !== "success"
            ) {
                dispatch(tableSlice.thunks.getColumns());
            }
        }
    }, [table, dispatch, columns, columnsState]);

    useEffect(() => {
        if (
            entities &&
            ids &&
            table &&
            Object.keys(entities).length === ids.length
        ) {
            setResponse(
                selectAll(
                    {
                        entities: entities,
                        ids: ids,
                    },
                    table
                )
            );
        }
    }, [entities, ids]);

    useEffect(() => {
        if (
            usersState &&
            usersState.entities &&
            Object.keys(usersState.entities).length &&
            usersState.ids &&
            Object.keys(usersState.ids).length
        ) {
            setUsers(usersState.entities);
        }
    }, [usersState]);

    useEffect(() => {
        switch (entitiesState) {
            case "loading":
                setLoading(true);
                break;
            default:
                setLoading(false);
        }
    }, [entitiesState]);

    useEffect(() => {
        if (searchText && searchText.length !== 0) {
            setTableEntity(FuseUtils.filterArrayByString(response, searchText));
            setPage(0);
        } else {
            setTableEntity(response);
        }
    }, [response, searchText]);

    const handleRequestSort = (event, property) => {
        const id = property;
        let direction = "desc";

        if (sorting.id === property && sorting.direction === "desc") {
            direction = "asc";
        }
        const { t } = useTranslation("apiraiserTranslations");

        setSorting({
            direction,
            id,
        });
    };

    const handleSelectAllClick = (event) => {
        if (event.target.checked) {
            setSelected(data.map((n) => n.Id));
            return;
        }
        setSelected([]);
    };

    const handleDeselect = () => {
        setSelected([]);
    };

    const handleClick = (item) => {
        props.navigate(`/apps/e-commerce/orders/${item.Id}`);
    };

    const handleCheck = (event, id) => {
        const selectedIndex = selected.indexOf(id);
        let newSelected = [];

        if (selectedIndex === -1) {
            newSelected = newSelected.concat(selected, id);
        } else if (selectedIndex === 0) {
            newSelected = newSelected.concat(selected.slice(1));
        } else if (selectedIndex === selected.length - 1) {
            newSelected = newSelected.concat(selected.slice(0, -1));
        } else if (selectedIndex > 0) {
            newSelected = newSelected.concat(
                selected.slice(0, selectedIndex),
                selected.slice(selectedIndex + 1)
            );
        }

        setSelected(newSelected);
    };

    const handleChangePage = (event, value) => {
        setPage(value);
    };

    const handleChangeRowsPerPage = (event) => {
        setRowsPerPage(event.target.value);
    };

    if (!table || loading || !columns || !columns.length) {
        return <FuseLoading />;
    }

    if (entitiesState === "error") {
        return (
            <motion.div
                initial={{ opacity: 0 }}
                animate={{ opacity: 1, transition: { delay: 0.1 } }}
                className="flex flex-1 items-center justify-center h-full"
            >
                {errors.map((error) => (
                    <Typography color="textSecondary" variant="h5">
                        {error}
                    </Typography>
                ))}
            </motion.div>
        );
    }

    return (
        <div className="w-full flex flex-col">
            <div className="w-full flex flex-row">
                <div className="p-24 pb-16">
                    <Button
                        onClick={showCreateDialog}
                        variant="outlined"
                        color="success"
                        className="w-150"
                    >
                        <Icon>add</Icon>
                        {t("ADD_NEW")}
                    </Button>
                </div>
                <div style={{ flex: "1 1 auto" }}></div>
                <div className="p-24 pb-16">
                    <Link
                        to={`/${schema}/${table}/design`}
                        className="btn btn-primary"
                    >
                        <Button
                            variant="outlined"
                            color="success"
                            className="w-150"
                        >
                            <Icon>edit</Icon>
                            {t("DESIGN")}
                        </Button>
                    </Link>
                </div>
            </div>
            {(!data || data.length === 0) && (
                <>
                    <motion.div
                        initial={{ opacity: 0 }}
                        animate={{ opacity: 1, transition: { delay: 0.1 } }}
                        className="flex flex-1 items-center justify-center h-full"
                    >
                        <Typography color="textSecondary" variant="h5">
                            There are no items!
                        </Typography>
                    </motion.div>
                </>
            )}
            {data && data.length > 0 && (
                <>
                    <FuseScrollbars className="grow overflow-x-auto">
                        <Table
                            stickyHeader
                            className="min-w-xl"
                            aria-labelledby="tableTitle"
                        >
                            <TableEntitiesTableHead
                                schema={schema}
                                table={table}
                                selectedTableEntityIds={selected}
                                Sorting={sorting}
                                onSelectAllClick={handleSelectAllClick}
                                onRequestSort={handleRequestSort}
                                rowCount={data.length}
                                onMenuItemClick={handleDeselect}
                            />

                            <TableBody>
                                {_.orderBy(
                                    data,
                                    [
                                        (o) => {
                                            switch (sorting.id) {
                                                case "Id": {
                                                    return parseInt(o.Id, 10);
                                                }
                                                // case "customer": {
                                                //   return o.customer.firstName;
                                                // }
                                                // case "payment": {
                                                //   return o.payment.method;
                                                // }
                                                // case "status": {
                                                //   return o.status[0].name;
                                                // }
                                                case "Actions": {
                                                    return "Actions";
                                                }
                                                default: {
                                                    return o[sorting.id];
                                                }
                                            }
                                        },
                                    ],
                                    [sorting.direction]
                                )
                                    .slice(
                                        page * rowsPerPage,
                                        page * rowsPerPage + rowsPerPage
                                    )
                                    .map((n) => {
                                        const isSelected =
                                            selected.indexOf(n.Id) !== -1;
                                        return (
                                            <TableRow
                                                className="h-72 cursor-pointer"
                                                hover
                                                role="checkbox"
                                                aria-checked={isSelected}
                                                tabIndex={-1}
                                                key={n.Id}
                                                selected={isSelected}
                                            >
                                                <TableCell
                                                    className="w-40 md:w-64 text-center"
                                                    padding="none"
                                                >
                                                    <Checkbox
                                                        checked={isSelected}
                                                        onClick={(event) =>
                                                            event.stopPropagation()
                                                        }
                                                        onChange={(event) =>
                                                            handleCheck(
                                                                event,
                                                                n.Id
                                                            )
                                                        }
                                                    />
                                                </TableCell>

                                                {columns.map((column) => (
                                                    <TableCell
                                                        key={column.Name}
                                                        className="p-4 md:p-16"
                                                        component="th"
                                                        scope="row"
                                                    >
                                                        <DynamicRowElement
                                                            column={column}
                                                            value={
                                                                n[column.Name]
                                                            }
                                                            users={users}
                                                        ></DynamicRowElement>
                                                    </TableCell>
                                                ))}

                                                <TableCell className="p-4 md:p-16">
                                                    <div className="flex items-center justify-center text-center">
                                                        <IconButton
                                                            className="min-w-auto"
                                                            size="small"
                                                            color="primary"
                                                            onClick={() => {
                                                                showEditDialog(
                                                                    n
                                                                );
                                                            }}
                                                        >
                                                            <Icon>edit</Icon>
                                                        </IconButton>
                                                        <IconButton
                                                            onClick={() => {
                                                                showDeleteDialog(
                                                                    n.Id
                                                                );
                                                            }}
                                                            className="min-w-auto"
                                                            size="small"
                                                            color="warning"
                                                        >
                                                            <Icon>delete</Icon>
                                                        </IconButton>
                                                    </div>
                                                </TableCell>
                                            </TableRow>
                                        );
                                    })}
                            </TableBody>
                        </Table>
                    </FuseScrollbars>

                    <TablePagination
                        className="shrink-0 border-t-1"
                        component="div"
                        count={data.length}
                        rowsPerPage={rowsPerPage}
                        page={page}
                        backIconButtonProps={{
                            "aria-label": "Previous Page",
                        }}
                        nextIconButtonProps={{
                            "aria-label": "Next Page",
                        }}
                        onPageChange={handleChangePage}
                        onRowsPerPageChange={handleChangeRowsPerPage}
                    />
                </>
            )}

            {isCreateEditDialogOpen && (
                <CreateEditTableEntityDialog
                    schema={schema}
                    table={table}
                    data={curEditRow}
                    open={isCreateEditDialogOpen}
                    onClose={handleCreateEditDialogClose}
                />
            )}
            {isDeleteDialogOpen && (
                <DeleteTableEntityDialog
                    schema={schema}
                    table={table}
                    keepMounted
                    id={curDeleteId}
                    open={isDeleteDialogOpen}
                    onClose={handleDeleteDialogClose}
                />
            )}
        </div>
    );
};

export const TableEntitiesTable = withRouter(_TableEntitiesTable);
