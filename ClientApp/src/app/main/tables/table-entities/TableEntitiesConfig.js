import { lazy } from "react";
const TableEntities = lazy(() => import("./TableEntities"));

const TableEntitiesConfig = {
    settings: {
        layout: {
            config: {},
        },
    },
    routes: [
        {
            path: "data/:table",
            element: <TableEntities schema={"Data"} />,
        },
        {
            path: "administration/:table",
            element: <TableEntities schema={"Administration"} />,
        },
    ],
};

export default TableEntitiesConfig;
