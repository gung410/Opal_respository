import {
  ChangeDetectorRef,
  Component,
  ContentChild,
  ElementRef,
  EventEmitter,
  forwardRef,
  HostBinding,
  Input,
  OnInit,
  Output,
  TemplateRef,
  ViewChild,
  ViewEncapsulation
} from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { NgOption, NgSelectComponent } from '@ng-select/ng-select';
import { DomUtils } from 'app-utilities/dom.utils';
import { Dictionary, Utils } from 'app-utilities/utils';
import { Observable, of, Subscription } from 'rxjs';
import { OpalBaseComponent } from '../component.abstract';
import {
  OpalLabelTemplateDirective,
  OpalOptionTemplateDirective
} from './templates.directives';

@Component({
  selector: 'opal-select',
  templateUrl: './select.component.html',
  styleUrls: ['./select.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => OpalSelectComponent),
      multi: true
    }
  ]
})
export class OpalSelectComponent
  extends OpalBaseComponent
  implements ControlValueAccessor, OnInit {
  @HostBinding('class.-opened') get isOpenedHostBinding(): boolean {
    return this.isOpened;
  }
  @HostBinding('class.-disabled') get disabledHostBinding(): boolean {
    return this.disabled;
  }
  @HostBinding('class.-readonly') get readOnlyHostBinding(): boolean {
    return this.readOnly;
  }

  @HostBinding('class.-push-down-mode')
  get pushDownModeHostBinding(): boolean {
    return this.pushDownMode;
  }
  @HostBinding('class.-static-mode')
  get staticModeHostBinding(): boolean {
    return this.staticMode;
  }
  @HostBinding('class.-multiple-inline')
  get multipleInlineHostBinding(): boolean {
    return this.multipleInline;
  }
  @HostBinding('class.-multiple-inline-no-wrap')
  get multipleInlineNoWrapHostBinding(): boolean {
    return this.multipleInlineNoWrap;
  }
  @HostBinding('class.-clearable') get clearableHostBinding(): boolean {
    return this.clearable;
  }
  get items(): Array<object | string | number> {
    return this._items;
  }
  @Input() set items(value: Array<object | string | number>) {
    if (value == null) {
      value = [];
    }
    if (!Utils.isDifferent(this._items, value)) {
      return;
    }
    if (this.loading) {
      this._prevItems = Utils.clone(this._items);
    }
    this._items =
      this.ignoreItemFn != null
        ? value.filter(
            (p) => !this.ignoreItemFn(p) || this.isItemMatchSelectedValue(p)
          )
        : value;
    this._selectAllItems = this._items.filter(
      (p) =>
        this.selectAllIgnoreItemLabel == null ||
        this._getItemLabel(p) !== this.selectAllIgnoreItemLabel
    );
    if (value.length >= OpalSelectComponent.autoVirtualScrollBreakPoint) {
      this.virtualScroll = true;
    }
    if (this.initiated) {
      this.itemsChangeEvent.emit(this._items);
    }

    const tempI = this._items.map((i) => this._getItemValue(i));

    this._allItemValuesDictionary = Utils.toDictionary(this._items, (i) =>
      this._getItemValue(i)
    );
  }
  get placeholder(): string {
    return this._placeholder;
  }
  @Input() set placeholder(value: string) {
    if (this._placeholder === value) {
      return;
    }
    this._placeholder = Utils.isNullOrEmpty(value)
      ? OpalSelectComponent.defaultPlaceholder
      : value;
  }
  get selectedValue(): unknown | Array<unknown> {
    return this._selectedValue;
  }
  @Input() set selectedValue(value: unknown | Array<unknown>) {
    if (this._selectedValue === value) {
      return;
    }
    this._selectedValue = value === '' ? null : value;
    this.selectedItemLabel = this.getSelectedItemLabel();
    this.detectChanges(this.changeDetectorRef);
  }
  get pushDownMode(): boolean {
    return this._pushDownMode;
  }
  @Input()
  set pushDownMode(v: boolean) {
    this._pushDownMode = v;
    if (!this.initiated) {
      return;
    }
    this._setupDropdownCloseWhenClickOutside(
      this.pushDownMode || this.notCloseDropdownOnFocusOut
    );
  }

  get notCloseDropdownOnFocusOut(): boolean {
    return this._notCloseDropdownOnFocusOut;
  }
  @Input()
  set notCloseDropdownOnFocusOut(v: boolean) {
    this._notCloseDropdownOnFocusOut = v;
    if (!this.initiated) {
      return;
    }
    this._setupDropdownCloseWhenClickOutside(
      this.pushDownMode || this.notCloseDropdownOnFocusOut
    );
  }

  get isOpened(): boolean {
    return this.ngSelect.isOpen;
  }
  set isOpened(v: boolean) {
    this.ngSelect.isOpen = v;
    this.detectChanges(this.changeDetectorRef, undefined, undefined, true);
  }
  get ngSelectNgClass(): object {
    return {
      '-borderless': this.borderless,
      '-dropdown-borderless': this.dropdownBorderless,
      '-dropdown-same-width': this.dropdownSameWidth,
      '-no-scroll': this.oneItemAsContainer
    };
  }
  get element(): HTMLElement {
    return this.elementRef.nativeElement;
  }
  static readonly defaultPlaceholder = '\u00a0';
  static readonly minDistanceToBoundary = 10;
  static readonly dropdownMaxBoundaryScale = 0.9;
  static readonly autoVirtualScrollBreakPoint = 500;

  @ContentChild(OpalOptionTemplateDirective, {
    read: TemplateRef,
    static: false
  })
  optionTemplate: TemplateRef<unknown>;
  @ContentChild(OpalLabelTemplateDirective, {
    read: TemplateRef,
    static: false
  })
  labelTemplate: TemplateRef<unknown>;
  @Input() fetchPageSize: number = 25;
  @Input() fetchDataByValuesFn?: (
    values: Array<unknown>
  ) => Observable<Array<object | string | number>>;
  get fetchDataFn():
    | ((
        searchText: string,
        skipCount: number,
        maxResultCount: number
      ) => Observable<Array<object | string | number>>)
    | null {
    return this._fetchDataFn;
  }
  @Input()
  set fetchDataFn(
    v:
      | ((
          searchText: string,
          skipCount: number,
          maxResultCount: number
        ) => Observable<Array<object | string | number>>)
      | null
  ) {
    if (this._fetchDataFn === v) {
      return;
    }
    this._fetchDataFn = v;
    if (this.initiated && this.fetchedOnce) {
      this.fetch();
    }
  }
  get fetchDataLogicDependenciesInput(): unknown {
    return this._fetchDataLogicDependenciesInput;
  }
  @Input()
  set fetchDataLogicDependenciesInput(v: unknown) {
    if (Utils.isDifferent(this._fetchDataLogicDependenciesInput, v)) {
      this._fetchDataLogicDependenciesInput = v;
      if (this.initiated && this.fetchedOnce) {
        Utils.delay(() => this.fetch(), 100, this.onDestroy$);
      }
    }
  }

  @Input() ignoreItemFn: (item: unknown) => boolean;
  @Input() valueField: string = 'value';
  @Input() labelField: string = 'label';
  @Input() searchFields?: string[];
  @Input() endOfGroupField: string = 'endOfGroup';
  @Input() borderless: boolean = false;
  @Input() clearable: boolean = false;
  @Input() searchable: boolean = true;
  @Input() hasMoreDataToFetch: boolean = false;
  @Input() isDisplayCloseIcon: boolean = true;
  @Input() dropdownBorderless: boolean = false;
  @Input() dropdownSameWidth: boolean = true;
  @Input() allowDropdownWithOverBoundaryHorizontal: boolean = false;
  @Input() multiple: boolean = false;
  @Input() disabled: boolean = false;
  @Input() readOnly: boolean = false;
  @Input() dropdownBoundary: string | undefined | HTMLElement;
  @Input() minDropdownHeight: number = 250;
  @Input() minDropdownDynamicHeight: number = 80;
  @Input() maxDropdownHeight: number | undefined;
  @Input() clearAllText: string = 'Clear all';
  @Input() virtualScroll: boolean = false;
  @Input() oneItemAsContainer: boolean = false;
  @Input() autoScrollToViewOnOpen: boolean = true;
  @Input() addTag: boolean = false;
  @Input() selectOnTab: boolean = true;
  @Input() addTagOnFocusOut: boolean = false;
  @Input() staticMode: boolean = false;
  @Input() addTagText: string = 'Add item';
  @Input() multipleInline: boolean = true;
  @Input() multipleInlineNoWrap: boolean = false;
  @Input() closeOnSelect: boolean | undefined;
  @Input() clearSearchOnAdd: boolean = false;
  @Input() customMarkedItemClass: string | undefined;
  @Input() customSelectedItemClass: string | undefined;
  @Input() labelForId: string | undefined;
  @Input() inputAttrs: { [key: string]: string } = {};
  @Input() tabIndex: number | undefined;
  @Input() notFoundText: string = 'No items found';
  @Input() searchFn: (
    term: string,
    item: object | string | number
  ) => boolean = null;
  @Input() dropdownItemLabelWrap: boolean = false;
  @Input() fetchNoPaging: boolean = false;
  @Input() disableSelectAll: boolean | undefined;
  @Input() changeConfirmationFn: (selectedItem: unknown) => Promise<boolean>;
  @Input() fetchOnInit: boolean = false;
  @Input() notClearSearchTextOnFocusOut: boolean = false;

  _selectAllIgnoreItemLabel?: string;
  get selectAllIgnoreItemLabel(): string | null {
    return this._selectAllIgnoreItemLabel;
  }
  @Input()
  set selectAllIgnoreItemLabel(v: string | null) {
    if (this._selectAllIgnoreItemLabel === v) {
      return;
    }
    this._selectAllIgnoreItemLabel = v;
    this._selectAllItems = this._items.filter(
      (p) =>
        this.selectAllIgnoreItemLabel == null ||
        this._getItemLabel(p) !== this.selectAllIgnoreItemLabel
    );
  }

  @Output() open: EventEmitter<unknown> = new EventEmitter<unknown>();
  @Output() close: EventEmitter<unknown> = new EventEmitter<unknown>();
  @Output()
  selectedValueChange: EventEmitter<unknown> = new EventEmitter<unknown>();
  @Output()
  selectedItemChange: EventEmitter<unknown> = new EventEmitter<unknown>();
  @Output() blur: EventEmitter<unknown> = new EventEmitter<unknown>();
  @Output('focus')
  focusEvent: EventEmitter<unknown> = new EventEmitter<unknown>();
  @Output() clear: EventEmitter<unknown> = new EventEmitter<unknown>();
  @Output('itemsChange') itemsChangeEvent: EventEmitter<
    Array<object | string | number>
  > = new EventEmitter();

  @ViewChild('ngSelect', { static: true }) ngSelect: NgSelectComponent;
  onChange?: (_: unknown) => void;
  onTouched?: (_: unknown) => void;
  selectedItemLabel: string | string[] | undefined;
  loading: boolean = false;
  fetchedOnce: boolean = false;

  protected destroyed: boolean = false;

  private _items: Array<object | string | number> = [];
  private _allItemValuesDictionary: Dictionary<unknown> = {};
  private _selectAllItems: Array<object | string | number> = [];
  private _prevItems: Array<object | string | number> = [];
  private _placeholder: string = OpalSelectComponent.defaultPlaceholder;
  private _selectedValue: unknown | Array<unknown> = undefined;
  private _pushDownMode: boolean = false;
  private _notCloseDropdownOnFocusOut: boolean = false;
  private _searchTerm: string = '';
  private _originalHandleMousedown: (($event: MouseEvent) => void) | undefined;
  private _fetchDataSub: Subscription = new Subscription();
  private _fetchDataByValuesSub: Subscription = new Subscription();
  private _noMoreItemsToFetch: boolean = false;
  private _currentFetchSkip: number = 0;
  private _loadDebounce = Utils.debounce(
    (term?: string, onDone?: () => void) => {
      this.fetch(term, 'reset', onDone);
    },
    700
  );
  private _fetchMoreDebounce = Utils.debounce(
    (term?: string, onDone?: () => void) => {
      this.fetchMore(term, onDone);
    },
    300
  );
  private _fetchDataFn:
    | ((
        searchText: string,
        skipCount: number,
        maxResultCount: number
      ) => Observable<Array<object | string | number>>)
    | null;
  private _fetchDataLogicDependenciesInput: unknown;
  private _isDataFetchedOnce: boolean = false;
  private readonly MAX_ITEMS_PER_REQUEST: number = 100000;

  constructor(
    public changeDetectorRef: ChangeDetectorRef,
    public elementRef: ElementRef
  ) {
    super();
  }

  ngOnInit(): void {
    this.selectedItemLabel = this.getSelectedItemLabel();
    this._setupDropdownCloseWhenClickOutside(
      this.pushDownMode || this.notCloseDropdownOnFocusOut
    );
    this._setupCloseOnSelect();
    this.checkToFetchItemsForSelectedValue();
    if (this.fetchOnInit === true) {
      this.fetch();
    }
    super.ngOnInit();
  }

  ngOnDestroy(): void {
    super.ngOnDestroy();
    this.initiated = false;
    this.destroyed = true;
  }

  onSelectedValueChange(newValue: unknown): void {
    if (!this.changeConfirmationFn) {
      this.selectedValue = newValue;
      return;
    }

    /*
      SUMMARY: Reset data back to old value.
        Set data to new value, then appRef.tick() to trigger change detection to
        sync data from ng-select(select2) with selectedValue. Then reset selectedValue back to oldValue.
      */
    const oldValue = this.selectedValue;
    this.selectedValue = newValue;
    this.detectChanges(this.changeDetectorRef, true);
    this.selectedValue = oldValue;

    // If confirm, apply new value
    this.changeConfirmationFn(newValue).then((result) => {
      if (result) {
        this.selectedValue = newValue;
      }
    });
  }

  fetchByValues(): void {
    this._fetchDataByValuesSub.unsubscribe();
    this.loading = false;
    if (
      this.fetchDataByValuesFn == null ||
      Utils.isNullOrEmpty(this.selectedValue)
    ) {
      return;
    }

    this.loading = true;
    this._fetchDataByValuesSub = this.fetchDataByValuesFn(
      this.selectedValue instanceof Array
        ? this.selectedValue
        : [this.selectedValue]
    )
      .pipe(this.untilDestroy())
      .subscribe(
        (apiResult) => {
          this.items = this._getNewCombinedWithFetchedItems(apiResult);
          this.loading = false;
          if (this.isOpened) {
            this.processDropdown(
              null,
              this.getCurrentVerticalScrollValue(),
              100
            );
          }
        },
        (error) => {
          this.loading = false;
        }
      );
  }

  fetch(
    term?: string,
    mode: 'reset' | 'concat' = 'reset',
    onDone?: () => void
  ): void {
    this.loading = false;
    if (
      this.fetchDataFn == null ||
      (this._noMoreItemsToFetch && mode === 'concat')
    ) {
      if (onDone != null) {
        onDone();
      }

      return;
    }
    this.loading = true;
    if (mode === 'reset') {
      this._currentFetchSkip = 0;
      this._noMoreItemsToFetch = false;
    }
    this._fetchDataSub.unsubscribe();
    this._fetchDataSub = this.fetchDataFn(
      term,
      this._currentFetchSkip,
      this.fetchPageSize
    )
      .pipe(this.untilDestroy())
      .subscribe(
        (apiResult) => {
          if (this.loading) {
            this._currentFetchSkip =
              this._currentFetchSkip + this.fetchPageSize;
          }

          const filteredApiResult = apiResult.filter(
            (r) =>
              this._allItemValuesDictionary[this._getItemValue(r)] === undefined
          );
          this.items =
            mode === 'concat'
              ? this.loading
                ? this.items.concat(filteredApiResult)
                : this._prevItems.concat(filteredApiResult)
              : this._getNewCombinedWithFetchedItems(apiResult);
          if (apiResult.length < this.fetchPageSize) {
            if (this.hasMoreDataToFetch === false) {
              this._noMoreItemsToFetch = true;
            }
          }
          if (this.isOpened) {
            this.processDropdown(
              null,
              this.getCurrentVerticalScrollValue(),
              100
            );
          }
          if (this.loading && onDone != null) {
            onDone();
          }
          this.loading = false;
          this.fetchedOnce = true;
        },
        (error) => {
          this.loading = false;
          if (onDone != null) {
            onDone();
          }
        }
      );
  }

  fetchMore(term?: string, onDone?: () => void): void {
    this.fetch(term, 'concat', onDone);
  }

  writeValue(value: unknown | Array<unknown>): void {
    this.selectedValue = value;
    this.checkToFetchItemsForSelectedValue();
  }
  registerOnChange(fn: (_: unknown) => void): void {
    this.onChange = fn;
  }
  registerOnTouched(fn: (_: unknown) => void): void {
    this.onTouched = fn;
  }

  getDropdownElement(): HTMLElement | undefined {
    return this.element.querySelector('.ng-dropdown-panel') as
      | HTMLElement
      | undefined;
  }

  onChanged(e: unknown): void {
    const prevTerm = this._searchTerm;
    this._emitChange();
    this._setSearchTerm(this.ngSelect.searchTerm || '');
    this.detectChanges(this.changeDetectorRef, true);
    if (this.notClearSearchTextOnFocusOut) {
      this.ngSelect.searchTerm = prevTerm;
      this._setSearchTerm(prevTerm);
    }
  }

  onFocused(e: unknown): void {
    if (this.onTouched != null) {
      this.onTouched(e);
    }
    this.focusEvent.emit(e);
  }

  onOpened(e: unknown): void {
    if (
      this.fetchDataFn != null &&
      (this.items.length <= 1 || !this._isDataFetchedOnce)
    ) {
      this.fetchMore(this.ngSelect.searchTerm, () => {
        this._isDataFetchedOnce = true;
        this.processDropdown(() => {
          this.open.emit(e);
        });
      });
    } else {
      this.processDropdown(() => {
        this.open.emit(e);
      });
    }
  }

  getCurrentVerticalScrollValue(): number | null {
    const dropdownElement = this.getDropdownElement();
    if (dropdownElement == null) {
      return null;
    }
    return this._getDropdownScrollContainerEl(dropdownElement).scrollTop;
  }

  processDropdown(
    onDone?: () => void,
    keepScrollPositionValue?: number,
    delayTime?: number
  ): void {
    const doProcessDropdown = () => {
      Utils.doInterval(
        (intervalSubscriber) => {
          if (this.getDropdownElement() == null) {
            return;
          }
          intervalSubscriber.unsubscribe();
          this._fixDropdownSize();
          this._fixDropdownPosition();
          if (this.virtualScroll) {
            this._triggerRenderItemsRangeForVirtualScroll();
          }
          this._displayDropdown();
          if (this.autoScrollToViewOnOpen) {
            this.scrollToViewOnOpened();
          }
          if (onDone) {
            onDone();
          }
          if (keepScrollPositionValue != null) {
            this.scrollToPosition(keepScrollPositionValue);
          }
        },
        10,
        200,
        this.untilDestroy()
      );
    };

    if (delayTime >= 0) {
      setTimeout(() => {
        doProcessDropdown();
      }, delayTime);
    } else {
      doProcessDropdown();
    }
  }

  onClosed(e: unknown): void {
    this.close.emit(e);
  }

  getSelectedItem():
    | (object | string | number)
    | Array<object | string | number>
    | undefined {
    if (this.selectedValue == null || this.selectedValue === '') {
      return undefined;
    }
    const items = this._getAllAvailableItems();
    if (this.selectedValue instanceof Array) {
      const selectedValueMap = Utils.toDictionarySelect(
        this.selectedValue,
        (p) => p,
        (p) => true
      );
      return items.filter((p) => {
        const itemValue = this._getItemValue(p);
        return itemValue != null && selectedValueMap[itemValue];
      });
    } else {
      return items.find((p) => {
        return this._checkItemOfValue(p, this.selectedValue);
      });
    }
  }

  isItemMatchSelectedValue(item: object | string | number): boolean {
    if (this.selectedValue instanceof Array) {
      return this.selectedValue.find(
        (value) => value === this._getItemValue(item)
      );
    } else {
      return this._getItemValue(item) === this.selectedValue;
    }
  }

  getSelectedItemLabel(): string | string[] | undefined {
    const selectedItem = this.getSelectedItem();
    if (selectedItem == null) {
      return undefined;
    }
    if (selectedItem instanceof Array) {
      return selectedItem.map((p) => {
        return typeof p === 'string' ? p : p[this.labelField];
      });
    }
    return typeof selectedItem === 'string'
      ? selectedItem
      : selectedItem[this.labelField];
  }

  onSearched(
    e: string | undefined | { term: string; items: Array<string | object> }
  ): void {
    this._setSearchTerm(
      e == null ? '' : typeof e === 'string' ? e.trim() : e.term.trim()
    );
  }

  onFocusOut($event: unknown): void {
    if (this.addTag && this.addTagOnFocusOut) {
      if (this._searchTerm) {
        let searchTermItem = this.ngSelect.itemsList.findItem(this._searchTerm);
        if (searchTermItem == null) {
          // Fix bug addItem two time because of same reference
          if (
            this.ngSelect.itemsList.filteredItems ===
            this.ngSelect.itemsList.items
          ) {
            // tslint:disable-next-line:no-any
            (this.ngSelect.itemsList as any)._filteredItems = Utils.clone(
              this.ngSelect.itemsList.filteredItems
            );
          }
          searchTermItem = this.ngSelect.itemsList.addItem(this._searchTerm);
        }

        if (this.multiple) {
          if (
            this.selectedValue == null ||
            (this.selectedValue instanceof Array &&
              this.selectedValue.indexOf(this._searchTerm) < 0)
          ) {
            this.selectedValue =
              this.selectedValue == null
                ? [this._searchTerm]
                : [...(this.selectedValue as Array<unknown>), this._searchTerm];
            this._emitChange();
            this.detectChanges(this.changeDetectorRef);
          }
        } else {
          if (this._selectedValue !== this._searchTerm) {
            this.selectedValue = this._searchTerm;
            this._emitChange();
            this.detectChanges(this.changeDetectorRef);
          }
        }
      } else if (this.ngSelect.showAddTag) {
        this.ngSelect.selectTag();
      }
    }
    if (!this.notClearSearchTextOnFocusOut) {
      this._setSearchTerm('');
    }
  }

  getDropdownBoundaryElContentRect(): ClientRect {
    const windowViewClientRect = {
      top: 0,
      bottom: window.innerHeight,
      left: 0,
      right: window.innerWidth,
      width: window.innerWidth,
      height: window.innerHeight
    };
    if (this.dropdownBoundary == null || this.dropdownBoundary === '') {
      return windowViewClientRect;
    }
    let boundaryEl: HTMLElement | null;
    if (this.dropdownBoundary instanceof Element) {
      boundaryEl = this.dropdownBoundary;
    } else {
      boundaryEl = this.element.closest(
        this.dropdownBoundary
      ) as HTMLElement | null;
    }
    if (boundaryEl == null) {
      return windowViewClientRect;
    }
    return DomUtils.getElementContentRect(boundaryEl);
  }

  closeDropdown(): void {
    if (this.isOpened === false) {
      return;
    }
    if (this.pushDownMode) {
      // tslint:disable-next-line:no-any
      (this.ngSelect as any)._manualOpen = false;
      this.ngSelect.close();
      // tslint:disable-next-line:no-any
      (this.ngSelect as any)._manualOpen = true;
    } else {
      this.ngSelect.close();
    }
  }

  openDropdown(): void {
    if (this.isOpened) {
      return;
    }
    if (this.pushDownMode) {
      // tslint:disable-next-line:no-any
      (this.ngSelect as any)._manualOpen = false;
      this.ngSelect.open();
      // tslint:disable-next-line:no-any
      (this.ngSelect as any)._manualOpen = true;
    } else {
      this.ngSelect.open();
    }
  }

  scrollToViewOnOpened(): void {
    DomUtils.scrollToView(this.element, true);
  }

  scrollToSelectedItem(): void {
    const dropdownElement = this.getDropdownElement();
    if (dropdownElement != null) {
      this._scrollToSelectedItem(dropdownElement);
    }
  }

  getSelectedItemEl(): Element {
    const dropdownEl = this.getDropdownElement();
    if (dropdownEl == null) {
      return undefined;
    }
    return this._getSelectedItemEl(dropdownEl);
  }

  onCleared(e: unknown): void {
    this.clear.emit(e);
  }

  onBlurred(e: unknown): void {
    this.blur.emit(e);
  }

  toggle(): void {
    if (!this.isOpened) {
      this.openDropdown();
    } else {
      this.closeDropdown();
    }
  }

  selectItemNgClass(item: object, item$: NgOption): object {
    const result = { '-end-of-group': item[this.endOfGroupField] === true };
    if (
      this.ngSelect.itemsList.markedItem === item$ &&
      this.customMarkedItemClass != null
    ) {
      result[this.customMarkedItemClass] = true;
    }
    if (item$.selected && this.customSelectedItemClass != null) {
      result[this.customSelectedItemClass] = true;
    }
    if (this.dropdownItemLabelWrap) {
      result['-text-wrap'] = true;
    }
    return result;
  }

  focus(): void {
    DomUtils.scrollToView(this.element);
    this.ngSelect.focus();
  }

  canFocus(): boolean {
    return this.searchable && !this.readOnly;
  }

  searchFnCreator():
    | ((term: string, item: object | string | number) => boolean)
    | null {
    if (this.searchFields != null) {
      return (term: string, item: object | string | number) => {
        const lowerCaseTerm = term ? term.toLocaleLowerCase().trim() : '';
        return Utils.any(this.searchFields, (fieldName) => {
          if (typeof item !== 'object') {
            return false;
          }

          const fieldValue = item[fieldName]
            ? (item[fieldName].toString() as string)
            : '';
          return (
            fieldValue.toLocaleLowerCase().trim().indexOf(lowerCaseTerm) >= 0
          );
        });
      };
    }
    return this.searchFn;
  }

  scrollToPosition(position: number): void {
    const dropdownEl = this.getDropdownElement();
    if (dropdownEl == null) {
      return;
    }

    const dropdownScrollContainerEl = this._getDropdownScrollContainerEl(
      dropdownEl
    );
    dropdownScrollContainerEl.scrollTop = position;
  }

  onScrollToEnd(e: unknown): void {
    if (!this.fetchNoPaging) {
      this._fetchMoreDebounce(this.ngSelect.searchTerm);
    }
  }

  selectAllItem(): void {
    const fetchAllLeftItemsObs = this.fetchDataFn
      ? this.fetchDataFn(null, this.MAX_ITEMS_PER_REQUEST, 0)
      : of([]);
    fetchAllLeftItemsObs.subscribe((allLeftItems) => {
      if (allLeftItems) {
        this.items = this.items.concat(allLeftItems);
      }
      this.selectedValue = this.isSelectAll
        ? []
        : this._selectAllItems.map((x) => this._getItemValue(x));
      this._emitChange();
    });
  }

  showSelectAll(): boolean {
    return this.items.length > 0 && this.multiple && !this.disableSelectAll;
  }

  get isSelectAll(): boolean {
    return (
      this.multiple &&
      this.selectedValue instanceof Array &&
      this.selectedValue.length >= this._selectAllItems.length
    );
  }

  private checkToFetchItemsForSelectedValue(): void {
    if (
      this.items.length === 0 &&
      this.fetchDataByValuesFn != null &&
      !Utils.isNullOrEmpty(this.selectedValue)
    ) {
      this.fetchByValues();
    }
  }

  private _fixDropdownPosition(): void {
    if (this.dropdownSameWidth) {
      return;
    }
    const dropdownElement = this.getDropdownElement();
    if (dropdownElement == null) {
      return;
    }
    const dropdownElementBoundary = dropdownElement.getBoundingClientRect();
    const dropdownBoundary = this.getDropdownBoundaryElContentRect();

    if (dropdownElementBoundary.right > dropdownBoundary.right) {
      const dropdownElementComputedStyle = window.getComputedStyle(
        dropdownElement
      );
      if (dropdownElementComputedStyle.left == null) {
        throw new Error('Bravo-select left must be defined.');
      }

      const currentDropdownElementStyleLeft = parseFloat(
        dropdownElementComputedStyle.left.replace('px', '')
      );
      const dropdownRightOverBoundaryLength =
        dropdownElementBoundary.right - dropdownBoundary.right;
      const nextToRightBoundaryDropdownStyleLeft =
        currentDropdownElementStyleLeft -
        dropdownRightOverBoundaryLength -
        OpalSelectComponent.minDistanceToBoundary;

      const nextToLeftBoundaryDropdownStyleLeft =
        currentDropdownElementStyleLeft -
        (this.element.getBoundingClientRect().left -
          dropdownElementBoundary.left) +
        OpalSelectComponent.minDistanceToBoundary;

      dropdownElement.style.left =
        dropdownElementBoundary.width <= dropdownBoundary.width
          ? nextToRightBoundaryDropdownStyleLeft + 'px'
          : nextToLeftBoundaryDropdownStyleLeft + 'px';
    }
  }

  private _getDropdownElementContentContainerEl(
    dropdownElement: HTMLElement
  ): Element {
    return dropdownElement.querySelector(
      '.ng-dropdown-panel-items > div:nth-child(2)'
    );
  }

  private _getDropdownScrollContainerEl(dropdownElement: HTMLElement): Element {
    return dropdownElement.querySelector('.scroll-host');
  }

  private _fixDropdownSize(): void {
    const dropdownElement = this.getDropdownElement();
    if (dropdownElement == null) {
      return;
    }

    const dropdownBoundary = this.getDropdownBoundaryElContentRect();

    const dropdownElementContentContainerEl = this._getDropdownElementContentContainerEl(
      dropdownElement
    );
    if (dropdownElementContentContainerEl != null) {
      dropdownElement.style.minHeight = this.minDropdownHeight + 'px';
    }

    if (
      !this.dropdownSameWidth &&
      !this.allowDropdownWithOverBoundaryHorizontal
    ) {
      dropdownElement.style.maxWidth =
        OpalSelectComponent.dropdownMaxBoundaryScale * dropdownBoundary.width +
        'px';
    }

    if (this.maxDropdownHeight == null || this.maxDropdownHeight <= 0) {
      const dropdownElementBoundary = dropdownElement.getBoundingClientRect();
      if (
        dropdownElementBoundary.top >= dropdownBoundary.top &&
        dropdownElementBoundary.top < dropdownBoundary.bottom
      ) {
        const maxHeightPreventOverflowBoundary =
          dropdownBoundary.bottom -
          dropdownElementBoundary.top -
          OpalSelectComponent.minDistanceToBoundary;
        dropdownElement.style.maxHeight =
          (maxHeightPreventOverflowBoundary >= this.minDropdownDynamicHeight
            ? maxHeightPreventOverflowBoundary
            : this.minDropdownDynamicHeight) + 'px';
      } else {
        dropdownElement.style.maxHeight = 'initial';
      }
    } else {
      dropdownElement.style.maxHeight = this.maxDropdownHeight + 'px';
    }
  }

  private _displayDropdown(): void {
    const dropdownElement = this.getDropdownElement();
    if (dropdownElement == null) {
      return;
    }
    dropdownElement.style.visibility = 'visible';
    this._scrollToSelectedItem(dropdownElement);
  }

  private _scrollToSelectedItem(dropdownElement: HTMLElement): void {
    const selectedItemEl = this._getSelectedItemEl(dropdownElement);
    if (selectedItemEl != null) {
      DomUtils.scrollToView(selectedItemEl);
    }
  }

  private _getSelectedItemEl(dropdownElement: HTMLElement): Element {
    return dropdownElement.querySelector('.ng-option-selected');
  }
  private _setupDropdownCloseWhenClickOutside(
    notCloseWhenClickOutside: boolean
  ): void {
    // tslint:disable-next-line:no-any
    (this.ngSelect as any)._manualOpen = notCloseWhenClickOutside;
    if (this._originalHandleMousedown == null) {
      this._originalHandleMousedown = this.ngSelect.handleMousedown.bind(
        this.ngSelect
      );
    }
    this.ngSelect.handleMousedown = ($event: MouseEvent) => {
      // tslint:disable-next-line:no-any
      (this.ngSelect as any)._manualOpen = false;
      if (this._originalHandleMousedown) {
        this._originalHandleMousedown($event);
      }
      // tslint:disable-next-line:no-any
      (this.ngSelect as any)._manualOpen = notCloseWhenClickOutside
        ? true
        : false;
    };
  }

  private _triggerRenderItemsRangeForVirtualScroll(): void {
    // tslint:disable-next-line:no-any
    (this.ngSelect.itemsList as any)._filteredItems = Utils.clone(
      this.ngSelect.itemsList.filteredItems
    );
    this.ngSelect.detectChanges();
  }

  private _emitChange(): void {
    this.selectedValueChange.emit(this.selectedValue);
    this.selectedItemChange.emit(this.getSelectedItem());
    if (this.onChange != null) {
      this.onChange(this.selectedValue);
    }
  }

  private _getNewCombinedWithFetchedItems(
    apiResult: Array<string | number | object>
  ): Array<string | number | object> {
    return Utils.distinctBy(
      apiResult.concat(
        this.items.filter((p) => this._checkItemOfSelectedValue(p))
      ),
      (p) => this._getItemValue(p)
    );
  }

  private _checkItemOfSelectedValue(item: string | number | object): boolean {
    if (this.selectedValue == null || this.selectedValue === '') {
      return false;
    }
    if (this.selectedValue instanceof Array) {
      const selectedValueMap = Utils.toDictionarySelect(
        this.selectedValue,
        (p) => p,
        (p) => true
      );
      return (
        (typeof item === 'string' && selectedValueMap[item]) ||
        (item != null && selectedValueMap[item[this.valueField]])
      );
    } else {
      return this._checkItemOfValue(item, this.selectedValue);
    }
  }

  private _checkItemOfValue(
    item: string | number | object,
    singleItemValue: unknown
  ): boolean {
    return (
      item === singleItemValue ||
      (item != null && item[this.valueField] === singleItemValue)
    );
  }

  // tslint:disable-next-line:no-any
  private _getItemValue(item: string | number | object): any {
    // return typeof item !== 'object' ? item : item[this.valueField];
    if (typeof item !== 'object') {
      return item;
    }

    let itemValue = Utils.cloneDeep(item);

    this.valueField.split('.').forEach((property: string) => {
      itemValue = itemValue[property];
    });

    return itemValue;
  }

  private _getItemLabel(item: string | number | object): string | null {
    if (item == null) {
      return null;
    }
    return typeof item !== 'object' ? item.toString() : item[this.labelField];
  }

  private _getAllAvailableItems(): Array<string | number | object> {
    return this.items.length >= this.ngSelect.itemsList.items.length
      ? this.items
      : this.ngSelect.itemsList.items.map((p) => p.value);
  }

  private _setSearchTerm(value: string): void {
    const prevSearchTerm = this._searchTerm;
    this._searchTerm = value;
    if (
      this.fetchDataFn != null &&
      !this.fetchNoPaging &&
      prevSearchTerm !== value
    ) {
      this._loadDebounce(this._searchTerm);
    }
  }

  private _setupCloseOnSelect(): void {
    if (this.closeOnSelect == null) {
      this.closeOnSelect = this.multiple ? false : true;
    }
  }
}
