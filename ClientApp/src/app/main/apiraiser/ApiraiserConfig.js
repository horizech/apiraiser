import { authRoles } from "app/auth";
import InitializeApiraiser from "./InitializeApiraiser";

const ApiraiserConfig = {
    settings: {
        layout: {
            config: {
                navbar: {
                    display: false,
                },
                toolbar: {
                    display: false,
                },
                footer: {
                    display: false,
                },
                leftSidePanel: {
                    display: false,
                },
                rightSidePanel: {
                    display: false,
                },
            },
        },
    },
    auth: authRoles.onlyGuest,
    routes: [
        {
            path: "/apiraiser/initialize",
            element: <InitializeApiraiser />,
        },
    ],
};

export default ApiraiserConfig;
