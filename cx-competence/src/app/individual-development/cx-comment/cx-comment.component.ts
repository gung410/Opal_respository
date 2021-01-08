import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import { CxConfirmationDialogComponent } from '@conexus/cx-angular-common';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';
import { User, UserBasicInfo } from 'app-models/auth.model';
import { UserService } from 'app-services/user.service';
import { Utilities } from 'app-utilities/utilities';
import { Constant } from 'app/shared/app.constant';
import { cloneDeep, isEmpty, remove, trim } from 'lodash';
import * as moment from 'moment';
import { ToastrService } from 'ngx-toastr';
import { AppConstant } from './../../shared/app.constant';
import { BaseComponent } from './../../shared/components/component.abstract';
import {
  CommentActionEnum,
  CommentChangeData,
  CommentData,
  CommentTagTypeEnum,
} from './comment.model';
declare var $: any;

@Component({
  selector: 'cx-comment',
  templateUrl: './cx-comment.component.html',
  styleUrls: ['./cx-comment.component.scss'],
})
export class CxCommentComponent
  extends BaseComponent
  implements OnInit, OnChanges {
  @Input() comments: CommentData[];
  /**
   * Set "True" if the logged-in user is allowed to post new comment.
   */
  @Input() canReply: boolean = true;
  /**
   * Set "True" if we want to prevent the logged-in user editing/deleting the existing comments.
   */
  @Input() blockExistingComments: boolean = false;
  @Input() approveText: string = 'ACKNOWLEDGED';
  @Input() rejectText: string = 'REJECTED';
  @Output()
  changeComment: EventEmitter<CommentChangeData> = new EventEmitter<CommentChangeData>();
  @ViewChild('textBox') textBox: ElementRef;

  commentText: string = '';
  currentUserId: number;
  showItemNoFromTop: number = 1;
  maxLength: number = Constant.LONG_TEXT_MAX_LENGTH;
  editMode: boolean = false;
  isFocusingTextBox: boolean = false;
  invalidLength: boolean = false;

  private currentUserInfo: UserBasicInfo;
  private viewMoreStep: number = 3;
  private currentEditItem: CommentData;
  private changeCommentErrorMsg: any = {
    [CommentActionEnum.ADD]: "Can't add comment, please try again",
    [CommentActionEnum.UPDATE]: "Can't update comment, please try again",
    [CommentActionEnum.DELETE]: "Can't delete comment, please try again",
  };

  constructor(
    changeDetectorRef: ChangeDetectorRef,
    private ngbModal: NgbModal,
    private toastr: ToastrService,
    private userService: UserService,
    private translateService: TranslateService
  ) {
    super(changeDetectorRef);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes && changes.comments && !isEmpty(this.comments)) {
      this.comments = this.comments.sort(this.sortCommentByDate);
    }
  }

  ngOnInit(): void {
    this.initData();
  }

  onClickReply(): void {
    if (!this.commentText) {
      return;
    }

    if (!this.validateCommentContent(this.commentText)) {
      return;
    }

    const trimmedText = trim(this.commentText);
    const commentItem = this.getCommentResultModel(trimmedText);

    if (commentItem) {
      const changeData = new CommentChangeData(
        commentItem,
        CommentActionEnum.ADD
      );
      this.isFocusingTextBox = false;
      this.changeComment.emit(changeData);
      this.commentText = '';
    } else {
      this.showToastrErrorBaseOnAction(CommentActionEnum.ADD);
    }
  }

  onClickLoadMore(): void {
    const expectValue = this.showItemNoFromTop + this.viewMoreStep;
    this.showItemNoFromTop = Math.min(expectValue, this.comments.length);
  }

  onClickEdit(commentItem: CommentData): void {
    this.currentEditItem = commentItem;
    this.commentText = commentItem.content;
    this.editMode = true;
    if (this.textBox && this.textBox.nativeElement) {
      const textBox = this.textBox.nativeElement;
      this.scrollViewToElement(textBox);
    }
  }

  onClickDelete(commentItem: CommentData): void {
    if (commentItem) {
      this.showConfirmDeleteDialog(commentItem);
    }
  }

  onClickSaveEdit(): void {
    if (!this.currentEditItem) {
      this.showToastrErrorBaseOnAction(CommentActionEnum.UPDATE);

      return;
    }

    if (!this.validateCommentContent(this.commentText)) {
      return;
    }
    this.isFocusingTextBox = true;
    const editedCommentItem = cloneDeep(this.currentEditItem);
    editedCommentItem.content = this.commentText;
    editedCommentItem.lastUpdated = Utilities.getCurrentDateISOString();
    editedCommentItem.owner.avatarUrl = this.currentUserInfo.avatarUrl;

    const changeData = new CommentChangeData(
      editedCommentItem,
      CommentActionEnum.UPDATE
    );
    this.changeComment.emit(changeData);
    this.clearEditMode();
  }

  onClickCancelEdit(): void {
    this.clearEditMode();
  }

  onFocus(): void {
    this.isFocusingTextBox = true;
  }

  onBlur(): void {
    this.isFocusingTextBox = isEmpty(this.commentText) ? false : true;
  }

  changeCommentResult(changeData: CommentChangeData): void {
    if (!changeData || !changeData.action || !changeData.commentItem) {
      return;
    }
    const commentItem = changeData.commentItem;
    const action = changeData.action;

    if (!changeData.changeResult) {
      // Error case
      this.showToastrErrorBaseOnAction(action);

      return;
    }

    // Success case
    switch (action) {
      case CommentActionEnum.ADD:
        if (!this.comments) {
          this.comments = [];
        }
        this.comments.unshift(commentItem);
        this.showItemNoFromTop++;
        break;
      case CommentActionEnum.UPDATE:
        const oldItem = this.comments.find(
          (comment) => comment.id === commentItem.id
        );
        if (oldItem) {
          oldItem.content = commentItem.content;
          oldItem.owner.avatarUrl = commentItem.owner.avatarUrl;
          oldItem.lastUpdated = commentItem.lastUpdated;
        }
        break;
      case CommentActionEnum.DELETE:
        remove(
          this.comments,
          (existingComment: CommentData) =>
            existingComment.id === commentItem.id
        );
        break;
      default:
        break;
    }

    this.changeDetectorRef.detectChanges();
  }

  checkShowItemByIndex(index: number): boolean {
    return index < this.showItemNoFromTop;
  }

  getAvatar(avatarUrl: string): string {
    return this.getUserImage(
      new User({ avatarUrl: avatarUrl ? avatarUrl : AppConstant.defaultAvatar })
    );
  }

  getTimeFromNow(date: string): string {
    return moment(date).fromNow();
  }

  isTagApproved(tag: CommentTagTypeEnum): boolean {
    return tag === CommentTagTypeEnum.Approval;
  }

  isTagRejected(tag: CommentTagTypeEnum): boolean {
    return tag === CommentTagTypeEnum.Rejection;
  }

  get isCommentTextEmpty(): boolean {
    const trimmedText = trim(this.commentText);

    return isEmpty(trimmedText);
  }

  get canLoadMore(): boolean {
    return this.showItemNoFromTop < this.comments.length;
  }

  get commentTextModel(): string {
    return this.commentText;
  }

  set commentTextModel(value: string) {
    this.commentText = value;
    this.invalidLength = this.commentText
      ? this.commentText.length > this.maxLength
      : false;
  }

  private initData(): void {
    // Get current user info
    this.currentUserInfo = this.userService.getCurrentUserBasicInfo();
    if (this.currentUserInfo && this.currentUserInfo.identity) {
      this.currentUserId = this.currentUserInfo.identity.id;
    }
  }

  private getCommentResultModel(commentText: string): CommentData {
    const owner = this.userService.getCurrentUserBasicInfo();

    if (!owner) {
      return;
    }

    const now = new Date().toISOString();
    const result = new CommentData({
      content: commentText,
      created: now,
      owner,
    });

    return result;
  }

  private clearEditMode(): void {
    this.isFocusingTextBox = false;
    this.currentEditItem = undefined;
    this.commentText = '';
    this.editMode = false;
  }

  private showConfirmDeleteDialog(commentItem: CommentData): void {
    const modalRef = this.ngbModal.open(CxConfirmationDialogComponent, {
      size: 'sm',
      centered: true,
    });

    const modalComponent = modalRef.componentInstance as CxConfirmationDialogComponent;
    modalComponent.cancelButtonText = this.translateService.instant(
      'Odp.ConfirmationDialog.No'
    ) as string;
    modalComponent.confirmButtonText = this.translateService.instant(
      'Odp.ConfirmationDialog.Confirm'
    ) as string;
    modalComponent.header = this.translateService.instant(
      'Odp.ConfirmationDialog.Header'
    ) as string;
    modalComponent.content = this.translateService.instant(
      'Common.Comment.DeleteConfirm'
    ) as string;
    modalComponent.cancel.subscribe(() => {
      modalRef.close();
    });
    modalComponent.confirm.subscribe(() => {
      const changeData = new CommentChangeData(
        commentItem,
        CommentActionEnum.DELETE
      );
      if (!!this.currentEditItem) {
        this.clearEditMode();
      }
      this.changeComment.emit(changeData);
      modalRef.close();
    });
  }

  private showToastrErrorBaseOnAction(action: CommentActionEnum): void {
    const msg = this.changeCommentErrorMsg[action] as string;
    this.toastr.error(msg);
  }

  private validateCommentContent(content: string): boolean {
    const isValid = content.length <= this.maxLength;
    if (!isValid) {
      this.toastr.error(`Please input less than ${this.maxLength} characters`);
    }

    return isValid;
  }

  private scrollViewToElement(element: any): void {
    if (!element) {
      return;
    }

    const parentElementScrollable = this.getParentElementScrollable(element);
    if (!parentElementScrollable) {
      return;
    }

    const jqueryElement = $(element);
    const offsetTop: number = jqueryElement.offset().top;
    $(parentElementScrollable).animate(
      {
        scrollTop: offsetTop - 300,
      },
      undefined,
      () => {
        element.focus();
      }
    );
  }

  private getParentElementScrollable(node: any): any {
    if (node == null) {
      return null;
    }

    const parentNode = node.parentNode;
    if (this.isElementScrollable(parentNode)) {
      return parentNode;
    }

    return this.getParentElementScrollable(parentNode);
  }

  private isElementScrollable(node: any): boolean {
    if (!node || !node.style) {
      return false;
    }

    return (
      node.style.overflowY === 'scroll' ||
      node.style.overflowY === 'auto' ||
      node.tagName === 'HTML'
    );
  }

  private sortCommentByDate(
    comment1: CommentData,
    comment2: CommentData
  ): number {
    const date1 = new Date(comment1.created);
    const date2 = new Date(comment2.created);
    if (date1 < date2) {
      return 1;
    }
    if (date1 > date2) {
      return -1;
    }

    return 0;
  }
}
