import { BasePageComponent, Guid, ModuleFacadeService, TranslationMessage, Utils } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import {
  IUpdateSurveyAnswerRequest,
  StandaloneSurveyQuestionAnswerModel,
  SurveyAnswerModel,
  SurveyQuestionModel,
  SurveyQuestionWithAnswerModel,
  SurveyWithQuestionsModel
} from '@opal20/domain-api';
import { Observable, combineLatest, of } from 'rxjs';

import { StandaloneSurveyAnswerPlayerComponent } from './standalone-survey-answer-player.component';
import { StandaloneSurveyQuizPlayerIntegrationsService } from '../../services/standalone-survey-quiz-player-integrations.service';
import { StandaloneSurveyQuizPlayerPageService } from '../../services/standalone-survey-quiz-player-page.service';
import { orderBy } from 'lodash-es';
import { switchMap } from 'rxjs/operators';

@Component({
  selector: 'standalone-survey-quiz-player-page',
  templateUrl: './standalone-survey-quiz-player-page.component.html'
})
export class StandaloneSurveyQuizPlayerPageComponent extends BasePageComponent {
  @Input() public formQuestionsData: SurveyQuestionModel[];
  @Input() public isPreviewMode: boolean = false;
  @Input() public isStandalone: boolean = false;
  @Output('onLoadQuestion') public onLoadQuestion: EventEmitter<SurveyQuestionWithAnswerModel> = new EventEmitter();
  @Output('onSubmitAnswer') public onSubmitAnswer: EventEmitter<SurveyQuestionWithAnswerModel> = new EventEmitter();
  @ViewChild('formAnswerPlayer', { static: false }) public formAnswerPlayer: StandaloneSurveyAnswerPlayerComponent;

  public currentFormId: string = '';
  public currentResourceId: string | null;
  public isPassingMarkEnabled: boolean | null;
  public formData: SurveyWithQuestionsModel | undefined;
  public formAnswersData: SurveyAnswerModel[] | undefined;
  public currentPlayingFormAnswer: SurveyAnswerModel | undefined;
  public isCurrentPlayingFormAnswerJustStartNew: boolean | undefined;
  public formLabelMultiLanguage: TranslationMessage;
  public formAnswerData: SurveyAnswerModel | undefined;
  public showViewButton: boolean = false;
  public reviewOnly: boolean = false;
  public canShowPollResults: boolean = false;
  private previewAttempt: number;

