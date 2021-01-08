import { BaseComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output, TemplateRef, ViewChild } from '@angular/core';
import {
  CourseContentItemModel,
  FormAnswerModel,
  IUpdateCourseStatus,
  LearningContentApiService,
  LectureType,
  MyCourseApiService,
  MyCourseStatus,
  MyLectureStatus,
  UserInfoModel,
  UserReviewItemType,
  UserReviewModel
} from '@opal20/domain-api';
import {
  EventTrackParam,
  LearningTrackingEventPayload,
  LectureEventPayload,
  QuizPlayingEventPlayload
} from '../user-activities-tracking/user-tracking.models';
import { LEARNER_PERMISSIONS, PlayerHelpers, QuizPlayerIntegrationsService, ScormPlayerMode } from '@opal20/domain-components';

import { CourseModel } from '../models/course.model';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { ILearningFeedbacksConfiguration } from '../models/learning-feedbacks.model';
import { LearnerLectureModel } from '../models/lecture.model';
import { LearningFeedbacksConfig } from '../constants/learning-feedbacks-configs';
import { TrackingSourceService } from '../user-activities-tracking/tracking-souce.service';
import { v4 as uuidv4 } from 'uuid';

@Component({
  selector: 'lecture-player',
  templateUrl: './lecture-player.component.html',
  host: {
    '(contextmenu)': 'preventRightClick($event)'
  }
})
export class LecturePlayerComponent extends BaseComponent {
  @Input()
  public myCourseId: string;
  @Input()
  public courseId: string;
  @Input()
  public isCompleted: boolean;
  @Input()
  public course: CourseModel;
  @Input()
  public courseTitle: string;
  @Input()
  public isMicrolearning: boolean;
  @Input()
  public currentLectureId: string | undefined;
  @Input()
  public tableOfContents: CourseContentItemModel[];
  @Input()
  public hasContentChanged: boolean;
  @Input()
  public reviewOnly: boolean = false;
  @Input()
  public displayMode: ScormPlayerMode = 'learn';

  @Output()
  public nextBtnClick: EventEmitter<void> = new EventEmitter<void>();
  @Output()
  public backClick: EventEmitter<void> = new EventEmitter<void>();
  @Output()
  public completeLecture: EventEmitter<LearnerLectureModel> = new EventEmitter<LearnerLectureModel>();

  public backConfirmationDialog: DialogRef;

  public hasLoadLecture: boolean = false;
  public lectureType: typeof LectureType = LectureType;
  public currentStep: number | undefined;
  public completedSteps: number = 0;

  public showFinishScreen: boolean = false;

  public showTableOfContents: boolean = false;
  public feedbacksConfig: ILearningFeedbacksConfiguration;
  public reviewType: UserReviewItemType = UserReviewItemType.Course;
  public isLectureNextButtonClicked: boolean = false;

  private _currentLecture: LearnerLectureModel | undefined;
  public get currentLecture(): LearnerLectureModel | undefined {
    return this._currentLecture;
  }
  public set currentLecture(newValue: LearnerLectureModel | undefined) {
    const oldValue = this._currentLecture;

    if (
      (oldValue !== newValue && (newValue === undefined || oldValue === undefined)) || // check new != old if one of them is undefined
      (oldValue && newValue && oldValue.lectureId !== newValue.lectureId) // check new != old by lectureId
    ) {
      this._currentLecture = newValue;

      if (oldValue !== undefined) {
        this.stopTrackingLecture(oldValue.lectureId);
      }

      if (newValue !== undefined) {
        this.startTrackingLecture(newValue.lectureId);
      }
    }
  }

  private _lectures: LearnerLectureModel[] = [];
  @Input()
  public set lectures(value: LearnerLectureModel[]) {
    if (JSON.stringify(value) === JSON.stringify(this.lectures)) {
      return;
    }

    this._lectures = Utils.cloneDeep(value);
    this.onLecturesUpdate();
    if (!this.hasLoadLecture) {
      this.initiateLecture();
    }
  }
  public get lectures(): LearnerLectureModel[] {
    return this._lectures;
  }

