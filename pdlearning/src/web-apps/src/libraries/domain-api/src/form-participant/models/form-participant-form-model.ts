import { IFormModel } from '../../form/models/form.model';
import { IFormParticipant } from './form-participant';

export interface IFormParticipantForm {
  formParticipant: IFormParticipant;
  form: IFormModel;
}

export class FormParticipantForm implements IFormParticipantForm {
  public formParticipant: IFormParticipant;
  public form: IFormModel;

  constructor(data?: IFormParticipantForm) {
    if (data == null) {
      return;
    }

    this.formParticipant = data.formParticipant;
    this.form = data.form;
  }
}
