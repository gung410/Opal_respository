import { ECertificateTemplateParam, IECertificateTemplateParam } from '../models/ecertificate-template.model';

export interface IGetPreviewECertificateTemplateRequest {
  eCertificateLayoutId: string;
  params: IECertificateTemplateParam[];
}

export class GetPreviewECertificateTemplateRequest implements GetPreviewECertificateTemplateRequest {
  public eCertificateLayoutId: string;
  public params: IECertificateTemplateParam[] = [];

  constructor(data?: IGetPreviewECertificateTemplateRequest) {
    if (data) {
      this.eCertificateLayoutId = data.eCertificateLayoutId;
      this.params = data.params.map(p => new ECertificateTemplateParam(p));
    }
  }
}
