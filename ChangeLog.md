# ChangeLog:

## Unreleased

### Added:

-   Added GetSchemasList API

### Changed:

-   Set Postgresql SSLMode to Disable

### Deprecated:

### Removed:

-   Removed Administration and Table API Controllers

### Fixed:

-   Fixed OrderBy if already exist

### Security:

## [v0.4.1] - 2022-07-06

### Added:

### Changed:

-   Changed User Controller to Authentication.
-   Use APIService.GetRowsByConditions in Table and Administration controller.

### Deprecated:

### Removed:

### Fixed:

### Security:

## [v0.4.0] - 2022-07-04

### Added:

-   Added dynamic API for system tables
-   Created templates for loading default tables.
-   Added Table columns check in Insert and Update API.

### Changed:

-   Changed schemas to Administration and Data.
-   Changed some default tables Schema to Application.
-   Use GetTables and GetTableColumns for both Schemas.
-   Use unit array while creating and getting images.

### Deprecated:

### Removed:

-   Removed Permissions, Permission Groups related stuff.

### Fixed:

-   Fixed System Permission Add and Update APIs.
-   Added Check for Predefined column in CreateTable.

### Security:

## [v0.3.1] - 2022-06-30

### Added:

### Changed:

-   Use Ids instead names when updating or deleting Roles, TablePermissions, SystemPermissions and UserAccessLevels.

### Deprecated:

### Removed:

### Fixed:

### Security:

## [v0.3.0] - 2022-06-30

### Added:

-   Added Table Permissions.
-   Added System Permissions.
-   Added multiple User Roles.

### Changed:

### Deprecated:

### Removed:

### Fixed:

-   Fixed email not saving in Initialize.

### Security:

## [v0.2.0] - 2022-06-24

### Added:

### Changed:

-   Changed Schema names.
-   Changed app name to Apiraiser.

### Deprecated:

### Removed:

### Fixed:

### Security:

## [v0.1.4] - 2022-06-24

### Added:

-   Added User Roles API.
-   Added Translations table and API.

### Changed:

### Deprecated:

### Removed:

### Fixed:

### Security:

## [v0.1.3] - 2022-06-22

### Added:

-   Added management of UserAccessLevels.
-   Added AWS S3 plugin.
-   Clear Cache on Create, Edit and Delete.

### Changed:

-   Changed the Configuration page.

### Deprecated:

### Removed:

### Fixed:

### Security:

## [v0.1.2] - 2022-05-06

### Added:

-   Use LevendrUserAccessLevel filter in API controller functions.

### Changed:

-   Renamed UserLevelAccess table to UserAccessLevels.
-   Updated UserLevelAccess filter
-   Changed Table Controller UserLevelAccess filter parameters
-   Use API Controller for Table records management in FE

### Deprecated:

### Removed:

### Fixed:

### Security:

## [v0.1.1] - 2022-04-25

### Added:

-   Added PermissionGroups in GetUserInfo result.
-   Use LevendrUserAccessLevel filter in Table controller functions.
-   Added LevendrUserAccessLevel filter.
-   Added boolen type output in Execute<T> function.
-   Implemented MemoryCache in many services.
-   Added MemoryCache system.
-   Connected UserLevelAccess with RolePermissionGroup
-   Added UserLevelAccess table.
-   Added error and success alerts and toasts
-   Implemented Alerts
-   Added title to alert
-   Implemented parseInt in create-edit public tables
-   Levendr Table create-edit-modal: implemented parseInt & updated dropdown
-   Implemented addUser FE
-   Implemented AddUser API
-   Implemened Levendr Dropdown component

### Changed:

-   Updated NavMenu Component.
-   Updated Table controller to use UserAccessLevel
-   Updated UpdateRows function.
-   Updated Permissions Configuration.
-   Updated LevendrAuthorized filter
-   Updated RolePermission in JWT and authorization.
-   Added tables to NavBar and removed sidebar
-   Updated navmenu with role, permissions, settings and users
-   Updated Login response.
-   Code cleanup

### Deprecated:

### Removed:

### Fixed:

-   Fixed AddUser API

### Security:

## [v0.1.0] - 2022-01-11

-   First release
