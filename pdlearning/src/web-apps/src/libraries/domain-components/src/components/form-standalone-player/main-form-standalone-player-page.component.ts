import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, OnInit } from '@angular/core';
import {
  FormAnswerModel,
  FormApiService,
  FormModel,
  FormParticipantApiService,
  FormParticipantStatus,
  FormQuestionModel,
  FormSection,
  FormSectionsQuestions,
  FormType,
  FormWithQuestionsModel,
  StandaloneSurveyApiService,
  StandaloneSurveyModel,
  StandaloneSurveySectionsQuestions,
  SurveyQuestionModel,
  SurveySection,
  SurveyWithQuestionsModel
} from '@opal20/domain-api';
import { Observable, Subject, combineLatest } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { QuizPlayerIntegrationsService } from '../../services/quiz-player-integrations.service';
import { XmlEntities } from 'html-entities';

@Component({
  selector: 'main-form-standalone-player-page',
  templateUrl: './main-form-standalone-player-page.component.html'
})
export class MainFormStandalonePlayerPageComponent extends BasePageComponent implements OnInit {
  public formId: string | undefined;
  public isFinished: boolean | undefined;
  public formData: FormModel = new FormModel();
  public playerType?: 'lnaform' | 'form';

  private _formQuestionsData: FormQuestionModel[] | SurveyQuestionModel[] = [];
  public get defaultFormQuestionsData(): FormQuestionModel[] {
    return <FormQuestionModel[]>this._formQuestionsData;
  }

  public get lnaFormQuestionsData(): SurveyQuestionModel[] {
    return <SurveyQuestionModel[]>this._formQuestionsData;
  }

  public set formQuestionsData(v: FormQuestionModel[] | SurveyQuestionModel[]) {
    this._formQuestionsData = v;
  }
  private _formDataSubject: Subject<FormModel | StandaloneSurveyModel> = new Subject();
  private _formQuestionsDataSubject: Subject<FormSectionsQuestions | StandaloneSurveySectionsQuestions> = new Subject();
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private formApiService: FormApiService,
    private lnaFormApiService: StandaloneSurveyApiService,
    private quizPlayerService: QuizPlayerIntegrationsService,
    private formParticipantApiService: FormParticipantApiService
  ) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.initData();
  }

  public onQuizFinished(formAnswerModel: FormAnswerModel): void {
    this.formParticipantApiService.updateFormParticipantStatus({
      formId: this.formData.id
    });
  }

  private initData(): void {
    const navData: { formId: string; from: 'lnaform' | 'form' } = this.moduleFacadeService.contextDataService.getData(
      this.moduleFacadeService.moduleInstance.contextDataKey
    );

    this.playerType = navData && navData.from ? navData.from : 'form';

    combineLatest(this.quizPlayerService.formOriginalObjectId$)
      .pipe(
        this.untilDestroy(),
        switchMap(([formOriginalObjectId]) => {
          this.formId = formOriginalObjectId ? formOriginalObjectId : navData ? navData.formId : null;
          if (this.formId) {
            this.formParticipantApiService.getMyParticipantData(this.formId).then(resp => {
              this.isFinished = resp.status === FormParticipantStatus.Completed;
            });
            return this.loadFormAndFormQuestionsData(this.formId, this.playerType);
          } else {
            return Promise.resolve(null);
          }
        })
      )
      .subscribe(response => {
        if (response) {
          this.formData = response.form;

          if (this.playerType === 'form') {
            AppGlobal.quizPlayerIntegrations.setFormId(this.formData.id);
          } else if (this.playerType === 'lnaform') {
            AppGlobal.standaloneSurveyIntegration.setFormId(this.formData.id);
          }

          this.formQuestionsData = this.processFormQuestionsData(response.questions);
          this.decodeFormQuestionsTitle(this._formQuestionsData);
        }
      });
  }

  private decodeFormQuestionsTitle(
    formQuestions: FormQuestionModel[] | SurveyQuestionModel[]
  ): FormQuestionModel[] | SurveyQuestionModel[] {
    formQuestions.forEach(question => {
      question.questionTitle = new XmlEntities().decode(question.questionTitle);
    });
    return formQuestions;
  }

  private processFormQuestionsData(
    formQuestions: FormQuestionModel[] | SurveyQuestionModel[]
  ): FormQuestionModel[] | SurveyQuestionModel[] {
    switch (this.formData.type) {
      case FormType.Survey:
      case FormType.Poll:
      case undefined: // Note: lna form do not have form type -> by default it is a survey.
        for (const fq of formQuestions) {
          fq.markQuestionAsNoRequireAnswer();
        }
        break;
    }
    return formQuestions;
  }

  private loadFormAndFormQuestionsData(
    formId: string,
    type: 'lnaform' | 'form',
    showSpinner: boolean = true
  ): Observable<
    | {
        form: FormModel | StandaloneSurveyModel;
        questions: FormQuestionModel[] | SurveyQuestionModel[];
        sections: FormSection[] | SurveySection[];
      }
    | undefined
  > {
    const getFormResult: Observable<FormWithQuestionsModel | SurveyWithQuestionsModel> =
      type === 'form'
        ? this.formApiService.getFormStandaloneById(formId, showSpinner)
        : this.lnaFormApiService.getSurveyStandaloneById(formId, showSpinner);

    return getFormResult.pipe(
      map(formWithQuestions => {
        this._formDataSubject.next(formWithQuestions.form);
        const formSectionsQuestions =
          type === 'form'
            ? new FormSectionsQuestions({
                formQuestions: (<FormWithQuestionsModel>formWithQuestions).formQuestions,
                formSections: (<FormWithQuestionsModel>formWithQuestions).formSections
              })
            : new StandaloneSurveySectionsQuestions({
                formQuestions: (<SurveyWithQuestionsModel>formWithQuestions).formQuestions,
                formSections: (<SurveyWithQuestionsModel>formWithQuestions).formSections
              });
        this._formQuestionsDataSubject.next(formSectionsQuestions);
        return { form: formWithQuestions.form, questions: formWithQuestions.formQuestions, sections: formWithQuestions.formSections };
      })
    );
  }
}
