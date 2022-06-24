import React, { useState, useEffect} from 'react';
import Select from "react-select";

export const ReactSelectAdapter = ({ input, ...rest }) => (
    <Select {...input} {...rest} searchable />
)