import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { FormAnswerModel, IFormAnswerModel } from '../models/form-answer.model';
import { ISaveFormAnswer, IUpdateFormAnswerRequest } from '../dtos/update-form-answer-request';

import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class FormAnswerApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.formApiUrl;
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public saveFormAnswer(requestDto: ISaveFormAnswer): Observable<FormAnswerModel> {
    return this.post<ISaveFormAnswer, IFormAnswerModel>('/form-answers', requestDto).pipe(
      map(savedFormAnswer => new FormAnswerModel(savedFormAnswer))
    );
  }

  public updateFormAnswer(requestDto: IUpdateFormAnswerRequest): Observable<FormAnswerModel> {
    return this.post<IUpdateFormAnswerRequest, IFormAnswerModel>(`/form-answers/update`, requestDto).pipe(
      map(formAnswer => new FormAnswerModel(formAnswer))
    );
  }

  public getByFormId(
    formId: string,
    courseId?: string,
    myCourseId?: string,
    classRunId?: string,
    assignmentId?: string,
    userId?: string
  ): Observable<FormAnswerModel[]> {
    return this.get<IFormAnswerModel[]>(`/form-answers/form-ids/${formId}`, {
      courseId: courseId,
      myCourseId: myCourseId,
      classRunId: classRunId,
      assignmentId: assignmentId,
      userId: userId
    }).pipe(map(formAnswer => formAnswer.map(p => new FormAnswerModel(p))));
  }

  public updateFormAnswerScore(requestDto: IUpdateFormAnswerRequest): Observable<FormAnswerModel> {
    return this.post<IUpdateFormAnswerRequest, IFormAnswerModel>(`/form-answers/update-score`, requestDto).pipe(
      map(formAnswer => new FormAnswerModel(formAnswer))
    );
  }
}
