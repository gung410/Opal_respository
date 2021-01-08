import { ClassRunRepository, PublicUserInfo, SearchClassRunResult, SearchClassRunType, UserRepository } from '@opal20/domain-api';
import { IFilter, Utils } from '@opal20/infrastructure';
import { Observable, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { ClassRunViewModel } from './../models/classrun-view.model';
import { Injectable } from '@angular/core';

@Injectable()
export class ListClassRunGridComponentService {
  constructor(private classRunRepository: ClassRunRepository, private userRepository: UserRepository) {}

  public loadClassRunsByCourseId(
    courseId: string = undefined,
    searchType?: SearchClassRunType,
    searchText: string = '',
    filter: IFilter = null,
    notStarted: boolean = false,
    notEnded: boolean = false,
    skipCount: number = 0,
    maxResultCount: number = 25,
    checkAll: boolean = false,
    selectedsFn: () => Dictionary<boolean> | null = null,
    loadHasContentInfo?: boolean
  ): Observable<OpalGridDataResult<ClassRunViewModel>> {
    return this.progressCourses(
      this.classRunRepository.loadClassRunsByCourseId(
        courseId,
        searchType,
        searchText,
        filter,
        notStarted,
        notEnded,
        skipCount,
        maxResultCount,
        loadHasContentInfo
      ),
      checkAll,
      selectedsFn
    );
  }

  private progressCourses(
    classRunObs: Observable<SearchClassRunResult>,
    checkAll: boolean = false,
    selectedsFn: () => Dictionary<boolean> | null = null
  ): Observable<OpalGridDataResult<ClassRunViewModel>> {
    return classRunObs.pipe(
      switchMap(searchClassRunResult => {
        if (searchClassRunResult.totalCount === 0) {
          return of(<OpalGridDataResult<ClassRunViewModel>>{
            data: [],
            total: searchClassRunResult.totalCount
          });
        }

        // Get list facilitatorIds
        let facilitatorIds = [];
        searchClassRunResult.items.forEach(_ => {
          facilitatorIds = facilitatorIds.concat(_.facilitatorIds);
        });

        // Get facilitator info
        return this.userRepository.loadPublicUserInfoList({ userIds: Utils.uniq(facilitatorIds) }).pipe(
          map(users => {
            const userDic = Utils.toDictionary(users, p => p.id);
            return <OpalGridDataResult<ClassRunViewModel>>{
              data: searchClassRunResult.items.map(classRun => {
                const facilitators = classRun.facilitatorIds
                  ? classRun.facilitatorIds
                      .filter(_ => userDic[_] != null)
                      .map(_ => userDic[_])
                      .map(
                        _ =>
                          new PublicUserInfo({
                            id: _.userCxId,
                            avatarUrl: _.avatarUrl,
                            fullName: _.fullName,
                            userCxId: _.id,
                            departmentId: _.departmentId,
                            departmentName: _.departmentName,
                            emailAddress: _.emailAddress
                          })
                      )
                  : [];
                return ClassRunViewModel.createFromModel(classRun, facilitators, checkAll, selectedsFn ? selectedsFn() : {});
              }),
              total: searchClassRunResult.totalCount
            };
          })
        );
      })
    );
  }
}
