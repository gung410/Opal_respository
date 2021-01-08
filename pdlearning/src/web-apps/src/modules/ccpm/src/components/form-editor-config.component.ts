import { AnswerFeedbackDisplayOption, FormConfiguration } from '@opal20/domain-api';
import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output } from '@angular/core';

import { FormDetailMode } from '@opal20/domain-components';
import { FormEditModeService } from '../services/form-edit-mode.service';
import { requiredIfValidator } from '@opal20/common-components';

@Component({
  selector: 'form-editor-config',
  templateUrl: './form-editor-config.component.html'
})
export class FormEditorConfigComponent extends BaseFormComponent {
  public inMinutesTimeLimit: number | undefined;
  public _formConfig: FormConfiguration = new FormConfiguration();
  public displayedFormConfiguration: FormConfiguration;
  public mode: FormDetailMode = this.formEditModeService.initMode;
  public FormDetailMode: typeof FormDetailMode = FormDetailMode;

  public answerFeedbackDisplayOptionSelectedItems = [
    {
      value: AnswerFeedbackDisplayOption.AfterAnsweredQuestion,
      display: this.translateCommon('Immediately after the question is answered')
    },
    {
      value: AnswerFeedbackDisplayOption.AfterCompletedQuiz,
      display: this.translateCommon('After the whole quiz is completed')
    },
    {
      value: AnswerFeedbackDisplayOption.AfterXAtemps,
      display: this.translateCommon('After x attempts on the quiz')
    }
  ];

  @Input() public passingMarkMaxScore: number;

  @Input()
  public set formConfig(value: FormConfiguration) {
    this._formConfig = value;

    if (value !== undefined) {
      this.inMinutesTimeLimit = value.inSecondTimeLimit !== undefined ? Math.floor(value.inSecondTimeLimit / 60) : undefined;
    }
  }
  public get formConfig(): FormConfiguration {
    return this._formConfig;
  }

  @Output()
  public formConfigChange: EventEmitter<FormConfiguration> = new EventEmitter<FormConfiguration>();

  constructor(protected moduleFacadeService: ModuleFacadeService, private formEditModeService: FormEditModeService) {
    super(moduleFacadeService);
  }

  public onConfigRandomizedQuestionsChange(value: boolean): void {
    this.formConfig = Utils.clone(this.formConfig, p => {
      p.randomizedQuestions = value;
    });
    this.formConfigChange.emit(this.formConfig);
  }

  public onTimeLimitChange(timeLimit: number | undefined): void {
    this.formConfig = Utils.clone(this.formConfig, p => {
      p.inSecondTimeLimit = timeLimit !== undefined && timeLimit !== null ? Number(timeLimit) * 60 : undefined;
    });
    this.formConfigChange.emit(this.formConfig);
  }

  public onMaxAttemptChange(value: number | undefined): void {
    this.formConfig = Utils.clone(this.formConfig, p => {
      p.maxAttempt = value;
    });
    this.formConfigChange.emit(this.formConfig);
  }

  public onPassingMarkPercentageChanged(value: number | undefined): void {
    this.formConfig = Utils.clone(this.formConfig, p => {
      p.passingMarkPercentage = value;
    });
    this.formConfigChange.emit(this.formConfig);
  }

  public onPassingMarkScoreChanged(value: number | undefined): void {
    this.formConfig = Utils.clone(this.formConfig, p => {
      p.passingMarkScore = value;
    });
    this.formConfigChange.emit(this.formConfig);
  }

  public onConfigAttemptToShowFeedbackChanged(value: number): void {
    this.formConfig = Utils.clone(this.formConfig, p => {
      p.attemptToShowFeedback = value;
    });
    this.formConfigChange.emit(this.formConfig);
  }

  public onConfigFeedbackDisplayOptionChanged(value: AnswerFeedbackDisplayOption): void {
    this.formConfig = Utils.clone(this.formConfig, p => {
      p.answerFeedbackDisplayOption = value;
    });
    this.formConfigChange.emit(this.formConfig);
  }

  public getMaxAttemptToShowFeedback(): number {
    if (this.formConfig.maxAttempt === 0 || Utils.isNullOrEmpty(this.formConfig.maxAttempt)) {
      return 1000;
    }
    return this.formConfig.maxAttempt;
  }

  public get canSelectAttempsToShowFeedback(): boolean {
    return this.formConfig.answerFeedbackDisplayOption === AnswerFeedbackDisplayOption.AfterXAtemps;
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'form',
      controls: {
        attemptToShowFeedback: {
          defaultValue: this.formConfig.attemptToShowFeedback,
          validators: [{ validator: requiredIfValidator(p => this.canSelectAttempsToShowFeedback), validatorType: 'required' }]
        }
      }
    };
  }

  protected onInit(): void {
    this.displayedFormConfiguration = Utils.clone(this.formConfig);
    this.subscribe(this.formEditModeService.modeChanged, mode => {
      this.mode = mode;
    });
  }
}
