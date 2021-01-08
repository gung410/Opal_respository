import { IPagedResultRequestDto } from '../../share/dtos/paged-request.dto';
import { VideoCommentSourceType } from '../models/video-comment.model';

export interface IVideoCommentSearchRequest extends IPagedResultRequestDto {
  objectId?: string;
  originalObjectId?: string;
  sourceType: VideoCommentSourceType;
  videoId: string;
  orderBy: VideoCommentOrderBy;
  orderType: 'ASC' | 'DESC';
}

export enum VideoCommentOrderBy {
  CreatedDate = 'CreatedDate',
  VideoTime = 'VideoTime'
}
