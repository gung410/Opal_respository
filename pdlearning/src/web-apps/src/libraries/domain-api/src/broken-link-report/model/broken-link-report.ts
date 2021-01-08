export interface IBrokenLinkReport {
  id: string;
  objectId: string;
  originalObjectId: string;
  reportBy: string;
  isSystemReport: boolean;
  url: string;
  description: string;
  createdDate: string;
}

export class BrokenLinkReport implements IBrokenLinkReport {
  public id: string;
  public objectId: string;
  public originalObjectId: string;
  public reportBy: string;
  public isSystemReport: boolean;
  public url: string;
  public description: string;
  public createdDate: string;
  constructor(data: IBrokenLinkReport) {
    if (!data) {
      return;
    }
    this.id = data.id;
    this.objectId = data.objectId;
    this.originalObjectId = data.originalObjectId;
    this.reportBy = data.reportBy;
    this.isSystemReport = data.isSystemReport;
    this.url = data.url;
    this.description = data.description;
    this.createdDate = data.createdDate;
  }
}
