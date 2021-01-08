export class DepartmentQueryModel {
  includeParent: boolean;
  includeChildren: boolean;
  countChildren?: boolean;
  maxChildrenLevel?: number;
  searchText: string;
  constructor(data?: Partial<DepartmentQueryModel>) {
    if (!data) {
      return;
    }
    this.includeChildren = data.includeChildren;
    this.includeParent = data.includeParent;
    this.searchText = data.searchText;
    this.countChildren = data.countChildren;
    this.maxChildrenLevel = data.maxChildrenLevel;
  }
}
