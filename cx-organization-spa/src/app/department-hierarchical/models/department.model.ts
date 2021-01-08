import { DepartmentType } from 'app-models/department-type.model';
import { EntityStatus } from 'app-models/entity-status.model';
import { Identity } from 'app-models/identity.model';

export class Department {
  organizationNumber: string;
  organizationCode: string;
  address: string;
  postalCode: string;
  city: string;
  phone: string;
  email: string;
  parentDepartmentId: number;
  name: string;
  departmentName: string;
  description: string;
  departmentDescription: string;
  tag: string;
  path: string;
  languageId: number;
  countryCode: number;
  jsonDynamicAttributes: any;
  identity: Identity;
  dynamicAttributes: any[];
  entityStatus: EntityStatus;
  levels: LocalizedData[];
  childrenCount: number;
  customData: any;
  organizationalUnitTypes: DepartmentType[];
  clusterSuperintendent: string;
  zoneDirector: string;
  educationLevel: any;
  userCount: number;
  constructor(data?: Partial<Department>) {
    if (!data) {
      return;
    }
    this.organizationNumber = data.organizationNumber;
    this.address = data.address;
    this.postalCode = data.postalCode;
    this.organizationCode = data.organizationCode;
    this.city = data.city;
    this.phone = data.phone;
    this.email = data.email;
    this.parentDepartmentId = data.parentDepartmentId;
    this.name = data.name;
    this.departmentName = data.departmentName ? data.departmentName : data.name;
    this.description = data.description;
    this.departmentDescription = data.departmentDescription;
    this.tag = data.tag;
    this.path = data.path;
    this.languageId = data.languageId;
    this.countryCode = data.countryCode;
    this.jsonDynamicAttributes = data.jsonDynamicAttributes;
    this.identity = data.identity;
    this.dynamicAttributes = data.dynamicAttributes;
    this.entityStatus = data.entityStatus;
    this.levels = data.levels;
    this.childrenCount = data.childrenCount;
    this.customData = data.customData;
    this.organizationalUnitTypes = data.organizationalUnitTypes;
    this.clusterSuperintendent = data.clusterSuperintendent;
    this.zoneDirector = data.zoneDirector;
    this.educationLevel = data.educationLevel;
    this.userCount = data.userCount;
  }
}

// tslint:disable-next-line:max-classes-per-file
export class LocalizedData {
  localizedData: any[];
  identity: Identity;
  entityStatus: EntityStatus;
}

// tslint:disable-next-line:max-classes-per-file
export class OrganisationUnitType {
  id: string;
  displayText: string;
  fullStatement: string;
}
