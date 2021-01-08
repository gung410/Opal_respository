import { BaseFormComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output, SimpleChanges, ViewChild } from '@angular/core';
import { ContextMenuComponent, ContextMenuPopupEvent } from '@progress/kendo-angular-menu';

import { QuestionAnswerSingleValue } from '@opal20/domain-api';

@Component({
  selector: 'question-blank-option-editor',
  templateUrl: './question-blank-option-editor.component.html'
})
export class QuestionBlankOptionEditorComponent extends BaseFormComponent {
  @Input() public value: QuestionAnswerSingleValue = '';
  @Input() public target: unknown;

  @Output() public submit: EventEmitter<QuestionAnswerSingleValue> = new EventEmitter<QuestionAnswerSingleValue>();

  @ViewChild(ContextMenuComponent, { static: true }) protected newTextOptionContextMenuCmp: ContextMenuComponent;

  protected previousValue: QuestionAnswerSingleValue;
  protected isSubmitBtnClicked: boolean = false;
  private isAddNewComponent: boolean;

  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public onPopupClose(data: ContextMenuPopupEvent): void {
    const isEqual = this.previousValue === this.value;
    if (this.isSubmitBtnClicked === false && !isEqual) {
      this.value = this.previousValue;
    }
  }

  public onPopupOpen(data: ContextMenuPopupEvent): void {
    this.isSubmitBtnClicked = false;
  }

  public onSubmitClicked(): void {
    if (!this.isAddNewComponent) {
      this.previousValue = this.value;
    }
    this.submit.emit(this.value);
    this.isSubmitBtnClicked = true;
    this.value = '';
    this.newTextOptionContextMenuCmp.hide();
  }

  // We need to confirm with B.A about validation on each blank option.
  // The code block below is validating content of blank option
  // at the time we're adding it to "Fill in The Blank" question.
  // ====================================================================
  // ====================================================================
  // protected createFormBuilderDefinition(): IFormBuilderDefinition {
  //   const result: IFormBuilderDefinition = {
  //     formName: 'form',
  //     controls: {
  //       value: {
  //         defaultValue: this.value,
  //         validators: this.notRequired
  //           ? QuestionOption.ValueValidators().filter(p => p.validatorType !== 'required')
  //           : QuestionOption.ValueValidators()
  //       },
  //       checked: {
  //         defaultValue: this.checked
  //       }
  //     }
  //   };
  //   return result;
  // }

  protected onChanges(changes: SimpleChanges): void {
    if (changes.value && !changes.value.isFirstChange() && !this.isAddNewComponent) {
      this.previousValue = changes.value.currentValue;
    }
  }

  protected onInit(): void {
    this.previousValue = this.value;
    this.isAddNewComponent = !this.value;
  }
}
