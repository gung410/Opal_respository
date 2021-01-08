export class CxNumberUtil {
    public static round(value: number, factionDigits: number = 0): number {
        const floatPointMovingValue = Math.pow(10, factionDigits) * 1.0;
        return Math.round(value * floatPointMovingValue) / floatPointMovingValue;
    }

    public static toFixed(value: number, factionDigits: number = 0): string {
        return CxNumberUtil.round(value, factionDigits).toFixed(factionDigits);
    }

    public static zeroOrBigger(value: number): number {
        return value >= 0 ? value : 0;
    }
}