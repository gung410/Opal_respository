import { AssignmentType, QuizAssignmentQuestionType } from '@opal20/domain-api';
import { BaseComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output } from '@angular/core';

import { AssignmentMode } from '@opal20/domain-components';
import { AssignmentQuestionTypeSelectionService } from '../services/assignment-question-type-selection.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'assignment-question-adder',
  templateUrl: './assignment-question-adder.component.html'
})
export class AssignmentQuestionAdderComponent extends BaseComponent {
  @Input() public id: string = Utils.createUUID();
  @Input() public inlineAdd: boolean = false;
  @Input() public type: AssignmentType = AssignmentType.Quiz;
  @Input() public priority: number;
  @Input() public mode: AssignmentMode = AssignmentMode.Edit;
  @Output('selectQuestion') public selectQuestionEvent: EventEmitter<{
    type: QuizAssignmentQuestionType;
    priority: number;
  }> = new EventEmitter();

  public AssignmentMode: typeof AssignmentMode = AssignmentMode;
  private questionSelectionSubscription: Subscription;

  constructor(
    moduleFacadeService: ModuleFacadeService,
    private assignmentQuestionTypeSelectionService: AssignmentQuestionTypeSelectionService
  ) {
    super(moduleFacadeService);
  }

  protected onInit(): void {
    this.questionSelectionSubscription = this.subscribeNewQuestionCreated();
  }

  protected onDestroy(): void {
    if (this.questionSelectionSubscription) {
      this.questionSelectionSubscription.unsubscribe();
    }
  }

  private subscribeNewQuestionCreated(): Subscription {
    return this.assignmentQuestionTypeSelectionService.getNewQuestionType$.subscribe(
      (data: { type: QuizAssignmentQuestionType; priority: number }) => {
        if (data && data.priority === this.priority) {
          this.selectQuestionEvent.emit(data);
        }
      }
    );
  }
}
