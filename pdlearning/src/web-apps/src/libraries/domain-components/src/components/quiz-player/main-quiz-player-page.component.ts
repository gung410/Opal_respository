import { BasePageComponent, Guid, ModuleFacadeService, TranslationMessage, Utils } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import {
  FormAnswerModel,
  FormQuestionAnswerModel,
  FormQuestionModel,
  FormQuestionWithAnswerModel,
  FormType,
  FormWithQuestionsModel,
  IUpdateFormAnswerRequest,
  QuestionType,
  SystemRoleEnum,
  UserInfoModel
} from '@opal20/domain-api';
import { Observable, combineLatest, of } from 'rxjs';
import { intersection, orderBy } from 'lodash-es';

import { FormAnswerPlayerComponent } from './form-answer-player.component';
import { MainQuizPlayerPageService } from '../../services/main-quiz-player-page.service';
import { QuizPlayerIntegrationsService } from '../../services/quiz-player-integrations.service';
import { switchMap } from 'rxjs/operators';

@Component({
  selector: 'main-quiz-player-page',
  templateUrl: './main-quiz-player-page.component.html'
})
export class MainQuizPlayerPageComponent extends BasePageComponent {
  public readonly formType: typeof FormType = FormType;

  @Input() public formQuestionsData: FormQuestionModel[];
  @Input() public isPreviewMode: boolean = false;
  @Input() public isStandalone: boolean = false;
  @Input() public isDisplayPollResultToLearners: boolean = true;
  @Input() public isFinished?: boolean;
  @Output('onLoadQuestion') public onLoadQuestion: EventEmitter<FormQuestionWithAnswerModel> = new EventEmitter();
  @Output('onSubmitAnswer') public onSubmitAnswer: EventEmitter<FormQuestionWithAnswerModel> = new EventEmitter();
  @Output('onQuizFinished') public onQuizFinished: EventEmitter<FormAnswerModel> = new EventEmitter();
  @ViewChild('formAnswerPlayer', { static: false }) public formAnswerPlayer: FormAnswerPlayerComponent;

  public currentMyCourseId: string = '';
  public currentFormId: string = '';
  public currentResourceId: string | null;
  public currentClassRunId: string | null;
  public currentAssignmentId: string | null;
  public isPassingMarkEnabled: boolean | null;
  public formData: FormWithQuestionsModel | undefined;
  public formAnswersData: FormAnswerModel[] | undefined;
  public currentPlayingFormAnswer: FormAnswerModel | undefined;
  public isCurrentPlayingFormAnswerJustStartNew: boolean | undefined;
  public formLabelMultiLanguage: TranslationMessage;
  public formAnswerData: FormAnswerModel | undefined;
  public showViewButton: boolean = false;
  public reviewOnly: boolean = false;
  public canShowPollResults: boolean = false;
  private previewAttempt: number;

