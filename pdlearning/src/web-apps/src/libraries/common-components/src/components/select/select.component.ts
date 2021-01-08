import { BaseComponent, DomUtils, IHasFocusableProcessing, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import {
  ChangeDetectorRef,
  Component,
  ContentChild,
  ElementRef,
  EventEmitter,
  HostBinding,
  Input,
  OnInit,
  Output,
  TemplateRef,
  ViewChild,
  ViewEncapsulation,
  forwardRef
} from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { NgOption, NgSelectComponent } from '@ng-select/ng-select';
import { Observable, Subscription, of } from 'rxjs';
import { OpalLabelTemplateDirective, OpalOptionTemplateDirective } from './templates.directives';

import { Constant } from '@opal20/authentication';

@Component({
  selector: 'opal-select',
  templateUrl: './select.component.html',
  encapsulation: ViewEncapsulation.None,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => OpalSelectComponent),
      multi: true
    }
  ]
})
export class OpalSelectComponent extends BaseComponent implements ControlValueAccessor, OnInit, IHasFocusableProcessing {
  @HostBinding('class.-opened') public get isOpenedHostBinding(): boolean {
    return this.isOpened;
  }
  @HostBinding('class.-disabled') public get disabledHostBinding(): boolean {
    return this.disabled;
  }
  @HostBinding('class.-readonly') public get readOnlyHostBinding(): boolean {
    return this.readOnly;
  }

  @HostBinding('class.-push-down-mode') public get pushDownModeHostBinding(): boolean {
    return this.pushDownMode;
  }
  @HostBinding('class.-static-mode') public get staticModeHostBinding(): boolean {
    return this.staticMode;
  }
  @HostBinding('class.-multiple-inline') public get multipleInlineHostBinding(): boolean {
    return this.multipleInline;
  }
  @HostBinding('class.-multiple-inline-no-wrap') public get multipleInlineNoWrapHostBinding(): boolean {
    return this.multipleInlineNoWrap;
  }
  @HostBinding('class.-clearable') public get clearableHostBinding(): boolean {
    return this.clearable;
  }
  public get items(): Array<object | string | number> {
    return this._items;
  }
  @Input() public set items(value: Array<object | string | number>) {
    if (value == null) {
      value = [];
    }
    if (!Utils.isDifferent(this._items, value)) {
      return;
    }
    if (this.loading) {
      this._prevItems = Utils.clone(this._items);
      this._prevAllItemValuesDictionary = Utils.toDictionary(this._items, i => this._getItemValue(i));
    }
    this._items = this.ignoreItemFn != null ? value.filter(p => !this.ignoreItemFn(p) || this.isItemMatchSelectedValue(p)) : value;
    this._selectAllItems = this._items.filter(
      p => this.selectAllIgnoreItemLabel == null || this._getItemLabel(p) !== this.selectAllIgnoreItemLabel
    );
    this._allItemValuesDictionary = Utils.toDictionary(this.items, i => this._getItemValue(i));
    if (value.length >= OpalSelectComponent.autoVirtualScrollBreakPoint) {
      this.virtualScroll = true;
    }

    if (this.initiated) {
      this.itemsChangeEvent.emit(this._items);
    }
  }
  public get placeholder(): string {
    return this._placeholder;
  }
  @Input() public set placeholder(value: string) {
    if (this._placeholder === value) {
      return;
    }
    this._placeholder = Utils.isNullOrEmpty(value) ? OpalSelectComponent.defaultPlaceholder : value;
  }
  public get selectedValue(): unknown | unknown[] {
    return this._selectedValue;
  }
  @Input() public set selectedValue(value: unknown | unknown[]) {
    if (this._selectedValue === value) {
      return;
    }
    this._selectedValue = value === '' ? null : value;
    this.selectedItemLabel = this.getSelectedItemLabel();
    this.detectChanges(this.changeDetectorRef);
  }
  public get pushDownMode(): boolean {
    return this._pushDownMode;
  }
  @Input()
  public set pushDownMode(v: boolean) {
    this._pushDownMode = v;
    if (!this.initiated) {
      return;
    }
    this._setupDropdownCloseWhenClickOutside(this.pushDownMode || this.notCloseDropdownOnFocusOut);
  }

