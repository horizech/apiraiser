import React, { Component } from 'react';
import { Loading } from './loading.component';
import {
    Dropdown,
    DropdownToggle,
    DropdownMenu,
    DropdownItem
} from "reactstrap";
import '../styles/ApiraiserDropdown.scss'
import { ButtonIcon } from './button-icon.component';

export const TableDropdown = ({ title, items, onClick }) => {
    const [dropdownOpen, setDropdownOpen] = React.useState(false);

    const toggle = () => setDropdownOpen(!dropdownOpen);
    const gotoPath = (subMenuPath) => {
        toggle();
        onClick(subMenuPath);
    }
    return (
        <div className="apiraiser-dropdown">
            <Dropdown isOpen={dropdownOpen} toggle={toggle}>
                <DropdownToggle
                    className="text-dark nav-link"
                    tag="a"
                    data-toggle="dropdown"
                    aria-expanded={dropdownOpen}
                    style={{ textAlign: "center", cursor: "pointer" }}
                >{title}
                </DropdownToggle>
                <DropdownMenu
                    style={{
                        textAlign: "center", cursor: "pointer", width: "250px"
                    }}>
                    <DropdownItem key={-1} style={{ textAlign: "center", cursor: "pointer" }} onClick={() => gotoPath("/table/create")}>Create New Table</DropdownItem>

                    {items &&
                        items.map((table, index) => {
                            return (
                                <React.Fragment>
                                    <div key={index} style={{display: 'flex', flexDirection: 'row', justifyContent: 'space-between'}}>
                                    <DropdownItem key={index + "-1"} style={{ width: "200px", textAlign: "center", cursor: "pointer" }} onClick={() => gotoPath(`/table/data/${table}`)}>{table}</DropdownItem>
                                    <DropdownItem key={index + 1} style={{ width: "50px", textAlign: "center", cursor: "pointer" }} onClick={() => gotoPath(`/table/design/${table}`)}>  
                                        <ButtonIcon className="ml-3" icon="table" color="black" />
                                    </DropdownItem>
                                    </div>
                                </React.Fragment>
                            )
                        })
                    }
                </DropdownMenu>
            </Dropdown>
        </div>
    )
}
