export class CxTreeText {
    public header: string;
    public moveBtn: string;
    public removeBtn: string;
    public addTooltip: string;
    public editTooltip: string;
    public moveTooltip: string;
    public removeTooltip: string;
    public saveTooltip: string;
    public closeTooltip: string;
    constructor(data?: Partial<CxTreeText>) {
        if (!data) {
            this.header = 'Name';
            this.moveBtn = 'Move';
            this.removeBtn = 'Remove';
            this.addTooltip = 'Add';
            this.editTooltip = 'Edit';
            this.moveTooltip = 'Move';
            this.removeTooltip = 'Remove';
            this.saveTooltip = 'Save';
            this.closeTooltip = 'Close';
            return;
        }
        this.header = data.header ? data.header : 'Name';
        this.moveBtn = data.moveBtn ? data.moveBtn : 'Move';
        this.removeBtn = data.removeBtn ? data.removeBtn : 'Remove';
        this.addTooltip = data.addTooltip ? data.addTooltip : 'Add';
        this.editTooltip = data.editTooltip ? data.editTooltip : 'Edit';
        this.moveTooltip = data.moveTooltip ? data.moveTooltip : 'Move';
        this.removeTooltip = data.removeTooltip ? data.removeTooltip : 'Remove';
        this.saveTooltip = data.saveTooltip ? data.saveTooltip : 'Save';
        this.closeTooltip = data.closeTooltip ? data.closeTooltip : 'Close';
    }
}
