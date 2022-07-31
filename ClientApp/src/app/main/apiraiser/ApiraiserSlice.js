import {
    createEntityAdapter,
    createSelector,
    createSlice,
    createAsyncThunk,
} from "@reduxjs/toolkit";

import {
    prependNavigationItem,
    removeNavigationItem,
} from "app/store/fuse/navigationSlice";
import NotificationModel from "app/fuse-layouts/shared-components/notificationPanel/model/NotificationModel";
import { addNotification } from "app/fuse-layouts/shared-components/notificationPanel/store/dataSlice";
import axios from "axios";
import { createTableSlice, selectAll } from "../tables/TableSlice";
import i18next from "i18next";

const initialState = {
    isInitializedState: "waiting",
    isInitializedResult: null,

    initializeState: "waiting",
    initializeResult: null,

    tables: {},
    loadTablesState: "waiting",
    loadTablesResult: null,

    accessibleTables: {},
    loadAccessibleTablesState: "waiting",
    loadAccessibleTablesResult: null,

    slices: {},
    prepareSlicesState: "waiting",
    prepareSlicesResult: null,

    prepareNavigationState: "waiting",
    prepareNavigationResult: null,

    prepareTranslationState: "waiting",
    prepareTranslationResult: null,

    createTableState: "waiting",
    createTableResult: null,
    errors: [],
};

const isInitialized = createAsyncThunk(
    `apiraiser/isInitialized`,
    async (thunkAPI) => {
        let response = await axios.get(`/API/Apiraiser/IsInitialized`);
        return response.data.Success;
    }
);

const initialize = createAsyncThunk(
    `apiraiser/initialize`,
    async (postData, thunkAPI) => {
        let response = await axios.post(`/API/Apiraiser/Initialize`, postData);
        if (response.data.Message == "Apiraiser is already initialized!") {
            return true;
        }

        if (!response.data.Success) {
            thunkAPI.dispatch(
                addNotification(
                    NotificationModel({
                        message:
                            response.data.Message || response.data.ErrorCode,
                        options: { variant: "error" },
                    })
                )
            );
        }
        return response.data.Success;
    }
);

const createTable = createAsyncThunk(
    `apiraiser/createTable`,
    async ({ schema, table }, thunkAPI) => {
        let response = await axios.post(
            `/API/${schema}/CreateTable?table=${table}`,
            []
        );
        if (response.data.Success) {
            thunkAPI.dispatch(
                addNotification(
                    NotificationModel({
                        message: "Table created successfully!",
                        options: { variant: "success" },
                    })
                )
            );
            // thunkAPI.dispatch(this.loadAccessibleTables(roleIds));
        } else {
            thunkAPI.dispatch(
                addNotification(
                    NotificationModel({
                        message:
                            response.data.Message || response.data.ErrorCode,
                        options: { variant: "error" },
                    })
                )
            );
        }
        return response.data.Success;
    }
);

const loadTables = createAsyncThunk(
    `apiraiser/loadTables`,
    async (roleIds, thunkAPI) => {
        let response = await axios.get(`/API/GetSchemasList`);
        if (response.data.Success) {
            let schemas = await response.data;
            let tablesResponse = await axios.all(
                schemas.Data.map((schema) =>
                    axios.get(`/API/${schema}/GetTablesList`)
                )
            );
            let tables = {};
            tablesResponse
                .map((res) => res.data)
                .forEach((x, i) => {
                    tables[schemas.Data[i]] = {};
                    x.Data.forEach((table) => {
                        tables[schemas.Data[i]][table] = {
                            CanDelete: false,
                            CanRead: false,
                            CanUpdate: false,
                            CanWrite: false,
                        };
                    });
                });

            return tables;
        } else {
            thunkAPI.dispatch(
                addNotification(
                    NotificationModel({
                        message:
                            response.data.Message || response.data.ErrorCode,
                        options: { variant: "error" },
                    })
                )
            );
            return null;
        }
    }
);

const loadAccessibleTables = createAsyncThunk(
    `apiraiser/loadAccessibleTables`,
    async (roleIds, thunkAPI) => {
        let response = await axios.get(`/API/Administration/TablePermissions`);

        if (response.data.Success) {
            const result = {};

            if (response.data.Data && response.data.Data.length) {
                let accessibleTables = response.data.Data.filter(
                    (x) => roleIds.includes(x.Role) && x.CanRead
                );

                accessibleTables.forEach((item) => {
                    if (
                        !result[item.Schema] ||
                        Object.keys(result[item.Schema]).length < 1
                    ) {
                        result[item.Schema] = [];
                    }
                    result[item.Schema].push(item.Table);
                });
            }
            return result;
        } else {
            thunkAPI.dispatch(
                addNotification(
                    NotificationModel({
                        message:
                            response.data.Message || response.data.ErrorCode,
                        options: { variant: "error" },
                    })
                )
            );
            return null;
        }
    }
);

