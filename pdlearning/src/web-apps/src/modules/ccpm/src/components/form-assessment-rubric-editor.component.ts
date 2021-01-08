import { BaseFormComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output, QueryList, ViewChildren } from '@angular/core';
import { FormQuestionModel, QuestionType } from '@opal20/domain-api';

import { AssessmentContextMenuAction } from '../models/assessment-context-menu-action';
import { CdkDragDrop } from '@angular/cdk/drag-drop';
import { ContextMenuItem } from '@opal20/common-components';
import { FormAssessmentIndicatorComponent } from './form-assessment-indicator.component';
export interface IRubricValidationsError {
  id: string;
  validationErrorMessage: string;
}

@Component({
  selector: 'form-assessment-rubric-editor',
  templateUrl: './form-assessment-rubric-editor.component.html'
})
export class FormAssessmentRubricEditorComponent extends BaseFormComponent {
  @ViewChildren(FormAssessmentIndicatorComponent) public asessmentIndicator: QueryList<FormAssessmentIndicatorComponent>;

  //#region Input
  @Input() public mode: 'Edit' | 'View' = 'Edit';
  @Input() public type: 'Scale' | 'Criteria' | 'Rubric' = 'Scale';
  @Input() public readOnly: boolean = false;
  @Input() public canShowTitle: boolean = true;
  @Input() public canShowPercentage: boolean = true;
  @Input() public canShowDescription: boolean = true;
  @Input() public set assessmentData(v: FormQuestionModel[]) {
    if (!Utils.isDifferent(v, this._assessmentData || !v)) {
      return;
    }

    this._assessmentData = v;
    this.editableData = this.getEditableData();
  }

  public get assessmentData(): FormQuestionModel[] {
    return this._assessmentData;
  }
  //#endregion

  //#region Output
  @Output('dataChange') public dataChangeEvent: EventEmitter<FormQuestionModel[]> = new EventEmitter<FormQuestionModel[]>();
  @Output('onDelete') public deleteEvent: EventEmitter<string> = new EventEmitter<string>();
  @Output('onMoveUp') public moveUpEvent: EventEmitter<string> = new EventEmitter<string>();
  @Output('onMoveDown') public moveDownEvent: EventEmitter<string> = new EventEmitter<string>();
  @Output('onDragAndDrop')
  public dragAndDropEvent: EventEmitter<{ previousIndexId: string; currentIndexId: string }> = new EventEmitter<{
    previousIndexId: string;
    currentIndexId: string;
  }>();
  //#endregion

  public rubricTitleValidationErrors: IRubricValidationsError[] = [];
  public rubricValueValidationErrors: IRubricValidationsError[] = [];

  public editableData: FormQuestionModel[] | undefined;
  public contextMenuItems: ContextMenuItem[] = [
    {
      id: AssessmentContextMenuAction.MoveUp,
      text: this.translateCommon('Move Up'),
      icon: 'sort-asc-sm'
    },
    {
      id: AssessmentContextMenuAction.MoveDown,
      text: this.translateCommon('Move Down'),
      icon: 'sort-desc-sm'
    },
    {
      id: AssessmentContextMenuAction.Delete,
      text: this.translateCommon('Delete'),
      icon: 'delete'
    }
  ];

  private _assessmentData: FormQuestionModel[] | undefined;

  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public getEditableData(): FormQuestionModel[] {
    switch (this.type) {
      case 'Scale':
        return this.assessmentData.filter(q => q.questionType === QuestionType.Scale);
      case 'Criteria':
        return this.assessmentData.filter(q => q.questionType === QuestionType.Criteria);
      default:
        return this.assessmentData;
    }
  }

  public onMenuItemSelect(key: string, id: string): void {
    switch (key) {
      case AssessmentContextMenuAction.Delete:
        this.deleteEvent.emit(id);
        break;
      case AssessmentContextMenuAction.MoveUp:
        this.moveUpEvent.emit(id);
        break;
      case AssessmentContextMenuAction.MoveDown:
        this.moveDownEvent.emit(id);
        break;
    }
  }

  public assessmentRubricTrackByFn(index: number, item: FormQuestionModel): string | FormQuestionModel {
    return item.id;
  }

  public updateData(updatefn: (data: FormQuestionModel[]) => void): void {
    this.assessmentData = Utils.clone(this.assessmentData, p => {
      updatefn(p);
    });

    this.dataChangeEvent.emit(this.assessmentData);
  }

