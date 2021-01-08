import { Component, Input } from '@angular/core';

@Component({
  selector: 'preview-document-player',
  templateUrl: './preview-document-player.component.html'
})
export class PreviewDocumentPlayerComponent {
  @Input() public url: string;
  @Input() public showSpinner: boolean;
}
