export class Columns {

    static DataTypes = {
        Integer: 0,
        IntegerArray: 1,
        Decimal: 2,
        Float: 3,
        Boolean: 4,
        BooleanArray: 5,
        DateTime: 6,
        Json: 7,
        JsonArray: 8,
        Money: 9,
        ShortText: 10,
        LongText: 11,
        Image: 12
    
    }

    static DataTypesOptions = [        
        { label: 'Nothing', value: -1},
        { label: 'Integer', value: 0 },
        { label: 'Integer Array', value: 1 },
        { label: 'Decimal', value: 2 },
        { label: 'Float', value: 3 },
        { label: 'Boolean', value: 4 },
        { label: 'Boolean Array', value: 5 },
        { label: 'DateTime', value: 6 },
        { label: 'Json', value: 7 },
        { label: 'Json Array', value: 8 },
        { label: 'Money', value: 9 },
        { label: 'Short Text', value: 10 },
        { label: 'Long Text', value: 11 },
        { label: 'Image', value: 12 }
    ];
}