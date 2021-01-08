import { IFilter, Utils } from '@opal20/infrastructure';
import { Observable, combineLatest, of } from 'rxjs';
import { ParticipantAssignmentTrackRepository, ParticipantAssignmentTrackResult, UserRepository } from '@opal20/domain-api';
import { map, switchMap } from 'rxjs/operators';

import { Injectable } from '@angular/core';
import { ParticipantAssignmentTrackViewModel } from '@opal20/domain-components';

@Injectable()
export class PeerAssessmentPageComponentService {
  constructor(private participantAssignmentTrackRepository: ParticipantAssignmentTrackRepository, private userRepository: UserRepository) {}

  public loadParticipantAssignmentTracks(
    courseId: string,
    classRunId: string,
    assignmentId: string,
    searchText: string = '',
    filter: IFilter = null,
    registrationIds: string[],
    skipCount: number = 0,
    maxResultCount: number = 25
  ): Observable<OpalGridDataResult<ParticipantAssignmentTrackViewModel>> {
    return this.progressParticipantAssignmentTracks(
      this.participantAssignmentTrackRepository.loadParticipantAssignmentTracks(
        courseId,
        classRunId,
        assignmentId,
        searchText,
        filter,
        registrationIds,
        skipCount,
        maxResultCount
      )
    );
  }
  private progressParticipantAssignmentTracks(
    participantAssignmentTrackObs: Observable<ParticipantAssignmentTrackResult>
  ): Observable<OpalGridDataResult<ParticipantAssignmentTrackViewModel>> {
    return participantAssignmentTrackObs.pipe(
      switchMap(searchParticipantAssignmentTracksResult => {
        if (searchParticipantAssignmentTracksResult.totalCount === 0) {
          return of(<OpalGridDataResult<ParticipantAssignmentTrackViewModel>>{
            data: [],
            total: searchParticipantAssignmentTracksResult.totalCount
          });
        }
        return combineLatest(
          this.userRepository.loadPublicUserInfoList({
            userIds: Utils.uniq(searchParticipantAssignmentTracksResult.items.map(_ => _.userId))
          })
        ).pipe(
          map(([courseOwners]) => {
            const registersDic = Utils.toDictionary(courseOwners, p => p.id);
            const searchParticipantAssignmentTracksVmResult = <OpalGridDataResult<ParticipantAssignmentTrackViewModel>>{
              data: searchParticipantAssignmentTracksResult.items.map(_ =>
                ParticipantAssignmentTrackViewModel.createFromModel(_, null, registersDic[_.userId], false, {}, {})
              ),
              total: searchParticipantAssignmentTracksResult.totalCount
            };
            return searchParticipantAssignmentTracksVmResult;
          })
        );
      })
    );
  }
}
