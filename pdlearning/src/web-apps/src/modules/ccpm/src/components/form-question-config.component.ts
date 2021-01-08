import { BaseComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormQuestionModel, QuestionOptionType, QuestionType } from '@opal20/domain-api';

import { FormDetailMode } from '@opal20/domain-components';
import { FormEditModeService } from '../services/form-edit-mode.service';

@Component({
  selector: 'form-question-config',
  templateUrl: './form-question-config.component.html'
})
export class FormQuestionConfigComponent extends BaseComponent {
  // The enums define here just use in template through variables.
  // In component class, we should directly call to these enums.
  public readonly questionTypeEnum: typeof QuestionType = QuestionType;
  public readonly questionOptionTypeEnum: typeof QuestionOptionType = QuestionOptionType;
  public FormDetailMode: typeof FormDetailMode = FormDetailMode;
  public _formQuestion: FormQuestionModel = new FormQuestionModel();
  public get formQuestion(): FormQuestionModel {
    return this._formQuestion;
  }
  @Input() public canDisableScore: boolean = false;
  @Input() public set formQuestion(formQuestion: FormQuestionModel) {
    this._formQuestion = formQuestion;
  }

  @Output() public formQuestionChange: EventEmitter<FormQuestionModel> = new EventEmitter<FormQuestionModel>();
  public mode: FormDetailMode = this.formEditModeService.initMode;

  constructor(protected moduleFacadeService: ModuleFacadeService, private formEditModeService: FormEditModeService) {
    super(moduleFacadeService);
  }

  public onRandomizedOptionsChange(event: boolean): void {
    this.formQuestion = Utils.clone(this.formQuestion, p => {
      p.randomizedOptions = event;
    });
    this.formQuestionChange.emit(this.formQuestion);
  }

  public onScoreChange(event: number): void {
    const score = event !== null ? Number(event) : 0;
    this.formQuestion = Utils.clone(this.formQuestion, p => {
      p.score = score;
    });
    this.formQuestionChange.emit(this.formQuestion);
  }

  public onAnswerExplanatoryNoteChange(event: string | undefined): void {
    this.formQuestion = Utils.clone(this.formQuestion, p => {
      p.answerExplanatoryNote = event;
    });
    this.formQuestionChange.emit(this.formQuestion);
  }

  public onQuestionHintChange(event: string | undefined): void {
    this.formQuestion = Utils.clone(this.formQuestion, p => {
      p.questionHint = event;
    });
    this.formQuestionChange.emit(this.formQuestion);
  }

  public onToggleEnableScore(value: boolean): void {
    if (!value && this.canDisableScore) {
      this.modalService.showErrorMessage('Please set at least 1 question with a mark.');
      setTimeout(() => {
        this.formQuestion.isScoreEnabled = true;
      }, 1);
      return;
    }

    this.formQuestion = Utils.clone(this.formQuestion, p => {
      p.isScoreEnabled = value;
      p.score = value ? 1 : 0;
    });
    this.formQuestionChange.emit(this.formQuestion);
  }

  public get isShowExplationNote(): boolean {
    return FormQuestionModel.questionTypeToShowExplationNote.includes(this.formQuestion.questionType);
  }

  protected onInit(): void {
    this.subscribe(this.formEditModeService.modeChanged, mode => {
      this.mode = mode;
    });
  }
}
