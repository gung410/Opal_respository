import { Injectable } from '@angular/core';
import { StatusTypeEnum } from 'app/shared/constants/user-status-type.enum';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { Utils } from '../../shared/utilities/utils';
import { IGetFileByUserIdRequest } from '../dtos/get-file-by-user-id-request';
import {
  PagingResponseModel,
  UserManagementQueryModel
} from '../models/user-management.model';
import { UserAccountsDataService } from '../user-accounts-data.service';
import { FileInfoListViewModel } from '../viewmodels/file-info-list.viewmodel';
import { FileInfoApiService } from './file-info-api.service';

@Injectable()
export class FileInfoListService {
  private newUploadedFileObservable: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(
    false
  );

  // tslint:disable-next-line:member-ordering
  newUploadedFile$: Observable<boolean> = this.newUploadedFileObservable.asObservable();

  private defaultStatusFilter: any[] = [
    StatusTypeEnum.Active.code,
    StatusTypeEnum.New.code,
    StatusTypeEnum.Deactive.code,
    StatusTypeEnum.Inactive.code,
    StatusTypeEnum.IdentityServerLocked.code
  ];

  constructor(
    private fileInfoApiService: FileInfoApiService,
    private userAccountsDataSvc: UserAccountsDataService
  ) {}

  notifyNewUploadedFile(isUpdated: boolean): void {
    this.newUploadedFileObservable.next(isUpdated);
  }

  getFilesInfoByUserId(
    request: IGetFileByUserIdRequest
  ): Observable<PagingResponseModel<FileInfoListViewModel>> {
    return this.fileInfoApiService.getFilesInfoByUserId(request).pipe(
      switchMap((files) => {
        if (files.totalItems === 0) {
          return of(null);
        }

        if (!files.items.length) {
          return of(files);
        }

        return this.userAccountsDataSvc
          .getUsers(
            new UserManagementQueryModel({
              extIds: Utils.distinct(files.items.map((file) => file.userGuid)),
              orderBy: 'firstName asc',
              userEntityStatuses: this.defaultStatusFilter,
              pageIndex: 0,
              pageSize: 0
            })
          )
          .pipe(
            switchMap((users) => {
              const userDic = Utils.toDictionary(users.items, (u) =>
                u.identity.extId.toUpperCase()
              );

              const fileInfoViewModelResult: PagingResponseModel<FileInfoListViewModel> = {
                items: files.items.map((file) =>
                  FileInfoListViewModel.createFromModel(
                    file,
                    userDic[file.userGuid.toUpperCase()]
                      ? userDic[file.userGuid.toUpperCase()].firstName
                      : 'N/A'
                  )
                ),
                hasMoreData: files.hasMoreData,
                pageIndex: files.pageIndex,
                pageSize: files.pageSize,
                totalItems: files.totalItems
              };

              return of(fileInfoViewModelResult);
            })
          );
      })
    );
  }
}
