import { BasePageComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { StandaloneSurveyModel, StandaloneSurveySectionsQuestions, SurveyConfiguration, SurveyQuestionModel } from '@opal20/domain-api';

import { StandaloneSurveyEditorComponent } from './standalone-survey-editor.component';

@Component({
  selector: 'standalone-survey-editor-page',
  templateUrl: './standalone-survey-editor-page.component.html'
})
export class StandaloneSurveyEditorPageComponent extends BasePageComponent {
  @ViewChild('formEditor', { static: false }) public formEditor: StandaloneSurveyEditorComponent;

  public isExpandedOption: boolean = false;
  @Input('formData') public formData: StandaloneSurveyModel = new StandaloneSurveyModel();

  @Input('formSectionsQuestions') public _formSectionsQuestions: StandaloneSurveySectionsQuestions = new StandaloneSurveySectionsQuestions({
    formQuestions: [],
    formSections: []
  });
  public get formSectionsQuestions(): StandaloneSurveySectionsQuestions {
    return this._formSectionsQuestions;
  }
  public set formSectionsQuestions(v: StandaloneSurveySectionsQuestions) {
    this._formSectionsQuestions = v;
    this.notDeletedFormQuestionsData = v.formQuestions.filter(p => !p.isDeleted);
    this.setSelectedQuestion(this.selectedQuestionId);
    this.formSectionsQuestionsChange.emit(this._formSectionsQuestions);
  }

  @Output() public formSectionsQuestionsChange: EventEmitter<StandaloneSurveySectionsQuestions> = new EventEmitter<
    StandaloneSurveySectionsQuestions
  >();
  @Output() public formDataChange: EventEmitter<StandaloneSurveyModel> = new EventEmitter<StandaloneSurveyModel>();

  public formConfig: SurveyConfiguration = new SurveyConfiguration();
  public notDeletedFormQuestionsData: SurveyQuestionModel[] = [];
  public selectedQuestionId: string | undefined;
  public selectedFormQuestion: SurveyQuestionModel | undefined;

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

  public onFormDataChanged($event: StandaloneSurveyModel): void {
    this.formData = $event;
    this.formDataChange.emit(this.formData);
  }

  public onFormConfigChange(event: SurveyConfiguration): void {
    this.formConfig = event;
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
  }

  public onSelectedFormQuestionChange(event: SurveyQuestionModel): void {
    this.selectedFormQuestion = event;
    const selectedFormQuestionIndex = this.getSelectedFormQuestionIndex();
    const currenSection = this.formSectionsQuestions.formSections;
    this.formSectionsQuestions = {
      formQuestions: Utils.clone(this.formSectionsQuestions.formQuestions),
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
