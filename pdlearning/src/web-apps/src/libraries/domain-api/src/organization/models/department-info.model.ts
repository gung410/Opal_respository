import { IEntityStatusModel, IIdentityModel } from '../../share/models/identity.model';

export const organizationUnitLevelConst = {
  Ministry: 30,
  Wing: 11,
  Division: 12,
  Branch: 13,
  Cluster: 14,
  School: 15,
  OrganizationUnit: 49
};

export enum OrganizationUnitLevelEnum {
  Ministry = 'ministry',
  Wing = 'wing',
  Division = 'division',
  Branch = 'branch',
  Cluster = 'cluster',
  School = 'school',
  OrganizationUnit = 'organizationUnit'
}

export interface IOrganizationalUnitTypesModel {
  identity: IIdentityModel;
  entityStatus: IEntityStatusModel;
}

export interface IDepartmentInfoModel {
  departmentName?: string;
  name?: string;
  parentDepartmentId: number;
  identity: IIdentityModel;
  entityStatus: IEntityStatusModel;
  organizationalUnitTypes?: IOrganizationalUnitTypesModel[];
}

export interface IDepartmentInfoResult {
  items: IDepartmentInfoModel[];
}

export class DepartmentInfoModel {
  public static optionalProps: (keyof DepartmentInfoModel)[] = ['isPartnering'];

  public id: number = -1;
  public extId: string = '';
  public parentDepartmentId: number = 0;
  public departmentName: string = '';
  public departmentType: OrganizationUnitLevelEnum | undefined;
  public isPartnering?: boolean;
  constructor(data?: IDepartmentInfoModel, isPartneringDepartments?: boolean) {
    if (data != null) {
      this.id = data.identity.id;
      this.departmentName = data.departmentName != null ? data.departmentName : data.name;
      this.parentDepartmentId = data.parentDepartmentId;

      const organizationUnitLevels =
        data.organizationalUnitTypes && data.organizationalUnitTypes.length > 0
          ? data.organizationalUnitTypes.filter(x => x.identity && x.identity != null).map(x => <OrganizationUnitLevelEnum>x.identity.extId)
          : null;

      this.departmentType = organizationUnitLevels ? organizationUnitLevels[0] : null;
      this.isPartnering = isPartneringDepartments;
      this.extId = data.identity.extId;
    }
  }

  public isSchoolDivision(): boolean {
    return this.extId === '10000666' && this.departmentType === OrganizationUnitLevelEnum.Division ? true : false;
  }
}
