import { lazy } from "react";
const TableDesign = lazy(() => import("./TableDesign"));

const TableDesignConfig = {
    settings: {
        layout: {
            config: {},
        },
    },
    routes: [
        {
            path: "data/:table/design",
            element: <TableDesign schema={"Data"} />,
        },
        {
            path: "administration/:table/design",
            element: <TableDesign schema={"Administration"} />,
        },
    ],
};

export default TableDesignConfig;
