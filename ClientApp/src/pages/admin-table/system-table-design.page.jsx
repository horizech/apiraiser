import React from "react";
import { Schemas } from "../../constants";
import { TableDesignPage } from "../table/table-design.page";

export const SystemTableDesignPage = ({ ...rest }) => {
    return (
        <TableDesignPage
            {...rest}
            schema={Schemas.Administration}
        ></TableDesignPage>
    );
};
