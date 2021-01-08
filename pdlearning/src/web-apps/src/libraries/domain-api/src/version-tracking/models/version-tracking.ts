export interface IVersionTracking {
  id: string;
  objectType: VersionTrackingObjectType;
  originalObjectId: string;
  canRollback: boolean;
  revertObjectId?: string;
  changedByUserId: string;
  majorVersion: number;
  minorVersion: number;
  comment?: string;
  createdDate: string;

  versionString?: string;
  isDisable?: boolean;
}

export class VersionTracking implements IVersionTracking {
  public id: string;
  public objectType: VersionTrackingObjectType;
  public originalObjectId: string;
  public canRollback: boolean;
  public revertObjectId?: string;
  public changedByUserId: string;
  public majorVersion: number;
  public minorVersion: number;
  public comment?: string;
  public createdDate: string;

  public isDisable: boolean = false;
  public get versionString(): string {
    return 'v' + this.majorVersion + '.' + this.minorVersion;
  }

  constructor(data?: IVersionTracking) {
    if (data == null) {
      return;
    }
    this.id = data.id;
    this.objectType = data.objectType;
    this.originalObjectId = data.originalObjectId;
    this.canRollback = data.canRollback;
    this.revertObjectId = data.revertObjectId;
    this.changedByUserId = data.changedByUserId;
    this.majorVersion = data.majorVersion;
    this.minorVersion = data.minorVersion;
    this.comment = data.comment;
    this.createdDate = data.createdDate;
  }
}

export enum VersionTrackingObjectType {
  Form = 'Form',
  DigitalContent = 'DigitalContent'
}
