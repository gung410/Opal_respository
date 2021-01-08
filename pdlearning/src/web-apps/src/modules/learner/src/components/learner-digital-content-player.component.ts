import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import {
  IMyDigitalContentRequest,
  MyDigitalContentApiService,
  MyDigitalContentStatus,
  UserInfoModel,
  UserReviewItemType
} from '@opal20/domain-api';

import { ILearningFeedbacksConfiguration } from '../models/learning-feedbacks.model';
import { LEARNER_PERMISSIONS } from '@opal20/domain-components';
import { LearningFeedbacksConfig } from '../constants/learning-feedbacks-configs';
import { LearningTrackingEventPayload } from '../user-activities-tracking/user-tracking.models';
import { MyDigitalContentDetail } from '../models/my-digital-content-detail.model';
import { TrackingSourceService } from '../user-activities-tracking/tracking-souce.service';

@Component({
  selector: 'learner-digital-content-player',
  templateUrl: './learner-digital-content-player.component.html'
})
export class LearnerDigitalContentPlayerComponent extends BaseComponent {
  @Input()
  public digitalContent: MyDigitalContentDetail;
  @Output()
  public backClick: EventEmitter<void> = new EventEmitter<void>();

  public myDigitalContentId: string;
  public digitalContentOriginalId: string;
  public isCompleted: boolean = false;
  public title: string;

  public showFinishScreen: boolean = false;
  public isScorm: boolean = false;
  public isScormPassed: boolean = false;

  public reviewType: UserReviewItemType = UserReviewItemType.DigitalContent;
  public feedbacksConfig: ILearningFeedbacksConfiguration = LearningFeedbacksConfig.digitalContent;

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private trackingSourceSrv: TrackingSourceService,
    private myDigitalContentApiService: MyDigitalContentApiService
  ) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.digitalContentOriginalId = this.digitalContent.digitalContent.originalObjectId;
    this.myDigitalContentId = this.digitalContent.myDigitalContent.id;
    this.isCompleted = this.digitalContent.myDigitalContent.status === MyDigitalContentStatus.Completed;
    this.title = this.digitalContent.digitalContent.title;
    this.isScorm = this.digitalContent.digitalContent.fileExtension === 'scorm';
  }

  public onDownloadDigitalContent(digitalContentId: string): void {
    this.trackingSourceSrv.eventTrack.next({
      eventName: 'LearningTracking',
      payload: <LearningTrackingEventPayload>{
        itemId: digitalContentId,
        trackingType: 'digitalContent',
        trackingAction: 'downloadContent'
      }
    });
  }

  public finishDigitalContent(): void {
    if (!this.digitalContentOriginalId || !this.canFinish()) {
      return;
    }
    if (!this.isCompleted) {
      const updateDigitalRequest: IMyDigitalContentRequest = {
        digitalContentId: this.digitalContentOriginalId,
        status: MyDigitalContentStatus.Completed
      };
      this.myDigitalContentApiService.updateMyDigitalContent(updateDigitalRequest).then(() => {
        this.digitalContent.myDigitalContent.status = MyDigitalContentStatus.Completed;
        this.isCompleted = true;
      });
    }
    this.showFinishScreen = true;
  }

  public onScormFinishCallback(): void {
    this.isScormPassed = true;
  }

  public onSubmitFeedbacks(): void {
    this.backClick.emit();
  }

  public onBackClick(): void {
    this.backClick.emit();
  }

  public previous(): void {
    if (this.showFinishScreen) {
      this.showFinishScreen = false;
      return;
    }
  }

  public canFinish(): boolean {
    return this.isCompleted || !this.isScorm || this.isScormPassed;
  }

  public get hasContentDownloadPermission(): boolean {
    return this.hasPermission(LEARNER_PERMISSIONS.Action_Checkin_DoAssignment_DownloadContent_DoPostCourse);
  }

  protected currentUserPermissionDic(): IPermissionDictionary {
    return UserInfoModel.getMyUserInfo().permissionDic;
  }
}
