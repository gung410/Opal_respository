import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild
} from '@angular/core';
import {
  CxConfirmationDialogComponent,
  CxSurveyjsComponent,
  CxSurveyjsEventModel
} from '@conexus/cx-angular-common';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { DepartmentType } from 'app-models/department-type.model';
import { UserType } from 'app-models/user-type.model';
import { DateTimeUtil } from 'app-utilities/date-time-utils';
import { DepartmentStoreService } from 'app/core/store-services/department-store.service';
import { StatusActionTypeEnum } from 'app/shared/constants/status-action-type.enum';
import { StatusTypeEnum } from 'app/shared/constants/user-status-type.enum';
import { ApprovalInfoTabModel } from '../edit-user-dialog/edit-user-dialog.model';
import { approveUserFormJSON } from '../form/approve-user-form';
import { UserManagement } from '../models/user-management.model';

@Component({
  selector: 'user-account-confirmation-dialog',
  templateUrl: './user-account-confirmation-dialog.component.html',
  styleUrls: ['./user-account-confirmation-dialog.component.scss']
})
export class UserAccountConfirmationDialogComponent implements OnInit {
  get hasLearnerRole(): boolean {
    return UserManagement.hasLearnerRole(this.getUsersSystemRoles());
  }
  @Input() message: string;
  @Input() doneButtonText: string;
  @Input() number: number;
  @Input() items: any[];
  @Input() status: string;
  @Output() done: EventEmitter<any> = new EventEmitter();
  @Output() cancel: EventEmitter<any> = new EventEmitter();
  minDate: string;
  expirationDate: Date;
  response: any = {};
  doneButtonDisabled: boolean = false;
  showRejectComment: boolean = false;
  rejectReason: string;
  showDatePicker: boolean = false;
  isFinalApproval: boolean;
  approveUserFormJSON: any = approveUserFormJSON;
  approveUserData: any;
  isProcessingSingleUser: boolean;
  @ViewChild('approveUserSurveyForm')
  approveUserSurveyForm: CxSurveyjsComponent;
  isFirstLevelApproval: boolean;
  firstLevelApprovalData: ApprovalInfoTabModel;
  /**
   * The department identifier of the first user in the list of selected users.
   */
  firstLevelApprovalDepartmentId: number;
  firstLevelApprovalDepartmentTypes: DepartmentType[];

  constructor(
    private departmentStoreService: DepartmentStoreService,
    public ngbModal: NgbModal
  ) {}

  async ngOnInit(): Promise<void> {
    this.isProcessingSingleUser = this.number === 1;
    this.minDate = DateTimeUtil.toDateString(
      new Date(),
      'YYYY-MM-DD'
    ).toString();
    this.showDatePicker =
      this.status === StatusActionTypeEnum.SetExpirationDate;
    if (this.showDatePicker) {
      this.doneButtonDisabled = true;
    }
    this.showRejectComment =
      this.status === StatusActionTypeEnum.Reject ||
      this.status === StatusTypeEnum.Deactive.code;
    this.initFirstLevelApprovalInfo();
    this.initFinalApprovalInfo();
  }

  onDateChanged(newDate: Date): void {
    this.expirationDate = newDate;
    this.doneButtonDisabled =
      this.showDatePicker && this.expirationDate === undefined;
  }

  onApproveUserChanged(surveyEvent: CxSurveyjsEventModel): void {
    const surveyDate = surveyEvent.survey.data;
    this.doneButtonDisabled = this.validateRequiredFieldsOnApproveUserForm(
      surveyDate
    );
  }

  onComplete(): void {
    this.response.date = DateTimeUtil.toDateString(
      this.expirationDate,
      'DD/MM/YYYY'
    );
    this.response.rejectReason = this.rejectReason;
    if (this.isFirstLevelApproval) {
      if (
        !(
          !this.firstLevelApprovalData.primaryApprovalGroup &&
          !this.firstLevelApprovalData.alternateApprovalGroup
        )
      ) {
        this.proceedToApproveUser1stLevel();
      } else {
        this.showNonAssignAOConfirmationDialog(
          () => {
            this.proceedToApproveUser1stLevel();
          },
          'Confirm',
          'You have not assigned Approving Officer/Alternate Approving Officer for this user. Proceed?'
        );
      }
    } else if (this.isFinalApproval) {
      this.approveUserSurveyForm.doComplete();
    } else {
      this.done.emit(this.response);
    }
  }

