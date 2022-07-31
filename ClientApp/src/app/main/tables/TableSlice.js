import {
    createEntityAdapter,
    createSelector,
    createSlice,
    createAsyncThunk,
} from "@reduxjs/toolkit";
import NotificationModel from "app/fuse-layouts/shared-components/notificationPanel/model/NotificationModel";
import { addNotification } from "app/fuse-layouts/shared-components/notificationPanel/store/dataSlice";
import axios from "axios";

export const createTableSlice = (schema, table) => {
    const initialState = {
        isInitialized: false,
        schema: schema,
        table: table,
        searchText: "",
        ids: [],
        entitiesState: "waiting",
        entities: {},
        columnsState: "waiting",
        columns: [],
        createState: "waiting",
        createResult: null,
        editState: "waiting",
        editResult: null,
        deleteState: "waiting",
        deleteResult: null,
        addColumnState: "waiting",
        addColumnResult: null,
        removeColumnState: "waiting",
        removeColumnResult: null,
        errors: [],
    };

    // First, create the thunk
    const fetchUserById = createAsyncThunk(
        "users/fetchByIdStatus",
        async (userId, thunkAPI) => {
            const response = await userAPI.fetchById(userId);
            return response.data;
        }
    );

    const getColumns = createAsyncThunk(
        `${schema}_${table}/getColumns`,
        async (thunkAPI) => {
            try {
                let response = await axios.get(
                    `/API/${schema}/${table}/Columns`
                );
                return response.data.Data;
            } catch (e) {
                return [];
            }

            // .then((response) => {
            //     if (response.data.Success) {
            //         return thunkAPI.dispatch(
            //             tablesActions.setColumns({
            //                 schema,
            //                 table,
            //                 data: response.data.Data,
            //             })
            //         );
            //     } else {
            //         return thunkAPI.dispatch(
            //             tablesActions.setError([response.data.ErrorCode])
            //         );
            //     }
            // });
        }
    );

    const getEntities = createAsyncThunk(
        `${schema}_${table}/getEntities`,
        async (thunkAPI) => {
            try {
                let response = await axios.get(`/API/${schema}/${table}`);
                return response.data.Data;
            } catch (e) {
                return [];
            }

            // .then((response) => {
            //     if (
            //         response.data.Success ||
            //         response.data.Message === "Nothing found!"
            //     ) {
            //         return dispatch(
            //             tablesActions.setEntities({
            //                 schema,
            //                 table,
            //                 data: response.data.Data,
            //             })
            //         );
            //     } else {
            //         return dispatch(
            //             tablesActions.setError([
            //                 response.data.Message || response.data.ErrorCode,
            //             ])
            //         );
            //     }
            // });
        }
    );

    const createEntity = createAsyncThunk(
        `${schema}_${table}/createEntity`,
        async (postData, thunkAPI) => {
            let response = await axios.post(
                `/API/${schema}/${table}`,
                postData
            );
            if (response.data.Success) {
                thunkAPI.dispatch(
                    addNotification(
                        NotificationModel({
                            message: table + " created sucessfully!",
                            options: { variant: "success" },
                        })
                    )
                );
                return response.data.Success;
            } else {
                dispatch(
                    tablesActions.setError([
                        response.data.Message || response.data.ErrorCode,
                    ])
                );
                return response.data.Success;
            }
        }
    );

    const editEntity = createAsyncThunk(
        `${schema}_${table}/editEntity`,
        async ({ id, postData }, thunkAPI) => {
            let response = await axios.put(
                `/API/${schema}/${table}/${id}`,
                postData
            );

            if (response.data.Success) {
                thunkAPI.dispatch(
                    addNotification(
                        NotificationModel({
                            message: table + " updated sucessfully!",
                            options: { variant: "success" },
                        })
                    )
                );
                return response.data.Success;
            } else {
                thunkAPI.dispatch(
                    tablesActions.setError([
                        response.data.Message || response.data.ErrorCode,
                    ])
                );
                return response.data.Success;
            }
        }
    );

    const deleteEntity = createAsyncThunk(
        `${schema}_${table}/deleteEntity`,
        async (id, thunkAPI) => {
            let response = await axios.delete(`/API/${schema}/${table}/${id}`);

            if (response.data.Success) {
                thunkAPI.dispatch(
                    addNotification(
                        NotificationModel({
                            message: table + " removed sucessfully!",
                            options: { variant: "success" },
                        })
                    )
                );
                return response.data.Success;
            } else {
                dispatch(
                    tablesActions.setError([
                        response.data.Message || response.data.ErrorCode,
                    ])
                );
                return response.data.Success;
            }
        }
    );

    const addColumn = createAsyncThunk(
        `${schema}_${table}/addColumn`,
        async (postData, thunkAPI) => {
            let response = await axios.post(
                `/API/${schema}/${table}/Column`,
                postData
            );
            if (response.data.Success) {
                thunkAPI.dispatch(
                    addNotification(
                        NotificationModel({
                            message: table + " updated sucessfully!",
                            options: { variant: "success" },
                        })
                    )
                );
                return response.data.Success;
            } else {
                dispatch(
                    tablesActions.setError([
                        response.data.Message || response.data.ErrorCode,
                    ])
                );
                return response.data.Success;
            }
        }
    );

    const removeColumn = createAsyncThunk(
        `${schema}_${table}/removeColumn`,
        async (name, thunkAPI) => {
            let response = await axios.delete(
                `/API/${schema}/${table}/Column/${name}`
            );
            if (response.data.Success) {
                thunkAPI.dispatch(
                    addNotification(
                        NotificationModel({
                            message: table + " updated sucessfully!",
                            options: { variant: "success" },
                        })
                    )
                );
                return response.data.Success;
            } else {
                dispatch(
                    tablesActions.setError([
                        response.data.Message || response.data.ErrorCode,
                    ])
                );
                return response.data.Success;
            }
        }
    );

    const slice = createSlice({
        name: `${schema}_${table}`,
        initialState,
        reducers: {
            initialize: (state, action) => {
                const { schema, table } = action.payload;
                state.schema = schema;
                state.table = table;
                state.isInitialized = true;
            },
            getEntities: (state, action) => {
                state.entitiesState = "loading";
                state.entities = {};
            },
            setEntities: (state, action) => {
                if (action.payload && action.payload.data) {
                    let { data } = action.payload;

                    let entities = {};
                    data.forEach((element) => {
                        entities[element.Id] = element;
                    });

                    state.entitiesState = "success";
                    state.entities = entities;
                    state.ids = Object.keys(entities);
                } else {
                    state.entitiesState = "success";
                    state.entities = {};
                    state.ids = [];
                }
            },
            setSearchText: {
                reducer: (state, action) => {
                    state.searchText = action.payload;
                },
                prepare: (event) => ({ payload: event.target.value || "" }),
            },
            getColumns: (state, action) => {
                state.columnsState = "loading";
                state.columns = [];
            },
            setColumns: (state, action) => {
                let { data } = action.payload;
                state.columnsState = "success";
                state.columns = data;
            },
            create: (state, action) => {
                state.createState = "loading";
                state.createResult = null;
            },
            setCreateResult: (state, action) => {
                state.createState = action.payload ? "success" : "error";
                state.createResult = action.payload;
            },
            edit: (state, action) => {
                state.editState = "loading";
                state.editResult = null;
            },
            setEditResult: (state, action) => {
                state.editState = action.payload ? "success" : "error";
                state.editResult = action.payload;
            },
            delete: (state, action) => {
                state.deleteState = "loading";
                state.deleteResult = null;
            },
            setDeleteResult: (state, action) => {
                state.deleteState = action.payload ? "success" : "error";
                state.deleteResult = action.payload;
            },
            setError: (state, action) => {
                state.errors = action.payload;
            },
        },
        extraReducers: (builder) => {
            // Add reducers for additional action types here, and handle loading state as needed
            builder.addCase(getColumns.pending, (state, action) => {
                state.columnsState = "loading";
                state.columns = [];
            });
            builder.addCase(getColumns.fulfilled, (state, action) => {
                state.columnsState = "success";
                state.columns = action.payload;
            });
            builder.addCase(getColumns.rejected, (state, action) => {
                state.columnsState = "error";
                state.columns = [];
            });

            builder.addCase(getEntities.pending, (state, action) => {
                state.entitiesState = "loading";
                state.entities = {};
            });
            builder.addCase(getEntities.fulfilled, (state, action) => {
                if (action.payload) {
                    let entities = {};
                    action.payload.forEach((element) => {
                        entities[element.Id] = element;
                    });

                    state.entitiesState = "success";
                    state.entities = entities;
                    state.ids = Object.keys(entities);
                } else {
                    state.entitiesState = "success";
                    state.entities = {};
                    state.ids = [];
                }
            });
            builder.addCase(getEntities.rejected, (state, action) => {
                state.entitiesState = "error";
                state.entities = {};
            });

            builder.addCase(createEntity.pending, (state, action) => {
                state.createState = "loading";
                state.createResult = null;
            });
            builder.addCase(createEntity.fulfilled, (state, action) => {
                state.createState = action.payload ? "success" : "error";
                state.createResult = action.payload;
            });
            builder.addCase(createEntity.rejected, (state, action) => {
                state.createState = "error";
                state.createResult = null;
            });

            builder.addCase(editEntity.pending, (state, action) => {
                state.editState = "loading";
                state.editResult = null;
            });
            builder.addCase(editEntity.fulfilled, (state, action) => {
                state.editState = action.payload ? "success" : "error";
                state.editResult = action.payload;
            });
            builder.addCase(editEntity.rejected, (state, action) => {
                state.editState = "error";
                state.editResult = null;
            });

            builder.addCase(deleteEntity.pending, (state, action) => {
                state.deleteState = "loading";
                state.deleteResult = null;
            });
            builder.addCase(deleteEntity.fulfilled, (state, action) => {
                state.deleteState = action.payload ? "success" : "error";
                state.deleteResult = action.payload;
            });
            builder.addCase(deleteEntity.rejected, (state, action) => {
                state.deleteState = "error";
                state.deleteResult = null;
            });

            builder.addCase(addColumn.pending, (state, action) => {
                state.addColumnState = "loading";
                state.addColumnResult = null;
            });
            builder.addCase(addColumn.fulfilled, (state, action) => {
                state.addColumnState = action.payload ? "success" : "error";
                state.addColumnResult = action.payload;
            });
            builder.addCase(addColumn.rejected, (state, action) => {
                state.addColumnState = "error";
                state.addColumnResult = null;
            });

            builder.addCase(removeColumn.pending, (state, action) => {
                state.removeColumnState = "loading";
                state.removeColumnResult = null;
            });
            builder.addCase(removeColumn.fulfilled, (state, action) => {
                state.removeColumnState = action.payload ? "success" : "error";
                state.removeColumnResult = action.payload;
            });
            builder.addCase(removeColumn.rejected, (state, action) => {
                state.removeColumnState = "error";
                state.removeColumnResult = null;
            });
        },
    });

    return {
        thunks: {
            getEntities,
            getColumns,
            createEntity,
            editEntity,
            deleteEntity,
            addColumn,
            removeColumn,
        },
        actions: slice.actions,
        reducer: slice.reducer,
    };
};

