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

export const CreateEditTableEntityDialog = ({
    schema,
    table,
    onClose,
    data,
    open,
    ...other
}) => {
    //const { onClose, value: valueProp, open, ...other } = props;

    const dispatch = useDispatch();
    const store = useStore();
    const statePath = `${schema}_${table}`;

    const tableSlice = useSelector(
        ({ apiraiser }) => apiraiser.slices[statePath]
    );

    const { t } = useTranslation("apiraiserTranslations");
    const [isCreating, setCreating] = React.useState(false);
    const [isEditing, setEditing] = React.useState(false);
    const [formSchema, setFormSchema] = React.useState();
    const [schemaContext, setSchemaContext] = React.useState({ loaded: false });
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
        // defaultValues: {},
        context: schemaContext,
        //  resolver: yupResolver(schema), // use this method if you have a static schema
        resolver: async (data, context) => {
            return await DataUtils.Resolve(data, context, formSchema);
        },
    });

    const { isValid, dirtyFields, errors } = formState;

    const [selectableElements, setSelectableElements] = React.useState([]);
    const [selectOptions, setSelectOptions] = React.useState({});

    const createState = useSelector((state) => state[statePath].createState);

    const editState = useSelector((state) => state[statePath].editState);

    const columns = useSelector((state) => state[statePath].columns);

    const dataState = useSelector((state) => state);

    const [columnInfo, setColumnInfo] = React.useState(null);

    React.useEffect(() => {
        if (data) {
            reset(data);
        }
    }, [data]);

    React.useEffect(() => {
        if (createState === "loading") {
            setCreating(true);
        } else if (createState === "success" && isCreating) {
            onClose();
            dispatch(tableSlice.thunks.getEntities());
        }
    }, [createState]);

    React.useEffect(() => {
        if (editState === "loading") {
            setEditing(true);
        } else if (editState === "success" && isEditing) {
            onClose();
            dispatch(tableSlice.thunks.getEntities());
        }
    }, [editState]);

    React.useEffect(() => {
        let newSelectOptions = {};

        Object.assign(newSelectOptions, selectOptions);

        if (columnInfo != null) {
            columnInfo.map((column) => {
                let foreignStatePath = `${column.ForeignSchema}_${column.ForeignTable}`;
                let foreignTable = dataState[foreignStatePath];
                if (
                    column != null &&
                    foreignTable != null &&
                    foreignTable.entitiesState === "success"
                ) {
                    newSelectOptions[column.Name] = Object.values(
                        foreignTable.entities
                    ).map((x) => {
                        return {
                            label:
                                x["Name"] ||
                                x["Fullname"] ||
                                x["Username"] ||
                                x["Label"],
                            value: x[column.ForeignName],
                        };
                    });
                }
            });
        }

        setSelectOptions(newSelectOptions);
    }, [dataState]);

    React.useEffect(() => {
        if (columnInfo != null) {
            columnInfo.map((column) => {
                let foreignStatePath = `${column.ForeignSchema}_${column.ForeignTable}`;
                if (foreignStatePath) {
                    let foreignState = store.getState()[foreignStatePath];
                    if (
                        !foreignState ||
                        !foreignState.entities ||
                        !foreignState.entities.length
                    ) {
                        dispatch(
                            store
                                .getState()
                                .apiraiser.slices[
                                    foreignStatePath
                                ].thunks.getEntities()
                        );
                    }
                }
            });
        }
    }, [columnInfo]);

    React.useEffect(() => {
        if (columns && columns.length) {
            setFormSchema(
                yup.object().shape(DataUtils.GenerateSchema(columns))
            );

            let foreignColumns = columns
                .filter((x) => x.IsForeignKey)
                .map((x) => x.Name);
            setColumnInfo(columns.filter((x) => x.IsForeignKey).map((x) => x));
            setSelectableElements(foreignColumns);
        }
    }, columns);

    const onSubmit = (model) => {
        const postData = DataUtils.EncodeData(model, columns);
        if (data && data.Id) {
            dispatch(tableSlice.thunks.editEntity({ id: data.Id, postData }));
        } else {
            dispatch(tableSlice.thunks.createEntity(postData));
        }
    };

    const handleCancel = () => {
        onClose();
    };

    const handleChange = (event) => {
        setValue(event.target.value);
    };

    return (
        <Dialog
            sx={{ "& .MuiDialog-paper": { width: "60%", maxHeight: 420 } }}
            open={open}
            {...other}
        >
            <DialogTitle>
                {data != null ? t("Update " + table) : t("Add " + table)}
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

CreateEditTableEntityDialog.propTypes = {
    onClose: PropTypes.func.isRequired,
    open: PropTypes.bool.isRequired,
};
