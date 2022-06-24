import React, { Component } from 'react';
import { connect } from 'react-redux';
import { RiPencilFill, RiDeleteRow } from 'react-icons/ri';
import { tablesActions } from '../actions';
import { CreateEditTableRecordModal } from '../modals'
import { DialogModal } from '../modals'
import { ButtonIcon } from './button-icon.component';
import { Table } from 'reactstrap';
import { Loading } from './loading.component';

import '../styles/ApiraiserTable.scss'

export const ApiraiserTable = ({headers, children}) => {
    return (
        <div className="apiraiser-table">
            <Table responsive size="sm">
                <thead>
                    <tr key={'apiraiser_table_header'}>
                        <th key={'apiraiser_table_header_#'} scope="col"></th>
                        {
                            headers.map(key => (
                                <th key={'apiraiser_table_header' + key} scope="col">{key}</th>
                            ))
                        }
                    </tr>
                </thead>
                <tbody>{children}</tbody>
            </Table>           
        </div>
    )
}
