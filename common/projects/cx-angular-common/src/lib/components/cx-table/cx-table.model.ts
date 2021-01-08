
export class CxTableIcon {
    public moreActionsClass?: string;
    public removeClass?: string;
    constructor(icon?: Partial<CxTableIcon>) {
        if (icon !== undefined) {
            this.moreActionsClass = icon.moreActionsClass
                ? icon.moreActionsClass : 'material-icons more-vert';
            this.removeClass = icon.removeClass
                ? icon.removeClass : 'material-icons remove-icon';
            return;
        }
        this.moreActionsClass = 'material-icons more-vert';
        this.removeClass = 'material-icons remove-icon';
    }
}

export enum CxColumnSortType {
    UNSORTED,
    ASCENDING,
    DESCENDING
}

export const CxColumnSortTypeText = [
    { key: CxColumnSortType.UNSORTED, description: '' },
    { key: CxColumnSortType.ASCENDING, description: 'Asc' },
    { key: CxColumnSortType.DESCENDING, description: 'Desc' },
];

export class CxTableHeaderModel {
    public text?: string;
    public fieldSort?: string;
    public sortType?: CxColumnSortType;
    public width?: number;
    constructor(data?: Partial<CxTableHeaderModel>) {
        if (!data) {
            this.text = '';
            this.fieldSort = '';
            return;
        }
        this.text = data.text ? data.text : '';
        this.sortType = data.sortType ? data.sortType : CxColumnSortType.UNSORTED;
        this.fieldSort = data.fieldSort ? data.fieldSort : '';
        this.width = data.width ? data.width : undefined;
    }
}
export class CxTableLabelModel {
    public selectedItemLabel?: string;

    constructor(data?: Partial<CxTableLabelModel>) {
        if (!data) {
            this.selectedItemLabel = 'selected';
            return;
        }

        this.selectedItemLabel = data.selectedItemLabel ? data.selectedItemLabel : 'selected';
    }
}
