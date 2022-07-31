import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useLocation } from "react-router-dom";
import history from "@history";
import apiraiser from "./ApiraiserSlice";
import { logoutUser } from "app/auth/store/userSlice";
import FuseSplashScreen from "@fuse/core/FuseSplashScreen";
import InitializeApiraiser from "./InitializeApiraiser";

export const ApiraiserCheck = ({ children }) => {
    const dispatch = useDispatch();
    const location = useLocation();

    const isInitializedState = useSelector(
        ({ apiraiser }) => apiraiser.isInitializedState
    );

    const [needsInitialize, setNeedsInitialize] = useState(false);

    useEffect(async () => {
        if (isInitializedState === "waiting") {
            dispatch(apiraiser.thunks.isInitialized());
        } else if (isInitializedState === "error") {
            try {
                dispatch(logoutUser());
            } catch (e) {}
            setTimeout(() => history.push("/apiraiser/initialize"), 1000);
            setNeedsInitialize(true);
        } else if (isInitializedState === "success") {
            if (location.pathname === "/apiraiser/initialize") {
                setTimeout(() => history.push("/"), 0);
            } else if (needsInitialize) {
                setTimeout(() => history.push(location.pathname), 0);
            }
        }
    }, [isInitializedState]);

    if (isInitializedState === "success") {
        return <>{children}</>;
    }
    if (isInitializedState === "error") {
        return <InitializeApiraiser />;
    }

    return <FuseSplashScreen />;
};
