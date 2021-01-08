export class CxNavbarItemModel {
  id: string;
  content: string;
  children: Array<CxNavbarItemModel>;
  isCollapsed: boolean;
  route: string;
  icon: string;
  iconActive: string;
  isActive: boolean;
  isDefault: boolean;

  constructor(data?: Partial<CxNavbarItemModel>) {
    if (!data) {
      this.content = '';
      this.children = [];
      this.isCollapsed = true;
      this.isActive = false;
      this.route = '';
      this.icon = '';
      this.iconActive = '';
      return;
    }
    this.icon = data.icon ? data.icon : '';
    this.iconActive = data.iconActive ? data.iconActive : '';
    this.route = data.route ? data.route : '';
    this.content = data.content ? data.content : '';
    this.children = data.children ? data.children : [];
    this.isCollapsed = data.isCollapsed !== undefined ? data.isCollapsed : true;
    this.isActive = data.isActive !== undefined ? data.isActive : false;
  }

  matchUrl(checkingUrl: string): boolean {
    return checkingUrl && checkingUrl.includes(this.route);
  }

}
