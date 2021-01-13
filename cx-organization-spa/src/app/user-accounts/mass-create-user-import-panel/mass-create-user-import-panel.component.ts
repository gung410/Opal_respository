import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  OnInit,
  ViewChild
} from '@angular/core';
import { Router } from '@angular/router';
import {
  CxConfirmationDialogComponent,
  CxGlobalLoaderService
} from '@conexus/cx-angular-common';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'app-auth/auth.service';
import { User } from 'app-models/auth.model';
import { ReportsDataService } from 'app/reports/reports-data.service';
import { AppConstant } from 'app/shared/app.constant';
import { MassUsersCreationFileError } from 'app/shared/constants/mass-users-creation-file-error-enum';
import { ToastrService } from 'ngx-toastr';
import { UploadFileRequest } from '../dtos/upload-file-request';
// tslint:disable-next-line:max-line-length
import { InvalidUsersCreationRecordDialogComponent } from '../invalid-users-ceation-record-dialog/invalid-users-creation-record-dialog.component';
import { FileInfo, IFileInfo } from '../models/file-info.model';
import {
  InvalidMassUsersCreationDto,
  MassUsersCreationException
} from '../models/mass-users-creation-exception.model';
import { FileInfoApiService } from '../services/file-info-api.service';
import { FileInfoListService } from '../services/file-info-list.service';
import { UserAccountsDataService } from '../user-accounts-data.service';

@Component({
  selector: 'mass-create-user-import-panel',
  templateUrl: './mass-create-user-import-panel.component.html',
  styleUrls: ['./mass-create-user-import-panel.component.scss']
})
export class MassCreateUserImportPanelComponent implements OnInit {
  get isCreateButtonEnabled(): boolean {
    return this.uploadedFile && this.isUploadedFileValid;
  }
  @ViewChild('uploadFile') uploadFileElement: ElementRef;
  massUsersCreationTemplateFile: string = 'mass_users_creation_template.csv';
  uploadedFile: File;
  uploadedFileName: string;
  isUploadedFileValid: boolean = false;
  currentUser: User;
  isShownInstructionTooltip: boolean = false;

  constructor(
    private reportsDataService: ReportsDataService,
    private globalLoader: CxGlobalLoaderService,
    private ngbModal: NgbModal,
    protected authService: AuthService,
    private router: Router,
    private toastrService: ToastrService,
    private fileInfoApiService: FileInfoApiService,
    private fileInfoListService: FileInfoListService,
    private translateService: TranslateService,
    private userAccountsDataService: UserAccountsDataService,
    private changeDetectorRef: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.getCurrentUser();
  }

  downloadMassUsersCreationTemplate(): void {
    this.reportsDataService.executeDownloadFile(
      this.massUsersCreationTemplateFile,
      `assets/template-file/${this.massUsersCreationTemplateFile}`
    );
  }

  async handleMassUsersCreationFileInput(event: any): Promise<void> {
    this.globalLoader.showLoader();
    const fileList: FileList = event.target.files;
    if (fileList && fileList.length) {
      this.uploadedFile = fileList[0];

      if (!this.isCSVFile(this.uploadedFile.name)) {
        this.showInvalidFileWarning(
          new MassUsersCreationException({
            errorType: MassUsersCreationFileError.FileFormatIsNotCorrect
          })
        );
        this.resetMassUsersCreationFileInput();
      } else {
        const validationResult = await this.userAccountsDataService.validateMassUsersCreationFile(
          this.uploadedFile
        );
        if (validationResult && validationResult.exception) {
          this.resetMassUsersCreationFileInput();
          this.showInvalidFileWarning(validationResult.exception);
        } else {
          this.uploadedFileName = this.uploadedFile.name;
          this.isUploadedFileValid = true;
        }
      }
      this.changeDetectorRef.detectChanges();
    }
    this.globalLoader.hideLoader();
  }

