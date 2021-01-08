import { CreateSurveyRequest, CreateSurveyRequestSurveyQuestion } from '../dtos/create-form-request';
import { ISearchSurveyResponse, SearchSurveyRequest, SearchSurveyResponse } from '../dtos/search-form-request';
import { ISurveyWithQuestionsModel, SurveyWithQuestionsModel } from '../models/form-with-questions.model';
import { Injectable, NgZone } from '@angular/core';
import { StandaloneSurveyDataModel, SurveyQuestionModel } from '../models/form-question.model';
import { StandaloneSurveyModel, SurveyStatus } from '../models/lna-form.model';
import { UpdateSurveyRequest, UpdateSurveyRequestSurveyQuestion } from '../dtos/update-form-request';

import { BaseStandaloneSurveyService } from './base-standalone-survey.service';
import { CloneSurveyRequest } from '../dtos/clone-form-request';
import { CommonFacadeService } from '@opal20/infrastructure';
import { CreateFormSectionRequest } from '../../form-section/dtos/create-form-section-request';
import { FormSection } from '../../form-section/models/form-section';
import { IArchiveRequest } from '../../share/dtos/archive-form-request';
import { IImportStandaloneSurveyRequest } from '../dtos/import-form-request';
import { ITransferOwnershipRequest } from '../../share/dtos/transfer-ownership-request';
import { Observable } from 'rxjs';
import { UpdateFormSectionRequest } from '../../form-section/dtos/update-form-section-request';
import { map } from 'rxjs/operators';

@Injectable()
export class StandaloneSurveyApiService extends BaseStandaloneSurveyService {
  constructor(protected commonFacadeService: CommonFacadeService, private ngZone: NgZone) {
    super(commonFacadeService);
  }

  protected get apiUrl(): string {
    return AppGlobal.environment.lnaFormApiUrl;
  }

  public deleteSurvey(formId: string): Observable<unknown> {
    return this.delete(`/forms/${formId}`);
  }

  public createSurvey(
    form: StandaloneSurveyModel,
    formQuestions: SurveyQuestionModel[] = [],
    formSections: FormSection[] = [],
    isAutoSave: boolean = false
  ): Observable<SurveyWithQuestionsModel> {
    const formQuestionsRequest: CreateSurveyRequestSurveyQuestion[] = formQuestions.map(p => new CreateSurveyRequestSurveyQuestion(p));
    const formSectionsRequest: CreateFormSectionRequest[] = formSections.map(section => new CreateFormSectionRequest(section));
    const requestBody = new CreateSurveyRequest(form, formQuestionsRequest, formSectionsRequest, isAutoSave);
    return this.post<CreateSurveyRequest, ISurveyWithQuestionsModel>('/forms/create', requestBody).pipe(
      map(_ => new SurveyWithQuestionsModel(_))
    );
  }

  public updateSurvey(
    form: StandaloneSurveyModel,
    formQuestions: SurveyQuestionModel[] = [],
    formSections: FormSection[] = [],
    isAutoSave: boolean = false,
    showSpinner: boolean = true,
    isUpdateToNewVersion: boolean = false
  ): Observable<SurveyWithQuestionsModel> {
    const toSaveFormQuestions = formQuestions.filter(p => !p.isDeleted).map(p => new UpdateSurveyRequestSurveyQuestion(p));
    const saveFormSections = formSections.filter(p => !p.isDeleted).map(p => new UpdateFormSectionRequest(p));
    const requestBody = new UpdateSurveyRequest(
      form,
      toSaveFormQuestions,
      formQuestions.filter(p => p.isDeleted === true).map(_ => _.id),
      saveFormSections,
      formSections.filter(section => section.isDeleted === true).map(section => section.id),
      isAutoSave,
      isUpdateToNewVersion
    );
    return this.post<UpdateSurveyRequest, ISurveyWithQuestionsModel>('/forms/update', requestBody, showSpinner).pipe(
      map(_ => new SurveyWithQuestionsModel(_))
    );
  }

