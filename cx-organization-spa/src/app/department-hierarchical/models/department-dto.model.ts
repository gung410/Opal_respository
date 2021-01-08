import { EntityStatus } from 'app-models/entity-status.model';
import { Identity } from 'app-models/identity.model';

export class DepartmentDto {
  organizationNumber: string;
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
  languageId: number;
  countryCode: number;
  jsonDynamicAttributes: any;
  identity: Identity;
  dynamicAttributes: any[];
  entityStatus: EntityStatus;
  levels: LocalizedData[];
  constructor(data?: Partial<DepartmentDto>) {
    if (!data) {
      return;
    }
    this.organizationNumber = data.organizationNumber
      ? data.organizationNumber
      : '';
    this.address = data.address ? data.address : '';
    this.postalCode = data.postalCode ? data.postalCode : '';
    this.city = data.city ? data.city : '';
    this.phone = data.phone ? data.phone : '';
    this.email = data.email ? data.email : '';
    this.parentDepartmentId = data.parentDepartmentId;
    this.name = data.name ? data.name : '';
    this.departmentName = data.departmentName ? data.departmentName : '';
    this.description = data.description ? data.description : '';
    this.departmentDescription = data.departmentDescription
      ? data.departmentDescription
      : '';
    this.tag = data.tag ? data.tag : '';
    this.languageId = data.languageId;
    this.countryCode = data.countryCode;
    this.jsonDynamicAttributes = data.jsonDynamicAttributes
      ? data.jsonDynamicAttributes
      : '';
    this.identity = data.identity;
    this.dynamicAttributes = data.dynamicAttributes
      ? data.dynamicAttributes
      : [];
    this.entityStatus = data.entityStatus;
    this.levels = data.levels ? data.levels : [];
  }
}

export class LocalizedData {
  localizedData: any[];
  identity: Identity;
  entityStatus: EntityStatus;
}
