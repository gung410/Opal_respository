import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  OnInit,
  Output,
} from '@angular/core';
import {
  CxGlobalLoaderService,
  CxInformationDialogService,
  CxSurveyjsEventModel,
  CxSurveyjsVariable,
} from '@conexus/cx-angular-common';
import { IdpService } from 'app-services/idp.service';
import { ReportsDataService } from 'app-services/reports-data.services';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import {
  AcceptExportDto,
  ExportLearningNeedsAnalysisParams,
} from 'app/learning-needs-analysis/models/export-learning-needs-analysis-params';
import { BaseSmartComponent } from 'app/shared/components/component.abstract';
import { FilterParamModel } from 'app/staff/staff.container/staff-list/models/filter-param.model';
import { ToastrService } from 'ngx-toastr';
import { exportLearningNeedsAnalysisFormJSON } from './export-learning-needs-analysis-form';

@Component({
  selector: 'export-learning-needs-analysis-dialog',
  templateUrl: './export-learning-needs-analysis-dialog.component.html',
  styleUrls: ['./export-learning-needs-analysis-dialog.component.scss'],
})
export class ExportLearningNeedsAnalysisDialogComponent
  extends BaseSmartComponent
  implements OnInit {
  appliedEmployeeFilter: FilterParamModel;
  surveyFormJSON: any = exportLearningNeedsAnalysisFormJSON;
  surveyVariables: CxSurveyjsVariable[] = [];
  totalUsers: number;
  @Output() completeExport: EventEmitter<any> = new EventEmitter<any>();
  @Output() cancel: EventEmitter<void> = new EventEmitter<void>();

  private isLongList: boolean;
  constructor(
    changeDetectorRef: ChangeDetectorRef,
    private idpService: IdpService,
    private toastrService: ToastrService,
    private cxGlobalLoaderService: CxGlobalLoaderService,
    private informationDialogService: CxInformationDialogService,
    private translateAdapterService: TranslateAdapterService,
    private reportsDataService: ReportsDataService
  ) {
    super(changeDetectorRef);
  }

  ngOnInit(): void {
    const longList = 2000;
    this.isLongList = this.totalUsers > longList;
    this.surveyVariables.push(
      new CxSurveyjsVariable({
        name: 'isLongList',
        value: this.isLongList,
      })
    );
  }

  onConfirmClick(event: CxSurveyjsEventModel): void {
    event.options.allowComplete = false;
    this.cxGlobalLoaderService.showLoader();
    const submittingData = event.survey.data;

    const exportLearningNeedsAnalysisParams = new ExportLearningNeedsAnalysisParams(
      {
        employeeFilter: this.appliedEmployeeFilter,
        exportOptions: {
          careerAspirationEvaluation: true,
          competencyEvaluation: true,
          learningAreaEvaluation: true,
        },
      }
    );

    if (!this.isLongList && submittingData.receiveMethod === 'yes') {
      this.idpService
        .exportLearningNeedsAnalysisAsync(exportLearningNeedsAnalysisParams)
        .subscribe(
          (acceptExportDto: AcceptExportDto) => {
            // TODO: Remove console.log after trouble shooting.
            console.log(
              `${new Date().toISOString()} Successfully requested ExportLearningNeedsAnalysis`
            );
            this.downloadFile(acceptExportDto, `LearningNeedsAnalysis`);
          },
          (_) => {
            this.toastrService.error(`An error occurred generating data.`);
            this.cxGlobalLoaderService.hideLoader();
            this.cancel.emit();
          }
        );
    } else {
      exportLearningNeedsAnalysisParams.sendEmail = true;
      this.idpService
        .exportLearningNeedsAnalysisAsync(exportLearningNeedsAnalysisParams)
        .subscribe(
          (_) => {
            this.informationDialogService.success({
              message: this.translateAdapterService.getValueImmediately(
                'Export_Learning_Needs_Analysis.Dialog.EmailSentMessage'
              ),
            });
            this.cxGlobalLoaderService.hideLoader();
            this.completeExport.emit(submittingData);
          },
          (_) => {
            this.toastrService.error(`An error occurred when sending email`);
            this.cxGlobalLoaderService.hideLoader();
            this.cancel.emit();
          }
        );
    }
  }

  onCancel(): void {
    this.cancel.emit();
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
              `${fileName}-${this.buildSuffixFileName()}.zip`
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
