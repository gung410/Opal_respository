import { AssessmentAnswer, PublicUserInfo } from '@opal20/domain-api';
import { Observable, combineLatest, of } from 'rxjs';

import { Utils } from '@opal20/infrastructure';
import { map } from 'rxjs/operators';

export class PeerAssessmentViewModel {
  public static readonly create = create;

  public selectedToAddPeerAssessor: PublicUserInfo | null;
  public displayPeerAssessorsDic: Dictionary<PublicUserInfo> = {};

  private _assessmentAnswers: AssessmentAnswer[] = [];

  constructor(assessmentAnswers?: AssessmentAnswer[], userInfoDic?: Dictionary<PublicUserInfo>) {
    if (assessmentAnswers) {
      this.peerAssessors = assessmentAnswers;
      this.displayPeerAssessorsDic = userInfoDic;
    }
  }

  public get peerAssessors(): AssessmentAnswer[] {
    return this._assessmentAnswers;
  }

  public set peerAssessors(v: AssessmentAnswer[]) {
    this._assessmentAnswers = v;
  }

  public isSelectedToAddPeerAssessor(): boolean {
    return this.selectedToAddPeerAssessor != null;
  }

  public get selectedToAddPeerAssessorId(): string | null {
    return this.selectedToAddPeerAssessor ? this.selectedToAddPeerAssessor.id : null;
  }

  public get peerAssessorIds(): string[] | null {
    return this.peerAssessors ? this.peerAssessors.map(p => p.userId) : [];
  }

  public setSelectedToAddPeerAssessor(user: PublicUserInfo | null): void {
    this.selectedToAddPeerAssessor = user;
  }
}

function create(
  assessmentAnswers: AssessmentAnswer[],
  getUsersByAssessmentAnswersFn: (assessmentAnswers: AssessmentAnswer[]) => Observable<PublicUserInfo[]>
): Observable<PeerAssessmentViewModel> {
  const peerAssessment = assessmentAnswers.filter(p => p.userId !== AssessmentAnswer.assessmentForFacilitator);
  return combineLatest(peerAssessment.length === 0 ? of([]) : getUsersByAssessmentAnswersFn(peerAssessment)).pipe(
    map(([users]) => {
      return new PeerAssessmentViewModel(Utils.clone(peerAssessment), Utils.toDictionary(users, p => p.id));
    })
  );
}
