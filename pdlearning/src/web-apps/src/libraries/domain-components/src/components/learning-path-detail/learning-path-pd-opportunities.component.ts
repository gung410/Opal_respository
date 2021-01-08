import { BaseComponent, FileUploaderSetting, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';

import { Course } from '@opal20/domain-api';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { FormGroup } from '@angular/forms';
import { LearningPathDetailViewModel } from '../../view-models/learning-path-detail-view.model';
import { LearningPathPdOpportunitiesDialogComponent } from '../learning-path-pd-opportunities-dialog/learning-path-pd-opportunities-dialog.component';
import { Observable } from 'rxjs';
import { OpalDialogService } from '@opal20/common-components';

@Component({
  selector: 'learning-path-pd-opportunities',
  templateUrl: './learning-path-pd-opportunities.component.html'
})
export class LearningPathPdOpportunitiesComponent extends BaseComponent {
  @Input() public isViewMode: boolean;

  @Input() public openCoursePreviewDialogFn: (courseId: string) => DialogRef;
  public get learningPathDetailVM(): LearningPathDetailViewModel {
    return this._learningPathDetailVM;
  }

  @Input()
  public set learningPathDetailVM(v: LearningPathDetailViewModel) {
    if (Utils.isDifferent(v, this._learningPathDetailVM)) {
      this._learningPathDetailVM = v;
    }
  }
  public fileUploaderSetting: FileUploaderSetting;

  @Input()
  public form: FormGroup;

  @Input()
  public fetchPublishedCoursesFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<Course[]> = null;

  private _learningPathDetailVM: LearningPathDetailViewModel;

  constructor(public moduleFacadeService: ModuleFacadeService, private opalDialogService: OpalDialogService) {
    super(moduleFacadeService);
    this.fileUploaderSetting = new FileUploaderSetting();
  }

  public openPdOpportunities(): void {
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(LearningPathPdOpportunitiesDialogComponent, {
      learningPathDetailVM: Utils.cloneDeep(this.learningPathDetailVM),
      isViewMode: this.isViewMode,
      fetchPublishedCoursesFn: this.fetchPublishedCoursesFn,
      openCoursePreviewDialogFn: this.openCoursePreviewDialogFn
    });

    this.subscribe(dialogRef.result, (data: { learningPathDetailVM: LearningPathDetailViewModel }) => {
      if (data && data.learningPathDetailVM) {
        this.learningPathDetailVM.coursesDict = data.learningPathDetailVM.coursesDict;
        this.learningPathDetailVM.listCourses = data.learningPathDetailVM.listCourses;
      }
    });
  }
}
