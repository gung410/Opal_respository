import { BaseUserInfo, PublicUserInfo, SystemRoleEnum } from '../../share/models/user-info.model';

import { BaseUserInfoWithCheckMoreData } from '../dtos/get-user-result.dto';
import { Observable } from 'rxjs';
import { UserRepository } from '../repositories/user.repository';

export class UserUtils {
  public static createFetchUsersByDeptWithCheckMoreDataFn(
    departmentIds: number[],
    userRepository: UserRepository,
    showSpinner: boolean = false
  ): (searchText: string, skipCount: number, maxResultCount: number) => Observable<BaseUserInfoWithCheckMoreData> {
    return (_searchText, _skipCount, _maxResultCount) => {
      return userRepository.loadBaseUserInfoListResult(
        {
          pageSize: _maxResultCount,
          pageIndex: _maxResultCount === 0 ? 1 : _skipCount / _maxResultCount + 1,
          searchKey: _searchText,
          departmentIds: departmentIds
        },
        showSpinner
      );
    };
  }

  public static createFetchUsersWithCheckMoreDataFn(
    inRoles: SystemRoleEnum[] = [],
    userRepository: UserRepository,
    showSpinner: boolean = false
  ): (searchText: string, skipCount: number, maxResultCount: number) => Observable<BaseUserInfoWithCheckMoreData> {
    return (_searchText, _skipCount, _maxResultCount) => {
      return userRepository.loadBaseUserInfoListResult(
        {
          userTypeExtIds: inRoles,
          pageSize: _maxResultCount,
          pageIndex: _maxResultCount === 0 ? 1 : _skipCount / _maxResultCount + 1,
          searchKey: _searchText
        },
        showSpinner
      );
    };
  }

  public static createFetchUsersInfoFn(
    userRepository: UserRepository,
    showSpinner: boolean = false
  ): (searchText: string, skipCount: number, maxResultCount: number) => Observable<BaseUserInfo[]> {
    return (_searchText, _skipCount, _maxResultCount) => {
      return userRepository.loadBaseUserInfoList(
        {
          pageSize: _maxResultCount,
          pageIndex: _maxResultCount === 0 ? 1 : _skipCount / _maxResultCount + 1,
          searchKey: _searchText
        },
        showSpinner
      );
    };
  }

  public static createFetchUsersFn(
    inRoles: SystemRoleEnum[] = [],
    userRepository: UserRepository,
    showSpinner: boolean = false
  ): (searchText: string, skipCount: number, maxResultCount: number) => Observable<BaseUserInfo[]> {
    return (_searchText, _skipCount, _maxResultCount) => {
      return userRepository.loadBaseUserInfoList(
        {
          userTypeExtIds: inRoles,
          pageSize: _maxResultCount,
          pageIndex: _maxResultCount === 0 ? 1 : _skipCount / _maxResultCount + 1,
          searchKey: _searchText
        },
        showSpinner
      );
    };
  }

  public static createFetchUsersByPermissionsFn(
    hasPermissions: string[] = [],
    userRepository: UserRepository,
    showSpinner: boolean = false
  ): (searchText: string, skipCount: number, maxResultCount: number) => Observable<BaseUserInfo[]> {
    return (_searchText, _skipCount, _maxResultCount) => {
      return userRepository.loadBaseUserInfoList(
        {
          pageSize: _maxResultCount,
          pageIndex: _maxResultCount === 0 ? 1 : _skipCount / _maxResultCount + 1,
          searchKey: _searchText,
          systemRolePermissions: hasPermissions
        },
        showSpinner
      );
    };
  }

  public static createFetchUsersByIdsFn(
    userRepository: UserRepository,
    showSpinner: boolean = false,
    additionalStatuses: string[] = []
  ): (ids: string[]) => Observable<BaseUserInfo[]> {
    return ids => {
      return userRepository.loadBaseUserInfoList(
        {
          extIds: ids,
          pageSize: 0,
          pageIndex: 0,
          entityStatuses: additionalStatuses
        },
        showSpinner
      );
    };
  }

  public static createFetchPublicUsersByIdsFn(
    userRepository: UserRepository,
    showSpinner: boolean = false
  ): (ids: string[]) => Observable<PublicUserInfo[]> {
    return ids => {
      return userRepository.loadPublicUserInfoList(
        {
          userIds: ids
        },
        showSpinner
      );
    };
  }
}
