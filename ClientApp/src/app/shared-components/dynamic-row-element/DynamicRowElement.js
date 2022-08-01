// import { ReactSelectAdapter } from "../adapters";
import { Checkbox } from "@mui/material";
import TextField from "@mui/material/TextField";
import { DataUtils } from "app/utils/data";
import moment from "moment";
import React from "react";
import { Controller } from "react-hook-form";
// import { isColumnPredefined, history } from '../helpers';
// import { Form, Field } from 'react-final-form'
import "./DynamicRowElement.css";

export const DynamicRowElement = ({ column, value, users }) => {
    const getElement = () => {
        switch (column.Datatype) {
            case "Integer": {
                if (
                    (column.Name === "CreatedBy" ||
                        column.Name === "LastUpdatedBy") &&
                    users &&
                    Object.keys(users) &&
                    Object.keys(users).length &&
                    users["" + value]
                ) {
                    return (
                        <span>
                            {users["" + value]["Fullname"] ||
                                users["" + value]["Username"] ||
                                value}
                        </span>
                    );
                } else {
                    return <span>{value}</span>;
                }
            }
            case "Decimal":
            case "Float":
                return <span>{value}</span>;
            case "IntegerArray":
                return <span>{`[${value ? value.join(", ") : ""}]`}</span>;
            default:
            case "ShortText":
            case "LongText":
                return (
                    <p
                        style={{
                            width: "200px",
                            overflow: "hidden",
                            whiteSpace: "nowrap",
                            textOverflow: "ellipsis",
                            wordWrap: "break-word",
                        }}
                    >
                        {value}
                    </p>
                );
            case "DateTime":
                return (
                    <span>{value ? moment(value).toLocaleString() : ""}</span>
                );
            case "Boolean":
                return <span>{value + ""}</span>;
            case "Image":
                var b64encoded = null;
                if (value) {
                    var b64encoded = window.btoa(
                        new Uint8Array(value).reduce(function (data, byte) {
                            return data + String.fromCharCode(byte);
                        }, "")
                    );

                    return (
                        <img
                            src={"data:image/jpg;base64," + b64encoded}
                            style={{ maxWidth: "32px", maxHeight: "32px" }}
                        />
                    );
                } else {
                    return <></>;
                }
            // var u8 = new Uint8Array(value);
            // var b64encoded = btoa(String.fromCharCode.apply(null, u8));

            // var decoder = new TextDecoder("utf8");
            // console.log("decoder", decoder);
            // var b64encoded = btoa(decoder.decode(unescape(encodeURIComponent(u8))));
            // console.log("b64encoded", b64encoded);
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
// class DynamicRowElement extends Component {
//   static displayName = DynamicRowElement.name;

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
