import React from "react";
import { TableCreatePage } from "../table/table-create.page";

export const SystemTableCreatePage = ({ ...rest }) => {
    return <TableCreatePage {...rest} schema={"System"}></TableCreatePage>;
};
