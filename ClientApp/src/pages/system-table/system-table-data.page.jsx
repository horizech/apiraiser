import React from "react";
import { TableDataPage } from "../table/table-data.page";

export const SystemTableDataPage = ({ ...rest }) => {
    return <TableDataPage {...rest} schema={"System"}></TableDataPage>;
};
