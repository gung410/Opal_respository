import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  HostListener,
  OnInit
} from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import {
  CxGlobalLoaderService,
  CxInformationDialogService,
  CxSurveyjsEventModel,
  CxSurveyjsVariable
} from '@conexus/cx-angular-common';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from 'app-auth/auth.service';
import { User } from 'app-models/auth.model';
import { CxSurveyjsExtendedService } from 'app-services/cx-surveyjs-extended.service';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { DateTimeUtil } from 'app-utilities/date-time-utils';
import { AppConstant } from 'app/shared/app.constant';
import { BaseScreenComponent } from 'app/shared/components/component.abstract';
import { FileTypeExtension } from 'app/shared/constants/file-type.enum';
import {
  CurrentFormatFileName,
  ReportTypeName
} from 'app/shared/constants/report-constant';
import { InstructionReporting } from 'app/user-accounts/models/reporting-by-systemrole.model';
import { ToastrService } from 'ngx-toastr';
import { ExportConfirmationComponent } from './export-confirmation/export-confirmation.component';
import { DateRange } from './models/date-range';
import { ExportReportInfo } from './models/export-report-info.model';
import { ExportSystemUsageParameters } from './models/export-system-usage-parameters';
import { ExportType } from './models/export-type';
import { ReportType, reportTypeUsingIframe } from './models/report-type';
import {
  AccountStatisticsFields,
  AuditReportFields,
  LoginStatisticsFields,
  OnBoardingStatisticsFields,
  PrivilegedAccountFields,
  ReportFormJSON
} from './report-form';
import { ReportHelpers } from './report-helper';
import { reportUrls } from './report-url';
import { ReportsDataService } from './reports-data.service';

@Component({
  selector: 'reports',
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.scss']
})
export class ReportsComponent extends BaseScreenComponent implements OnInit {
  reportFormJSON: any = ReportFormJSON;
  surveyVariables: CxSurveyjsVariable[] = [];
  hideExportButton: boolean = true;
  constructor(
    protected changeDetectorRef: ChangeDetectorRef,
    protected authService: AuthService,
    private cxGlobalLoaderService: CxGlobalLoaderService,
    private reportsDataService: ReportsDataService,
    private ngbModal: NgbModal,
    private informationDialogService: CxInformationDialogService,
    private translateAdapterService: TranslateAdapterService,
    private activatedRoute: ActivatedRoute,
    private cxSurveyjsExtendedService: CxSurveyjsExtendedService,
    private toastrService: ToastrService,
    private elementRef: ElementRef
  ) {
    super(changeDetectorRef, authService);
  }

  async ngOnInit(): Promise<void> {
    this.authService.userData().subscribe(async (user: User) => {
      this.currentUser = user;
      this.cxSurveyjsExtendedService.setCurrentUserVariables(user);
    });

    this.activatedRoute.queryParamMap.subscribe((queryParams: ParamMap) => {
      const filePath = queryParams.get('filepath');

      if (filePath) {
        this.reportsDataService
          .checkFileExists(filePath)
          .subscribe((fileExists) => {
            if (fileExists) {
              const fileName = this.getFileTypeName(filePath);
              const fileNameWithSuffix = this.buildSuffixFileName(fileName);
              this.reportsDataService.downloadFile(
                filePath,
                fileNameWithSuffix
              );
            } else {
              console.error(`Not found file path '${filePath}'`);
              this.toastrService.error(
                this.translateAdapterService.getValueImmediately(
                  'RequestErrorMessage.FileNotFound'
                )
              );
            }
          });
      }
    });
  }

  onExportClick(event: CxSurveyjsEventModel): void {
    event.options.allowComplete = false;
    const submittingData = event.survey.data;
    const dateRange = ReportHelpers.buildStartTimeAndEndTime(submittingData);
    const reportType = submittingData.reportType as ReportType;

    const modalRef = this.ngbModal.open(ExportConfirmationComponent, {
      size: 'lg',
      backdrop: 'static'
    });
    const modalRefComponent = modalRef.componentInstance as ExportConfirmationComponent;
    modalRefComponent.isLongProcess = ReportHelpers.isLongPeriod(
      reportType,
      dateRange
    );
    this.subscription.add(
      modalRefComponent.complete.subscribe((receiveViaEmail: boolean) => {
        modalRef.close();
        this.cxGlobalLoaderService.showLoader();
        switch (reportType) {
          case ReportType.UserAccountDetails:
            this.exportUserAccountDetailsReport(
              submittingData,
              dateRange,
              receiveViaEmail
            );
            break;
          case ReportType.AccountStatistics:
            this.exportAccountStatisticsReport(
              submittingData,
              dateRange,
              receiveViaEmail
            );
            break;
          case ReportType.PrivilegedAccounts:
            this.exportPrivilegedAccountsReport(
              submittingData,
              dateRange,
              receiveViaEmail
            );
            break;
          default:
            break;
        }
      })
    );
    this.subscription.add(
      modalRefComponent.cancel.subscribe(() => {
        modalRef.close();
      })
    );
  }

