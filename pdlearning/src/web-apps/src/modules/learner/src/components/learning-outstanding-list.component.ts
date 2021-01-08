import * as moment from 'moment';

import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import {
  IGetMyOutstandingTaskRequest,
  MyOutstandingTaskApiService,
  OutstandingTask,
  OutstandingTaskType,
  UserInfoModel
} from '@opal20/domain-api';

import { LEARNER_PERMISSIONS } from '@opal20/domain-components';

const pageSize = 10;
@Component({
  selector: 'learning-outstanding-list',
  templateUrl: './learning-outstanding-list.component.html'
})
export class LearningOutstandingListComponent extends BaseComponent implements OnInit {
  @Input()
  public outstandingTaskItems: OutstandingTask[] = [];
  @Input()
  public outstandingTaskItemsTotalCount: number = 0;
  @Input()
  public isShowMore: boolean = false;
  @Input()
  public numberOfHeightTimes: number;
  @Output()
  public outStandingTaskClick: EventEmitter<OutstandingTask> = new EventEmitter<OutstandingTask>();
  @Output()
  public showMoreOutstandingTaskClick: EventEmitter<void> = new EventEmitter<void>();

  public pageNumber: number = 1;

  constructor(protected moduleFacadeService: ModuleFacadeService, private myOutstandingTaskApiService: MyOutstandingTaskApiService) {
    super(moduleFacadeService);
  }

  public canShowProgress(outstandingTask: OutstandingTask): boolean {
    return outstandingTask.isCourseTask || outstandingTask.isMicrolearningTask;
  }

  public canShowFileIcon(outstandingTask: OutstandingTask): boolean {
    return outstandingTask.isAssignmentTask || outstandingTask.isDigitalContentTask || outstandingTask.isStandaloneFormTask;
  }

  public getFileIcon(outstandingTask: OutstandingTask): string {
    if (outstandingTask.isAssignmentTask) {
      return 'assets/images/icons/assignments.svg';
    }

    if (outstandingTask.isStandaloneFormTask) {
      return 'assets/images/icons/' + outstandingTask.formType.toLocaleLowerCase() + '.svg';
    }

    return !outstandingTask.fileExtension
      ? 'assets/images/icons/sm/docx.svg'
      : 'assets/images/icons/sm/' + outstandingTask.fileExtension.toLocaleLowerCase() + '.svg';
  }

  public canStartTask(outstandingTask: OutstandingTask): boolean {
    if (outstandingTask.isDigitalContentTask || outstandingTask.isMicrolearningTask) {
      return true;
    }

    if (outstandingTask.isStandaloneFormTask) {
      if (outstandingTask.dueDate) {
        return moment(outstandingTask.dueDate).isSameOrAfter(moment(), 'day');
      }
      return true;
    }

    return moment(outstandingTask.startDate).isSameOrBefore(moment(), 'day');
  }

  public get isEmpty(): boolean {
    return this.outstandingTaskItemsTotalCount != null && this.outstandingTaskItemsTotalCount === 0;
  }

  public getStartDate(outstandingTask: OutstandingTask): string {
    return !outstandingTask.startDate ? this.translate('N/A') : moment(outstandingTask.startDate).format('DD/MM/YYYY');
  }

  public getDueDate(outstandingTask: OutstandingTask): string {
    return !outstandingTask.dueDate ? this.translate('N/A') : moment(outstandingTask.dueDate).format('DD/MM/YYYY');
  }

  public onShowMoreClicked(): void {
    this.showMoreOutstandingTaskClick.emit();
  }

  public onOutstandingTaskClicked(event: OutstandingTask): void {
    this.outStandingTaskClick.emit(event);
  }

  public onLoadMoreClicked(): void {
    this.increasePageNumber();
    this.loadOutstandingTaskItems(pageSize, pageSize * (this.pageNumber - 1));
  }

  public get canLoadMore(): boolean {
    return this.pageNumber * pageSize <= this.outstandingTaskItemsTotalCount;
  }
  public buttonTooltip(outstandingTask: OutstandingTask): string {
    if (this.canStartTask(outstandingTask)) {
      return '';
    }

    const title = "Unable to ##status## because it's not yet reached the start date";
    const status = outstandingTask.isNotStarted ? 'start' : 'continue';

    return this.translate(title.replace('##status##', status), { status: status });
  }

  public showStartByPermission(task: OutstandingTask): boolean {
    switch (task.type) {
      case OutstandingTaskType.Course:
      case OutstandingTaskType.DigitalContent:
      case OutstandingTaskType.Microlearning:
        return this.hasPermission(LEARNER_PERMISSIONS.Action_StartLearning);
      case OutstandingTaskType.Assignment:
        return this.hasPermission(LEARNER_PERMISSIONS.Action_Checkin_DoAssignment_DownloadContent_DoPostCourse);
      case OutstandingTaskType.StandaloneForm:
      default:
        return true;
    }
  }

  protected currentUserPermissionDic(): IPermissionDictionary {
    return UserInfoModel.getMyUserInfo().permissionDic;
  }

  private loadOutstandingTaskItems(maxResultCount: number, skipCount: number): void {
    this.myOutstandingTaskApiService
      .getOutstandingTasks(<IGetMyOutstandingTaskRequest>{
        maxResultCount: maxResultCount,
        skipCount: skipCount
      })
      .then(result => {
        this.outstandingTaskItems = this.outstandingTaskItems.concat(result.items);
        this.outstandingTaskItemsTotalCount = result.totalCount;
      });
  }

  private increasePageNumber(): void {
    this.pageNumber = this.pageNumber + 1;
  }
}
