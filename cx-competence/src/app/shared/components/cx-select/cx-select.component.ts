// tslint:disable:member-ordering
import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
  ViewEncapsulation,
} from '@angular/core';
import { NgSelectComponent } from '@ng-select/ng-select';
import { isEmpty } from 'lodash';
import { BehaviorSubject, concat, Observable, of } from 'rxjs';
import {
  catchError,
  debounceTime,
  distinctUntilChanged,
  map,
  switchMap,
  tap,
} from 'rxjs/operators';
import { CxSelectConfigModel, CxSelectItemModel } from './cx-select.model';

@Component({
  selector: 'cx-select',
  templateUrl: './cx-select.component.html',
  styleUrls: ['./cx-select.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class CxSelectComponent<T> implements OnInit, AfterViewInit {
  @Input() config: CxSelectConfigModel = new CxSelectConfigModel();
  @Input() disabled: boolean = false;
  @Input() itemObs: (
    term?: string,
    pageIndex?: number,
    pageSize?: number
  ) => Observable<CxSelectItemModel<T>[]>;
  @Input() selectedItems: CxSelectItemModel<T>[];
  @Input() disabledItems: CxSelectItemModel<T>[];
  @Output() selectedItemsChange: EventEmitter<
    CxSelectItemModel<T>[]
  > = new EventEmitter();

  @ViewChild('cxSelect') ngSelect: NgSelectComponent;

  public itemLoading: boolean = false;
  public itemInput$: BehaviorSubject<string> = new BehaviorSubject<string>('');

  _items: CxSelectItemModel<T>[] = [];
  public get items(): CxSelectItemModel<T>[] {
    return this._items;
  }
  public set items(value: CxSelectItemModel<T>[]) {
    this._items = value;
    this.cdr.detectChanges();
  }

  _item$: Observable<CxSelectItemModel<T>[]>;
  public get item$(): Observable<CxSelectItemModel<T>[]> {
    return this._item$;
  }
  public set item$(value: Observable<CxSelectItemModel<T>[]>) {
    this._item$ = value;
    this._item$.subscribe((items) => {
      this.items = items;
    });
  }

  private skipLoadItems: boolean = false;
  private loadMorePageSize: number = this.config.loadMorePageSize;
  private pageIndex: number = this.config.pageNumberStartAt;
  private hasMoreData: boolean = true;

  constructor(private ref: ElementRef, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.loadItem();
  }

  ngAfterViewInit(): void {
    if (this.config.searchable) {
      this.setPlaceHolderForSearchInput(this.config.placeholder);
    }
  }

  trackByFn = (item: CxSelectItemModel<T>) => item.id;

  onSelectItem(selectedItems: CxSelectItemModel<T>[]): void {
    this.selectedItemsChange.emit(selectedItems);
    this.resetSearchInput();
  }

  onBlurSelect(): void {
    if (this.config.searchable) {
      this.resetSearchInput();
    }
  }

  onScrollToEnd(): void {
    const currentSearchTerm = this.itemInput$.getValue();
    if (
      this.itemObs &&
      this.config.autoLoadMore &&
      this.hasMoreData &&
      !this.isNullOrEmpty(currentSearchTerm)
    ) {
      this.pageIndex = this.pageIndex += 1;
      this.item$ = this.getItemObs(currentSearchTerm);
    }
  }

  onFocusSelect(): void {
    if (!this.config.searchable) {
      this.loadItem();
    }
  }

  private resetSearchInput(): void {
    // set flag to Skip load items to keep current items
    // only apply for single and searchable dropdown .
    if (this.config.searchable && !this.config.multiple) {
      this.skipLoadItems = true;
    }

    this.ngSelect.typeahead.next('');

    // wait for end of subscription.
    // 600 = 500 (debounce time) + 100 (offset delay time)
    setTimeout(() => {
      this.skipLoadItems = false;
    }, 600);
  }

  private loadItem(): void {
    this.item$ = this.config.searchable
      ? concat([], this.getItemWhenSearch())
      : this.getItemObs();
  }

  private getItemObs = (searchText?: string) => {
    if (this.skipLoadItems) {
      return of(this.items);
    }
    if (!this.itemObs) {
      return of([]);
    }

    if (this.config.searchable && this.isNullOrEmpty(searchText)) {
      return of([]);
    }
    this.showLoading(searchText);

    return this.itemObs(searchText, this.pageIndex, this.loadMorePageSize).pipe(
      catchError(() => of([])),
      map(this.checkDisabledItem),
      switchMap((response) => {
        let result: CxSelectItemModel<T>[] = [];

        // Stop load more if the response does not enough items.
        if (response.length < this.loadMorePageSize) {
          this.hasMoreData = false;
        }

        if (this.config.autoLoadMore && this.pageIndex > 0) {
          result = this.items.concat(response);
        } else {
          result = response;
        }

        this.items = result;

        return of(result);
      }),
      tap(this.hideLoading)
    );
  };

  private getItemWhenSearch = () => {
    return this.itemInput$.pipe(
      tap(this.resetPageIndex),
      tap(this.resetData),
      tap(this.showLoading),
      debounceTime(500),
      distinctUntilChanged(),
      switchMap(this.getItemObs),
      tap(this.hideLoading)
    );
  };

  private checkDisabledItem = (
    items: CxSelectItemModel<T>[]
  ): CxSelectItemModel<T>[] => {
    if (!isEmpty(this.disabledItems)) {
      items.forEach((item) => {
        const result = this.disabledItems.find(
          (disabledItem) => disabledItem && disabledItem.id === item.id
        );
        if (result !== undefined) {
          item.disabled = true;
        }
      });
    }

    return items;
  };

  private isNullOrEmpty(text: string): boolean {
    return text === null || text === undefined || `${text}`.trim() === '';
  }

  private showLoading = (searchText: string) => {
    this.itemLoading = !this.isNullOrEmpty(searchText);
  };

  private hideLoading = () => {
    this.itemLoading = false;
  };

  private resetPageIndex = () => {
    this.hasMoreData = true;
    this.pageIndex = this.config.pageNumberStartAt;
  };

  private resetData = () => {
    if (this.skipLoadItems) {
      return;
    }
    this.items = [];
    this.cdr.detectChanges();
  };

  private setPlaceHolderForSearchInput(placeholder: string): void {
    if (!this.ref) {
      return;
    }
    const currentElement: any = this.ref.nativeElement;
    const inputQuery = 'div.ng-input > input[type=text]';
    const searchInputElement: any = currentElement.querySelector(inputQuery);
    if (!searchInputElement) {
      return;
    }
    searchInputElement.placeholder = placeholder;
  }
}
