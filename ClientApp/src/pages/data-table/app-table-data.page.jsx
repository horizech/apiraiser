import React from "react";
import { Schemas } from "../../constants";
import { TableDataPage } from "../table/table-data.page";

export const AppTableDataPage = ({ ...rest }) => {
    return <TableDataPage {...rest} schema={Schemas.Data}></TableDataPage>;
};