  public updateStatusAndData(
    form: StandaloneSurveyModel,
    formQuestions: SurveyQuestionModel[] = [],
    formSections: FormSection[] = [],
    isAutoSave: boolean = false,
    showSpinner: boolean = true,
    isUpdateToNewVersion: boolean = false
  ): Observable<SurveyWithQuestionsModel> {
    const toSaveFormQuestions = formQuestions.filter(p => p.isDeleted !== true).map(p => new UpdateSurveyRequestSurveyQuestion(p));
    const saveFormSections = formSections.filter(p => !p.isDeleted).map(p => new UpdateFormSectionRequest(p));
    const requestBody = new UpdateSurveyRequest(
      form,
      toSaveFormQuestions,
      formQuestions.filter(p => p.isDeleted === true).map(_ => _.id),
      saveFormSections,
      formSections.filter(section => section.isDeleted === true).map(section => section.id),
      isAutoSave,
      isUpdateToNewVersion
    );
    return this.put<UpdateSurveyRequest, ISurveyWithQuestionsModel>('/forms/update-status-and-data', requestBody, showSpinner).pipe(
      map(_ => new SurveyWithQuestionsModel(_))
    );
  }

  public getSurveyDataByVersionTrackingId(versionTrackingId: string, showSpinner?: boolean): Promise<StandaloneSurveyDataModel> {
    return this.get<StandaloneSurveyDataModel>(`/forms/version-tracking-survey-data/${versionTrackingId}`, null, showSpinner)
      .pipe(map(_ => new StandaloneSurveyDataModel(_)))
      .toPromise();
  }

  public getSurveyWithQuestionsById(formId: string, showSpinner?: boolean): Observable<SurveyWithQuestionsModel> {
    return this.get<ISurveyWithQuestionsModel>(`/forms/${formId}/full-with-questions-by-id`, null, showSpinner).pipe(
      map(_ => new SurveyWithQuestionsModel(_))
    );
  }

  public getSurveyStandaloneById(formId: string, showSpinner?: boolean): Observable<SurveyWithQuestionsModel> {
    return this.get<ISurveyWithQuestionsModel>(`/forms/${formId}/participant`, null, showSpinner).pipe(
      map(_ => new SurveyWithQuestionsModel(_))
    );
  }

  public cloneSurveys(formId: string, newFormTitle: string): Observable<SurveyWithQuestionsModel> {
    const requestBody = new CloneSurveyRequest(formId, newFormTitle);
    return this.post<CloneSurveyRequest, ISurveyWithQuestionsModel>('/forms/clone', requestBody).pipe(
      map(_ => new SurveyWithQuestionsModel(_))
    );
  }

  public transferOwnerShip(request: ITransferOwnershipRequest, showSpinner: boolean = true): Observable<void> {
    return this.put<ITransferOwnershipRequest, void>('/forms/transfer', request, showSpinner);
  }

  public archiveSurvey(request: IArchiveRequest, showSpinner: boolean = true): Observable<void> {
    return this.put<IArchiveRequest, void>('/forms/archive', request, showSpinner);
  }

  public importStandaloneSurvey(request: IImportStandaloneSurveyRequest, showSpinner: boolean = true): Observable<void> {
    return this.post<IImportStandaloneSurveyRequest, void>('/forms/import', request, showSpinner);
  }

  public searchSurvey(
    skipCount: number,
    maxResultCount: number,
    searchFormTitle: string | undefined = undefined,
    filterByStatus: SurveyStatus[] = [],
    includeFormForImportToCourse: boolean = false,
    showSpinner?: boolean
  ): Observable<SearchSurveyResponse> {
    const requestBody: SearchSurveyRequest = {
      pagedInfo: {
        skipCount: skipCount,
        maxResultCount: maxResultCount
      },
      searchFormTitle: searchFormTitle,
      filterByStatus: filterByStatus,
      includeFormForImportToCourse: includeFormForImportToCourse
    };
    return this.post<SearchSurveyRequest, ISearchSurveyResponse>('/forms/search', requestBody, showSpinner).pipe(
      map(_ => new SearchSurveyResponse(_))
    );
  }
}
