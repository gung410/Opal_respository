
export class CxPagingIcon {
    public next?: string;
    public previous?: string;
    constructor(icon?: Partial<CxPagingIcon>) {
        if (icon !== undefined) {
            this.next = icon.next
                ? icon.next : 'material-icons play-arrow-icon navigation-next';
            this.previous = icon.previous
                ? icon.previous : 'material-icons play-arrow-icon navigation-previous';
            return;
        }
        this.next = 'material-icons play-arrow-icon navigation-next';
        this.previous = 'material-icons play-arrow-icon navigation-previous';
    }
}

export class CxPagingText {
    public next?: string;
    public previous?: string;
    constructor(text?: Partial<CxPagingText>) {
        if (text !== undefined) {
            this.next = text.next
                ? text.next : 'Next';
            this.previous = text.previous
                ? text.previous : 'Previous';
            return;
        }
        this.next = 'Next';
        this.previous = 'Previous';
    }
}
