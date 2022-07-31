import FuseLoading from "@fuse/core/FuseLoading";
import FuseUtils from "@fuse/utils";
import CallbackConfig from "app/main/callback/CallbackConfig";
import LoginConfig from "app/main/login/LoginConfig";
import LogoutConfig from "app/main/logout/LogoutConfig";
import RegisterConfig from "app/main/register/RegisterConfig";
import { Navigate } from "react-router-dom";
import tablesConfigs from "app/main/tables/tablesConfigs";
import ApiraiserConfig from "app/main/apiraiser/ApiraiserConfig";
import errorsConfigs from "app/main/errors/errorsConfigs";
import FaqPageConfig from "app/main/faq/FaqPageConfig";
import HomePageConfig from "app/main/home/HomePageConfig";

const routeConfigs = [
    ...tablesConfigs,
    ...errorsConfigs,
    ApiraiserConfig,
    FaqPageConfig,
    LoginConfig,
    RegisterConfig,
    LogoutConfig,
    CallbackConfig,
    HomePageConfig,
];

const routes = [
    // if you want to make whole app auth protected by default change defaultAuth for example:
    // ...FuseUtils.generateRoutesFromConfigs(routeConfigs, ['admin','staff','user']),
    // The individual route configs which has auth option won't be overridden.
    ...FuseUtils.generateRoutesFromConfigs(routeConfigs, null),
    {
        path: "/",
        element: <Navigate to="apps/dashboards/analytics" />,
    },
    {
        path: "loading",
        element: <FuseLoading />,
    },
    {
        path: "*",
        element: <Navigate to="pages/errors/error-404" />,
    },
];

export default routes;