  public get notCloseDropdownOnFocusOut(): boolean {
    return this._notCloseDropdownOnFocusOut;
  }
  @Input()
  public set notCloseDropdownOnFocusOut(v: boolean) {
    this._notCloseDropdownOnFocusOut = v;
    if (!this.initiated) {
      return;
    }
    this._setupDropdownCloseWhenClickOutside(this.pushDownMode || this.notCloseDropdownOnFocusOut);
  }

  public get isOpened(): boolean {
    return this.ngSelect.isOpen;
  }
  public set isOpened(v: boolean) {
    this.ngSelect.isOpen = v;
    this.detectChanges(this.changeDetectorRef, undefined, undefined, true);
  }
  public get ngSelectNgClass(): object {
    return {
      '-borderless': this.borderless,
      '-dropdown-borderless': this.dropdownBorderless,
      '-dropdown-same-width': this.dropdownSameWidth,
      '-no-scroll': this.oneItemAsContainer
    };
  }
  public get element(): HTMLElement {
    return this.elementRef.nativeElement;
  }
  public static readonly defaultPlaceholder = '\u00a0';
  public static readonly minDistanceToBoundary = 10;
  public static readonly dropdownMaxBoundaryScale = 0.9;
  public static readonly autoVirtualScrollBreakPoint = 500;

  @ContentChild(OpalOptionTemplateDirective, { read: TemplateRef, static: false }) public optionTemplate: TemplateRef<unknown>;
  @ContentChild(OpalLabelTemplateDirective, { read: TemplateRef, static: false }) public labelTemplate: TemplateRef<unknown>;
  @Input() public fetchPageSize: number = 25;
  @Input() public fetchDataByValuesFn?: (values: unknown[]) => Observable<(object | string | number)[]>;
  public get fetchDataFn():
    | ((searchText: string, skipCount: number, maxResultCount: number) => Observable<(object | string | number)[]>)
    | null {
    return this._fetchDataFn;
  }
  @Input()
  public set fetchDataFn(
    v: ((searchText: string, skipCount: number, maxResultCount: number) => Observable<(object | string | number)[]>) | null
  ) {
    if (this._fetchDataFn === v) {
      return;
    }
    this._fetchDataFn = v;
    if (this.initiated && this.fetchedOnce) {
      this.fetch();
    }
  }
  public get fetchDataLogicDependenciesInput(): unknown {
    return this._fetchDataLogicDependenciesInput;
  }
  @Input()
  public set fetchDataLogicDependenciesInput(v: unknown) {
    if (Utils.isDifferent(this._fetchDataLogicDependenciesInput, v)) {
      this._fetchDataLogicDependenciesInput = v;
      if (this.initiated && this.fetchedOnce) {
        Utils.delay(() => this.fetch(), 100, this.onDestroy$);
      }
    }
  }

  @Input() public ignoreItemFn: (item: unknown) => boolean;
  @Input() public valueField: string = 'value';
  @Input() public labelField: string = 'label';
  @Input() public searchFields?: string[];
  @Input() public endOfGroupField: string = 'endOfGroup';
  @Input() public borderless: boolean = false;
  @Input() public clearable: boolean = false;
  @Input() public searchable: boolean = true;
  @Input() public hasMoreDataToFetch: boolean = false;
  @Input() public dropdownBorderless: boolean = false;
  @Input() public dropdownSameWidth: boolean = true;
  @Input() public allowDropdownWithOverBoundaryHorizontal: boolean = false;
  @Input() public multiple: boolean = false;
  @Input() public disabled: boolean = false;
  @Input() public readOnly: boolean = false;
  @Input() public dropdownBoundary: string | undefined | HTMLElement;
  @Input() public minDropdownHeight: number = 250;
  @Input() public minDropdownDynamicHeight: number = 80;
  @Input() public maxDropdownHeight: number | undefined;
  @Input() public clearAllText: string = 'Clear all';
  @Input() public virtualScroll: boolean = false;
  @Input() public oneItemAsContainer: boolean = false;
  @Input() public autoScrollToViewOnOpen: boolean = false;
  @Input() public addTag: boolean = false;
  @Input() public selectOnTab: boolean = true;
  @Input() public addTagOnFocusOut: boolean = false;
  @Input() public staticMode: boolean = false;
  @Input() public addTagText: string = 'Add item';
  @Input() public multipleInline: boolean = true;
  @Input() public multipleInlineNoWrap: boolean = false;
  @Input() public closeOnSelect: boolean | undefined;
  @Input() public clearSearchOnAdd: boolean = false;
  @Input() public customMarkedItemClass: string | undefined;
  @Input() public customSelectedItemClass: string | undefined;
  @Input() public labelForId: string | undefined;
  @Input() public inputAttrs: { [key: string]: string } = {};
  @Input() public tabIndex: number | undefined;
  @Input() public notFoundText: string = 'No items found';
  @Input() public searchFn: (term: string, item: object | string | number) => boolean = null;
  @Input() public dropdownItemLabelWrap: boolean = false;
  @Input() public fetchNoPaging: boolean = false;
  @Input() public disableSelectAll: boolean | undefined;
  @Input() public changeConfirmationFn: (selectedItem: unknown) => Promise<boolean>;
  @Input() public fetchOnInit: boolean = false;
  @Input() public notClearSearchTextOnFocusOut: boolean = false;
  @Input() public dropdownPosition: 'bottom' | 'top' | 'auto' = 'bottom';
  @Input() public fetchOnOpen: boolean = false;

