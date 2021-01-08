import { ColDef } from 'ag-grid-community';

export class AgGridConfigModel<T = any> {
  columnDefs: ColumDefModel[] | ColDef[];
  columnShowHide: any[];
  frameworkComponents: any;
  rowData: T[];
  selectedRows: any[];
  rowSelection: string;
  gridApi: any;
  gridColumnApi: any;
  context: any;
  defaultColDef: any = {
    resizable: true
  };
  rowClassRules: any;
  constructor(data?: Partial<AgGridConfigModel<T>>) {
    if (!data) {
      return;
    }
    this.columnDefs = data.columnDefs ? data.columnDefs : [];
    this.columnShowHide = data.columnShowHide ? data.columnShowHide : [];
    this.frameworkComponents = data.frameworkComponents
      ? data.frameworkComponents
      : undefined;
    this.rowData = data.rowData ? data.rowData : [];
    this.selectedRows = data.selectedRows ? data.selectedRows : [];
    this.rowSelection = data.rowSelection ? data.rowSelection : 'multiple';
    this.gridApi = data.gridApi ? data.gridApi : undefined;
    this.gridColumnApi = data.gridColumnApi ? data.gridColumnApi : undefined;
    this.context = data.context ? data.context : undefined;
    this.defaultColDef = data.defaultColDef ? data.defaultColDef : undefined;
    this.rowClassRules = data.rowClassRules ? data.rowClassRules : undefined;
  }
}

// tslint:disable-next-line:max-classes-per-file
export class ColumDefModel {
  headerName: string;
  field: string;
  colId: string;
  width: number;
  minWidth?: number;
  tooltipField?: string;
  maxWidth?: number;
  headerComponentParams: any;
  checkboxSelection: boolean;
  headerCheckboxSelection?: boolean;
  sortable: boolean;
  cellStyle: object;
  cellRenderer: string | any;
  suppressMenu: boolean;
  cellRendererParams: object;
  autoHeight: boolean;
  pinned: string;
  suppressSizeToFit: boolean;
  hide: boolean;
  headerClass?: string;
  cellClass?: string;
  lockPinned?: boolean;
  valueFormatter: (params: any) => any;
  constructor(data?: Partial<ColumDefModel>) {
    this.headerName = data.headerName ? data.headerName : '';
    this.field = data.field ? data.field : '';
    this.tooltipField = data.tooltipField ? data.tooltipField : undefined;
    this.colId = data.colId ? data.colId : '';
    this.width = data.width ? data.width : undefined;
    this.minWidth = data.minWidth ? data.minWidth : undefined;
    this.maxWidth = data.maxWidth ? data.maxWidth : undefined;
    this.headerComponentParams = data.headerComponentParams
      ? data.headerComponentParams
      : undefined;
    this.checkboxSelection = data.checkboxSelection
      ? data.checkboxSelection
      : false;
    this.headerCheckboxSelection = data.headerCheckboxSelection
      ? data.headerCheckboxSelection
      : false;
    this.sortable = data.sortable ? data.sortable : false;
    this.cellStyle = data.cellStyle ? data.cellStyle : undefined;
    this.cellRenderer = data.cellRenderer ? data.cellRenderer : undefined;
    this.suppressMenu = data.suppressMenu ? data.suppressMenu : undefined;
    this.cellRendererParams = data.cellRendererParams
      ? data.cellRendererParams
      : undefined;
    this.autoHeight = data.autoHeight ? data.autoHeight : false;
    this.pinned = data.pinned ? data.pinned : undefined;
    this.suppressSizeToFit = data.suppressSizeToFit
      ? data.suppressSizeToFit
      : false;
    this.valueFormatter = data.valueFormatter ? data.valueFormatter : undefined;
    this.hide = data.hide;
    this.headerClass = data.headerClass;
    this.cellClass = data.cellClass;
    this.lockPinned = data.lockPinned;
  }
}
