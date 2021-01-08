import { VideoCommentSourceType } from '@opal20/domain-api';

export enum VideoAnnotationMode {
  View = 'View',
  Learn = 'Learn',
  Management = 'Management'
}

interface VideoAnnotationBasicInfo {
  objectId: string;
  originalObjectId?: string;
}

export interface VideoAnnotationChapterInfo extends VideoAnnotationBasicInfo {}

export interface VideoAnnotationCommentInfo extends VideoAnnotationBasicInfo {
  sourceType: VideoCommentSourceType;
}
