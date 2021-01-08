import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, ViewChild } from '@angular/core';
import { Course, CourseRepository, SearchCourseType } from '@opal20/domain-api';

import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { DialogRef } from '@progress/kendo-angular-dialog';
@Component({
  selector: 'copy-metadata-dialog',
  templateUrl: './copy-metadata-dialog.component.html'
})
export class CopyMetadataDialogComponent extends BasePageComponent {
  public courseData: Course[] = [];
  public model: string;
  public selectedCourse: Course;
  public virtual: unknown = {
    itemHeight: 28
  };
  @ViewChild('comboBox', { static: true }) public comboBox: ComboBoxComponent;
  constructor(public moduleFacadeService: ModuleFacadeService, public dialogRef: DialogRef, private courseRepository: CourseRepository) {
    super(moduleFacadeService);
  }

  public onCancel(): void {
    this.dialogRef.close();
  }

  public onSelectionChange(event: Course): void {
    this.selectedCourse = event;
  }

  public onCourseImport(): void {
    if (this.selectedCourse) {
      this.dialogRef.close(this.selectedCourse);
    }
  }

  public openPopup(): void {
    this.comboBox.toggle(true);
  }

  protected onInit(): void {
    this.courseRepository.loadSearchCourses(null, null, SearchCourseType.CopyMetadataRight).subscribe(searchCourseResult => {
      this.courseData = searchCourseResult.items;
    });
  }
}
