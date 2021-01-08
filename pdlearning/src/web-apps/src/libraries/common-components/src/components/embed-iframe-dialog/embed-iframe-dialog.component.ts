import { BaseFormComponent, LocalTranslatorService, ModuleFacadeService } from '@opal20/infrastructure';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

import { Component } from '@angular/core';
import { DialogRef } from '@progress/kendo-angular-dialog';

@Component({
  selector: 'embed-iframe-dialog', // old selector is duplicate with upload file dialog component
  templateUrl: './embed-iframe-dialog.component.html'
})
export class EmbedIframeDialogComponent extends BaseFormComponent {
  public preview: SafeHtml;
  public embedHtml: string;
  public height: string = '315';
  public width: string = '560';
  public embedUrl: string;
  public mode: 'upload' | 'create' = 'create';

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private dialogRef: DialogRef,
    public translator: LocalTranslatorService,
    private sanitizer: DomSanitizer
  ) {
    super(moduleFacadeService);
  }

  public closeDialog(): void {
    this.dialogRef.close();
  }

  public insertEmbedHtml(): void {
    this.dialogRef.close(this.embedHtml);
  }
  public updateEmbedIframe(): void {
    if (this.embedUrl) {
      const videoId = this.getVideoId(this.embedUrl);
      if (videoId) {
        const innerHTML = `<iframe width="${this.width}" height="${this.height}" src="//www.youtube.com/embed/${videoId}" frameborder="0"  allowfullscreen></iframe>`;
        this.embedHtml = innerHTML;
        this.preview = this.sanitizer.bypassSecurityTrustHtml(this.embedHtml);
      }
    }
  }

  private getVideoId(url: string): string {
    const regExp = /^.*(youtu.be\/|v\/|u\/\w\/|embed\/|watch\?v=|&v=)([^#&?]*).*/;
    const match = url.match(regExp);

    return match && match[2].length === 11 ? match[2] : null;
  }
}
