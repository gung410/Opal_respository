import {
  CopyrightCommonTermsOfUse,
  CopyrightLicenseTerritory,
  CopyrightLicenseType,
  CopyrightOwnership,
  IAttributionElement,
  ICopyright
} from '../../share/models/copyright';

import { DigitalContentStatus } from '../../share/models/digital-content-status.enum';
import { DigitalContentType } from '../models/digital-content-type.enum';

export interface IDigitalContentRequest extends ICopyright {
  id: string | undefined;
  title: string;
  description: string;
  type: DigitalContentType;
  status: DigitalContentStatus;

  ownership: CopyrightOwnership;
  licenseType: CopyrightLicenseType;
  termsOfUse: string | CopyrightCommonTermsOfUse;
  startDate: Date | undefined;
  expiredDate: Date | undefined;
  licenseTerritory: CopyrightLicenseTerritory;
  publisher: string | undefined;
  isAllowReusable: boolean;
  isAllowDownload: boolean;
  isAllowModification: boolean;
  acknowledgementAndCredit: string | undefined;
  remarks: string | undefined;
  attributionElements: IAttributionElement[];
  isSubmitForApproval: boolean;
  primaryApprovingOfficerId: string;
  alternativeApprovingOfficerId: string;
  isAutoSave: boolean | null | undefined;
  archiveDate: Date | undefined;
  autoPublishDate: Date | undefined;
  isAutoPublish: boolean;
}
