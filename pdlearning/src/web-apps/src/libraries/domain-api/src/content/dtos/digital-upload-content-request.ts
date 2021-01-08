import {
  CopyrightCommonTermsOfUse,
  CopyrightLicenseTerritory,
  CopyrightLicenseType,
  CopyrightOwnership,
  IAttributionElement
} from '../../share/models/copyright';

import { DigitalContentStatus } from '../../share/models/digital-content-status.enum';
import { DigitalContentType } from '../models/digital-content-type.enum';
import { IDigitalContent } from '../models/digital-content';
import { IDigitalContentRequest } from './digital-content-request';
import { IVideoChapter } from '../models/video-chapter.model';

export class DigitalUploadContentRequest implements IDigitalContentRequest {
  public attributionElements: IAttributionElement[];
  public id: string | undefined;
  public title: string;
  public description: string;
  public type: DigitalContentType = DigitalContentType.UploadedContent;
  public status: DigitalContentStatus;

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
  public isSubmitForApproval: boolean;

  public fileName: string;
  public fileType: string;
  public fileExtension: string;
  public fileSize: number;
  public fileLocation: string;
  public primaryApprovingOfficerId: string;
  public alternativeApprovingOfficerId: string;
  public isAutoSave: boolean;
  public archiveDate: Date;
  public autoPublishDate: Date;
  public isAutoPublish: boolean;

  public fileDuration?: number;
  public chapters?: IVideoChapter[];

  constructor(uploadContent: IDigitalContent, isAutoSaveRequest: boolean = false) {
    this.id = uploadContent.id;
    this.title = uploadContent.title;
    this.description = uploadContent.description;
    this.status = DigitalContentStatus[uploadContent.status];
    this.fileName = uploadContent.fileName;
    this.fileExtension = uploadContent.fileExtension;
    this.fileSize = uploadContent.fileSize;
    this.fileType = uploadContent.fileType;
    this.fileLocation = uploadContent.fileLocation;
    this.fileDuration = uploadContent.fileDuration;
    this.isSubmitForApproval = uploadContent.isSubmitForApproval;

    // Copyrights
    this.ownership = uploadContent.ownership;
    this.licenseType = uploadContent.licenseType;
    this.termsOfUse = uploadContent.termsOfUse;
    this.startDate = uploadContent.startDate ? new Date(uploadContent.startDate) : undefined;
    this.expiredDate = uploadContent.expiredDate ? new Date(uploadContent.expiredDate) : undefined;
    this.licenseTerritory = uploadContent.licenseTerritory;
    this.publisher = uploadContent.publisher;
    this.isAllowReusable = uploadContent.isAllowReusable;
    this.isAllowDownload = uploadContent.isAllowDownload;
    this.isAllowModification = uploadContent.isAllowModification;
    this.acknowledgementAndCredit = uploadContent.acknowledgementAndCredit;
    this.remarks = uploadContent.remarks;
    this.attributionElements = uploadContent.attributionElements;
    this.primaryApprovingOfficerId = uploadContent.primaryApprovingOfficerId;
    this.alternativeApprovingOfficerId = uploadContent.alternativeApprovingOfficerId;
    this.archiveDate = uploadContent.archiveDate;
    this.autoPublishDate = uploadContent.autoPublishDate;
    this.isAutoPublish = uploadContent.isAutoPublish;

    // Autosave
    this.isAutoSave = isAutoSaveRequest;

    // Chapter
    this.chapters = uploadContent.chapters;
  }
}
