import { AfterViewInit, Component, ElementRef, EventEmitter, Input, Output, QueryList, ViewChildren } from '@angular/core';
import { AssignmentDetailViewModel, AssignmentMode, AssignmentQuestionEditorComponent } from '@opal20/domain-components';
import { AssignmentType, QuizAssignmentFormQuestion, QuizAssignmentQuestionType } from '@opal20/domain-api';
import { BaseFormComponent, DomUtils, ModuleFacadeService, Utils } from '@opal20/infrastructure';

@Component({
  selector: 'assignment-editor',
  templateUrl: './assignment-editor.component.html'
})
export class AssignmentEditorComponent extends BaseFormComponent implements AfterViewInit {
  public AssignmentType: typeof AssignmentType = AssignmentType;
  public isShowExplanationNote: boolean = false;
  public AssignmentMode: typeof AssignmentMode = AssignmentMode;

  @ViewChildren(AssignmentQuestionEditorComponent) public questionEditors: QueryList<AssignmentQuestionEditorComponent>;

  @Input() public assignmentVm: AssignmentDetailViewModel = new AssignmentDetailViewModel();
  @Input() public mode: AssignmentMode = AssignmentMode.Edit;

  @Output() public assignmentVmChange: EventEmitter<AssignmentDetailViewModel> = new EventEmitter<AssignmentDetailViewModel>();
  @Output() public selectedQuestionChange: EventEmitter<QuizAssignmentFormQuestion | undefined> = new EventEmitter<
    QuizAssignmentFormQuestion | undefined
  >();

  public get disableSetCorrectAnswer(): boolean {
    return this.assignmentVm.type !== AssignmentType.Quiz || this.mode === AssignmentMode.View;
  }

  constructor(protected moduleFacadeService: ModuleFacadeService, private elementRef: ElementRef) {
    super(moduleFacadeService);
  }

  public isQuestionSelected(question: QuizAssignmentFormQuestion): boolean {
    return this.assignmentVm.selectedAssignmentQuestion && this.assignmentVm.selectedAssignmentQuestion.id === question.id;
  }

  public onQuestionChanged(newData: QuizAssignmentFormQuestion): void {
    this.assignmentVm.updateQuestion(p => {
      const questionIndex = p.findIndex(x => x.id === newData.id);
      if (questionIndex > -1) {
        p[questionIndex] = newData;
      }
    });
    this.assignmentVmChange.emit(this.assignmentVm);
  }

  public assignmentQuestionsDataTrackByFn(index: number, item: QuizAssignmentFormQuestion): string | QuizAssignmentFormQuestion {
    return item.id;
  }

  public onQuestionTemplateSelect(event: { type: QuizAssignmentQuestionType; priority: number }): void {
    this.assignmentVm.insertQuestion(event.type, event.priority);
    this.assignmentVmChange.emit(this.assignmentVm);
  }

  public onQuestionDelete(id: string): void {
    this.assignmentVm.deleteQuestion(id);
    this.selectedQuestionChange.emit(this.assignmentVm.selectedAssignmentQuestion);
  }

  public onQuestionMoveUp(id: string): void {
    this.onMoveQuestion(id, -1);
  }

  public onQuestionMoveDown(id: string): void {
    this.onMoveQuestion(id, 1);
  }

  public onSelectQuestion(question: QuizAssignmentFormQuestion): void {
    this.assignmentVm.selectedAssignmentQuestion = question;
    this.selectedQuestionChange.emit(this.assignmentVm.selectedAssignmentQuestion);
  }

  public onMoveQuestion(id: string, step: number): void {
    this.assignmentVm.moveQuestion(id, step);
    if (this.assignmentVm.selectedAssignmentQuestion != null && this.assignmentVm.selectedAssignmentQuestion.id === id) {
      Utils.delay(() => this.scrollToSelectedQuestion());
    }
  }

  public getSelectedQuestionContainer(): HTMLElement | Element | undefined {
    if (this.viewInitiated) {
      return DomUtils.findFirstElement(<HTMLElement>this.elementRef.nativeElement, p =>
        p.matches('.assignment-editor__question-container.-selected')
      );
    }
  }

  public scrollToSelectedQuestion(): void {
    const selectedQuestionContainer = this.getSelectedQuestionContainer();
    if (selectedQuestionContainer != null) {
      DomUtils.scrollToView(selectedQuestionContainer);
    }
  }

  public showExplanationNote(): void {
    this.isShowExplanationNote = !this.isShowExplanationNote;
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
}
