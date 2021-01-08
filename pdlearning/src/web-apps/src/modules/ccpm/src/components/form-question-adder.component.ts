import { BaseComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormType, IQuestionBankSelection, QuestionType } from '@opal20/domain-api';

import { FormDetailMode } from '@opal20/domain-components';
import { FormEditModeService } from '../services/form-edit-mode.service';
import { QuestionTypeSelectionService } from '../services/question-type-selection.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'form-question-adder',
  templateUrl: './form-question-adder.component.html'
})
export class FormQuestionAdderComponent extends BaseComponent {
  @Input() public id: string = Utils.createUUID();
  @Input() public inlineAdd: boolean = false;
  @Input() public type: FormType = FormType.Quiz;
  @Input() public priority: number;
  @Input() public minorPriority: number | undefined;

  @Output('selectQuestion') public selectQuestionEvent: EventEmitter<{
    type: QuestionType;
    priority: number;
    minorPriority: number;
  }> = new EventEmitter();

  @Output('importQuestions') public importQuestionsEvent: EventEmitter<IQuestionBankSelection> = new EventEmitter();
  public mode: FormDetailMode = this.formEditModeService.initMode;
  public FormDetailMode: typeof FormDetailMode = FormDetailMode;
  private questionSelectionSubscription: Subscription;
  private importQuestionsSubscription: Subscription;

  constructor(
    moduleFacadeService: ModuleFacadeService,
    public formEditModeService: FormEditModeService,
    private questionTypeSelectionService: QuestionTypeSelectionService
  ) {
    super(moduleFacadeService);
  }

  protected onInit(): void {
    this.subscribe(this.formEditModeService.modeChanged, mode => {
      this.mode = mode;
    });
    this.questionSelectionSubscription = this.subscribeNewQuestionCreated();
    this.importQuestionsSubscription = this.subscribeQuestionsImported();
  }

  protected onDestroy(): void {
    if (this.questionSelectionSubscription) {
      this.questionSelectionSubscription.unsubscribe();
    }
    if (this.importQuestionsSubscription) {
      this.importQuestionsSubscription.unsubscribe();
    }
  }

  private subscribeNewQuestionCreated(): Subscription {
    return this.questionTypeSelectionService.getNewQuestionType$.subscribe(
      (data: { type: QuestionType; priority: number; minorPriority: number }) => {
        if (data && data.priority === this.priority && data.minorPriority === this.minorPriority) {
          this.selectQuestionEvent.emit(data);
        }
      }
    );
  }

  private subscribeQuestionsImported(): Subscription {
    return this.questionTypeSelectionService.getQuestionListImport$.subscribe((data: IQuestionBankSelection) => {
      if (
        data &&
        data.listQuestion &&
        data.listQuestion.length &&
        data.priority === this.priority &&
        data.minorPriority === this.minorPriority
      ) {
        this.importQuestionsEvent.emit(data);
      }
    });
  }
}
