import { BaseGridComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Observable, Subscription } from 'rxjs';
import { SearchECertificateType, UserInfoModel } from '@opal20/domain-api';

import { CellClickEvent } from '@progress/kendo-angular-grid';
import { ContextMenuAction } from './../../models/context-menu-action.model';
import { ContextMenuEmit } from './../../models/context-menu-emit.model';
import { ContextMenuItem } from '@opal20/common-components';
import { ContextMenuSelectEvent } from '@progress/kendo-angular-menu';
import { ECERTIFICATE_STATUS_COLOR_MAP } from './../../models/ecertificate-status-color-map.model';
import { ECertificateViewModel } from './../../models/ecertificate-view.model';
import { ListECertificateGridComponentService } from './../../services/list-ecertificate-grid-component.service';

@Component({
  selector: 'list-ecertificate-grid',
  templateUrl: './list-ecertificate-grid.component.html'
})
export class ListECertificateGridComponent extends BaseGridComponent<ECertificateViewModel> {
  @Input() public searchType: SearchECertificateType = SearchECertificateType.CustomECertificateTemplateManagement;
  @Input() public contextMenuItems: ContextMenuItem[] = [];
  @Output() public selectedContextMenu: EventEmitter<ContextMenuEmit<ECertificateViewModel>> = new EventEmitter();
  @Input() public set ignoreColumns(ignoreColumns: ListECertificateTemplateGridDisplayColumns[]) {
    this._ignoreColumns = ignoreColumns;
    if (this.initiated) {
      this.updateIgnoreColumnsDict();
    }
  }

  @Output('viewECertificate')
  public viewECertificateEvent: EventEmitter<ECertificateViewModel> = new EventEmitter<ECertificateViewModel>();

  public checkCourseContent: boolean = true;
  public query: Observable<unknown>;
  public loading: boolean;
  public dicIgnoreColumns: Dictionary<boolean> = {};

  private _loadDataSub: Subscription = new Subscription();
  private currentUser = UserInfoModel.getMyUserInfo();
  private _ignoreColumns: ListECertificateTemplateGridDisplayColumns[] = [];
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private listECertificateGridComponentService: ListECertificateGridComponentService
  ) {
    super(moduleFacadeService);
  }

  public getContextMenuByECertificateTemplate(rowData: ECertificateViewModel): ContextMenuItem[] {
    return this.contextMenuItems.filter(
      item => rowData.canDeleteECertificateTemplate(this.currentUser) && item.id === ContextMenuAction.Delete
    );
  }

  public onSelectedContextMenu(event: ContextMenuSelectEvent, dataItem: ECertificateViewModel, rowIndex: number): void {
    this.selectedContextMenu.emit(new ContextMenuEmit(event, dataItem, rowIndex));
  }

  public loadData(): void {
    this._loadDataSub.unsubscribe();
    this._loadDataSub = this.listECertificateGridComponentService
      .loadECertificates(this.filter.search, this.searchType, this.state.skip, this.state.take, this.checkAll, () => this.selecteds)
      .pipe(this.untilDestroy())
      .subscribe(data => {
        this.gridData = data;
        this.updateSelectedsAndGridData();
      });
  }

  public onGridCellClick(event: CellClickEvent): void {
    if (event.dataItem instanceof ECertificateViewModel && !this.indexActionColumns.includes(event.columnIndex)) {
      this.viewECertificateEvent.emit(event.dataItem);
    }
  }

  public get statusColorMap(): unknown {
    return ECERTIFICATE_STATUS_COLOR_MAP;
  }

  public showTitleColumn(): boolean {
    return !this.dicIgnoreColumns[ListECertificateTemplateGridDisplayColumns.title];
  }

  public showTotalCoursesUsingColumn(): boolean {
    return !this.dicIgnoreColumns[ListECertificateTemplateGridDisplayColumns.totalCoursesUsing];
  }

  public showTotalLearnersReceivedColumn(): boolean {
    return !this.dicIgnoreColumns[ListECertificateTemplateGridDisplayColumns.totalLearnersReceived];
  }

  public showStatusColumn(): boolean {
    return !this.dicIgnoreColumns[ListECertificateTemplateGridDisplayColumns.status];
  }

  public showActionColumn(): boolean {
    return !this.dicIgnoreColumns[ListECertificateTemplateGridDisplayColumns.actions];
  }

  protected onInit(): void {
    super.onInit();
    this.updateIgnoreColumnsDict();
  }

  private updateIgnoreColumnsDict(): void {
    this.dicIgnoreColumns = Utils.toDictionarySelect(this._ignoreColumns, p => p, p => true);
  }
}

export enum ListECertificateTemplateGridDisplayColumns {
  selected = 'selected',
  title = 'title',
  totalCoursesUsing = 'totalCoursesUsing',
  totalLearnersReceived = 'totalLearnersReceived',
  status = 'status',
  actions = 'actions'
}