  public _selectAllIgnoreItemLabel?: string;
  public get selectAllIgnoreItemLabel(): string | null {
    return this._selectAllIgnoreItemLabel;
  }
  @Input()
  public set selectAllIgnoreItemLabel(v: string | null) {
    if (this._selectAllIgnoreItemLabel === v) {
      return;
    }
    this._selectAllIgnoreItemLabel = v;
    this._selectAllItems = this._items.filter(
      p => this.selectAllIgnoreItemLabel == null || this._getItemLabel(p) !== this.selectAllIgnoreItemLabel
    );
  }

  @Output() public open: EventEmitter<unknown> = new EventEmitter<unknown>();
  @Output() public close: EventEmitter<unknown> = new EventEmitter<unknown>();
  @Output() public selectedValueChange: EventEmitter<unknown> = new EventEmitter<unknown>();
  @Output() public selectedItemChange: EventEmitter<unknown> = new EventEmitter<unknown>();
  @Output() public blur: EventEmitter<unknown> = new EventEmitter<unknown>();
  @Output('focus') public focusEvent: EventEmitter<unknown> = new EventEmitter<unknown>();
  @Output() public clear: EventEmitter<unknown> = new EventEmitter<unknown>();
  @Output('itemsChange') public itemsChangeEvent: EventEmitter<Array<object | string | number>> = new EventEmitter();

  @ViewChild('ngSelect', { static: true }) public ngSelect: NgSelectComponent;
  public onChange?: (_: unknown) => void;
  public onTouched?: (_: unknown) => void;
  public selectedItemLabel: string | string[] | undefined;
  public loading: boolean = false;
  public fetchedOnce: boolean = false;

  protected destroyed: boolean = false;

