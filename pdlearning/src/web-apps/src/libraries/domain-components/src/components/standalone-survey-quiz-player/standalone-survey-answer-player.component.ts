import { BaseComponent, Guid, ModuleFacadeService, TranslationMessage, Utils } from '@opal20/infrastructure';
import {
  BrokenLinkContentType,
  BrokenLinkModuleIdentifier,
  ISurveySection,
  ISurveySectionViewModel,
  IUpdateSurveyAnswerRequest,
  StandaloneSurveyQuestionAnswerModel,
  StandaloneSurveyQuestionModelAnswerValue,
  StandaloneSurveyQuestionType,
  SurveyAnswerModel,
  SurveyQuestionModel,
  SurveySection,
  SurveySectionViewModel,
  SurveyWithQuestionsModel
} from '@opal20/domain-api';
import { Component, ElementRef, EventEmitter, Input, Output, ViewChild } from '@angular/core';

import { Align } from '@progress/kendo-angular-popup';
import { BrokenLinkReportDialogComponent } from '../broken-link-report-dialog/broken-link-report-dialog.component';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { PlayerHelpers } from '../../helpers/player.helper';
import { StandaloneSurveyQuestionAnswerPlayerComponent } from './standalone-survey-question-answer-player.component';
import { WebAppLinkBuilder } from '../../helpers/webapp-link-builder.helper';
import { shuffle } from 'lodash-es';

@Component({
  selector: 'standalone-survey-answer-player',
  templateUrl: './standalone-survey-answer-player.component.html'
})
export class StandaloneSurveyAnswerPlayerComponent extends BaseComponent {
  public get formData(): SurveyWithQuestionsModel {
    return this._formData;
  }
  @Input()
  public set formData(v: SurveyWithQuestionsModel) {
    if (!Utils.isDifferent(this._formData, v)) {
      return;
    }
    this._formData = v;
    if (this.initiated) {
      this.orderedQuestions = this.getOrderedFormQuestions();
    }
  }

  public get formAnswer(): SurveyAnswerModel {
    return this._formAnswer;
  }
  @Input()
  public set formAnswer(v: SurveyAnswerModel) {
    if (!Utils.isDifferent(this._formAnswer, v)) {
      return;
    }
    this._formAnswer = v;
    if (this.initiated) {
      this.buildDictionary(this._formData);
      this.orderedQuestions = this.getOrderedFormQuestions();
      this.updateAnswerDic();
      this.questionIdToFormAnswerDic = Utils.toDictionary(v.questionAnswers, p => p.formQuestionId);
      this.formQuestionIdToOptionCodeOrderListDic = this.buildFormQuestionIdToOptionCodeOrderListDic();
    }
    if (!this.justStartNew) {
      this.updateAnswerDic();
    }
  }
  @Input() public justStartNew: boolean = false;
  @Input() public formQuestionsData: SurveyQuestionModel[];
  @Input() public isPreviewMode: boolean = false;
  @Input() public isPassingMarkEnabled: boolean;
  @Input() public reachNumberOfAttemptToShowReviewPage: boolean = false;
  @Input() public reviewOnly: boolean = false;
  @Input() public isStandalone: boolean = false;

  @Output('formAnswerChange') public formAnswerChangeEvent: EventEmitter<SurveyAnswerModel> = new EventEmitter();
  @Output('exit') public exitEvent: EventEmitter<unknown> = new EventEmitter<unknown>();
  @Output('finish') public finishEvent: EventEmitter<unknown> = new EventEmitter<unknown>();
  @Output('retry') public retryEvent: EventEmitter<unknown> = new EventEmitter<unknown>();
  @Output('timeout') public timeoutEvent: EventEmitter<unknown> = new EventEmitter<unknown>();
  @Output('submit') public submitEvent: EventEmitter<IUpdateSurveyAnswerRequest> = new EventEmitter();
  @ViewChild('formQuestionAnswerPlayer', { static: false }) public formQuestionAnswerPlayer: StandaloneSurveyQuestionAnswerPlayerComponent;

