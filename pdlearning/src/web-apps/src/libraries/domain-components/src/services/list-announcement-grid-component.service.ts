import { AnnouncementRepository, SearchAnnouncementResult, UserRepository } from '@opal20/domain-api';
import { IFilter, Utils } from '@opal20/infrastructure';
import { Observable, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { AnnouncementViewModel } from '../models/announcement-view.model';
import { Injectable } from '@angular/core';

@Injectable()
export class ListAnnouncementGridComponentService {
  constructor(private announcementRepository: AnnouncementRepository, private userRepository: UserRepository) {}

  public loadAnnouncements(
    courseId: string,
    classRunId: string,
    filter: IFilter,
    skipCount: number = 0,
    maxResultCount: number = 25,
    checkAll: boolean = false,
    selectedsFn: () => Dictionary<boolean> | null = null
  ): Observable<OpalGridDataResult<AnnouncementViewModel>> {
    return this.progressAnnouncements(
      this.announcementRepository.searchAnnouncement(courseId, classRunId, filter, skipCount, maxResultCount),
      checkAll,
      selectedsFn
    );
  }

  private progressAnnouncements(
    announcementObs: Observable<SearchAnnouncementResult>,
    checkAll: boolean = false,
    selectedsFn: () => Dictionary<boolean> | null = null
  ): Observable<OpalGridDataResult<AnnouncementViewModel>> {
    return announcementObs.pipe(
      switchMap(searchAnnouncementResult => {
        if (searchAnnouncementResult.totalCount === 0) {
          return of(<OpalGridDataResult<AnnouncementViewModel>>{
            data: [],
            total: searchAnnouncementResult.totalCount
          });
        }

        return this.userRepository
          .loadPublicUserInfoList({ userIds: Utils.uniq(searchAnnouncementResult.items.map(_ => _.createdBy)) })
          .pipe(
            map(users => {
              const userDic = Utils.toDictionary(users, p => p.id);
              return <OpalGridDataResult<AnnouncementViewModel>>{
                data: searchAnnouncementResult.items.map(announcement => {
                  return AnnouncementViewModel.createFromModel(
                    announcement,
                    userDic[announcement.createdBy],
                    checkAll,
                    selectedsFn != null ? selectedsFn() : {}
                  );
                }),
                total: searchAnnouncementResult.totalCount
              };
            })
          );
      })
    );
  }
}
