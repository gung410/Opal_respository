import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { IVideoChapter, VideoChapter } from '../models/video-chapter.model';

import { IVideoChapterSaveRequest } from '../dtos/video-chapter-save-request.model';
import { IVideoChapterSearchRequest } from '../dtos/video-chapter-search-request.model';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';

@Injectable()
export class VideoChapterApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.contentApiUrl + '/content/videochapter';
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public create(request: IVideoChapterSaveRequest, showSpinner: boolean = true): Promise<VideoChapter[]> {
    return this.post<IVideoChapterSaveRequest, IVideoChapter[]>('/create', request, showSpinner)
      .pipe(map(result => result.map(p => new VideoChapter(p)).sort((c1, c2) => c1.timeStart - c2.timeStart)))
      .toPromise();
  }

  public update(request: IVideoChapterSaveRequest, showSpinner: boolean = true): Promise<VideoChapter[]> {
    return this.post<IVideoChapterSaveRequest, IVideoChapter[]>('/update', request, showSpinner)
      .pipe(map(result => result.map(p => new VideoChapter(p)).sort((c1, c2) => c1.timeStart - c2.timeStart)))
      .toPromise();
  }

  public search(request: IVideoChapterSearchRequest, showSpinner: boolean = true): Promise<VideoChapter[]> {
    return this.post<IVideoChapterSearchRequest, IVideoChapter[]>('/search', request, showSpinner)
      .pipe(map(result => result.map(p => new VideoChapter(p)).sort((c1, c2) => c1.timeStart - c2.timeStart)))
      .toPromise();
  }
}
