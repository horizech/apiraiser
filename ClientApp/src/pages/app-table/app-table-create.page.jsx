import React from "react";
import { TableCreatePage } from "../table/table-create.page";

export const AppTableCreatePage = ({ ...rest }) => {
    return <TableCreatePage {...rest} schema={"Application"}></TableCreatePage>;
};
