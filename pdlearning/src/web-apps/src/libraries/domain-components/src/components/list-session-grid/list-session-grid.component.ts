import { BaseGridComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import {
  BookingSource,
  ClassRun,
  Course,
  CourseStatus,
  IChangeLearningMethodRequest,
  SearchClassRunType,
  SearchSessionType,
  Session,
  SessionRepository,
  UserInfoModel,
  WebinarApiService
} from '@opal20/domain-api';
import { CellClickEvent, GridDataResult, RowClassArgs } from '@progress/kendo-angular-grid';
import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { ContextMenuItem, DialogAction, IOpalSelectDefaultItem, OpalDialogService } from '@opal20/common-components';
import { Observable, Subscription, combineLatest, from, of } from 'rxjs';

import { COURSE_STATUS_COLOR_MAP } from '../../models/course-status-color-map.model';
import { ContextMenuAction } from './../../models/context-menu-action.model';
import { ContextMenuEmit } from './../../models/context-menu-emit.model';
import { ContextMenuSelectEvent } from '@progress/kendo-angular-menu';
import { DropDownListComponent } from '@progress/kendo-angular-dropdowns';
import { IRowCallbackModel } from '../../models/row-callback.model';
import { ListSessionGridComponentService } from '../../services/list-session-grid-component.service';
import { SessionViewModel } from './../../models/session-view.model';
import { switchMap } from 'rxjs/operators';

@Component({
  selector: 'list-session-grid',
  templateUrl: './list-session-grid.component.html'
})
export class ListSessionGridComponent extends BaseGridComponent<SessionViewModel> {
  public get classRunId(): string | undefined {
    return this._classRunId;
  }

  @Input()
  public set classRunId(v: string | undefined) {
    if (Utils.isDifferent(this._classRunId, v)) {
      this._classRunId = v;
    }
  }

  public get classRun(): ClassRun | undefined {
    return this._classRun;
  }

  @Input()
  public set classRun(v: ClassRun | undefined) {
    if (Utils.isDifferent(this._classRun, v)) {
      this._classRun = v;
    }
  }

  public get course(): Course | undefined {
    return this._course;
  }

  @Input()
  public set course(v: Course | undefined) {
    if (Utils.isDifferent(this._course, v)) {
      this._course = v;
    }
  }

  @Input() public isRescheduleMode: boolean = false;
  @Input() public searchType: SearchSessionType = SearchSessionType.Owner;
  @Input() public classRunSearchType: SearchClassRunType;
  @Input() public indexActionColumn: number = null;
  @Input() public disableChangeLearningMethod: boolean = false;
  @Output() public loadedData: EventEmitter<GridDataResult> = new EventEmitter();
  @Output() public selectedContextMenu: EventEmitter<ContextMenuEmit<Session>> = new EventEmitter();
  @Output('viewSession') public viewSessionEvent: EventEmitter<SessionViewModel> = new EventEmitter<SessionViewModel>();
  @ViewChild('dropdownlist', { static: true }) public dropdownlist: DropDownListComponent;
  public CourseStatus: typeof CourseStatus = CourseStatus;

  public query: Observable<unknown>;
  public loading: boolean = false;
  public courseStatusColorMap = COURSE_STATUS_COLOR_MAP;
  public SearchClassRunType: typeof SearchClassRunType = SearchClassRunType;
  public maxMinutesCanJoinWebinarEarly: number = 0;
  public learningMethodTypes: IOpalSelectDefaultItem<boolean>[] = [{ value: true, label: 'Online' }, { value: false, label: 'Offline' }];
  public dicDisplayColumns: Dictionary<boolean>;

  public contextMenuItems: ContextMenuItem[] = [
    {
      id: ContextMenuAction.Delete,
      text: this.translateCommon('Delete'),
      icon: 'delete'
    }
  ];

  @Input()
  public set displayColumns(displayColumns: ListSessionGridDisplayColumns[]) {
    this._displayColumns = displayColumns;
    this.dicDisplayColumns = Utils.toDictionarySelect(this._displayColumns, p => p, _ => true);
  }

  private _displayColumns: ListSessionGridDisplayColumns[] = [
    ListSessionGridDisplayColumns.title,
    ListSessionGridDisplayColumns.createdDate,
    ListSessionGridDisplayColumns.sessionDate,
    ListSessionGridDisplayColumns.learningTime,
    ListSessionGridDisplayColumns.sessionVenue,
    ListSessionGridDisplayColumns.learningMethod,
    ListSessionGridDisplayColumns.webinar,
    ListSessionGridDisplayColumns.actions
  ];

  private _loadDataSub: Subscription = new Subscription();
  private _classRun: ClassRun = new ClassRun();
  private _course: Course = new Course();
  private currentUser: UserInfoModel = UserInfoModel.getMyUserInfo();
  private _classRunId: string;

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public listSessionGridComponentService: ListSessionGridComponentService,
    private opalDialogService: OpalDialogService,
    private sessionRepository: SessionRepository,
    private webinarApiService: WebinarApiService
  ) {
    super(moduleFacadeService);
    this.dicDisplayColumns = Utils.toDictionarySelect(this._displayColumns, p => p, _ => true);
  }

  public rowCallback(context: RowClassArgs): IRowCallbackModel {
    return {
      selected: context.dataItem.selected
    };
  }

  public loadData(): void {
    this._loadDataSub.unsubscribe();
    this._loadDataSub = combineLatest(
      this.listSessionGridComponentService.loadSessionsByClassRunId(
        this.classRunId,
        this.searchType,
        this.state.skip,
        this.state.take,
        this.checkAll,
        () => this.selecteds
      ),
      from(this.sessionRepository.loadMaxMinutesCanJoinWebinarEarly())
    )
      .pipe(this.untilDestroy())
      .subscribe(([gridData, limitTime]) => {
        this.gridData = gridData;
        this.maxMinutesCanJoinWebinarEarly = limitTime;
        this.loadedData.emit(this.gridData);
        this.updateSelectedsAndGridData();
      });
  }

  public onGridCellClick(event: CellClickEvent): void {
    if (event.dataItem instanceof SessionViewModel && !this.indexActionColumns.includes(event.columnIndex)) {
      this.viewSessionEvent.emit(event.dataItem);
    }
  }

  public getContextMenuBySession(rowData: Session): ContextMenuItem[] {
    return this.contextMenuItems.filter(
      item =>
        rowData.canBeModified(this.classRun, this.course) &&
        rowData.hasModifiedPermission(this.course, this.currentUser) &&
        item.id === ContextMenuAction.Delete
    );
  }

  public onSelectedContextMenu(event: ContextMenuSelectEvent, dataItem: Session, rowIndex: number): void {
    this.selectedContextMenu.emit(new ContextMenuEmit(event, dataItem, rowIndex));
  }

  public canChangeLearningMethod(dataItem: Session): boolean {
    return dataItem.canChangeLearningMethod() && !this.disableChangeLearningMethod;
  }

  public changeConfirmationFn(dataItem: Session): (selectedValue: boolean) => Promise<boolean> {
    return (selectedValue: boolean) =>
      this.opalDialogService
        .openConfirmDialog({
          confirmTitle: 'Confirm new "Learning Method"',
          confirmMsgHtml: 'Do you really want to change the Learning Method? This action will affect the learning venue.',
          noBtnText: 'Cancel',
          yesBtnText: 'Proceed'
        })
        .pipe(
          switchMap(confirmed => {
            if (confirmed === DialogAction.OK) {
              return from(
                this.changeLearningMethod(
                  Utils.clone(dataItem, item => {
                    item.learningMethod = selectedValue;
                    return item;
                  })
                ).then(_ => {
                  this.showNotification(this.translate('Switched successfully'));
                  return true;
                })
              );
            }
            return of(false);
          })
        )
        .toPromise();
  }

  public joinWebinar(sessionId: string): Promise<void> {
    return this.webinarApiService.getJoinURL(sessionId, BookingSource.Course).then(result => {
      if (result.isSuccess) {
        window.open(result.joinUrl);
      }
    });
  }

  public showTitleColumn(): boolean {
    return this.dicDisplayColumns[ListSessionGridDisplayColumns.title];
  }

  public showCreatedDateColumn(): boolean {
    return this.dicDisplayColumns[ListSessionGridDisplayColumns.createdDate];
  }

  public showSessionDateColumn(): boolean {
    return this.dicDisplayColumns[ListSessionGridDisplayColumns.sessionDate];
  }

  public showLearningTimeColumn(): boolean {
    return this.dicDisplayColumns[ListSessionGridDisplayColumns.learningTime];
  }

  public showSessionVenueColumn(): boolean {
    return this.dicDisplayColumns[ListSessionGridDisplayColumns.sessionVenue];
  }

  public showLearningMethodColumn(): boolean {
    return this.dicDisplayColumns[ListSessionGridDisplayColumns.learningMethod];
  }

  public showWebinarColumn(): boolean {
    return this.dicDisplayColumns[ListSessionGridDisplayColumns.webinar];
  }

  public showActionColumn(): boolean {
    return this.dicDisplayColumns[ListSessionGridDisplayColumns.actions];
  }

  protected onInit(): void {
    super.onInit();
  }

  private changeLearningMethod(session: Session): Promise<void> {
    return new Promise((resolve, reject) => {
      const request: IChangeLearningMethodRequest = {
        id: session.id,
        learningMethod: session.learningMethod
      };
      this.sessionRepository.changeLearningMethod(request).then(p => {
        resolve(p);
      }, reject);
    });
  }
}

export enum ListSessionGridDisplayColumns {
  title = 'title',
  createdDate = 'createdDate',
  sessionDate = 'sessionDate',
  learningTime = 'learningTime',
  sessionVenue = 'sessionVenue',
  learningMethod = 'learningMethod',
  webinar = 'webinar',
  actions = 'actions'
}
