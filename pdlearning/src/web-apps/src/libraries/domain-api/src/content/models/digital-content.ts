import {
  CopyrightCommonTermsOfUse,
  CopyrightLicenseTerritory,
  CopyrightLicenseType,
  CopyrightOwnership,
  IAttributionElement,
  ICopyright
} from '../../share/models/copyright';
import { IVideoChapter, VideoChapter } from './video-chapter.model';

import { DigitalContentStatus } from '../../share/models/digital-content-status.enum';
import { DigitalContentType } from './digital-content-type.enum';
import { IValidatorDefinition } from '@opal20/infrastructure';
import { Validators } from '@angular/forms';

export interface IDigitalContent extends ICopyright {
  id?: string;
  originalObjectId?: string;
  title?: string;
  description?: string;
  type?: string;
  status?: DigitalContentStatus;
  createdDate?: string;
  changedDate?: string;
  htmlContent?: string;
  fileId?: string;
  fileName?: string;
  fileType?: string;
  fileExtension?: string;
  fileSize?: number;
  fileLocation?: string;
  fileDuration?: number;
  ownerId?: string;
  externalId?: string;
  isSubmitForApproval?: boolean;
  primaryApprovingOfficerId?: string;
  alternativeApprovingOfficerId?: string;
  archiveDate?: Date;
  archivedBy?: string;
  isArchived?: boolean;
  chapters?: IVideoChapter[];
  autoPublishDate?: Date;
  isAutoPublish?: boolean;
}

export class DigitalContent implements IDigitalContent {
  public static titleValidators: IValidatorDefinition[] = [
    { validator: Validators.required, validatorType: 'required' },
    { validator: Validators.maxLength(255), validatorType: 'maxLength' }
  ];

  public static optionalProps: (keyof DigitalContent)[] = [];

  public id?: string;
  public originalObjectId?: string;
  public title?: string;
  public description?: string;
  public type?: string;
  public status?: DigitalContentStatus;
  public createdDate?: string;
  public changedDate?: string;
  public htmlContent?: string;
  public fileId?: string;
  public fileName?: string;
  public fileType?: string;
  public fileExtension?: string;
  public fileSize?: number;
  public fileLocation?: string;
  public fileDuration?: number;
  public ownerId?: string;
  public externalId?: string;

  public ownership: CopyrightOwnership;
  public licenseType: CopyrightLicenseType;
  public termsOfUse: string | CopyrightCommonTermsOfUse;
  public startDate: Date | undefined;
  public expiredDate: Date | undefined;
  public licenseTerritory: CopyrightLicenseTerritory;
  public publisher: string | undefined;
  public isAllowReusable: boolean;
  public isAllowDownload: boolean;
  public isAllowModification: boolean;
  public acknowledgementAndCredit: string | undefined;
  public remarks: string | undefined;
  public attributionElements: IAttributionElement[];
  public primaryApprovingOfficerId?: string | undefined;
  public alternativeApprovingOfficerId?: string | undefined;

  public archiveDate?: Date;
  public archivedBy?: string;
  public isArchived?: boolean;
  public autoPublishDate?: Date;
  public isAutoPublish?: boolean;

  public chapters?: VideoChapter[];

  constructor(data?: IDigitalContent) {
    if (data == null) {
      return;
    }
    this.id = data.id;
    this.originalObjectId = data.originalObjectId;
    this.title = data.title;
    this.description = data.description;
    this.type = data.type;
    this.status = data.status;
    this.expiredDate = data.expiredDate;
    this.createdDate = data.createdDate;
    this.changedDate = data.changedDate;
    this.htmlContent = data.htmlContent;
    this.fileId = data.fileId;
    this.fileName = data.fileName;
    this.fileType = data.fileType;
    this.fileExtension = data.fileExtension;
    this.fileSize = data.fileSize;
    this.fileLocation = data.fileLocation;
    this.ownerId = data.ownerId;
    this.externalId = data.externalId;
    this.primaryApprovingOfficerId = data.primaryApprovingOfficerId;
    this.alternativeApprovingOfficerId = data.alternativeApprovingOfficerId;
    this.ownership = data.ownership;
    this.licenseType = data.licenseType;
    this.termsOfUse = data.termsOfUse;
    this.licenseTerritory = data.licenseTerritory;
    this.publisher = data.publisher;
    this.archiveDate = data.archiveDate ? new Date(data.archiveDate) : null;
    this.archivedBy = data.archivedBy;
    this.isAllowDownload = data.isAllowDownload;
    this.isArchived = data.isArchived;
    this.fileDuration = data.fileDuration;
    this.autoPublishDate = data.autoPublishDate ? new Date(data.autoPublishDate) : null;
    this.isAutoPublish = data.isAutoPublish;
    this.initChapters(data);
  }

  public getVideoId(): string {
    const regex = /^(.+\/)(.+)\.(.+)$/;
    const matches = this.fileLocation.match(regex);
    if (!matches || !matches[2]) {
      return;
    }
    return matches[2];
  }

  private initChapters(data: IDigitalContent): void {
    if (data.type !== DigitalContentType.UploadedContent) {
      this.chapters = null;
      return;
    }
    this.chapters = data.chapters ? data.chapters.map(p => new VideoChapter(p)).sort((c1, c2) => c1.timeStart - c2.timeStart) : [];
  }
}
