import FuseLoading from "@fuse/core/FuseLoading";
import { CircularProgress } from "@mui/material";
import Button from "@mui/material/Button";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogTitle from "@mui/material/DialogTitle";
import { DynamicElement } from "app/shared-components/dynamic-element/DynamicElement";
import { DataUtils } from "app/utils/data";
import PropTypes from "prop-types";
import * as React from "react";
import { useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { useDispatch, useSelector, useStore } from "react-redux";
import * as yup from "yup";
import { TableService } from "../TableService";

export const CreateEditTableColumnDialog = ({
    schema,
    table,
    onClose,
    open,
    ...other
}) => {
    //const { onClose, value: valueProp, open, ...other } = props;

    const dispatch = useDispatch();

    const statePath = `${schema}_${table}`;
    const tableSlice = useSelector(
        ({ apiraiser }) => apiraiser.slices[statePath]
    );

    const { t } = useTranslation("apiraiserTranslations");
    const [isCreating, setCreating] = React.useState(false);
    const [isEditing, setEditing] = React.useState(false);
    const schemaContext = { loaded: false };

    const defaultValues = {
        Name: "",
        Datatype: -1,
        IsRequired: false,
        IsUnique: false,
        IsForeignKey: false,
        ForeignSchema: "",
        ForeignTable: "",
        ForeignName: "",
        DefaultValue: "",
    };

    const {
        register,
        control,
        setValue,
        formState,
        handleSubmit,
        reset,
        trigger,
        setError,
    } = useForm({
        mode: "onChange",
        defaultValues: defaultValues,
        context: schemaContext,
        //  resolver: yupResolver(schema), // use this method if you have a static schema
        resolver: async (data, context) => {
            return await DataUtils.Resolve(data, context, formSchema);
        },
    });

    const { isValid, dirtyFields, errors } = formState;

    const addColumnState = useSelector(
        (state) => state[statePath].addColumnState
    );

    const store = useStore();

    const columns = DataUtils.GetDesignColumns();
    const formSchema = yup.object().shape(DataUtils.GenerateSchema(columns));
    const selectableElements = [
        "Datatype",
        "ForeignSchema",
        "ForeignTable",
        "ForeignName",
    ];
    const [selectOptions, setSelectOptions] = React.useState({
        Datatype: DataUtils.DataTypesOptions,
        ForeignSchema: DataUtils.SchemaOptions,
        ForeignTable: [],
        ForeignName: [],
    });

    const [foreignSchema, setForeignSchema] = React.useState(null);
    const [foreignTable, setForeignTable] = React.useState(null);
    const [foreignStatePath, setForeignStatePath] = React.useState(null);

    const foreignTablesColumns = useSelector((state) =>
        foreignSchema && foreignTable
            ? state[`${foreignSchema}_${foreignTable}`].columns
            : []
    );
    const foreignTablesColumnsState = useSelector((state) =>
        foreignSchema && foreignTable
            ? state[`${foreignSchema}_${foreignTable}`].columnsState
            : "waiting"
    );

    const accessibleTables = useSelector(
        ({ apiraiser }) => apiraiser.accessibleTables
    );

    React.useEffect(() => {
        if (foreignSchema) {
            let newSelectOptions = {};
            Object.assign(newSelectOptions, selectOptions);
            newSelectOptions["ForeignTable"] = accessibleTables[
                foreignSchema
            ].map((x) => {
                return {
                    label: x,
                    value: x,
                };
            });
            setSelectOptions(newSelectOptions);
        } else {
            setSelectOptions(selectOptions);
        }
    }, [foreignSchema]);

    React.useEffect(() => {
        if (foreignSchema && foreignTable) {
            setForeignStatePath(`${foreignSchema}_${foreignTable}`);
        } else {
            setForeignStatePath(null);
        }
    }, [foreignTable]);

    React.useEffect(() => {
        if (foreignStatePath) {
            let foreignState = store.getState()[foreignStatePath];
            if (
                !foreignState ||
                !foreignState.columns ||
                !foreignState.columns.length
            ) {
                dispatch(
                    store
                        .getState()
                        .apiraiser.slices[foreignStatePath].thunks.getColumns()
                );
            }
        }
    }, [foreignStatePath]);

    React.useEffect(() => {
        if (
            foreignSchema &&
            foreignTable &&
            foreignTablesColumns &&
            foreignTablesColumns.length
        ) {
            let newSelectOptions = {};
            Object.assign(newSelectOptions, selectOptions);
            newSelectOptions["ForeignName"] = foreignTablesColumns.map((x) => {
                return {
                    label: x.Name,
                    value: x.Name,
                };
            });
            setSelectOptions(newSelectOptions);
        }
    }, [foreignSchema, foreignTable, foreignTablesColumns]);

    React.useEffect(() => {
        if (addColumnState === "loading") {
            setCreating(true);
        } else if (addColumnState === "success" && isCreating) {
            onClose();
            dispatch(tableSlice.thunks.getColumns());
        }
    }, [addColumnState]);

    const onSubmit = (model) => {
        const postData = DataUtils.EncodeData(model, columns);
        dispatch(tableSlice.thunks.addColumn(postData));
    };

    const handleCancel = () => {
        onClose();
    };

    const handleChange = (event) => {
        setValue(event.target.value);
    };

    const onChange = (v) => {
        if (v.name == "ForeignSchema") {
            setForeignSchema(v.value);
        }

        if (v.name == "ForeignTable") {
            setForeignTable(v.value);
        }
    };

    return (
        <Dialog
            sx={{ "& .MuiDialog-paper": { width: "60%", maxHeight: 420 } }}
            open={open}
            {...other}
        >
            <DialogTitle>
                {defaultValues != null
                    ? t("Update " + table)
                    : t("Add " + table)}
            </DialogTitle>
            <DialogContent
                dividers
                style={{ display: "flex", flexDirection: "column" }}
            >
                {(!columns || !columns.length) && <FuseLoading />}
                {columns && columns.length && (
                    <form
                        name={`createEdit${table}Form`}
                        noValidate
                        className="flex flex-col justify-center w-full"
                        onSubmit={handleSubmit(onSubmit)}
                    >
                        {columns &&
                            columns
                                .filter(
                                    (column) =>
                                        !DataUtils.IsColumnPredefined(column)
                                )
                                .map((column) => {
                                    return (
                                        <DynamicElement
                                            key={column.Name}
                                            control={control}
                                            column={column}
                                            errors={errors}
                                            disabled={isCreating || isEditing}
                                            isSelect={selectableElements.includes(
                                                column.Name
                                            )}
                                            selectOptions={
                                                selectOptions[column.Name]
                                            }
                                            onChange={onChange}
                                        ></DynamicElement>
                                    );
                                })}
                        <DialogActions>
                            <Button
                                autoFocus
                                variant="outlined"
                                type="button"
                                onClick={handleCancel}
                                color="primary"
                                disabled={isCreating || isEditing}
                            >
                                {t("CANCEL")}
                            </Button>
                            <Button
                                disabled={isCreating || isEditing}
                                type="submit"
                                variant="outlined"
                                color="success"
                            >
                                {(isCreating || isEditing) && (
                                    <CircularProgress size={"2rem"} />
                                )}
                                {t("SAVE")}
                            </Button>

                            {/* <Button type="submit" disabled={isCreating || isEditing}>Ok</Button> */}
                        </DialogActions>
                        {/* <Button type="submit">Ok</Button> */}
                    </form>
                )}
            </DialogContent>
        </Dialog>
    );
};

CreateEditTableColumnDialog.propTypes = {
    onClose: PropTypes.func.isRequired,
    open: PropTypes.bool.isRequired,
};
