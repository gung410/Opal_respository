export interface IECertificateModel {
  id: string;
  title: string;
  tagIds: string[];
  completionDate: Date;
}

export class ECertificateModel implements IECertificateModel {
  public id: string;
  public title: string;
  public tagIds: string[];
  public completionDate: Date;

  constructor(data?: IECertificateModel) {
    if (data) {
      this.id = data.id;
      this.title = data.title;
      this.tagIds = data.tagIds;
      this.completionDate = data.completionDate;
    }
  }
}
