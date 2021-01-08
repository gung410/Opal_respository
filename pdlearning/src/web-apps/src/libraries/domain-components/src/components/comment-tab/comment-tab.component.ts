import { BaseComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Comment, CommentApiService, CommentComponentService } from '@opal20/domain-api';
import { Component, Input, ViewEncapsulation } from '@angular/core';
import { GridDataResult, PageChangeEvent, RowClassArgs } from '@progress/kendo-angular-grid';

import { CommentTabInput } from './../../models/comment-tab-input.model';

@Component({
  selector: 'comment-tab',
  templateUrl: './comment-tab.component.html',
  encapsulation: ViewEncapsulation.None
})
export class CommentTabComponent extends BaseComponent {
  public get input(): CommentTabInput {
    return this._input;
  }

  @Input()
  public set input(v: CommentTabInput) {
    if (Utils.isDifferent(this._input, v) && v && v.originalObjectId) {
      this._input = v;
      this.commentApiService.initApiService(v.commentServiceType);
      this.loadComments(false);
    }
  }

  public gridView: GridDataResult;
  public comment: string = '';
  public state: PageChangeEvent = {
    skip: 0,
    take: 10
  };

  public get showLoadMore(): boolean {
    return this.gridView != null && this.gridView.total > (this.state.skip + 1) * this.state.take;
  }

  public get avatarUrl(): string {
    return (AppGlobal.user ? AppGlobal.user.avatarUrl : null) || './assets/images/others/default-avatar.png';
  }

  public get disableButton(): boolean {
    return Utils.isNullOrEmpty(this.comment) || this.input.mode === 'view';
  }

  private _input: CommentTabInput;

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    protected commentApiService: CommentApiService,
    protected commentComponentService: CommentComponentService
  ) {
    super(moduleFacadeService);
  }

  public onShowMore(): void {
    this.state.skip++;
    this.loadComments();
  }

  public saveComment(): void {
    this.commentApiService
      .saveComment(
        {
          content: this.comment,
          objectId: this._input.originalObjectId,
          entityCommentType: this._input.entityCommentType != null ? this._input.entityCommentType : null,
          commentNotification: this._input.commentNotification != null ? this._input.commentNotification : null
        },
        true
      )
      .then(() => {
        this.loadComments(true);
        this.comment = '';
      });
  }

  public onSubmitComment(): void {
    this.saveComment();
  }

  public rowCommentCallback(context: RowClassArgs): object {
    const isEven = context.index % 2 === 0;
    return {
      'comment-row even': isEven,
      'comment-row odd': !isEven
    };
  }

  public loadComments(showSpinner: boolean = false): void {
    this.commentComponentService
      .loadComments(
        this._input.originalObjectId,
        this._input.entityCommentType,
        this._input.actionType,
        0,
        (this.state.skip + 1) * this.state.take,
        showSpinner
      )
      .subscribe(result => {
        if (result != null && result.data != null && !this.input.orderByDescendingDate) {
          result.data.reverse();
        }
        this.gridView = result;
      });
  }

  public displayActionField(dataItem: Comment): boolean {
    return dataItem.action && this._input.mappingAction != null && this._input.mappingAction[dataItem.action] != null;
  }
}
