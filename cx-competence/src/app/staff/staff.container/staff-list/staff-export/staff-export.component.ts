import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  OnInit,
  Output,
  ViewEncapsulation,
} from '@angular/core';
import {
  CxSurveyjsVariable,
  CxGlobalLoaderService,
  CxSurveyjsEventModel,
} from '@conexus/cx-angular-common';
import { UserService } from 'app-services/user.service';
import { BaseSmartComponent } from 'app/shared/components/component.abstract';
import { StatusTypeEnum } from 'app/shared/constants/user-status-type.enum';
import { ToastrService } from 'ngx-toastr';

import { ExportModel, FilterParamModel } from '../models/filter-param.model';
import { ExportFields, ExportFormJSON } from './models/export-fields.model';
import { AcceptExportDto } from 'app/learning-needs-analysis/models/export-learning-needs-analysis-params';
import { ReportsDataService } from 'app-services/reports-data.services';

@Component({
  selector: 'staff-export',
  templateUrl: './staff-export.component.html',
  styleUrls: ['./staff-export.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StaffExportComponent extends BaseSmartComponent implements OnInit {
  exportFileName: string = 'staffList';
  appliedFilterData: FilterParamModel;
  exportFormJSON: any = ExportFormJSON;
  exportFields: any = ExportFields;
  selectedItems: any[];
  surveyVariables: CxSurveyjsVariable[] = [];
  @Output() completeExport: EventEmitter<any> = new EventEmitter<any>();
  @Output() cancel: EventEmitter<any> = new EventEmitter<any>();
  constructor(
    changeDetectorRef: ChangeDetectorRef,
    private userService: UserService,
    private toastrService: ToastrService,
    private cxGlobalLoaderService: CxGlobalLoaderService,
    private reportsDataService: ReportsDataService
  ) {
    super(changeDetectorRef);
  }

  ngOnInit(): void {
    if (this.selectedItems && this.selectedItems.length > 0) {
      this.surveyVariables = [
        new CxSurveyjsVariable({
          name: 'selectedUserCount',
          value: this.selectedItems.length,
        }),
      ];
    }
  }

  onExportClick(event: CxSurveyjsEventModel): void {
    event.options.allowComplete = false;
    const surveyResult = event.survey.data;
    this.cxGlobalLoaderService.showLoader();
    const exportFields = {};
    const selectedFields = this.exportFields.filter((field: any) =>
      surveyResult.fieldsExport.includes(field.value)
    );
    selectedFields.forEach((field: any) => {
      exportFields[field.value] = field.text;
    });
    if (this.selectedItems && this.selectedItems.length > 0) {
      this.exportSelectedUsers(exportFields);
    } else {
      const includedSubDepartment =
        surveyResult.includeSubDepartment &&
        surveyResult.includeSubDepartment[0];
      this.exportAll(exportFields, includedSubDepartment);
    }
  }

  onCancel(): void {
    this.cancel.emit();
  }

  private exportSelectedUsers(fieldsExport: any): void {
    const selectedUserIds = this.selectedItems.map(
      (user: any) => user.staff.identity.id
    );
    const filterWithSelectedUsers = new FilterParamModel({
      userIds: selectedUserIds,
      exportOptions: new ExportModel({
        exportFields: fieldsExport,
      }),
      pageIndex: 0,
      pageSize: 0,
      entityStatuses: [StatusTypeEnum.All.code],
    });
    this.requestExportUser(filterWithSelectedUsers);
  }

  private exportAll(fieldsExport: any, includeSubDepartment: boolean): void {
    const filterData = { ...this.appliedFilterData };
    filterData.exportOptions = new ExportModel({
      exportFields: fieldsExport,
    });
    filterData.pageIndex = 0;
    filterData.pageSize = 0;
    filterData.filterOnSubDepartment = includeSubDepartment;
    this.requestExportUser(filterData);
  }

  private requestExportUser(filterWithSelectedUsers: FilterParamModel): void {
    this.userService.exportUser(filterWithSelectedUsers).subscribe(
      (acceptExportDto: AcceptExportDto) => {
        this.downloadFile(acceptExportDto, this.exportFileName);
      },
      (error: any) => {
        this.toastrService.error(error);
      }
    );
  }

  private downloadFile(
    acceptExportDto: AcceptExportDto,
    fileName: string
  ): void {
    let interval: any;
    const checkDownloadFile = () => {
      this.reportsDataService
        .checkFileExists(acceptExportDto.filePath)
        .subscribe((fileExists) => {
          if (fileExists) {
            // Stop calling the check download file function and save the file.
            clearInterval(interval);
            this.cxGlobalLoaderService.hideLoader();
            this.completeExport.emit();
            this.reportsDataService.downloadFile(
              acceptExportDto.filePath,
              `${fileName}-${this.buildSuffixFileName()}.csv`
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

  private buildSuffixFileName(): string {
    return new Date().toISOString();
  }
}
