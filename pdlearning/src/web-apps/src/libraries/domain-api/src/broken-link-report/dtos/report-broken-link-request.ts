import { BrokenLinkContentType } from '../model/broken-link-content-type';
import { IGetParams } from '@opal20/infrastructure';

export interface IReportBrokenLinkRequest extends IGetParams {
  objectId: string;
  url: string;
  description: string;
  originalObjectId: string;
  parentId: string;
  objectDetailUrl: string;
  objectOwnerId: string;
  objectTitle: string;
  objectOwnerName: string;
  reporterName: string;
  contentType: BrokenLinkContentType;
}

export interface ICheckBrokenLinkRequest {
  url: string;
}
