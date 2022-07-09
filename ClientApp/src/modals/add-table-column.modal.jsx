import React, { useEffect, useState } from "react";
import { Field, Form } from "react-final-form";
import { Button, Modal, ModalBody, ModalFooter, ModalHeader } from "reactstrap";

import { Columns } from "../enums";
import { AddTableColumn } from "../components";

const AddTableColumnModal = ({
    headerLabel,
    schema,
    table,
    tableColumns,
    predefinedColumns,
    handleOnClose,
}) => {
    const [isVisible, setIsVisible] = useState(true);
    // const [isRequiredChecked, setIsRequiredChecked] = useState(false);
    // const [isUniqueChecked, setIsUniqueChecked] = useState(false);
    // const [isForeignKeyChecked, setIsForeignKeyChecked] = useState(false);
    const toggle = () => {
        const newIsVisible = !isVisible;
        setIsVisible(newIsVisible);
        handleOnClose(newIsVisible);
    };

    const handleOnModalClose = (result) => {
        const newIsVisible = !isVisible;
        setIsVisible(newIsVisible);
        handleOnClose(result);
    };

    const handleDatatypeValueChange = ({ value }) => {
        console.log(value);
    };

    return (
        <div>
            <Modal isOpen={isVisible} toggle={toggle}>
                <ModalHeader toggle={toggle}>{headerLabel}</ModalHeader>
                <ModalBody>
                    <AddTableColumn
                        key={table}
                        schema={schema}
                        table={table}
                        tableColumns={tableColumns}
                        predefinedColumns={predefinedColumns}
                        handleOnComplete={handleOnModalClose}
                    />
                </ModalBody>
                <ModalFooter>
                    {/* {props.buttonsConfig.map((button, i) => 
                    <Button key={"button_" + i} className={button.class}  onClick={button.handle}>{button.label}</Button>
                )} */}

                    {/* 
                    <Button color="primary" type="submit" form="form" value="Submit" >Confirm</Button>{' '}
                    <Button color="secondary" onClick={toggle}>Cancel</Button> */}
                </ModalFooter>
            </Modal>
        </div>
    );
};

export default AddTableColumnModal;
