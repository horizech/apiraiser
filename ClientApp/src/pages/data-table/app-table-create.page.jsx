import React from "react";
import { Schemas } from "../../constants";
import { TableCreatePage } from "../table/table-create.page";

export const AppTableCreatePage = ({ ...rest }) => {
    return <TableCreatePage {...rest} schema={Schemas.Data}></TableCreatePage>;
};