  public updateAssessmentData(newData: FormQuestionModel): void {
    this.updateData(currentData => {
      const questionIndex = currentData.findIndex(question => question.id === newData.id);
      if (questionIndex > -1) {
        currentData[questionIndex] = newData;
      }
    });

    this._validateAdditionalErrorLogic();
  }

  public onAssessmentDataDropped(event: CdkDragDrop<FormQuestionModel>): void {
    const previousIndexId = this.editableData[event.previousIndex].id;
    const currentIndexId = this.editableData[event.currentIndex].id;

    this.dragAndDropEvent.emit({ previousIndexId: previousIndexId, currentIndexId: currentIndexId });
  }

  public onRubricValueChange(value: string, scaleId: string, criteriaId: string): void {
    this.updateData(currentData => {
      currentData.forEach(data => {
        if (data.id === criteriaId) {
          data.questionOptions.forEach(option => {
            if (option.scaleId === scaleId) {
              option.value = value;
            }
          });
        }
      });
    });
  }

  //#region Getter
  public getRubricValue(scaleId: string, criteriaId: string): string {
    const a = this.editableData.find(data => data.id === criteriaId);

    return a.questionOptions.find(x => x.scaleId === scaleId).value.toString();
  }

  public getTitleError(id: string): string {
    const errIndex = this.rubricTitleValidationErrors.findIndex(err => err.id === id);
    if (errIndex < 0) {
      return;
    }

    return this.rubricTitleValidationErrors[errIndex].validationErrorMessage;
  }

  public getScaleError(id: string): string {
    const errIndex = this.rubricValueValidationErrors.findIndex(err => err.id === id);
    if (errIndex < 0) {
      return;
    }

    return this.rubricValueValidationErrors[errIndex].validationErrorMessage;
  }

  public canDeleteAssessment(): boolean {
    const minItemUnit: number = 1;
    return this.editableData.length > minItemUnit;
  }

  public canMoveUpAssessment(id: string): boolean {
    const firstItemIndex: number = 0;
    return this._findAssesmentIndex(id) > firstItemIndex;
  }

  public canMoveDownAssessment(id: string): boolean {
    const minItemUnit: number = 1;
    const lastItemIndex: number = this.editableData.length - minItemUnit;
    return this._findAssesmentIndex(id) < lastItemIndex && this.editableData.length > minItemUnit;
  }

  public get isValueModeEditor(): boolean {
    return this.type === 'Rubric';
  }

  public get scaleData(): FormQuestionModel[] {
    return this.editableData.filter(q => q.questionType === QuestionType.Scale);
  }

  public get criteriaData(): FormQuestionModel[] {
    return this.editableData.filter(q => q.questionType === QuestionType.Criteria);
  }

  public get currentPercentage(): number[] {
    return this.editableData.map(d => d.score);
  }
  //#endregion

  protected additionalCanSaveCheck(): Promise<boolean> {
    return Promise.all([
      ...this.asessmentIndicator
        .toArray()
        .reverse()
        .map(p => p.validate())
    ]).then(finalResult => {
      return !finalResult.includes(false) && this._validateAdditionalErrorLogic();
    });
  }

  private _findAssesmentIndex(id: string): number {
    return this.editableData.findIndex(data => data.id === id);
  }

  private _validateAdditionalErrorLogic(): boolean {
    this.rubricTitleValidationErrors = [];
    this.rubricValueValidationErrors = [];

    this.editableData.forEach(data => {
      if (data.questionTitle && this._hasDuplicateTitle(data.questionTitle)) {
        const errorMessage = 'The Title is already in used.';
        this.rubricTitleValidationErrors.push({ id: data.id, validationErrorMessage: errorMessage });
      }

      if (!Utils.isNullOrUndefined(data.score) && this._hasDuplicateValue(data.score)) {
        const errorMessage = 'The Scale is already in used.';
        this.rubricValueValidationErrors.push({ id: data.id, validationErrorMessage: errorMessage });
      }
    });

    return Utils.isNullOrEmpty(this.rubricTitleValidationErrors) && Utils.isNullOrEmpty(this.rubricValueValidationErrors);
  }

  private _hasDuplicateTitle(title: string): boolean {
    return this.editableData.filter(data => data.questionTitle.trim() === title.trim()).length >= 2;
  }

  private _hasDuplicateValue(value: number): boolean {
    return this.editableData.filter(data => data.score === value).length >= 2;
  }
}
