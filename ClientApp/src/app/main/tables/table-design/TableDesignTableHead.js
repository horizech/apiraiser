import TableCell from "@mui/material/TableCell";
import TableRow from "@mui/material/TableRow";
import TableSortLabel from "@mui/material/TableSortLabel";
import Tooltip from "@mui/material/Tooltip";
import { useDispatch, useSelector } from "react-redux";
import TableHead from "@mui/material/TableHead";
import { useTranslation } from "react-i18next";
import { TableService } from "../TableService";
import { DataUtils } from "app/utils/data";

const TableDesignTableHead = (props) => {
    const schema = props.schema;
    const table = props.table;

    const statePath = `${schema}_${table}`;
    const tableSlice = useSelector(
        ({ apiraiser }) => apiraiser.slices[statePath]
    );

    const dispatch = useDispatch();

    const { t } = useTranslation("apiraiserTranslations");

    const columns = DataUtils.GetDesignColumns();

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
    const rows = r;

    const columnsState = useSelector((state) => state[statePath].columnsState);

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

    return (
        <TableHead>
            <TableRow className="h-48 sm:h-64">
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

export default TableDesignTableHead;