const dataAdapter = createEntityAdapter({});

// export const GetEntitiesByTable = createSelector(
//     [
//         // Usual first input - extract value from `state`
//         (state) => state.data,
//         // Take the second arg, `category`, and forward to the output selector
//         (state, table) => table,
//     ],
//     // Output selector gets (`items, category)` as args
//     (data, table) => tables.entities
// );

export const selectAll = ({ entities, ids }) => {
    if (ids && ids.length && entities && Object.keys(entities)) {
        return ids.map((key) => entities[key]);
    }
    return [];
};
// createSelector(
//         (state) => state.data.entities,
//         (state) => state.data.ids,
//         (entities, ids) => {
//             if (ids && ids.length && entities && Object.keys(entities)) {
//                 console.log("table: ", table);
//                 console.log("entities", entities);
//                 console.log("ids: ", ids);
//                 return ids.map((key) => entities[key]);
//             }
//             return [];
//         }
//     );

// {

//     console.log("state", state);
//     console.log("table", table);

//     let entities = state.data.entities;
//     let ids = state.data.ids;
//     if (ids && ids.length && entities && Object.keys(entities)) {
//         console.log("table: ", table);
//         console.log("entities", entities);
//         console.log("ids: ", ids);
//         return ids.map((key) => entities[key]);
//     }
//     return [];

