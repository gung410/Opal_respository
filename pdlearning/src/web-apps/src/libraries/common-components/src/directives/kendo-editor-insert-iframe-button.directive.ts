import { Directive, Host, OnDestroy, OnInit } from '@angular/core';

import { DialogRef } from '@progress/kendo-angular-dialog';
import { EditorComponent } from '@progress/kendo-angular-editor';
import { EmbedIframeDialogComponent } from '../components/embed-iframe-dialog/embed-iframe-dialog.component';
import { ModuleFacadeService } from '@opal20/infrastructure';
import { Subscription } from 'rxjs';
import { ToolBarButtonComponent } from '@progress/kendo-angular-toolbar';

enum ToolbarIcon {
  Image = 'youtube',
  Text = 'Add youtube video',
  Title = 'Add youtube video',
  ShowText = 'overflow'
}

@Directive({
  selector: 'kendo-toolbar-button[kendoEditorCustomInsertIframeButton]'
})
export class KendoEditorCustomInsertIframeButtonDirective implements OnInit, OnDestroy {
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
        const dialogRef: DialogRef = this.moduleFacadeService.dialogService.open({ content: EmbedIframeDialogComponent });
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
