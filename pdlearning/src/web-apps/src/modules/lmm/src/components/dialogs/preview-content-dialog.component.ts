import {
  Assignment,
  FormModel,
  FormQuestionModel,
  FormSection,
  FormType,
  LearningContentRepository,
  LectureModel
} from '@opal20/domain-api';
import { BaseComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import { FormEditorPageService, LectureContentViewModel, PreviewMode } from '@opal20/domain-components';
import { Observable, Subscription, of } from 'rxjs';

import { DialogAction } from '@opal20/common-components';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { XmlEntities } from 'html-entities';
import { switchMap } from 'rxjs/operators';

@Component({
  selector: 'preview-content-dialog',
  templateUrl: './preview-content-dialog.component.html'
})
export class PreviewContentDialogComponent extends BaseComponent {
  public get assignmentId(): string | undefined {
    return this._assignmentId;
  }

  @Input()
  public set assignmentId(v: string | undefined) {
    if (Utils.isEqual(this._assignmentId, v)) {
      return;
    }
    this._assignmentId = v;
    if (this._assignmentId && this.initiated) {
      this.loadData();
    }
  }

  public _assignmentData: Assignment | undefined;
  public get assignmentData(): Assignment | undefined {
    return this._assignmentData;
  }
  @Input()
  public set assignmentData(v: Assignment | undefined) {
    if (Utils.isDifferent(this._assignmentData, v)) {
      this._assignmentData = v;
      if (this._assignmentData && this.initiated) {
        this.loadData();
      }
    }
  }

  public get lectureId(): string | undefined {
    return this._lectureId;
  }
  @Input()
  public set lectureId(v: string | undefined) {
    if (Utils.isEqual(this._lectureId, v)) {
      return;
    }
    this._lectureId = v;
    if (this._lectureId && this.initiated) {
      this.loadData();
    }
  }

  public _lectureData: LectureModel | undefined;
  public get lectureData(): LectureModel | undefined {
    return this._lectureData;
  }
  @Input()
  public set lectureData(v: LectureModel | undefined) {
    if (Utils.isDifferent(this._lectureData, v)) {
      // Clone data because we change the data, so that we don't want to affect original data
      this._lectureData = Utils.cloneDeep(v);
      if (this._lectureData && this.initiated) {
        this.loadData();
      }
    }
  }

  @Input() public previewMode: PreviewMode = PreviewMode.Web;
  @Input() public playerType: 'Lecture' | 'Assignment' = 'Lecture';

  public _formQuestionsData: FormQuestionModel[] = [];
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
  public FormQuestionModel: typeof FormQuestionModel = FormQuestionModel;
  public lectureVm: LectureContentViewModel = new LectureContentViewModel();
  public resourceId: string;
  private _assignmentId: string;
  private _lectureId: string;
  private _loadDataSub: Subscription = new Subscription();

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private formEditorPageService: FormEditorPageService,
    private learningContentRepository: LearningContentRepository,
    public dialogRef: DialogRef
  ) {
    super(moduleFacadeService);
  }

  public closePreview(): void {
    this.dialogRef.close({ action: DialogAction.Close });
  }

  protected onInit(): void {
    this.loadData();
  }

  private getLecture(): Observable<LectureModel> {
    if (this.lectureData != null) {
      return of(this.lectureData);
    }
    if (this.lectureId != null) {
      return this.learningContentRepository.getLecture(this.lectureId).pipe(this.untilDestroy());
    }
    return of(new LectureModel());
  }

  private loadData(): void {
    this._loadDataSub.unsubscribe();
    this._loadDataSub = this.getLecture()
      .pipe(
        this.untilDestroy(),
        switchMap(lecture => {
          this.lectureVm = new LectureContentViewModel(lecture);
          this.resourceId = this.lectureVm.resourceId;
          return this.lectureVm.isQuiz() ? this.formEditorPageService.loadFormAndFormQuestionsData(this.resourceId) : of(null);
        })
      )
      .subscribe((data: { form: FormModel; questions: FormQuestionModel[]; sections: FormSection[] }) => {
        if (data != null) {
          this.formData = data.form;
          this.formQuestionsData = this.processFormQuestionsData(data.questions);

          // Set formId to make main-quiz-player-page load quiz form
          AppGlobal.quizPlayerIntegrations.setFormId(this.formData.id);
        }
      });
  }

  private processFormQuestionsData(formQuestions: FormQuestionModel[]): FormQuestionModel[] {
    switch (this.formData.type) {
      case FormType.Survey:
      case FormType.Poll:
        formQuestions = formQuestions.map(fq => {
          fq = fq.markQuestionAsNoRequireAnswer();
          fq.questionTitle = new XmlEntities().decode(fq.questionTitle);
          return fq;
        });
        break;
    }
    return formQuestions;
  }
}
