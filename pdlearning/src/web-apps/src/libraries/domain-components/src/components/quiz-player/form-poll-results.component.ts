import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import {
  FormAnswerModel,
  FormQuestionAnswerModel,
  FormQuestionAnswerService,
  FormQuestionAnswerStatisticsModel,
  FormQuestionModel,
  FormQuestionModelSingleAnswerValue,
  FormWithQuestionsModel,
  QuestionOption,
  QuestionType
} from '@opal20/domain-api';

import { MainQuizPlayerPageService } from '../../services/main-quiz-player-page.service';

@Component({
  selector: 'form-poll-results',
  templateUrl: './form-poll-results.component.html'
})
export class FormPollResultsComponent extends BaseComponent {
  @Input() public questions: FormQuestionModel[];
  @Input() public formAnswersData: FormAnswerModel;
  @Input() public formData: FormWithQuestionsModel;

  public safeQuestionTitle: SafeHtml;
  public questionType = QuestionType;
  public firstQuestionOptions: QuestionOption[];
  public firstQuestionAnswer: FormQuestionAnswerModel;
  public userMultipleChoiceAnswered: FormQuestionModelSingleAnswerValue[];
  public pollStatisticsData: FormQuestionAnswerStatisticsModel[];
  public totalVotes: number = 0;
  public questionNumber: number = 1;

  constructor(
    private mainQuizPlayerService: MainQuizPlayerPageService,
    public moduleFacadeService: ModuleFacadeService,
    private formQuestionAnswerService: FormQuestionAnswerService,
    private sanitizer: DomSanitizer
  ) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.initData();
    this.processQuestionTitle();
    this.setUserMultipleChoiceAnswered();
    this.getQuestionStatistics();
  }

  public getQuestionStatistics(): void {
    if (this.formData.form.isAllowedDisplayPollResult) {
      this.formQuestionAnswerService.getQuestionStatistics(this.questions[0].id).subscribe(data => {
        this.pollStatisticsData = data;
        this.renderStatisticsData();
      });
    }
  }

  public getVotes(vote: number): string {
    return this.translate(vote > 1 ? `${vote} votes` : `${vote} vote`);
  }

  public getMediaUrl(url: string): string {
    return url ? `${AppGlobal.environment.cloudfrontUrl}/${url}` : '';
  }

  public isSelectedValue(questionValue: string): boolean {
    return this.userMultipleChoiceAnswered && this.userMultipleChoiceAnswered.findIndex(answer => answer === questionValue) >= 0;
  }

  private initData(): void {
    if (!this.questions) {
      this.questions = this.formData.formQuestions;
    }
    if (this.formAnswersData.questionAnswers.length > 1) {
      this.formAnswersData.questionAnswers.sort((a, b) => new Date(a.submittedDate).getTime() - new Date(b.submittedDate).getTime());
    }
    this.firstQuestionAnswer = this.formAnswersData.questionAnswers[0];
    this.firstQuestionOptions = this.questions[0].questionOptions;
  }

  private setUserMultipleChoiceAnswered(): void {
    if (this.questions[0].questionType === QuestionType.MultipleChoice) {
      this.userMultipleChoiceAnswered = <FormQuestionModelSingleAnswerValue[]>this.firstQuestionAnswer.answerValue;
    }
  }

  private renderStatisticsData(): void {
    this.firstQuestionOptions.forEach(item => {
      const mapItem = this.pollStatisticsData.filter(x => x.answerCode === item.code);
      this.totalVotes += mapItem[0] ? mapItem[0].answerCount : 0;
      item.answerCount = mapItem[0] ? mapItem[0].answerCount : 0;
      item.answerPercentage = mapItem[0] ? mapItem[0].answerPercentage : 0;
    });
  }

  private processQuestionTitle(): void {
    this.mainQuizPlayerService.applyToPreparedPopulate(this.questions[0].questionTitle).then(questionTitle =>
      this.mainQuizPlayerService
        .applyPopulatedFields(questionTitle)
        .then(newQuestionTitle => this.mainQuizPlayerService.applyToDisabledPopuplatedFields(newQuestionTitle))
        .then(newTitle => {
          this.safeQuestionTitle = this.sanitizer.bypassSecurityTrustHtml(newTitle);
        })
    );
  }
}