  onChange(surveyEvent: CxSurveyjsEventModel): void {
    const surveyOptions = surveyEvent.options;
    const question = surveyOptions.question;
    if (question.name === 'reportType') {
      this.hideExportButton =
        !question.value || reportTypeUsingIframe.includes(question.value);
      if (reportTypeUsingIframe.includes(question.value)) {
        this.surveyVariables = [
          new CxSurveyjsVariable({
            name: 'iframeUrl',
            value: `${reportUrls[question.value]}`
          })
        ];
        this.cxGlobalLoaderService.showLoader();
      } else {
        this.surveyVariables = [];
      }
    }
  }

  @HostListener('window:message', ['$event'])
  /**
   * Hide loader and expand the height of the iframe.
   * NOTE: This function is not only fired from the report iframe.
   *  The silent request is also triggered this function so we need to be carefully when handling this function.
   */
  onMessage(event: MessageEvent): void {
    const data = event.data.params;

    // This is to ensure that the event is fired from the report iframe.
    if (
      !data ||
      !data.height ||
      this.elementRef.nativeElement.querySelector('#embededReport') === null
    ) {
      return;
    }

    const extraHeight = 20;
    this.elementRef.nativeElement.querySelector('#embededReport').height =
      data.height + extraHeight;
    this.cxGlobalLoaderService.hideLoader();
  }

  private exportPrivilegedAccountsReport(
    submittingData: any,
    dateRange: DateRange,
    receiveViaEmail: boolean
  ): void {
    const exportFields = this.buildExportFields(PrivilegedAccountFields);

    const exportPrivilegedAccountParameters: ExportReportInfo = {
      exportOption: {
        exportFields,
        exportType: ExportType.Excel,
        exportTitle: 'Privileged Accounts',
        timeZoneOffset: DateTimeUtil.getLocalTimezone(),
        infoRecords: ReportHelpers.buildStandardReportInfo(
          this.currentUser,
          dateRange
        )
      },
      sendEmail: receiveViaEmail,
      parentDepartmentIds: [AppConstant.topDepartmentId],
      filterOnSubDepartment: true,
      userCreatedAfter: dateRange.startTime,
      userCreatedBefore: dateRange.endTime,
      pageSize: 0
    };

    this.reportsDataService
      .exportPrivilegedAccounts(exportPrivilegedAccountParameters)
      .subscribe((instructionReporting) => {
        if (receiveViaEmail) {
          this.cxGlobalLoaderService.hideLoader();
          this.informationDialogService.success({
            message: this.translateAdapterService.getValueImmediately(
              'Common.Messages.ReportPreparingSendViaEmail'
            )
          });
        } else {
          this.downloadFile(
            instructionReporting,
            ReportTypeName.PrivilegedAccounts,
            submittingData
          );
        }
      }, this.handleError);
  }

  private exportUserAccountDetailsReport(
    submittingData: any,
    dateRange: DateRange,
    receiveViaEmail: boolean
  ): void {
    const exportFields = this.buildExportFields(AuditReportFields);

    const exportReportInfo: ExportReportInfo = {
      exportOption: {
        exportFields,
        exportType: ExportType.Excel,
        exportTitle: 'User Accounts Details',
        timeZoneOffset: DateTimeUtil.getLocalTimezone(),
        infoRecords: ReportHelpers.buildStandardReportInfo(
          this.currentUser,
          dateRange
        ),
        showRowNumber: true,
        rowNumberColumnCaption: 'S/N'
      },
      sendEmail: receiveViaEmail,
      separatedByAccountType: true,
      filterOnSubDepartment: true,
      // TODO: For now, it retrieves all users in the system since the report is only using by SA and UAA.
      parentDepartmentIds: [AppConstant.topDepartmentId],
      userCreatedAfter: dateRange.startTime,
      userCreatedBefore: dateRange.endTime,
      pageSize: 0
    };

    this.reportsDataService
      .exportUserAccountDetails(exportReportInfo)
      .subscribe((instructionReporting) => {
        if (receiveViaEmail) {
          this.cxGlobalLoaderService.hideLoader();
          this.informationDialogService.success({
            message: this.translateAdapterService.getValueImmediately(
              'Common.Messages.ReportPreparingSendViaEmail'
            )
          });
        } else {
          this.downloadFile(
            instructionReporting,
            ReportTypeName.UserAccountDetails,
            submittingData
          );
        }
      }, this.handleError);
  }

