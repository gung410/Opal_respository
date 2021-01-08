import { BasePageComponent, IGridFilter, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { ClassRun, Course, SearchClassRunType, SearchSessionType, UserInfoModel } from '@opal20/domain-api';
import { Component, HostBinding, Input } from '@angular/core';
import {
  LMMRoutePaths,
  LMMTabConfiguration,
  ListSessionGridDisplayColumns,
  NavigationPageService,
  RouterPageInput,
  SessionDetailMode,
  SessionViewModel
} from '@opal20/domain-components';

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

  @Input() public classRunSearchType: SearchClassRunType;
  @Input() public classRunDetailPageInput: RouterPageInput<ClassRunDetailPageInput, LMMTabConfiguration, unknown> | undefined;

  public allClassRunDisplayColumns: ListSessionGridDisplayColumns[] = [
    ListSessionGridDisplayColumns.title,
    ListSessionGridDisplayColumns.createdDate,
    ListSessionGridDisplayColumns.sessionDate,
    ListSessionGridDisplayColumns.learningTime,
    ListSessionGridDisplayColumns.sessionVenue,
    ListSessionGridDisplayColumns.learningMethod,
    ListSessionGridDisplayColumns.webinar
  ];

  private _classRun: ClassRun;
  private _course: Course;
  private currentUser: UserInfoModel = UserInfoModel.getMyUserInfo();

  constructor(public moduleFacadeService: ModuleFacadeService, private navigationPageService: NavigationPageService) {
    super(moduleFacadeService);
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  public onViewSession(data: SessionViewModel): void {
    if (this.canViewSession()) {
      this.navigationPageService.navigateTo(
        LMMRoutePaths.SessionDetailPage,
        {
          activeTab: LMMTabConfiguration.SessionInfoTab,
          data: {
            mode: SessionDetailMode.View,
            id: data.id,
            courseId: this.course.id,
            classRunId: this.classRun.id,
            classRunSearchType: this.classRunSearchType
          }
        },
        this.classRunDetailPageInput
      );
    }
  }

  public loadedDataGrid(gridData: GridDataResult): void {
    this.gridData = gridData;
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
}