  private currentFormAnswerId: string;

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    protected mainQuizPageSvc: MainQuizPlayerPageService,
    protected integrationsSvc: QuizPlayerIntegrationsService
  ) {
    super(moduleFacadeService);
    this.mainQuizPageSvc.formData$.pipe(this.untilDestroy()).subscribe(formData => {
      this.formData = Utils.clone(formData);
      if (!this.formData) {
        return;
      }
      this.formData.formQuestions = FormQuestionModel.removeUnsupportedQuestionTypes(this.formData.formQuestions);

      switch (this.formData.form.type) {
        case FormType.Survey: {
          this.formLabelMultiLanguage = new TranslationMessage(this.moduleFacadeService.globalTranslator, FormType.Survey);
          break;
        }
        case FormType.Poll: {
          this.formLabelMultiLanguage = new TranslationMessage(this.moduleFacadeService.globalTranslator, FormType.Poll);
          break;
        }
        default: {
          this.formLabelMultiLanguage = new TranslationMessage(this.moduleFacadeService.globalTranslator, FormType.Quiz);
          break;
        }
      }
    });
    this.mainQuizPageSvc.formAnswersData$.pipe(this.untilDestroy()).subscribe(formAnswerData => {
      this.formAnswersData = formAnswerData;
      if (this.currentPlayingFormAnswer !== undefined) {
        this.currentPlayingFormAnswer = Utils.cloneDeep(formAnswerData.find(p => p.id === this.currentPlayingFormAnswer.id));
      }
    });
    this.previewAttempt = 0;
  }

  public ngOnInit(): void {
    super.ngOnInit();
    this.initData();
    this.integrationsSvc.notifyQuizInitiated();
  }

  public loadData(): Observable<[FormWithQuestionsModel, FormAnswerModel[]] | undefined> {
    this.formData = undefined;
    this.formAnswersData = undefined;
    this.currentPlayingFormAnswer = undefined;
    if (this.currentFormId === '') {
      return of(undefined);
    }
    return this.mainQuizPageSvc.loadData(
      this.currentFormId,
      this.currentResourceId,
      this.currentMyCourseId,
      this.currentClassRunId,
      this.currentAssignmentId
    );
  }

  public get canStartNew(): boolean {
    return (!this.hasLastNotCompletedAvailableFormAnswer() && !this.reachFormMaxAnswerAttempt()) || this.isPreviewMode;
  }

  public get canContinue(): boolean {
    return this.hasLastNotCompletedAvailableFormAnswer();
  }
  public hasLastNotCompletedAvailableFormAnswer(): boolean {
    return this.getLastNotCompletedAvailableAnswer() !== undefined;
  }

  public reachFormMaxAnswerAttempt(): boolean {
    return (
      this.formData.form.maxAttempt !== undefined &&
      this.formData.form.maxAttempt > 0 &&
      this.formAnswersData.length >= this.formData.form.maxAttempt
    );
  }

  public getLastNotCompletedAvailableAnswer(): FormAnswerModel | undefined {
    return this.formAnswersData != null &&
      this.formAnswersData.length > 0 &&
      !this.formAnswersData[0].isCompleted &&
      this.formAnswersData[0].isAvailable()
      ? this.formAnswersData[0]
      : this.isFinished !== null && !this.isFinished
      ? this.formAnswersData[0]
      : undefined;
  }

  public getLastAnswer(): FormAnswerModel | undefined {
    return this.formAnswersData != null && this.formAnswersData.length > 0 && !this.reviewOnly
      ? this.formAnswersData[0]
      : this.formAnswerData;
  }

  public continueLastNotCompletedFormAnswer(): void {
    if (!this.hasLastNotCompletedAvailableFormAnswer()) {
      return;
    }
    this.currentPlayingFormAnswer = Utils.cloneDeep(this.getLastNotCompletedAvailableAnswer());
    this.isCurrentPlayingFormAnswerJustStartNew = false;
  }

  public startNewFormAnswer(): void {
    const initalFormAnswer: FormAnswerModel = this.isPreviewMode ? this.initPreviewData() : null;
    this.currentFormAnswerId = initalFormAnswer ? initalFormAnswer.id : null;
    this.mainQuizPageSvc
      .startNewFormAnswer(
        this.currentFormId,
        this.currentResourceId,
        this.currentMyCourseId,
        this.currentClassRunId,
        this.currentAssignmentId,
        initalFormAnswer
      )
      .subscribe(formAnswer => {
        this.currentPlayingFormAnswer = Utils.cloneDeep(formAnswer);
        this.isCurrentPlayingFormAnswerJustStartNew = true;

        this.onLoadQuestion.emit({
          formQuestion: this.formQuestionsData,
          formAnswer: this.currentPlayingFormAnswer
        });
      });
  }

  public exitQuiz(): void {
    this.currentPlayingFormAnswer = undefined;
    this.showViewButton = true;
    this.integrationsSvc.notifyQuizExit(this.currentPlayingFormAnswer);
  }

  public finishQuiz(): void {
    this.integrationsSvc.notifyQuizFinished(this.currentPlayingFormAnswer);
    this.onQuizFinished.emit(this.currentPlayingFormAnswer);
    this.currentPlayingFormAnswer = undefined;
    if (this.formData.form.isStandalone) {
      this.processShowPollResults();
    }
  }

  public retryQuiz(): void {
    this.currentPlayingFormAnswer = undefined;
  }

  public onFormAnswerPlayerSubmitted(formAnswer: IUpdateFormAnswerRequest): void {
    const updatedFormAnswer: FormAnswerModel = this.isPreviewMode ? this.updatePreviewData(formAnswer) : null;
    this.subscribe(this.mainQuizPageSvc.updateFormAnswer(formAnswer, updatedFormAnswer), _ => {
      if (formAnswer.isSubmit) {
        this.integrationsSvc.notifyQuizSubmitted(_);
      }
    });

    this.onSubmitAnswer.emit({
      formQuestion: this.formQuestionsData,
      formAnswer: this.currentPlayingFormAnswer
    });
  }

  public answeredNumberDescTranslationParams(formAnswerModel: FormAnswerModel | undefined): unknown {
    if (formAnswerModel === undefined) {
      return undefined;
    }
    return {
      questionNumber: formAnswerModel.questionAnswers.filter(p => p.answerValue !== undefined && p.submittedDate !== undefined).length,
      total: formAnswerModel.questionAnswers.length
    };
  }

  public correctAnsweredNumberDescTranslationParams(formAnswerModel: FormAnswerModel | undefined): unknown {
    if (formAnswerModel === undefined) {
      return undefined;
    }
    return {
      questionNumber: formAnswerModel.questionAnswers.filter(p => FormQuestionAnswerModel.isCorrect(p) && p.submittedDate !== undefined)
        .length,
      total: formAnswerModel.questionAnswers.length
    };
  }

  public onTimeout(): void {
    this.integrationsSvc.notifyQuizTimeout(this.currentPlayingFormAnswer);
  }

  public get reachNumberOfAttemptToShowReviewPage(): boolean {
    if (this.isPreviewMode) {
      return (
        this.formData.form.attemptToShowFeedback !== undefined &&
        this.formData.form.attemptToShowFeedback > 0 &&
        this.previewAttempt > this.formData.form.attemptToShowFeedback
      );
    } else {
      return (
        this.formData.form.maxAttempt !== undefined &&
        this.formData.form.maxAttempt > 0 &&
        this.formAnswersData.length >= this.formData.form.attemptToShowFeedback
      );
    }
  }

  public viewFormAnswer(): void {
    this.currentPlayingFormAnswer = this.formAnswerData;
    this.showViewButton = false;
  }

  public canDisplayPollResultToLearners(): boolean {
    const currentUser = UserInfoModel.getMyUserInfo();
    if (currentUser.hasOnlyRole(SystemRoleEnum.Learner)) {
      return this.isDisplayPollResultToLearners;
    }
    return true;
  }

  protected onDestroy(): void {
    this.removeExistingStorageItem('FORMANSWER_' + this.currentFormAnswerId);
  }

  private removeExistingStorageItem(key: string): void {
    if (localStorage.getItem(key)) {
      localStorage.removeItem(key);
    }
  }
  private initPreviewData(): FormAnswerModel {
    const previewFormAnswerModel = new FormAnswerModel();
    previewFormAnswerModel.id = this.currentFormAnswerId ? this.currentFormAnswerId : Guid.create().toString();
    previewFormAnswerModel.formId = this.currentFormId;
    previewFormAnswerModel.startDate = new Date();
    previewFormAnswerModel.score = 0;
    previewFormAnswerModel.scorePercentage = 0;
    previewFormAnswerModel.attempt = this.previewAttempt;
    previewFormAnswerModel.formMetaData = {
      questionIdOrderList: [],
      formQuestionOptionsOrderInfoList: []
    };
    previewFormAnswerModel.formMetaData.formQuestionOptionsOrderInfoList = [];
    previewFormAnswerModel.formMetaData.questionIdOrderList = [];
    previewFormAnswerModel.createdBy = null;
    previewFormAnswerModel.questionAnswers = new Array<FormQuestionAnswerModel>();

    this.formQuestionsData.forEach(formQuestion => {
      const formQuestionAnswer: FormQuestionAnswerModel = new FormQuestionAnswerModel();
      formQuestionAnswer.formAnswerId = previewFormAnswerModel.id;
      formQuestionAnswer.formQuestionId = formQuestion.id;
      formQuestionAnswer.score = 0;
      formQuestionAnswer.maxScore = FormQuestionModel.calcMaxScore(Array.of(formQuestion));
      previewFormAnswerModel.questionAnswers.push(formQuestionAnswer);
    });
    previewFormAnswerModel.isCompleted = false;

    localStorage.setItem('FORMANSWER_' + previewFormAnswerModel.id, JSON.stringify(previewFormAnswerModel));

    return previewFormAnswerModel;
  }

  private updatePreviewData(formAnswer: IUpdateFormAnswerRequest): FormAnswerModel {
    const currentRetrievedFormAnswer = JSON.parse(localStorage.getItem('FORMANSWER_' + formAnswer.formAnswerId));
    const previewFormAnswerModel = new FormAnswerModel(currentRetrievedFormAnswer);
    previewFormAnswerModel.id = currentRetrievedFormAnswer.id;
    previewFormAnswerModel.formId = currentRetrievedFormAnswer.formId;
    previewFormAnswerModel.startDate = currentRetrievedFormAnswer.startDate;
    previewFormAnswerModel.submitDate = new Date();
    previewFormAnswerModel.questionAnswers = new Array<FormQuestionAnswerModel>();
    previewFormAnswerModel.questionAnswers = Utils.cloneDeep(Utils.cloneDeep(currentRetrievedFormAnswer.questionAnswers));

    const currentQuestionAnswer = previewFormAnswerModel.questionAnswers.find(
      item => item.formQuestionId === formAnswer.questionAnswers[0].formQuestionId
    );
    currentQuestionAnswer.answerValue = formAnswer.questionAnswers[0].answerValue;
    currentQuestionAnswer.formAnswerAttachments = formAnswer.questionAnswers[0].formAnswerAttachments;
    currentQuestionAnswer.submittedDate = new Date();

    const formQuestion = this.formQuestionsData.find(question => question.id === currentQuestionAnswer.formQuestionId);
    if (formQuestion.isAnswerCorrect(currentQuestionAnswer.answerValue)) {
      let score = 0;
      if (formQuestion.questionType !== QuestionType.MultipleChoice) {
        score = currentQuestionAnswer.maxScore;
      } else {
        const correctAnswerSelected = intersection(
          currentQuestionAnswer.answerValue as string[],
          formQuestion.questionCorrectAnswer as string[]
        );
        const totalCorrectAnswer = (formQuestion.questionCorrectAnswer as string[]).length;
        score = Math.round((currentQuestionAnswer.maxScore / totalCorrectAnswer) * correctAnswerSelected.length * 100) / 100;
      }
      currentQuestionAnswer.score = currentQuestionAnswer.score + score;
      previewFormAnswerModel.score = previewFormAnswerModel.score + currentQuestionAnswer.score;
    } else {
      previewFormAnswerModel.score = currentRetrievedFormAnswer.score;
    }

    previewFormAnswerModel.scorePercentage = Number(
      ((previewFormAnswerModel.score / FormQuestionModel.calcMaxScore(this.formQuestionsData)) * 100).toFixed(2)
    );
    previewFormAnswerModel.formMetaData = {
      questionIdOrderList: [],
      formQuestionOptionsOrderInfoList: []
    };
    previewFormAnswerModel.isCompleted = !previewFormAnswerModel.questionAnswers.some(fq => Utils.isNullOrEmpty(fq.answerValue));
    this.previewAttempt = previewFormAnswerModel.isCompleted ? this.previewAttempt + 1 : this.previewAttempt;
    previewFormAnswerModel.attempt = this.previewAttempt;

    // Store the object in local storage
    localStorage.setItem('FORMANSWER_' + previewFormAnswerModel.id, JSON.stringify(previewFormAnswerModel));

    return previewFormAnswerModel;
  }

  private initData(): void {
    combineLatest(
      this.integrationsSvc.myCourseId$,
      this.integrationsSvc.resourceId$,
      this.integrationsSvc.classRunId$,
      this.integrationsSvc.assignmentId$,
      this.integrationsSvc.formId$,
      this.integrationsSvc.isPassingMarkEnabled$,
      this.integrationsSvc.reviewOnly$
    )
      .pipe(
        this.untilDestroy(),
        switchMap(([myCourseId, resourceId, classRunId, assignmentId, formId, isPassingMarkEnabled, reviewOnly]) => {
          this.currentMyCourseId = myCourseId;
          this.currentResourceId = resourceId;
          this.currentClassRunId = classRunId;
          this.currentAssignmentId = assignmentId;
          this.currentFormId = formId;
          this.isPassingMarkEnabled = isPassingMarkEnabled;
          this.reviewOnly = reviewOnly;
          return this.loadData();
        })
      )
      .subscribe(response => {
        if (this.reviewOnly) {
          this.formQuestionsData = response[0] && response[0].formQuestions;
          const formAnswers = response[1] && orderBy(response[1], p => p.submitDate, 'desc');
          const formAnswer = formAnswers.find(p => p.isCompleted);
          this.formAnswerData = formAnswer;
          this.showViewButton = true;
        }
        this.processShowPollResults();
      });
  }

  private processShowPollResults(): void {
    if (
      !this.isPreviewMode &&
      this.formData &&
      this.formData.form.type === FormType.Poll &&
      (this.reviewOnly || this.formData.form.isStandalone) &&
      this.formAnswersData &&
      this.formAnswersData.length > 0 &&
      this.formAnswersData[this.formAnswersData.length - 1].isCompleted
    ) {
      this.showPollResults();
    }
  }

  private showPollResults(): void {
    this.canShowPollResults = true;
    this.currentPlayingFormAnswer = null;
  }
}
