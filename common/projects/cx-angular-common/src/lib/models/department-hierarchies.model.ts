import { CxTreeIcon } from '../components/cx-tree/models/cx-tree-icon.model';
import { CxTreeText } from '../components/cx-tree/models/cx-tree-text.model';
import { CxTreeButtonCondition } from '../components/cx-tree/models/cx-tree-button-condition.model';

export class DepartmentHierarchiesModel {
  isShowSearchResult: boolean;
  isDetectExpandTree: boolean;
  isNoSearchResult: boolean;
  isDisplayOrganisationNavigation: boolean;
  havingExtensiveArea: boolean;
  isViewMode: boolean;
  enableSearch: boolean;
  selectedObjectIds: any;
  searchDepartmentResult: any;
  departmentPathMap: any = {};
  departments: any[] = [];
  currentDepartmentId: number;
  noResultFoundMessage: string;
  idFieldRoute: string;
  parentIdFieldRoute: string;
  treeHeader: string;
  icon: CxTreeIcon = new CxTreeIcon();
  text: CxTreeText = new CxTreeText();

  constructor(data?: Partial<DepartmentHierarchiesModel>) {
    if (!data) { return; }
    this.noResultFoundMessage = data.noResultFoundMessage;
    this.departments = data.departments;
    this.isShowSearchResult = data.isShowSearchResult;
    this.isDetectExpandTree = data.isDetectExpandTree;
    this.isNoSearchResult = data.isNoSearchResult;
    this.isDisplayOrganisationNavigation = data.isDisplayOrganisationNavigation;
    this.havingExtensiveArea = data.havingExtensiveArea;
    this.selectedObjectIds = data.selectedObjectIds;
    this.currentDepartmentId = data.currentDepartmentId;
    this.searchDepartmentResult = data.searchDepartmentResult;
    this.departmentPathMap = data.departmentPathMap;
    this.idFieldRoute = data.idFieldRoute;
    this.parentIdFieldRoute = data.parentIdFieldRoute;
    this.enableSearch = data.enableSearch;
    this.treeHeader = data.treeHeader;
    this.icon = data.icon;
    this.text = data.text;
    this.isViewMode = data.isViewMode;
  }
}
