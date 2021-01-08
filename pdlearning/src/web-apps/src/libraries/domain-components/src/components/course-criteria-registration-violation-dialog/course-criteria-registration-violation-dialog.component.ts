import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import { IChangeRegistrationCourseCriteriaOverridedStatusRequest, RegistrationRepository } from '@opal20/domain-api';

import { CourseCriteriaRegistrationViolationDetailItemViewModel } from '../../models/course-criteria-registration-violation-detail-item-view.model';
import { DialogAction } from '@opal20/common-components';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { RegistrationViewModel } from '../../models/registration-view.model';
import { from } from 'rxjs';

@Component({
  selector: 'course-criteria-registration-violation-dialog',
  templateUrl: './course-criteria-registration-violation-dialog.component.html'
})
export class CourseCriteriaRegistrationViolationDialogComponent extends BaseComponent {
  @Input() public registrationVM: RegistrationViewModel;

  public courseCriteriaRegistrationViolationDetailItems: CourseCriteriaRegistrationViolationDetailItemViewModel[] = [];
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public dialogRef: DialogRef,
    private registrationRepository: RegistrationRepository
  ) {
    super(moduleFacadeService);
  }

  public overrideCourseCriteria(): void {
    const registrationChangeStatus: IChangeRegistrationCourseCriteriaOverridedStatusRequest = {
      registrationIds: [this.registrationVM.id],
      classrunId: this.registrationVM.classRunId
    };
    from(
      this.registrationRepository.overrideRegistrationCourseCriteria(registrationChangeStatus).then(() => {
        this.showNotification();
        this.onClose();
      })
    ).pipe(this.untilDestroy());
  }

  public onClose(): void {
    this.dialogRef.close({ action: DialogAction.Close });
  }

  protected onInit(): void {
    this.courseCriteriaRegistrationViolationDetailItems = this.registrationVM.buildCourseCriteriaRegistrationViolationDetailItems();
  }
}
