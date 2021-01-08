import {
  Assignment,
  AssignmentRepository,
  AssignmentType,
  ISaveAssignmentRequest,
  ISaveAssignmentRequestDataQuizForm,
  QuizAssignmentFormQuestion,
  SaveAssignmentRequestData
} from '@opal20/domain-api';
import { AssignmentDetailViewModel, AssignmentMode, PreviewMode } from '@opal20/domain-components';
import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, ElementRef, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { Observable, from } from 'rxjs';

import { AssignmentEditorComponent } from './assignment-editor.component';
import { requiredAndNoWhitespaceValidator } from '@opal20/common-components';

@Component({
  selector: 'assignment-form-editor',
  templateUrl: './assignment-form-editor.component.html'
})
export class AssignmentFormEditor extends BaseFormComponent {
  @ViewChild('assignmentEditor', { static: false }) public assignmentEditor: AssignmentEditorComponent;
  @ViewChild('assignmentEditorPageLeftCol', { static: false }) public assignmentEditorPageLeftCol: ElementRef;

  public AssignmentType: typeof AssignmentType = AssignmentType;
  public isExpandedOption: boolean = false;
  public AssignmentMode: typeof AssignmentMode = AssignmentMode;
  public isPreviewing: boolean = false;
  public previewingMode: PreviewMode = PreviewMode.Web;
  public previewDropdownItems: { text: string; value: PreviewMode }[] = [
    { text: 'Web', value: PreviewMode.Web },
    { text: 'Mobile', value: PreviewMode.Mobile }
  ];

  public _assignmentVm: AssignmentDetailViewModel = new AssignmentDetailViewModel();
  public get assignmentVm(): AssignmentDetailViewModel {
    return this._assignmentVm;
  }
  @Input()
  public set assignmentVm(v: AssignmentDetailViewModel) {
    if (Utils.isDifferent(this._assignmentVm, v)) {
      this._assignmentVm = Utils.cloneDeep(v);
    }
  }
  @Input() public mode: AssignmentMode = AssignmentMode.Edit;
  @Input() public onSaveFn?: (data: Assignment) => void;
  @Input() public onCancelFn?: () => void;
  @Input() public enablePreview: boolean = false;

  @Output() public onSave: EventEmitter<Assignment> = new EventEmitter<Assignment>();
  @Output() public assignmentVmChange: EventEmitter<AssignmentDetailViewModel> = new EventEmitter<AssignmentDetailViewModel>();
  @Output() public onCancel = new EventEmitter();

  public get selectedQuestionText(): string {
    return `${this.translate('Selected Question')} ${this.assignmentVm.selectedAssignmentQuestion.priority + 1}`;
  }

  constructor(protected moduleFacadeService: ModuleFacadeService, private assignmentRepository: AssignmentRepository) {
    super(moduleFacadeService);
  }

  public onSelectedQuestionChange(question: QuizAssignmentFormQuestion | undefined): void {
    if (this.assignmentVm.selectedAssignmentQuestion != null && this.assignmentVm.selectedAssignmentQuestion.id !== question.id) {
      this.isExpandedOption = true;
      if (this.assignmentEditorPageLeftCol) {
        this.assignmentEditorPageLeftCol.nativeElement.scroll({
          top: 0,
          behavior: 'smooth'
        });
      }
    }
  }

  public hasDataChanged(): boolean {
    return this.assignmentVm.hasDataChanged();
  }

  public onPanelBarItemClick(): void {
    this.isExpandedOption = false;
  }

  public onClickSave(): void {
    this.validateAndSaveAssignmentData().subscribe();
  }

  public onClickCancel(): void {
    if (this.onCancelFn != null) {
      this.onCancelFn();
    } else {
      this.assignmentVm.data = Utils.clone(this.assignmentVm.originalData);
      this.onCancel.emit();
    }
  }

  public saveAssignmentData(): Promise<Assignment> {
    const questionRequestData: ISaveAssignmentRequestDataQuizForm = {
      randomizedQuestions: this.assignmentVm.randomizedQuestions,
      questions: this.assignmentVm.questions
    };
    const assignmentRequest: ISaveAssignmentRequest = {
      data: new SaveAssignmentRequestData(this.assignmentVm.data, questionRequestData)
    };

    return this.assignmentRepository
      .saveAssignment(assignmentRequest)
      .toPromise()
      .then(_ => {
        this.assignmentVm = new AssignmentDetailViewModel(_, this.assignmentVm.assessments);
        this.showNotification();
        if (this.onSaveFn) {
          this.onSaveFn(_);
        } else {
          this.onSave.emit(_);
        }
        return _;
      });
  }

  public validateAndSaveAssignmentData(): Observable<Assignment> {
    return from(
      new Promise<Assignment>((resolve, reject) => {
        this.validate().then(val => {
          if (val) {
            this.saveAssignmentData().then(_ => {
              resolve(_);
            }, reject);
          } else {
            reject('validation error');
          }
        }, reject);
      })
    );
  }

  public emitAssignmentVmChanged(): void {
    this.assignmentVmChange.emit(this.assignmentVm);
  }

  public onPreviewClicked(previewMode: PreviewMode): void {
    this.previewingMode = previewMode;
    this.isPreviewing = true;
  }

  public closePreview(): void {
    this.isPreviewing = false;
  }

  protected additionalCanSaveCheck(controls?: string[]): Promise<boolean> {
    return this.assignmentEditor.validate(controls);
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'form',
      controls: {
        title: {
          defaultValue: this.assignmentVm.title,
          validators: [
            {
              validator: requiredAndNoWhitespaceValidator(),
              validatorType: 'required'
            }
          ]
        },
        assessmentId: {
          defaultValue: this.assignmentVm.assessmentId,
          validators: null
        },
        randomizedQuestions: {
          defaultValue: this.assignmentVm.randomizedQuestions,
          validators: null
        }
      }
    };
  }
}
