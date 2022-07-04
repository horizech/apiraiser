import React, { useState, useEffect } from "react";
import Select from "react-select";
import { Button } from "reactstrap";
import { isColumnPredefined } from "../helpers";

export const ReactImageAdapter = ({
  user,
  input,
  column,
  isWorking,
  onChange,
  onFocus,
  onBlur,
  label,
  isSelect,
  selectOptions,
  ...rest
}) => {
  const [byteArray, setByteArray] = useState(null);
  const [selectedFile, setSelectedFile] = useState(null);
  const [selectedFileName, setSelectedFileName] = useState(null);
  const [preview, setPreview] = useState(null);
  const inputFileRef = React.useRef();

  const toBase64 = (file) =>
    new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => resolve(reader.result);
      reader.onerror = (error) => reject(error);
    });

  const processImage = async (e) => {
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
      let binaryString = String.fromCharCode.apply(null, array);
      input.onChange(array);
      console.log(array);
      setByteArray(array);
    };
    reader.readAsArrayBuffer(e.target.files[0]);

    // reader.onload = function () {
    //   // let arrayBuffer = this.result;
    //   // let array = new Uint8Array(arrayBuffer);
    //   // let binaryString = String.fromCharCode.apply(null, array);
    //   input.onChange(reader.result);
    //   console.log(reader.result);
    //   // setByteArray(array);
    // };
    // reader.readAsDataURL(e.target.files[0]);
  };

  // create a preview as a side effect, whenever selected file is changed
  useEffect(() => {
    if (!selectedFile) {
      setPreview(undefined);
      return;
    }

    const objectUrl = URL.createObjectURL(selectedFile);
    setPreview(objectUrl);

    // free memory when ever this component is unmounted
    return () => URL.revokeObjectURL(objectUrl);
  }, [selectedFile]);

  return (
    <>
      <Button onClick={() => inputFileRef.current.click()}>
        {" "}
        Load {column.Name}
      </Button>
      <input
        id={column.Name}
        ref={inputFileRef}
        hidden
        type="file"
        disabled={isWorking || isColumnPredefined(column)}
        onChange={(e) => processImage(e)}
        // onChange={(e) => { input.onChange(e); if (onChange) { onChange(e); }}}
        // onFocus={(e) => { input.onFocus(e); if (onFocus) { onFocus(e); }}}
        // onBlur={(e) => { input.onBlur(e); if (onBlur) { onBlur(e); }}}
      />
      <span>{selectedFileName}</span>
      {(label || column.Name) + (column.IsRequired ? " *" : "")}
      <img src={preview} style={{ maxWidth: "64px", maxHeight: "64px" }} />
    </>
  );
};