  private _items: Array<object | string | number> = [];
  private _allItemValuesDictionary: Dictionary<unknown> = {};
  private _prevAllItemValuesDictionary: Dictionary<unknown> = {};
  private _selectAllItems: Array<object | string | number> = [];
  private _prevItems: Array<object | string | number> = [];
  private _placeholder: string = OpalSelectComponent.defaultPlaceholder;
  private _selectedValue: unknown | unknown[] = undefined;
  private _pushDownMode: boolean = false;
  private _notCloseDropdownOnFocusOut: boolean = false;
  private _searchTerm: string = '';
  private _originalHandleMousedown: (($event: MouseEvent) => void) | undefined;
  private _fetchDataSub: Subscription = new Subscription();
  private _fetchDataByValuesSub: Subscription = new Subscription();
  private _noMoreItemsToFetch: boolean = false;
  private _currentFetchSkip: number = 0;
  private _loadDebounce = Utils.debounce((term?: string, onDone?: () => void) => {
    this.fetch(term, 'reset', onDone);
  }, 700);
  private _fetchMoreDebounce = Utils.debounce((term?: string, onDone?: () => void) => {
    this.fetchMore(term, onDone);
  }, 300);
  private _fetchDataFn:
    | ((searchText: string, skipCount: number, maxResultCount: number) => Observable<(object | string | number)[]>)
    | null;
  private _fetchDataLogicDependenciesInput: unknown;
  private _isDataFetchedOnce: boolean = false;

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    public changeDetectorRef: ChangeDetectorRef,
    public elementRef: ElementRef
  ) {
    super(moduleFacadeService);
  }

  public ngOnInit(): void {
    this.selectedItemLabel = this.getSelectedItemLabel();
    this._setupDropdownCloseWhenClickOutside(this.pushDownMode || this.notCloseDropdownOnFocusOut);
    this._setupCloseOnSelect();
    this.checkToFetchItemsForSelectedValue();
    if (this.fetchOnInit === true) {
      this.fetch();
    }
    super.ngOnInit();
  }

  public ngOnDestroy(): void {
    super.ngOnDestroy();
    this.initiated = false;
    this.destroyed = true;
  }

  public onSelectedValueChange(newValue: unknown): void {
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
    this.changeConfirmationFn(newValue).then(result => {
      if (result) {
        this.selectedValue = newValue;
      }
    });
  }

  public fetchByValues(): void {
    this._fetchDataByValuesSub.unsubscribe();
    this.loading = false;
    if (this.fetchDataByValuesFn == null || Utils.isNullOrEmpty(this.selectedValue)) {
      return;
    }

    this.loading = true;
    this._fetchDataByValuesSub = this.fetchDataByValuesFn(this.selectedValue instanceof Array ? this.selectedValue : [this.selectedValue])
      .pipe(this.untilDestroy())
      .subscribe(
        apiResult => {
          this.items = this._getNewCombinedWithFetchedItems(apiResult);
          this.loading = false;
          if (this.isOpened) {
            this.processDropdown(null, this.getCurrentVerticalScrollValue(), 100);
          }
        },
        error => {
          this.loading = false;
        }
      );
  }

  public fetch(term?: string, mode: 'reset' | 'concat' = 'reset', onDone?: () => void): void {
    this.loading = false;
    if (this.fetchDataFn == null || (this._noMoreItemsToFetch && mode === 'concat')) {
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
    this._fetchDataSub = this.fetchDataFn(term, this._currentFetchSkip, this.fetchPageSize)
      .pipe(this.untilDestroy())
      .subscribe(
        apiResult => {
          if (this.loading) {
            this._currentFetchSkip = this._currentFetchSkip + this.fetchPageSize;
          }

          const filteredApiResult = this.loading
            ? apiResult.filter(r => this._allItemValuesDictionary[this._getItemValue(r)] === undefined)
            : apiResult.filter(r => this._prevAllItemValuesDictionary[this._getItemValue(r)] === undefined);
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
            this.processDropdown(null, this.getCurrentVerticalScrollValue(), 100);
          }
          if (this.loading && onDone != null) {
            onDone();
          }
          this.loading = false;
          this.fetchedOnce = true;
          if (!this.initiated) {
            this.itemsChangeEvent.emit(this._items);
          }
        },
        error => {
          this.loading = false;
          if (onDone != null) {
            onDone();
          }
        }
      );
  }

  public fetchMore(term?: string, onDone?: () => void): void {
    this.fetch(term, 'concat', onDone);
  }

  public writeValue(value: unknown | unknown[]): void {
    this.selectedValue = value;
    this.checkToFetchItemsForSelectedValue();
  }
  public registerOnChange(fn: (_: unknown) => void): void {
    this.onChange = fn;
  }
  public registerOnTouched(fn: (_: unknown) => void): void {
    this.onTouched = fn;
  }

  public getDropdownElement(): HTMLElement | undefined {
    return <HTMLElement | undefined>this.element.querySelector('.ng-dropdown-panel');
  }

  public onChanged(e: unknown): void {
    const prevTerm = this._searchTerm;
    this._emitChange();
    this._setSearchTerm(this.ngSelect.filterValue || '');
    this.detectChanges(this.changeDetectorRef, true);
    if (this.notClearSearchTextOnFocusOut) {
      this.ngSelect.filterValue = prevTerm;
      this._setSearchTerm(prevTerm);
    }
  }

  public onFocused(e: unknown): void {
    if (this.onTouched != null) {
      this.onTouched(e);
    }
    this.focusEvent.emit(e);

    const event = document.createEvent('MouseEvents');
    event.initEvent('mousedown', true, true);
    document.dispatchEvent(event);
  }

  public onOpened(e: unknown): void {
    if (this.fetchOnOpen) {
      this.fetch(null, 'reset');
      this.processDropdown(() => {
        this.open.emit(e);
      });
      return;
    }

    if (this.fetchDataFn != null && (this.items.length <= 1 || !this._isDataFetchedOnce)) {
      this.fetchMore(this.ngSelect.filterValue, () => {
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

  public getCurrentVerticalScrollValue(): number | null {
    const dropdownElement = this.getDropdownElement();
    if (dropdownElement == null) {
      return null;
    }
    return this._getDropdownScrollContainerEl(dropdownElement).scrollTop;
  }

  public processDropdown(onDone?: () => void, keepScrollPositionValue?: number, delayTime?: number): void {
    const doProcessDropdown = () => {
      Utils.doInterval(
        intervalSubscriber => {
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

  public onClosed(e: unknown): void {
    this.close.emit(e);
  }

  public getSelectedItem(): (object | string | number) | (object | string | number)[] | undefined {
    if (this.selectedValue == null || this.selectedValue === '') {
      return undefined;
    }
    const items = this._getAllAvailableItems();
    if (this.selectedValue instanceof Array) {
      const selectedValueMap = Utils.toDictionarySelect(this.selectedValue, p => p, p => true);
      return items.filter(p => {
        const itemValue = this._getItemValue(p);
        return itemValue != null && selectedValueMap[itemValue];
      });
    } else {
      return items.find(p => {
        return this._checkItemOfValue(p, this.selectedValue);
      });
    }
  }

  public isItemMatchSelectedValue(item: object | string | number): boolean {
    if (this.selectedValue instanceof Array) {
      return this.selectedValue.find(value => value === this._getItemValue(item));
    } else {
      return this._getItemValue(item) === this.selectedValue;
    }
  }

  public getSelectedItemLabel(): string | string[] | undefined {
    const selectedItem = this.getSelectedItem();
    if (selectedItem == null) {
      return undefined;
    }
    if (selectedItem instanceof Array) {
      return selectedItem.map(p => {
        return typeof p === 'string' ? p : p[this.labelField];
      });
    }
    return typeof selectedItem === 'string' ? selectedItem : selectedItem[this.labelField];
  }

  public onSearched(e: string | undefined | { term: string; items: Array<string | object> }): void {
    this._setSearchTerm(e == null ? '' : typeof e === 'string' ? e.trim() : e.term.trim());
  }

  public onFocusOut($event: unknown): void {
    if (this.addTag && this.addTagOnFocusOut) {
      if (this._searchTerm) {
        let searchTermItem = this.ngSelect.itemsList.findItem(this._searchTerm);
        if (searchTermItem == null) {
          // Fix bug addItem two time because of same reference
          if (this.ngSelect.itemsList.filteredItems === this.ngSelect.itemsList.items) {
            // tslint:disable-next-line:no-any
            (<any>this.ngSelect.itemsList)._filteredItems = Utils.clone(this.ngSelect.itemsList.filteredItems);
          }
          searchTermItem = this.ngSelect.itemsList.addItem(this._searchTerm);
        }

        if (this.multiple) {
          if (this.selectedValue == null || (this.selectedValue instanceof Array && this.selectedValue.indexOf(this._searchTerm) < 0)) {
            this.selectedValue = this.selectedValue == null ? [this._searchTerm] : [...(<unknown[]>this.selectedValue), this._searchTerm];
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

  public getDropdownBoundaryElContentRect(): ClientRect {
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
      boundaryEl = <HTMLElement | null>this.element.closest(this.dropdownBoundary);
    }
    if (boundaryEl == null) {
      return windowViewClientRect;
    }
    return DomUtils.getElementContentRect(boundaryEl);
  }

  public closeDropdown(): void {
    if (this.isOpened === false) {
      return;
    }
    if (this.pushDownMode) {
      // tslint:disable-next-line:no-any
      (<any>this.ngSelect)._manualOpen = false;
      this.ngSelect.close();
      // tslint:disable-next-line:no-any
      (<any>this.ngSelect)._manualOpen = true;
    } else {
      this.ngSelect.close();
    }
  }

  public openDropdown(): void {
    if (this.isOpened) {
      return;
    }
    if (this.pushDownMode) {
      // tslint:disable-next-line:no-any
      (<any>this.ngSelect)._manualOpen = false;
      this.ngSelect.open();
      // tslint:disable-next-line:no-any
      (<any>this.ngSelect)._manualOpen = true;
    } else {
      this.ngSelect.open();
    }
  }

  public scrollToViewOnOpened(): void {
    DomUtils.scrollToView(this.element, true);
  }

  public scrollToSelectedItem(): void {
    const dropdownElement = this.getDropdownElement();
    if (dropdownElement != null) {
      this._scrollToSelectedItem(dropdownElement);
    }
  }

  public getSelectedItemEl(): Element {
    const dropdownEl = this.getDropdownElement();
    if (dropdownEl == null) {
      return undefined;
    }
    return this._getSelectedItemEl(dropdownEl);
  }

  public onCleared(e: unknown): void {
    this.clear.emit(e);
  }

  public onBlurred(e: unknown): void {
    this.blur.emit(e);
  }

  public toggle(): void {
    if (!this.isOpened) {
      this.openDropdown();
    } else {
      this.closeDropdown();
    }
  }

  public selectItemNgClass(item: object, item$: NgOption): object {
    const result = { '-end-of-group': item[this.endOfGroupField] === true };
    if (this.ngSelect.itemsList.markedItem === item$ && this.customMarkedItemClass != null) {
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

  public focus(): void {
    DomUtils.scrollToView(this.element);
    this.ngSelect.focus();
  }

  public canFocus(): boolean {
    return this.searchable && !this.readOnly;
  }

  public searchFnCreator(): ((term: string, item: object | string | number) => boolean) | null {
    if (this.searchFields != null) {
      return (term: string, item: object | string | number) => {
        const lowerCaseTerm = term ? term.toLocaleLowerCase().trim() : '';
        return Utils.any(this.searchFields, fieldName => {
          if (typeof item !== 'object') {
            return false;
          }

          const fieldValue = item[fieldName] ? <string>item[fieldName].toString() : '';
          return (
            fieldValue
              .toLocaleLowerCase()
              .trim()
              .indexOf(lowerCaseTerm) >= 0
          );
        });
      };
    }
    return this.searchFn;
  }

  public scrollToPosition(position: number): void {
    const dropdownEl = this.getDropdownElement();
    if (dropdownEl == null) {
      return;
    }

    const dropdownScrollContainerEl = this._getDropdownScrollContainerEl(dropdownEl);
    dropdownScrollContainerEl.scrollTop = position;
  }

  public onScrollToEnd(e: unknown): void {
    if (!this.fetchNoPaging) {
      this._fetchMoreDebounce(this.ngSelect.filterValue);
    }
  }

  public selectAllItem(): void {
    const fetchAllLeftItemsObs = this.fetchDataFn ? this.fetchDataFn(null, Constant.MAX_ITEMS_PER_REQUEST, 0) : of([]);
    fetchAllLeftItemsObs.subscribe(allLeftItems => {
      if (allLeftItems) {
        const oldValues = new Set(this.items.map(item => this._getItemValue(item)));
        this.items = this.items.concat(allLeftItems.filter(newItem => !oldValues.has(this._getItemValue(newItem))));
      }
      this.selectedValue = this.isSelectAll ? [] : this._selectAllItems.map(x => this._getItemValue(x));
      this._emitChange();
    });
  }

  public showSelectAll(): boolean {
    return this.items.length > 0 && this.multiple && !this.disableSelectAll;
  }

  public get isSelectAll(): boolean {
    return this.multiple && this.selectedValue instanceof Array && this.selectedValue.length >= this._selectAllItems.length;
  }

  private checkToFetchItemsForSelectedValue(): void {
    if (this.items.length === 0 && this.fetchDataByValuesFn != null && !Utils.isNullOrEmpty(this.selectedValue)) {
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
      const dropdownElementComputedStyle = window.getComputedStyle(dropdownElement);
      if (dropdownElementComputedStyle.left == null) {
        throw new Error('Bravo-select left must be defined.');
      }

      const currentDropdownElementStyleLeft = parseFloat(dropdownElementComputedStyle.left.replace('px', ''));
      const dropdownRightOverBoundaryLength = dropdownElementBoundary.right - dropdownBoundary.right;
      const nextToRightBoundaryDropdownStyleLeft =
        currentDropdownElementStyleLeft - dropdownRightOverBoundaryLength - OpalSelectComponent.minDistanceToBoundary;

      const nextToLeftBoundaryDropdownStyleLeft =
        currentDropdownElementStyleLeft -
        (this.element.getBoundingClientRect().left - dropdownElementBoundary.left) +
        OpalSelectComponent.minDistanceToBoundary;

      dropdownElement.style.left =
        dropdownElementBoundary.width <= dropdownBoundary.width
          ? nextToRightBoundaryDropdownStyleLeft + 'px'
          : nextToLeftBoundaryDropdownStyleLeft + 'px';
    }
  }

  private _getDropdownElementContentContainerEl(dropdownElement: HTMLElement): Element {
    return dropdownElement.querySelector('.ng-dropdown-panel-items > div:nth-child(2)');
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

    const dropdownElementContentContainerEl = this._getDropdownElementContentContainerEl(dropdownElement);
    if (dropdownElementContentContainerEl != null) {
      dropdownElement.style.minHeight = this.minDropdownHeight + 'px';
    }

    if (!this.dropdownSameWidth && !this.allowDropdownWithOverBoundaryHorizontal) {
      dropdownElement.style.maxWidth = OpalSelectComponent.dropdownMaxBoundaryScale * dropdownBoundary.width + 'px';
    }

    if (this.maxDropdownHeight == null || this.maxDropdownHeight <= 0) {
      const dropdownElementBoundary = dropdownElement.getBoundingClientRect();
      if (dropdownElementBoundary.top >= dropdownBoundary.top && dropdownElementBoundary.top < dropdownBoundary.bottom) {
        const maxHeightPreventOverflowBoundary =
          dropdownBoundary.bottom - dropdownElementBoundary.top - OpalSelectComponent.minDistanceToBoundary;
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
  private _setupDropdownCloseWhenClickOutside(notCloseWhenClickOutside: boolean): void {
    // tslint:disable-next-line:no-any
    (<any>this.ngSelect)._manualOpen = notCloseWhenClickOutside;
    if (this._originalHandleMousedown == null) {
      this._originalHandleMousedown = this.ngSelect.handleMousedown.bind(this.ngSelect);
    }
    this.ngSelect.handleMousedown = ($event: MouseEvent) => {
      // tslint:disable-next-line:no-any
      (<any>this.ngSelect)._manualOpen = false;
      if (this._originalHandleMousedown) {
        this._originalHandleMousedown($event);
      }
      // tslint:disable-next-line:no-any
      (<any>this.ngSelect)._manualOpen = notCloseWhenClickOutside ? true : false;
    };
  }

  private _triggerRenderItemsRangeForVirtualScroll(): void {
    // tslint:disable-next-line:no-any
    (<any>this.ngSelect.itemsList)._filteredItems = Utils.clone(this.ngSelect.itemsList.filteredItems);
    this.ngSelect.detectChanges();
  }

  private _emitChange(): void {
    this.selectedValueChange.emit(this.selectedValue);
    this.selectedItemChange.emit(this.getSelectedItem());
    if (this.onChange != null) {
      this.onChange(this.selectedValue);
    }
  }

  private _getNewCombinedWithFetchedItems(apiResult: (string | number | object)[]): (string | number | object)[] {
    return Utils.distinctBy(apiResult.concat(this.items.filter(p => this._checkItemOfSelectedValue(p))), p => this._getItemValue(p));
  }

  private _checkItemOfSelectedValue(item: string | number | object): boolean {
    if (this.selectedValue == null || this.selectedValue === '') {
      return false;
    }
    if (this.selectedValue instanceof Array) {
      const selectedValueMap = Utils.toDictionarySelect(this.selectedValue, p => p, p => true);
      return (typeof item === 'string' && selectedValueMap[item]) || (item != null && selectedValueMap[item[this.valueField]]);
    } else {
      return this._checkItemOfValue(item, this.selectedValue);
    }
  }

  private _checkItemOfValue(item: string | number | object, singleItemValue: unknown): boolean {
    return item === singleItemValue || (item != null && item[this.valueField] === singleItemValue);
  }

  // tslint:disable-next-line:no-any
  private _getItemValue(item: string | number | object): any {
    return typeof item !== 'object' ? item : item[this.valueField];
  }

  private _getItemLabel(item: string | number | object): string | null {
    if (item == null) {
      return null;
    }
    return typeof item !== 'object' ? item.toString() : item[this.labelField];
  }

  private _getAllAvailableItems(): (string | number | object)[] {
    return this.items.length >= this.ngSelect.itemsList.items.length ? this.items : this.ngSelect.itemsList.items.map(p => p.value);
  }

  private _setSearchTerm(value: string): void {
    const prevSearchTerm = this._searchTerm;
    this._searchTerm = value;
    if (this.fetchDataFn != null && !this.fetchNoPaging && prevSearchTerm !== value) {
      this._loadDebounce(this._searchTerm);
    }
  }

  private _setupCloseOnSelect(): void {
    if (this.closeOnSelect == null) {
      this.closeOnSelect = this.multiple ? false : true;
    }
  }
}
