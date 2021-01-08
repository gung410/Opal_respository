export interface IPreviewECertificateTemplateModel {
  base64ECertificateTemplate: string;
  width: number;
  height: number;
}

export class PreviewECertificateTemplateModel implements IPreviewECertificateTemplateModel {
  public base64ECertificateTemplate: string = '';
  public width: number = 1056;
  public height: number = 816;

  constructor(data?: PreviewECertificateTemplateModel) {
    if (data != null) {
      this.base64ECertificateTemplate = data.base64ECertificateTemplate;
      this.width = data.width;
      this.height = data.height;
    }
  }
}
