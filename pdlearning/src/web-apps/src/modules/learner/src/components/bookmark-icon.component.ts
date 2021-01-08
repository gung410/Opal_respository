import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

import { BaseComponent } from '@opal20/infrastructure';

@Component({
  selector: 'bookmark-icon',
  templateUrl: './bookmark-icon.component.html'
})
export class BookmarkIconComponent extends BaseComponent implements OnInit {
  @Input() public isBookmark: boolean = false;
  @Output() public iconBookmarkClick: EventEmitter<Event> = new EventEmitter<Event>();

  public onBookmarkChange(): void {
    this.iconBookmarkClick.emit();
  }
}
