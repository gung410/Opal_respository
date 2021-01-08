import {
  AnswerFeedbackDisplayOption,
  BrokenLinkContentType,
  BrokenLinkModuleIdentifier,
  FormAnswerModel,
  FormQuestionAnswerModel,
  FormQuestionModel,
  FormQuestionModelAnswerValue,
  FormSection,
  FormSectionViewModel,
  FormType,
  FormWithQuestionsModel,
  IFormSection,
  IFormSectionViewModel,
  IUpdateFormAnswerRequest,
  QuestionType
} from '@opal20/domain-api';
import { BaseComponent, Guid, ModuleFacadeService, TranslationMessage, Utils } from '@opal20/infrastructure';
import { Component, ElementRef, EventEmitter, Input, Output, ViewChild } from '@angular/core';

import { Align } from '@progress/kendo-angular-popup';
import { BrokenLinkReportDialogComponent } from '../broken-link-report-dialog/broken-link-report-dialog.component';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { FormQuestionAnswerPlayerComponent } from './form-question-answer-player.component';
import { PlayerHelpers } from '../../helpers/player.helper';
import { WebAppLinkBuilder } from './../../helpers/webapp-link-builder.helper';
import { shuffle } from 'lodash-es';

@Component({
  selector: 'form-answer-player',
  templateUrl: './form-answer-player.component.html',
  host: {
    '(contextmenu)': 'preventRightClick($event)'
  }
})
export class FormAnswerPlayerComponent extends BaseComponent {
  public get formData(): FormWithQuestionsModel {
    return this._formData;
  }
  @Input()
  public set formData(v: FormWithQuestionsModel) {
    if (!Utils.isDifferent(this._formData, v)) {
      return;
    }
    this._formData = v;
    this.formMaxScore = FormQuestionModel.calcMaxScore(v.formQuestions);
    if (this.initiated) {
      this.orderedQuestions = this.getOrderedFormQuestions();
    }
  }

