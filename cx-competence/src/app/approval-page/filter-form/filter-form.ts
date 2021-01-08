import { ChangeDetectorRef, OnDestroy } from '@angular/core';
import { IDictionary } from 'app-models/dictionary';
import { BaseComponent } from 'app/shared/components/component.abstract';
import { FilterSlidebarService } from '../services/filter-slidebar.service';

export interface IFilterForm {
  resetFilterForm(): void;
  getFilterParam(): IDictionary<unknown>;
  applyFilterForm(): void;
  getCurrentFilterValues(): void;
}

export abstract class BaseFilterFormComponent
  extends BaseComponent
  implements OnDestroy, IFilterForm {
  disableDatePicker: boolean = false;
  constructor(
    protected filterSlidebarService: FilterSlidebarService,
    protected changeDetectorRef: ChangeDetectorRef
  ) {
    super(changeDetectorRef);

    this.subscriptionAdder = this.filterSlidebarService.onResetFilter.subscribe(
      () => {
        this.resetFilterForm();
      }
    );
  }
  public getFilterParam(): IDictionary<unknown> {
    return {};
  }

  public resetFilterForm(): void {
    // Virtual method
  }

  public applyFilterForm(): void {}

  //This is to force Date-Picker compnent fired the Onchange's event
  public getCurrentFilterValues(): void {
    this.disableDatePicker = true;
    this.changeDetectorRef.detectChanges();
    this.disableDatePicker = false;
  }
}
