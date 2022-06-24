import React, { Component } from 'react';
import { Loading } from './loading.component';
import {
    Dropdown,
    DropdownToggle,
    DropdownMenu,
    DropdownItem
} from "reactstrap";
import '../styles/ApiraiserDropdown.scss'

export const ApiraiserDropdown = ({ title, items, onClick }) => {
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
                        textAlign: "center", cursor: "pointer",
                    }}>

                    {items &&
                        items.map((item, index) => {
                            return (
                                <DropdownItem key={index} style={{ textAlign: "center", cursor: "pointer" }} onClick={() => gotoPath(item.path)}>{item.name}</DropdownItem>
                            )
                        })
                    }
                </DropdownMenu>
            </Dropdown>
        </div>
    )
}
