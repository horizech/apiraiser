import FuseLoading from "@fuse/core/FuseLoading";
import FusePageCarded from "@fuse/core/FusePageCarded";
import { styled } from "@mui/material/styles";
import React, { useEffect, useState } from "react";
import { useSelector, useStore } from "react-redux";
import { useParams } from "react-router-dom";
import TableEntitiesHeader from "./TableEntitiesHeader";
import { TableEntitiesTable } from "./TableEntitiesTable";

const Root = styled(FusePageCarded)(({ theme }) => ({
    "& .FusePageCarded-header": {
        minHeight: 72,
        height: 72,
        alignItems: "center",
        [theme.breakpoints.up("sm")]: {
            minHeight: 136,
            height: 136,
        },
    },
    "& .FusePageCarded-content": {
        display: "flex",
    },
    "& .FusePageCarded-contentCard": {
        overflow: "hidden",
    },
}));

const TableEntities = ({ schema }) => {
    const store = useStore();
    const routeParams = useParams();

    const [table, setTable] = useState();

    useEffect(() => {
        if (routeParams.table && routeParams.table !== table) {
            setTable(routeParams.table);
        }
    }, [routeParams]);

    const isReady = useSelector(({ apiraiser }) => {
        if (
            schema &&
            table &&
            apiraiser.prepareSlicesState == "success" &&
            apiraiser.slices[`${schema}_${table}`] &&
            apiraiser.prepareTranslationState == "success"
        ) {
            return true;
        } else {
            return false;
        }
    });

    if (schema && table && isReady) {
        return (
            <Root
                header={<TableEntitiesHeader schema={schema} table={table} />}
                content={<TableEntitiesTable schema={schema} table={table} />}
                innerScroll
            />
        );
    } else {
        return <FuseLoading />;
    }
};

export default TableEntities;
