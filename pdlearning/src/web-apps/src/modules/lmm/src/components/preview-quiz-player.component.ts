import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import { FormEditorPageService, QuizPlayerIntegrationsService } from '@opal20/domain-components';

import { FormQuestionModel } from '@opal20/domain-api';

@Component({
  selector: 'preview-quiz-player',
  templateUrl: './preview-quiz-player.component.html'
})
export class PreviewQuizPlayerComponent extends BaseComponent {
  public formData: FormQuestionModel[];

  private _formId: string | null;
  public get formId(): string | null {
    return this._formId;
  }
  @Input() public set formId(v: string | null) {
    if (this._formId === v) {
      return;
    }
    this._formId = v;

    if (this.initiated) {
      this.quizPlayerIntegrationsService.setFormId(this.formId);
      this.initFormQuestionData(this.formId);
    }
  }

  private _resourceId: string | null;
  public get resourceId(): string | null {
    return this._resourceId;
  }
  @Input() public set resourceId(v: string | null) {
    if (this._resourceId === v) {
      return;
    }
    this._resourceId = v;

    if (this.initiated) {
      this.quizPlayerIntegrationsService.setResourceId(this.resourceId);
    }
  }

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private quizPlayerIntegrationsService: QuizPlayerIntegrationsService,
    private formEditorPageService: FormEditorPageService
  ) {
    super(moduleFacadeService);
  }

  protected onInit(): void {
    // Virtual method
    this.quizPlayerIntegrationsService.setFormId(this.formId);
    this.quizPlayerIntegrationsService.setResourceId(this.resourceId);
    this.initFormQuestionData(this.formId);
  }

  private initFormQuestionData(formId: string): void {
    this.formEditorPageService.loadFormAndFormQuestionsData(this.formId).subscribe(data => {
      this.formData = data.questions;
    });
  }
}
