import { Injectable } from '@angular/core';
import { PDCatalogueEnumerationDto } from 'app/user-accounts/models/pd-catalogue.model';
import { PDCatalogueDataService } from 'app/user-accounts/services/pd-catalouge-data.service';

import { BaseArrayStoreService } from './base-array-store.service';
import { cloneDeep } from 'lodash';
import { BehaviorSubject } from 'rxjs';

@Injectable()
export class PDCatalogueStoreService extends BaseArrayStoreService<
  PDCatalogueEnumerationDto[]
> {
  private get entities(): { [id: string]: PDCatalogueEnumerationDto[] } {
    return this._entities;
  }
  private set entities(value: { [id: string]: PDCatalogueEnumerationDto[] }) {
    this._entities = Object.freeze(value);
  }
  private _entities: {
    [id: string]: PDCatalogueEnumerationDto[];
  } = Object.freeze({});
  constructor(private pdCatalougeDataService: PDCatalogueDataService) {
    super();
  }

  get(): BehaviorSubject<PDCatalogueEnumerationDto[]> {
    const currentValue = this.checkForCurrentData();
    if (currentValue) {
      return this.storeSubject;
    }
    this.pdCatalougeDataService.getPDCatalogue().subscribe(
      (data: PDCatalogueEnumerationDto[]) => {
        if (data) {
          this.storeSubject.next(data);
        }
      },
      (error) => {
        this.storeSubject.error(error);
      }
    );

    return this.storeSubject;
  }

  getPDCatalogueCategory(
    categoryId: string
  ): BehaviorSubject<PDCatalogueEnumerationDto[]> {
    const currentValue = this.checkForCurrentData();
    if (currentValue) {
      return this.storeSubject;
    }
    this.pdCatalougeDataService.getPDCategory(categoryId).subscribe(
      (data: PDCatalogueEnumerationDto[]) => {
        if (data) {
          this.storeSubject.next(data);
        }
      },
      (error) => {
        this.storeSubject.error(error);
      }
    );

    return this.storeSubject;
  }

  async getPDCatalogueAsync(
    categoryId: string
  ): Promise<PDCatalogueEnumerationDto[]> {
    const entities = this.entities;
    if (entities[categoryId]) {
      return entities[categoryId];
    }
    await this.addPdCatalogueAsync(categoryId);

    return this.entities[categoryId];
  }

  add(element: any): BehaviorSubject<PDCatalogueEnumerationDto[]> {
    throw new Error('Method not implemented.');
  }

  edit(element: any): BehaviorSubject<PDCatalogueEnumerationDto[]> {
    throw new Error('Method not implemented.');
  }

  delete(element: any): BehaviorSubject<PDCatalogueEnumerationDto[]> {
    throw new Error('Method not implemented.');
  }

  private async addPdCatalogueAsync(categoryId: string): Promise<void> {
    const cloneEntities = cloneDeep(this.entities);
    await this.pdCatalougeDataService
      .getPDCategory(categoryId)
      .toPromise()
      .then((data: PDCatalogueEnumerationDto[]) => {
        if (data) {
          cloneEntities[categoryId] = data;
          this.entities = cloneEntities;
        }
      });
  }
}
