import { Directive, Host, OnDestroy, OnInit } from '@angular/core';

import { DialogRef } from '@progress/kendo-angular-dialog';
import { EditorComponent } from '@progress/kendo-angular-editor';
import { ModuleFacadeService } from '@opal20/infrastructure';
import { Subscription } from 'rxjs';
import { ToolBarButtonComponent } from '@progress/kendo-angular-toolbar';
import { UploadImageDialogComponent } from '../components/upload-image-dialog/upload-image-dialog.component';

enum ToolbarIcon {
  Image = 'image',
  Text = 'Insert image',
  Title = 'Insert image',
  ShowText = 'overflow'
}

enum EditorCmd {
  InserImage = 'insertImage'
}

@Directive({
  selector: 'kendo-toolbar-button[kendoEditorCustomInsertImageButton]'
})
export class KendoEditorCustomInsertImageButtonDirective implements OnInit, OnDestroy {
  private subs: Subscription[] = [];

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private button: ToolBarButtonComponent,
    @Host() private editor: EditorComponent
  ) {}

  public ngOnInit(): void {
    this.button.icon = ToolbarIcon.Image;
    this.button.showText = ToolbarIcon.ShowText;
    this.button.text = ToolbarIcon.Text;
    this.button.title = ToolbarIcon.Title;

    this.subs.push(
      this.button.click.subscribe(() => {
        const dialogRef: DialogRef = this.moduleFacadeService.dialogService.open({ content: UploadImageDialogComponent });
        dialogRef.result.toPromise().then((url: string) => {
          if (url && typeof url === 'string') {
            this.editor.exec(EditorCmd.InserImage, { src: url, style: 'max-width: 100%' });
          }
        });
      })
    );
  }

  public ngOnDestroy(): void {
    this.subs.forEach(sub => sub.unsubscribe());
  }
}
