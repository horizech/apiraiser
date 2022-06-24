export const GetElementTypeFromDataType = (datatype) => {
    let result = 'text';

    switch (datatype) {
        case "Nothing":
            result = -1;
            break;
        case "Integer":
            result = 'number';
            break;
        default: case "LongText": case "ShortText": case "ExtraLongText":
            result = 'text';
            break;
        case "DateTime":
            result = 'date';
            break;

        case "Boolean":
            result = 'checkbox';
            break;

        case "Nothing":
            result = 'text';
            break;
        case "Integer Array":
            result = 'number';
            break;
        case "Decimal":
            result = 'number';
            break;
        case "Float":
            result = 'number';
            break;
        case "Boolean Array":
            result = 'checkbox';
            break;
        case "Json":
            result = 'text';
            break;
        case "Json Array":
            result = 'text';
            break;
        case "Money":
            result = 'number';
            break;
        case "Image":
            result = 'image';
            break;
    }
    return result;
}

export const shadeColor = (color, percent) => {

    var R = 0;
    var G = 0;
    var B = 0;
    var A = '';

    if (color.length < 6) {
        R = parseInt(color.substring(1, 2) + color.substring(1, 2), 16);
        G = parseInt(color.substring(2, 3) + color.substring(2, 3), 16);
        B = parseInt(color.substring(3, 4) + color.substring(3, 4), 16);
        A = color.length > 4 ? parseInt(color.substring(4, 5) + color.substring(4, 5), 16) : 255;
    }
    else {
        R = parseInt(color.substring(1, 3), 16);
        G = parseInt(color.substring(3, 5), 16);
        B = parseInt(color.substring(5, 7), 16);
        A = color.length > 7 ? parseInt(color.substring(7, 9), 16) : 255;
    }

    R = parseInt(R * (100 + percent) / 100);
    G = parseInt(G * (100 + percent) / 100);
    B = parseInt(B * (100 + percent) / 100);

    R = (R < 255) ? R : 255;
    G = (G < 255) ? G : 255;
    B = (B < 255) ? B : 255;

    var RR = ((R.toString(16).length == 1) ? "0" + R.toString(16) : R.toString(16));
    var GG = ((G.toString(16).length == 1) ? "0" + G.toString(16) : G.toString(16));
    var BB = ((B.toString(16).length == 1) ? "0" + B.toString(16) : B.toString(16));
    var AA = ((A.toString(16).length == 1) ? "0" + A.toString(16) : A.toString(16));

    return "#" + RR + GG + BB + AA;
}


export const transparentColor = (color, percent) => {

    var R = 0;
    var G = 0;
    var B = 0;
    var A = 255;

    if (color.length < 6) {
        R = parseInt(color.substring(1, 2) + color.substring(1, 2), 16);
        G = parseInt(color.substring(2, 3) + color.substring(2, 3), 16);
        B = parseInt(color.substring(3, 4) + color.substring(3, 4), 16);
        A = color.length > 4 ? parseInt(color.substring(4, 5) + color.substring(4, 5), 16) : 255;
    }
    else {
        R = parseInt(color.substring(1, 3), 16);
        G = parseInt(color.substring(3, 5), 16);
        B = parseInt(color.substring(5, 7), 16);
        A = color.length > 7 ? parseInt(color.substring(7, 9), 16) : 255;
    }

    A = parseInt((A * percent) / 100);

    var RR = ((R.toString(16).length == 1) ? "0" + R.toString(16) : R.toString(16));
    var GG = ((G.toString(16).length == 1) ? "0" + G.toString(16) : G.toString(16));
    var BB = ((B.toString(16).length == 1) ? "0" + B.toString(16) : B.toString(16));
    var AA = ((A.toString(16).length == 1) ? "0" + A.toString(16) : A.toString(16));

    return "#" + RR + GG + BB + AA;
}