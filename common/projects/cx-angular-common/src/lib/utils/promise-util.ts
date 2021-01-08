// @dynamic
export class CxPromiseUtil {
    public static errorInline<T, TError extends (object | string | number) = object>(
        promise: Promise<T>): Promise<[TError, undefined] | [undefined, T]> {
        return promise.then(data => {
            return [undefined, data] as [undefined, T];
        }).catch(err => [err, undefined] as [TError, undefined]);
    }
}