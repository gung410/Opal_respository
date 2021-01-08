import { BaseFormComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output, QueryList, ViewChildren } from '@angular/core';
import { FormModel, FormQuestionModel, FormType, QuestionOption, QuestionType } from '@opal20/domain-api';

import { FormAssessmentRubricEditorComponent } from './form-assessment-rubric-editor.component';
import { FormDetailMode } from '@opal20/domain-components';

@Component({
  selector: 'form-assessment-rubric-management-page',
  templateUrl: './form-assessment-rubric-management-page.component.html'
})
export class FormAssessmentRubricManagementPage extends BaseFormComponent {
  @ViewChildren(FormAssessmentRubricEditorComponent) public assessmentIdicator: QueryList<FormAssessmentRubricEditorComponent>;

  //#region Input
  @Input() public type: FormType = FormType.Holistic;
  @Input() public mode: FormDetailMode = FormDetailMode.View;
  @Input() public formData: FormModel | undefined;
  @Input() public set assessmentData(v: FormQuestionModel[]) {
    if (!Utils.isDifferent(v, this._assessmentData)) {
      return;
    }

    this._assessmentData = v;
    this.assessmentDataChange.emit(this._assessmentData);
  }

  public get assessmentData(): FormQuestionModel[] {
    return this._assessmentData;
  }
  //#endregion

  //#region Output
  @Output() public formDataChange: EventEmitter<FormModel> = new EventEmitter<FormModel>();
  @Output() public assessmentDataChange: EventEmitter<FormQuestionModel[]> = new EventEmitter<FormQuestionModel[]>();
  //#endregion

  private _assessmentData: FormQuestionModel[];

  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public updateData(updatefn: (data: FormQuestionModel[]) => void): void {
    this.assessmentData = Utils.clone(this.assessmentData, p => {
      updatefn(p);
    });
  }

  public addNewHolistic(): void {
    this.updateData(currentData => {
      const newPriority = currentData.length;
      const newCcale = new FormQuestionModel().initBasicQuestionData(QuestionType.Scale, newPriority);

      currentData.push(newCcale);
    });
  }

  public addNewAnalytic(type: 'Scale' | 'Criteria'): void {
    this.updateData(currentData => {
      const newPriority: number = currentData.length;
      let newItem: FormQuestionModel = new FormQuestionModel();

      if (type === 'Criteria') {
        newItem = newItem.initBasicQuestionData(QuestionType.Criteria, newPriority);
        // Every time a new Criteria is added, System will automacally generates a set of Rubric value corresponding to number of current Scale
        currentData.forEach((data, index) => {
          if (data.questionType === QuestionType.Scale) {
            const newRubricValue = new QuestionOption(index, '');
            newRubricValue.scaleId = data.id;
            newItem.questionOptions.push(newRubricValue);
          }
        });
      }

      if (type === 'Scale') {
        newItem = newItem.initBasicQuestionData(QuestionType.Scale, newPriority);
        //  Every time a new Scale is added, System will automacally generates an empty rubric value for all current Criteria
        currentData.forEach(data => {
          if (data.questionType === QuestionType.Criteria) {
            const newRubricValue = new QuestionOption(data.questionOptions.length, '');
            newRubricValue.scaleId = newItem.id;
            data.questionOptions.push(newRubricValue);
          }
        });
      }

      currentData.push(newItem);
    });

    this.refreshRubricOptionPriority();
  }

  public onDeleteAssessmentData(id: string, isRemoveRubricValue: boolean = false): void {
    this.updateData(currentData => {
      const index = currentData.findIndex(question => question.id === id);
      currentData[index].isDeleted = true;
    });

    if (isRemoveRubricValue) {
      this.removeRubricOption(id);
      this.refreshRubricOptionPriority();
    }

    this.refreshAssessmentDataPriority();
  }

