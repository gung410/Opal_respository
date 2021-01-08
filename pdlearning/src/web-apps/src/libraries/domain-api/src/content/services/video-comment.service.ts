import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { IVideoComment, VideoComment } from '../models/video-comment.model';

import { IPagedResultDto } from '../../share/dtos/paged-result.dto';
import { IVideoCommentCreateRequest } from '../dtos/video-comment-create-request.model';
import { IVideoCommentSearchRequest } from '../dtos/video-comment-search-request.model';
import { IVideoCommentUpdateRequest } from '../dtos/video-comment-update-request.model';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';

@Injectable()
export class VideoCommentApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.contentApiUrl + '/content/videocomment';
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public getVideoComments(request: IVideoCommentSearchRequest, showSpinner: boolean = true): Promise<IPagedResultDto<VideoComment>> {
    return this.post<IVideoCommentSearchRequest, IPagedResultDto<IVideoComment>>(`/search`, request, showSpinner)
      .pipe(
        map(p => {
          const mockupResponse: IPagedResultDto<IVideoComment> = {
            items: p.items.map(_ => new VideoComment(_)),
            totalCount: p.totalCount
          };
          return mockupResponse;
        })
      )
      .toPromise();
  }

  public createVideoComment(request: IVideoCommentCreateRequest, showSpinner: boolean = true): Promise<VideoComment> {
    return this.post<IVideoCommentCreateRequest, IVideoComment>(`/create`, request, showSpinner)
      .pipe(map(p => new VideoComment(p)))
      .toPromise();
  }

  public updateVideoComment(request: IVideoCommentUpdateRequest, showSpinner: boolean = true): Promise<VideoComment> {
    return this.post<IVideoCommentUpdateRequest, IVideoComment>(`/update`, request, showSpinner)
      .pipe(map(p => new VideoComment(p)))
      .toPromise();
  }

  public deleteVideoComment(videoCommentId: string, showSpinner: boolean = true): Promise<void> {
    return this.delete<void>(`/${videoCommentId}`, showSpinner).toPromise();
  }
}
