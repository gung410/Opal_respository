import {
  AttendanceTrackingRepository,
  MetadataTagModel,
  SearchAttendaceTrackingResult,
  TaggingRepository,
  UserRepository
} from '@opal20/domain-api';
import { IFilter, Utils } from '@opal20/infrastructure';
import { Observable, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { AttendanceTrackingViewModel } from '../models/attendance-tracking-view.model';
import { Injectable } from '@angular/core';

@Injectable()
export class ListAttendanceTrackingGridComponentService {
  constructor(
    private attendaceTrackingRepository: AttendanceTrackingRepository,
    private userRepository: UserRepository,
    private taggingRepository: TaggingRepository
  ) {}

  public loadSearchAttendanceTracking(
    sessionId: string,
    searchText: string = '',
    filter: IFilter = null,
    skipCount: number = 0,
    maxResultCount: number = 25,
    checkAll: boolean = false,
    selectedsFn?: () => Dictionary<boolean> | null
  ): Observable<OpalGridDataResult<AttendanceTrackingViewModel>> {
    return this.progressAttendaceTracking(
      this.attendaceTrackingRepository.loadSearchAttendaceTracking(sessionId, searchText, filter, skipCount, maxResultCount),
      checkAll,
      selectedsFn
    );
  }

  private progressAttendaceTracking(
    attendanceTrakingObs: Observable<SearchAttendaceTrackingResult>,
    checkAll: boolean = false,
    selectedsFn?: () => Dictionary<boolean> | null
  ): Observable<OpalGridDataResult<AttendanceTrackingViewModel>> {
    return attendanceTrakingObs.pipe(
      switchMap(searchAttendanceTrakingResult => {
        if (searchAttendanceTrakingResult.totalCount === 0) {
          return of(<OpalGridDataResult<AttendanceTrackingViewModel>>{
            data: [],
            total: searchAttendanceTrakingResult.totalCount
          });
        }
        // Get facilitator infos
        return this.userRepository
          .loadPublicUserInfoList({ userIds: Utils.uniq(searchAttendanceTrakingResult.items.map(_ => _.userId)) })
          .pipe(
            switchMap(registers => {
              const registersAllMetadataIds = Utils.distinct(Utils.flatTwoDimensionsArray(registers.map(p => p.getAllMetadataIds())));
              const metadatasObs = Utils.isEmpty(registersAllMetadataIds)
                ? of(<MetadataTagModel[]>[])
                : this.taggingRepository.loadMetaDataTagsByIds(registersAllMetadataIds);
              return metadatasObs.pipe(
                map(metadatas => {
                  return {
                    registers,
                    metadatas
                  };
                })
              );
            }),
            map(data => {
              const registersDic = Utils.toDictionary(data.registers, p => p.id);
              return <OpalGridDataResult<AttendanceTrackingViewModel>>{
                data: searchAttendanceTrakingResult.items.map(_ =>
                  AttendanceTrackingViewModel.createFromModel(
                    _,
                    registersDic[_.userId],
                    checkAll,
                    selectedsFn != null ? selectedsFn() : {},
                    Utils.toDictionary(data.metadatas, p => p.id)
                  )
                ),
                total: searchAttendanceTrakingResult.totalCount
              };
            })
          );
      })
    );
  }
}
