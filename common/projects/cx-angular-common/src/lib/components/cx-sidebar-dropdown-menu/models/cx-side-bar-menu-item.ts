export class CxSideBarMenuItem {
    public id: string;
    public content: string;
    public children: Array<CxSideBarMenuItem>;
    public isDisplay: boolean;
    public role: Array<string>;
    public isCollapsed: boolean;
    public level: number;

    constructor(data?: Partial<CxSideBarMenuItem>) {
        if (!data) {
          this.content = '';
          this.children = [];
          this.isDisplay = true;
          this.role = [];
          this.isCollapsed = true;
          this.level = 1;
          return;
        }
        this.level = data.level ? data.level : 1;
        this.content = data.content ? data.content : '';
        this.children = data.children ? data.children : [];
        this.isDisplay = data.isDisplay ? data.isDisplay : true;
        this.role = data.role ? data.role : [];
        this.isCollapsed = data.isCollapsed ? data.isCollapsed : true;
    }
}

