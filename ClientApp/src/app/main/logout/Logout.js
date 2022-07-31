import { logoutUser } from "app/auth/store/userSlice";
import { useDispatch } from "react-redux";
import { useEffect } from "react";

const Logout = () => {
    const dispatch = useDispatch();

    useEffect(() => {
        dispatch(logoutUser());
    }, [dispatch]);

    return "Logging out..";
};

export default Logout;
