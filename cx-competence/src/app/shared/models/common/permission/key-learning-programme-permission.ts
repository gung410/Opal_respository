import { User } from 'app-models/auth.model';
import { BaseUserPermission, IUserPermission } from './permission-setting';

export interface IKeyLearningProgrammePermission {
  keyLearningProgrammePermission: KeyLearningProgrammePermission;
  /**
   * Initializes the user permission.
   * @param loginUser The current logged-in user.
   */
  initKeyLearningProgrammePermission(loginUser: User): void;
}

export class KeyLearningProgrammePermission extends BaseUserPermission {
  learningAreas: IUserPermission;
  PDO: IUserPermission;
  externalPDO: IUserPermission;
  allowNominate: boolean;
  allowMassNominate: boolean;
  allowRecommend: boolean;

  constructor(loginUser?: User) {
    super();
    if (!loginUser) {
      return;
    }

    this.allowCreate = loginUser.hasPermission(ActionKey.LearningKLPCUD);
    this.allowEdit = loginUser.hasPermission(ActionKey.LearningKLPCUD);
    this.allowNominate = loginUser.hasPermission(ActionKey.LearningKLPCUD);
    this.allowMassNominate = loginUser.hasPermission(ActionKey.LearningKLPCUD);
    this.allowRecommend = loginUser.hasPermission(ActionKey.LearningKLPCUD);
    this.learningAreas = {
      allowCreate: loginUser.hasPermission(ActionKey.LearningKLPCUD),
      allowDelete: loginUser.hasPermission(ActionKey.LearningKLPCUD),
    } as IUserPermission;

    this.PDO = {
      allowCreate: loginUser.hasPermission(ActionKey.LearningKLPCUD),
      allowDelete: loginUser.hasPermission(ActionKey.LearningKLPCUD),
    } as IUserPermission;

    this.externalPDO = {
      allowCreate: loginUser.hasPermission(ActionKey.LearningKLPCUD),
      allowEdit: loginUser.hasPermission(ActionKey.LearningKLPCUD),
    } as IUserPermission;
  }
}

export enum ActionKey {
  LearningKLPCUD = 'CompetenceSpa.OrganisationalDevelopment.OrganisationalPDJourney.KLP.CUD',
}
