import React from "react";
import { TableDataPage } from "../table/table-data.page";

export const AppTableDataPage = ({ ...rest }) => {
    return <TableDataPage {...rest} schema={"Application"}></TableDataPage>;
};
