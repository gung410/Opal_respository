import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output, ViewEncapsulation } from '@angular/core';
import { FormQuestionModel, FormSection, FormType } from '@opal20/domain-api';

import { FormDetailMode } from '@opal20/domain-components';
import { FormEditModeService } from '../services/form-edit-mode.service';
import { Validators } from '@angular/forms';

@Component({
  selector: 'form-section-editor',
  templateUrl: './form-section-editor.component.html',
  encapsulation: ViewEncapsulation.None
})
export class FormSectionEditorComponent extends BaseFormComponent {
  public readonly formTypeEnum: typeof FormType = FormType;
  public mode: FormDetailMode = this.formEditModeService.initMode;
  @Input() public formType: FormType;
  @Input() public formSection: FormSection;
  @Input() public branchingOptionQuestions: FormQuestionModel[];
  @Input() public disableMoveUp: boolean = false;
  @Output() public formSectionChange: EventEmitter<FormSection> = new EventEmitter<FormSection>();
  @Output('move')
  public moveEvent: EventEmitter<{ id: string; step: number }> = new EventEmitter();
  @Output('delete')
  public deleteEvent: EventEmitter<string> = new EventEmitter();
  constructor(protected moduleFacadeService: ModuleFacadeService, private formEditModeService: FormEditModeService) {
    super(moduleFacadeService);
  }

  public get isDisableMoveUp(): boolean {
    return this.formSection.priority === 0 || this.disableMoveUp;
  }

  public get isDisableMoveDown(): boolean {
    return this.formSection.priority === this.branchingOptionQuestions.length - 1 || this.formType === this.formTypeEnum.Poll;
  }

  public onContextMenuItemSelect(eventData: { id: string }): void {
    switch (eventData.id) {
      case 'Delete':
        this.delete();
        break;
      case 'MoveUp':
        this.moveUp();
        break;
      case 'MoveDown':
        this.moveDown();
        break;
    }
  }

  public canEditQuestion(): boolean {
    return this.mode !== FormDetailMode.View;
  }

  public updateformSectionData(updatefn: (formSection: FormSection) => void): void {
    this.formSection = Utils.clone(this.formSection, p => {
      updatefn(p);
    });
    this.formSectionChange.emit(this.formSection);
  }

  public delete(): void {
    this.deleteEvent.emit(this.formSection.id);
  }
  public moveUp(): void {
    this.moveEvent.emit({ id: this.formSection.id, step: -1 });
  }
  public moveDown(): void {
    this.moveEvent.emit({ id: this.formSection.id, step: 1 });
  }

  public onSectionTitleChanged(title: string): void {
    this.updateformSectionData(section => {
      section.mainDescription = title;
    });
  }

  public onSectionDescriptionChanged(description: string): void {
    this.updateformSectionData(section => {
      section.additionalDescription = description;
    });
  }

  public onChooseNextQuestion(nextQuestionId: string): void {
    this.updateformSectionData(section => (section.nextQuestionId = nextQuestionId));
  }

  public getBranchingOptions(): IDataItem[] {
    const branchingOptions = this.branchingOptionQuestions
      .filter(question => question.id !== this.formSection.id)
      .map(question => {
        return <IDataItem>{
          text: `${question.priority + 1}`,
          value: question.id
        };
      });
    return [{ text: this.translate('Continue to next question'), value: null }].concat(branchingOptions);
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    const result: IFormBuilderDefinition = {
      formName: 'form',
      controls: {
        title: {
          defaultValue: this.formSection.mainDescription,
          validators: [
            { validator: Validators.required, validatorType: 'required' },
            { validator: Validators.maxLength(8000), validatorType: 'maxlength' }
          ]
        },
        description: {
          defaultValue: this.formSection.additionalDescription
        },
        nextQuestionId: {
          defaultValue: this.formSection.nextQuestionId
        }
      }
    };
    return result;
  }

  protected onInit(): void {
    this.subscribe(this.formEditModeService.modeChanged, mode => {
      this.mode = mode;
    });
  }
}
