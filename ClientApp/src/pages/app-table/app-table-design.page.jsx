import React from "react";
import { TableDesignPage } from "../table/table-design.page";

export const AppTableDesignPage = ({ ...rest }) => {
    return <TableDesignPage {...rest} schema={"Application"}></TableDesignPage>;
};
