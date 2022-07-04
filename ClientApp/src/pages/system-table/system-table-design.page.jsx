import React from "react";
import { TableDesignPage } from "../table/table-design.page";

export const SystemTableDesignPage = ({ ...rest }) => {
    return <TableDesignPage {...rest} schema={"System"}></TableDesignPage>;
};
