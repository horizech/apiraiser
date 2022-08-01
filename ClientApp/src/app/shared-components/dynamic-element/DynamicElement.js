// import { ReactSelectAdapter } from "../adapters";
import {
    Autocomplete,
    Button,
    Checkbox,
    Chip,
    InputLabel,
    MenuItem,
    Select,
} from "@mui/material";
import TextField from "@mui/material/TextField";
import { DataUtils } from "app/utils/data";
import clsx from "clsx";
import React, { useEffect, useState } from "react";
import { Controller, useForm } from "react-hook-form";
// import { isColumnPredefined, history } from '../helpers';
// import { Form, Field } from 'react-final-form'
import "./DynamicElement.css";
import { DynamicImageElement } from "./DynamicImageElement";

export const DynamicElement = ({
    input,
    column,
    control,
    disabled,
    errors,
    isWorking,
    onChange,
    onFocus,
    onBlur,
    label,
    isSelect,
    selectOptions,
}) => {
    const { register, watch, setValue } = useForm({
        mode: "onChange",
        defaultValues: "",
    });
    const [fieldValue, setFieldValue] = useState([]);

    //   const updateInterArray(field, value) {
    //     field.onChange(value ? value.map((x) => `${x}`).join(",") : "";

    //     result[key] = data[key].split(",").map((x) => parseInt("" + x.trim()));

    //   }

    const getElement = () => {
        if (isSelect) {
            return (
                <div className={DataUtils.GetInputWidth(column)}>
                    <Controller
                        name={column.Name}
                        control={control}
                        render={({ field }) => (
                            <>
                                <InputLabel id={column.Name}>
                                    {column.Name}
                                </InputLabel>
                                <Select
                                    {...field}
                                    className="mb-16"
                                    autoFocus
                                    fullWidth
                                    type="number"
                                    error={!!errors[column.Name]}
                                    helperText={errors[column.Name]?.message}
                                    variant="outlined"
                                    required={column.IsRequired}
                                    disabled={disabled}
                                    onChange={(e, v) => {
                                        e.preventDefault();
                                        field.onChange(e);
                                        if (
                                            onChange &&
                                            v.props &&
                                            v.props.value
                                        ) {
                                            onChange({
                                                name: column.Name,
                                                value: v.props.value,
                                            });
                                        }
                                    }}
                                >
                                    {selectOptions &&
                                        selectOptions.length &&
                                        selectOptions.map((option) => (
                                            <MenuItem value={option.value}>
                                                {option.label}
                                            </MenuItem>
                                        ))}
                                </Select>
                            </>
                        )}
                    />
                </div>
            );

            /*
      return (
        <div className={DataUtils.GetInputWidth(column)}>
          <label htmlFor={column.Name}>{(label || column.Name) + (column.IsRequired ? ' *' : '')}</label>
          <ReactSelectAdapter input={input} options={selectOptions}/>
        </div>
      )
      */
        } else {
            switch (column.Datatype) {
                case "Integer":
                    return (
                        <div className={DataUtils.GetInputWidth(column)}>
                            <Controller
                                name={column.Name}
                                control={control}
                                render={({ field }) => (
                                    <TextField
                                        {...field}
                                        className="mb-16"
                                        label={column.Name}
                                        autoFocus
                                        type="number"
                                        error={!!errors[column.Name]}
                                        helperText={
                                            errors[column.Name]?.message
                                        }
                                        variant="outlined"
                                        required={column.IsRequired}
                                        fullWidth
                                        disabled={disabled}
                                    />
                                )}
                            />
                        </div>
                    );
                default:
                case "LongText":
                    return (
                        <div className={DataUtils.GetInputWidth(column)}>
                            <Controller
                                name={column.Name}
                                control={control}
                                render={({ field }) => {
                                    return (
                                        <TextField
                                            {...field}
                                            className="mb-16"
                                            label={column.Name}
                                            autoFocus
                                            type="text"
                                            error={!!errors[column.Name]}
                                            helperText={
                                                errors[column.Name]?.message
                                            }
                                            variant="outlined"
                                            required={column.IsRequired}
                                            fullWidth
                                            disabled={disabled}
                                            multiline={true}
                                            rows={5}
                                        />
                                    );
                                }}
                            />
                        </div>
                    );
                case "ShortText":
                    return (
                        <div className={DataUtils.GetInputWidth(column)}>
                            <Controller
                                name={column.Name}
                                control={control}
                                render={({ field }) => {
                                    return (
                                        <TextField
                                            {...field}
                                            className="mb-16"
                                            label={column.Name}
                                            autoFocus
                                            type="text"
                                            error={!!errors[column.Name]}
                                            helperText={
                                                errors[column.Name]?.message
                                            }
                                            variant="outlined"
                                            required={column.IsRequired}
                                            fullWidth
                                            disabled={disabled}
                                        />
                                    );
                                }}
                            />
                        </div>
                    );
                //   value={field.value.split(",").map((id) => id)}
                //   onChange={(event, newValue) => {
                //     setValue(
                //       "idLabels",
                //       newValue.map((item) => item.id)
                //     );
                //   }}
                case "IntegerArray":
                    return (
                        <div className={DataUtils.GetInputWidth(column)}>
                            <Controller
                                name={column.Name}
                                control={control}
                                render={({ field }) => {
                                    let defaultValues = field.value
                                        ? field.value.split(",")
                                        : [];
                                    return (
                                        <Autocomplete
                                            className="mt-8 mb-16"
                                            multiple
                                            freeSolo
                                            options={defaultValues}
                                            getOptionLabel={(label) => {
                                                return label;
                                            }}
                                            value={defaultValues}
                                            onChange={(e, v) => {
                                                e.preventDefault();
                                                field.onChange(v.join(","));

                                                let value = v.join(",");
                                                if (onChange) {
                                                    onChange({
                                                        name: column.Name,
                                                        value: value,
                                                    });
                                                }
                                            }}
                                            renderTags={(value, getTagProps) =>
                                                value.map((option, index) => {
                                                    return (
                                                        <Chip
                                                            label={option}
                                                            {...getTagProps({
                                                                index,
                                                            })}
                                                            className={clsx(
                                                                "m-3"
                                                            )}
                                                        />
                                                    );
                                                })
                                            }
                                            renderInput={(params) => {
                                                return (
                                                    <TextField
                                                        {...params}
                                                        placeholder="Select multiple Labels"
                                                        label="Labels"
                                                        variant="outlined"
                                                        type="number"
                                                        InputLabelProps={{
                                                            shrink: true,
                                                        }}
                                                    />
                                                );
                                            }}
                                        />
                                    );
                                }}
                            />
                            {/* <Controller
                name={column.Name}
                control={control}
                render={({ field }) => {
                  console.log("field", field);
                  return (
                    <TextField
                      {...field}
                      className="mb-16"
                      multiple
                      freeSolo
                      label={column.Name}
                      autoFocus
                      type="text"
                      error={!!errors[column.Name]}
                      helperText={errors[column.Name]?.message}
                      variant="outlined"
                      required={column.IsRequired}
                      fullWidth
                      disabled={disabled}
                      renderTags={(field, getTagProps) =>
                        field.value.map((a, index) => {
                          return (
                            <Chip
                              label={field.a}
                              {...getTagProps({ index })}
                              className={clsx("m-3", option.class)}
                            />
                          );
                        })
                      }
                      renderInput={(params) => (
                        <TextField
                          {...params}
                          placeholder="Select multiple Labels"
                          label="Labels"
                          variant="outlined"
                          InputLabelProps={{
                            shrink: true,
                          }}
                        />
                      )}
                    />
                  );
                }}
              /> */}
                        </div>
                    );

                case "DateTime":
                    return (
                        <div className={DataUtils.GetInputWidth(column)}>
                            <Controller
                                name={column.Name}
                                control={control}
                                render={({ field }) => (
                                    <TextField
                                        {...field}
                                        className="mb-16"
                                        label={column.Name}
                                        autoFocus
                                        type="datetime-local"
                                        error={!!errors[column.Name]}
                                        helperText={
                                            errors[column.Name]?.message
                                        }
                                        variant="outlined"
                                        fullWidth
                                        disabled={disabled}
                                    />
                                )}
                            />
                        </div>
                    );
                case "Boolean":
                    return (
                        <div className={DataUtils.GetInputWidth(column)}>
                            <Controller
                                name={column.Name}
                                control={control}
                                render={({ field }) => (
                                    <div style={{ display: "flex" }}>
                                        <Checkbox
                                            className="mb-16"
                                            {...field}
                                            autoFocus
                                            error={!!errors[column.Name]}
                                            helperText={
                                                errors[column.Name]?.message
                                            }
                                            variant="outlined"
                                            required={column.IsRequired}
                                            fullWidth
                                            checked={column.ischecked}
                                            disabled={disabled}
                                        />
                                        <span style={{ marginTop: "12px" }}>
                                            {column.Name}
                                        </span>
                                    </div>
                                )}
                            />
                            {/* <div className="element-boolean">
                <label htmlFor={column.Name}>
                  <input
                    {...input}
                    id={column.Name}
                    type="checkbox"
                    disabled={isWorking || isColumnPredefined(column)}
                    // onChange={(e) => { input.onChange(e); if (onChange) { onChange(e); }}}
                    // onFocus={(e) => { input.onFocus(e); if (onFocus) { onFocus(e); }}}
                    // onBlur={(e) => { input.onBlur(e); if (onBlur) { onBlur(e); }}}
                  />
                  <span
                    className={
                      input.checked
                        ? "element-boolean-checked"
                        : "element-boolean-unchecked"
                    }
                  ></span>
                  {(label || column.Name) + (column.IsRequired ? " *" : "")}
                </label>
              </div> */}
                        </div>
                    );
                case "Image":
                    return (
                        <DynamicImageElement
                            control={control}
                            column={column}
                            disabled={disabled}
                            errors={errors}
                        ></DynamicImageElement>
                    );
            }
        }
    };

    if (column) {
        return getElement();
    } else {
        return (
            <p>
                <em>Loading...</em>
            </p>
        );
    }
};
// class DynamicElement extends Component {
//   static displayName = DynamicElement.name;

