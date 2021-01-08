import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { QuestionOptionType, QuestionType, QuizAssignmentFormQuestion } from '@opal20/domain-api';

import { AssignmentMode } from '@opal20/domain-components';

@Component({
  selector: 'assignment-question-config',
  templateUrl: './assignment-question-config.component.html'
})
export class AssignmentQuestionConfigComponent extends BaseComponent {
  // The enums define here just use in template through variables.
  // In component class, we should directly call to these enums.
  public QuestionType: typeof QuestionType = QuestionType;
  public QuestionOptionType: typeof QuestionOptionType = QuestionOptionType;
  public AssignmentMode: typeof AssignmentMode = AssignmentMode;
  public _assignmentQuestion: QuizAssignmentFormQuestion = new QuizAssignmentFormQuestion();
  public get assignmentQuestion(): QuizAssignmentFormQuestion {
    return this._assignmentQuestion;
  }
  @Input() public set assignmentQuestion(assignmentQuestion: QuizAssignmentFormQuestion) {
    this._assignmentQuestion = assignmentQuestion;
  }
  @Input() public mode: AssignmentMode = AssignmentMode.Edit;
  @Output() public assignmentQuestionChange: EventEmitter<QuizAssignmentFormQuestion> = new EventEmitter<QuizAssignmentFormQuestion>();

  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public get isShowExplationNote(): boolean {
    return QuizAssignmentFormQuestion.questionTypeToShowExplationNote.includes(this.assignmentQuestion.question_Type);
  }

  public get isShowRandomizeOption(): boolean {
    return QuizAssignmentFormQuestion.questionTypeToShowRandomizeOption.includes(this.assignmentQuestion.question_Type);
  }
}