  @ViewChild('backConfirmation', { static: false })
  private backConfirmationTemplate: TemplateRef<unknown>;

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private quizPlayerService: QuizPlayerIntegrationsService,
    private myCourseApiService: MyCourseApiService,
    private trackingSource: TrackingSourceService,
    private learningContentApiService: LearningContentApiService
  ) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.feedbacksConfig = this.isMicrolearning ? LearningFeedbacksConfig.microlearning : LearningFeedbacksConfig.faceToFace;
  }

  public canShowNextButton(lecture: LearnerLectureModel): boolean {
    if (lecture.lectureDetail.type === LectureType.InlineContent) {
      return true;
    }

    if (lecture.lectureDetail.type === LectureType.DigitalContent && lecture.lectureDetail.mimeType !== 'application/scorm') {
      return true;
    }

    return false;
  }

  public previousLecture(): void {
    if (this.currentStep === 1) {
      return;
    }
    if (this.showFinishScreen) {
      this.showFinishScreen = false;
      return;
    }
    this.currentLecture = this.lectures[this.currentStep - 2];
    this.updateCurrentStep();
    this.initiateLecture();
  }

  public finishDigitalContent(): void {
    this.nextOrFinish(true);
  }

  public nextOrFinish(isFinished: boolean = false): void {
    if (!isFinished && this.disableNext) {
      return;
    }
    this.isLectureNextButtonClicked = true;

    if (this.currentLecture.myLectureInfo.status === MyLectureStatus.Completed) {
      if (this.currentStep < this.lectures.length) {
        this.nextLecture();
      } else {
        if (this.showFinishScreen) {
          this.backClick.emit();
          this.stopTrackingLecture(this.currentLecture.lectureId);
        }
        this.finishCourse();
      }
      return;
    }

    if (this.currentLecture.myLectureInfo.status === MyLectureStatus.NotStarted) {
      this.myCourseApiService.completeLecture(this.currentLecture.myLectureInfo.id).then(() => {
        this.currentLecture.myLectureInfo.status = MyLectureStatus.Completed;
        this.completeLecture.emit(this.currentLecture);
        setTimeout(() => {
          if (this.currentStep < this.lectures.length) {
            this.nextLecture();
          } else {
            if (this.showFinishScreen) {
              this.backClick.emit();
              this.stopTrackingLecture(this.currentLecture.lectureId);
            }
            this.finishCourse();
          }
        }, 50);
      });
    }
  }

  public get disableNext(): boolean {
    return (
      this.currentLecture &&
      this.currentLecture.lectureDetail.type === LectureType.Quiz &&
      this.currentLecture.myLectureInfo.status !== MyLectureStatus.Completed
    );
  }

  public nextLecture(): void {
    this.currentLecture = this.lectures[this.currentStep];
    this.updateCurrentStep();
    this.initiateLecture();
  }

  public finishCourse(): void {
    if (this.courseId === undefined) {
      return;
    }
    if (this.isMicrolearning && !this.isCompleted) {
      const myCourseRequest: IUpdateCourseStatus = {
        courseId: this.courseId,
        status: MyCourseStatus.Completed
      };
      this.myCourseApiService.updateStatusCourse(myCourseRequest).then(() => {
        this.isCompleted = true;
      });
    }
    this.stopTrackingLecture(this.currentLecture.lectureId);
    this.showFinishScreen = true;
  }

  public onSubmitFeedbacks(review: UserReviewModel): void {
    this.onBackToCoursePageClick();
  }

  public showBackConfirmationDialog(): void {
    this.backConfirmationDialog = this.moduleFacadeService.dialogService.open({
      content: this.backConfirmationTemplate
    });
    this.backConfirmationDialog.dialog.location.nativeElement.classList.add('confirmation-dialog');
  }

  public onBackClick(): void {
    this.backConfirmationDialog.close();
    this.backClick.emit();
    this.stopTrackingLecture(this.currentLecture.lectureId);
  }

  public onLectureClick(lectureId: string): void {
    this.showFinishScreen = false;
    const lecture = this.lectures.find(p => p.lectureId === lectureId);
    if (
      lecture === undefined ||
      (lecture.myLectureInfo.status !== MyLectureStatus.Completed && lecture.lectureId !== this.nextLearnerableLectureId)
    ) {
      return;
    }

    this.currentLecture = this.lectures.find(p => p.lectureId === lectureId);
    this.updateCurrentStep();
    this.initiateLecture();
    this.toggleTableOfContents();
  }

  public onDownloadDigitalContent(digitalContentId: string): void {
    this.trackingSource.eventTrack.next({
      eventName: 'LearningTracking',
      payload: <LearningTrackingEventPayload>{
        itemId: digitalContentId,
        trackingType: 'digitalContent',
        trackingAction: 'downloadContent'
      }
    });
  }

  public onBackToCoursePageClick(): void {
    this.backClick.emit();
  }

  public get completedLectureIds(): string[] {
    if (this.lectures.length === 0 && this.lectures.some(p => p.myLectureInfo === undefined)) {
      return [];
    }
    return this.lectures.filter(p => p.myLectureInfo.status === MyLectureStatus.Completed).map(p => p.lectureId);
  }

  public toggleTableOfContents(): void {
    this.showTableOfContents = !this.showTableOfContents;
  }

  public get nextLearnerableLectureId(): string | undefined {
    if (this.lectures === undefined) {
      return undefined;
    }
    const firstNotStartedLecture = this.lectures.find(p => p.myLectureInfo.status === MyLectureStatus.NotStarted);
    if (firstNotStartedLecture === undefined) {
      return undefined;
    }
    return firstNotStartedLecture.lectureId;
  }

  public get hideNextOrFinish(): boolean {
    if (this.currentLecture === undefined) {
      return true;
    }

    return (
      this.showFinishScreen ||
      (this.currentLecture.myLectureInfo.status === MyLectureStatus.NotStarted &&
        (this.currentLecture.lectureDetail.type === LectureType.Quiz || this.currentLecture.lectureDetail.mimeType === 'application/scorm'))
    );
  }

  public preventRightClick(e: MouseEvent): void {
    PlayerHelpers.preventRightClick(e);
  }

  public get hasContentDownloadPermission(): boolean {
    return this.hasPermission(LEARNER_PERMISSIONS.Action_Checkin_DoAssignment_DownloadContent_DoPostCourse);
  }

  protected currentUserPermissionDic(): IPermissionDictionary {
    return UserInfoModel.getMyUserInfo().permissionDic;
  }

  private initiateLecture(): void {
    this.getCurrentLectureDetail().then(() => {
      if (this.currentLecture.lectureDetail.type === LectureType.Quiz) {
        this.loadQuizPlayer();
      }
    });
  }

  private loadQuizPlayer(): void {
    if (this.currentLecture === undefined || this.currentLecture.lectureDetail.type !== LectureType.Quiz) {
      return;
    }

    const playingSessionId = uuidv4();
    this.quizPlayerService.setup({
      onQuizFinished: (formAnswerModel: FormAnswerModel) => {
        this.nextOrFinish(true);
        this.trackingSource.eventTrack.next(<EventTrackParam>{
          eventName: 'StopQuiz',
          payload: <QuizPlayingEventPlayload>{
            playingSessionId: playingSessionId,
            formId: formAnswerModel.formId,
            formAnswerId: formAnswerModel.id
          }
        });
      },
      onQuizInitiated: () => {
        this.trackingSource.eventTrack.next(<EventTrackParam>{
          eventName: 'PlayQuiz',
          payload: <QuizPlayingEventPlayload>{ playingSessionId: playingSessionId, formId: this.currentLecture.lectureDetail.resourceId }
        });
      },
      onQuizExited: (formAnswerModel: FormAnswerModel) => {
        this.trackingSource.eventTrack.next(<EventTrackParam>{
          eventName: 'StopQuiz',
          payload: <QuizPlayingEventPlayload>{
            playingSessionId: playingSessionId,
            formId: formAnswerModel.formId,
            formAnswerId: formAnswerModel.id
          }
        });
      },
      onQuizSubmitted: (formAnswerModel: FormAnswerModel) => {
        if (this.currentLecture.myLectureInfo.status === MyLectureStatus.NotStarted) {
          this.myCourseApiService.completeLecture(this.currentLecture.myLectureInfo.id).then(() => {
            this.currentLecture.myLectureInfo.status = MyLectureStatus.Completed;
            this.completeLecture.emit(this.currentLecture);
          });
        }
        this.trackingSource.eventTrack.next(<EventTrackParam>{
          eventName: 'SubmittedQuizAnswer',
          payload: <QuizPlayingEventPlayload>{
            playingSessionId: playingSessionId,
            formId: formAnswerModel.formId,
            formAnswerId: formAnswerModel.id
          }
        });
      }
    });
    this.quizPlayerService.setFormId(this.currentLecture.lectureDetail.resourceId);
    this.quizPlayerService.setResourceId(this.courseId);
    // TODO: set this to null because there is a problem with BehaviorSubject, don't have time to invest why they used BehaviorSubject
    this.quizPlayerService.setClassRunId(this.course.myClassRun.classRunId);
    this.quizPlayerService.setMyCourseId(this.myCourseId);
    this.quizPlayerService.setAssignmentId(null);
    this.quizPlayerService.setPassingRateEnable(this.currentLecture.enablePassingRate);
    this.quizPlayerService.setReviewOnly(this.reviewOnly);
  }

  private onLecturesUpdate(): void {
    if (this.lectures === undefined) {
      return;
    }
    this.updateCurrentLecture();
    if (!this.hasLoadLecture) {
      this.calculateCurrentLecture();
    }
    this.calculateCompletedSteps();
  }

  private calculateCurrentLecture(): void {
    if (this.lectures === undefined || this.lectures.length === 0 || this.lectures.some(p => p.myLectureInfo === undefined)) {
      this.currentLecture = undefined;
      return;
    }

    if (this.currentLectureId !== undefined) {
      this.currentLecture = this.lectures.find(p => p.lectureId === this.currentLectureId);
      this.updateCurrentStep();
      return;
    }

    const firstNotStartedLecture = this.lectures.find(p => p.myLectureInfo.status === MyLectureStatus.NotStarted);
    if (firstNotStartedLecture === undefined && this.lectures.every(p => p.myLectureInfo.status === MyLectureStatus.Completed)) {
      this.currentLecture = this.lectures[this.lectures.length - 1];
      this.currentStep = this.lectures.length;
      return;
    }

    this.currentLecture = firstNotStartedLecture;
    this.updateCurrentStep();
  }

  private calculateCompletedSteps(): void {
    if (this.lectures === undefined || this.lectures.length === 0 || this.lectures.some(p => p.myLectureInfo === undefined)) {
      this.completedSteps = 0;
      return;
    }

    this.completedSteps = this.lectures.filter(p => p.myLectureInfo.status === MyLectureStatus.Completed).length;
  }

  private getCurrentLectureDetail(): Promise<void> {
    if (this.lectures === undefined || this.currentLecture === undefined) {
      return Promise.resolve();
    }
    return this.learningContentApiService.getLecture(this.currentLecture.lectureId).then(lectureDetail => {
      this.currentLecture.lectureDetail = lectureDetail;
      this.hasLoadLecture = true;
    });
  }

  private startTrackingLecture(lectureId: string): void {
    this.trackingSource.eventTrack.next({
      eventName: 'StartLecture',
      payload: <LectureEventPayload>{
        courseId: this.courseId,
        lectureid: lectureId
      }
    });
  }

  private stopTrackingLecture(lectureId: string): void {
    this.trackingSource.eventTrack.next({
      eventName: 'FinishLecture',
      payload: <LectureEventPayload>{
        courseId: this.courseId,
        lectureid: lectureId
      }
    });
  }

  private updateCurrentStep(): void {
    if (this.lectures === undefined || this.currentLecture === undefined) {
      return;
    }
    this.currentStep = this.lectures.findIndex(p => p.lectureId === this.currentLecture.lectureId) + 1;
  }

  private updateCurrentLecture(): void {
    if (this.currentLecture === undefined) {
      return;
    }

    const lectureDetail = this.currentLecture.lectureDetail;
    this.currentLecture = this.lectures.find(p => p.lectureId === this.currentLecture.lectureId);
    this.currentLecture.lectureDetail = lectureDetail;
  }
}
