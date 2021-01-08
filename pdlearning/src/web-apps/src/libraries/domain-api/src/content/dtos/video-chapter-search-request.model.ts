import { VideoChapterSourceType } from '../models/video-chapter.model';

export interface IVideoChapterSearchRequest {
  objectId: string;
  sourceType?: VideoChapterSourceType;
}
