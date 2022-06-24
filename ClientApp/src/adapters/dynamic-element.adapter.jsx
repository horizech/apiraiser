import React, { useState, useEffect} from 'react';
import { DynamicElement } from '../components';

export const DynamicElementAdapter = ({ input, meta, isWorking, column, label, isSelect, selectOptions, ...rest }) => (
    <DynamicElement isWorking={isWorking} column={column} isSelect={isSelect} label={label} selectOptions={selectOptions} input={input} errorText={meta.touched ? meta.error : ''}/>
)