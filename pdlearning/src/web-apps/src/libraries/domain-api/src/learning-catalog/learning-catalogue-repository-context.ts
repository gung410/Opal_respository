import { BaseRepositoryContext } from '@opal20/infrastructure';
import { BehaviorSubject } from 'rxjs';
import { IResource } from './models/catalog-search-results.model';
import { Injectable } from '@angular/core';

@Injectable()
export class LearningCatalogueRepositoryContext extends BaseRepositoryContext {
  public catalogueResourceSubject: BehaviorSubject<Dictionary<IResource>> = new BehaviorSubject({});
}
