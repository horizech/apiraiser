import React from "react";
import { Schemas } from "../../constants";
import { TableCreatePage } from "../table/table-create.page";

export const SystemTableCreatePage = ({ ...rest }) => {
    return (
        <TableCreatePage
            {...rest}
            schema={Schemas.Administration}
        ></TableCreatePage>
    );
};
