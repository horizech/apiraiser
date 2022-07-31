import { lazy } from "react";

const FaqPage = lazy(() => import("./FaqPage"));

const FaqPageConfig = {
    settings: {
        layout: {
            config: {},
        },
    },
    routes: [
        {
            path: "faq",
            element: <FaqPage />,
        },
    ],
};

export default FaqPageConfig;
