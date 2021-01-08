import {
  AssessmentAnswerRepository,
  Assignment,
  AssignmentRepository,
  MetadataTagModel,
  NoOfAssessmentDoneInfo,
  ParticipantAssignmentTrackRepository,
  ParticipantAssignmentTrackResult,
  PublicUserInfo,
  TaggingRepository,
  UserRepository
} from '@opal20/domain-api';
import { IFilter, Utils } from '@opal20/infrastructure';
import { Observable, combineLatest, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { Injectable } from '@angular/core';
import { ParticipantAssignmentTrackViewModel } from '../models/participant-assignment-track-view.model';

@Injectable()
export class ListParticipantAssignmentTrackGridComponentService {
  constructor(
    private participantAssignmentTrackRepository: ParticipantAssignmentTrackRepository,
    private userRepository: UserRepository,
    private assignmentRepository: AssignmentRepository,
    private taggingRepository: TaggingRepository,
    private assessmentAnswerRepository: AssessmentAnswerRepository
  ) {}

  public loadParticipantAssignmentTracks(
    courseId: string,
    classRunId: string,
    assignmentId: string,
    searchText: string = '',
    filter: IFilter = null,
    registrationIds: string[],
    skipCount: number = 0,
    maxResultCount: number = 25,
    checkAll: boolean = false,
    selectedsFn: () => Dictionary<boolean> | null = null,
    includeQuizAssignmentFormAnswer: boolean = false
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
        maxResultCount,
        includeQuizAssignmentFormAnswer
      ),
      checkAll,
      selectedsFn
    );
  }
  private progressParticipantAssignmentTracks(
    participantAssignmentTrackObs: Observable<ParticipantAssignmentTrackResult>,
    checkAll: boolean = false,
    selectedsFn: () => Dictionary<boolean> | null = null
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
          }),
          this.assignmentRepository.getAssignmentById(
            searchParticipantAssignmentTracksResult.items !== [] ? searchParticipantAssignmentTracksResult.items[0].assignmentId : null
          )
        ).pipe(
          switchMap(([courseOwners, assignment]) => {
            const registersAllMetadataIds = Utils.distinct(Utils.flatTwoDimensionsArray(courseOwners.map(p => p.getAllMetadataIds())));
            const metadatasObs = Utils.isEmpty(registersAllMetadataIds)
              ? of(<MetadataTagModel[]>[])
              : this.taggingRepository.loadMetaDataTagsByIds(registersAllMetadataIds);

            const assessmentObs = <Observable<NoOfAssessmentDoneInfo[]>>(
              (assignment.assessmentConfig
                ? this.assessmentAnswerRepository.loadNoOfAssessmentDone(searchParticipantAssignmentTracksResult.items.map(_ => _.id))
                : of(null))
            );
            return combineLatest(metadatasObs, assessmentObs).pipe(
              map(([metadatas, noOfAssessmentDones]) => {
                return <[PublicUserInfo[], Assignment, MetadataTagModel[], NoOfAssessmentDoneInfo[]]>[
                  courseOwners,
                  assignment,
                  metadatas,
                  noOfAssessmentDones
                ];
              })
            );
          }),
          map(([courseOwners, assignment, metadatas, noOfAssessmentDones]) => {
            const noOfAssessmentDonesDic = Utils.toDictionary(noOfAssessmentDones, p => p.participantAssignmentTrackId);
            const registersDic = Utils.toDictionary(courseOwners, p => p.id);
            const searchParticipantAssignmentTracksVmResult = <OpalGridDataResult<ParticipantAssignmentTrackViewModel>>{
              data: searchParticipantAssignmentTracksResult.items.map(_ =>
                ParticipantAssignmentTrackViewModel.createFromModel(
                  _,
                  assignment,
                  registersDic[_.userId],
                  checkAll,
                  selectedsFn != null ? selectedsFn() : {},
                  Utils.toDictionary(metadatas, p => p.id),
                  noOfAssessmentDonesDic[_.id]
                )
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
