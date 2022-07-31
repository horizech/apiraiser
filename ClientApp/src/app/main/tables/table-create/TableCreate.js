import FuseHighlight from "@fuse/core/FuseHighlight";
import FuseUtils from "@fuse/utils";
import Button from "@mui/material/Button";
import Typography from "@mui/material/Typography";
import { authRoles } from "app/auth";
import { useDispatch, useSelector } from "react-redux";
import {
    appendNavigationItem,
    prependNavigationItem,
    removeNavigationItem,
    updateNavigationItem,
    setNavigation,
    resetNavigation,
} from "app/store/fuse/navigationSlice";
import {
    Icon,
    Paper,
    Input,
    ThemeProvider,
    CircularProgress,
} from "@mui/material";
import { motion } from "framer-motion";
import { useTranslation } from "react-i18next";
import { selectMainTheme } from "app/store/fuse/settingsSlice";
import ApiraiserSlice from "app/main/apiraiser/ApiraiserSlice";
import React from "react";
import apiraiser from "app/main/apiraiser/ApiraiserSlice";

function TableCreate({ schema }) {
    const dispatch = useDispatch();
    const { t } = useTranslation("apiraiserTranslations");
    const mainTheme = useSelector(selectMainTheme);

    const [isCreating, setCreating] = React.useState(false);
    const [table, setTable] = React.useState("");

    const roleIds = useSelector(({ auth }) =>
        auth.user && auth.user.roleIds ? auth.user.roleIds : []
    );

    const createTableState = useSelector(
        ({ apiraiser }) => apiraiser.createTableState
    );

    React.useEffect(() => {
        if (createTableState === "loading") {
            setCreating(true);
        } else if (createTableState === "success" && isCreating) {
            // onClose();
            dispatch(apiraiser.thunks.loadAccessibleTables(roleIds));
            setCreating(false);
        }
    }, [createTableState]);

    const createTable = () => {
        dispatch(ApiraiserSlice.thunks.createTable({ schema, table }));
    };

    return (
        <>
            <div className="flex flex-1 w-full items-center justify-between">
                <div className="flex flex-1 flex-col w-full items-center justify-between">
                    <motion.div
                        initial={{ opacity: 0 }}
                        animate={{ opacity: 1, transition: { delay: 0.1 } }}
                        className="flex flex-1 items-center justify-center h-full mb-24"
                    >
                        <Typography color="textSecondary" variant="h5">
                            {t("ADD_NEW_TABLE")}
                        </Typography>
                    </motion.div>

                    <ThemeProvider theme={mainTheme}>
                        <div className="flex flex-1 w-full items-center justify-center">
                            <Paper
                                component={motion.div}
                                initial={{ y: -20, opacity: 0 }}
                                animate={{
                                    y: 0,
                                    opacity: 1,
                                    transition: { delay: 0.2 },
                                }}
                                className="flex items-center w-full max-w-512 px-8 py-4 rounded-16 shadow"
                            >
                                <Input
                                    placeholder={t("TABLE_NAME")}
                                    className="flex flex-1 mx-8"
                                    disableUnderline
                                    fullWidth
                                    inputProps={{
                                        "aria-label": "Search",
                                    }}
                                    onChange={(ev) =>
                                        void setTable(ev.target.value || "")
                                    }
                                />
                            </Paper>
                            <div className="pl-16">
                                <Button
                                    onClick={createTable}
                                    variant="outlined"
                                    color="success"
                                    disabled={isCreating}
                                >
                                    {isCreating && (
                                        <CircularProgress size={"2rem"} />
                                    )}
                                    <Icon>add</Icon>
                                    {t("ADD_NEW")}
                                </Button>
                            </div>
                        </div>
                    </ThemeProvider>
                </div>
            </div>
        </>
    );
}

export default TableCreate;
