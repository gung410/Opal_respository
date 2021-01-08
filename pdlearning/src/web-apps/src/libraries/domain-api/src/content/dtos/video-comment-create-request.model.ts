import { VideoCommentSourceType } from '../models/video-comment.model';

export interface IVideoCommentCreateRequest {
  objectId?: string;
  originalObjectId?: string;
  sourceType: VideoCommentSourceType;
  content: string;
  videoId: string;
  videoTime: number;
}
