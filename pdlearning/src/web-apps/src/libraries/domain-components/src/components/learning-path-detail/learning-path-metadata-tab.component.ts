import { BaseComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import { MetadataTagModel, SearchTag, TaggingRepository } from '@opal20/domain-api';

import { CheckedState } from '@progress/kendo-angular-treeview';
import { FormGroup } from '@angular/forms';
import { LearningPathDetailMode } from './../../models/learning-path-detail-mode.model';
import { LearningPathDetailViewModel } from './../../view-models/learning-path-detail-view.model';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
  selector: 'learning-path-metadata-tab',
  templateUrl: './learning-path-metadata-tab.component.html'
})
export class LearningPathMetadataTabComponent extends BaseComponent {
  @Input() public form: FormGroup;
  @Input() public isViewMode: boolean;
  @Input() public subjectAreaItemIsCheckedFn: (dataItem: MetadataTagModel, index: string) => CheckedState;
  @Input() public learningFrameworkItemIsCheckedFn: (dataItem: MetadataTagModel, index: string) => CheckedState;
  @Input() public learningDimensionAreaItemIsCheckedFn: (dataItem: MetadataTagModel, index: string) => CheckedState;
  @Input()
  public set learningPathDetailVM(v: LearningPathDetailViewModel) {
    if (Utils.isDifferent(v, this._learningPathDetailVM)) {
      this._learningPathDetailVM = v;
    }
  }

  public get learningPathDetailVM(): LearningPathDetailViewModel {
    return this._learningPathDetailVM;
  }

  public fetchSuggestedSearchTagsFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<SearchTag[]>;

  public LearningPathDetailMode: typeof LearningPathDetailMode = LearningPathDetailMode;

  private _learningPathDetailVM: LearningPathDetailViewModel;

  constructor(public moduleFacadeService: ModuleFacadeService, private taggingRepository: TaggingRepository) {
    super(moduleFacadeService);
    this.fetchSuggestedSearchTagsFn = this._createFetchSuggestedSearchTagsFn();
  }

  private _createFetchSuggestedSearchTagsFn(): (searchText: string, skipCount: number, maxResultCount: number) => Observable<SearchTag[]> {
    return (searchText, skipCount, maxResultCount) => {
      return this.taggingRepository
        .loadSearchTags(
          {
            searchText: searchText,
            pagedInfo: {
              skipCount: skipCount,
              maxResultCount: maxResultCount
            }
          },
          false
        )
        .pipe(map(_ => _.items));
    };
  }
}