const prepareSlices = createAsyncThunk(
    `apiraiser/prepareSlices`,
    async (accessibleTables, thunkAPI) => {
        const injectReducer = require("app/store/injectReducer").default;

        let slices = {};

        Object.keys(accessibleTables).forEach((schema) => {
            accessibleTables[schema].forEach((table) => {
                const statePath = `${schema}_${table}`;
                slices[statePath] = createTableSlice(schema, table);

                injectReducer(statePath, slices[statePath].reducer);
            });
        });

        return slices;
    }
);

const prepareNavigation = createAsyncThunk(
    `apiraiser/prepareNavigation`,
    async ({ slices, tables, accessibleTables }, thunkAPI) => {
        if (slices["Administration_Users"]) {
            thunkAPI.dispatch(
                slices["Administration_Users"].thunks.getEntities()
            );
        }
        if (slices["Administration_Roles"]) {
            thunkAPI.dispatch(
                slices["Administration_Roles"].thunks.getEntities()
            );
        }
        Object.keys(tables).forEach((schema) => {
            thunkAPI.dispatch(
                prependNavigationItem({
                    id: schema.toLowerCase(),
                    title: schema,
                    type: "group",
                    icon: schema == "Data" ? "widgets" : "settings",
                    children: [
                        {
                            id: "addTable" + schema,
                            title: "Create new table",
                            type: "item",
                            icon: "add",
                            url: `${schema.toLowerCase()}`,
                            end: true,
                        },
                        // {
                        //     type: "divider",
                        // },
                    ].concat(
                        accessibleTables[schema]
                            ? accessibleTables[schema].map((table) => {
                                  return {
                                      id: table.toLowerCase(),
                                      title: table,
                                      type: "item",
                                      icon: "Data" ? "widgets" : "settings",
                                      url: `${schema.toLowerCase()}/${table}`,
                                  };
                              })
                            : []
                    ),
                })
            );
        });

        return true;
    }
);

const prepareTranslation = createAsyncThunk(
    `apiraiser/prepareTranslation`,
    async ({ slices }, thunkAPI) => {
        if (slices["Administration_Translations"]) {
            let data = await thunkAPI.dispatch(
                slices["Administration_Translations"].thunks.getEntities()
            );
            return data.payload;
        } else {
            return null;
        }
    }
);

const resetSlices = createAsyncThunk(
    `apiraiser/resetSlices`,
    async (accessibleTables, thunkAPI) => {
        const removeReducer = require("app/store/removeReducer").default;

        let slices = {};
        // let accessibleTables = thunkAPI.getState().apiraiser.accessibleTables;
        Object.keys(accessibleTables).forEach((schema) => {
            accessibleTables[schema].forEach((table) => {
                const statePath = `${schema}_${table}`;
                slices[statePath] = null;
                removeReducer(statePath);
            });
        });

        return true;
    }
);

const resetNavigation = createAsyncThunk(
    `apiraiser/resetNavigation`,
    async (tables, thunkAPI) => {
        Object.keys(tables).forEach((schema) => {
            thunkAPI.dispatch(removeNavigationItem(schema.toLowerCase()));
        });
        return true;
    }
);

