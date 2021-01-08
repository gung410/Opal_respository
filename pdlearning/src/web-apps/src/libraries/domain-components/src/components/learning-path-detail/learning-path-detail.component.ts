import { BaseComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, ElementRef, Input, ViewChild } from '@angular/core';
import { Course, CourseRepository, MetadataTagModel, SearchCourseType } from '@opal20/domain-api';

import { CheckedState } from '@progress/kendo-angular-treeview';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { FormGroup } from '@angular/forms';
import { LearningPathDetailMode } from './../../models/learning-path-detail-mode.model';
import { LearningPathDetailViewModel } from './../../view-models/learning-path-detail-view.model';
import { LearningPathTabInfo } from './../../models/learning-path-tab.model';
import { Observable } from 'rxjs';
import { ScrollableMenu } from '@opal20/common-components';
import { map } from 'rxjs/operators';

@Component({
  selector: 'learning-path-detail',
  templateUrl: 'learning-path-detail.component.html'
})
export class LearningPathDetailComponent extends BaseComponent {
  @Input() public stickyDependElement: HTMLElement;
  @Input() public stickySpacing: number;
  public get learningPathDetailVM(): LearningPathDetailViewModel {
    return this._learningPathDetailVM;
  }

  @Input() public openCoursePreviewDialogFn: (courseId: string) => DialogRef;
  @Input('learningPathDetailVM')
  public set learningPathDetailVM(v: LearningPathDetailViewModel) {
    if (Utils.isDifferent(this._learningPathDetailVM, v)) {
      this._learningPathDetailVM = v;
    }
  }
  @Input() public mode: LearningPathDetailMode;

  @Input() public form: FormGroup;

  @ViewChild('basicInfoTabContainer', { static: true })
  public basicInfoTabContainer: ElementRef;
  @ViewChild('pdOpportunitiesTabContainer', { static: true })
  public pdOpportunitiesTabContainer: ElementRef;
  @ViewChild('metadataTabContainer', { static: true })
  public metadataTabContainer: ElementRef;

  public tabs: ScrollableMenu[] = [
    {
      id: LearningPathTabInfo.BasicInfo,
      title: 'Basic Information',
      elementFn: () => {
        return this.basicInfoTabContainer;
      }
    },
    {
      id: LearningPathTabInfo.PdOpportunities,
      title: 'PD Opportunities',
      elementFn: () => {
        return this.pdOpportunitiesTabContainer;
      }
    },
    {
      id: LearningPathTabInfo.Metadata,
      title: 'Metadata',
      elementFn: () => {
        return this.metadataTabContainer;
      }
    }
  ];

  public fetchPublishedCoursesFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<Course[]> = null;
  public subjectAreaItemIsCheckedFn: (dataItem: MetadataTagModel, index: string) => CheckedState;
  public learningFrameworkItemIsCheckedFn: (dataItem: MetadataTagModel, index: string) => CheckedState;
  public learningDimensionAreaItemIsCheckedFn: (dataItem: MetadataTagModel, index: string) => CheckedState;

  public activeTab: string = LearningPathTabInfo.BasicInfo;
  public loadingData: boolean = false;
  private _learningPathDetailVM: LearningPathDetailViewModel;

  constructor(public moduleFacadeService: ModuleFacadeService, private courseRepository: CourseRepository) {
    super(moduleFacadeService);
    this.fetchPublishedCoursesFn = this.createFetchPublishedCoursesFn();
    this.subjectAreaItemIsCheckedFn = this.tagTvItemIsCheckedFnFactory(() => {
      return this.learningPathDetailVM.subjectAreaIds;
    });

    this.learningFrameworkItemIsCheckedFn = this.tagTvItemIsCheckedFnFactory(() => {
      return this.learningPathDetailVM.learningFrameworkIds;
    });

    this.learningDimensionAreaItemIsCheckedFn = this.tagTvItemIsCheckedFnFactory(() => {
      return this.learningPathDetailVM.learningDimensionAreas;
    });
  }

  public asViewMode(mode: LearningPathDetailMode): boolean {
    return mode === LearningPathDetailMode.View;
  }

  public asEditMode(mode: LearningPathDetailMode): boolean {
    return mode === LearningPathDetailMode.Edit || mode === LearningPathDetailMode.NewLearningPath;
  }

  public tagTvItemIsCheckedFnFactory(checkedKeysFn: () => string[]): (dataItem: MetadataTagModel, index: string) => CheckedState {
    return (dataItem: MetadataTagModel, index: string) => {
      if (
        checkedKeysFn().indexOf(dataItem.tagId) > -1 ||
        (dataItem.childs !== undefined &&
          dataItem.childs.length > 0 &&
          Utils.includesAll(checkedKeysFn(), dataItem.childs.map(p => p.tagId)))
      ) {
        return 'checked';
      }

      if (this.tagTvItemIsIndeterminate(dataItem.childs, checkedKeysFn)) {
        return 'indeterminate';
      }

      return 'none';
    };
  }

  private createFetchPublishedCoursesFn(): (searchText: string, skipCount: number, maxResultCount: number) => Observable<Course[]> {
    return (searchText: string, skipCount: number, maxResultCount: number) =>
      this.courseRepository
        .loadSearchCourses(searchText, null, SearchCourseType.Learner, null, skipCount, maxResultCount, null, null, false)
        .pipe(map(_ => _.items));
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
}
