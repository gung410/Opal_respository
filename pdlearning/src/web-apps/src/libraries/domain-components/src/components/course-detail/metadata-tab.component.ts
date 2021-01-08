import { BaseComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import { MetadataTagModel, SearchTag, TaggingRepository } from '@opal20/domain-api';

import { CheckedState } from '@progress/kendo-angular-treeview';
import { CourseDetailComponent } from './course-detail.component';
import { CourseDetailMode } from '../../models/course-detail-mode.model';
import { CourseDetailViewModel } from '../../view-models/course-detail-view.model';
import { FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
  selector: 'metadata-tab',
  templateUrl: './metadata-tab.component.html'
})
export class MetadataTabComponent extends BaseComponent {
  @Input() public form: FormGroup;
  @Input() public course: CourseDetailViewModel;
  @Input() public mode: CourseDetailMode | undefined;
  public subjectAreaItemIsCheckedFn: (dataItem: MetadataTagModel, index: string) => CheckedState;
  public learningFrameworkItemIsCheckedFn: (dataItem: MetadataTagModel, index: string) => CheckedState;
  public learningDimensionAreaItemIsCheckedFn: (dataItem: MetadataTagModel, index: string) => CheckedState;
  public teacherOutcomeItemIsCheckedFn: (dataItem: MetadataTagModel, index: string) => CheckedState;
  public fetchSuggestedSearchTagsFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<SearchTag[]>;

  public CourseDetailMode: typeof CourseDetailMode = CourseDetailMode;
  constructor(public moduleFacadeService: ModuleFacadeService, private taggingRepository: TaggingRepository) {
    super(moduleFacadeService);

    this.subjectAreaItemIsCheckedFn = this.tagTvItemIsCheckedFnFactory(() => {
      return this.course.subjectAreaIds;
    });

    this.learningFrameworkItemIsCheckedFn = this.tagTvItemIsCheckedFnFactory(() => {
      return this.course.learningFrameworkIds;
    });

    this.learningDimensionAreaItemIsCheckedFn = this.tagTvItemIsCheckedFnFactory(() => {
      return this.course.learningDimensionAreas;
    });

    this.teacherOutcomeItemIsCheckedFn = this.tagTvItemIsCheckedFnFactory(() => {
      return this.course.teacherOutcomeIds;
    });

    this.fetchSuggestedSearchTagsFn = this._createFetchSuggestedSearchTagsFn();
  }

  public asViewMode(): boolean {
    return CourseDetailComponent.asViewMode(this.mode);
  }

  public asViewModeForCompletingCourseForPlanning(): boolean {
    return CourseDetailComponent.asViewModeForCompletingCourseForPlanning(this.course);
  }
  public isPlanningVerificationRequired(): boolean {
    return CourseDetailComponent.isPlanningVerificationRequired(this.course);
  }

  public canViewFieldOfCourseInPlanningCycle(): boolean {
    return CourseDetailComponent.canViewFieldOfCourseInPlanningCycle(this.course);
  }

  public tagTvItemIsCheckedFnFactory(checkedKeysFn: () => string[]): (dataItem: MetadataTagModel, index: string) => CheckedState {
    return (dataItem: MetadataTagModel, index: string) => {
      const checkedKeys = checkedKeysFn();
      if (
        checkedKeys.indexOf(dataItem.tagId) > -1 ||
        (dataItem.childs !== undefined && dataItem.childs.length > 0 && Utils.includesAll(checkedKeys, dataItem.childs.map(p => p.tagId)))
      ) {
        return 'checked';
      }

      if (this.tagTvItemIsIndeterminate(dataItem.childs, checkedKeysFn)) {
        return 'indeterminate';
      }

      return 'none';
    };
  }

  private tagTvItemIsIndeterminate(itemChilds: MetadataTagModel[] | undefined, checkedKeysFn: () => string[]): boolean {
    if (itemChilds === undefined) {
      return false;
    }
    let idx = 0;
    let item: MetadataTagModel;
    const checkKeysDic = Utils.toDictionary(checkedKeysFn());

    while ((item = itemChilds[idx])) {
      if (checkKeysDic[item.tagId] != null || this.tagTvItemIsIndeterminate(item.childs, checkedKeysFn)) {
        return true;
      }

      idx += 1;
    }

    return false;
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