const slice = createSlice({
    name: "apiraiser",
    initialState,
    reducers: {
        setError: (state, action) => {
            state.errors = action.payload;
        },
    },
    extraReducers: (builder) => {
        builder.addCase(isInitialized.pending, (state, action) => {
            state.isInitializedState = "loading";
            state.isInitializedResult = null;
        });
        builder.addCase(isInitialized.fulfilled, (state, action) => {
            state.isInitializedState = action.payload ? "success" : "error";
            state.isInitializedResult = action.payload;
        });
        builder.addCase(isInitialized.rejected, (state, action) => {
            state.isInitializedState = "error";
            state.isInitializedResult = null;
        });

        builder.addCase(initialize.pending, (state, action) => {
            state.initializeState = "loading";
            state.initializeResult = null;
        });
        builder.addCase(initialize.fulfilled, (state, action) => {
            state.initializeState = action.payload ? "success" : "error";
            state.initializeResult = action.payload;

            if (action.payload) {
                state.isInitializedState = action.payload ? "success" : "error";
                state.isInitializedResult = action.payload;
            }
        });
        builder.addCase(initialize.rejected, (state, action) => {
            state.initializeState = "error";
            state.initializeResult = null;
        });

        builder.addCase(loadTables.pending, (state, action) => {
            state.loadTablesState = "loading";
            state.loadTablesResult = null;
            state.tables = {};
        });
        builder.addCase(loadTables.fulfilled, (state, action) => {
            state.loadTablesState = action.payload ? "success" : "error";
            state.loadTablesResult =
                action.payload && Object.keys(action.payload).length > 0
                    ? true
                    : false;
            state.tables = action.payload;
        });
        builder.addCase(loadTables.rejected, (state, action) => {
            state.loadTablesState = "error";
            state.loadTablesResult = null;
            state.tables = {};
        });

        builder.addCase(loadAccessibleTables.pending, (state, action) => {
            state.loadAccessibleTablesState = "loading";
            state.loadAccessibleTablesResult = null;
            state.accessibleTables = {};
        });
        builder.addCase(loadAccessibleTables.fulfilled, (state, action) => {
            state.loadAccessibleTablesState = action.payload
                ? "success"
                : "error";
            state.loadAccessibleTablesResult =
                action.payload && Object.keys(action.payload).length > 0
                    ? true
                    : false;
            state.accessibleTables = action.payload;
        });
        builder.addCase(loadAccessibleTables.rejected, (state, action) => {
            state.loadAccessibleTablesState = "error";
            state.loadAccessibleTablesResult = null;
            state.accessibleTables = {};
        });

        builder.addCase(prepareSlices.pending, (state, action) => {
            state.prepareSlicesState = "loading";
            state.prepareSlicesResult = null;
            state.slices = {};
        });
        builder.addCase(prepareSlices.fulfilled, (state, action) => {
            state.prepareSlicesState = action.payload ? "success" : "error";
            state.prepareSlicesResult =
                action.payload && Object.keys(action.payload).length > 0
                    ? true
                    : false;
            state.slices = action.payload;
        });
        builder.addCase(prepareSlices.rejected, (state, action) => {
            state.prepareSlicesState = "error";
            state.prepareSlicesResult = null;
            state.slices = {};
        });

        builder.addCase(prepareNavigation.pending, (state, action) => {
            state.prepareNavigationState = "loading";
            state.prepareNavigationResult = null;
        });
        builder.addCase(prepareNavigation.fulfilled, (state, action) => {
            state.prepareNavigationState = action.payload ? "success" : "error";
            state.prepareNavigationResult = action.payload;
        });
        builder.addCase(prepareNavigation.rejected, (state, action) => {
            state.prepareNavigationState = "error";
            state.prepareNavigationResult = null;
        });

        builder.addCase(prepareTranslation.pending, (state, action) => {
            state.prepareTranslationState = "loading";
            state.prepareTranslationResult = null;
        });
        builder.addCase(prepareTranslation.fulfilled, (state, action) => {
            state.prepareTranslationState = action.payload
                ? "success"
                : "error";
            let en = {};

            if (action.payload) {
                action.payload.forEach((x) => {
                    en[x.Name] = x.English;
                });
            }
            // For now, use the same translations
            let tr = en;
            let ar = en;

            state.prepareTranslationResult = { en, tr, ar };

            i18next.addResourceBundle("en", "apiraiserTranslations", en);
            i18next.addResourceBundle("tr", "apiraiserTranslations", tr);
            i18next.addResourceBundle("ar", "apiraiserTranslations", ar);
        });
        builder.addCase(prepareTranslation.rejected, (state, action) => {
            state.prepareTranslationState = "error";
            state.prepareTranslationResult = null;
        });

        builder.addCase(createTable.pending, (state, action) => {
            state.createTableState = "loading";
            state.createTableResult = null;
        });
        builder.addCase(createTable.fulfilled, (state, action) => {
            state.createTableState = action.payload ? "success" : "error";
            state.createTableResult = action.payload;
        });
        builder.addCase(createTable.rejected, (state, action) => {
            state.createTableState = "error";
            state.createTableResult = null;
        });
    },
});

const apiraiser = {
    thunks: {
        isInitialized,
        initialize,
        createTable,
        loadTables,
        loadAccessibleTables,
        prepareNavigation,
        prepareSlices,
        prepareTranslation,
        resetSlices,
        resetNavigation,
    },
    actions: slice.actions,
    reducer: slice.reducer,
};

export default apiraiser;

export const apiraiserReducer = apiraiser.reducer;
