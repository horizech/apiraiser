import React from "react";
import { Schemas } from "../../constants";
import { TableDesignPage } from "../table/table-design.page";

export const AppTableDesignPage = ({ ...rest }) => {
    return <TableDesignPage {...rest} schema={Schemas.Data}></TableDesignPage>;
};