  public randomizedQuestionsList: SurveyQuestionModel[];
  public orderedQuestions: SurveyQuestionModel[] = [];
  public currentQuestionIndex: number = 0;
  public sectionsQuestion: SurveySectionViewModel[] = [];
  public get currentQuestionNumberDescTranslateParams(): { questionNumber: number; total: number } {
    return { questionNumber: this.currentQuestionIndex + 1, total: this.orderedQuestions.length };
  }
  public questionIdToAnswerValueDic: Dictionary<StandaloneSurveyQuestionModelAnswerValue | undefined> = {};
  public questionIdToFormAnswerDic: Dictionary<StandaloneSurveyQuestionAnswerModel | undefined> = {};
  public firstQuestionSectionDic: Dictionary<SurveyQuestionModel> = {};
  public questionsSectionDic: Dictionary<SurveyQuestionModel[]> = {};
  public sectionHasBranchingDic: Dictionary<SurveySection> = {};
  public get currentQuestion(): SurveyQuestionModel | undefined {
    return this.orderedQuestions[this.currentQuestionIndex];
  }
  public get currentQuestionAnswer(): StandaloneSurveyQuestionAnswerModel | undefined {
    if (this.currentQuestion === undefined) {
      return undefined;
    }
    return this.questionIdToFormAnswerDic[this.currentQuestion.id];
  }
  public get currentSectionId(): string {
    return this.currentQuestion.formSectionId;
  }
  public get formSectionList(): ISurveySection[] {
    return this.formData.formSections;
  }
  public get currentSection(): SurveySectionViewModel {
    if (this.currentSectionId) {
      return this.sectionsQuestion.find(s => s.sectionId === this.currentSectionId);
    }
  }
  public showingFinishResult: boolean = false;
  public showingFeedbackPage: boolean = false;
  public showingFinishScore: boolean = false;
  public showingPollResults: boolean = false;
  public get correctAnswersInfoTranslationParams(): unknown {
    return { questionNumber: this.getCorrectAnswers().length, total: this.orderedQuestions.length };
  }
  public formQuestionIdToOptionCodeOrderListDic: Dictionary<string[]> = {};

  public popupAlign: Align = { horizontal: 'left', vertical: 'bottom' };
  public showHintPopup: boolean = false;
  public nextQuestions: (SurveyQuestionModel | SurveySection)[] = [];

  @ViewChild('hintAnchor', { static: false }) private hintAnchorElementRef: ElementRef;

  private _formAnswer: SurveyAnswerModel;
  private _formData: SurveyWithQuestionsModel;
  private quizTimedOut: boolean = false;
  private formMaxScore: number | undefined;

