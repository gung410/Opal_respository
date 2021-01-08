import {
  Assignment,
  AssignmentRepository,
  BrokenLinkContentType,
  BrokenLinkModuleIdentifier,
  ISaveAssignmentQuizAnswerRequest,
  MyAssignment,
  MyAssignmentApiService,
  ParticipantAssignmentTrack,
  ParticipantAssignmentTrackRepository,
  QuizAssignmentFormQuestion,
  UserRepository
} from '@opal20/domain-api';
import { BaseFormComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, HostBinding, Input, QueryList, ViewChildren } from '@angular/core';
import { DialogAction, OpalDialogService } from '@opal20/common-components';
import { Observable, Subscription, combineLatest, from, of } from 'rxjs';

import { AssignmentDetailViewModel } from './../../view-models/assignment-detail-view.model';
import { AssignmentMode } from '../../models/assignment-mode.model';
import { AssignmentPlayerIntegrationsService } from './../../services/assignment-player-integrations.service';
import { AssignmentQuestionEditorComponent } from './assignment-question-editor.component';
import { BrokenLinkReportDialogComponent } from '../broken-link-report-dialog/broken-link-report-dialog.component';
import { ClassRunDetailMode } from './../../models/classrun-detail-mode.model';
import { CourseDetailMode } from './../../models/course-detail-mode.model';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { LMMTabConfiguration } from './../../module-constants/lmm/tab';
import { MY_ASSIGNMENT_STATUS_COLOR_MAP } from './../../models/my-assignment-status-color-map.model';
import { ParticipantAssignmentTrackDetailViewModel } from './../../view-models/participant-assignment-track-detail-view.model';
import { WebAppLinkBuilder } from './../../helpers/webapp-link-builder.helper';
import { switchMap } from 'rxjs/operators';

@Component({
  selector: 'main-assignment-player',
  templateUrl: './main-assignment-player.component.html'
})
export class MainAssignmentPlayerComponent extends BaseFormComponent {
  @ViewChildren(AssignmentQuestionEditorComponent) public questionEditors: QueryList<AssignmentQuestionEditorComponent>;
  @HostBinding('class.-preview-mobile') public get previewMobileHostBinding(): boolean {
    return this.isMobileMode;
  }

  @Input() public isMobileMode: boolean = false;
  public _assignmentData: Assignment | undefined;
  public get assignmentData(): Assignment | undefined {
    return this._assignmentData;
  }
  @Input()
  public set assignmentData(v: Assignment | undefined) {
    this._assignmentData = v;
    if (this.initiated) {
      this.loadData();
    }
  }
  @Input() public forPreview: boolean = false;

  public assignmentId: string;
  public participantAssignmentTrackId: string;
  public assignmentVm: AssignmentDetailViewModel = new AssignmentDetailViewModel();
  public participantAssignmentTrackVm: ParticipantAssignmentTrackDetailViewModel = new ParticipantAssignmentTrackDetailViewModel();
  public AssignmentMode: typeof AssignmentMode = AssignmentMode;
  public urlListData: string[] = [];
  public myAssignment: MyAssignment = new MyAssignment();
  public get disableSetCorrectAnswer(): boolean {
    return this.participantAssignmentTrackVm.submittedDate == null ? false : true;
  }

  public get disableSubmitButton(): boolean {
    const questionsDontHaveAnswer = this.participantAssignmentTrackVm.assignmentVm.questions.filter(x => !x.hasCorrectAnswer());
    return questionsDontHaveAnswer.length > 0;
  }

  public get statusColorMap(): unknown {
    return MY_ASSIGNMENT_STATUS_COLOR_MAP;
  }

