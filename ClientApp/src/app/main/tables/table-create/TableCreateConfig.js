import { lazy } from "react";
const TableCreate = lazy(() => import("./TableCreate"));

const TableCreateConfig = {
    settings: {
        layout: {
            config: {},
        },
    },
    routes: [
        {
            path: "data",
            element: <TableCreate schema={"Data"} />,
        },
        {
            path: "administration",
            element: <TableCreate schema={"Administration"} />,
        },
    ],
};

export default TableCreateConfig;
