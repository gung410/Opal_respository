import {
  Component,
  EventEmitter,
  Input,
  Output,
  ViewEncapsulation,
} from '@angular/core';

@Component({
  selector: 'catalog-toolbar',
  templateUrl: './catalog-toolbar.component.html',
  styleUrls: ['./catalog-toolbar.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class CatalogToolbarComponent {
  @Input() enableBookmark: boolean = true;
  @Input() bookmarkSize: number = 0;
  @Input() pickCourseMode: boolean = false;
  @Input() allowAddExternalPDOPermission: boolean = true;
  @Output() search: EventEmitter<string> = new EventEmitter();
  @Output() filter: EventEmitter<void> = new EventEmitter();
  @Output() bookmark: EventEmitter<void> = new EventEmitter();
  @Output() addExternal: EventEmitter<void> = new EventEmitter();

  onSearch(text: string): void {
    this.search.emit(text);
  }

  onFilterClicked(): void {
    this.filter.emit();
  }

  onBookmarkClicked(): void {
    this.bookmark.emit();
  }

  onAddExternalPDOClicked(): void {
    this.addExternal.emit();
  }

  onClickClearSearch(searchInput: any): void {
    searchInput.value = '';
    this.search.emit('');
  }
}
