const PDPLAN_CONSTANT = {
  OwnerId: 3001,
  CustomerId: 2052,
  Archetype: 'Assessment'
};

interface IDeactivatePDPlanDto {
  identities: IResultIdentity[];
  deactivateAllVersion: boolean;
}

export interface IResultIdentity {
  archetype?: string;
  id?: number;
  ownerId?: number;
  customerId?: number;
  extId?: string;
}

export class DeactivatePDPlanDto implements IDeactivatePDPlanDto {
  public identities: IResultIdentity[];
  public deactivateAllVersion: boolean;
  constructor(resultId?: string) {
    if (!resultId) {
      return;
    }
    this.identities = [
      {
        extId: resultId,
        ownerId: PDPLAN_CONSTANT.OwnerId,
        customerId: PDPLAN_CONSTANT.CustomerId,
        archetype: PDPLAN_CONSTANT.Archetype
      }
    ];
    this.deactivateAllVersion = true;
  }
}
