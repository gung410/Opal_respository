export class CxTreeIcon {
    public add: string;
    public edit: string;
    public move: string;
    public remove: string;
    public save: string;
    public close: string;
    public expand: string;
    public collapse: string;
    public root: string;
    public node: string;
    public emptyNode: string;
    public indeterminate: string;
    constructor(data?: Partial<CxTreeIcon>) {
        if (!data) {
            this.add = 'add-icon';
            this.edit = 'edit-icon';
            this.move = 'move-icon';
            this.remove = 'remove-icon';
            this.save = 'save-icon';
            this.close = 'close-icon';
            this.expand = 'expand-icon';
            this.collapse = 'collapse-icon';
            this.root = 'root-icon';
            this.node = 'node-icon';
            this.emptyNode = 'empty-node-icon';
            this.indeterminate = 'indeterminate-icon';
            return;
        }
        this.add = data.add ? data.add : 'add-icon';
        this.edit = data.edit ? data.edit : 'edit-icon';
        this.move = data.move ? data.move : 'move-icon';
        this.remove = data.remove ? data.remove : 'remove-icon';
        this.save = data.save ? data.save : 'save-icon';
        this.close = data.close ? data.close : 'close-icon';
        this.expand = data.expand ? data.expand : 'expand-icon';
        this.collapse = data.collapse ? data.collapse : 'collapse-icon';
        this.root = data.root ? data.root : 'root-icon';
        this.node = data.node ? data.node : 'node-icon';
        this.emptyNode = data.emptyNode ? data.emptyNode : 'empty-node-icon';
        this.indeterminate = data.indeterminate ? data.indeterminate : 'indeterminate-icon';
    }
}
