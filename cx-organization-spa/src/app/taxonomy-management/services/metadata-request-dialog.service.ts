import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { MetadataTagGroupCode } from '../constant/metadata-tag-group-code.enum';
import { MetadataTagModel } from '../models/metadata-tag.model';
import { MetadataRequestApiService } from './metadata-request-api.services';

@Injectable()
export class MetadataRequestDialogService {
  metadatas$: BehaviorSubject<MetadataTagModel[]> = new BehaviorSubject<
    MetadataTagModel[]
  >(null);

  private _serviceSchemes$: BehaviorSubject<
    MetadataTagModel[]
  > = new BehaviorSubject<MetadataTagModel[]>(null);

  get allMetadatas(): MetadataTagModel[] {
    return this.metadatas$.getValue();
  }

  get serviceSchemes(): MetadataTagModel[] {
    return this._serviceSchemes$.getValue();
  }

  constructor(private metadataRequestApiService: MetadataRequestApiService) {}

  async getMetadataTag(): Promise<MetadataTagModel[]> {
    const metadatas = this.metadatas$.getValue();

    if (metadatas) {
      return metadatas;
    }

    const metadataFromApi = await this.metadataRequestApiService.getAllMetaDataTags();

    this.metadatas$.next(metadataFromApi);

    return metadataFromApi;
  }

  async getServiceSchemeInfoById(tagId: string): Promise<MetadataTagModel> {
    const serviceSchemes = await this.getServiceScheme();

    return serviceSchemes.find((service) => service.id === tagId);
  }

  async getServiceScheme(): Promise<MetadataTagModel[]> {
    const serviceScheme = this._serviceSchemes$.getValue();

    if (serviceScheme) {
      return serviceScheme;
    }

    const metadatas = await this.getMetadataTag();
    const filteredServiceScheme = metadatas
      .filter((m) => m?.groupCode === MetadataTagGroupCode.SERVICE_SCHEMES)
      .filter((m) => m?.codingScheme !== 'NA');

    this._serviceSchemes$.next(filteredServiceScheme);

    return filteredServiceScheme;
  }

  getMetadataInfo(tagId: string): string {
    const metadata = this.getMetadataById(tagId);

    if (!metadata) {
      return '';
    }

    const codingScheme = metadata.codingScheme ? metadata.codingScheme : '';

    return codingScheme.length
      ? `${metadata.displayText} (${codingScheme})`
      : metadata.displayText;
  }

  getMetadataById(tagId: string): MetadataTagModel {
    const metadatas = this.metadatas$.getValue();

    if (metadatas && metadatas.length) {
      return metadatas.find((metadata) => metadata.id === tagId);
    }

    return null;
  }

  getPathByMetadataId(tagId: string, currentServiceSchemeId: string): string {
    const parents = this.getParents(tagId);
    const currentServiceScheme = this.serviceSchemes.filter(
      (serviceScheme) => serviceScheme.tagId === currentServiceSchemeId
    )[0];

    const path: string[] = [currentServiceScheme.codingScheme].concat(
      parents.map((metadata) => metadata.displayText)
    );

    return path.join(' / ');
  }

  private getParents(
    itemId: string,
    pathItems: MetadataTagModel[] = []
  ): MetadataTagModel[] {
    if (!itemId) {
      return pathItems;
    }

    const metadataItem = this.allMetadatas.find(
      (metadata) => metadata.tagId === itemId
    );
    pathItems = [metadataItem].concat(pathItems);

    return this.getParents(metadataItem.parentTagId, pathItems);
  }
}
