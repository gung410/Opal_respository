import { Component, EventEmitter, Input, Output } from '@angular/core';

import { BaseComponent } from '@opal20/infrastructure';

@Component({
  selector: 'learner-toolbar',
  templateUrl: './learner-toolbar.component.html'
})
export class LearnerToolbarComponent extends BaseComponent {
  @Input() public titleHtml?: string | undefined;
  @Input() public title: string | undefined;
  @Input() public totalTextClass: string | undefined;
  @Input() public toolbarCustomClass: string | undefined;
  @Input() public total: number | undefined;
  @Input() public showTitle: boolean = false;
  @Input() public hasBackButton: boolean = false;
  @Input() public detailUrl?: string;
  @Input() public showCopyPermalink?: boolean;
  @Input() public isBookmark?: boolean;
  @Input() public showBookmark?: boolean;
  @Output() public iconBookmarkClick: EventEmitter<Event> = new EventEmitter<Event>();
  @Output() public backButtonClick: EventEmitter<boolean> = new EventEmitter<boolean>();
  @Output() public iconPermalinkClick: EventEmitter<Event> = new EventEmitter<Event>();
}
