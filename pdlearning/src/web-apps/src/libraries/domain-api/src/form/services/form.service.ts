import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { CreateFormRequest, CreateFormRequestFormQuestion } from '../dtos/create-form-request';
import { FormDataModel, FormQuestionModel } from '../models/form-question.model';
import { FormModel, FormStatus, FormSurveyType, FormType } from '../models/form.model';
import { FormWithQuestionsModel, IFormWithQuestionsModel } from '../models/form-with-questions.model';
import {
  GetPendingApprovalFormsResponseResponse,
  IGetPendingApprovalFormsRequest,
  IGetPendingApprovalFormsResponse
} from '../dtos/get-pending-approval-forms-request';
import { ISearchFormResponse, SearchFormRequest, SearchFormResponse } from '../dtos/search-form-request';
import { Injectable, NgZone } from '@angular/core';
import { UpdateFormRequest, UpdateFormRequestFormQuestion } from '../dtos/update-form-request';

import { CloneFormRequest } from '../dtos/clone-form-request';
import { CreateFormSectionRequest } from '../../form-section/dtos/create-form-section-request';
import { FormSection } from '../../form-section/models/form-section';
import { IArchiveRequest } from '../../share/dtos/archive-form-request';
import { IImportFormRequest } from '../dtos/import-form-request';
import { ITransferOwnershipRequest } from '../../share/dtos/transfer-ownership-request';
import { Observable } from 'rxjs';
import { UpdateFormSectionRequest } from '../../form-section/dtos/update-form-section-request';
import { map } from 'rxjs/operators';

@Injectable()
export class FormApiService extends BaseBackendService {
  constructor(protected commonFacadeService: CommonFacadeService, private ngZone: NgZone) {
    super(commonFacadeService);
  }

  protected get apiUrl(): string {
    return AppGlobal.environment.formApiUrl;
  }

  public deleteForm(formId: string): Observable<unknown> {
    return this.delete(`/forms/${formId}`);
  }

  public createForm(
    form: FormModel,
    formQuestions: FormQuestionModel[] = [],
    formSections: FormSection[] = [],
    isAutoSave: boolean = false
  ): Observable<FormWithQuestionsModel> {
    const formQuestionsRequest: CreateFormRequestFormQuestion[] = formQuestions.map(p => new CreateFormRequestFormQuestion(p));
    const formSectionsRequest: CreateFormSectionRequest[] = formSections.map(section => new CreateFormSectionRequest(section));
    const requestBody = new CreateFormRequest(form, formQuestionsRequest, formSectionsRequest, isAutoSave);
    return this.post<CreateFormRequest, IFormWithQuestionsModel>('/forms/create', requestBody).pipe(
      map(_ => new FormWithQuestionsModel(_))
    );
  }

  public updateForm(
    form: FormModel,
    formQuestions: FormQuestionModel[] = [],
    formSections: FormSection[] = [],
    isAutoSave: boolean = false,
    showSpinner: boolean = true,
    isUpdateToNewVersion: boolean = false
  ): Observable<FormWithQuestionsModel> {
    const toSaveFormQuestions = formQuestions.filter(p => !p.isDeleted).map(p => new UpdateFormRequestFormQuestion(p));
    const saveFormSections = formSections.filter(p => !p.isDeleted).map(p => new UpdateFormSectionRequest(p));
    const requestBody = new UpdateFormRequest(
      form,
      toSaveFormQuestions,
      formQuestions.filter(p => p.isDeleted === true).map(_ => _.id),
      saveFormSections,
      formSections.filter(section => section.isDeleted === true).map(section => section.id),
      isAutoSave,
      isUpdateToNewVersion
    );
    return this.post<UpdateFormRequest, IFormWithQuestionsModel>('/forms/update', requestBody, showSpinner).pipe(
      map(_ => new FormWithQuestionsModel(_))
    );
  }

