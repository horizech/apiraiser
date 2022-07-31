import Icon from "@mui/material/Icon";
import Input from "@mui/material/Input";
import Paper from "@mui/material/Paper";
import { ThemeProvider } from "@mui/material/styles";
import Typography from "@mui/material/Typography";
import { motion } from "framer-motion";
import { useDispatch, useSelector } from "react-redux";
import { selectMainTheme } from "app/store/fuse/settingsSlice";
import { useTranslation } from "react-i18next";
import { TableService } from "../TableService";

const TableDesignHeader = ({ schema, table }) => {
    const dispatch = useDispatch();

    const statePath = `${schema}_${table}`;

    const tableSlice = useSelector(
        ({ apiraiser }) => apiraiser.slices[statePath]
    );

    const searchText = useSelector((state) =>
        state[statePath] ? state[statePath].searchText : ""
    );
    const mainTheme = useSelector(selectMainTheme);
    const { t } = useTranslation("apiraiserTranslations");

    return (
        <div className="flex flex-1 w-full items-center justify-between">
            <div className="flex items-center">
                <Icon
                    component={motion.span}
                    initial={{ scale: 0 }}
                    animate={{ scale: 1, transition: { delay: 0.2 } }}
                    className="text-24 md:text-32"
                >
                    {schema == "Data" ? "widgets" : "settings"}
                </Icon>
                <Typography
                    component={motion.span}
                    initial={{ x: -20 }}
                    animate={{ x: 0, transition: { delay: 0.2 } }}
                    delay={300}
                    className="text-16 md:text-24 mx-12 font-semibold"
                >
                    {t(table)}
                </Typography>
            </div>

            <div className="flex flex-1 items-center justify-center px-12">
                <ThemeProvider theme={mainTheme}>
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
                        <Icon color="action">search</Icon>

                        <Input
                            placeholder="Search"
                            className="flex flex-1 mx-8"
                            disableUnderline
                            fullWidth
                            value={searchText[table]}
                            inputProps={{
                                "aria-label": "Search",
                            }}
                            onChange={(ev) =>
                                dispatch(tableSlice.actions.setSearchText(ev))
                            }
                        />
                    </Paper>
                </ThemeProvider>
            </div>
        </div>
    );
};

export default TableDesignHeader;