  private _loadDataSub: Subscription = new Subscription();
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private assignmentRepository: AssignmentRepository,
    private participantAssignmentTrackRepository: ParticipantAssignmentTrackRepository,
    private assignmentPlayerIntegrationsService: AssignmentPlayerIntegrationsService,
    private userRepository: UserRepository,
    private myAssignmentApiService: MyAssignmentApiService,
    private opalDialogService: OpalDialogService
  ) {
    super(moduleFacadeService);
  }

  public assignmentQuestionsDataTrackByFn(index: number, item: QuizAssignmentFormQuestion): string | QuizAssignmentFormQuestion {
    return item.id;
  }

  public onQuestionChanged(newData: QuizAssignmentFormQuestion): void {
    this.assignmentVm.updateQuestion(p => {
      const questionIndex = p.findIndex(x => x.id === newData.id);
      if (questionIndex > -1) {
        p[questionIndex] = newData;
      }

      this.participantAssignmentTrackVm.updateAssignmentAnswerTrack(newData);
      this.assignmentPlayerIntegrationsService.notifyAssignmentQuestionChanged(newData);
    });
  }

  public onSubmit(): void {
    this.validateAndSaveLearnerAnswerForAssignment(true).subscribe(_ =>
      this.assignmentPlayerIntegrationsService.notifyAssignmentSubmitted(_)
    );
  }

  public onSave(): void {
    from(this.saveLearnerAnswerForAssignment(false)).subscribe(_ => this.assignmentPlayerIntegrationsService.notifyAssignmentSaved(_));
  }

  public onBack(): void {
    from(this.saveLearnerAnswerForAssignment(false, false)).subscribe(_ => {
      this.assignmentPlayerIntegrationsService.notifyAssignmentSaved(_);
      this.assignmentPlayerIntegrationsService.notifyAssignmentBack();
    });
  }

  public saveLearnerAnswerForAssignment(isSubmit: boolean, showMessage: boolean = true): Promise<ParticipantAssignmentTrack> {
    const request: ISaveAssignmentQuizAnswerRequest = {
      registrationId: this.participantAssignmentTrackVm.participantAssignmentTrack.registrationId,
      assignmentId: this.assignmentId,
      questionAnswers: this.participantAssignmentTrackVm.assignmentVm.questions.map(x => {
        return {
          quizAssignmentFormQuestionId: x.id,
          answerValue: x.question_CorrectAnswer
        };
      }),
      isSubmit: isSubmit
    };
    return this.participantAssignmentTrackRepository
      .saveAssignmentQuizAnswer(request)
      .toPromise()
      .then(_ => {
        this.participantAssignmentTrackVm = new ParticipantAssignmentTrackDetailViewModel(_, this.assignmentVm, true);
        if (!AppGlobal.assignmentPlayerIntegrations.isMobile && showMessage) {
          this.showNotification(
            isSubmit ? this.translate('Assignment submitted successfully') : this.translate('Assignment saved successfully')
          );
        }
        return _;
      });
  }

  public showBackConfirmationDialog(): void {
    if (!this.dataHasChanged()) {
      this.assignmentPlayerIntegrationsService.notifyAssignmentBack();
      return;
    }

    this.opalDialogService
      .openConfirmDialog({
        confirmTitle: 'Warning',
        confirmMsg: 'You have unsaved changes, would you like to save it now?',
        hideNoBtn: false,
        yesBtnText: 'Yes'
      })
      .subscribe(action => {
        if (action === DialogAction.Cancel) {
          this.assignmentPlayerIntegrationsService.notifyAssignmentBack();
        } else if (action === DialogAction.OK) {
          this.onBack();
        }
      });
  }

  public validateAndSaveLearnerAnswerForAssignment(isSubmit: boolean): Observable<ParticipantAssignmentTrack> {
    return from(
      new Promise<ParticipantAssignmentTrack>((resolve, reject) => {
        this.validate().then(val => {
          if (val) {
            this.saveLearnerAnswerForAssignment(isSubmit).then(_ => {
              resolve(_);
            }, reject);
          } else {
            reject('validation error');
          }
        }, reject);
      })
    );
  }

  public canShowReportBrokenLinkBtn(): boolean {
    return this.urlListData.length > 0;
  }

  public openReportBrokenLinkDialog(): void {
    this.userRepository
      .loadPublicUserInfoList(
        {
          userIds: [this.assignmentVm.createdBy]
        },
        true
      )
      .subscribe(pagedOwners => {
        const owner = pagedOwners.find(u => u.id === this.assignmentVm.createdBy);
        const dialogRef: DialogRef = this.moduleFacadeService.dialogService.open({ content: BrokenLinkReportDialogComponent });
        const configurationPopup = dialogRef.content.instance as BrokenLinkReportDialogComponent;
        configurationPopup.urlData = this.urlListData;
        configurationPopup.objectId = this.assignmentVm.id;
        configurationPopup.module = BrokenLinkModuleIdentifier.Course;
        configurationPopup.contentType = BrokenLinkContentType.Assignment;
        configurationPopup.objectOwnerName = owner ? owner.fullName : '';
        configurationPopup.objectTitle = this.assignmentVm.title;
        configurationPopup.objectOwnerId = this.assignmentVm.createdBy;
        configurationPopup.parentId = this.assignmentVm.classRunId ? this.assignmentVm.classRunId : this.assignmentVm.courseId;
        configurationPopup.objectDetailUrl = this.assignmentVm.classRunId
          ? WebAppLinkBuilder.buildClassRunDetailLinkForLMMModule(
              LMMTabConfiguration.CoursesTab,
              LMMTabConfiguration.ClassRunsTab,
              LMMTabConfiguration.AllClassRunsTab,
              CourseDetailMode.View,
              LMMTabConfiguration.ClassRunInfoTab,
              ClassRunDetailMode.View,
              this.assignmentVm.courseId,
              this.assignmentVm.classRunId
            )
          : WebAppLinkBuilder.buildCourseDetailLinkForLMMModule(
              LMMTabConfiguration.CoursesTab,
              LMMTabConfiguration.CourseInfoTab,
              LMMTabConfiguration.AllClassRunsTab,
              CourseDetailMode.View,
              this.assignmentVm.courseId
            );
      });
  }

  protected onInit(): void {
    this.loadData();
  }

  protected additionalCanSaveCheck(): Promise<boolean> {
    return Promise.all(
      this.questionEditors
        .toArray()
        .reverse()
        .map(p => p.validate())
    ).then(finalResult => {
      return !finalResult.includes(false);
    });
  }

  private loadData(): void {
    this._loadDataSub.unsubscribe();
    this._loadDataSub = combineLatest(
      this.assignmentData ? of(this.assignmentData.id) : this.assignmentPlayerIntegrationsService.assignmentId$,
      this.assignmentPlayerIntegrationsService.participantAssignmentTrackId$
    )
      .pipe(
        this.untilDestroy(),
        switchMap(([assignmentId, participantAssignmentTrackId]) => {
          this.assignmentId = assignmentId;
          this.participantAssignmentTrackId = participantAssignmentTrackId;
          const assignmentObs: Observable<Assignment | null> =
            this.assignmentId != null && this.assignmentData == null
              ? this.assignmentRepository.getAssignmentById(this.assignmentId, true)
              : of(this.assignmentData);
          const myAssignmentObs: Observable<MyAssignment[] | null> =
            this.assignmentId != null && !this.forPreview
              ? from(this.myAssignmentApiService.getMyAssignmentsByAssignmentIds([this.assignmentId], false))
              : of(null);
          const participantAssignmentTrackObs: Observable<ParticipantAssignmentTrack | null> =
            this.participantAssignmentTrackId != null
              ? this.participantAssignmentTrackRepository.getParticipantAssignmentTrackById(this.participantAssignmentTrackId)
              : of(null);
          return combineLatest(assignmentObs, myAssignmentObs, participantAssignmentTrackObs);
        })
      )
      .subscribe(([assignment, myAssignments, participantAssignmentTrack]) => {
        if (myAssignments != null && myAssignments.length > 0) {
          this.myAssignment = myAssignments[0];
        }
        this.assignmentVm = new AssignmentDetailViewModel(assignment);
        this.participantAssignmentTrackVm = new ParticipantAssignmentTrackDetailViewModel(
          participantAssignmentTrack,
          this.assignmentVm,
          true
        );
        this.assignmentPlayerIntegrationsService.notifyAssignmentInitiated();
        this.urlListData = Utils.flatTwoDimensionsArray(
          this.assignmentVm.questions.map(p => p.question_Title).map(p => this.extractUrlList(p))
        );
      });
  }

  private dataHasChanged(): boolean {
    return this.participantAssignmentTrackVm && this.participantAssignmentTrackVm.dataHasChanged();
  }

  private extractUrlList(html: string): string[] {
    if (Utils.isNullOrUndefined(html)) {
      return [];
    }
    return Utils.extracUrlfromHtml(html);
  }
}
