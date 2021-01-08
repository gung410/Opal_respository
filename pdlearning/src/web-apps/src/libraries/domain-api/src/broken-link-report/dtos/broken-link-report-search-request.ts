import { BrokenLinkContentType } from '../model/broken-link-content-type';
import { IGetParams } from '@opal20/infrastructure';

export interface IPagedInfo extends IGetParams {
  skipCount: number;
  maxResultCount: number;
}

export interface IBrokenLinkReportSearchRequest extends IGetParams {
  originalObjectId?: string;
  objectId?: string;
  module?: BrokenLinkModuleIdentifier;
  contentType?: BrokenLinkContentType;
  userId?: string;
  parentIds?: string[];
  pagedInfo: IPagedInfo;
}

export enum BrokenLinkModuleIdentifier {
  Unknown = 'Unknown',
  Course = 'Course',
  Content = 'Content',
  Form = 'Form',
  LnaForm = 'LnaForm',
  Learner = 'Learner',
  Community = 'Community'
}
