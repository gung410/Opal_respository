import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { MatRadioChange } from '@angular/material/radio';
import {
  CxConfirmationDialogComponent,
  CxGlobalLoaderService,
} from '@conexus/cx-angular-common';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';
import { MassNominationErrorTypeEnum } from 'app-enums/mass-nomination-enum';
import { MassAssignPDOpportunityPayload } from 'app-models/mpj/assign-pdo.model';
import { PickApprovingOfficerTarget } from 'app-models/mpj/pdo-action-item.model';
import {
  MassNominationExceptionDto,
  MassNominationResultDto,
} from 'app-models/pdcatalog/pdcatalog.dto';
import { AssignPDOService } from 'app-services/idp/assign-pdo/assign-pdo.service';
import { ResultHelper } from 'app-services/idp/result-helpers';
import { ReportsDataService } from 'app-services/reports-data.services';
import { IdpDto } from 'app/organisational-development/models/idp.model';
import {
  CxSelectConfigModel,
  CxSelectItemModel,
} from 'app/shared/components/cx-select/cx-select.model';
import { Staff } from 'app/staff/staff.container/staff-list/models/staff.model';
import { Observable } from 'rxjs';
import { InvalidNominationRecordDialogComponent } from '../../invalid-nomination-record-dialog/invalid-nomination-record-dialog.component';
import { AssignModeEnum } from '../../planned-pdo-detail.model';
import { MassNominationResultListComponent } from '../mass-nomination-list/mass-nomination-result-list.component';

@Component({
  selector: 'mass-nomination-panel',
  templateUrl: './mass-nomination-panel.component.html',
  styleUrls: ['./mass-nomination-panel.component.scss'],
})
export class MassNominationPanelComponent implements OnInit {
  @Input() klpDto: IdpDto = undefined;
  @Input() departmentId: number = 0;
  @Input() isAdHoc: boolean = false;
  @Output() onCompleteToNominate: EventEmitter<void> = new EventEmitter();
  @ViewChild('uploadFile') uploadFileElement: ElementRef;
  @ViewChild('massNominationResult')
  massNominationResultListElement: MassNominationResultListComponent;

  //select admin
  approvingAdminObs: (
    searchKey?: string
  ) => Observable<CxSelectItemModel<Staff>[]>;
  adminSelectorConfig: CxSelectConfigModel;
  selectedAdminSelectItem: CxSelectItemModel<Staff>;
  approvingOfficerMode: PickApprovingOfficerTarget =
    PickApprovingOfficerTarget.Admin;

  //mass nomination file
  uploadFile: File;
  uploadFileName: string;
  massNominationPayLoad: MassAssignPDOpportunityPayload;
  massNominationTemplateFileName: string = 'mass_nomination_template.csv';

  //loading
  loading: boolean = true;
  isValidFile: boolean = false;

  private klpExtId: string;

  constructor(
    private assignPDOService: AssignPDOService,
    private translateService: TranslateService,
    private changeDetectorRef: ChangeDetectorRef,
    private ngbModal: NgbModal,
    private globalLoader: CxGlobalLoaderService,
    private reportsDataService: ReportsDataService
  ) {}
  ngOnInit(): void {}

  ngAfterViewInit(): void {
    this.initData();
  }

  get canNominate() {
    return (
      this.uploadFile &&
      this.isValidFile &&
      ((this.selectedAdminSelectItem &&
        this.approvingOfficerMode === PickApprovingOfficerTarget.Admin) ||
        this.approvingOfficerMode === PickApprovingOfficerTarget.CAO)
    );
  }

  async handleFileInput(event: any) {
    this.globalLoader.showLoader();
    const fileList: FileList = event.target.files;
    if (fileList.length > 0) {
      this.uploadFile = fileList[0];
      if (!this.checkExtension(this.uploadFile.name)) {
        this.showInvalidFileWarning(
          new MassNominationExceptionDto({
            errorType: MassNominationErrorTypeEnum.FileFormatIsNotCorrect,
          })
        );
        this.resetFileInput();
      } else {
        const validationResult = await this.assignPDOService.validateMassNominationFile(
          this.uploadFile
        );
        if (validationResult.exception) {
          this.resetFileInput();
          this.showInvalidFileWarning(validationResult.exception);
        } else {
          this.uploadFileName = this.uploadFile.name;
          this.isValidFile = true;
        }
      }
      this.changeDetectorRef.detectChanges();
    }
    this.globalLoader.hideLoader();
  }

  downloadFile(): void {
    this.reportsDataService.executeDownloadFile(
      this.massNominationTemplateFileName,
      `assets/template-file/${this.massNominationTemplateFileName}`
    );
  }

