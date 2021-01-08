import { CxTableIcon, CxTableHeaderModel } from '../cx-table.model';

export class CxTableContainersIcon extends CxTableIcon {
    public moveGlobalClass?: string;
    public containerEmptyClass?: string;
    public containerHasChildClass?: string;
    constructor(icon?: Partial<CxTableContainersIcon>) {
        super(icon);
        if (icon !== undefined) {
            this.moveGlobalClass = icon.moveGlobalClass
                ? icon.moveGlobalClass : 'material-icons move-icon-global';
            this.containerEmptyClass = icon.containerEmptyClass
            ? icon.containerEmptyClass : 'material-icons folder-open';
            this.containerHasChildClass = icon.containerHasChildClass
            ? icon.containerHasChildClass : 'material-icons folder';
            return;
        }
        this.moveGlobalClass = 'material-icons move-icon-global';
        this.containerEmptyClass = 'material-icons folder-open';
        this.containerHasChildClass = 'material-icons folder';
    }
}

export class CxItemTableHeaderModel extends CxTableHeaderModel {
    constructor(data?: Partial<CxItemTableHeaderModel>) {
        super(data);
    }
}

