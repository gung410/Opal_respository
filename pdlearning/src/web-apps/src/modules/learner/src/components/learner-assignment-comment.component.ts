import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService, TranslationMessage, Utils } from '@opal20/infrastructure';
import { CommentApiService, CommentComponentService, CommentViewModel } from '@opal20/domain-api';
import { Component, Input } from '@angular/core';
import { OpalDialogService, requiredIfValidator } from '@opal20/common-components';

import { CommentTabInput } from '@opal20/domain-components';
import { PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subscription } from 'rxjs';

@Component({
  selector: 'learner-assignment-comment',
  templateUrl: './learner-assignment-comment.component.html'
})
export class LearnerAssignmentCommentComponent extends BaseFormComponent {
  public get input(): CommentTabInput {
    return this._input;
  }

  @Input()
  public set input(v: CommentTabInput) {
    if (Utils.isDifferent(this._input, v) && v && v.originalObjectId) {
      this._input = v;
      this.commentApiService.initApiService(v.commentServiceType);
      this.loadComments(true, true);
    }
  }

  public comment: string = '';
  public requiredCommentField: boolean = true;
  public avatarUrl: string = '';
  public commentsVm: CommentViewModel[] = [];
  public total: number = 0;
  public pagingState: PageChangeEvent = {
    skip: 0,
    take: 6 // items per page
  };

  private _loadCommentSubscription: Subscription = new Subscription();
  private _input: CommentTabInput;

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    protected commentApiService: CommentApiService,
    protected commentComponentService: CommentComponentService,
    protected opalDialogService: OpalDialogService
  ) {
    super(moduleFacadeService);
    this.avatarUrl = (AppGlobal.user ? AppGlobal.user.avatarUrl : undefined) || './assets/images/others/default-avatar.png';
  }

  public onSubmit(): void {
    this.validate().then(valid => {
      if (valid) {
        const bookmarkTypeMessage = this.moduleFacadeService.translator.translate('Comment sent successfully');
        this.showNotification(bookmarkTypeMessage);
        this.saveComment(this.comment);
      }
    });
  }

  public saveComment(comment: string): Promise<void> {
    return this.commentApiService
      .saveComment(
        {
          content: comment,
          objectId: this.input.originalObjectId,
          entityCommentType: this.input.entityCommentType != null ? this.input.entityCommentType : null,
          commentNotification: this.input.commentNotification != null ? this.input.commentNotification : null
        },
        true
      )
      .then(() => {
        this.comment = '';
        this.resetForm(this.form);
        this.loadAfterCreatedComment();
      });
  }

  public loadComments(isInitial: boolean = false, showSpinner: boolean = false): void {
    // need to unsubscribe the older
    this._loadCommentSubscription.unsubscribe();
    this._loadCommentSubscription = this.commentComponentService
      .loadComments(
        this.input.originalObjectId,
        this.input.entityCommentType,
        null,
        this.pagingState.skip,
        this.pagingState.take,
        showSpinner
      )
      .pipe(this.untilDestroy())
      .subscribe(result => {
        if (!result || !result.data || !result.data.length) {
          return;
        }

        // Case: have only 1 comment, then another user reply it.
        // At the initialization, we need to replace the comments to avoid duplicated comments
        // Affter that: The new items will be appended into the comment list when we click on load more
        this.commentsVm = isInitial ? result.data.slice() : this.commentsVm.concat(result.data);
        this.total = result.total;
      });
  }

  public showMoreComment(): void {
    this.pagingState.skip += this.pagingState.take;
    this.loadComments(false, true);
  }

  public get isShowMore(): boolean {
    return this.commentsVm.length < this.total;
  }

  public get remainingComment(): number {
    return this.total - this.commentsVm.length;
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'form',
      controls: {
        comment: {
          defaultValue: null,
          validators: [
            {
              validator: requiredIfValidator(() => this.requiredCommentField),
              validatorType: 'required',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Please fill in your comment')
            }
          ]
        }
      }
    };
  }

  // After submit a comment, get the lastest comment and add to the beginning of comment list
  private loadAfterCreatedComment(): void {
    // need to unsubscribe the older
    this._loadCommentSubscription.unsubscribe();
    this._loadCommentSubscription = this.commentComponentService
      .loadComments(this.input.originalObjectId, this.input.entityCommentType, null, 0, this.pagingState.take)
      .pipe(this.untilDestroy())
      .subscribe(result => {
        if (!result || !result.data || !result.data.length) {
          return;
        }

        // new comment added before
        const newComment: CommentViewModel = result.data[0];

        const isCommentExisting = this.commentsVm.findIndex(comment => comment.id === newComment.id) > -1;
        if (isCommentExisting) {
          return;
        }

        // add new comment to the list comment
        this.commentsVm.unshift(newComment);
        // Need to change reference to update the UI
        this.commentsVm = [...this.commentsVm];
        // do not need to load the new comment in the next times
        this.pagingState.skip++;
        this.total = result.total;
      });
  }
}