  onSubmittingApproveUserForm(surveyEvent: CxSurveyjsEventModel): void {
    const surveyDate = surveyEvent.survey.data;
    this.response.finalApprovalInfo = surveyDate;
    this.done.emit(this.response);
  }

  onCancel(): void {
    this.cancel.emit();
  }

  getUsersSystemRoles(): UserType[] {
    const systemRoles: UserType[] = [];
    this.items.map((user) =>
      user.systemRoles.map((role) => {
        systemRoles.push(role);
      })
    );

    return systemRoles;
  }

  private proceedToApproveUser1stLevel(): void {
    this.response.primaryApprovalGroup = this.firstLevelApprovalData.primaryApprovalGroup;
    this.response.alternateApprovalGroup = this.firstLevelApprovalData.alternateApprovalGroup;

    this.done.emit(this.response);
  }

  private async initFirstLevelApprovalInfo(): Promise<void> {
    this.isFirstLevelApproval =
      this.status === StatusActionTypeEnum.Accept &&
      this.items &&
      this.items[0].entityStatus.statusId ===
        StatusTypeEnum.PendingApproval1st.code;
    if (this.isFirstLevelApproval) {
      this.firstLevelApprovalData = new ApprovalInfoTabModel();
      const firstUser = this.items[0];
      this.firstLevelApprovalDepartmentId = firstUser.departmentId;
      this.firstLevelApprovalDepartmentTypes = await this.departmentStoreService.getDepartmentTypesByDepartmentIdToPromise(
        firstUser.departmentId
      );
    }
  }

  private initFinalApprovalInfo(): void {
    this.isFinalApproval =
      this.status === StatusActionTypeEnum.Accept &&
      this.items &&
      (this.items[0].entityStatus.statusId ===
        StatusTypeEnum.PendingApproval2nd.code ||
        this.items[0].entityStatus.statusId ===
          StatusTypeEnum.PendingApproval3rd.code);
    if (this.isProcessingSingleUser && this.isFinalApproval) {
      const firstUser = this.items[0];
      this.approveUserData = {
        activeDate: DateTimeUtil.toSurveyFormat(
          firstUser.entityStatus.activeDate
        ),
        expirationDate: DateTimeUtil.toSurveyFormat(
          firstUser.entityStatus.expirationDate
        ),
        setDateOption: 'forSelectedUsers',
        isProcessingSingleUser: this.isProcessingSingleUser
      };
    }
    this.doneButtonDisabled =
      this.isFinalApproval &&
      this.validateRequiredFieldsOnApproveUserForm(this.approveUserData);
  }

  private validateRequiredFieldsOnApproveUserForm(
    approveUserData: any
  ): boolean {
    return (
      !approveUserData ||
      !approveUserData.activeDate ||
      !approveUserData.expirationDate
    );
  }

  private showNonAssignAOConfirmationDialog(
    onConfirmed: () => void,
    headerName: string,
    content: string
  ): void {
    const cxConfirmationDialogModalRef = this.ngbModal.open(
      CxConfirmationDialogComponent,
      {
        size: 'sm',
        centered: true
      }
    );

    const cxConfirmationDialogModal = cxConfirmationDialogModalRef.componentInstance as CxConfirmationDialogComponent;
    cxConfirmationDialogModal.showConfirmButton = true;
    cxConfirmationDialogModal.showCloseButton = true;
    cxConfirmationDialogModal.confirmButtonText = 'Confirm';
    cxConfirmationDialogModal.cancelButtonText = 'Cancel';
    cxConfirmationDialogModal.header = headerName;
    cxConfirmationDialogModal.content = content;

    cxConfirmationDialogModal.confirm.subscribe(() => {
      onConfirmed();
      cxConfirmationDialogModalRef.close();
    });
    cxConfirmationDialogModal.cancel.subscribe(() => {
      cxConfirmationDialogModalRef.close();
    });
  }
}
