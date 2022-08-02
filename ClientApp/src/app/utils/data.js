import * as yup from "yup";

const EncodeData = (data, columnsArr) => {
    let result = {};
    let columns = {};
    columnsArr.forEach((x) => {
        columns[x.Name] = x;
    });

    let keys = Object.keys(data);
    keys.forEach((key) => {
        switch (columns[key].Datatype) {
            case "Integer": {
                result[key] = parseInt("" + data[key]);
                break;
            }
            case "IntegerArray": {
                result[key] = data[key]
                    .split(",")
                    .map((x) => parseInt("" + x.trim()));
                break;
            }
            case "Decimal":
            case "Float":
            case "Money": {
                result[key] = parseFloat("" + data[key]);
                break;
            }
            case "Boolean": {
                result[key] =
                    !data[key] || data[key].toLowerCase() === "false"
                        ? false
                        : true;
                break;
            }
            case "BooleanArray": {
                result[key] = data[key]
                    .split(",")
                    .map((x) =>
                        !x || x.toLowerCase() === "false" ? false : true
                    );
                break;
            }
            case "Image": {
                if (data[key]) {
                    result[key] = data[key].split(",").map((x) => parseInt(x));
                } else {
                    result[key] = null;
                }
                break;
            }
            default: {
                result[key] = data[key];
            }
        }
    });

    return result;
};

const DecodeData = (data, columnsArr) => {
    let result = {};
    let columns = {};
    columnsArr.forEach((x) => {
        columns[x.Name] = x;
    });

    let keys = Object.keys(data);
    keys.forEach((key) => {
        console.log(key);
        console.log("data", data);
        switch (columns[key].Datatype) {
            case "Integer":
            case "Decimal":
            case "Float": {
                result[key] = `${data[key]}`;
                break;
            }
            case "IntegerArray":
            case "Image": {
                result[key] = data[key]
                    ? data[key].map((x) => `${x}`).join(",")
                    : "";
                break;
            }
            case "Boolean": {
                result[key] = data[key];
                break;
            }
            default: {
                result[key] = data[key];
            }
        }
    });

    return result;
};

const IsColumnPredefined = (column) => {
    switch (column.Name) {
        case "Id":
        case "CreatedOn":
        case "CreatedBy":
        case "LastUpdatedOn":
        case "LastUpdatedBy":
            return true;
        default:
            return false;
    }
};

const GetInputWidth = (column) => {
    switch (column.Datatype) {
        case "Integer":
            return " col-md-4";
        default:
        case "ShortText":
            return " col-md-4";
        case "ExtraLongText":
            return " col-md-12";
        case "LongText":
            return " col-md-8";
        case "DateTime":
            return " col-md-4";
        case "Boolean":
            return " col-md-4";
    }
};

const GetInputDefaultValue = (column) => {
    if (column) {
        switch (column.Datatype) {
            case "Integer":
                return 0;
            default:
            case "ShortText":
                return "";
            case "LongText":
                return "";
            case "DateTime":
                return ""; //new Date();
            case "Boolean":
                return false;
        }
    } else {
        return undefined;
    }
};

const GetDesignColumns = () => {
    return [
        {
            Name: "Name",
            Datatype: "ShortText",
            IsRequired: true,
        },
        {
            Name: "Datatype",
            Datatype: "Integer",
            IsRequired: true,
        },
        {
            Name: "IsRequired",
            Datatype: "Boolean",
            IsRequired: false,
        },
        {
            Name: "IsUnique",
            Datatype: "Boolean",
            IsRequired: false,
        },
        {
            Name: "IsForeignKey",
            Datatype: "Boolean",
            IsRequired: false,
        },
        {
            Name: "ForeignSchema",
            Datatype: "ShortText",
            IsRequired: false,
        },
        {
            Name: "ForeignTable",
            Datatype: "ShortText",
            IsRequired: false,
        },
        {
            Name: "ForeignName",
            Datatype: "ShortText",
            IsRequired: false,
        },
        {
            Name: "DefaultValue",
            Datatype: "ShortText",
            IsRequired: false,
        },
    ];
};

const DataTypes = {
    Integer: 0,
    IntegerArray: 1,
    Decimal: 2,
    Float: 3,
    Boolean: 4,
    BooleanArray: 5,
    DateTime: 6,
    Json: 7,
    JsonArray: 8,
    Money: 9,
    ShortText: 10,
    LongText: 11,
    Image: 12,
};

const DataTypesOptions = [
    { label: "Nothing", value: -1 },
    { label: "Integer", value: 0 },
    { label: "Integer Array", value: 1 },
    { label: "Decimal", value: 2 },
    { label: "Float", value: 3 },
    { label: "Boolean", value: 4 },
    { label: "Boolean Array", value: 5 },
    { label: "DateTime", value: 6 },
    { label: "Json", value: 7 },
    { label: "Json Array", value: 8 },
    { label: "Money", value: 9 },
    { label: "Short Text", value: 10 },
    { label: "Long Text", value: 11 },
    { label: "Image", value: 12 },
];

const SchemaOptions = [
    { label: "Data", value: "Data" },
    { label: "Administration", value: "Administration" },
];
const Resolve = async (data, context, schema) => {
    let validationResult = {
        values: {},
        errors: {},
    };

    if (!schema) {
        return validationResult;
    }

    try {
        validationResult.values = await schema.validate(data, {
            abortEarly: false,
        });
    } catch (e) {
        e.errors.forEach((error) => {
            Object.keys(error).forEach((key) => {
                validationResult.errors[key] = {
                    message: error[key],
                    type: "required",
                };
            });
        });
    }
    return validationResult;
};

const GenerateSchema = (columns) => {
    // const schema = yup.object().shape({
    //   Name: yup.string().required("You must enter a Name"),
    //   Description: yup
    //     .string()
    //     .required("Please enter Description.")
    //     .min(8, "Description is too short - should be 8 chars minimum."),
    // });

    let schemaProps = {};
    columns
        .filter((column) => !DataUtils.IsColumnPredefined(column))
        .forEach((column) => {
            let requiredError = {};
            let yupValidator = null;
            if (column.DataType == "Integer") {
                yupValidator = yup.number();
            } else {
                yupValidator = yup.string();
            }
            if (column.IsRequired) {
                requiredError[column.Name] = `You must enter ${column.Name}`;
                yupValidator = yupValidator.required(requiredError);
            }
            schemaProps[column.Name] = yupValidator;
        });
    return schemaProps;
};

export const DataUtils = {
    EncodeData: EncodeData,
    DecodeData: DecodeData,
    IsColumnPredefined: IsColumnPredefined,
    GetInputWidth: GetInputWidth,
    GetInputDefaultValue: GetInputDefaultValue,
    GetDesignColumns: GetDesignColumns,
    DataTypes: DataTypes,
    DataTypesOptions: DataTypesOptions,
    SchemaOptions: SchemaOptions,
    Resolve: Resolve,
    GenerateSchema: GenerateSchema,
};