  private currentFormAnswerId: string;

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    protected mainQuizPageSvc: StandaloneSurveyQuizPlayerPageService,
    protected integrationsSvc: StandaloneSurveyQuizPlayerIntegrationsService
  ) {
    super(moduleFacadeService);
    this.mainQuizPageSvc.formData$.pipe(this.untilDestroy()).subscribe(formData => {
      this.formData = Utils.clone(formData);
      if (!this.formData) {
        return;
      }
      this.formData.formQuestions = SurveyQuestionModel.removeUnsupportedQuestionTypes(this.formData.formQuestions);

      this.formLabelMultiLanguage = new TranslationMessage(this.moduleFacadeService.globalTranslator, 'Survey');
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

  public loadData(): Observable<[SurveyWithQuestionsModel, SurveyAnswerModel[]] | undefined> {
    this.formData = undefined;
    this.formAnswersData = undefined;
    this.currentPlayingFormAnswer = undefined;
    if (this.currentFormId === '') {
      return of(undefined);
    }
    return this.mainQuizPageSvc.loadData(this.currentFormId, this.currentResourceId);
  }

  public hasLastNotCompletedAvailableFormAnswer(): boolean {
    return this.getLastNotCompletedAvailableAnswer() !== undefined;
  }

  public getLastNotCompletedAvailableAnswer(): SurveyAnswerModel | undefined {
    return this.formAnswersData != null &&
      this.formAnswersData.length > 0 &&
      !this.formAnswersData[0].isCompleted &&
      this.formAnswersData[0].isAvailable()
      ? this.formAnswersData[0]
      : undefined;
  }

  public getLastAnswer(): SurveyAnswerModel | undefined {
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
    const initalFormAnswer: SurveyAnswerModel = this.isPreviewMode ? this.initPreviewData() : null;
    this.currentFormAnswerId = initalFormAnswer ? initalFormAnswer.id : null;
    this.mainQuizPageSvc.startNewFormAnswer(this.currentFormId, this.currentResourceId, initalFormAnswer).subscribe(formAnswer => {
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
    this.currentPlayingFormAnswer = undefined;
    if (this.formData.form.isStandalone) {
      this.processShowPollResults();
    }
  }

  public retryQuiz(): void {
    this.currentPlayingFormAnswer = undefined;
  }

  public onFormAnswerPlayerSubmitted(formAnswer: IUpdateSurveyAnswerRequest): void {
    const updatedFormAnswer: SurveyAnswerModel = this.isPreviewMode ? this.updatePreviewData(formAnswer) : null;
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

  public answeredNumberDescTranslationParams(formAnswerModel: SurveyAnswerModel | undefined): unknown {
    if (formAnswerModel === undefined) {
      return undefined;
    }
    return {
      questionNumber: formAnswerModel.questionAnswers.filter(p => p.answerValue !== undefined && p.submittedDate !== undefined).length,
      total: formAnswerModel.questionAnswers.length
    };
  }

  public onTimeout(): void {
    this.integrationsSvc.notifyQuizTimeout(this.currentPlayingFormAnswer);
  }

  public viewFormAnswer(): void {
    this.currentPlayingFormAnswer = this.formAnswerData;
    this.showViewButton = false;
  }

  protected onDestroy(): void {
    this.removeExistingStorageItem('FORMANSWER_' + this.currentFormAnswerId);
  }

  private removeExistingStorageItem(key: string): void {
    if (localStorage.getItem(key)) {
      localStorage.removeItem(key);
    }
  }
  private initPreviewData(): SurveyAnswerModel {
    const previewFormAnswerModel = new SurveyAnswerModel();
    previewFormAnswerModel.id = this.currentFormAnswerId ? this.currentFormAnswerId : Guid.create().toString();
    previewFormAnswerModel.formId = this.currentFormId;
    previewFormAnswerModel.startDate = new Date();
    previewFormAnswerModel.attempt = this.previewAttempt;
    previewFormAnswerModel.formMetaData = {
      questionIdOrderList: [],
      formQuestionOptionsOrderInfoList: []
    };
    previewFormAnswerModel.formMetaData.formQuestionOptionsOrderInfoList = [];
    previewFormAnswerModel.formMetaData.questionIdOrderList = [];
    previewFormAnswerModel.ownerId = null;
    previewFormAnswerModel.questionAnswers = new Array<StandaloneSurveyQuestionAnswerModel>();

    this.formQuestionsData.forEach(formQuestion => {
      const formQuestionAnswer: StandaloneSurveyQuestionAnswerModel = new StandaloneSurveyQuestionAnswerModel();
      formQuestionAnswer.formAnswerId = previewFormAnswerModel.id;
      formQuestionAnswer.formQuestionId = formQuestion.id;
      previewFormAnswerModel.questionAnswers.push(formQuestionAnswer);
    });
    previewFormAnswerModel.isCompleted = false;

    localStorage.setItem('FORMANSWER_' + previewFormAnswerModel.id, JSON.stringify(previewFormAnswerModel));

    return previewFormAnswerModel;
  }

  private updatePreviewData(formAnswer: IUpdateSurveyAnswerRequest): SurveyAnswerModel {
    const currentRetrievedFormAnswer = JSON.parse(localStorage.getItem('FORMANSWER_' + formAnswer.formAnswerId));
    const previewFormAnswerModel = new SurveyAnswerModel(currentRetrievedFormAnswer);
    previewFormAnswerModel.id = currentRetrievedFormAnswer.id;
    previewFormAnswerModel.formId = currentRetrievedFormAnswer.formId;
    previewFormAnswerModel.startDate = currentRetrievedFormAnswer.startDate;
    previewFormAnswerModel.submitDate = new Date();
    previewFormAnswerModel.questionAnswers = new Array<StandaloneSurveyQuestionAnswerModel>();
    previewFormAnswerModel.questionAnswers = Utils.cloneDeep(Utils.cloneDeep(currentRetrievedFormAnswer.questionAnswers));

    const currentQuestionAnswer = previewFormAnswerModel.questionAnswers.find(
      item => item.formQuestionId === formAnswer.questionAnswers[0].formQuestionId
    );
    currentQuestionAnswer.answerValue = formAnswer.questionAnswers[0].answerValue;
    currentQuestionAnswer.submittedDate = new Date();

    previewFormAnswerModel.formMetaData = {
      questionIdOrderList: [],
      formQuestionOptionsOrderInfoList: []
    };
    previewFormAnswerModel.isCompleted = !previewFormAnswerModel.questionAnswers.some(fq => Utils.isNullOrEmpty(fq.answerValue));
    this.previewAttempt = previewFormAnswerModel.isCompleted ? this.previewAttempt + 1 : this.previewAttempt;
    previewFormAnswerModel.attempt = this.previewAttempt;

    localStorage.setItem('FORMANSWER_' + previewFormAnswerModel.id, JSON.stringify(previewFormAnswerModel));

    return previewFormAnswerModel;
  }

  private initData(): void {
    combineLatest(
      this.integrationsSvc.resourceId$,
      this.integrationsSvc.formId$,
      this.integrationsSvc.isPassingMarkEnabled$,
      this.integrationsSvc.reviewOnly$
    )
      .pipe(
        this.untilDestroy(),
        switchMap(([resourceId, formId, isPassingMarkEnabled, reviewOnly]) => {
          this.currentResourceId = resourceId;
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
