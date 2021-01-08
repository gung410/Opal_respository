export class CxKeyboardUtil {
    public static isNumberKey(key: string) {
        return numberKeys.indexOf(key) >= 0;
    }

    public static isCharKey(key: string) {
        return charKeys.indexOf(key) >= 0;
    }
}

const numberKeys = [
    '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
];
const charKeys = [
    'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
];
