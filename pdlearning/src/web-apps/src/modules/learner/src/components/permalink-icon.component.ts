import { BaseComponent, ClipboardUtil, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'permalink-icon',
  templateUrl: './permalink-icon.component.html'
})
export class PermalinkIconComponent extends BaseComponent implements OnInit {
  @Input() public detailUrl: string;
  @Output() public iconPermalinkClick: EventEmitter<Event> = new EventEmitter<Event>();

  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public copyPermalink(e: Event): void {
    ClipboardUtil.copyTextToClipboard(this.detailUrl);
    const bookmarkTypeMessage = this.moduleFacadeService.translator.translate('Copied successfully');
    this.showNotification(bookmarkTypeMessage);
    this.iconPermalinkClick.emit(e);
  }
}
