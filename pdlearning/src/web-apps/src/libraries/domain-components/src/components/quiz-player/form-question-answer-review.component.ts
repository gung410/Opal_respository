import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import { FormAnswerModel, FormQuestionModel, FormSectionViewModel, FormType } from '@opal20/domain-api';

import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'form-question-answer-review',
  templateUrl: './form-question-answer-review.component.html'
})
export class FormQuestionAnswerReviewComponent extends BaseComponent {
  @Input() public formQuestion: FormQuestionModel[];
  @Input() public formAnswer: FormAnswerModel;
  @Input() public formDataType: FormType;
  @Input() public sectionsQuestion: FormSectionViewModel[];

  public readonly FORMTYPE = FormType;

  constructor(private sanitizer: DomSanitizer, moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.sectionsQuestion.forEach(section => {
      section.questions.forEach(question => {
        question.questionTitleSafeHtml = this.sanitizer.bypassSecurityTrustHtml(question.questionTitle);
      });
    });
  }

  public get isQuiz(): boolean {
    return this.formDataType === this.FORMTYPE.Quiz;
  }

  public get correctAnswer(): number {
    let countCorrectAnswer: number;

    if (this.formAnswer.questionAnswers.length) {
      countCorrectAnswer = 0;
      this.formAnswer.questionAnswers.forEach(answer => {
        if (answer.score > 0) {
          countCorrectAnswer++;
        }
      });
    }

    return countCorrectAnswer;
  }

  public get incorrectAnswer(): number {
    let incountCorrectAnswer: number;

    if (this.formAnswer.questionAnswers && this.formAnswer.questionAnswers.length) {
      incountCorrectAnswer = this.formAnswer.questionAnswers.length - this.correctAnswer;
    }

    return incountCorrectAnswer;
  }
}