  async onMassUsersCreateButtonClicked(): Promise<void> {
    // If no file selected, throw warning message and return immediately.
    if (!this.uploadedFile) {
      this.showInvalidFileWarning(
        new MassUsersCreationException({
          errorType: MassUsersCreationFileError.FileIsNotSelected
        })
      );

      return;
    }

    // If file template is valid, then checking for the file's data in the server and return checking result.
    this.globalLoader.showLoader();
    const massUsersCreationDataValidationResult = await this.userAccountsDataService.validateMassUsersCreation(
      this.uploadedFile
    );
    this.globalLoader.hideLoader();

    this.resetMassUsersCreationFileInput();
    this.changeDetectorRef.detectChanges();

    // process for the result in FE.
    // If the data file is valid to create users.
    if (
      massUsersCreationDataValidationResult &&
      massUsersCreationDataValidationResult.isValidToMassUserCreation
    ) {
      this.processMassUsersCreation()
        .then((fileResult) => {
          this.resetMassUsersCreationFileInput();
          this.changeDetectorRef.detectChanges();

          return;
        })
        .catch((err) => {
          this.toastrService.error(err);
        });
    }

    // This is for data file is invalid to create users.
    if (massUsersCreationDataValidationResult.exception) {
      this.showInvalidFileWarning(
        massUsersCreationDataValidationResult.exception
      );
      this.resetMassUsersCreationFileInput();

      return;
    }
    if (
      massUsersCreationDataValidationResult.invalidMassUserCreationDto &&
      massUsersCreationDataValidationResult.invalidMassUserCreationDto.length
    ) {
      this.showInvalidUsersCreationWarning(
        massUsersCreationDataValidationResult.invalidMassUserCreationDto
      );

      return;
    }
  }

  async processMassUsersCreation(): Promise<FileInfo> {
    return new Promise<FileInfo>((resolve, reject) => {
      this.userAccountsDataService
        .createMassUsers(this.uploadedFile)
        .then((massCreatedUsers) => {
          if (!massCreatedUsers) {
            reject('There is no users created');
          }
          this.showUsersCreationProcessingMessage();
          resolve();
        })
        .then(() => this.processUploadFile())
        .then((fileInfo) => {
          if (!fileInfo) {
            reject('File has not uploaded');
          }
          this.fileInfoListService.notifyNewUploadedFile(true);
          resolve(fileInfo);
        })
        .catch((error) => {
          this.toastrService.error(
            'Something went wrong in the process of upload file'
          );
        });
    });
  }

  onBackButtonClicked(): void {
    this.navigateBackToUserAccountList();
  }

  toggleInstructionTooltip(value: boolean): void {
    this.isShownInstructionTooltip = value
      ? value
      : !this.isShownInstructionTooltip;
  }

  private processUploadFile(): Promise<IFileInfo> {
    return this.fileInfoApiService.uploadFileCSV(
      new UploadFileRequest(this.uploadedFile, this.currentUser.identity.extId)
    );
  }

  private navigateBackToUserAccountList(): void {
    this.router.navigate([`/${AppConstant.siteURL.menus.userAccounts}`]);
  }

  private isCSVFile(fileName: string): boolean {
    const valToLower = fileName.toLowerCase();
    const regex = new RegExp('(.*?).(csv)$');
    const regexTest = regex.test(valToLower);
    return regexTest;
  }