  public get formAnswer(): FormAnswerModel {
    return this._formAnswer;
  }
  @Input()
  public set formAnswer(v: FormAnswerModel) {
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
  @Input() public isFinished?: boolean = true;
  @Input() public justStartNew: boolean = false;
  @Input() public formQuestionsData: FormQuestionModel[];
  @Input() public isPreviewMode: boolean = false;
  @Input() public isPassingMarkEnabled: boolean;
  @Input() public reachNumberOfAttemptToShowReviewPage: boolean = false;
  @Input() public reviewOnly: boolean = false;
  @Input() public myCourseId: string = '';
  @Input() public isStandalone: boolean = false;
  @Input() public isDisplayPollResultToLearners: boolean = true;

  @Output('formAnswerChange') public formAnswerChangeEvent: EventEmitter<FormAnswerModel> = new EventEmitter();
  @Output('exit') public exitEvent: EventEmitter<unknown> = new EventEmitter<unknown>();
  @Output('finish') public finishEvent: EventEmitter<unknown> = new EventEmitter<unknown>();
  @Output('retry') public retryEvent: EventEmitter<unknown> = new EventEmitter<unknown>();
  @Output('timeout') public timeoutEvent: EventEmitter<unknown> = new EventEmitter<unknown>();
  @Output('submit') public submitEvent: EventEmitter<IUpdateFormAnswerRequest> = new EventEmitter();
  @ViewChild('formQuestionAnswerPlayer', { static: false }) public formQuestionAnswerPlayer: FormQuestionAnswerPlayerComponent;

  public randomizedQuestionsList: FormQuestionModel[];
  public orderedQuestions: FormQuestionModel[] = [];
  public currentQuestionIndex: number = 0;
  public sectionsQuestion: FormSectionViewModel[] = [];
  public get currentQuestionNumberDescTranslateParams(): { questionNumber: number; total: number } {
    return { questionNumber: this.currentQuestionIndex + 1, total: this.orderedQuestions.length };
  }
  public questionIdToAnswerValueDic: Dictionary<FormQuestionModelAnswerValue | undefined> = {};
  public questionIdToFormAnswerDic: Dictionary<FormQuestionAnswerModel | undefined> = {};
  public firstQuestionSectionDic: Dictionary<FormQuestionModel> = {};
  public questionsSectionDic: Dictionary<FormQuestionModel[]> = {};
  public sectionHasBranchingDic: Dictionary<FormSection> = {};
  public get currentQuestion(): FormQuestionModel | undefined {
    return this.orderedQuestions[this.currentQuestionIndex];
  }
  public get currentQuestionAnswer(): FormQuestionAnswerModel | undefined {
    if (this.currentQuestion === undefined) {
      return undefined;
    }
    return this.questionIdToFormAnswerDic[this.currentQuestion.id];
  }
  public get formDataType(): FormType {
    return this.formData.form.type;
  }
  public get currentSectionId(): string {
    return this.currentQuestion.formSectionId;
  }
  public get formSectionList(): IFormSection[] {
    return this.formData.formSections;
  }
  public get currentSection(): FormSectionViewModel {
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
  public nextQuestions: (FormQuestionModel | FormSection)[] = [];

  @ViewChild('hintAnchor', { static: false }) private hintAnchorElementRef: ElementRef;

  private _formAnswer: FormAnswerModel;
  private _formData: FormWithQuestionsModel;
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

  public buildDictionary(formData: FormWithQuestionsModel): void {
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
  public createOrderedSectionsList(formQuestion: FormQuestionModel[]): FormSectionViewModel[] {
    const sectionsQuestion: FormSectionViewModel[] = [];

    formQuestion.forEach(question => {
      // If question doesn't exist any section, pushing a new section item into the section list with sectionId = null
      if (!question.formSectionId) {
        const sectionVm: IFormSectionViewModel = {
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
          const sectionVm: IFormSectionViewModel = {
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

    // apply braching for Survey only
    if (this.formData.form.type === FormType.Survey) {
      let nextQuestionId = this.currentQuestion.nextQuestionId;
      switch (this.currentQuestion.questionType) {
        case QuestionType.DropDown:
        case QuestionType.TrueFalse:
        case QuestionType.SingleChoice:
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
    }
    // End Braching question logic.

    // Find the question that index grather than current question and its answer is not answered
    const defaultNextQuestionIndex = this.orderedQuestions.findIndex(
      (p, i) =>
        i >= this.currentQuestionIndex &&
        (this.questionIdToFormAnswerDic[p.id] === undefined || this.questionIdToFormAnswerDic[p.id].submittedDate === undefined)
    );
    return defaultNextQuestionIndex;
  }

  public getOrderedFormQuestions(): FormQuestionModel[] {
    let formQuestionsData = this.formQuestionsData ? this.formQuestionsData : this.formData.formQuestions;

    if (this.formData.form.type === FormType.Quiz && this.formData.form.randomizedQuestions) {
      if (!this.randomizedQuestionsList) {
        this.randomizedQuestionsList = this.shuffleQuestionsList(formQuestionsData);
        formQuestionsData = this.randomizedQuestionsList;
      } else {
        formQuestionsData = this.randomizedQuestionsList;
      }
    } else if (this.formData.form.type === FormType.Survey) {
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
          (question.questionType === QuestionType.DropDown ||
            question.questionType === QuestionType.TrueFalse ||
            question.questionType === QuestionType.SingleChoice)
        ) {
          question.questionOptions.forEach(option => {
            if (option.nextQuestionId && this.firstQuestionSectionDic[option.nextQuestionId]) {
              option.nextQuestionId = this.firstQuestionSectionDic[option.nextQuestionId].id;
            }
          });
        }
      });
    }

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
        _[p.formQuestionId] = !Utils.isNullOrEmpty(p.answerValue) ? p.answerValue : '';
      });
    });
  }

  public exitPlayer(): void {
    this.exitEvent.emit();
  }

  public submitAnswer(): void {
    const currentQuestion = this.orderedQuestions[this.currentQuestionIndex];
    let answer = this.getCurrentQuestionAnswer();
    const currentAnswerAttachment = this.currentQuestionAnswer.formAnswerAttachments;

    if (typeof answer === 'string') {
      answer = answer.trim();
    }

    if (answer instanceof Array && currentQuestion.questionType === QuestionType.FillInTheBlanks) {
      answer = answer.map(value => value.toString().trim());
    }

    this.submitEvent.emit({
      formAnswerId: this.formAnswer.id,
      myCourseId: this.myCourseId,
      questionAnswers: [
        {
          formQuestionId: this.orderedQuestions[this.currentQuestionIndex].id,
          answerValue: answer,
          isSubmit: true,
          spentTimeInSeconds: undefined,
          formAnswerAttachments: currentAnswerAttachment
        }
      ],
      isSubmit: this.currentQuestionIndex === this.orderedQuestions.length - 1
    });
  }

  public getCurrentQuestionAnswer(): FormQuestionModelAnswerValue {
    const currentFormQuestionId = this.orderedQuestions[this.currentQuestionIndex].id;
    return this.questionIdToAnswerValueDic[currentFormQuestionId];
  }

  public isDisabledSubmitAnswer(): boolean {
    const currentAnswerValue = this.getCurrentQuestionAnswer();
    const currentQuestion = this.orderedQuestions[this.currentQuestionIndex];
    const currentAnswerAttachment = this.currentQuestionAnswer.formAnswerAttachments;

    switch (currentQuestion.questionType) {
      case QuestionType.FillInTheBlanks:
        let answerArray: FormQuestionModelAnswerValue[];
        let numberOfCurrentAnswer: number = 0;
        const numberOfCorrectAnswer = currentQuestion.getCorrectAnswerLenght();
        if (currentAnswerValue) {
          answerArray = currentAnswerValue as FormQuestionModelAnswerValue[];
          numberOfCurrentAnswer = answerArray.length;
        }
        return !currentAnswerValue || answerArray.includes('') || numberOfCurrentAnswer !== numberOfCorrectAnswer;

      case QuestionType.DateRangePicker:
        return !(currentAnswerValue instanceof Array) || (currentAnswerValue instanceof Array && currentAnswerValue.length < 2);

      case QuestionType.FreeResponse:
        return Utils.isNullOrEmpty(currentAnswerValue) && Utils.isNullOrEmpty(currentAnswerAttachment);

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
    configurationPopup.module = BrokenLinkModuleIdentifier.Form;
    configurationPopup.contentType = BrokenLinkContentType.Question;
    configurationPopup.objectDetailUrl = WebAppLinkBuilder.buildDigitalContentUrl(this._formData.form.id);
    configurationPopup.objectOwnerId = this._formData.form.ownerId;
    configurationPopup.objectOwnerName = this._formData.form.owner ? this._formData.form.owner.fullName : '';
    configurationPopup.objectTitle = this._formData.form.title;
    configurationPopup.isPreviewMode = this.isPreviewMode;
  }

  public canShowHintBtn(): boolean {
    return this.currentQuestion && this.currentQuestion.questionHint && this.formData.form.type === FormType.Quiz;
  }

  // Check can show poll results after finishing poll
  // Do not show poll results after finishing in standalone page
  public canShowPollResultAfterFinish(): boolean {
    return (
      (this.showingFeedbackPage || !this.showingFinishResult) &&
      this.formData.form.type === FormType.Poll &&
      !this.isStandalone &&
      (this.isDisplayPollResultToLearners || (this.isPreviewMode && this.formData.form.isAllowedDisplayPollResult))
    );
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

  public isPassedQuiz(): boolean {
    return (
      this.canShowFinishBtn() &&
      this.isPassingMarkEnabled &&
      this.formAnswer.isPassed(this.formData.form.passingMarkPercentage, this.formData.form.passingMarkScore)
    );
  }

  public isLearningWithoutScoringFinished(): boolean {
    return (
      (this.canShowFinishBtn() && !this.isPassingMarkEnabled) ||
      (this.canShowFinishBtn() &&
        this.isPassingMarkEnabled &&
        !this.formData.form.passingMarkPercentage &&
        !this.formData.form.passingMarkScore)
    );
  }

  public canShowSubmitBtn(): boolean {
    return this.currentQuestionAnswer && !this.currentQuestionAnswer.submittedDate && this.formAnswer.isAvailable();
  }

  public goToNextQuestion(): void {
    this.currentQuestionIndex = this.getNextQuestionIndex();
  }

  public finishTheFormAnswer(): void {
    if (this.canShowFeedbackByAfterCompletedQuiz()) {
      this.onShowReviewPage();
      return;
    }
    if (this.canShowFeedbackByAfterXAttempt()) {
      this.onShowReviewPage();
      return;
    }
    if (this.canShowFinishScoreResult()) {
      this.onShowFinishScore();
      return;
    }
    if (this.canShowPollResultAfterFinish()) {
      this.onShowPollResults();
      return;
    }
    this.isFinished = true;
    this.finishEvent.emit();
  }

  public canShowFeedbackByAfterCompletedQuiz(): boolean {
    return (
      !this.showingFinishResult &&
      this.formData.form.type === FormType.Quiz &&
      this.formData.form.answerFeedbackDisplayOption === AnswerFeedbackDisplayOption.AfterCompletedQuiz
    );
  }

  public canShowFeedbackByAfterXAttempt(): boolean {
    return (
      !this.showingFinishResult &&
      this.formData.form.type === FormType.Quiz &&
      this.formData.form.answerFeedbackDisplayOption === AnswerFeedbackDisplayOption.AfterXAtemps &&
      this.reachNumberOfAttemptToShowReviewPage
    );
  }

  public canShowFinishScoreResult(): boolean {
    return (
      (this.isFinished === null || this.isFinished) &&
      (this.showingFeedbackPage || !this.showingFinishResult) &&
      this.formData.form.type === FormType.Quiz
    );
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

  public currentQuestionAnswerChanged(e: FormQuestionAnswerModel): void {
    this.formAnswer = Utils.clone(this.formAnswer, formAnswer => {
      formAnswer.questionAnswers = Utils.addOrReplace(formAnswer.questionAnswers, e, p => p.formQuestionId === e.formQuestionId);
    });
    this.formAnswerChangeEvent.emit(this.formAnswer);
  }

  public getCorrectAnswers(): FormQuestionAnswerModel[] {
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
    if (this.formAnswer.scorePercentage == null || this.formAnswer.score == null) {
      return '0%';
    }

    if (isNaN(this.formAnswer.scorePercentage)) {
      return `N/A`;
    }

    return `${this.formAnswer.scorePercentage}% (${this.formAnswer.score}/${this.formMaxScore})`;
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

  private shuffleQuestionsList(formQuestion: FormQuestionModel[]): FormQuestionModel[] {
    // in case has section, all questions in section are counted as one to shuffle
    if (formQuestion.some(x => x.formSectionId)) {
      let questionsList: FormQuestionModel[] = [];
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

  public get questionDescription(): string {
    const questionNumber = this.currentQuestionIndex + 1;
    const questionTotal = this.orderedQuestions.length;
    const text = '##questionNumber## of ##total## ##questionTextGrammatically##';

    const result = text
      .replace('##questionNumber##', questionNumber.toString())
      .replace('##total##', questionTotal.toString())
      .replace('##questionTextGrammatically##', questionNumber > 1 ? 'questions' : 'question');

    return this.translate(result);
  }
}
