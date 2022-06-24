import React, { Component } from "react";
import { shadeColor, transparentColor } from "../helpers";

export const ButtonIcon = ({disabled, onClick, icon, color, className}) => {

// Calculate color
let finalColor = color || '#000';
if(disabled) {
  finalColor = transparentColor(finalColor, 30);
}

// Create style
let styles = { margin: "4px", color: finalColor };
if(!disabled) {
  styles['cursor'] = "pointer"; 
}

// Create classes
let classes = (className || '') + " fas fa-" + (icon || "edit");

return (
  <i
    onClick={ disabled? () => {}: onClick }
    style={ styles }
    className={ classes }>
  </i>
);

}

