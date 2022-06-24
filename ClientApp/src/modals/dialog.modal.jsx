import React, { useState } from 'react';
import { Button, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';

const DialogModal = (props) => {
    const { headerLabel, text, handleOnClose } = props;
    // let isVisible=isVisible;
    
    // console.log(props);

    const [isVisible, setIsVisible] = useState(true);
    // const [mode, setmode] = useState();
    // const [buttonLabel, setbuttonLabel] = useState();
    const toggle = () => {
        const newIsVisible = !isVisible;
        setIsVisible(newIsVisible);
        if(!newIsVisible) {
            handleOnClose(null);
        }
    }

    const handleOnModalClose = (result) => {
        setIsVisible(false);
        handleOnClose(result);
    }
    
    // const button= (button)=> {
    //     return 
    // }
    return (
        <div>
            {/* <RiDeleteRow onClick={toggle} /> */}
            <Modal isOpen={isVisible} toggle={toggle} >
                <ModalHeader toggle={toggle}>{headerLabel} Row</ModalHeader>
                <ModalBody>

                <p>{text}</p>

                </ModalBody>
                <ModalFooter >
                {props.buttonsConfig.map((button, i) => 
                    <Button key={"button_" + i} className={button.class}  onClick={button.handle}>{button.label}</Button>
                )}

                    {/* <Button color="primary" onClick={() => handleOnModalClose(true)}>Confirm</Button>{' '}
                    <Button color="secondary" onClick={toggle}>Cancel</Button> */}
                </ModalFooter>
            </Modal>
        </div>
    );
}

export default DialogModal;