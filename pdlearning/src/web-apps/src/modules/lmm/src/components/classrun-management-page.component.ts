import { BasePageComponent, IGridFilter, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { ButtonAction, DialogAction, OpalDialogService } from '@opal20/common-components';
import {
  ClassRunDetailMode,
  ClassRunViewModel,
  CommentDialogComponent,
  CourseDetailMode,
  IDialogActionEvent,
  LMMRoutePaths,
  LMMTabConfiguration,
  ListClassrunGridDisplayColumns,
  NavigationPageService,
  RouterPageInput
} from '@opal20/domain-components';
import { Component, HostBinding, Input } from '@angular/core';
import { ContentStatus, Course, LearningContentRepository, SearchClassRunType } from '@opal20/domain-api';
import { Observable, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { CourseDetailPageInput } from '../models/course-detail-page-input.model';
import { SelectEvent } from '@progress/kendo-angular-layout';

@Component({
  selector: 'classrun-management-page',
  templateUrl: './classrun-management-page.component.html'
})
export class ClassRunManagementPageComponent extends BasePageComponent {
  @Input() public stickyDependElement: HTMLElement;

  public searchText: string = '';
  public filterData: unknown = null;
  public filter: IGridFilter = {
    search: '',
    filter: null
  };

  public get course(): Course | undefined {
    return this._course;
  }

  @Input()
  public set course(v: Course | undefined) {
    if (Utils.isDifferent(this._course, v)) {
      this._course = v;
    }
  }

  public get courseDetailPageInput(): RouterPageInput<CourseDetailPageInput, LMMTabConfiguration, LMMTabConfiguration> | undefined {
    return this._courseDetailPageInput;
  }

  @Input() public set courseDetailPageInput(
    courseDetailPageInput: RouterPageInput<CourseDetailPageInput, LMMTabConfiguration, LMMTabConfiguration> | undefined
  ) {
    this._courseDetailPageInput = courseDetailPageInput;
  }
  public allClassRunSelectedItems: ClassRunViewModel[] = [];

  public get hasActionBtnGroup(): boolean {
    return this.actionBtnGroup.findIndex(x => x.actionFn != null && (x.hiddenFn == null || !x.hiddenFn())) > -1;
  }

  public actionBtnGroup: ButtonAction<ClassRunViewModel>[] = [
    {
      id: 'approve',
      text: this.translateCommon('Approve'),
      conditionFn: dataItem => this.canApprovalContentClassRun(dataItem),
      actionFn: dataItems => this.handleMassAction(ClassRunContentMassAction.Approve, dataItems),
      hiddenFn: () =>
        this.selectedTab !== LMMTabConfiguration.AllClassRunsTab || this.courseDetailPageInput.data.mode !== CourseDetailMode.ForApprover
    },
    {
      id: 'reject',
      text: this.translateCommon('Reject'),
      conditionFn: dataItem => this.canApprovalContentClassRun(dataItem),
      actionFn: dataItems => this.handleMassAction(ClassRunContentMassAction.Reject, dataItems),
      hiddenFn: () =>
        this.selectedTab !== LMMTabConfiguration.AllClassRunsTab || this.courseDetailPageInput.data.mode !== CourseDetailMode.ForApprover
    },
    {
      id: 'publish',
      text: this.translateCommon('Publish'),
      conditionFn: dataItem => dataItem.canPublishContent(this.course),
      actionFn: dataItems => this.handleMassAction(ClassRunContentMassAction.Publish, dataItems),
      hiddenFn: () => this.selectedTab !== LMMTabConfiguration.AllClassRunsTab
    },
    {
      id: 'unpublish',
      text: this.translateCommon('Unpublish'),
      conditionFn: dataItem => dataItem.canUnpublishContent(this.course),
      actionFn: dataItems => this.handleMassAction(ClassRunContentMassAction.Unpublish, dataItems),
      hiddenFn: () => this.selectedTab !== LMMTabConfiguration.AllClassRunsTab
    }
  ];

  public allClassRunsDisplayColumns: ListClassrunGridDisplayColumns[] = [
    ListClassrunGridDisplayColumns.selected,
    ListClassrunGridDisplayColumns.title,
    ListClassrunGridDisplayColumns.learningPeriod,
    ListClassrunGridDisplayColumns.applicationPeriod,
    ListClassrunGridDisplayColumns.owner,
    ListClassrunGridDisplayColumns.status,
    ListClassrunGridDisplayColumns.actions
  ];

  public SearchClassRunType: typeof SearchClassRunType = SearchClassRunType;
  public LMMTabConfiguration: typeof LMMTabConfiguration = LMMTabConfiguration;
  private _course: Course;
  public get selectedTab(): LMMTabConfiguration {
    return this.courseDetailPageInput.subActiveTab != null ? this.courseDetailPageInput.subActiveTab : LMMTabConfiguration.ClassRunInfoTab;
  }

  private _courseDetailPageInput: RouterPageInput<CourseDetailPageInput, LMMTabConfiguration, LMMTabConfiguration> | undefined;
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private navigationPageService: NavigationPageService,
    private opalDialogService: OpalDialogService,
    private learningContentRepository: LearningContentRepository
  ) {
    super(moduleFacadeService);
  }

  public get getSelectedItemsForMassAction(): ClassRunViewModel[] {
    if (this.selectedTab === LMMTabConfiguration.AllClassRunsTab) {
      return this.allClassRunSelectedItems;
    }
    return [];
  }

  public handleMassAction(massAction: ClassRunContentMassAction, dataItems: ClassRunViewModel[]): Promise<boolean> {
    let massActionPromise: Promise<boolean>;
    switch (massAction) {
      case ClassRunContentMassAction.Approve:
        massActionPromise = this.showApprovalDialog(
          {
            title: `${this.translate('Approve Content Class')}: ${this.course.courseName}`,
            requiredCommentField: false
          },
          dataItems,
          ContentStatus.Approved,
          false
        );
        break;
      case ClassRunContentMassAction.Reject:
        massActionPromise = this.showApprovalDialog(
          {
            title: `${this.translate('Reject Content Class')}: ${this.course.courseName}`,
            requiredCommentField: false
          },
          dataItems,
          ContentStatus.Rejected,
          false
        );
        break;
      case ClassRunContentMassAction.Publish:
        massActionPromise = this.changeClassRunContentStatus(dataItems, ContentStatus.Published, '', false).toPromise();
        break;
      case ClassRunContentMassAction.Unpublish:
        massActionPromise = this.changeClassRunContentStatus(dataItems, ContentStatus.Unpublished, '', false).toPromise();
        break;
    }
    return massActionPromise.then(_ => {
      this.resetSelectedItems();
      return _;
    });
  }

  public resetSelectedItems(): void {
    this.allClassRunSelectedItems = [];
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  public onTabSelected(event: SelectEvent): void {
    this.courseDetailPageInput.subActiveTab = classRunManagementPageTabIndexMap[event.index];
    this.navigationPageService.navigateByRouter(this.courseDetailPageInput);
  }

  public onViewClassRun(data: ClassRunViewModel): void {
    this.navigationPageService.navigateTo(
      LMMRoutePaths.ClassRunDetailPage,
      {
        activeTab: LMMTabConfiguration.ClassRunInfoTab,
        data: {
          mode:
            this.courseDetailPageInput.data.mode !== CourseDetailMode.ForApprover
              ? ClassRunDetailMode.View
              : ClassRunDetailMode.ForApprover,
          id: data.id,
          courseId: this.course.id
        }
      },
      this.courseDetailPageInput
    );
  }

  public resetFilter(): void {
    this.filter = {
      ...this.filter,
      search: this.searchText
    };
  }

  private canApprovalContentClassRun(classRun: ClassRunViewModel): boolean {
    return (
      this.courseDetailPageInput.data.mode === CourseDetailMode.ForApprover && classRun.contentStatus === ContentStatus.PendingApproval
    );
  }

  private changeClassRunContentStatus(
    classRuns: ClassRunViewModel[],
    contentStatus: ContentStatus,
    comment: string = '',
    showNotification: boolean = true
  ): Observable<boolean> {
    return this.learningContentRepository
      .changeClassRunContentStatus({
        ids: classRuns.map(p => p.id),
        contentStatus: contentStatus,
        comment: comment
      })
      .pipe(
        this.untilDestroy(),
        map(_ => {
          if (showNotification) {
            this.showNotification();
          }
          return true;
        })
      );
  }

  private showApprovalDialog(
    input: Partial<CommentDialogComponent>,
    classRuns: ClassRunViewModel[],
    contentStatus: ContentStatus,
    showNotification: boolean = true
  ): Promise<boolean> {
    return this.opalDialogService
      .openDialogRef(CommentDialogComponent, input)
      .result.pipe(
        switchMap((data: IDialogActionEvent) => {
          if (data.action === DialogAction.OK) {
            return this.changeClassRunContentStatus(classRuns, contentStatus, data.comment, showNotification);
          }
          return of(false);
        })
      )
      .toPromise();
  }
}

export enum ClassRunContentMassAction {
  Publish = 'publish',
  Unpublish = 'unpublish',
  Approve = 'approve',
  Reject = 'reject'
}

export const classRunManagementPageTabIndexMap = {
  0: LMMTabConfiguration.AllClassRunsTab
};
