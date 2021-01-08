import { BaseRepositoryContext } from '@opal20/infrastructure';
import { BehaviorSubject } from 'rxjs';
import { FormParticipant } from './models/form-participant';
import { FormParticipantForm } from './models/form-participant-form-model';
import { Injectable } from '@angular/core';

@Injectable()
export class FormParticipantRepositoryContext extends BaseRepositoryContext {
  public formParticipantsSubject: BehaviorSubject<Dictionary<FormParticipant>> = new BehaviorSubject({});
  public formParticipantsFormSubject: BehaviorSubject<Dictionary<FormParticipantForm>> = new BehaviorSubject({});
}
