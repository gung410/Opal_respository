import { Directive, Host, OnDestroy, OnInit } from '@angular/core';

import { DialogRef } from '@progress/kendo-angular-dialog';
import { EditorComponent } from '@progress/kendo-angular-editor';
import { ModuleFacadeService } from '@opal20/infrastructure';
import { Subscription } from 'rxjs';
import { ToolBarButtonComponent } from '@progress/kendo-angular-toolbar';
import { UploadFileDialogComponent } from '../components/upload-file-dialog/upload-file-dialog.component';

enum ToolbarIcon {
  Image = 'assets/images/icons/files/file-upload.svg',
  Text = 'Insert File',
  Title = 'Insert File',
  ShowText = 'overflow'
}

@Directive({
  selector: 'kendo-toolbar-button[kendoEditorCustomInsertFileButton]'
})
export class KendoEditorCustomInsertFileButtonDirective implements OnInit, OnDestroy {
  private subs: Subscription[] = [];

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private button: ToolBarButtonComponent,
    @Host() private editor: EditorComponent
  ) {}

  public ngOnInit(): void {
    this.button.imageUrl = ToolbarIcon.Image;
    this.button.showText = ToolbarIcon.ShowText;
    this.button.text = ToolbarIcon.Text;
    this.button.title = ToolbarIcon.Title;

    this.subs.push(
      this.button.click.subscribe(() => {
        const dialogRef: DialogRef = this.moduleFacadeService.dialogService.open({ content: UploadFileDialogComponent });
        const configurationPopup = dialogRef.content.instance as UploadFileDialogComponent;

        configurationPopup.canRunValidation = false;

        dialogRef.result.toPromise().then((html: string) => {
          if (html && typeof html === 'string') {
            this.editor.exec('insertText', { text: '[insertHTML]' });
            const src = this.editor.value.replace('<p>[insertHTML]</p>', `${html}<p></p>`).replace('[insertHTML]', `</p>${html}<p>`);
            this.editor.exec('setHTML', src);
          }
        });
      })
    );
  }

  public ngOnDestroy(): void {
    this.subs.forEach(sub => sub.unsubscribe());
  }
}
