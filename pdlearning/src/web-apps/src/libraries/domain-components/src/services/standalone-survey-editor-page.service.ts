import { BehaviorSubject, Observable, Subject } from 'rxjs';
import {
  StandaloneSurveyApiService,
  StandaloneSurveyModel,
  StandaloneSurveySectionsQuestions,
  SurveyQuestionModel,
  SurveySection,
  SurveyWithQuestionsModel
} from '@opal20/domain-api';

import { Injectable } from '@angular/core';
import { Utils } from '@opal20/infrastructure';
import { map } from 'rxjs/operators';

@Injectable()
export class StandaloneSurveyEditorPageService {
  public get formData$(): Observable<StandaloneSurveyModel> {
    return this._formDataSubject.asObservable();
  }
  public get formAutoSaveInformer$(): Observable<boolean> {
    return this._formAutoSaveInformerSubject.asObservable();
  }
  public get formQuestionsData$(): Observable<StandaloneSurveySectionsQuestions> {
    return this._formQuestionsDataSubject.asObservable();
  }
  public formUpdatedEvent: Subject<IFormEditorPageFormUpdatedEventData> = new Subject();

  private _formDataSubject: Subject<StandaloneSurveyModel> = new Subject();
  private _formQuestionsDataSubject: Subject<StandaloneSurveySectionsQuestions> = new Subject();

  private _formAutoSaveInformerSubject: BehaviorSubject<boolean> = new BehaviorSubject(false);

  constructor(private formApiService: StandaloneSurveyApiService) {}

  // Update data without change status
  public updateFormData(
    form: StandaloneSurveyModel,
    formQuestions?: SurveyQuestionModel[],
    formSections?: SurveySection[],
    isAutoSave: boolean = false,
    showSpinner: boolean = true
  ): Observable<SurveyWithQuestionsModel> {
    form = Utils.clone(form);
    formQuestions = formQuestions !== undefined ? Utils.clone(formQuestions) : [];
    return this.formApiService.updateSurvey(form, formQuestions, formSections, isAutoSave, showSpinner).pipe(
      map((_: SurveyWithQuestionsModel) => {
        this._formDataSubject.next(_.form);
        const lnaFormSectionsQuestions = new StandaloneSurveySectionsQuestions({
          formQuestions: _.formQuestions,
          formSections: _.formSections
        });
        this._formQuestionsDataSubject.next(lnaFormSectionsQuestions);
        this._formAutoSaveInformerSubject.next(isAutoSave);
        return _;
      })
    );
  }

  // update data and update status
  public changeStatusAndUpdateData(
    form: StandaloneSurveyModel,
    formQuestions?: SurveyQuestionModel[],
    formSections?: SurveySection[],
    isAutoSave: boolean = false,
    showSpinner: boolean = true,
    isUpdateToNewVersion: boolean = false
  ): Observable<SurveyWithQuestionsModel> {
    form = Utils.clone(form);
    formQuestions = formQuestions !== undefined ? Utils.clone(formQuestions) : [];

    return this.formApiService.updateStatusAndData(form, formQuestions, formSections, isAutoSave, showSpinner, isUpdateToNewVersion).pipe(
      map((_: SurveyWithQuestionsModel) => {
        this._formDataSubject.next(_.form);
        const formSectionsQuestions = new StandaloneSurveySectionsQuestions({
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
  ): Observable<{ form: StandaloneSurveyModel; questions: SurveyQuestionModel[]; sections: SurveySection[] } | undefined> {
    return this.formApiService.getSurveyWithQuestionsById(formId, showSpinner).pipe(
      map(formWithQuestions => {
        this._formDataSubject.next(formWithQuestions.form);
        const formSectionsQuestions = new StandaloneSurveySectionsQuestions({
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
  form: StandaloneSurveyModel;
  formQuestions: SurveyQuestionModel[];
}
