import { AttendanceStatus, RegistrationStatus, SearchRegistrationsType, UserInfoModel } from '@opal20/domain-api';
import { BaseGridComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { CellClickEvent, GridDataResult, RowClassArgs } from '@progress/kendo-angular-grid';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Observable, Subscription } from 'rxjs';

import { ATTENDANCE_TRACKING_STATUS_COLOR_MAP } from './../../models/attendance-tracking-status-color-map.model';
import { AbsenceDetailDialogComponent } from '../absence-detail-dialog/absence-detail-dialog.component';
import { AttendanceTrackingDetailViewModel } from '../../view-models/attendance-tracking-detail-view.model';
import { AttendanceTrackingViewModel } from './../../models/attendance-tracking-view.model';
import { IRowCallbackModel } from '../../models/row-callback.model';
import { ListAttendanceTrackingGridComponentService } from '../../services/list-attendance-tracking.service';
import { OpalDialogService } from '@opal20/common-components';
@Component({
  selector: 'list-attendance-tracking-grid',
  templateUrl: './list-attendance-tracking-grid.component.html'
})
export class ListAttendanceTrackingGridComponent extends BaseGridComponent<AttendanceTrackingViewModel> {
  public get sessionId(): string | undefined {
    return this._sessionId;
  }

  @Input()
  public set sessionId(v: string | undefined) {
    if (Utils.isDifferent(this._sessionId, v)) {
      this._sessionId = v;
      if (this.initiated) {
        this.refreshData();
      }
    }
  }

  public get classRunId(): string | undefined {
    return this._classRunId;
  }

  @Input()
  public set classRunId(v: string | undefined) {
    if (Utils.isDifferent(this._classRunId, v)) {
      this._classRunId = v;
    }
  }

  public get AttendanceTrackingBySessionId(): AttendanceTrackingDetailViewModel | undefined {
    return this._attendanceTrackingBySessionID;
  }

  public get statusColorMap(): unknown {
    return ATTENDANCE_TRACKING_STATUS_COLOR_MAP;
  }

  @Input()
  public set AttendanceTrackingBySessionId(v: AttendanceTrackingDetailViewModel | undefined) {
    if (Utils.isDifferent(this._attendanceTrackingBySessionID, v)) {
      this._attendanceTrackingBySessionID = v;
    }
  }

  @Output() public loadedData: EventEmitter<GridDataResult> = new EventEmitter();

  @Output('viewAttendanceTracking') public viewAttendanceTrackingEvent: EventEmitter<AttendanceTrackingViewModel> = new EventEmitter<
    AttendanceTrackingViewModel
  >();

  public SearchRegistrationsType: typeof SearchRegistrationsType = SearchRegistrationsType;
  public RegistrationStatus: typeof RegistrationStatus = RegistrationStatus;
  public AttendanceStatus: typeof AttendanceStatus = AttendanceStatus;

  public query: Observable<unknown>;
  public loading: boolean;
  public name: string;
  public organization: string;
  public reasonForAbsence: string;
  public attendanceStatus: string;

  public dicDisplayColumns: Dictionary<boolean> = {};

  private _loadDataSub: Subscription = new Subscription();
  private _sessionId: string;
  private _classRunId: string;
  private _attendanceTrackingBySessionID: AttendanceTrackingDetailViewModel;

  private currentUser = UserInfoModel.getMyUserInfo();
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public opalDialogService: OpalDialogService,
    public listAttendanceTrackingGridComponentService: ListAttendanceTrackingGridComponentService
  ) {
    super(moduleFacadeService);
  }

  public rowCallback(context: RowClassArgs): IRowCallbackModel {
    return {
      selected: context.dataItem.selected
    };
  }

  public onGridCellClick(event: CellClickEvent): void {
    // columnIndex isn't action column
    if (event.dataItem instanceof AttendanceTrackingViewModel && !this.indexActionColumns.includes(event.columnIndex)) {
      this.viewAttendanceTrackingEvent.emit(event.dataItem);
    }
  }

  public loadData(): void {
    if (this.sessionId != null) {
      this._loadDataSub.unsubscribe();
      this._loadDataSub = this.listAttendanceTrackingGridComponentService
        .loadSearchAttendanceTracking(
          this.sessionId,
          this.filter.search,
          this.filter.filter,
          this.state.skip,
          this.state.take,
          this.checkAll,
          () => this.selecteds
        )
        .pipe(this.untilDestroy())
        .subscribe(data => {
          if (data) {
            this.gridData = data;
            this.loadedData.emit(this.gridData);
            this.updateSelectedsAndGridData();
          }
        });
    }
  }

  public getReasonData(dataItem: AttendanceTrackingViewModel): string {
    if (dataItem.status === AttendanceStatus.Present) {
      return '';
    }
    if (dataItem.status === AttendanceStatus.Absent && !dataItem.reasonForAbsence) {
      return this.translateCommon('Pending learner’s submission for reason');
    }
    if (dataItem.status === AttendanceStatus.Absent && dataItem.reasonForAbsence) {
      return dataItem.reasonForAbsence;
    }

    return '';
  }

  public viewAbsenceDetail(dataItem: AttendanceTrackingViewModel): void {
    if (dataItem.hasViewReasonForAbsentPermission(this.currentUser)) {
      this.opalDialogService.openDialogRef(AbsenceDetailDialogComponent, {
        title: `${this.translate('Reason for Absence')}`,
        reasonForAbsence: dataItem.reasonForAbsence,
        attachment: dataItem.attachment
      });
    }
  }

  protected onInit(): void {
    super.onInit();
  }
}