//     //return state.entities;
// };

export const selectIds = (table) => (state) => {
    console.log("table: ", table);
    console.log("data: ", state.data);
    return state.data ? state.data.ids : [];
};

export const selectEntities = (table) => (state) =>
    state.data ? state.data.entities : {};

export const selectAll1 = (table) =>
    createSelector(selectIds(table), selectEntities(table), (ids, entities) =>
        ids.map((id) => entities[id])
    );

// export const selectAll = () =>
//     createSelector(
//         [
//             // Usual first input - extract value from `state`
//             (a) => a.data,
//             // Take the second arg, `category`, and forward to the output selector
//             (b) => b,
//         ],
//         // Output selector gets (`items, category)` as args
//         (data, category) => {
//             console.log(data, category);
//             data.filter((item) => item.category === category);
//         }
//     );

// let entities = state.data.entities;
// let ids = state.data.ids;
// if (ids && ids.length && entities && Object.keys(entities)) {
//     console.log("table: ", table);
//     console.log("entities", entities);
//     console.log("ids: ", ids);
//     return ids.map((key) => entities[key]);
// }
// return [];
// };

// export const selectAll = (table) => (state) => {
//     selectIds,
//       selectEntities,
//       (ids, entities): T[] => ids.map((id) => entities[id]!)

//     // let entities = state.data.entities;
//     // let ids = state.data.ids;
//     // if (ids && ids.length && entities && Object.keys(entities)) {
//     //     console.log("table: ", table);
//     //     console.log("entities", entities);
//     //     console.log("ids: ", ids);
//     //     return ids.map((key) => entities[key]);
//     // }
//     // return [];
// };

// const booksAdapter = createEntityAdapter({
//     // Assume IDs are stored in a field other than `book.id`
//     selectId: (x) => x.Id,
//     selectAll: (x) => Object.keys(x.entities
//     // Keep the "all IDs" array sorted based on book titles
//     sortComparer: (a, b) => a.title.localeCompare(b.title),
//   })

//dataAdapter. .addOne(selectItemsByTable);

let defaultSelectors = dataAdapter.getSelectors((state) => state.data);
//defaultSelectors['selectItemsByTable'] = selectItemsByTable;
export const dataSelectors = defaultSelectors;

// export const dataSelectors = {
//   getAllTableEntities: _dataSelectors.selectAll,
//   getTableEntityById: _dataSelectors.selectById,
// };

// export const getTableSlice = (schema, table) => {
//     return createTableSlice(schema, table);
// };
