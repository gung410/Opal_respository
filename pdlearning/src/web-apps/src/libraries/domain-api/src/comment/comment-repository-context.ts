import { BaseRepositoryContext } from '@opal20/infrastructure';
import { BehaviorSubject } from 'rxjs';
import { Comment } from './models/comment';
import { Injectable } from '@angular/core';

@Injectable()
export class CommentRepositoryContext extends BaseRepositoryContext {
  public contentsSubject: BehaviorSubject<Dictionary<Comment>> = new BehaviorSubject({});
}
