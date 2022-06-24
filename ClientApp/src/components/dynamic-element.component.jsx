import React, { Component } from "react";
import { Link } from "react-router-dom";
import { connect } from "react-redux";
import { isColumnPredefined, history } from '../helpers';
import { Form, Field } from 'react-final-form'
import "../styles/DynamicElement.css";
import { ReactSelectAdapter } from "../adapters";


const DynamicElement = ({ user, input, column, isWorking, onChange, onFocus, onBlur, label, isSelect, selectOptions }) => {

  const getElement = () => {
  
    if(isSelect) {
      return (
        <div className={getWidth()}>
          <label htmlFor={column.Name}>{(label || column.Name) + (column.IsRequired ? ' *' : '')}</label>
          <ReactSelectAdapter input={input} options={selectOptions}/>
        </div>
      )
    }
    else {
      switch (column.Datatype) {
        case "Integer": return (
          <div className={getWidth()}>
            <div className={'form-group'}>
              <label htmlFor={column.Name}>{(label || column.Name) + (column.IsRequired ? ' *' : '')}</label>
              <input className="form-control" {...input} id={column.Name}  type="number" required={column.IsRequired} disabled={ isWorking || isColumnPredefined(column)}
                // onChange={(e) => { input.onChange(e); if (onChange) { onChange(e); }}}
                // onFocus={(e) => { input.onFocus(e); if (onFocus) { onFocus(e); }}}
                // onBlur={(e) => { input.onBlur(e); if (onBlur) { onBlur(e); }}}
              />
            </div>
          </div>
        );
        default: case "LongText": case "ShortText": return (
          <div className={getWidth()}>
            <div className={'form-group'}>
              <label htmlFor={column.Name}>{(label || column.Name) + (column.IsRequired ? ' *' : '')}</label>
              <input className="form-control" {...input} id={column.Name}  type="text" required={column.IsRequired} disabled={ isWorking || isColumnPredefined(column)}
                // onChange={(e) => { input.onChange(e); if (onChange) { onChange(e); }}}
                // onFocus={(e) => { input.onFocus(e); if (onFocus) { onFocus(e); }}}
                // onBlur={(e) => { input.onBlur(e); if (onBlur) { onBlur(e); }}}
              />
            </div>
          </div>
         
        );
        case "DateTime": return (
          <div className={getWidth()}>
            <div className={'form-group'}>
              <label htmlFor={column.Name}>{(label || column.Name) + (column.IsRequired ? ' *' : '')}</label>
              <input className="form-control" {...input} id={column.Name}  type="date" required={column.IsRequired} disabled={ isWorking || isColumnPredefined(column)}
                // onChange={(e) => { input.onChange(e); if (onChange) { onChange(e); }}}
                // onFocus={(e) => { input.onFocus(e); if (onFocus) { onFocus(e); }}}
                // onBlur={(e) => { input.onBlur(e); if (onBlur) { onBlur(e); }}}
              />
            </div>
          </div>
        );
        case "Boolean": return (
          <div className={getWidth()}>
            <div className="element-boolean">
              <label htmlFor={column.Name}>
                <input {...input} id={column.Name} type="checkbox" disabled={ isWorking || isColumnPredefined(column)}
                  // onChange={(e) => { input.onChange(e); if (onChange) { onChange(e); }}}
                  // onFocus={(e) => { input.onFocus(e); if (onFocus) { onFocus(e); }}}
                  // onBlur={(e) => { input.onBlur(e); if (onBlur) { onBlur(e); }}}
                />
                <span className={input.checked ? 'element-boolean-checked': 'element-boolean-unchecked'}></span>{(label || column.Name) + (column.IsRequired ? ' *' : '')}
              </label>
            </div>
          </div>
        );
  
        
      }
  
    }
  }

  const getWidth = () => {
    switch (column.Datatype) {
      case "Integer": return ' col-md-4';
      default: case "ShortText": return ' col-md-4';
      case "ExtraLongText": return ' col-md-12';
      case "LongText": return ' col-md-8';
      case "DateTime": return ' col-md-4';
      case "Boolean": return ' col-md-4';
    }
  }

  const getDefaultValue = () => {
    
    if (column) {
      switch (column.Datatype) {
        case "Integer": return 0;
        default: case "ShortText": return '';
        case "LongText": return '';
        case "DateTime": return (new Date());
        case "Boolean": return false;
      }
    }
    else {
      return undefined;
    }
  }

    if (column) {
      return (
        getElement()
      )
    }
    else {
      return (
        <p>
          <em>Loading...</em>
        </p>
      );
    }
  

}
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
//         <div className={this.getWidth()}>
//           <label htmlFor={column.Name}>{(label || column.Name) + (column.IsRequired ? ' *' : '')}</label>
//           <ReactSelectAdapter input={input} options={selectOptions}/>
//         </div>
//       )
//     }
//     else {
//       switch (column.Datatype) {
//         case "Integer": return (
//           <div className={this.getWidth()}>
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
//           <div className={this.getWidth()}>
//             <div className={'form-group'}>
//               <label htmlFor={column.Name}>{(label || column.Name) + (column.IsRequired ? ' *' : '')}</label>
//               <input className="form-control" {...input} id={column.Name}  type="text" required={column.IsRequired} disabled={ this.props.isWorking || isColumnPredefined(column)}
//                 // onChange={(e) => { input.onChange(e); if (onChange) { onChange(e); }}}
//                 // onFocus={(e) => { input.onFocus(e); if (onFocus) { onFocus(e); }}}
//                 // onBlur={(e) => { input.onBlur(e); if (onBlur) { onBlur(e); }}}
//               />
//             </div>
//           </div>
//           // <div className={this.getWidth()}>
//           //   <div className={'form-group'}>
//           //     <label htmlFor={column.Name}>{(label || column.Name) + (column.IsRequired ? ' *' : '')}</label>
//           //     <input type="text" required={column.IsRequired} disabled={ this.props.isWorking || getIsReadOnly(column)} className="form-control" value={this.state.value} placeholder={column.Name} onChange={this.handleChange} />
//           //   </div>
//           // </div>
//         );
//         case "DateTime": return (
//           <div className={this.getWidth()}>
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
//           <div className={this.getWidth()}>
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

//   getWidth() {
//     switch (this.props.column.Datatype) {
//       case "Integer": return ' col-md-4';
//       default: case "ShortText": return ' col-md-4';
//       case "LongText": return ' col-md-8';
//       case "DateTime": return ' col-md-4';
//       case "Boolean": return ' col-md-4';
//     }
//   }

//   getDefaultValue() {
//     const { user, column, value } = this.props;
//     // if (value !== undefined) {
//     //   return value;
//     // }
//     if (column) {
//       switch (column.Datatype) {
//         case "Integer": return 0;
//         default: case "ShortText": return '';
//         case "LongText": return '';
//         case "DateTime": return (new Date());
//         case "Boolean": return false;
//       }
//     }
//     else {
//       return undefined;
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

function mapStateToProps(state) {
  const { user, loggedIn } = state.authentication;

  return {
    user,
    loggedIn
  };
}

const connectedDynamicElement = connect(mapStateToProps)(DynamicElement);
export { connectedDynamicElement as DynamicElement };