//   constructor(props) {
//     super(props);
//     this.handleChange = this.handleChange.bind(this);
//     this.getDefaultValue = this.getDefaultValue.bind(this);

//     if (this.props.column) {
//       this.state = {
//         checked: this.props.value || this.getDefaultValue(),
//         value: this.props.value || this.getDefaultValue()
//       };
//       //   console.log("state value");
//       //   console.log(this.getDefaultValue());
//       //   console.log("state value");

//       //   if (this.props.column.DataType === 'Boolean') {
//       //     this.props.handleChange({ name: this.props.column.Name, value: this.state.checked });
//       //   }
//       //   else {
//       //     this.props.handleChange({ name: this.props.column.Name, value: this.state.value });
//       //   }
//     }
//   }

//   // componentDidMount() {
//   // }

//   handleChange(e) {
//     const { name, value, checked } = e.target;
//     if (this.props.column.Datatype === "Boolean") {
//       this.setState({ checked: checked });
//       this.props.handleChange({ name, value: checked });
//     }
//     else {
//       this.setState({ value: value });
//       this.props.handleChange({ name, value });
//     }
//   }

//   getElement() {
//     // To check if fonts are available.
//     // document.fonts.ready.then(function () {
//     //   console.log('All fonts in use by visible text have loaded.');
//     //   console.log('Nucleo Outline loaded? ' + document.fonts.check("1em 'Nucleo Outline'"));  // false
//     // });

