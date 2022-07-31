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

export const DeleteTableEntityDialog = ({
    schema,
    table,
    onClose,
    id,
    open,
    ...other
}) => {
    const dispatch = useDispatch();

    const statePath = `${schema}_${table}`;
    const tableSlice = useSelector(
        ({ apiraiser }) => apiraiser.slices[statePath]
    );

    const deleteTableEntity = () => {
        dispatch(tableSlice.thunks.deleteEntity(id));
    };
    const { t } = useTranslation("apiraiserTranslations");
    const [isDeleting, setDeleting] = React.useState(false);

    const deleteState = useSelector((state) => state[statePath].deleteState);

    React.useEffect(() => {
        if (deleteState === "loading") {
            setDeleting(true);
        } else if (deleteState === "success" && isDeleting) {
            onClose();
            dispatch(tableSlice.thunks.getEntities());
        }
    }, [deleteState]);

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
                    <Button onClick={deleteTableEntity} disabled={isDeleting}>
                        {isDeleting && <CircularProgress size={"2rem"} />}
                        {t("YES")}
                    </Button>
                </DialogActions>
            </DialogContent>
        </Dialog>
    );
};

DeleteTableEntityDialog.propTypes = {
    id: PropTypes.number.isRequired,
    onClose: PropTypes.func.isRequired,
    open: PropTypes.bool.isRequired,
};
