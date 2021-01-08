import { Observable, from } from 'rxjs';

import { BaseRepository } from '@opal20/infrastructure';
import { FormParticipant } from '../models/form-participant';
import { FormParticipantApiService } from '../services/form-participant-api.service';
import { FormParticipantForm } from '../models/form-participant-form-model';
import { FormParticipantRepositoryContext } from '../form-participant-repository-context';
import { FormParticipantSearchResult } from '../dtos/form-participant-search-result';
import { Injectable } from '@angular/core';

@Injectable()
export class FormParticipantRepository extends BaseRepository<FormParticipantRepositoryContext> {
  constructor(context: FormParticipantRepositoryContext, private formParticipantApiService: FormParticipantApiService) {
    super(context);
  }

  public searchFormParticipants(
    formOriginalObjectId: string = '',
    skipCount: number = 0,
    maxResultCount: number = 10,
    showSpinner: boolean = true
  ): Observable<FormParticipantSearchResult> {
    return this.processUpsertData<FormParticipant, FormParticipantSearchResult>(
      this.context.formParticipantsSubject,
      implicitLoad =>
        from(
          this.formParticipantApiService.searchFormParticipants(
            {
              formOriginalObjectId: formOriginalObjectId,
              pagedInfo: {
                skipCount: skipCount,
                maxResultCount: maxResultCount
              }
            },
            showSpinner && !implicitLoad
          )
        ),
      'searchFormParticipants',
      [formOriginalObjectId, skipCount, maxResultCount],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items = apiResult.items.map(item => repoData[item.id]).filter(_ => _ != null);
        return apiResult;
      },
      apiResult => apiResult.items.map(_ => new FormParticipant(_)),
      x => x.id
    );
  }

  public getFormParticipantsByFormIds(formIds: string[], showSpinner: boolean = true): Observable<FormParticipantForm[]> {
    return this.processUpsertData<FormParticipantForm, FormParticipantForm[]>(
      this.context.formParticipantsFormSubject,
      implicitLoad =>
        from(
          this.formParticipantApiService.getFormParticipantsByFormIds(
            {
              formIds: formIds
            },
            showSpinner && !implicitLoad
          )
        ),
      'getFormParticipantsByFormIds',
      [formIds],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult = apiResult.map(item => repoData[item.form.id]).filter(_ => _ != null);
        return apiResult;
      },
      apiResult => apiResult.map(_ => new FormParticipantForm(_)),
      x => x.form.id
    );
  }
}
