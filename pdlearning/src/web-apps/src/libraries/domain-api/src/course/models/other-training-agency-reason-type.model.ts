export enum OtherTrainingAgencyReasonType {
  Bandwidth = 'Bandwidth',
  Capacity = 'Capacity',
  PhysicalResources = 'PhysicalResources',
  OwnerDivisionAcademy = 'OwnerDivision/Academy'
}

export const otherTrainingAgencyReasonDic = {
  [OtherTrainingAgencyReasonType.Bandwidth]: 'Bandwidth',
  [OtherTrainingAgencyReasonType.Capacity]: 'Capacity',
  [OtherTrainingAgencyReasonType.PhysicalResources]: 'Physical Resources',
  [OtherTrainingAgencyReasonType.OwnerDivisionAcademy]: 'Owner Division/ Academy'
};