  public onMoveAssessmentDataClicked(id: string, direction: 'UP' | 'DOWN'): void {
    let moveToIndex: number;
    const moveFromIndex = this.assessmentData.findIndex(a => a.id === id);
    if (moveFromIndex < 0) {
      return;
    }

    const moveItem = this.assessmentData[moveFromIndex];
    let movingStep: number = 1;
    switch (direction) {
      case 'UP':
        for (let i = moveFromIndex - 1; i >= 0; i--) {
          if (this.assessmentData[i].questionType === moveItem.questionType) {
            break;
          }
          movingStep += 1;
        }
        moveToIndex = moveFromIndex - movingStep;
        break;
      case 'DOWN':
        for (let i = moveFromIndex + 1; i <= this.assessmentData.length; i++) {
          if (this.assessmentData[i].questionType === moveItem.questionType) {
            break;
          }
          movingStep += 1;
        }
        moveToIndex = moveFromIndex + movingStep;
        break;
    }

    this.moveAssessmentData(moveFromIndex, moveToIndex);
  }

  public onDragAndDropAssessmentData(event: { previousIndexId: string; currentIndexId: string }): void {
    const moveFromIndex = this.assessmentData.findIndex(data => data.id === event.previousIndexId);
    const moveToIndex = this.assessmentData.findIndex(data => data.id === event.currentIndexId);

    this.moveAssessmentData(moveFromIndex, moveToIndex);
  }

  public moveAssessmentData(fromIndex: number, toIndex: number): void {
    this.updateData(currentData => {
      const deleteCount: number = 0;
      const toMovingItemsCount: number = 1;

      const toMovingData = currentData.splice(fromIndex, toMovingItemsCount);
      currentData = currentData.splice(toIndex, deleteCount, ...toMovingData);
    });

    this.refreshAssessmentDataPriority();
  }

  public onAssessmentDataChange(newData: FormQuestionModel[]): void {
    this.updateData(currentData => {
      newData.forEach(data => {
        const index = currentData.findIndex(question => question.id === data.id);
        currentData[index] = data;
      });
    });
  }

  // Because API not support to remove data from array
  // It just mark the item with isDelete flag
  // So we need to re-update priority after soft-delete value
  public refreshAssessmentDataPriority(): void {
    let priority = 0;

    this.updateData(currentData => {
      currentData.forEach(data => {
        data.priority = priority;
        priority = data.isDeleted ? priority : priority + 1;
      });
    });
  }

  public removeRubricOption(scaleId: string): void {
    this.updateData(currentData => {
      currentData.forEach(data => {
        const rubricOptionIndex: number = data.questionOptions.findIndex(option => option.scaleId === scaleId);
        const numberOfItemToRemove: number = 1;
        data.questionOptions.splice(rubricOptionIndex, numberOfItemToRemove);
      });
    });
  }

  public refreshRubricOptionPriority(): void {
    this.updateData(currentData => {
      currentData.forEach(data => {
        data.questionOptions = data.questionOptions.map((value, index) => {
          value.code = index + 1;
          return value;
        });
      });
    });
  }

  //#region Getter
  public get assessmentTitle(): string {
    return `${this.type} Rubric`;
  }

  public get isViewMode(): boolean {
    return this.mode !== FormDetailMode.Edit;
  }

  public get editableAssessmentData(): FormQuestionModel[] {
    return Utils.orderBy(this.assessmentData.filter(data => !data.isDeleted), x => x.priority);
  }

  public get disabledAddAssessment(): boolean {
    const maxAssessmentDataLength = this.type === FormType.Holistic ? 100 : 200;
    return this.assessmentData.length >= maxAssessmentDataLength;
  }
  //#endregion

  protected additionalCanSaveCheck(): Promise<boolean> {
    return Promise.all([
      ...this.assessmentIdicator
        .toArray()
        .reverse()
        .map(p => p.validate())
    ]).then(finalResult => {
      return !finalResult.includes(false);
    });
  }
}
