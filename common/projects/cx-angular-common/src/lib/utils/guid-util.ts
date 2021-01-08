export class CxGuidUtil {
    public static newGuid(isNoDash?: boolean, prefix: string = '') {
        const hexadecimalOfTwoByte = 0x10000;
        const dash = isNoDash ? '' : '-';
        return prefix + s4() + s4() + dash + s4() + dash + s4() + dash +
            s4() + dash + s4() + s4() + s4();

        function s4() {
            return Math.floor(hexadecimalOfTwoByte * (Math.random() + 1))
                .toString(16)
                .substring(1);
        }
    }
}