  private exportAccountStatisticsReport(
    submittingData: any,
    dateRange: DateRange,
    receiveViaEmail: boolean
  ): void {
    const verticalExportFields = this.buildVerticalExportFields();

    const exportSystemUsageParameters: ExportSystemUsageParameters = {
      exportOption: {
        exportFields: {
          ExternalMastered: 'MOE-HRMS User',
          NonExternalMastered: 'External User',
          All: 'Total'
        },
        verticalExportFields,
        exportType: ExportType.Excel,
        exportTitle: 'Account Statistics',
        timeZoneOffset: DateTimeUtil.getLocalTimezone(),
        infoRecords: ReportHelpers.buildStandardReportInfo(
          this.currentUser,
          dateRange
        )
      },
      sendEmail: receiveViaEmail,
      fromDate: dateRange.startTime,
      toDate: dateRange.endTime
    };

    this.reportsDataService
      .exportAccountStatistics(exportSystemUsageParameters)
      .subscribe((instructionReporting) => {
        if (receiveViaEmail) {
          this.cxGlobalLoaderService.hideLoader();
          this.informationDialogService.success({
            message: this.translateAdapterService.getValueImmediately(
              'Common.Messages.ReportPreparingSendViaEmail'
            )
          });
        } else {
          this.downloadFile(
            instructionReporting,
            ReportTypeName.AccountStatistics,
            submittingData
          );
        }
      }, this.handleError);
  }

  private buildVerticalExportFields(): any {
    const verticalExportFields = Object.create({});
    verticalExportFields.accountStatistics = {
      caption: 'Account Statistics',
      isGroupField: true
    };
    Object.assign(
      verticalExportFields,
      this.buildExportFields(AccountStatisticsFields)
    );
    verticalExportFields.loginStatistics = {
      caption: 'Login Statistics',
      isGroupField: true
    };
    Object.assign(
      verticalExportFields,
      this.buildExportFields(LoginStatisticsFields)
    );
    verticalExportFields.onBoardingStatistics = {
      caption: 'Onboarding Statistics',
      isGroupField: true
    };
    Object.assign(
      verticalExportFields,
      this.buildExportFields(OnBoardingStatisticsFields)
    );

    return verticalExportFields;
  }

  private buildExportFields(availableFields: any[]): any {
    const exportFields = Object.create({});
    availableFields.forEach((availableField) => {
      exportFields[availableField.value] = availableField.text;
    });

    return exportFields;
  }

  private handleError = (error: any) => {
    this.cxGlobalLoaderService.hideLoader();
    console.error('Error occurred', error);
  };

  private downloadFile(
    instructionReporting: InstructionReporting,
    fileName: string,
    submittingData: any
  ): void {
    let interval: any;
    const checkDownloadFile = () => {
      this.reportsDataService
        .checkFileExists(instructionReporting.filePath)
        .subscribe((fileExists) => {
          if (fileExists) {
            // Stop calling the check download file function and save the file.
            clearInterval(interval);
            this.cxGlobalLoaderService.hideLoader();
            this.reportsDataService.downloadFile(
              instructionReporting.filePath,
              this.buildSuffixFileName(fileName)
            );
          }
        });
    };
    /**
     * Set interval time to call check download file after sometime.
     */
    const intervalInMilliseconds = 10000;
    interval = setInterval(checkDownloadFile, intervalInMilliseconds);
  }

  private buildSuffixFileName(fileName: string): string {
    return `${fileName}-${new Date().toISOString()}.${FileTypeExtension.EXCEL}`;
  }

  private getFileTypeName(filePath: string): string {
    const decodedFilePathUrl = decodeURI(filePath);
    const oldFileName = decodedFilePathUrl.split('/').slice(-1)[0];
    const expectedFileName = oldFileName.substring(
      0,
      oldFileName.length - CurrentFormatFileName.Format.length
    );

    switch (expectedFileName) {
      case `${ReportTypeName.UserStatistics}-`:
        return ReportTypeName.AccountStatistics;
      case `${ReportTypeName.PrivilegedUserAccount}-`:
        return ReportTypeName.PrivilegedAccounts;
      default:
        return expectedFileName;
    }
  }
}
