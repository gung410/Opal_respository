import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import { FormModel, FormQuestionModel, FormType } from '@opal20/domain-api';

import { DialogAction } from '@opal20/common-components';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { FormEditorPageService } from './../../services/form-editor-page.service';
import { PreviewMode } from './../../models/preview-mode.model';
import { XmlEntities } from 'html-entities';

@Component({
  selector: 'preview-form-dialog',
  templateUrl: './preview-form-dialog.component.html'
})
export class PreviewFormDialogComponent extends BaseComponent {
  public get formId(): string | undefined {
    return this._formId;
  }
  @Input()
  public set formId(v: string | undefined) {
    this._formId = v;
    if (this._formId) {
      this.loadData();
    }
  }

  public get formQuestionsData(): FormQuestionModel[] {
    return this._formQuestionsData;
  }

  public set formQuestionsData(v: FormQuestionModel[]) {
    this._formQuestionsData = v;
    this.selectedFormQuestion = this.formQuestionsData[this.selectedQuestionNumber - 1];
  }
  public previewOptions = [PreviewMode.Web, PreviewMode.Mobile];
  public formData: FormModel = new FormModel();
  public selectedFormQuestion: FormQuestionModel | undefined;
  public selectedQuestionNumber: number | undefined;
  public isWebPreviewPostForm: boolean = false;
  public previewFormMode: PreviewMode = PreviewMode.Web;
  public PreviewMode: typeof PreviewMode = PreviewMode;
  public FormQuestionModel: typeof FormQuestionModel = FormQuestionModel;
  private _formId: string;
  private _formQuestionsData: FormQuestionModel[] = [];
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public dialogRef: DialogRef,
    private formEditorPageService: FormEditorPageService
  ) {
    super(moduleFacadeService);
  }

  public closePreview(): void {
    this.dialogRef.close({ action: DialogAction.Close });
  }

  private loadData(): void {
    this.formEditorPageService.loadFormAndFormQuestionsData(this.formId).subscribe(data => {
      this.formData = data.form;
      AppGlobal.quizPlayerIntegrations.setFormId(this.formData.id);
      this.formQuestionsData = this.processFormQuestionsData(data.questions);
      this.decodeFormQuestionsTitle(this.formQuestionsData);
    });
  }

  private decodeFormQuestionsTitle(formQuestions: FormQuestionModel[]): FormQuestionModel[] {
    formQuestions.forEach(question => {
      question.questionTitle = new XmlEntities().decode(question.questionTitle);
    });
    return formQuestions;
  }

  private processFormQuestionsData(formQuestions: FormQuestionModel[]): FormQuestionModel[] {
    switch (this.formData.type) {
      case FormType.Survey:
      case FormType.Poll:
        formQuestions = formQuestions.map(fq => fq.markQuestionAsNoRequireAnswer());
        break;
    }
    return formQuestions;
  }
}
