import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, Output } from '@angular/core';

import { AdvanceSearchCatalogModel } from '../models/catalogue-adv-search-filter.model';
import { CatalogueDataService } from '../services/catalogue-data.service';
import { MetadataTagGroupCode } from '@opal20/domain-api';

@Component({
  selector: 'learner-catalog-search-filter',
  templateUrl: './learner-catalogue-search-filter.component.html'
})
export class CatalogSearchFilterComponent extends BaseComponent {
  @Output() public close = new EventEmitter<void>();
  @Output() public applyFilter = new EventEmitter<void>();

  public get model(): AdvanceSearchCatalogModel {
    return this.catalogDataService.model;
  }
  public loadedData: boolean = false;
  public metadataTagGroupCode: typeof MetadataTagGroupCode = MetadataTagGroupCode;
  public showMore: boolean = false;

  constructor(protected moduleFacadeService: ModuleFacadeService, private catalogDataService: CatalogueDataService) {
    super(moduleFacadeService);
  }

  public onShowComponent(): void {
    this.showMore = false;
  }

  public onClose(): void {
    this.catalogDataService.cancel();
    this.close.emit();
  }

  public applyAndClose(): void {
    this.catalogDataService.applyFilter();
    this.applyFilter.emit();
    this.close.emit();
  }

  public clearFilter(): void {
    this.catalogDataService.clearFilter();
  }

  public toggleShowMore(): void {
    this.showMore = !this.showMore;
  }
}
