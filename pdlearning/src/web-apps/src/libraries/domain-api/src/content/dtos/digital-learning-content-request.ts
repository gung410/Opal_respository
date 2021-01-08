import { CopyrightLicenseTerritory, CopyrightLicenseType, CopyrightOwnership, IAttributionElement } from '../../share/models/copyright';

import { DigitalContentStatus } from '../../share/models/digital-content-status.enum';
import { DigitalContentType } from '../models/digital-content-type.enum';
import { IDigitalContent } from '../models/digital-content';
import { IDigitalContentRequest } from './digital-content-request';

export class DigitalLearningContentRequest implements IDigitalContentRequest {
  public id: string;
  public title: string;
  public description: string;
  public status: DigitalContentStatus;
  public type: DigitalContentType = DigitalContentType.LearningContent;
  public htmlContent: string;
  public ownership: CopyrightOwnership;
  public licenseType: CopyrightLicenseType;
  public termsOfUse: string;
  public startDate: Date;
  public expiredDate: Date;
  public licenseTerritory: CopyrightLicenseTerritory;
  public publisher: string;
  public isAllowReusable: boolean;
  public isAllowDownload: boolean;
  public isAllowModification: boolean;
  public acknowledgementAndCredit: string;
  public remarks: string;
  public attributionElements: IAttributionElement[] = [];
  public isSubmitForApproval: boolean;
  public primaryApprovingOfficerId: string;
  public alternativeApprovingOfficerId: string;
  public isAutoSave: boolean;
  public archiveDate: Date;
  public autoPublishDate: Date;
  public isAutoPublish: boolean;

  constructor(learningContent: IDigitalContent, isAutoSaveRequest: boolean = false) {
    this.id = learningContent.id;
    this.title = learningContent.title;
    this.description = learningContent.description;
    this.status = DigitalContentStatus[learningContent.status];
    this.htmlContent = learningContent.htmlContent;

    this.ownership = learningContent.ownership;
    this.licenseType = learningContent.licenseType;
    this.termsOfUse = learningContent.termsOfUse;
    this.startDate = learningContent.startDate ? new Date(learningContent.startDate) : undefined;
    this.expiredDate = learningContent.expiredDate ? new Date(learningContent.expiredDate) : undefined;
    this.licenseTerritory = learningContent.licenseTerritory;
    this.publisher = learningContent.publisher;
    this.isAllowReusable = learningContent.isAllowReusable;
    this.isAllowDownload = learningContent.isAllowDownload;
    this.isAllowModification = learningContent.isAllowModification;
    this.acknowledgementAndCredit = learningContent.acknowledgementAndCredit;
    this.remarks = learningContent.remarks;
    this.attributionElements = learningContent.attributionElements;
    this.isSubmitForApproval = learningContent.isSubmitForApproval;
    this.primaryApprovingOfficerId = learningContent.primaryApprovingOfficerId;
    this.alternativeApprovingOfficerId = learningContent.alternativeApprovingOfficerId;
    this.archiveDate = learningContent.archiveDate;
    this.autoPublishDate = learningContent.autoPublishDate;
    this.isAutoPublish = learningContent.isAutoPublish;

    // Autosave
    this.isAutoSave = isAutoSaveRequest;
  }
}
