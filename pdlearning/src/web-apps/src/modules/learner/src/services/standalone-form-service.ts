import { Observable, of } from 'rxjs';

import { FormParticipantListService } from '@opal20/domain-components';
import { Injectable } from '@angular/core';
import { StandaloneFormItemModel } from '../models/standalone-form-item.model';
import { Utils } from '@opal20/infrastructure';
import { map } from 'rxjs/operators';

@Injectable()
export class StandaloneFormDataService {
  constructor(private formParticipantListService: FormParticipantListService) {}

  public getStandaloneFormLearningItems(formIds: string[]): Observable<StandaloneFormItemModel[]> {
    if (formIds.length > 0) {
      return this.formParticipantListService
        .getFormParticipantsByFormIds(Utils.distinct(formIds))
        .pipe(map(e => e.map(d => StandaloneFormItemModel.createStandaloneFormItemModel(d))));
    }
    return of([]);
  }
}
