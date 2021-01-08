import { BasePageComponent, IGridFilter, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { ButtonAction, DialogAction, OpalDialogService } from '@opal20/common-components';
import {
  CAMRoutePaths,
  CAMTabConfiguration,
  ContextMenuAction,
  ContextMenuEmit,
  ListSessionGridDisplayColumns,
  NavigationPageService,
  RouterPageInput,
  SessionDetailMode,
  SessionViewModel
} from '@opal20/domain-components';
import { ClassRun, Course, SearchClassRunType, SearchSessionType, Session, SessionRepository, UserInfoModel } from '@opal20/domain-api';
import { Component, HostBinding, Input } from '@angular/core';

import { ClassRunDetailPageInput } from '../models/classrun-detail-page-input.model';
import { GridDataResult } from '@progress/kendo-angular-grid';

@Component({
  selector: 'session-management-page',
  templateUrl: './session-management-page.component.html'
})
export class SessionManagementPageComponent extends BasePageComponent {
  @Input() public stickyDependElement: HTMLElement;
  public searchText: string = '';
  public filterData: unknown = null;
  public gridData: GridDataResult;
  public SearchSessionType: typeof SearchSessionType = SearchSessionType;
  public filter: IGridFilter = {
    search: '',
    filter: null
  };
  public get classRun(): ClassRun | undefined {
    return this._classRun;
  }

  public loadingData: boolean = false;

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
  @Input() public classRunSearchType: SearchClassRunType;
  @Input() public classRunDetailPageInput: RouterPageInput<ClassRunDetailPageInput, CAMTabConfiguration, unknown> | undefined;

  public actionBtnGroup: ButtonAction<unknown>[] = [
    {
      id: 'publish',
      text: this.translateCommon('Publish')
    },
    {
      id: 'unpublish',
      text: this.translateCommon('Unpublish')
    },
    {
      id: 'approve',
      text: this.translateCommon('Approve')
    },
    {
      id: 'reject',
      text: this.translateCommon('Reject')
    }
  ];

  public allClassRunDisplayColumns: ListSessionGridDisplayColumns[] = [
    ListSessionGridDisplayColumns.title,
    ListSessionGridDisplayColumns.createdDate,
    ListSessionGridDisplayColumns.sessionDate,
    ListSessionGridDisplayColumns.learningTime,
    ListSessionGridDisplayColumns.sessionVenue,
    ListSessionGridDisplayColumns.learningMethod,
    ListSessionGridDisplayColumns.webinar,
    ListSessionGridDisplayColumns.actions
  ];

  private _classRun: ClassRun;
  private _course: Course;
  private currentUser: UserInfoModel = UserInfoModel.getMyUserInfo();

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private navigationPageService: NavigationPageService,
    private opalDialogService: OpalDialogService,
    private sessionRepository: SessionRepository
  ) {
    super(moduleFacadeService);
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  public onViewSession(data: SessionViewModel): void {
    if (this.canViewSession()) {
      this.navigationPageService.navigateTo(
        CAMRoutePaths.SessionDetailPage,
        {
          activeTab: CAMTabConfiguration.SessionInfoTab,
          data: {
            mode: SessionDetailMode.View,
            id: data.id,
            courseId: this.course.id,
            classRunId: this.classRun.id,
            classRunSearchType: this.classRunSearchType,
            isRescheduleMode: this.isRescheduleMode
          }
        },
        this.classRunDetailPageInput
      );
    }
  }

  public onCreateSession(): void {
    if (this.gridData && this.course.numOfSessionPerClass <= this.gridData.total) {
      this.opalDialogService
        .openConfirmDialog({
          confirmTitle: 'Warning',
          confirmMsg: 'You are about to create a session that exceeds the planned number of sessions per class. Do you want to proceed?',
          yesBtnText: 'Ok',
          noBtnText: 'Cancel'
        })
        .subscribe(action => {
          if (action === DialogAction.OK) {
            this.navigationCreateSession();
          }
        });
    } else {
      this.navigationCreateSession();
    }
  }

  public loadedDataGrid(gridData: GridDataResult): void {
    this.gridData = gridData;
  }

  public canCreateSession(): boolean {
    return (
      this.classRun && this.classRun.canCreateSession(this.course) && ClassRun.hasCreateSessionPermission(this.currentUser, this.course)
    );
  }

  public selectedContextMenu(contextMenuEmit: ContextMenuEmit<Session>): void {
    switch (contextMenuEmit.event.item.id) {
      case ContextMenuAction.Delete:
        this.opalDialogService
          .openConfirmDialog({
            confirmTitle: 'Warning',
            confirmMsg: 'You’re about to delete this session. Do you want to proceed?',
            yesBtnText: 'Yes',
            noBtnText: 'No'
          })
          .subscribe(action => {
            if (action === DialogAction.OK) {
              this.sessionRepository.deleteSession(contextMenuEmit.dataItem.id).then(_ => {
                this.showNotification(this.translate('Session deleted successfully'));
              });
            }
          });
        break;
      default:
        break;
    }
  }

  public resetFilter(): void {
    this.filter = {
      ...this.filter,
      search: this.searchText
    };
  }

  private canViewSession(): boolean {
    return this.classRun && this.classRun.canViewSession() && ClassRun.hasViewSessionPermission(this.currentUser, this.course);
  }

  private navigationCreateSession(): void {
    this.navigationPageService.navigateTo(
      CAMRoutePaths.SessionDetailPage,
      {
        activeTab: CAMTabConfiguration.SessionInfoTab,
        data: {
          mode: SessionDetailMode.NewSesion,
          courseId: this.course.id,
          classRunId: this.classRun.id
        }
      },
      this.classRunDetailPageInput
    );
  }
}
