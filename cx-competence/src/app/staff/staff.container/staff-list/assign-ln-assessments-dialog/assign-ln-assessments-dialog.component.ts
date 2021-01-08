import {
  Component,
  ChangeDetectionStrategy,
  ViewEncapsulation,
  Output,
  EventEmitter,
  Input,
  ChangeDetectorRef,
  ViewChild,
} from '@angular/core';
import { BaseSmartComponent } from 'app/shared/components/component.abstract';
import { LnAssessmentsDataService } from './ln-assessments-data.service';
import { ToastrService } from 'ngx-toastr';
import { HttpErrorResponse } from '@angular/common/http';
import { IdpStatusCodeEnum } from 'app/individual-development/idp.constant';
import { AssignLnAssessmentResultModel } from 'app/organisational-development/models/idp.model';
import { HttpStatusCodeEnum } from 'app-enums/http-status-code-enum';
import { Staff } from '../models/staff.model';
import { DateTimeUtil } from 'app-utilities/date-time-utils';
import { AssignLearningNeedsAnalysisForm } from './assign-lna-form';
import {
  CxSurveyjsEventModel,
  CxSurveyjsComponent,
  CxDateUtil,
  CxGlobalLoaderService,
} from '@conexus/cx-angular-common';

@Component({
  selector: 'assign-ln-assessments-dialog',
  templateUrl: './assign-ln-assessments-dialog.component.html',
  styleUrls: ['./assign-ln-assessments-dialog.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [LnAssessmentsDataService],
})
export class AssignLnAssessmentsDialogComponent extends BaseSmartComponent {
  @Input() selectedEmployees: Staff[];
  @Output() cancel: EventEmitter<undefined> = new EventEmitter<undefined>();
  @Output() done: EventEmitter<Staff[]> = new EventEmitter<Staff[]>();
  @ViewChild('assignLNASurveyJS', { static: true }) assignLNASurveyJS: CxSurveyjsComponent;

  public unassignedSelectedEmployees: Staff[] = [];
  public expiredLNAEmployees: Staff[] = [];
  public loadingUnassignedEmployees: boolean = false;
  public jsonForm: any = AssignLearningNeedsAnalysisForm;
  public data: any;
  constructor(
    changeDetectorRef: ChangeDetectorRef,
    private lnAssessmentsDataService: LnAssessmentsDataService,
    private toastrService: ToastrService,
    private cxGlobalLoaderService: CxGlobalLoaderService
  ) {
    super(changeDetectorRef);
  }

  ngOnInit() {
    this.loadingUnassignedEmployees = true;
    this.unassignedSelectedEmployees = this.filterUnassignedEmployees();
    this.expiredLNAEmployees = this.filterExpiredLNAEmployees();

    this.data = {
      unassignedLNAUsers: this.unassignedSelectedEmployees,
      unassignedLNAUsersCount: this.unassignedSelectedEmployees.length,
      expiredLNAUsers: this.expiredLNAEmployees,
    };

    this.loadingUnassignedEmployees = false;
    this.changeDetectorRef.detectChanges();
  }

  onCancel(): void {
    this.cancel.emit();
  }

  onSubmitSurvey(surveyEvent: CxSurveyjsEventModel): void {
    this.cxGlobalLoaderService.showLoader();
    surveyEvent.options.allowComplete = false;
    const dueDate = DateTimeUtil.parseSurveyDate(
      surveyEvent.survey.data.dueDate
    );

    const assignedEmployeeIdentities = this.unassignedSelectedEmployees
      .concat(this.expiredLNAEmployees)
      .map((emp) => {
        return {
          archetype: emp.identity.archetype,
          id: emp.identity.id,
          extId: emp.identity.extId,
        };
      });
    this.subscription.add(
      this.lnAssessmentsDataService
        .assignLNAssessmentsToEmployees(assignedEmployeeIdentities, dueDate)
        .subscribe(
          (assignLNAResults: AssignLnAssessmentResultModel[]) => {
            this.processAssignResult(
              this.unassignedSelectedEmployees,
              this.expiredLNAEmployees,
              assignLNAResults,
              dueDate
            );
            this.cxGlobalLoaderService.hideLoader();
          },
          (error: HttpErrorResponse) => {
            this.processAssignResult(
              this.unassignedSelectedEmployees,
              this.expiredLNAEmployees,
              error.error,
              dueDate
            );
            this.cxGlobalLoaderService.hideLoader();
          }
        )
    );
  }

  onDone(): void {
    this.assignLNASurveyJS.doComplete();
  }

  private processAssignResult(
    unassignedEmployees: Staff[],
    expiredLNAEmployees: Staff[],
    assignLNAResults: AssignLnAssessmentResultModel[],
    dueDate: Date
  ): void {
    const processingEmployees = unassignedEmployees.concat(expiredLNAEmployees);
    const successHttpStatusCodes = [
      HttpStatusCodeEnum.Status201Created,
      HttpStatusCodeEnum.Status200OK,
    ];
    const successEmployees: Staff[] = [];
    const failedEmployees: Staff[] = [];
    assignLNAResults.forEach((assignResult) => {
      const matchingProcessingEmployee = processingEmployees.find(
        (emp) => emp.identity.extId === assignResult.identity.extId
      );
      if (!matchingProcessingEmployee) {
        return;
      } // Ignore record which isn't found in the list.

      if (successHttpStatusCodes.includes(assignResult.statusCode)) {
        if (matchingProcessingEmployee) {
          matchingProcessingEmployee.assessmentInfos.LearningNeed.statusInfo =
            assignResult.assessmentStatusInfo;
          successEmployees.push(matchingProcessingEmployee);
        }
      } else {
        failedEmployees.push(matchingProcessingEmployee);
      }
    });
    this.showMessage(
      unassignedEmployees,
      expiredLNAEmployees,
      successEmployees,
      failedEmployees
    );
    this.done.emit(
      this.getAssignedSuccessfullyWithCutOffDate(successEmployees, dueDate)
    );
  }

  private showMessage(
    unassignedEmployees: Staff[],
    expiredLNAEmployees: Staff[],
    successEmployees: Staff[],
    failedEmployees: Staff[]
  ): void {
    if (successEmployees.length > 0) {
      const successAssignedEmployeeCount = this.countMatchingEmployees(
        successEmployees,
        unassignedEmployees
      );
      const successSetNewCompleteDateEmployeeCount = this.countMatchingEmployees(
        successEmployees,
        expiredLNAEmployees
      );
      if (successAssignedEmployeeCount > 0) {
        this.toastrService.success(
          `Successfully assigned to ${successAssignedEmployeeCount} employee(s).`
        );
      }
      if (successSetNewCompleteDateEmployeeCount > 0) {
        this.toastrService.success(
          `Successfully set new complete date for ${successSetNewCompleteDateEmployeeCount} employee(s).`
        );
      }
    } else if (failedEmployees.length === 0) {
      this.toastrService.warning(`No employee assigned.`);
    }
    if (failedEmployees.length > 0) {
      this.toastrService.error(
        `Assign ${failedEmployees.length} employee(s) failed.`
      );
    }
  }

  private countMatchingEmployees(
    firstEmployeeList: Staff[],
    secondEmployeeList: Staff[]
  ): number {
    if (
      !firstEmployeeList ||
      firstEmployeeList.length === 0 ||
      !secondEmployeeList ||
      secondEmployeeList.length === 0
    ) {
      return 0;
    }

    const employeeIdsOfSecondList = secondEmployeeList.map(
      (p) => p.identity.id
    );

    return firstEmployeeList.filter((emp) =>
      employeeIdsOfSecondList.includes(emp.identity.id)
    ).length;
  }

  private getAssignedSuccessfullyWithCutOffDate(
    assignedEmployees: Staff[],
    updatedDueDate: Date
  ): Staff[] {
    return assignedEmployees.map((emp) => {
      return {
        ...emp,
        assessmentInfos: {
          ...emp.assessmentInfos,
          LearningNeed: {
            ...emp.assessmentInfos.LearningNeed,
            dueDate: updatedDueDate,
          },
        },
      };
    });
  }

  private filterExpiredLNAEmployees(): Staff[] {
    const checkingDueDateOfLNAStatuses: string[] = [
      IdpStatusCodeEnum.NotStarted,
      IdpStatusCodeEnum.Started,
      IdpStatusCodeEnum.Rejected,
    ];

    return this.selectedEmployees.filter(
      (emp) =>
        checkingDueDateOfLNAStatuses.includes(
          emp.assessmentInfos.LearningNeed.statusInfo.assessmentStatusCode
        ) &&
        emp.assessmentInfos.LearningNeed.dueDate &&
        DateTimeUtil.isInThePast(emp.assessmentInfos.LearningNeed.dueDate)
    );
  }

  private filterUnassignedEmployees(): Staff[] {
    return this.selectedEmployees.filter(
      (emp) =>
        emp.assessmentInfos.LearningNeed.statusInfo.assessmentStatusCode ===
        IdpStatusCodeEnum.NotAdded
    );
  }
}
