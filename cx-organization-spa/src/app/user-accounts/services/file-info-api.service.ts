import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { HttpHelpers } from 'app-utilities/http-helpers';
import { AsyncResponse, toCxAsync } from 'app/core/cx-async';
import { AppConstant } from 'app/shared/app.constant';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { IGetFileByUserIdRequest } from '../dtos/get-file-by-user-id-request';
import { UploadFileRequest } from '../dtos/upload-file-request';
import { IFileInfo } from '../models/file-info.model';
import { PagingResponseModel } from '../models/user-management.model';

@Injectable()
export class FileInfoApiService {
  private readonly fileInfoApiUrl: string = `${AppConstant.api.organization}/file`;
  private readonly MASS_USER_CREATION: string = 'massusercreation';

  constructor(
    private httpHelper: HttpHelpers,
    private http: HttpClient,
    private translateAdapterService: TranslateAdapterService,
    private toastrService: ToastrService
  ) {}

  getFilesInfoByUserId(
    request: IGetFileByUserIdRequest
  ): Observable<PagingResponseModel<IFileInfo>> {
    return this.httpHelper.get<PagingResponseModel<IFileInfo>>(
      `${this.fileInfoApiUrl}/${this.MASS_USER_CREATION}`,
      request
    );
  }

  async uploadFileCSV(
    uploadFileRequest: UploadFileRequest
  ): Promise<IFileInfo> {
    const result = await this.uploadFile(uploadFileRequest);
    if (!result && !result.data) {
      this.toastrService.error(
        this.translateAdapterService.getValueImmediately(
          'RequestErrorMessage.504'
        )
      );

      return;
    }

    return result.data;
  }

  private async uploadFile(
    uploadFileRequest: UploadFileRequest
  ): Promise<AsyncResponse<IFileInfo>> {
    const formData = uploadFileRequest.toFormData();
    const headers = new HttpHeaders();

    return await toCxAsync(
      this.http
        .post<IFileInfo>(
          `${this.fileInfoApiUrl}/upload/${this.MASS_USER_CREATION}`,
          formData,
          { headers }
        )
        .toPromise()
    );
  }
}
