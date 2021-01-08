import { BaseFormComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'comment-dialog-template',
  templateUrl: './comment-dialog-template.component.html'
})
export class CommentDialogTemplateComponent extends BaseFormComponent {
  @Input()
  public dialogTitle: string;

  @Input()
  public btnSaveText: string;

  @Input()
  public requireComment: boolean = true;

  @Output()
  public onSaveClick: EventEmitter<string> = new EventEmitter<string>();

  @Output()
  public onCancelClick: EventEmitter<boolean> = new EventEmitter<boolean>();

  public commentContent: string;

  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public cancel(): void {
    this.onCancelClick.emit(true);
  }

  public save(): void {
    this.onSaveClick.emit(this.commentContent);
  }
}
