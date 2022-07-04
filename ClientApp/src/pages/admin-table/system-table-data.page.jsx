import React from "react";
import { Schemas } from "../../constants";
import { TableDataPage } from "../table/table-data.page";

export const SystemTableDataPage = ({ ...rest }) => {
    return (
        <TableDataPage
            {...rest}
            schema={Schemas.Administration}
        ></TableDataPage>
    );
};
