import { CircularProgress } from "@mui/material";
import Button from "@mui/material/Button";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogTitle from "@mui/material/DialogTitle";
import PropTypes from "prop-types";
import * as React from "react";
import { useTranslation } from "react-i18next";
import { useDispatch, useSelector } from "react-redux";
import { TableService } from "../TableService";

export const DeleteTableColumnDialog = ({
    schema,
    table,
    onClose,
    name,
    open,
    ...other
}) => {
    const dispatch = useDispatch();

    const statePath = `${schema}_${table}`;
    const tableSlice = useSelector(
        ({ apiraiser }) => apiraiser.slices[statePath]
    );

    const deleteTableColumn = () => {
        dispatch(tableSlice.thunks.removeColumn(name));
    };
    const { t } = useTranslation("apiraiserTranslations");
    const [isDeleting, setDeleting] = React.useState(false);

    const removeColumnState = useSelector(
        (state) => state[statePath].removeColumnState
    );

    React.useEffect(() => {
        if (removeColumnState === "loading") {
            setDeleting(true);
        } else if (removeColumnState === "success" && isDeleting) {
            onClose();
            dispatch(tableSlice.thunks.getColumns());
        }
    }, [removeColumnState]);

    return (
        <Dialog
            sx={{ "& .MuiDialog-paper": { width: "30%", maxHeight: 400 } }}
            maxWidth="sm"
            open={open}
            {...other}
        >
            <DialogTitle>{t("Delete " + table)}</DialogTitle>
            <DialogContent
                dividers
                style={{ display: "flex", flexDirection: "column" }}
            >
                <p>{t("Are_You_Sure_you_want_to_delete")}</p>
                <DialogActions>
                    <Button autoFocus onClick={onClose} disabled={isDeleting}>
                        {t("NO")}
                    </Button>
                    <Button onClick={deleteTableColumn} disabled={isDeleting}>
                        {isDeleting && <CircularProgress size={"2rem"} />}
                        {t("YES")}
                    </Button>
                </DialogActions>
            </DialogContent>
        </Dialog>
    );
};

DeleteTableColumnDialog.propTypes = {
    name: PropTypes.string.isRequired,
    onClose: PropTypes.func.isRequired,
    open: PropTypes.bool.isRequired,
};
