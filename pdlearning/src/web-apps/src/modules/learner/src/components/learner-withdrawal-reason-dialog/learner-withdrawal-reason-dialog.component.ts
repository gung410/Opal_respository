import { BaseComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { CommentServiceType, EntityCommentType } from '@opal20/domain-api';
import { Component, Input } from '@angular/core';

import { CommentTabInput } from '@opal20/domain-components';
import { DialogAction } from '@opal20/common-components';
import { DialogRef } from '@progress/kendo-angular-dialog';

@Component({
  selector: 'learner-withdrawal-reason-dialog',
  templateUrl: './learner-withdrawal-reason-dialog.component.html'
})
export class LearnerWithdrawalReasonDialog extends BaseComponent {
  @Input() public set registrationId(value: string) {
    if (!Utils.isDifferent(value, this._registrationId)) {
      return;
    }
    this._registrationId = value;
    this.commentTabInput = {
      originalObjectId: this._registrationId,
      commentServiceType: CommentServiceType.Course,
      entityCommentType: EntityCommentType.Registration,
      actionType: 'withdrawn',
      hasReply: false,
      orderByDescendingDate: true
    };
  }
  public get registrationId(): string {
    return this._registrationId;
  }
  public commentTabInput: CommentTabInput;

  private _registrationId: string;
  constructor(public moduleFacadeService: ModuleFacadeService, public dialogRef: DialogRef) {
    super(moduleFacadeService);
  }

  public onClose(): void {
    this.dialogRef.close({ action: DialogAction.Close });
  }
}
