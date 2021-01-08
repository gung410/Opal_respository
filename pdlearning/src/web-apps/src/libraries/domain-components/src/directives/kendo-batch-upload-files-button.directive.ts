import { Directive, Host, Input, OnDestroy, OnInit } from '@angular/core';
import { FileUploaderUtils, OpalDialogService } from '@opal20/common-components';

import { DialogRef } from '@progress/kendo-angular-dialog';
import { EditorComponent } from '@progress/kendo-angular-editor';
import { PersonalFileDialogComponent } from '../components/personal-file-dialog/personal-file-dialog.component';
import { Subscription } from 'rxjs';
import { ToolBarButtonComponent } from '@progress/kendo-angular-toolbar';
import { UploadParameters } from '@opal20/infrastructure';

enum ToolbarIcon {
  Image = 'assets/images/icons/files/file-upload.svg',
  Text = 'Insert File',
  Title = 'Insert File',
  ShowText = 'overflow'
}

@Directive({
  selector: 'kendo-toolbar-button[kendoBatchUploadFilesButton]'
})
export class KendoBatchUploadFilesButtonDirective implements OnInit, OnDestroy {
  public height: string = 'auto';
  public width: string = 'auto';
  public align: 'left' | 'right' | 'center' = 'left';
  @Input() public icon: string;
  private subs: Subscription[] = [];
  constructor(
    private opalDialogService: OpalDialogService,
    private button: ToolBarButtonComponent,
    @Host() private editor: EditorComponent
  ) {}
  public ngOnInit(): void {
    this.button.imageUrl = ToolbarIcon.Image;
    this.button.showText = ToolbarIcon.ShowText;
    this.button.text = ToolbarIcon.Text;
    this.button.title = ToolbarIcon.Title;
    this.button.click.subscribe(() => {
      const dialogRef: DialogRef = this.opalDialogService.openDialogRef(PersonalFileDialogComponent);
      dialogRef.content.instance.icon = this.icon;
      dialogRef.content.instance.uploadFolder = 'editor-uploads';
      dialogRef.content.instance.settings.extensions = FileUploaderUtils.allowedExtensions;
      dialogRef.content.instance.filterByExtensions = FileUploaderUtils.allowedExtensions;
      dialogRef.result.toPromise().then((uploadParameters: UploadParameters[]) => {
        if (uploadParameters && uploadParameters.length > 0) {
          const html = this.createEmbedHtml(uploadParameters);
          this.editor.exec('insertText', { text: '[insertHTML]' });
          const src = this.editor.value.replace('<p>[insertHTML]</p>', `${html}<p></p>`).replace('[insertHTML]', `</p>${html}<p>`);
          this.editor.exec('setHTML', src);
        }
      });
    });
  }

  public createEmbedHtml(uploadParameters: UploadParameters[]): string {
    let returnHtml = ``;
    uploadParameters.forEach((fileParameter: UploadParameters) => {
      returnHtml = `${returnHtml}${this.insertEmbedHtml(FileUploaderUtils.wrappingFile(fileParameter, this.width, this.height))}`;
    });
    return returnHtml;
  }

  public insertEmbedHtml(innerHTML: string): string {
    const el = document.createElement('div');
    el.setAttribute('style', `text-align: ${this.align};`);
    el.innerHTML = innerHTML;
    if (Number(this.width) === 0 || Number(this.width) > 99999 || Number(this.height) === 0 || Number(this.height) > 99999) {
      this.width = 'auto';
      this.height = 'auto';
      return;
    }
    el.firstElementChild.setAttribute('width', this.width);
    el.firstElementChild.setAttribute('height', this.height);
    return el.outerHTML;
  }

  public ngOnDestroy(): void {
    this.subs.forEach(sub => sub.unsubscribe());
  }
}
