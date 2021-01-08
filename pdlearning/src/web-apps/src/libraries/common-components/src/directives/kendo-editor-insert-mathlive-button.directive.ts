import { Directive, Host, OnDestroy, OnInit } from '@angular/core';
import { ISvgParameters, MathLiveEditorDialogComponent } from '../components/math-live-editor-dialog/math-live-editor-dialog.component';

import { DialogRef } from '@progress/kendo-angular-dialog';
import { EditorComponent } from '@progress/kendo-angular-editor';
import { ModuleFacadeService } from '@opal20/infrastructure';
import { Subscription } from 'rxjs';
import { ToolBarButtonComponent } from '@progress/kendo-angular-toolbar';

enum ToolbarIcon {
  Image = 'sum',
  Text = 'Add mathematics symbol',
  Title = 'Add mathematics symbol',
  ShowText = 'overflow'
}

@Directive({
  selector: 'kendo-toolbar-button[kendoEditorCustomInsertMathLiveButtonDirective]'
})
export class KendoEditorCustomInsertMathLiveButtonDirective implements OnInit, OnDestroy {
  private subs: Subscription[] = [];
  private dialogRef: DialogRef;

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private button: ToolBarButtonComponent,
    @Host() private editor: EditorComponent
  ) {}

  public ngOnDestroy(): void {
    this.subs.forEach(sub => sub.unsubscribe());
  }

  public ngOnInit(): void {
    this.button.icon = ToolbarIcon.Image;
    this.button.showText = ToolbarIcon.ShowText;
    this.button.text = ToolbarIcon.Text;
    this.button.title = ToolbarIcon.Title;
    this.subs.push(
      this.button.click.subscribe(() => {
        this.dialogRef = this.moduleFacadeService.dialogService.open({ content: MathLiveEditorDialogComponent });

        this.dialogRef.result.toPromise().then((result: ISvgParameters) => {
          if (result.s3Url) {
            this.appendS3UrlForEditor(result);
          }
        });
      })
    );
  }

  private appendS3UrlForEditor(parameters: ISvgParameters): void {
    const imageResult = document.createElement('img');
    imageResult.src = parameters.s3Url;
    imageResult.setAttribute('alt', parameters.latexCode);
    imageResult.setAttribute('style', 'max-width: initial');

    this.editor.exec('insertText', { text: '[insertHTML]' });
    const src = this.editor.value
      .replace('<p>[insertHTML]</p>', `${imageResult.outerHTML}<p></p>`)
      .replace('[insertHTML]', `</p>${imageResult.outerHTML}<p>`);

    this.editor.exec('setHTML', src);
  }
}
