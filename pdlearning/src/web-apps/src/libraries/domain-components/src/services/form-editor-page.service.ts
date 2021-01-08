import { BehaviorSubject, Observable, Subject } from 'rxjs';
import {
  FormApiService,
  FormModel,
  FormQuestionModel,
  FormSection,
  FormSectionsQuestions,
  FormWithQuestionsModel
} from '@opal20/domain-api';

import { Injectable } from '@angular/core';
import { Utils } from '@opal20/infrastructure';
import { map } from 'rxjs/operators';

@Injectable()
export class FormEditorPageService {
  public get formData$(): Observable<FormModel> {
    return this._formDataSubject.asObservable();
  }
  public get formAutoSaveInformer$(): Observable<boolean> {
    return this._formAutoSaveInformerSubject.asObservable();
  }
  public get formQuestionsData$(): Observable<FormSectionsQuestions> {
    return this._formQuestionsDataSubject.asObservable();
  }
  public formUpdatedEvent: Subject<IFormEditorPageFormUpdatedEventData> = new Subject();

  private _formDataSubject: Subject<FormModel> = new Subject();
  private _formQuestionsDataSubject: Subject<FormSectionsQuestions> = new Subject();

  private _formAutoSaveInformerSubject: BehaviorSubject<boolean> = new BehaviorSubject(false);

  constructor(private formApiService: FormApiService) {}

  // Update data without change status
  public updateFormData(
    form: FormModel,
    formQuestions?: FormQuestionModel[],
    formSections?: FormSection[],
    isAutoSave: boolean = false,
    showSpinner: boolean = true
  ): Observable<FormWithQuestionsModel> {
    form = Utils.clone(form);
    formQuestions = formQuestions !== undefined ? Utils.clone(formQuestions) : [];
    return this.formApiService.updateForm(form, formQuestions, formSections, isAutoSave, showSpinner).pipe(
      map((_: FormWithQuestionsModel) => {
        this._formDataSubject.next(_.form);
        const formSectionsQuestions = new FormSectionsQuestions({
          formQuestions: _.formQuestions,
          formSections: _.formSections
        });
        this._formQuestionsDataSubject.next(formSectionsQuestions);
        this._formAutoSaveInformerSubject.next(isAutoSave);
        return _;
      })
    );
  }

  // update data and update status
  public changeStatusAndUpdateData(
    form: FormModel,
    formQuestions?: FormQuestionModel[],
    formSections?: FormSection[],
    isAutoSave: boolean = false,
    showSpinner: boolean = true,
    isUpdateToNewVersion: boolean = false,
    comment?: string
  ): Observable<FormWithQuestionsModel> {
    form = Utils.clone(form);
    formQuestions = formQuestions !== undefined ? Utils.clone(formQuestions) : [];

    return this.formApiService
      .updateStatusAndData(form, formQuestions, formSections, isAutoSave, showSpinner, isUpdateToNewVersion, comment)
      .pipe(
        map((_: FormWithQuestionsModel) => {
          this._formDataSubject.next(_.form);
          const formSectionsQuestions = new FormSectionsQuestions({
            formQuestions: _.formQuestions,
            formSections: _.formSections
          });
          this._formQuestionsDataSubject.next(formSectionsQuestions);
          return _;
        })
      );
  }

  public loadFormAndFormQuestionsData(
    formId: string,
    showSpinner: boolean = true
  ): Observable<{ form: FormModel; questions: FormQuestionModel[]; sections: FormSection[] } | undefined> {
    return this.formApiService.getFormWithQuestionsById(formId, showSpinner).pipe(
      map(formWithQuestions => {
        this._formDataSubject.next(formWithQuestions.form);
        const formSectionsQuestions = new FormSectionsQuestions({
          formQuestions: formWithQuestions.formQuestions,
          formSections: formWithQuestions.formSections
        });
        this._formQuestionsDataSubject.next(formSectionsQuestions);
        return { form: formWithQuestions.form, questions: formWithQuestions.formQuestions, sections: formWithQuestions.formSections };
      })
    );
  }

  public resetFormAutoSaveInformerSubjet(): void {
    this._formAutoSaveInformerSubject.next(false);
  }
}

export interface IFormEditorPageFormUpdatedEventData {
  form: FormModel;
  formQuestions: FormQuestionModel[];
}
