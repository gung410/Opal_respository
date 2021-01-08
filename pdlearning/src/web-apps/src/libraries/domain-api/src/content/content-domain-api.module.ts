import { ModuleWithProviders, NgModule } from '@angular/core';

import { ContentApiService } from './services/content-api.service';
import { ContentRepository } from './repositories/content.repository';
import { ContentRepositoryContext } from './content-repository-context';
import { VideoChapterApiService } from './services/video-chapter.service';
import { VideoCommentApiService } from './services/video-comment.service';

@NgModule({
  providers: [ContentApiService, ContentRepository, VideoCommentApiService, VideoChapterApiService]
})
export class ContentDomainApiModule {
  public static forRoot(): ModuleWithProviders {
    return {
      ngModule: ContentDomainApiModule,
      providers: [ContentRepositoryContext]
    };
  }
}
