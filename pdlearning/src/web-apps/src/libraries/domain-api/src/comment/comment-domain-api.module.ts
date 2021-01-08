import { ModuleWithProviders, NgModule } from '@angular/core';

import { CommentApiService } from './services/comment-api.services';
import { CommentComponentService } from './services/comment-component.service';
import { CommentRepository } from './repositories/comment.repository';
import { CommentRepositoryContext } from './comment-repository-context';

@NgModule({
  providers: [CommentApiService, CommentComponentService, CommentRepository]
})
export class CommentDomainApiModule {
  public static forRoot(): ModuleWithProviders {
    return {
      ngModule: CommentDomainApiModule,
      providers: [CommentRepositoryContext]
    };
  }
}
