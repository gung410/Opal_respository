export interface IRegistrationECertificateModel {
  id: string;
  userId: string;
  pdfFileName: string;
  base64Pdf: string;
  base64Image: string;
  width: number;
  height: number;
}

export class RegistrationECertificateModel implements IRegistrationECertificateModel {
  public id: string = '';
  public userId: string = '';
  public pdfFileName: string = '';
  public base64Pdf: string = '';
  public base64Image: string = '';
  public width: number = 0;
  public height: number = 0;

  constructor(data?: IRegistrationECertificateModel) {
    if (data != null) {
      this.id = data.id;
      this.userId = data.userId;
      this.pdfFileName = data.pdfFileName;
      this.base64Pdf = data.base64Pdf;
      this.base64Image = data.base64Image;
      this.width = data.width;
      this.height = data.height;
    }
  }
}
