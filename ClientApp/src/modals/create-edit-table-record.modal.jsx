import React, { useState } from "react";
import { Button, Modal, ModalHeader, ModalBody, ModalFooter } from "reactstrap";
import { CreateEditTableRecord } from "../components";

const CreateEditTableRecordModal = ({
    schema,
    table,
    tableColumns,
    row,
    label,
    mode,
    handleOnClose,
}) => {
    const [isVisible, setIsVisible] = useState(true);

    const toggle = () => {
        const newIsVisible = !isVisible;
        setIsVisible(newIsVisible);
        if (!newIsVisible) {
            handleOnClose(null);
        }
    };

    const handleOnModalClose = (result) => {
        const newIsVisible = !isVisible;
        setIsVisible(newIsVisible);
        handleOnClose(result);
    };

    return (
        <div>
            <Modal isOpen={isVisible} toggle={toggle} title={label}>
                <ModalHeader toggle={toggle}>{label} Row</ModalHeader>
                <ModalBody>
                    <CreateEditTableRecord
                        schema={schema}
                        key={table}
                        tableColumns={tableColumns}
                        mode={mode}
                        table={table}
                        row={row}
                        handleOnComplete={handleOnModalClose}
                    />
                </ModalBody>
            </Modal>
        </div>
    );
};

export default CreateEditTableRecordModal;
