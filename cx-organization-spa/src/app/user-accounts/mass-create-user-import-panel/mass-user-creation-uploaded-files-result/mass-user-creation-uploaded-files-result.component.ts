import { ChangeDetectorRef, Component, Input, OnInit } from '@angular/core';
import { CxGlobalLoaderService } from '@conexus/cx-angular-common';
import { AuthService } from 'app-auth/auth.service';
import { User } from 'app-models/auth.model';
import { AppConstant } from 'app/shared/app.constant';
import { GetFileByUserIdRequest } from 'app/user-accounts/dtos/get-file-by-user-id-request';
import { PagingResponseModel } from 'app/user-accounts/models/user-management.model';
import { FileInfoListService } from 'app/user-accounts/services/file-info-list.service';
import { FileInfoListViewModel } from 'app/user-accounts/viewmodels/file-info-list.viewmodel';
import { ToastrService } from 'ngx-toastr';
import { FileTarget } from '../../models/file-info.model';

@Component({
  selector: 'mass-user-creation-uploaded-files-result',
  templateUrl: './mass-user-creation-uploaded-files-result.component.html',
  styleUrls: ['./mass-user-creation-uploaded-files-result.component.scss']
})
export class MassUserCreationUploadedFilesResultComponent implements OnInit {
  fileInfoDataPaging: PagingResponseModel<FileInfoListViewModel>;
  @Input() currentUser: User;

  private _getFilesByUserIdRequest: GetFileByUserIdRequest;
  private defaultPageSize: number = AppConstant.Item25PerPage;
  private _defaultSortField: string = 'CreatedDate';
  private _defaultSortType: string = 'Desc';

  constructor(
    private fileInfoListService: FileInfoListService,
    protected authService: AuthService,
    private cxGlobalLoaderService: CxGlobalLoaderService,
    private toastrService: ToastrService,
    protected changeDetectorRef: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.initFileInfoRequestPayload();
    this.getFilesInfo();
    this.fileInfoListService.newUploadedFile$.subscribe((isNewUploadedFile) => {
      if (isNewUploadedFile) {
        this.getFilesInfo();
      }
    });
  }

  getFilesInfo(): void {
    this.cxGlobalLoaderService.showLoader();
    const loadFileSub = this.fileInfoListService
      .getFilesInfoByUserId(this._getFilesByUserIdRequest)
      .subscribe(
        (fileInfoDataPaging) => {
          if (!fileInfoDataPaging) {
            return;
          }

          this.fileInfoDataPaging = fileInfoDataPaging;
        },
        () => {
          this.toastrService.error('Error during getting broadcast messages');
        },
        () => {
          this.changeDetectorRef.detectChanges();
          this.cxGlobalLoaderService.hideLoader();
        }
      );

    // this.subscription.add(loadFileSub);
  }

  private initFileInfoRequestPayload(): void {
    this._getFilesByUserIdRequest = new GetFileByUserIdRequest({
      pageIndex: 1,
      pageSize: this.defaultPageSize,
      orderBy: this._defaultSortField,
      orderType: this._defaultSortType,
      userGuid: this.currentUser.identity.extId,
      fileTarget: FileTarget.MassUserCreation
    });
  }
}
