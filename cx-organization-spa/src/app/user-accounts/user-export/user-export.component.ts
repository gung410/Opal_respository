import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  OnInit,
  Output,
  ViewEncapsulation
} from '@angular/core';
import {
  CxGlobalLoaderService,
  CxSurveyjsEventModel,
  CxSurveyjsVariable
} from '@conexus/cx-angular-common';
import { ToastrAdapterService } from 'app-services/toastr-adapter.service';
import { BaseSmartComponent } from 'app/shared/components/component.abstract';
import { StatusTypeEnum } from 'app/shared/constants/user-status-type.enum';

import { ReportsDataService } from 'app/reports/reports-data.service';
import { InstructionReporting } from '../models/reporting-by-systemrole.model';
import { UserManagementQueryModel } from '../models/user-management.model';
import { UserAccountsDataService } from '../user-accounts-data.service';
import { ExportFields, ExportFormJSON } from './models/export-fields.model';

@Component({
  selector: 'user-export',
  templateUrl: './user-export.component.html',
  styleUrls: ['./user-export.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserExportComponent extends BaseSmartComponent implements OnInit {
  appliedFilterData: UserManagementQueryModel;
  exportFormJSON: any = ExportFormJSON;
  selectedItems: any[];
  surveyVariables: CxSurveyjsVariable[] = [];
  @Output() completeExport: EventEmitter<any> = new EventEmitter<any>();
  @Output() cancel: EventEmitter<any> = new EventEmitter<any>();
  constructor(
    changeDetectorRef: ChangeDetectorRef,
    private userAccountsDataService: UserAccountsDataService,
    private cxGlobalLoaderService: CxGlobalLoaderService,
    private toastr: ToastrAdapterService,
    private reportsDataService: ReportsDataService
  ) {
    super(changeDetectorRef);
  }

  ngOnInit(): void {
    if (this.selectedItems && this.selectedItems.length > 0) {
      this.surveyVariables = [
        new CxSurveyjsVariable({
          name: 'selectedUserCount',
          value: this.selectedItems.length
        })
      ];
    }
  }

  onExportClick(event: CxSurveyjsEventModel): void {
    event.options.allowComplete = false;
    if (this.selectedItems && this.selectedItems.length) {
      this.exportSelectedUsers(event.survey.data);
    } else {
      this.exportAll(event.survey.data);
    }
  }

  onCancel(): void {
    this.cancel.emit();
  }

  private exportSelectedUsers(surveyResult: any): void {
    const selectedUserIds = this.selectedItems.map(
      (user: any) => user.identity.id
    );
    const filterWithSelectedUsers = new UserManagementQueryModel({
      userIds: selectedUserIds,
      exportOption: {
        exportFields: this.convertToExportFields(surveyResult.fieldsExport)
      },
      pageIndex: 0,
      pageSize: 0,
      userEntityStatuses: [StatusTypeEnum.All.code]
    });
    this.requestExportUser(filterWithSelectedUsers);
  }

  private exportAll(surveyResult: any): void {
    const filterData = { ...this.appliedFilterData };
    filterData.exportOption = {
      exportFields: this.convertToExportFields(surveyResult.fieldsExport)
    };
    filterData.pageIndex = 0;
    filterData.pageSize = 0;
    filterData.filterOnSubDepartment = !!surveyResult.includeSubDepartment;
    this.requestExportUser(filterData);
  }

  private convertToExportFields(exportFields: string[]): any {
    const exportFieldsObject = {};

    exportFields.forEach((exportField: string) => {
      const displayName = ExportFields.find(
        (item) => item.value === exportField
      ).text;
      exportFieldsObject[exportField] = displayName;
    });

    return exportFieldsObject;
  }

  private requestExportUser(
    filterWithSelectedUsers: UserManagementQueryModel
  ): void {
    this.cxGlobalLoaderService.showLoader();
    this.userAccountsDataService
      .exportAsyncAccounts(filterWithSelectedUsers)
      .subscribe(
        (instructionReporting: InstructionReporting) => {
          this.downloadFile(instructionReporting, `user-accounts`);
        },
        () => {
          this.toastr.error(`An error occurred when exporting user accounts.`);
          this.cxGlobalLoaderService.hideLoader();
        }
      );
  }

  private downloadFile(
    instructionReporting: InstructionReporting,
    fileName: string
  ): void {
    let interval: any;
    const checkDownloadFile = () => {
      this.reportsDataService
        .checkFileExists(instructionReporting.filePath)
        .subscribe(
          (fileExists) => {
            if (fileExists) {
              // Stop calling the check download file function and save the file.
              clearInterval(interval);
              this.cxGlobalLoaderService.hideLoader();
              this.completeExport.emit();
              this.reportsDataService.downloadFile(
                instructionReporting.filePath,
                `${fileName}-${this.buildSuffixFileName()}.csv`
              );
            }
          },
          () => {
            this.cxGlobalLoaderService.hideLoader();
          }
        );
    };
    /**
     * Set interval time to call check download file after sometime.
     */
    const intervalInMilliseconds = 10000;
    interval = setInterval(checkDownloadFile, intervalInMilliseconds);
  }

  private buildSuffixFileName(): string {
    return new Date().toISOString();
  }
}