//     const { input, column, isWorking, onChange, onFocus, onBlur, label, isSelect, selectOptions } = this.props;
//     if(isSelect) {
//       return (
//         <div className={this.DataUtils.GetInputWidth(column)}>
//           <label htmlFor={column.Name}>{(label || column.Name) + (column.IsRequired ? ' *' : '')}</label>
//           <ReactSelectAdapter input={input} options={selectOptions}/>
//         </div>
//       )
//     }
//     else {
//       switch (column.Datatype) {
//         case "Integer": return (
//           <div className={this.DataUtils.GetInputWidth(column)}>
//             <div className={'form-group'}>
//               <label htmlFor={column.Name}>{(label || column.Name) + (column.IsRequired ? ' *' : '')}</label>
//               <input className="form-control" {...input} id={column.Name}  type="number" required={column.IsRequired} disabled={ this.props.isWorking || isColumnPredefined(column)}
//                 // onChange={(e) => { input.onChange(e); if (onChange) { onChange(e); }}}
//                 // onFocus={(e) => { input.onFocus(e); if (onFocus) { onFocus(e); }}}
//                 // onBlur={(e) => { input.onBlur(e); if (onBlur) { onBlur(e); }}}
//               />
//             </div>
//           </div>
//         );
//         default: case "LongText": case "ShortText": return (
//           <div className={this.DataUtils.GetInputWidth(column)}>
//             <div className={'form-group'}>
//               <label htmlFor={column.Name}>{(label || column.Name) + (column.IsRequired ? ' *' : '')}</label>
//               <input className="form-control" {...input} id={column.Name}  type="text" required={column.IsRequired} disabled={ this.props.isWorking || isColumnPredefined(column)}
//                 // onChange={(e) => { input.onChange(e); if (onChange) { onChange(e); }}}
//                 // onFocus={(e) => { input.onFocus(e); if (onFocus) { onFocus(e); }}}
//                 // onBlur={(e) => { input.onBlur(e); if (onBlur) { onBlur(e); }}}
//               />
//             </div>
//           </div>
//           // <div className={this.DataUtils.GetInputWidth(column)}>
//           //   <div className={'form-group'}>
//           //     <label htmlFor={column.Name}>{(label || column.Name) + (column.IsRequired ? ' *' : '')}</label>
//           //     <input type="text" required={column.IsRequired} disabled={ this.props.isWorking || getIsReadOnly(column)} className="form-control" value={this.state.value} placeholder={column.Name} onChange={this.handleChange} />
//           //   </div>
//           // </div>
//         );
//         case "DateTime": return (
//           <div className={this.DataUtils.GetInputWidth(column)}>
//             <div className={'form-group'}>
//               <label htmlFor={column.Name}>{(label || column.Name) + (column.IsRequired ? ' *' : '')}</label>
//               <input className="form-control" {...input} id={column.Name}  type="date" required={column.IsRequired} disabled={ this.props.isWorking || isColumnPredefined(column)}
//                 // onChange={(e) => { input.onChange(e); if (onChange) { onChange(e); }}}
//                 // onFocus={(e) => { input.onFocus(e); if (onFocus) { onFocus(e); }}}
//                 // onBlur={(e) => { input.onBlur(e); if (onBlur) { onBlur(e); }}}
//               />
//             </div>
//           </div>
//         );
//         case "Boolean": return (
//           <div className={this.DataUtils.GetInputWidth(column)}>
//             <div className="element-boolean">
//               <label htmlFor={column.Name}>
//                 <input {...input} id={column.Name} type="checkbox" disabled={ this.props.isWorking || isColumnPredefined(column)}
//                   // onChange={(e) => { input.onChange(e); if (onChange) { onChange(e); }}}
//                   // onFocus={(e) => { input.onFocus(e); if (onFocus) { onFocus(e); }}}
//                   // onBlur={(e) => { input.onBlur(e); if (onBlur) { onBlur(e); }}}
//                 />
//                 <span className={input.checked ? 'element-boolean-checked': 'element-boolean-unchecked'}></span>{(label || column.Name) + (column.IsRequired ? ' *' : '')}
//               </label>
//             </div>
//           </div>
//         );

//       }

//     }
//   }

//   render() {
//     if (this.props.column) {
//       return (
//         this.getElement()
//       )
//     }
//     else {
//       return (
//         <p>
//           <em>Loading...</em>
//         </p>
//       );
//     }
//   }

// }
