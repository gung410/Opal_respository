import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ContextMenuItem, requiredIfValidator } from '@opal20/common-components';

import { Align } from '@progress/kendo-angular-popup';
import { AssessmentContextMenuAction } from '../models/assessment-context-menu-action';
import { FormQuestionModel } from '@opal20/domain-api';
import { MenuEvent } from '@progress/kendo-angular-menu';
import { Validators } from '@angular/forms';

@Component({
  selector: 'form-assessment-indicator',
  templateUrl: './form-assessment-indicator.component.html'
})
export class FormAssessmentIndicatorComponent extends BaseFormComponent {
  //#region Input
  @Input() public readOnly: boolean = false;
  @Input() public data: FormQuestionModel | undefined;
  @Input() public canShowPercentage: boolean = true;
  @Input() public canShowDescription: boolean = true;
  @Input() public canShowTitle: boolean = true;
  @Input() public currentPercentage: number[] = [];
  @Input() public contextMenuItems: ContextMenuItem[] = [];
  @Input() public contextMenuAnchorAlign: Align = { horizontal: 'center', vertical: 'bottom' };
  @Input() public contextMenuPopupAlign: Align = { horizontal: 'center', vertical: 'top' };

  @Input() public canDelete: boolean = true;
  @Input() public canMoveUp: boolean = true;
  @Input() public canMoveDown: boolean = true;
  //#endregion

  //#region Output
  @Output('onAssessmentDataChange') public dataChangeEvent: EventEmitter<FormQuestionModel> = new EventEmitter<FormQuestionModel>();
  @Output() public onMenuItemSelect: EventEmitter<string> = new EventEmitter<string>();
  //#endregion

  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public updateData(updatefn: (data: FormQuestionModel) => void): void {
    this.data = Utils.clone(this.data, p => {
      updatefn(p);
    });

    this.dataChangeEvent.emit(this.data);
  }

  public onTitleChange(value: string): void {
    this.updateData(currentData => {
      currentData.questionTitle = value;
    });
  }

  public onDescriptionChange(value: string): void {
    this.updateData(currentData => {
      currentData.description = value;
    });
  }

  public onScaleValueChange(value: number): void {
    this.updateData(currentData => {
      currentData.score = value;
    });
  }

  public onMenuItemClick(event: MenuEvent): void {
    this.onMenuItemSelect.emit(event.item.id);
  }

  public get validContextMenuItems(): ContextMenuItem[] {
    return this.contextMenuItems.filter(
      data =>
        (data.id === AssessmentContextMenuAction.MoveUp && this.canMoveUp) ||
        (data.id === AssessmentContextMenuAction.MoveDown && this.canMoveDown) ||
        (data.id === AssessmentContextMenuAction.Delete && this.canDelete)
    );
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'form',
      controls: {
        title: {
          defaultValue: null,
          validators: [
            { validator: Validators.required, validatorType: 'required' },
            { validator: Validators.maxLength(255), validatorType: 'maxlength' }
          ]
        },
        scaleValue: {
          defaultValue: null,
          validators: [
            {
              validator: requiredIfValidator(() => this.canShowPercentage),
              validatorType: 'required'
            }
          ]
        },
        description: {
          defaultValue: null,
          validators: [{ validator: Validators.maxLength(255), validatorType: 'maxlength' }]
        }
      }
    };
  }
}
