export class CxTreeButtonCondition<T> {
    public enableAdd: (x: T) => boolean;
    public enableEdit: (x: T) => boolean;
    public enableMove: (x: T) => boolean;
    public enableRemove: (x: T) => boolean;
    public showCollapseExpand: (x: T) => boolean;
    constructor(data?: Partial<CxTreeButtonCondition<T>>) {
        if (!data) {
            this.enableAdd =  x => true;
            this.enableEdit =  x => true;
            this.enableMove =  x => true;
            this.enableRemove =  x => true;
            this.showCollapseExpand = x => true;
            return;
        }
        this.enableAdd = data.enableAdd ? data.enableAdd : x => true;
        this.enableEdit = data.enableEdit ? data.enableEdit : x => true;
        this.enableMove = data.enableMove ? data.enableMove : x => true;
        this.enableRemove = data.enableRemove ? data.enableRemove : x => true;
        this.showCollapseExpand = data.showCollapseExpand ? data.showCollapseExpand : x => true;
    }
}
