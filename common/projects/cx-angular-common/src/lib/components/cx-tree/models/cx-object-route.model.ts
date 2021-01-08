export class CxObjectRoute<T> {
    public route: {};
    public object: T;
    constructor(data?: any) {
        this.route = data ? data.route : {};
        this.object = data ? data.object : {};
    }
}