  private showInvalidFileWarning(exception: MassUsersCreationException): void {
    const cxConfirmationDialogModalRef = this.ngbModal.open(
      CxConfirmationDialogComponent,
      {
        size: 'sm',
        centered: true
      }
    );

    const cxConfirmationDialogModal = cxConfirmationDialogModalRef.componentInstance as CxConfirmationDialogComponent;
    cxConfirmationDialogModal.showConfirmButton = false;
    cxConfirmationDialogModal.showCloseButton = false;
    cxConfirmationDialogModal.cancelButtonText = this.translateService.instant(
      'Common.Button.Close'
    ) as string;
    cxConfirmationDialogModal.header = this.translateService.instant(
      'MassUsersCreation.UploadedFile.Header'
    ) as string;
    let messageContent = '';
    if (exception.errorType === MassUsersCreationFileError.FileIsEmpty) {
      messageContent = this.translateService.instant(
        'MassUsersCreation.EmptyFile.Content'
      ) as string;
    } else {
      switch (exception.errorType) {
        case MassUsersCreationFileError.FileFormatIsNotCorrect:
          messageContent = this.translateService.instant(
            'MassUsersCreation.UploadedFile.Content'
          ) as string;
          break;
        case MassUsersCreationFileError.FileTemplateIsInvalid:
          messageContent = this.translateService.instant(
            'MassUsersCreation.InvalidTemplate.Content'
          ) as string;
          break;
        case MassUsersCreationFileError.FileIsNotSelected:
          messageContent = this.translateService.instant(
            'MassUsersCreation.FileIsNotSelected.Content'
          ) as string;
          break;
        case MassUsersCreationFileError.FileExceedLimitRecord: {
          messageContent = this.translateService.instant(
            'MassUsersCreation.LimitRecord.Content'
          ) as string;
          break;
        }
        case MassUsersCreationFileError.UserRowEmptyException: {
          messageContent = this.translateService.instant(
            'MassUsersCreation.UserRowEmpty.Content'
          ) as string;
          break;
        }
        default:
          messageContent = this.translateService.instant(
            'MassUsersCreation.GeneralError.Content'
          ) as string;
          break;
      }
    }
    cxConfirmationDialogModal.content = messageContent;
    cxConfirmationDialogModal.cancel.subscribe(() => {
      cxConfirmationDialogModalRef.close();
    });
  }

  private showUsersCreationProcessingMessage(): void {
    const modalRef = this.ngbModal.open(CxConfirmationDialogComponent, {
      size: 'sm',
      centered: true
    });

    const modalComponent = modalRef.componentInstance as CxConfirmationDialogComponent;
    modalComponent.showCloseButton = false;
    modalComponent.showCancelButton = false;
    modalComponent.cancelButtonText = this.translateService.instant(
      'Common.Button.Close'
    ) as string;
    modalComponent.header = this.translateService.instant(
      'MassUsersCreation.Processing.Header'
    ) as string;
    modalComponent.content = this.translateService.instant(
      'MassUsersCreation.Processing.Content'
    );
    modalComponent.confirm.subscribe(() => {
      modalRef.close();
    });
  }

  private getCurrentUser(): void {
    this.authService.userData().subscribe(async (user: User) => {
      if (user) {
        this.currentUser = user;
      }
    });
  }

  private resetMassUsersCreationFileInput(): void {
    this.isUploadedFileValid = false;
    this.uploadFileElement.nativeElement.value = null;
    this.uploadedFileName = '';
  }

  private showInvalidUsersCreationWarning(
    invalidUsersCreationResults: InvalidMassUsersCreationDto[]
  ): void {
    const modalRef = this.ngbModal.open(CxConfirmationDialogComponent, {
      size: 'sm',
      centered: true
    });

    const modalComponent = modalRef.componentInstance as CxConfirmationDialogComponent;
    modalComponent.showCloseButton = false;
    modalComponent.cancelButtonText = this.translateService.instant(
      'Common.Button.Close'
    ) as string;
    modalComponent.confirmButtonText = this.translateService.instant(
      'MassUsersCreation.Warning.ConfirmButton'
    ) as string;
    modalComponent.header = this.translateService.instant(
      'MassUsersCreation.Warning.Header'
    ) as string;
    modalComponent.content = this.translateService.instant(
      'MassUsersCreation.Warning.Content'
    ) as string;
    modalComponent.cancel.subscribe(() => {
      modalRef.close();
    });
    modalComponent.confirm.subscribe(() => {
      this.showInvalidUsersCreationRecords(invalidUsersCreationResults);
      modalRef.close();
    });
  }

  private showInvalidUsersCreationRecords(
    invalidUsersCreationResults: InvalidMassUsersCreationDto[]
  ): void {
    const modalRef = this.ngbModal.open(
      InvalidUsersCreationRecordDialogComponent,
      {
        centered: true,
        windowClass: 'modal-size-xl'
      }
    );
    const modalComponent = modalRef.componentInstance as InvalidUsersCreationRecordDialogComponent;
    modalComponent.cancelButtonText = this.translateService.instant(
      'Common.Button.Close'
    ) as string;
    modalComponent.invalidUsersCreationResults = invalidUsersCreationResults;
    modalComponent.cancel.subscribe(() => {
      modalRef.close();
    });
  }
}
