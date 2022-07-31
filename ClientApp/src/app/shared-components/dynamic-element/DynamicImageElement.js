// import { ReactSelectAdapter } from "../adapters";
import { Button, Checkbox } from "@mui/material";
import TextField from "@mui/material/TextField";
import { DataUtils } from "app/utils/data";
import React, { useEffect, useState } from "react";
import { Controller } from "react-hook-form";
// import { isColumnPredefined, history } from '../helpers';
// import { Form, Field } from 'react-final-form'
import "./DynamicElement.css";

export const DynamicImageElement = ({ column, control, disabled, errors }) => {
    const [byteArray, setByteArray] = useState(null);
    const [selectedFile, setSelectedFile] = useState(null);
    const [selectedFileName, setSelectedFileName] = useState(null);
    // const [preview, setPreview] = useState(null);
    const inputFileRef = React.useRef();

    const toBase64 = (file) =>
        new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.readAsDataURL(file);
            reader.onload = () => resolve(reader.result);
            reader.onerror = (error) => reject(error);
        });

    const dataImgStringToBase64 = (data) => {
        if (data && data.length > 0) {
            var value = null;
            if (!Array.isArray(data)) {
                value = data.split(",").map((x) => parseInt(x));
            } else {
                value = data;
            }
            // console.log(value);

            // var u8 = new Uint8Array(value);
            // var b64encoded = btoa(String.fromCharCode.apply(null, u8));

            var b64encoded = window.btoa(
                new Uint8Array(value).reduce(function (data, byte) {
                    return data + String.fromCharCode(byte);
                }, "")
            );

            // var b64encoded = Buffer.from(value, "latin1").toString("base64");

            return b64encoded;
        } else {
            return null;
        }
    };

    const clearImage = (field) => {
        setSelectedFile(null);
        setSelectedFileName(null);
        field.onChange("");
    };

    const processImage = async (e, field) => {
        if (!e.target.files || e.target.files.length === 0) {
            setSelectedFile(undefined);
            return;
        }

        // I've kept this example simple by using the first image instead of multiple
        setSelectedFile(e.target.files[0]);
        setSelectedFileName(e.target.files[0].name);

        // let base64Data = await toBase64(e.target.files[0]);
        // console.log("Base 64: ", base64Data);

        var reader = new FileReader();
        reader.onload = function () {
            let arrayBuffer = this.result;
            let array = new Uint8Array(arrayBuffer);
            //let binaryString = String.fromCharCode.apply(null, array);
            field.onChange(array.toString());
            //console.log(array.map((x) => x.toString()).join(","));
            // setByteArray(array);
        };
        reader.readAsArrayBuffer(e.target.files[0]);
    };

    // create a preview as a side effect, whenever selected file is changed
    // useEffect(() => {
    //     if (!selectedFile) {
    //         setPreview(undefined);
    //         return;
    //     }

    //     const objectUrl = URL.createObjectURL(selectedFile);
    //     setPreview(objectUrl);

    //     // free memory when ever this component is unmounted
    //     return () => URL.revokeObjectURL(objectUrl);
    // }, [selectedFile]);

    const getElement = () => (
        <>
            <Controller
                name={column.Name}
                control={control}
                render={({ field }) => (
                    <div
                        style={{
                            display: "flex",
                            alignItems: "center",
                            height: "52px",
                            marginBottom: "12px",
                        }}
                    >
                        {column.Name + (column.IsRequired ? " *" : "")}
                        <TextField
                            style={{ width: "0px", display: "none" }}
                            {...field}
                            className="mb-16"
                            label={column.Name}
                            autoFocus
                            hidden
                            type="hidden"
                            error={!!errors[column.Name]}
                            helperText={errors[column.Name]?.message}
                            variant="outlined"
                            fullWidth
                            disabled={disabled}
                        />

                        <input
                            id={column.Name}
                            ref={inputFileRef}
                            hidden
                            type="file"
                            disabled={disabled}
                            onChange={(e) => processImage(e, field)}
                            // onChange={(e) => { input.onChange(e); if (onChange) { onChange(e); }}}
                            // onFocus={(e) => { input.onFocus(e); if (onFocus) { onFocus(e); }}}
                            // onBlur={(e) => { input.onBlur(e); if (onBlur) { onBlur(e); }}}
                        />
                        {/* <span>{selectedFileName}</span> */}

                        {field.value && (
                            <img
                                src={
                                    "data:image/jpg;base64," +
                                    dataImgStringToBase64(field.value)
                                }
                                style={{
                                    marginLeft: "16px",
                                    maxWidth: "48px",
                                    maxHeight: "48px",
                                }}
                            />
                        )}

                        <div style={{ flex: "1 1 auto" }}></div>
                        <Button
                            variant="outlined"
                            color="warning"
                            style={{ width: "128px", height: "32px" }}
                            onClick={() => clearImage(field)}
                        >
                            Clear
                        </Button>
                        <Button
                            variant="outlined"
                            color="success"
                            style={{
                                marginLeft: "8px",
                                width: "128px",
                                height: "32px",
                            }}
                            onClick={() => inputFileRef.current.click()}
                        >
                            Load
                        </Button>

                        {/* <img
                                src={preview}
                                style={{
                                    maxWidth: "64px",
                                    maxHeight: "64px",
                                }}
                            /> */}
                    </div>
                )}
            />
        </>
    );

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
