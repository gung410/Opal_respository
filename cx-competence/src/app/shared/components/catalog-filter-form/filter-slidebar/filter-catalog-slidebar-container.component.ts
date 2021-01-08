import {
  ChangeDetectorRef,
  Component,
  ComponentFactoryResolver,
  ComponentRef,
  Type,
  ViewChild,
  ViewEncapsulation,
} from '@angular/core';
import { CxSlidebarComponent } from '@conexus/cx-angular-common';
import { BaseComponent } from 'app/shared/components/component.abstract';
import { FilterCatalogFormPlaceholderDirective } from '../directives/filter-catalog-form-placeholder.derective';
import { IFilterForm } from '../filter-form/filter-form';
import { PdoFilterFormComponent } from '../filter-form/pdo/pdo-filter-form.component';
import { FilterCatalogSlidebarService } from '../services/filter-catalog-slidebar.service';

@Component({
  selector: 'filter-slidebar-container',
  templateUrl: './filter-catalog-slidebar-container.component.html',
  styleUrls: ['./filter-catalog-slidebar-container.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class FilterCatalogueSlidebarContainerComponent extends BaseComponent {
  public numberOfActiveFilter: number;

  @ViewChild(CxSlidebarComponent, { static: true })
  private slidebarComponent: CxSlidebarComponent;

  @ViewChild(FilterCatalogFormPlaceholderDirective, { static: true })
  private filterFormPlaceholder: FilterCatalogFormPlaceholderDirective;

  private componentRef: ComponentRef<any>;
  private filterFormComponent: Type<any>;

  constructor(
    private filterSlidebarService: FilterCatalogSlidebarService,
    private componentFactoryResolver: ComponentFactoryResolver,
    protected changeDetectorRef: ChangeDetectorRef
  ) {
    super(changeDetectorRef);

    this.filterSlidebarService.initSlidebar(PdoFilterFormComponent);
    this.subscriptionAdder = this.filterSlidebarService.onInitFilterForm.subscribe(
      (filterFormComponent: Type<any>) => {
        const oldFilterFormComponent = this.filterFormComponent;
        this.filterFormComponent = filterFormComponent;

        // Rerender filter form if the form component was changed
        if (
          oldFilterFormComponent &&
          filterFormComponent &&
          oldFilterFormComponent.name !== filterFormComponent.name
        ) {
          this.renderFilterForm();
        }
      }
    );

    this.subscriptionAdder = this.filterSlidebarService.onSubmitFilter.subscribe(
      (filterParams) => {
        this.numberOfActiveFilter = Object.keys(filterParams).filter(
          (key) => filterParams[key] !== undefined && filterParams[key] !== null
        ).length;
        this.changeDetectorRef.detectChanges();
      }
    );

    this.subscriptionAdder = this.filterSlidebarService.onResetFilter.subscribe(
      () => {
        this.numberOfActiveFilter = 0;
      }
    );
  }

  public openFilterSlidebar(): void {
    this.slidebarComponent.openSlidebar(true);
    this.renderFilterForm();
    (this.componentRef.instance as IFilterForm).getCurrentFilterValues();
  }

  public onSubmitFilter(): void {
    const params = (this.componentRef.instance as IFilterForm).getFilterParam();
    this.filterSlidebarService.onSubmitFilter.next(params);
    (this.componentRef.instance as IFilterForm).applyFilterForm();
    this.slidebarComponent.closeSlidebar();
  }

  public onResetFilter(): void {
    this.filterSlidebarService.onResetFilter.next(true);
    this.filterSlidebarService.onSubmitFilter.next({});
  }

  public onClearFilter(): void {
    this.filterSlidebarService.onResetFilter.next(true);
  }

  public renderFilterForm(): void {
    if (
      !this.componentRef &&
      this.filterFormComponent &&
      this.filterFormPlaceholder
    ) {
      this.filterFormPlaceholder.viewContainerRef.clear();

      const componentFactory = this.componentFactoryResolver.resolveComponentFactory(
        this.filterFormComponent
      );

      this.componentRef = this.filterFormPlaceholder.viewContainerRef.createComponent(
        componentFactory
      );
      this.changeDetectorRef.detectChanges();
    }
  }
}
