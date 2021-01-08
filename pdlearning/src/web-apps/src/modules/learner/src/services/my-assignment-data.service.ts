import {
  Assignment,
  AssignmentApiService,
  IMyAssignmentRequest,
  MyAssignmentApiService,
  ParticipantAssignmentTrack,
  ParticipantAssignmentTrackApiService
} from '@opal20/domain-api';

import { Injectable } from '@angular/core';
import { MyAssignmentDetail } from '../models/my-assignment-detail-model';

@Injectable()
export class MyAssignmentDataService {
  public static calculateScore(
    pat: ParticipantAssignmentTrack
  ): { participantAssignmentTrack: ParticipantAssignmentTrack; score: number; totalScore: number } {
    if (!pat || !pat.quizAnswer) {
      return { participantAssignmentTrack: pat, score: 0, totalScore: 0 };
    }

    const score = pat.quizAnswer.score ? pat.quizAnswer.score : 0;
    const scorePercentage = pat.quizAnswer.scorePercentage ? pat.quizAnswer.scorePercentage : 0;
    const total = scorePercentage === 0 ? 0 : Math.round((100 * score) / scorePercentage);
    return { participantAssignmentTrack: pat, score: score, totalScore: total };
  }

  constructor(
    private myAssignmentApiService: MyAssignmentApiService,
    private assignmentApiService: AssignmentApiService,
    private participantAssignmentTrackApiService: ParticipantAssignmentTrackApiService
  ) {}

  public getMyAssignments(registrationId: string): Promise<MyAssignmentDetail[]> {
    const request: IMyAssignmentRequest = { registrationId: registrationId, maxResultCount: 10000, skipCount: 0 };
    return this.myAssignmentApiService.getMyAssignments(request).then(myResult => {
      const assignmentIds: string[] = myResult.map(p => p.assignmentId);
      const participantAssignmentTracksIds: string[] = myResult.map(p => p.participantAssignmentTrackId);

      const getListPATTask: Promise<
        ParticipantAssignmentTrack[]
      > = this.participantAssignmentTrackApiService.getParticipantAssignmentTrackByIds(participantAssignmentTracksIds);

      const getAssignmentsByIdsTask: Promise<Assignment[]> = this.assignmentApiService.getAssignmentsByIds(assignmentIds);

      return Promise.all([getListPATTask, getAssignmentsByIdsTask]).then(
        ([listPATResult, listAssignmentResult]: [ParticipantAssignmentTrack[], Assignment[]]) => {
          return listAssignmentResult.map(m => {
            const myAssignment = myResult.find(x => x.assignmentId === m.id);
            const { participantAssignmentTrack, totalScore, score } = MyAssignmentDataService.calculateScore(
              listPATResult.find(x => x.assignmentId === m.id)
            );

            return MyAssignmentDetail.createMyAssignmentDetail(myAssignment, m, participantAssignmentTrack, totalScore, score);
          });
        }
      );
    });
  }
}
