import { Assignment, MyAssignment, ParticipantAssignmentTrack } from '@opal20/domain-api';

export interface IMyAssignmentDetail {
  myAssignment: MyAssignment;
  assignment: Assignment;
  participantAssignmentTrack: ParticipantAssignmentTrack;

  score: number;
  totalScore: number;
}

export class MyAssignmentDetail implements IMyAssignmentDetail {
  public myAssignment: MyAssignment;
  public assignment: Assignment;
  public participantAssignmentTrack: ParticipantAssignmentTrack;

  public score: number;
  public totalScore: number;

  public static createMyAssignmentDetail(
    myAssignment: MyAssignment,
    assignment: Assignment,
    participantAssignmentTrack: ParticipantAssignmentTrack,
    totalScore: number,
    score: number
  ): MyAssignmentDetail {
    return new MyAssignmentDetail({ myAssignment, assignment, participantAssignmentTrack, score, totalScore });
  }
  constructor(data?: IMyAssignmentDetail) {
    if (!data) {
      return;
    }
    this.myAssignment = data.myAssignment;
    this.assignment = data.assignment;
    this.participantAssignmentTrack = data.participantAssignmentTrack;
    this.score = data.score;
    this.totalScore = data.totalScore;
  }
}
