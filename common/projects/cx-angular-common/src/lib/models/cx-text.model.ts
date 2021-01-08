export class CxText {
    public addTooltip: string;
    public editTooltip: string;
    public moveTooltip: string;
    public removeTooltip: string;
    public saveTooltip: string;
    public closeTooltip: string;
    constructor(data?: Partial<CxText>) {
        if (!data) {
            this.addTooltip = 'Add';
            this.editTooltip = 'Edit';
            this.moveTooltip = 'Move';
            this.removeTooltip = 'Remove';
            this.saveTooltip = 'Save';
            this.closeTooltip = 'Close';
            return;
        }
        this.addTooltip = data.addTooltip ? data.addTooltip : 'Add';
        this.editTooltip = data.editTooltip ? data.editTooltip : 'Edit';
        this.moveTooltip = data.moveTooltip ? data.moveTooltip : 'Move';
        this.removeTooltip = data.removeTooltip ? data.removeTooltip : 'Remove';
        this.saveTooltip = data.saveTooltip ? data.saveTooltip : 'Save';
        this.closeTooltip = data.closeTooltip ? data.closeTooltip : 'Close';
    }
}
