// Department type (name - extId)
export const organizationUnitLevelsConst = {
  Ministry: 'ministry',
  Wing: 'wing',
  Division: 'division',
  Branch_Zone: 'branch',
  Cluster: 'cluster',
  School: 'school',
  Undefined: undefined
};

export const organizationUnitLevelIdsConst = {
  Ministry: 30,
  Wing: 11, //'200',
  Division: 12,
  Branch: 13,
  Cluster: 14, //'500',
  School: 15,
  OrganizationUnit: 49
};

export const organizationUnitLevelFlowConst = [
  {
    parentIdIdentity: organizationUnitLevelsConst.Undefined,
    childrenIdIdentity: organizationUnitLevelsConst.School
  }
];
