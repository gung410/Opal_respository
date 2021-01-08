import { AgGridAngular } from '@ag-grid-community/angular';
import { RowNode } from '@ag-grid-community/core';
import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { CxColumnSortType } from '@conexus/cx-angular-common';
import { UserService } from 'app-services/user.service';
import { AppConstant } from 'app/shared/app.constant';
import { TextOverflowTruncatedComponent } from 'app/shared/components/ag-grid-renderer/common/text-overflow-truncated/text-overflow-truncated.component';
import { NameRendererComponent } from 'app/shared/components/ag-grid-renderer/user/name-renderer/name-renderer.component';
import { RemoveUserRendererComponent } from 'app/shared/components/ag-grid-renderer/user/remove-user-renderer/remove-user-renderer.component';
import { BaseSmartComponent } from 'app/shared/components/component.abstract';
import { FilterParamModel } from 'app/staff/staff.container/staff-list/models/filter-param.model';
import { PagedStaffsList } from 'app/staff/staff.container/staff-list/models/staff.model';
import { ToastrService } from 'ngx-toastr';
import { ListType } from '../list.type';

@Component({
  selector: 'cx-people-list',
  templateUrl: './cx-people-list.component.html',
  styleUrls: ['./cx-people-list.component.scss'],
})
export class CxPeopleListComponent
  extends BaseSmartComponent
  implements OnInit {
  @Input()
  listType: ListType;
  @Input()
  allowDeletion: boolean = false;
  @Input()
  filterParams: FilterParamModel;
  @Input()
  showResults: boolean = true;
  @Input()
  defaultPageSize: number = AppConstant.ItemPerPageOnDialog;

  @Output()
  selectionChanged: EventEmitter<any[]> = new EventEmitter<any[]>();
  @Output()
  peopleRemoved: EventEmitter<any> = new EventEmitter<any>();

  defaultColDef: any;
  context: any;
  frameworkComponents: any;
  paginatedList: PagedStaffsList;
  sortField: string = '';
  sortDirection: CxColumnSortType = CxColumnSortType.UNSORTED;

  noRowsTemplate: '<div class="grid-nodata">No results found.</div>';
  columnDefs: any;
  rowClassRules: any;
  rowSelection: any;
  rowData: any = [];

  @ViewChild(AgGridAngular, { static: true }) grid: AgGridAngular;

  constructor(
    changeDetectorRef: ChangeDetectorRef,
    private userService: UserService,
    private toastrService: ToastrService
  ) {
    super(changeDetectorRef);

    this.context = { componentParent: this };
  }

  ngOnInit(): void {
    this.initColumnDefinition(this.listType, this.allowDeletion);
    this.initRowClassRules();
    this.frameworkComponents = {
      nameRenderer: NameRendererComponent,
      textOverflowTruncatedComponent: TextOverflowTruncatedComponent,
      removeUserRendererComponent: RemoveUserRendererComponent,
    };
  }

  onGridReady(params: any): void {
    if (this.showResults === true) {
      params.api.sizeColumnsToFit();
    }
  }

  onSelectionChanged(gridEvent: any): void {
    this.selectionChanged.emit(gridEvent.api.getSelectedRows());
  }

  getSelectedPeople(): any[] {
    return this.grid.api.getSelectedRows();
  }

  onPeopleRemoved(person: any): void {
    person.removing = true;
    this.peopleRemoved.emit(person);

    // Refresh the row data in order to get rowClassRules is triggered.
    this.grid.api.forEachNode((rowNode: RowNode, index: number) => {
      if (
        rowNode.data.user.identity.id === person.user.identity.id &&
        rowNode.data.removing === true
      ) {
        const newData = { ...rowNode.data };
        rowNode.setData(newData);
      }
    });
  }

  onSortChanged(event: any): void {
    const sortModels = this.grid.api.getSortModel();
    if (sortModels) {
      const sortModel = sortModels[0];
      this.sortDirection = sortModel
        ? sortModel.sort === 'asc'
          ? CxColumnSortType.ASCENDING
          : CxColumnSortType.DESCENDING
        : CxColumnSortType.UNSORTED;
      this.sortField = sortModel ? sortModel.colId : '';
      this.getList(this.filterParams);
    }
  }

  onCurrentPageChange(pageIndex: number): void {
    this.filterParams.pageIndex = pageIndex;
    this.getList(this.filterParams);
  }

  onPageSizeChange(pageSize: number): void {
    if (Number(pageSize) > Number(this.filterParams.pageSize)) {
      this.filterParams.pageIndex = 1;
    }
    this.filterParams.pageSize = +pageSize;
    this.getList(this.filterParams);
  }

  onFirstDataRendered(params: any): void {
    params.api.sizeColumnsToFit();
  }

  getList(filterParams: FilterParamModel): void {
    this.grid.api.showLoadingOverlay();
    this.showResults = true;

    // set filter params for idp/employeelist api
    this.addMoreFilterCondition(filterParams);
    // get employees
    this.subscription.add(
      this.userService.getListEmployee(filterParams).subscribe(
        (pagedEmployeeList: any) => {
          if (pagedEmployeeList) {
            pagedEmployeeList.items = pagedEmployeeList.items
              ? pagedEmployeeList.items
              : [];

            this.paginatedList = pagedEmployeeList;

            this.setRowData(this.paginatedList.items);
          }
          this.grid.api.hideOverlay();
          this.changeDetectorRef.detectChanges();
        },
        () => {
          this.toastrService.error(
            'Oops, Something went wrong, please try again.'
          );
          this.grid.api.hideOverlay();
          this.changeDetectorRef.detectChanges();
        }
      )
    );
  }
  private addMoreFilterCondition(filterParams: FilterParamModel): void {
    filterParams.sortField = this.sortField === '' ? undefined : this.sortField;
    filterParams.sortOrder =
      this.sortDirection === CxColumnSortType.UNSORTED
        ? undefined
        : this.sortDirection === CxColumnSortType.ASCENDING
        ? 'Ascending'
        : 'Descending';
  }
  private setRowData(users: any): void {
    if (users) {
      if (this.grid.columnApi) {
        this.rowData = new Array();
        users.forEach((user) => {
          this.rowData.push({
            name: {
              email: user.email,
              fullName: user.fullName,
              id: user.identity.id,
              avatarUrl: user.avatarUrl
                ? user.avatarUrl
                : AppConstant.defaultAvatar,
            },
            departmentName: user.department.name,
            serviceScheme:
              user.personnelGroups && user.personnelGroups.length > 0
                ? user.personnelGroups[0].name
                : '',
            user,
          });
        });
      }
    }
  }

  private isFirstColumn(params: any): boolean {
    const displayedColumns = params.columnApi.getAllDisplayedColumns();
    const thisIsFirstColumn = displayedColumns[0] === params.column;

    return thisIsFirstColumn;
  }

  private initRowClassRules(): void {
    this.rowClassRules = {
      'removing-row'(params: any): boolean {
        return params.data.removing === true;
      },
    };
  }

  private initColumnDefinition(
    listType: ListType,
    allowDeletion: boolean
  ): void {
    this.defaultColDef = this.buildDefaultColumnDefinition(listType);
    const showRemoveAction =
      listType === ListType.ViewingSelectedPeople && allowDeletion;
    this.columnDefs = this.buildColumnDefs(showRemoveAction);
  }

  private buildDefaultColumnDefinition(listType: ListType): any {
    const columnDefinition = {
      resizable: true,
      headerCheckboxSelection: undefined,
      checkboxSelection: undefined,
    };
    if (listType === ListType.PickingPeople) {
      columnDefinition.headerCheckboxSelection = this.isFirstColumn;
      columnDefinition.checkboxSelection = this.isFirstColumn;
      this.rowSelection = 'multiple';
    }

    return columnDefinition;
  }

  private buildColumnDefs(showRemoveAction: boolean): any[] {
    const columns: any[] = [
      {
        headerName: 'NAME',
        field: 'name',
        cellRenderer: 'nameRenderer',
        colId: 'firstName',
        sortable: true,
        sort: 'asc',
        hide: false,
        minWidth: 250,
        cellClass: 'text-overflow-overlay',
      },
      {
        headerName: 'ORGANISATION UNIT',
        field: 'departmentName',
        colId: 'departmentName',
        cellRenderer: 'textOverflowTruncatedComponent',
        hide: false,
        width: 120,
        headerClass: 'text-overflow-wrap',
        cellClass: 'text-overlay-truncated',
      },
      {
        headerName: 'SERVICE SCHEME',
        field: 'serviceScheme',
        colId: 'PersonnelGroup',
        hide: false,
        width: 65,
        headerClass: 'text-overflow-wrap',
        sortable: false,
      },
    ];

    if (showRemoveAction) {
      columns.push({
        headerName: '',
        field: 'user',
        cellRenderer: 'removeUserRendererComponent',
        colId: 'deleteCommand',
        hide: false,
        width: 50,
        headerClass: 'text-overflow-wrap',
        sortable: false,
      });
    }

    return columns;
  }
}
