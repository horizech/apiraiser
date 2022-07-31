import Checkbox from "@mui/material/Checkbox";
import Icon from "@mui/material/Icon";
import IconButton from "@mui/material/IconButton";
import ListItemIcon from "@mui/material/ListItemIcon";
import ListItemText from "@mui/material/ListItemText";
import Menu from "@mui/material/Menu";
import MenuItem from "@mui/material/MenuItem";
import MenuList from "@mui/material/MenuList";
import TableCell from "@mui/material/TableCell";
import TableRow from "@mui/material/TableRow";
import TableSortLabel from "@mui/material/TableSortLabel";
import Tooltip from "@mui/material/Tooltip";
import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { Box } from "@mui/system";
import TableHead from "@mui/material/TableHead";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";
import { TableService } from "../TableService";

const TableEntitiesTableHead = (props) => {
    const { selectedTableEntityIds } = props;
    const numSelected = selectedTableEntityIds.length;
    const schema = props.schema;
    const table = props.table;

    const statePath = `${schema}_${table}`;
    const tableSlice = useSelector(
        ({ apiraiser }) => apiraiser.slices[statePath]
    );

    const [selectedTableEntityMenu, setSelectedTableEntityMenu] =
        useState(null);

    const dispatch = useDispatch();

    const { t } = useTranslation("apiraiserTranslations");

    const columns = useSelector((state) => state[statePath].columns);

    const columnsState = useSelector((state) => state[statePath].columnsState);

    const [rows, setRows] = useState([]);

    useEffect(() => {
        if (table && dispatch) {
            if (!columns && columnsState !== "Loading") {
                dispatch(tableSlice.thunks.getColumns());
            }
        }
    }, [table, dispatch, columns, columnsState]);

    useEffect(() => {
        if (columns && table && columns) {
            let r = columns.map((column) => {
                return {
                    id: column.Name,
                    align: "left",
                    disablePadding: false,
                    label: t(column.Name.toUpperCase()),
                    sort: true,
                };
            });
            r.push({
                id: "Actions",
                align: "left",
                disablePadding: false,
                label: t("ACTIONS"),
                sort: false,
            });
            setRows(r);
        }
    }, [table, columns]);

    // const rows = [
    //     {
    //         id: "Id",
    //         align: "left",
    //         disablePadding: false,
    //         label: t("ID"),
    //         sort: true,
    //     },
    //     {
    //         id: "CreatedBy",
    //         align: "left",
    //         disablePadding: false,
    //         label: t("CREATEDBY"),
    //         sort: true,
    //     },
    //     {
    //         id: "CreatedOn",
    //         align: "left",
    //         disablePadding: false,
    //         label: t("CREATEDON"),
    //         sort: true,
    //     },
    //     {
    //         id: "LastUpdatedBy",
    //         align: "right",
    //         disablePadding: false,
    //         label: t("LASTUPDATEDBY"),
    //         sort: true,
    //     },
    //     {
    //         id: "LastUpdatedOn",
    //         align: "left",
    //         disablePadding: false,
    //         label: t("LASTUPDATEDON"),
    //         sort: true,
    //     },
    //     {
    //         id: "Name",
    //         align: "left",
    //         disablePadding: false,
    //         label: t("NAME"),
    //         sort: true,
    //     },
    //     {
    //         id: "Description",
    //         align: "left",
    //         disablePadding: false,
    //         label: t("DESCRIPTION"),
    //         sort: true,
    //     },
    //     {
    //         id: "Actions",
    //         align: "left",
    //         disablePadding: false,
    //         label: t("ACTIONS"),
    //         sort: false,
    //     },
    // ];

    const createSortHandler = (property) => (event) => {
        props.onRequestSort(event, property);
    };

    const openSelectedTableEntityMenu = (event) => {
        setSelectedTableEntityMenu(event.currentTarget);
    };

    const closeSelectedTableEntityMenu = () => {
        setSelectedTableEntityMenu(null);
    };

    // const {onSelectAllClick, TableEntity, orderBy, numSelected, rowCount} = props;

    return (
        <TableHead>
            <TableRow className="h-48 sm:h-64">
                <TableCell
                    padding="none"
                    className="w-40 md:w-64 text-center z-99"
                >
                    <Checkbox
                        indeterminate={
                            numSelected > 0 && numSelected < props.rowCount
                        }
                        checked={
                            props.rowCount !== 0 &&
                            numSelected === props.rowCount
                        }
                        onChange={props.onSelectAllClick}
                    />
                    {numSelected > 0 && (
                        <Box
                            className="flex items-center justify-center absolute w-64 top-0 ltr:left-0 rtl:right-0 mx-56 h-64 z-10 border-b-1"
                            sx={{
                                background: (theme) =>
                                    theme.palette.background.paper,
                            }}
                        >
                            <IconButton
                                aria-owns={
                                    selectedTableEntityMenu
                                        ? "selectedTableEntityMenu"
                                        : null
                                }
                                aria-haspopup="true"
                                onClick={openSelectedTableEntityMenu}
                                size="large"
                            >
                                <Icon>more_horiz</Icon>
                            </IconButton>
                            <Menu
                                id="selectedTableEntityMenu"
                                anchorEl={selectedTableEntityMenu}
                                open={Boolean(selectedTableEntityMenu)}
                                onClose={closeSelectedTableEntityMenu}
                            >
                                <MenuList>
                                    <MenuItem
                                    // onClick={() => {
                                    //   dispatch(removeTableEntity(selectedTableEntityIds));
                                    //   props.onMenuItemClick();
                                    //   closeSelectedTableEntityMenu();
                                    // }}
                                    >
                                        <ListItemIcon className="min-w-40">
                                            <Icon>delete</Icon>
                                        </ListItemIcon>
                                        <ListItemText primary="Remove" />
                                    </MenuItem>
                                </MenuList>
                            </Menu>
                        </Box>
                    )}
                </TableCell>
                {rows.map((row) => {
                    return (
                        <TableCell
                            className="p-4 md:p-16"
                            key={row.id}
                            align={row.align}
                            padding={row.disablePadding ? "none" : "normal"}
                            sortDirection={
                                props.Sorting.id === row.id
                                    ? props.Sorting.direction
                                    : false
                            }
                        >
                            {!row.sort && (
                                <p className="font-semibold">{row.label}</p>
                            )}
                            {row.sort && (
                                <Tooltip
                                    title="Sort"
                                    placement={
                                        row.align === "right"
                                            ? "bottom-end"
                                            : "bottom-start"
                                    }
                                    enterDelay={300}
                                >
                                    <TableSortLabel
                                        active={props.Sorting.id === row.id}
                                        direction={props.Sorting.direction}
                                        onClick={createSortHandler(row.id)}
                                        className="font-semibold"
                                    >
                                        {row.label}
                                    </TableSortLabel>
                                </Tooltip>
                            )}
                        </TableCell>
                    );
                }, this)}
            </TableRow>
        </TableHead>
    );
};

export default TableEntitiesTableHead;
