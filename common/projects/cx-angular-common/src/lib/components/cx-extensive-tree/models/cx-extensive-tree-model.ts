export class CxExtensiveTreeModel {
    public object: any;
    public showChildren: boolean;
    public isDisplay: boolean;
    public level: number;
    public isEditingItem: boolean;
    public isAddingNewItem: boolean;
    public isLoadChildren: boolean;
    public children: any;
    public isSelected: boolean;
    constructor(data?: Partial<CxExtensiveTreeModel>) {
        if (!data) { return; }
        this.object = data.object ? data.object : '';
        this.showChildren = data.showChildren;
        this.level = data.level;
        this.isSelected = data.isSelected;
        this.children = data.children;
        this.isDisplay = data.isDisplay;
        this.isEditingItem = data.isEditingItem;
        this.isAddingNewItem = data.isAddingNewItem;
        this.isLoadChildren = data.isLoadChildren;
    }
}