  public updateStatusAndData(
    form: FormModel,
    formQuestions: FormQuestionModel[] = [],
    formSections: FormSection[] = [],
    isAutoSave: boolean = false,
    showSpinner: boolean = true,
    isUpdateToNewVersion: boolean = false,
    comment?: string
  ): Observable<FormWithQuestionsModel> {
    const toSaveFormQuestions = formQuestions.filter(p => p.isDeleted !== true).map(p => new UpdateFormRequestFormQuestion(p));
    const saveFormSections = formSections.filter(p => !p.isDeleted).map(p => new UpdateFormSectionRequest(p));
    const requestBody = new UpdateFormRequest(
      form,
      toSaveFormQuestions,
      formQuestions.filter(p => p.isDeleted === true).map(_ => _.id),
      saveFormSections,
      formSections.filter(section => section.isDeleted === true).map(section => section.id),
      isAutoSave,
      isUpdateToNewVersion,
      comment
    );
    return this.put<UpdateFormRequest, IFormWithQuestionsModel>('/forms/update-status-and-data', requestBody, showSpinner).pipe(
      map(_ => new FormWithQuestionsModel(_))
    );
  }

  public getFormWithQuestionsById(formId: string, showSpinner?: boolean): Observable<FormWithQuestionsModel> {
    return this.get<IFormWithQuestionsModel>(`/forms/${formId}/full-with-questions-by-id`, null, showSpinner).pipe(
      map(_ => new FormWithQuestionsModel(_))
    );
  }

  public getFormStandaloneById(formId: string, showSpinner?: boolean): Observable<FormWithQuestionsModel> {
    return this.get<IFormWithQuestionsModel>(`/forms/${formId}/standalone`, null, showSpinner).pipe(
      map(_ => new FormWithQuestionsModel(_))
    );
  }

  public getFormDataByVersionTrackingId(versionTrackingId: string, showSpinner?: boolean): Promise<FormDataModel> {
    return this.get<FormDataModel>(`/forms/version-tracking-form-data/${versionTrackingId}`, null, showSpinner)
      .pipe(map(_ => new FormDataModel(_)))
      .toPromise();
  }

  public cloneForm(formId: string, newFormTitle: string): Observable<FormWithQuestionsModel> {
    const requestBody = new CloneFormRequest(formId, newFormTitle);
    return this.post<CloneFormRequest, IFormWithQuestionsModel>('/forms/clone', requestBody).pipe(map(_ => new FormWithQuestionsModel(_)));
  }

  public transferOwnerShip(request: ITransferOwnershipRequest, showSpinner: boolean = true): Observable<void> {
    return this.put<ITransferOwnershipRequest, void>('/forms/transfer', request, showSpinner);
  }

  public archiveForm(request: IArchiveRequest, showSpinner: boolean = true): Observable<void> {
    return this.put<IArchiveRequest, void>('/forms/archive', request, showSpinner);
  }

  public importForm(request: IImportFormRequest, showSpinner: boolean = true): Observable<void> {
    return this.post<IImportFormRequest, void>('/forms/import', request, showSpinner);
  }

  public searchForm(
    skipCount: number,
    maxResultCount: number,
    searchFormTitle: string | undefined = undefined,
    filterByStatus: FormStatus[] = [],
    includeFormForImportToCourse: boolean = false,
    filterByType?: FormType,
    filterBySurveyTypes: FormSurveyType[] = [],
    showSpinner?: boolean,
    isSurveyTemplate?: boolean,
    excludeBySurveyTypes: FormSurveyType[] = []
  ): Observable<SearchFormResponse> {
    const requestBody: SearchFormRequest = {
      pagedInfo: {
        skipCount: skipCount,
        maxResultCount: maxResultCount
      },
      searchFormTitle: searchFormTitle,
      filterByStatus: filterByStatus,
      includeFormForImportToCourse: includeFormForImportToCourse,
      filterByType: filterByType,
      filterBySurveyTypes: filterBySurveyTypes,
      isSurveyTemplate: isSurveyTemplate,
      excludeBySurveyTypes: excludeBySurveyTypes
    };
    return this.post<SearchFormRequest, ISearchFormResponse>('/forms/search', requestBody, showSpinner).pipe(
      map(_ => new SearchFormResponse(_))
    );
  }

  public getPendingApprovalForms(skipCount: number, maxResultCount: number): Observable<GetPendingApprovalFormsResponseResponse> {
    const requestBody: IGetPendingApprovalFormsRequest = {
      pagedInfo: {
        skipCount: skipCount,
        maxResultCount: maxResultCount
      }
    };
    return this.post<IGetPendingApprovalFormsRequest, IGetPendingApprovalFormsResponse>('/forms/getPendingApprovalForms', requestBody).pipe(
      map(response => new GetPendingApprovalFormsResponseResponse(response))
    );
  }
}
