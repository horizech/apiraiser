export const isColumnPredefined = (column) => {
    if (column) {
        switch (column.Name) {
            
            case "Id": case "CreatedOn": case "CreatedBy": case "LastUpdatedOn": case "LastUpdatedBy": return true;
            default: return false;
        }
    }
    else {
        return true;
    }
}

  