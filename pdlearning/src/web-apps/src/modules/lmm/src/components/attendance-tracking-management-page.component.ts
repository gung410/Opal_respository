import {
  AttendanceStatus,
  AttendanceTracking,
  AttendanceTrackingRepository,
  ClassRun,
  Course,
  IAttendanceTrackingStatusRequest,
  SearchSessionType,
  Session,
  SessionRepository,
  UserInfoModel
} from '@opal20/domain-api';
import {
  AttendanceTrackingViewModel,
  IOpalReportDynamicParams,
  OpalReportDynamicComponent,
  RegistrationFilterComponent,
  RegistrationFilterModel
} from '@opal20/domain-components';
import { BasePageComponent, ComponentType, IGridFilter, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, EventEmitter, HostBinding, Input, Output } from '@angular/core';

import { Constant } from '@opal20/authentication';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { Observable } from 'rxjs';
import { OpalDialogService } from '@opal20/common-components';
import { QRCodeDialogComponent } from './dialogs/qr-code-dialog.component';
import { map } from 'rxjs/operators';

@Component({
  selector: 'attendance-tracking-management-page',
  templateUrl: './attendance-tracking-management-page.component.html'
})
export class AttendanceTrackingManagementPageComponent extends BasePageComponent {
  @Input() public stickyDependElement: HTMLElement;
  public filterPopupContent: ComponentType<RegistrationFilterComponent> = RegistrationFilterComponent;
  public sessionItems: Session[] = [];
  public gridData: GridDataResult;
  public paramsReportDynamic: IOpalReportDynamicParams | null;
  public onShowExportBtn: boolean;
  public attendanceStatusValue: string;
  public attendanceTrackingSelectedItems: AttendanceTrackingViewModel[] = [];
  public searchText: string = '';
  public filterData: RegistrationFilterModel = null;
  public filter: IGridFilter = {
    search: '',
    filter: null
  };

  public get classRunId(): string | undefined {
    return this._classRunId;
  }

  @Input()
  public set classRunId(v: string | undefined) {
    if (Utils.isDifferent(this._classRunId, v)) {
      this._classRunId = v;
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

  public get classRun(): ClassRun | undefined {
    return this._classRun;
  }

  @Input()
  public set classRun(v: ClassRun | undefined) {
    if (Utils.isDifferent(this._classRun, v)) {
      this._classRun = v;
    }
  }

  public get selectedSession(): Session | null {
    return this._selectedSession;
  }

  public set selectedSession(v: Session | null) {
    this._selectedSession = v;
  }

  @Output('viewAttendanceTracking')
  public viewAttendanceTrackingEvent: EventEmitter<AttendanceTrackingViewModel> = new EventEmitter<AttendanceTrackingViewModel>();
  public fetchSessionsFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<Session[]> = null;
  private _selectedSession: Session | null;
  private _classRun: ClassRun;
  private _course: Course;
  private _classRunId: string;

  private currentUser = UserInfoModel.getMyUserInfo();

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private sessionRepository: SessionRepository,
    private attendanceTrackingRepository: AttendanceTrackingRepository,
    private dialogService: OpalDialogService
  ) {
    super(moduleFacadeService);
    this.fetchSessionsFn = this.createFetchSessionsFn();
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  public onSubmitSearch(): void {
    this.filter = {
      ...this.filter,
      search: this.searchText
    };
  }

  public onApplyFilter(data: RegistrationFilterModel): void {
    this.filterData = data;

    this.filter = {
      ...this.filter,
      filter: this.filterData ? this.filterData.convert() : null
    };
  }

  public onPresent(): void {
    this.changeAttendanceStatus(AttendanceStatus.Present);
  }

  public onAbsent(): void {
    this.changeAttendanceStatus(AttendanceStatus.Absent);
  }

  public changeAttendanceStatus(status: AttendanceStatus): void {
    if (this.selectedSession != null) {
      const request: IAttendanceTrackingStatusRequest = {
        sessionId: this.selectedSession.id,
        ids: this.attendanceTrackingSelectedItems.map(item => item.id),
        status: status
      };
      this.attendanceTrackingRepository.changAttendanceStatus(request).subscribe(() => {
        this.showNotification();
        this.attendanceTrackingSelectedItems = [];
      });

      this.attendanceStatusValue = status;
      this.setParamsReportDynamic();
    }
  }

  public setParamsReportDynamic(): void {
    if (this.selectedSession) {
      const ids = this.attendanceTrackingSelectedItems.map(item => item.id);
      this.paramsReportDynamic = OpalReportDynamicComponent.buildAttendanceSummaryByPDOpportunity(
        ids,
        this.course.id,
        this.classRun.id,
        this.selectedSession.id,
        this.attendanceStatusValue
      );
    }
  }

  public loadedDataGrid(gridData: GridDataResult): void {
    this.gridData = gridData;
    this.onShowExportButtons();
  }

  public onDisable(): boolean {
    if (
      !this.attendanceTrackingSelectedItems ||
      !AttendanceTracking.hasSetPresentAbsentPermission(this.currentUser) ||
      (this.attendanceTrackingSelectedItems && this.attendanceTrackingSelectedItems.length === 0)
    ) {
      return true;
    } else {
      return false;
    }
  }

  public onShowExportButtons(): void {
    this.onShowExportBtn = this.gridData.total > 0;
    this.setParamsReportDynamic();
  }

  public viewAttendanceTracking(event: AttendanceTrackingViewModel): void {
    this.viewAttendanceTrackingEvent.emit(event);
  }

  public openQRDialog(): void {
    if (this.selectedSession != null) {
      this.dialogService.openDialogRef(QRCodeDialogComponent, {
        session: this.selectedSession,
        classRun: this.classRun
      });
    }
  }

  public updateSelectedSession(session: Session): void {
    this.selectedSession = session;
    this.setParamsReportDynamic();
  }

  public canShowGetSessionCodeBtn(): boolean {
    return this.selectedSession != null && Session.hasViewSessionCodePermission(this.currentUser);
  }

  public resetFilter(): void {
    this.filter = {
      ...this.filter,
      search: this.searchText,
      filter: this.filterData ? this.filterData.convert() : null
    };
  }

  private createFetchSessionsFn(): (searchText: string, skipCount: number, maxResultCount: number) => Observable<Session[]> {
    return (searchText: string, skipCount: number, maxResultCount: number) =>
      this.sessionRepository
        .loadSessionsByClassRunId(this.classRun.id, SearchSessionType.Owner, 0, Constant.MAX_ITEMS_PER_REQUEST, false)
        .pipe(map(_ => _.items));
  }
}
