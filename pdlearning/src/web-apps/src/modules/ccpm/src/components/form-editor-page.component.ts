import { BasePageComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, ElementRef, EventEmitter, Input, Output, SimpleChanges, ViewChild } from '@angular/core';
import { FormConfiguration, FormModel, FormQuestionModel, FormSectionsQuestions, FormType } from '@opal20/domain-api';

import { FormDetailMode } from '@opal20/domain-components';
import { FormEditorComponent } from './form-editor.component';
import { FormEditorConfigComponent } from './form-editor-config.component';

@Component({
  selector: 'form-editor-page',
  templateUrl: './form-editor-page.component.html'
})
export class FormEditorPageComponent extends BasePageComponent {
  @ViewChild('formEditor', { static: false }) public formEditor: FormEditorComponent;
  @ViewChild('formEditorConfig', { static: false }) public formEditorConfig: FormEditorConfigComponent;
  @ViewChild('formEditorPageLeftCol', { static: false }) public formEditorPageLeftCol: ElementRef;

  public readonly formType: typeof FormType = FormType;
  public isExpandedOption: boolean = false;
  @Input('formData') public formData: FormModel = new FormModel();

  @Input('mode') public mode: FormDetailMode = FormDetailMode.View;

  @Input('formSectionsQuestions') public _formSectionsQuestions: FormSectionsQuestions = new FormSectionsQuestions({
    formQuestions: [],
    formSections: []
  });
  public get formSectionsQuestions(): FormSectionsQuestions {
    return this._formSectionsQuestions;
  }
  public set formSectionsQuestions(v: FormSectionsQuestions) {
    this._formSectionsQuestions = v;
    this.notDeletedFormQuestionsData = v.formQuestions.filter(p => !p.isDeleted);
    this.setSelectedQuestion(this.selectedQuestionId);
    this.formSectionsQuestionsChange.emit(this._formSectionsQuestions);
  }

  @Output() public formSectionsQuestionsChange: EventEmitter<FormSectionsQuestions> = new EventEmitter<FormSectionsQuestions>();
  @Output() public formDataChange: EventEmitter<FormModel> = new EventEmitter<FormModel>();

  public formConfig: FormConfiguration = new FormConfiguration();
  public notDeletedFormQuestionsData: FormQuestionModel[] = [];
  public selectedQuestionId: string | undefined;
  public selectedFormQuestion: FormQuestionModel | undefined;

  public get indexInform(): string {
    if (this.selectedFormQuestion.priority !== null) {
      const sectionIndex = this.selectedFormQuestion.priority + 1;
      const questionIndex = this.selectedFormQuestion.minorPriority >= 0 ? this.selectedFormQuestion.minorPriority + 1 : '';
      return questionIndex ? `${sectionIndex}.${questionIndex}` : `${sectionIndex}`;
    }
  }

  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public onFormDataChanged($event: FormModel): void {
    this.formData = $event;
    this.formDataChange.emit(this.formData);
  }

  public onFormConfigChange(event: FormConfiguration): void {
    this.formConfig = event;
    this.formData.inSecondTimeLimit = this.formConfig.inSecondTimeLimit;
    this.formData.randomizedQuestions = this.formConfig.randomizedQuestions;
    this.formData.maxAttempt = this.formConfig.maxAttempt;
    this.formData.passingMarkPercentage = this.formConfig.passingMarkPercentage;
    this.formData.passingMarkScore = this.formConfig.passingMarkScore;
    this.formData.attemptToShowFeedback = this.formConfig.attemptToShowFeedback;
    this.formData.answerFeedbackDisplayOption = this.formConfig.answerFeedbackDisplayOption;
    this.formDataChange.emit(this.formData);
  }

  public onPanelStateChange(): void {
    if (!this.isExpandedOption) {
      return;
    }
    this.isExpandedOption = false;
  }

  public onQuestionSelect(): void {
    this.isExpandedOption = true;
  }

  public onSelectedQuestionIdChange(id: string | undefined): void {
    this.setSelectedQuestion(id);
    if (this.formEditorPageLeftCol) {
      this.formEditorPageLeftCol.nativeElement.scroll({
        top: 0,
        behavior: 'smooth'
      });
    }
  }

  public onSelectedFormQuestionChange(event: FormQuestionModel): void {
    this.selectedFormQuestion = event;
    const selectedFormQuestionIndex = this.getSelectedFormQuestionIndex();
    const currenSection = this.formSectionsQuestions.formSections;
    this.formSectionsQuestions = {
      formQuestions: Utils.clone(this.formSectionsQuestions.formQuestions, questions => {
        questions[selectedFormQuestionIndex].questionHint = this.selectedFormQuestion.questionHint;
        questions[selectedFormQuestionIndex].randomizedOptions = this.selectedFormQuestion.randomizedOptions;
        questions[selectedFormQuestionIndex].score = this.selectedFormQuestion.score;
        questions[selectedFormQuestionIndex].answerExplanatoryNote = this.selectedFormQuestion.answerExplanatoryNote;
        questions[selectedFormQuestionIndex].isScoreEnabled = this.selectedFormQuestion.isScoreEnabled;
      }),
      formSections: currenSection
    };
    this.formSectionsQuestionsChange.emit(this.formSectionsQuestions);
  }

  public getSelectedFormQuestionIndex(): number {
    if (this.selectedFormQuestion == null) {
      return -1;
    }
    return this.formSectionsQuestions.formQuestions.findIndex(p => p.id === this.selectedFormQuestion.id);
  }

  public onQuestionMoved(movedQuestionId: string): void {
    this.setSelectedQuestion(movedQuestionId);
  }

  public initFormConfigData(): void {
    this.formConfig = new FormConfiguration({
      inSecondTimeLimit: this.formData.inSecondTimeLimit,
      randomizedQuestions: this.formData.randomizedQuestions,
      maxAttempt: this.formData.maxAttempt,
      passingMarkScore: this.formData.passingMarkScore,
      passingMarkPercentage: this.formData.passingMarkPercentage,
      attemptToShowFeedback: this.formData.attemptToShowFeedback,
      answerFeedbackDisplayOption: this.formData.answerFeedbackDisplayOption
    });
  }

  public get passingMarkMaxScore(): number {
    const totalScore = FormQuestionModel.calcMaxScore(this._formSectionsQuestions.formQuestions);
    return totalScore === 0 ? 1000 : totalScore;
  }

  public get canDisableScore(): boolean {
    if (!this.formData.passingMarkPercentage && !this.formData.passingMarkScore) {
      return false;
    }

    const currentQuestionList = this.formSectionsQuestions.formQuestions;
    return currentQuestionList.filter(q => q.isScoreEnabled).length === 1;
  }

  protected onChanges(changes: SimpleChanges): void {
    if (!changes.formData) {
      return;
    }
    this.initFormConfigData();
  }

  protected onInit(): void {
    //
  }

  protected onDestroy(): void {
    //
  }

  private setSelectedQuestion(id: string): void {
    const selectedFormQuestionIndex = this.formSectionsQuestions.formQuestions.findIndex(p => p.id === id);
    if (selectedFormQuestionIndex > -1) {
      this.selectedFormQuestion = this.formSectionsQuestions.formQuestions[selectedFormQuestionIndex];
    }
    this.selectedQuestionId = id;
  }
}
