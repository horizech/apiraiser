import { lazy } from "react";

const HomePage = lazy(() => import("./HomePage"));

const HomePageConfig = {
    settings: {
        layout: {
            config: {},
        },
    },
    routes: [
        {
            path: "/",
            element: <HomePage />,
        },
    ],
};

export default HomePageConfig;
