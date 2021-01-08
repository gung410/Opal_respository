import { ChangeDetectorRef, OnDestroy } from '@angular/core';
import { IDictionary } from 'app-models/dictionary';
import { BaseComponent } from 'app/shared/components/component.abstract';
import { FilterCatalogSlidebarService } from '../services/filter-catalog-slidebar.service';

export interface IFilterForm {
  resetFilterForm(): void;
  getFilterParam(): IDictionary<unknown>;
  applyFilterForm(): void;
  getCurrentFilterValues(): void;
}

export abstract class BaseFilterFormComponent
  extends BaseComponent
  implements OnDestroy, IFilterForm {
  constructor(
    protected filterSlidebarService: FilterCatalogSlidebarService,
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

  public getCurrentFilterValues(): void {}
}