  constructor(moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public ngOnInit(): void {
    super.ngOnInit();
    this.buildDictionary(this.formData);
    this.orderedQuestions = this.getOrderedFormQuestions();
    this.sectionsQuestion = this.createOrderedSectionsList(this.orderedQuestions);
    this.questionIdToFormAnswerDic = Utils.toDictionary(this.formAnswer.questionAnswers, p => p.formQuestionId);
    this.currentQuestionIndex = this.getNextQuestionIndex();
    this.formQuestionIdToOptionCodeOrderListDic = this.buildFormQuestionIdToOptionCodeOrderListDic();
  }

  public buildDictionary(formData: SurveyWithQuestionsModel): void {
    this.firstQuestionSectionDic = Utils.toDictionary(
      formData.formQuestions.filter(question => question.minorPriority === 0),
      question => question.formSectionId
    );
    this.questionsSectionDic = Utils.toDictionaryGroupBy(formData.formQuestions, question => question.formSectionId);
    this.sectionHasBranchingDic = Utils.toDictionary(
      formData.formSections.filter(section => section.nextQuestionId),
      section => section.id
    );
  }

  // Create a section list contain the questions belong to it
  public createOrderedSectionsList(formQuestion: SurveyQuestionModel[]): SurveySectionViewModel[] {
    const sectionsQuestion: SurveySectionViewModel[] = [];

    formQuestion.forEach(question => {
      // If question doesn't exist any section, pushing a new section item into the section list with sectionId = null
      if (!question.formSectionId) {
        const sectionVm: ISurveySectionViewModel = {
          sectionId: null,
          questions: [question]
        };

        sectionsQuestion.push(sectionVm);
      } else {
        // If the question exist in a section, check this section exists
        const section = sectionsQuestion.find(s => s.sectionId === question.formSectionId);
        const sectionIdx = sectionsQuestion.findIndex(s => s.sectionId === question.formSectionId);

        // If the section exist in the section list, pushing the question into its question list
        if (section) {
          sectionsQuestion[sectionIdx].questions.push(question);
        } else {
          // If the section doesn't exist in list section, pushing a new section item into the section list with sectionId = formSectionId
          const sectionFormInfo = this.formSectionList.find(s => s.id === question.formSectionId);
          // Get title and description for each section
          const title = sectionFormInfo.mainDescription ? sectionFormInfo.mainDescription : undefined;
          const desc = sectionFormInfo.additionalDescription ? sectionFormInfo.additionalDescription : undefined;
          const sectionVm: ISurveySectionViewModel = {
            sectionId: question.formSectionId,
            questions: [question],
            title: title,
            description: desc
          };

          sectionsQuestion.push(sectionVm);
        }
      }
    });

    return sectionsQuestion;
  }

  // The added question in a section will have the naming format [Section number].[Question number].
  public get generateCurrentQuestionNumber(): string {
    const curentQuestionId = this.currentQuestion.id;
    let sectionIdx: number;
    let questionIdx: number;
    let isExitInSection: boolean = false;

    for (let idx = 0; idx < this.sectionsQuestion.length; idx++) {
      const section = this.sectionsQuestion[idx];
      const question = section.questions.find(q => q.id === curentQuestionId);

      if (question) {
        sectionIdx = idx;
        questionIdx = section.questions.findIndex(q => q.id === curentQuestionId);

        if (section.sectionId) {
          isExitInSection = true;
        }
        break;
      }
    }

    const currentQuestionNumber = isExitInSection ? `${sectionIdx + 1}.${questionIdx + 1}` : `${sectionIdx + 1}`;

    return currentQuestionNumber;
  }

  public onHintPopupClose(): void {
    this.showHintPopup = false;
  }

  public openHintPopup(): void {
    this.showHintPopup = true;
  }

  public getNextQuestionIndex(): number {
    // Braching question logic

    // If there is no question was answered then return 0 as first question.
    const numberOfAnswer = Utils.countDictionaryValue(this.questionIdToAnswerValueDic);
    if (numberOfAnswer === 0) {
      return 0;
    }

    let nextQuestionId = this.currentQuestion.nextQuestionId;
    switch (this.currentQuestion.questionType) {
      case StandaloneSurveyQuestionType.DropDown:
      case StandaloneSurveyQuestionType.TrueFalse:
      case StandaloneSurveyQuestionType.SingleChoice:
        const currentAnswer = this.questionIdToAnswerValueDic[this.currentQuestion.id];
        const questionOptionSelected = this.currentQuestion.questionOptions.find(p => p.value === currentAnswer);
        if (questionOptionSelected.nextQuestionId && questionOptionSelected.nextQuestionId !== Guid.EMPTY) {
          nextQuestionId = questionOptionSelected.nextQuestionId;
        }
        break;
    }

    if (nextQuestionId && nextQuestionId !== Guid.EMPTY) {
      const nextQuestionIndex = this.orderedQuestions.findIndex(
        p =>
          p.id === nextQuestionId &&
          (this.questionIdToFormAnswerDic[p.id] === undefined || this.questionIdToFormAnswerDic[p.id].submittedDate === undefined)
      );
      if (nextQuestionIndex > -1) {
        return nextQuestionIndex;
      }
    }

    // Find the question that index grather than current question and its answer is not answered
    const defaultNextQuestionIndex = this.orderedQuestions.findIndex(
      (p, i) =>
        i >= this.currentQuestionIndex &&
        (this.questionIdToFormAnswerDic[p.id] === undefined || this.questionIdToFormAnswerDic[p.id].submittedDate === undefined)
    );
    return defaultNextQuestionIndex;
  }

  public getOrderedFormQuestions(): SurveyQuestionModel[] {
    const formQuestionsData = this.formQuestionsData ? this.formQuestionsData : this.formData.formQuestions;
    formQuestionsData.forEach(question => {
      // if question point to section then set nextQuestionId is first question of section
      if (question.nextQuestionId && this.firstQuestionSectionDic[question.nextQuestionId]) {
        question.nextQuestionId = this.firstQuestionSectionDic[question.nextQuestionId].id;
      }
      // if question is last question of section and section have next question then set next question of last question of section
      if (
        question.formSectionId &&
        this.sectionHasBranchingDic[question.formSectionId] &&
        question.minorPriority + 1 === this.questionsSectionDic[question.formSectionId].length
      ) {
        const nextQuestionIdOfSection = this.sectionHasBranchingDic[question.formSectionId].nextQuestionId;
        question.nextQuestionId = this.firstQuestionSectionDic[nextQuestionIdOfSection]
          ? this.firstQuestionSectionDic[nextQuestionIdOfSection].id
          : nextQuestionIdOfSection;
      }
      // update next question for question option
      if (
        question.questionOptions != null &&
        (question.questionType === StandaloneSurveyQuestionType.DropDown ||
          question.questionType === StandaloneSurveyQuestionType.TrueFalse ||
          question.questionType === StandaloneSurveyQuestionType.SingleChoice)
      ) {
        question.questionOptions.forEach(option => {
          if (option.nextQuestionId && this.firstQuestionSectionDic[option.nextQuestionId]) {
            option.nextQuestionId = this.firstQuestionSectionDic[option.nextQuestionId].id;
          }
        });
      }
    });

    if (this.formAnswer.formMetaData.questionIdOrderList !== undefined && this.formAnswer.formMetaData.questionIdOrderList.length > 0) {
      const formQuestionsDic = Utils.toDictionary(formQuestionsData, item => item.id);
      return this.formAnswer.formMetaData.questionIdOrderList.map(id => formQuestionsDic[id]).filter(_ => _ !== undefined);
    } else {
      return formQuestionsData;
    }
  }

  public updateAnswerDic(): void {
    this.questionIdToAnswerValueDic = Utils.clone(this.questionIdToAnswerValueDic, _ => {
      this.formAnswer.questionAnswers.forEach(p => {
        _[p.formQuestionId] = p.answerValue;
      });
    });
  }

  public exitPlayer(): void {
    this.exitEvent.emit();
  }

  public submitAnswer(): void {
    const currentQuestion = this.orderedQuestions[this.currentQuestionIndex];
    let answer = this.getCurrentQuestionAnswer();
    if (typeof answer === 'string') {
      answer = answer.trim();
    }

    if (answer instanceof Array && currentQuestion.questionType === StandaloneSurveyQuestionType.FillInTheBlanks) {
      answer = answer.map(value => value.toString().trim());
    }
    this.submitEvent.emit({
      formAnswerId: this.formAnswer.id,
      questionAnswers: [
        {
          formQuestionId: this.orderedQuestions[this.currentQuestionIndex].id,
          answerValue: answer,
          isSubmit: true,
          spentTimeInSeconds: undefined
        }
      ],
      isSubmit: this.currentQuestionIndex === this.orderedQuestions.length - 1
    });
  }

  public getCurrentQuestionAnswer(): StandaloneSurveyQuestionModelAnswerValue {
    const currentFormQuestionId = this.orderedQuestions[this.currentQuestionIndex].id;
    return this.questionIdToAnswerValueDic[currentFormQuestionId];
  }

  public isDisabledSubmitAnswer(): boolean {
    const currentAnswerValue = this.getCurrentQuestionAnswer();
    const currentQuestion = this.orderedQuestions[this.currentQuestionIndex];

    switch (currentQuestion.questionType) {
      case StandaloneSurveyQuestionType.FillInTheBlanks:
        let answerArray: StandaloneSurveyQuestionModelAnswerValue[];
        let numberOfCurrentAnswer: number = 0;
        const numberOfCorrectAnswer = currentQuestion.getCorrectAnswerLenght();
        if (currentAnswerValue) {
          answerArray = currentAnswerValue as StandaloneSurveyQuestionModelAnswerValue[];
          numberOfCurrentAnswer = answerArray.length;
        }
        return !currentAnswerValue || answerArray.includes('') || numberOfCurrentAnswer !== numberOfCorrectAnswer;
      case StandaloneSurveyQuestionType.DateRangePicker:
        return !(currentAnswerValue instanceof Array) || (currentAnswerValue instanceof Array && currentAnswerValue.length < 2);
      default:
        return Utils.isNullOrEmpty(currentAnswerValue);
    }
  }

  public openReportBrokenLinkDialog(): void {
    const dialogRef: DialogRef = this.moduleFacadeService.dialogService.open({ content: BrokenLinkReportDialogComponent });
    const configurationPopup = dialogRef.content.instance as BrokenLinkReportDialogComponent;

    const urlData = Utils.extracUrlfromHtml(this.orderedQuestions[this.currentQuestionIndex].questionTitle);

    configurationPopup.urlData = urlData;
    configurationPopup.objectId = this._formData.form.id;
    configurationPopup.originalObjectId = this._formData.form.originalObjectId;
    configurationPopup.module = BrokenLinkModuleIdentifier.LnaForm;
    configurationPopup.contentType = BrokenLinkContentType.Question;
    configurationPopup.objectDetailUrl = WebAppLinkBuilder.buildLnaFormUrl(this._formData.form.id);
    configurationPopup.objectOwnerId = this._formData.form.ownerId;
    configurationPopup.objectOwnerName = this._formData.form.owner ? this._formData.form.owner.fullName : '';
    configurationPopup.objectTitle = this._formData.form.title;
    configurationPopup.isPreviewMode = this.isPreviewMode;
  }

  public canShowNextBtn(): boolean {
    return (
      this.currentQuestionAnswer &&
      this.currentQuestionAnswer.submittedDate &&
      this.getNextQuestionIndex() >= 0 &&
      this.formAnswer.isAvailable() &&
      !this.quizTimedOut
    );
  }

  public canShowFinishBtn(): boolean {
    return (
      this.currentQuestionAnswer &&
      ((this.currentQuestionAnswer.submittedDate && this.getNextQuestionIndex() < 0) ||
        ((this.justStartNew && this.quizTimedOut) || !this.formAnswer.isAvailable()))
    );
  }

  public canShowBrokenLinkReportBtn(): boolean {
    const url = Utils.extracUrlfromHtml(this.orderedQuestions[this.currentQuestionIndex].questionTitle);
    return !Utils.isNullOrEmpty(url);
  }

  public canShowSubmitBtn(): boolean {
    return this.currentQuestionAnswer && !this.currentQuestionAnswer.submittedDate && this.formAnswer.isAvailable();
  }

  public goToNextQuestion(): void {
    this.currentQuestionIndex = this.getNextQuestionIndex();
  }

  public finishTheFormAnswer(): void {
    this.finishEvent.emit();
  }

  public onShowReviewPage(): void {
    this.showingFeedbackPage = true;
    this.showingFinishResult = true;
  }

  public onShowFinishScore(): void {
    this.showingFeedbackPage = false;
    this.showingFinishResult = true;
    this.showingFinishScore = true;
  }

  public onShowPollResults(): void {
    this.showingFeedbackPage = false;
    this.showingFinishResult = true;
    this.showingFinishScore = false;
    this.showingPollResults = true;
  }

  public retryTheFormAnswer(): void {
    this.retryEvent.emit();
  }

  public currentQuestionAnswerChanged(e: StandaloneSurveyQuestionAnswerModel): void {
    this.formAnswer = Utils.clone(this.formAnswer, formAnswer => {
      formAnswer.questionAnswers = Utils.addOrReplace(formAnswer.questionAnswers, e, p => p.formQuestionId === e.formQuestionId);
    });
    this.formAnswerChangeEvent.emit(this.formAnswer);
  }

  public getCorrectAnswers(): StandaloneSurveyQuestionAnswerModel[] {
    return this.orderedQuestions
      .filter(p => p.isAnswerCorrect(this.questionIdToFormAnswerDic[p.id].answerValue))
      .map(p => this.questionIdToFormAnswerDic[p.id]);
  }

  public onQuizTimeOut(): void {
    if (this.quizTimedOut) {
      return;
    }
    this.quizTimedOut = true;
    this.timeoutEvent.emit();
    this.moduleFacadeService.modalService.showInformationMessage(
      new TranslationMessage(this.moduleFacadeService.globalTranslator, 'Your quiz time has ended'),
      () => {
        this.finishTheFormAnswer();
      },
      () => {
        this.finishTheFormAnswer();
      }
    );
  }

  public formAnswerScoreInfoDisplay(): string {
    if (this.formMaxScore == null) {
      return 'n/a';
    }
  }

  public getHintAnchor(): ElementRef {
    return this.hintAnchorElementRef.nativeElement;
  }

  public get canDoOnFormPlayer(): boolean {
    return !this.showingFinishResult && this.currentQuestionIndex >= 0 && !this.reviewOnly;
  }

  public get canShowFinishScore(): boolean {
    return (this.showingFinishScore || this.currentQuestionIndex === -1) && !this.reviewOnly;
  }

  public get canShowReviewForm(): boolean {
    return this.showingFeedbackPage || this.reviewOnly;
  }

  public preventRightClick(e: MouseEvent): void {
    PlayerHelpers.preventRightClick(e);
  }

  private shuffleQuestionsList(formQuestion: SurveyQuestionModel[]): SurveyQuestionModel[] {
    // in case has section, all questions in section are counted as one to shuffle
    if (formQuestion.some(x => x.formSectionId)) {
      let questionsList: SurveyQuestionModel[] = [];
      const shuffledSectionsList = shuffle(this.createOrderedSectionsList(formQuestion));

      shuffledSectionsList.forEach(section => {
        questionsList = questionsList.concat(section.questions);
      });

      return questionsList;
    }
    return shuffle(formQuestion);
  }

  private buildFormQuestionIdToOptionCodeOrderListDic(): Dictionary<string[]> | undefined {
    if (this.formAnswer.formMetaData.formQuestionOptionsOrderInfoList === undefined) {
      return {};
    }
    return Utils.toDictionarySelect(
      this.formAnswer.formMetaData.formQuestionOptionsOrderInfoList,
      p => p.formQuestionId,
      p => p.optionCodeOrderList
    );
  }
}
