import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService } from '@opal20/infrastructure';
import {
  CommentApiService,
  CommentServiceType,
  EntityCommentType,
  IComment,
  ICreateCommentRequest,
  ISearchCommentRequest
} from '@opal20/domain-api';
import { Component, EventEmitter, Input, Output } from '@angular/core';

import { Validators } from '@angular/forms';

@Component({
  selector: 'learning-comment',
  templateUrl: './learning-comment.component.html'
})
export class LearningCommentComponent extends BaseFormComponent {
  @Input() public learningId: string;
  @Input() public learningTitle: string;
  @Input() public classRunId: string;
  @Output() public submit: EventEmitter<IComment> = new EventEmitter<IComment>();
  public comment: IComment;
  public rating: number;
  public reviewComment: string;

  public displayMode: boolean;
  public searchCommentRequest: ISearchCommentRequest = {
    objectId: this.learningId,
    pagedInfo: {
      skipCount: 0,
      maxResultCount: 10
    }
  };
  constructor(protected moduleFacadeService: ModuleFacadeService, private commentApiService: CommentApiService) {
    super(moduleFacadeService);
    this.commentApiService.initApiService(CommentServiceType.Course);
  }

  public onInit(): void {
    this.loadMyComment();
  }

  public onSubmit(): void {
    this.validate().then(val => {
      if (!val) {
        return;
      }
      if (!this.comment) {
        this.submitMyComment();
        return;
      }
      this.submit.emit();
    });
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    const formBuilder: IFormBuilderDefinition = {
      formName: 'form',
      controls: {}
    };
    const requiredValidator = { validators: [{ validator: Validators.required }] };

    formBuilder.controls.comment = {};
    formBuilder.controls.comment = requiredValidator;
    return formBuilder;
  }

  protected ignoreValidateForm(): boolean {
    return this.displayMode;
  }

  private loadMyComment(): void {
    this.commentApiService.searchComments(this.searchCommentRequest).then(resp => {
      const _comment = resp.items.pop();
      if (_comment) {
        this.comment = _comment;
      }
    });
  }

  private submitMyComment(): void {
    const commentRequest: ICreateCommentRequest = {
      content: this.reviewComment,
      objectId: this.learningId,
      entityCommentType: EntityCommentType.CourseContent
    };
    this.commentApiService.saveComment(commentRequest).then(resp => {
      this.submit.emit(resp);
    });
  }
}
