import { BaseRepositoryContext } from '@opal20/infrastructure';
import { BehaviorSubject } from 'rxjs';
import { Injectable } from '@angular/core';
import { MetadataTagModel } from './models/metadata-tag.model';
import { ResourceMetadataModel } from './models/resource-metadata';
import { SearchTag } from './models/search-tag.model';

@Injectable()
export class TaggingRepositoryContext extends BaseRepositoryContext {
  public resourceMetadatasSubject: BehaviorSubject<Dictionary<ResourceMetadataModel>> = new BehaviorSubject({});
  public metadataTagsSubject: BehaviorSubject<Dictionary<MetadataTagModel>> = new BehaviorSubject({});
  public searchTagsSubject: BehaviorSubject<Dictionary<SearchTag>> = new BehaviorSubject({});
}
