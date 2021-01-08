import { IRegistrationECertificateModel, RegistrationECertificateModel } from '@opal20/domain-api';

export interface IRegistrationECertificateViewModel {
  registrationECertificate: IRegistrationECertificateModel;
}

export class RegistrationECertificateViewModel implements IRegistrationECertificateViewModel {
  public registrationECertificate: RegistrationECertificateModel = new RegistrationECertificateModel();
  constructor(data?: IRegistrationECertificateViewModel) {
    if (data != null) {
      this.registrationECertificate = new RegistrationECertificateModel(data.registrationECertificate);
    }
  }
}
