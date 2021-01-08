import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input, ViewEncapsulation } from '@angular/core';
import { IQuerySearchTagRequest, ResourceModel, SearchTag, TaggingApiService } from '@opal20/domain-api';

import { FormDetailMode } from '@opal20/domain-components';
import { FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
  selector: 'form-metadata',
  templateUrl: './form-metadata.component.html',
  encapsulation: ViewEncapsulation.None
})
export class FormMetadataComponent extends BaseComponent {
  public FormDetailMode: typeof FormDetailMode = FormDetailMode;
  @Input() public form: FormGroup;
  @Input()
  public mode: FormDetailMode = FormDetailMode.Edit;
  @Input()
  public resource: ResourceModel = new ResourceModel();

  public customSearchTagAddedFn: (searchTag: string) => Promise<SearchTag>;
  public fetchSearchTagsFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<SearchTag[]>;
  public fetchSearchTagsByNamesFn: (names: string[]) => Observable<SearchTag[]>;
  constructor(public moduleFacadeService: ModuleFacadeService, private taggingBackendSvc: TaggingApiService) {
    super(moduleFacadeService);
    this.customSearchTagAddedFn = (searchTag: string) => {
      this.resource.searchTags.push(searchTag);
      return new Promise(resolve => {
        resolve(
          new SearchTag({
            name: searchTag
          })
        );
      });
    };
    this.fetchSearchTagsByNamesFn = (searchTags: string[]) => {
      return this.taggingBackendSvc.getSearchTagByNames(searchTags);
    };
    this.fetchSearchTagsFn = (searchText: string, skipCount: number, maxResultCount: number) => {
      const request: IQuerySearchTagRequest = {
        searchText: searchText,
        pagedInfo: {
          skipCount: skipCount,
          maxResultCount: maxResultCount
        }
      };
      return this.taggingBackendSvc.querySearchTag(request, false).pipe(map(response => response.items));
    };
  }
}