  async onClickedNominate(): Promise<void> {
    if (!this.uploadFile) {
      this.showInvalidFileWarning(
        new MassNominationExceptionDto({
          errorType: MassNominationErrorTypeEnum.FileIsNotSelected,
        })
      );
    } else {
      this.globalLoader.showLoader();
      this.buildMassAssignPDOPayload();
      const massNominationValidationResult = await this.assignPDOService.validateMassNomination(
        this.massNominationPayLoad
      );
      this.globalLoader.hideLoader();
      this.clearNominateFormData();
      if (!massNominationValidationResult.isValidToMassNominate) {
        if (massNominationValidationResult.exception) {
          this.showInvalidFileWarning(massNominationValidationResult.exception);
        } else if (
          massNominationValidationResult.invalidNominations.length > 0
        ) {
          this.showNominationWarning(
            massNominationValidationResult.invalidNominations
          );
        }
      } else {
        this.showAsyncNominationProccessingMessage();
        await this.assignPDOService.massNominate(
          this.massNominationPayLoad,
          this.isAdHoc ? AssignModeEnum.AdhocNominate : AssignModeEnum.Nominate
        );

        this.onCompleteToNominate.emit();
        this.changeDetectorRef.detectChanges();
      }
      this.resetFileInput();
    }
  }

  buildMassAssignPDOPayload(): void {
    const approvingOfficerExtId = this.isApprovingAdminMode
      ? this.getUserExtId(this.selectedAdminSelectItem)
      : undefined;
    const isRouteForIndividualAOForGroup = this.isApprovingOfficerMode;
    this.massNominationPayLoad = new MassAssignPDOpportunityPayload({
      file: this.uploadFile,
      keyLearningProgramExtId: this.klpExtId,
      departmentId: this.departmentId,
      proceedAssign: true,
      nominationApprovingOfficerExtId: approvingOfficerExtId,
      isRouteIndividualLearnerAOForApproval: isRouteForIndividualAOForGroup,
      isAdhoc: this.isAdHoc,
    });
  }

  getUserExtId(userSelectItem: CxSelectItemModel<Staff>): string {
    if (!userSelectItem || !userSelectItem.dataObject) {
      return;
    }

    return userSelectItem.dataObject.identity.extId;
  }

  onChangeApprovingOfficerMode(change: MatRadioChange): void {
    if (!change) {
      return;
    }
    this.approvingOfficerMode = change.value;
    this.selectedAdminSelectItem = undefined;
  }

  get isApprovingOfficerMode(): boolean {
    return this.approvingOfficerMode === PickApprovingOfficerTarget.CAO;
  }

  get isApprovingAdminMode(): boolean {
    return this.approvingOfficerMode === PickApprovingOfficerTarget.Admin;
  }

  private async initData(): Promise<void> {
    if (!this.processAndValidateData()) {
      console.error('Invalid input data for this component');
      this.globalLoader.hideLoader();

      return;
    }
    this.loading = true;
    this.globalLoader.showLoader();
    this.adminSelectorConfig = new CxSelectConfigModel({
      placeholder: this.translateService.instant(
        'Odp.LearningPlan.PlannedPDODetail.AdminSelectPlaceholder'
      ),
      searchable: false,
      multiple: false,
      hideSelected: false,
      clearable: true,
    });
    this.approvingAdminObs = this.getApprovingAdminObs;
    this.loading = false;
    this.changeDetectorRef.detectChanges();
    this.globalLoader.hideLoader();
  }

  private processAndValidateData(): boolean {
    if (!this.isAdHoc) this.klpExtId = ResultHelper.getResultExtId(this.klpDto);
    if (!this.departmentId)
      this.departmentId = ResultHelper.getObjectiveId(this.klpDto);

    return (!!this.klpExtId || this.isAdHoc) && !!this.departmentId;
  }

  private clearNominateFormData(): void {
    this.selectedAdminSelectItem = null;
    this.resetFileInput();
    this.changeDetectorRef.detectChanges();
  }

  private getApprovingAdminObs = (
    searchKey: string
  ): Observable<CxSelectItemModel<Staff>[]> => {
    return this.assignPDOService.getAdminObs(searchKey, this.departmentId);
  };

