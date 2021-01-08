import {
  Assignment,
  IParticipantAssignmentTrack,
  MetadataTagModel,
  NoOfAssessmentDoneInfo,
  ParticipantAssignmentTrack,
  PublicUserInfo
} from '@opal20/domain-api';

import { IGridDataItem } from '@opal20/infrastructure';

export interface IParticipantAssignmentTrackViewModel extends IParticipantAssignmentTrack {
  selected: boolean;
  user: PublicUserInfo;
  assignment: Assignment;
  registerAllMetadataDic: Dictionary<MetadataTagModel>;
  noOfAssessmentDone?: NoOfAssessmentDoneInfo;
}

// @dynamic
export class ParticipantAssignmentTrackViewModel extends ParticipantAssignmentTrack implements IGridDataItem {
  public id: string;
  public selected: boolean;
  public user: PublicUserInfo;
  public assignment: Assignment;
  public teachingSubjectsDisplayText: string = '';
  public teachingLevelDisplayText: string = '';
  public serviceChemeDisplayText: string = '';
  public registerAllMetadataDic: Dictionary<MetadataTagModel> = {};
  public noOfAssessmentDone?: NoOfAssessmentDoneInfo;

  public static createFromModel(
    participantAssignmentTrack: ParticipantAssignmentTrack,
    assignment: Assignment,
    user: PublicUserInfo,
    checkAll: boolean = false,
    selecteds: Dictionary<boolean> = {},
    registerAllMetadataDic: Dictionary<MetadataTagModel>,
    noOfAssessmentDone?: NoOfAssessmentDoneInfo
  ): ParticipantAssignmentTrackViewModel {
    return new ParticipantAssignmentTrackViewModel({
      ...participantAssignmentTrack,
      selected: checkAll || selecteds[participantAssignmentTrack.id],
      user: user,
      assignment: assignment,
      registerAllMetadataDic: registerAllMetadataDic,
      noOfAssessmentDone:
        noOfAssessmentDone != null
          ? noOfAssessmentDone
          : new NoOfAssessmentDoneInfo({
              participantAssignmentTrackId: participantAssignmentTrack.id,
              totalAssessments: 0,
              doneAssessments: 0
            })
    });
  }

  public get completionRate(): number {
    return this.getParticipantAssignmentTrackCompletionRate(this.assignment);
  }

  constructor(data?: IParticipantAssignmentTrackViewModel) {
    super(data);
    if (data != null) {
      this.selected = data.selected;
      this.user = data.user;
      this.assignment = data.assignment;
      this.registerAllMetadataDic = data.registerAllMetadataDic;
      this.serviceChemeDisplayText = this.getServiceSchemeDisplayText();
      this.teachingLevelDisplayText = this.getTeachingLevelDisplayText();
      this.teachingSubjectsDisplayText = this.getTeachingSubjectsDisplayText();
      this.noOfAssessmentDone = data.noOfAssessmentDone;
    }
  }
  public getServiceSchemeDisplayText(): string {
    if (this.user == null) {
      return '';
    }
    const serviceScheme = this.registerAllMetadataDic[this.user.serviceScheme];
    return serviceScheme ? serviceScheme.displayText : '';
  }

  public getTeachingSubjectsDisplayText(): string {
    if (this.user == null) {
      return '';
    }
    return []
      .concat(this.user.teachingSubjects)
      .map(p => this.registerAllMetadataDic[p])
      .filter(p => p != null)
      .map(p => p.displayText)
      .join(', ');
  }
  public getTeachingLevelDisplayText(): string {
    if (this.user == null) {
      return '';
    }
    return this.user.teachingLevels
      .map(p => this.registerAllMetadataDic[p])
      .filter(p => p != null)
      .map(p => p.displayText)
      .join(', ');
  }
}
