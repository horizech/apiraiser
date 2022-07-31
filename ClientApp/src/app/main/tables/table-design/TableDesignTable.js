import FuseLoading from "@fuse/core/FuseLoading";
import FuseScrollbars from "@fuse/core/FuseScrollbars";
import withRouter from "@fuse/core/withRouter";
import FuseUtils from "@fuse/utils";
import _ from "@lodash";
import { Button, Icon, IconButton } from "@mui/material";
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
import { CreateEditTableColumnDialog } from "./CreateEditTableColumnDialog";
import { DeleteTableColumnDialog } from "./DeleteTableColumnDialog";
import TableDesignTableHead from "./TableDesignTableHead";
import { DynamicRowElement } from "app/shared-components/dynamic-row-element/DynamicRowElement";
import { DataUtils } from "app/utils/data";
import { TableService } from "../TableService";
import { selectAll } from "../TableSlice";

const _TableDesignTable = (props) => {
    const dispatch = useDispatch();
    const schema = props.schema;
    const table = props.table;

    const statePath = `${schema}_${table}`;
    const tableSlice = useSelector(
        ({ apiraiser }) => apiraiser.slices[statePath]
    );

    const [response, setResponse] = useState([]);

    const { t } = useTranslation("apiraiserTranslations");

    const usersState = useSelector((state) => state["Administration_Users"]);
    const [users, setUsers] = useState({});

    const entities = useSelector((state) => state[statePath].columns);
    const columns = DataUtils.GetDesignColumns();

    const columnsState = useSelector((state) => state[statePath].columnsState);

    const searchText = useSelector((state) => state[statePath].searchText);

    const errors = useSelector((state) => state[statePath].errors);

    const [loading, setLoading] = useState(true);
    const [data, setTableColumn] = useState(response || []);
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(10);
    const [sorting, setSorting] = useState({
        direction: "asc",
        id: null,
    });

    const [isCreateEditDialogOpen, setCreateEditDialogOpen] = useState(false);
    const [curEditRow, setCurEditRow] = useState(null);
    const [isDeleteDialogOpen, setDeleteDialogOpen] = useState(false);
    const [curDeleteName, setCurDeleteName] = useState(null);

    const showCreateDialog = () => {
        setCurEditRow(null);
        setCreateEditDialogOpen(true);
    };
    const showEditDialog = (row) => {
        setCurEditRow(DataUtils.DecodeData(row, entities));
        setCreateEditDialogOpen(true);
    };
    const handleCreateEditDialogClose = () => {
        setCreateEditDialogOpen(false);
    };

    const showDeleteDialog = (name) => {
        setCurDeleteName(name);
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
                (!entities || !entities.length) &&
                columnsState !== "loading" &&
                columnsState !== "success"
            ) {
                dispatch(tableSlice.thunks.getColumns());
            }
        }
    }, [table, dispatch, entities, columnsState]);

    useEffect(() => {
        switch (columnsState) {
            case "loading":
                setLoading(true);
                break;
            default:
                setLoading(false);
        }
    }, [columnsState]);

    useEffect(() => {
        if (searchText && searchText.length !== 0) {
            setTableColumn(FuseUtils.filterArrayByString(response, searchText));
            setPage(0);
        } else {
            setTableColumn(response);
        }
    }, [response, searchText]);

    const handleRequestSort = (event, property) => {
        const id = property;
        let direction = "desc";

        if (sorting.id === property && sorting.direction === "desc") {
            direction = "asc";
        }

        setSorting({
            direction,
            id,
        });
    };

    const handleChangePage = (event, value) => {
        setPage(value);
    };

    const handleChangeRowsPerPage = (event) => {
        setRowsPerPage(event.target.value);
    };

    if (!table || loading || !entities || !entities.length) {
        return <FuseLoading />;
    }

    if (columnsState === "error") {
        return (
            <motion.div
                initial={{ opacity: 0 }}
                animate={{ opacity: 1, transition: { delay: 0.1 } }}
                className="flex flex-1 items-center justify-center h-full"
            >
                {errors.map((error, i) => (
                    <Typography
                        key={"error_" + i}
                        color="textSecondary"
                        variant="h5"
                    >
                        {error}
                    </Typography>
                ))}
            </motion.div>
        );
    }

    return (
        <div className="w-full flex flex-col">
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
            {(!entities || entities.length === 0) && (
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
            {entities && entities.length > 0 && (
                <>
                    <FuseScrollbars className="grow overflow-x-auto">
                        <Table
                            stickyHeader
                            className="min-w-xl"
                            aria-labelledby="tableTitle"
                        >
                            <TableDesignTableHead
                                schema={schema}
                                table={table}
                                Sorting={sorting}
                                onRequestSort={handleRequestSort}
                                rowCount={entities.length}
                            />

                            <TableBody>
                                {_.orderBy(
                                    entities,
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
                                        return (
                                            <TableRow
                                                className="h-72 cursor-pointer"
                                                hover
                                                tabIndex={-1}
                                                key={n.Name}
                                            >
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
                                                            disabled={DataUtils.IsColumnPredefined(
                                                                n
                                                            )}
                                                            onClick={() => {
                                                                showDeleteDialog(
                                                                    n.Name
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
                        count={entities.length}
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
                <CreateEditTableColumnDialog
                    schema={schema}
                    table={table}
                    data={curEditRow}
                    open={isCreateEditDialogOpen}
                    onClose={handleCreateEditDialogClose}
                />
            )}
            {isDeleteDialogOpen && (
                <DeleteTableColumnDialog
                    schema={schema}
                    table={table}
                    keepMounted
                    name={curDeleteName}
                    open={isDeleteDialogOpen}
                    onClose={handleDeleteDialogClose}
                />
            )}
        </div>
    );
};

export const TableDesignTable = withRouter(_TableDesignTable);
