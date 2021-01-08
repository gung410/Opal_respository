import { Injectable } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { EntityStatusEnum } from 'app-enums/entityStatusEnum';
import { User } from 'app-models/auth.model';
import { findIndexCommon } from 'app/shared/constants/common.const';
import { FilterParamModel } from 'app/staff/staff.container/staff-list/models/filter-param.model';
import { ToastrService } from 'ngx-toastr';
import { CxPeopleListDialogComponent } from './cx-people-list-dialog/cx-people-list-dialog.component';
import { CxPeoplePickerDialogComponent } from './cx-people-picker-dialog/cx-people-picker-dialog.component';
import { StartingHierarchyDepartment } from './cx-people-picker-dialog/starting-hierarchy-department.enum';

@Injectable({
  providedIn: 'root',
})
export class CxPeoplePickerService {
  private successMessage: string =
    '{numberOflearners} {singular/plural} been added successfully.';
  private warningMessage: string =
    '{numberOflearners} {singular/plural} been added previously.';
  constructor(
    private ngbModal: NgbModal,
    private toastrService: ToastrService
  ) {}

  initQuestionPeoplePicker({
    currentUser,
    question,
    htmlElement,
    startingHierarchyDepartment,
    specificDepartmentId,
    windowClass = '',
  }: {
    currentUser: User;
    question: any;
    htmlElement: any;
    startingHierarchyDepartment: StartingHierarchyDepartment;
    specificDepartmentId?: number;
    windowClass?: string;
  }): void {
    question.onOpenPeoplePickerClicked = () => {
      const modalRef = this.ngbModal.open(CxPeoplePickerDialogComponent, {
        size: 'lg',
        windowClass,
      });
      const modalRefComponent = modalRef.componentInstance as CxPeoplePickerDialogComponent;
      modalRefComponent.currentUser = currentUser;
      modalRefComponent.maxNumberOfPeople = +question.maxRowCount;
      modalRefComponent.currentNumberOfPeople = question.value
        ? question.value.length
        : 0;
      modalRefComponent.supportFilterByServiceScheme = true;
      modalRefComponent.startingHierarchyDepartment = startingHierarchyDepartment;
      modalRefComponent.specifyStartingHierarchyDepartmentId = specificDepartmentId;
      modalRefComponent.includeUsersInDescendentDepartments = true;
      modalRefComponent.dialogTitle = question.addRowDialogTitle;
      modalRefComponent.done.subscribe((addingUsers: any[]) => {
        const newValues: number[] = question.value ? question.value : [];
        const duplicatedUserIds: number[] = [];

        addingUsers.forEach((addingUser) => {
          const addingUserId = addingUser.user.identity.id;
          if (newValues.indexOf(addingUserId) === findIndexCommon.notFound) {
            newValues.push(addingUserId);
          } else {
            duplicatedUserIds.push(addingUserId);
          }
        });
        this.checkDuplication(
          question,
          addingUsers.length,
          duplicatedUserIds.length
        );

        question.value = newValues;
        modalRef.close();
      });
      modalRefComponent.cancel.subscribe(() => {
        modalRef.close();
      });
    };

    question.onOpenSelectedPeopleClicked = () => {
      const modalRef = this.ngbModal.open(CxPeopleListDialogComponent, {
        size: 'lg',
      });
      const modalRefComponent = modalRef.componentInstance as CxPeopleListDialogComponent;
      modalRefComponent.currentUser = currentUser;
      modalRefComponent.editMode = question.survey.isEditMode;
      modalRefComponent.allowDeletion = modalRefComponent.editMode; // TODO: Handle who should be allowed.
      modalRefComponent.filterParams = new FilterParamModel({
        userIds: question.value,
        entityStatuses: [EntityStatusEnum.All],
      });
      modalRefComponent.dialogTitle = 'Targeted Individual Learner(s)'; // TODO: Put this as the custom widget property.
      modalRefComponent.done.subscribe((removingRows: any[]) => {
        let selectedUserIds = question.value ? question.value : [];
        if (
          selectedUserIds &&
          selectedUserIds.length &&
          removingRows &&
          removingRows.length > 0
        ) {
          const removingUserIds = removingRows.map(
            (r: any) => r.user.identity.id
          );
          selectedUserIds = selectedUserIds.filter(
            (userId) =>
              removingUserIds.indexOf(userId) === findIndexCommon.notFound
          );
        }
        question.value = selectedUserIds;
        modalRef.close();
      });
      modalRefComponent.cancel.subscribe(() => {
        modalRef.close();
      });
    };
  }

  private checkDuplication(
    question: any,
    numberOfAddingUsers: number,
    numberOfDuplicatedUsers: number
  ): void {
    const successMessage = this.buildMessage(
      this.successMessage,
      numberOfAddingUsers - numberOfDuplicatedUsers
    );
    const warningMessage = this.buildMessage(
      this.warningMessage,
      numberOfDuplicatedUsers
    );
    if (!numberOfDuplicatedUsers) {
      this.toastrService.success(successMessage);

      return;
    }
    if (question.showDuplicatedWarning && numberOfDuplicatedUsers) {
      if (numberOfAddingUsers === numberOfDuplicatedUsers) {
        this.toastrService.warning(warningMessage);
      } else {
        this.toastrService.warning(warningMessage);
        this.toastrService.success(successMessage);
      }

      return;
    }
    this.toastrService.error('Something wrong.');
  }

  private buildMessage(message: string, numberOflearners: number): string {
    return message
      .replace('{numberOflearners}', `${numberOflearners}`)
      .replace(
        '{singular/plural}',
        `${numberOflearners > 1 ? 'learners have' : 'learner has'}`
      );
  }
}