  private showAsyncNominationProccessingMessage(): void {
    const modalRef = this.ngbModal.open(CxConfirmationDialogComponent, {
      size: 'sm',
      centered: true,
    });

    const modalComponent = modalRef.componentInstance as CxConfirmationDialogComponent;
    modalComponent.showCloseButton = false;
    modalComponent.showCancelButton = false;
    modalComponent.cancelButtonText = this.translateService.instant(
      'Common.Action.Close'
    ) as string;
    modalComponent.header = this.translateService.instant(
      'MassNomination.AsyncProcessing.Header'
    ) as string;
    modalComponent.content = this.translateService.instant(
      'MassNomination.AsyncProcessing.Content'
    );
    modalComponent.confirm.subscribe(() => {
      modalRef.close();
    });
  }

  private showNominationWarning(
    invalidNominatingResults: MassNominationResultDto[]
  ): void {
    const modalRef = this.ngbModal.open(CxConfirmationDialogComponent, {
      size: 'sm',
      centered: true,
    });

    const modalComponent = modalRef.componentInstance as CxConfirmationDialogComponent;
    modalComponent.showCloseButton = false;
    modalComponent.cancelButtonText = this.translateService.instant(
      'Common.Action.Close'
    ) as string;
    modalComponent.confirmButtonText = this.translateService.instant(
      'MassNomination.Warning.ConfirmButton'
    );
    modalComponent.header = this.translateService.instant(
      'MassNomination.Warning.Header'
    ) as string;
    modalComponent.content = this.translateService.instant(
      'MassNomination.Warning.Content'
    );
    modalComponent.cancel.subscribe(() => {
      modalRef.close();
    });
    modalComponent.confirm.subscribe(() => {
      this.showInvalidNominationRecords(invalidNominatingResults);
      modalRef.close();
    });
  }

  private showInvalidFileWarning(exception: MassNominationExceptionDto): void {
    const modalRef = this.ngbModal.open(CxConfirmationDialogComponent, {
      size: 'sm',
      centered: true,
    });

    const modalComponent = modalRef.componentInstance as CxConfirmationDialogComponent;
    modalComponent.showConfirmButton = false;
    modalComponent.showCloseButton = false;
    modalComponent.cancelButtonText = this.translateService.instant(
      'Common.Action.Close'
    ) as string;
    modalComponent.header = this.translateService.instant(
      'MassNomination.UploadFile.Header'
    ) as string;
    let messageContent = '';
    if (exception.errorType === MassNominationErrorTypeEnum.FileIsEmpty) {
      messageContent = this.translateService.instant(
        'MassNomination.EmptyFile.Content'
      );
    } else if (
      exception.errorType ===
      MassNominationErrorTypeEnum.FileContainsEmptyRecord
    ) {
      messageContent = this.translateService.instant(
        'MassNomination.FileContainsEmptyRecord.Content'
      );
    } else if (
      exception.errorType === MassNominationErrorTypeEnum.FileFormatIsNotCorrect
    ) {
      messageContent = this.translateService.instant(
        'MassNomination.UploadFile.Content'
      );
    } else if (
      exception.errorType === MassNominationErrorTypeEnum.FileTemplateIsInvalid
    ) {
      messageContent = this.translateService.instant(
        'MassNomination.InvalidTemplate.Content'
      );
    } else if (
      exception.errorType === MassNominationErrorTypeEnum.FileIsNotSelected
    ) {
      messageContent = this.translateService.instant(
        'MassNomination.FileIsNotSelected.Content'
      );
    } else if (
      exception.errorType === MassNominationErrorTypeEnum.FileExceedLimitRecord
    ) {
      messageContent = this.translateService.instant(
        'MassNomination.LimitRecord.Content'
      ) as string;
    } else {
      messageContent = this.translateService.instant(
        'MassNomination.GeneralError.Content'
      );
    }
    modalComponent.content = messageContent;

    modalComponent.cancel.subscribe(() => {
      modalRef.close();
    });
  }

  private showInvalidNominationRecords(
    invalidNominatingResults: MassNominationResultDto[]
  ): void {
    const modalRef = this.ngbModal.open(
      InvalidNominationRecordDialogComponent,
      {
        centered: true,
        windowClass: 'modal-size-xl',
      }
    );
    const modalComponent = modalRef.componentInstance as InvalidNominationRecordDialogComponent;
    modalComponent.cancelButtonText = this.translateService.instant(
      'Common.Action.Close'
    ) as string;
    modalComponent.invalidNominatingResults = invalidNominatingResults;
    modalComponent.cancel.subscribe(() => {
      modalRef.close();
    });
  }

  private checkExtension(fileName: string): boolean {
    let valToLower = fileName.toLowerCase();
    let regex = new RegExp('(.*?).(csv)$');
    let regexTest = regex.test(valToLower);
    return regexTest;
  }

  private resetFileInput(): void {
    this.uploadFileElement.nativeElement.value = null;
    this.isValidFile = false;
    this.uploadFileName = '';
  }
}
