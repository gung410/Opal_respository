import { CellClickEvent, PageChangeEvent } from '@progress/kendo-angular-grid';
import { ElementRef, EventEmitter, HostBinding, Input, Output } from '@angular/core';

import { BasePageComponent } from './base-page.component';
import { IGridFilter } from '../models/grid-filter.model';
import { ModuleFacadeService } from '../services/module-facade.service';
import { Utils } from '../utils/utils';

export abstract class BaseGridComponent<T extends IGridDataItem> extends BasePageComponent {
  @Input() public mergeHeader: boolean = false;
  @Input() public stickyDependElement?: HTMLElement | ElementRef;

  public get filter(): IGridFilter | undefined {
    return this._filter;
  }
  @Input()
  public set filter(v: IGridFilter | undefined) {
    if (Utils.isEqual(this._filter, v)) {
      return;
    }
    this._filter = v;
    if (this.initiated) {
      this.refreshData();
    }
  }

  public get selectedItems(): T[] {
    return this._selectedItems;
  }

  @Input()
  public set selectedItems(v: T[]) {
    this._selectedItems = Utils.clone(v);
    if (this.initiated) {
      this.updateSelectedsAndGridData();
    }
  }

  public checkAll: boolean = false;

  public gridData: OpalGridDataResult<T> | null;
  public selecteds = {};
  public state: PageChangeEvent = {
    skip: 0,
    take: 15
  };

  @Input() public indexActionColumns: number[] = [];

  @Output('selectedItemsChange')
  public selectedItemsChangeEvent: EventEmitter<T[]> = new EventEmitter<T[]>();

  protected _selectedItems: T[] = [];
  protected _filter: IGridFilter | undefined = {
    search: '',
    filter: null
  };
  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public onCheckAll(checked: boolean): void {
    this.checkAll = checked;

    // Clear list selected
    if (checked === false) {
      this.selectedItems = [];
      this.selecteds = {};
    }
    this.gridData.data.forEach(item => {
      if (checked && !this.selectedItems.find(x => x.id === item.id)) {
        this.selectedItems.push(item);
      }
      this.selecteds[item.id] = checked;
      item.selected = checked;
    });
    this.selectedItemsChangeEvent.emit(this.selectedItems);
  }

  public onCheckItem(checked: boolean, dataItem: T): void {
    this.selecteds[dataItem.id] = checked;
    if (checked) {
      if (!this.selectedItems.includes(dataItem)) {
        // clone new object to new array to ensure the setter work.
        const selectedItems = [...this.selectedItems];
        selectedItems.push(dataItem);
        this.selectedItems = selectedItems;
      }
    } else {
      this.selectedItems = Utils.removeFirst(this.selectedItems, dataItem) as T[];
    }

    this.checkAll = this.selectedItems.length === this.gridData.data.length;
    this.selectedItemsChangeEvent.emit(this.selectedItems);
  }

  public onPageChange(state: PageChangeEvent): void {
    this.state = state;
    this.selectedItems = [];
    this.loadData();
  }

  @HostBinding('class.base-opal-grid') public get isBaseOpalGrid(): boolean {
    return true;
  }

  public abstract onGridCellClick(event: CellClickEvent): void;

  public ngOnDestroy(): void {
    this.selectedItems = [];
    this.selectedItemsChangeEvent.emit(this.selectedItems);
    super.ngOnDestroy();
  }

  protected onInit(): void {
    this.refreshData();
  }

  protected abstract loadData(): void;

  protected refreshData(): void {
    this.selectedItems = [];
    this.state.skip = 0;
    this.loadData();
  }

  protected updateSelectedsAndGridData(): void {
    if (this.selectedItems.length === 0) {
      this.checkAll = false;
    } else if (this.selectedItems.length === this.gridData.data.length) {
      this.checkAll = true;
    }

    this.selecteds = {};
    if (this.gridData != null) {
      this.gridData.data.forEach(item => {
        if (this.selectedItems.includes(item)) {
          this.selecteds[item.id] = true;
          item.selected = true;
        } else {
          this.selecteds[item.id] = false;
          item.selected = false;
        }
      });
    }
  }
}
export interface IGridDataItem {
  id: string;
  selected: boolean;
}
