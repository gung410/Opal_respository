import { ECertificateTemplateModel, IECertificateTemplateModel } from './../models/ecertificate-template.model';

export interface ISearchECertificateResult {
  items: IECertificateTemplateModel[];
  totalCount: number;
}

export class SearchECertificateResult implements ISearchECertificateResult {
  public items: ECertificateTemplateModel[] = [];
  public totalCount: number = 0;

  constructor(data?: ISearchECertificateResult) {
    if (data == null) {
      return;
    }

    this.items = data.items.map(item => new ECertificateTemplateModel(item));
    this.totalCount = data.totalCount;
  }
}
