import { Identity } from './common.model';
import { EntityStatus } from './entity-status.model';
import { LocalizedData } from './localized-data.model';
import { DepartmentType } from './department-type.model';

export class Department {
  public organizationNumber: string;
  public address: string;
  public postalCode: string;
  public city: string;
  public phone: string;
  public email: string;
  public parentDepartmentId: number;
  public name: string;
  public departmentName: string;
  public description: string;
  public departmentDescription: string;
  public tag: string;
  public languageId: number;
  public countryCode: number;
  public jsonDynamicAttributes: any;
  public identity: Identity;
  public dynamicAttributes: any[];
  public entityStatus: EntityStatus;
  public levels: LocalizedData[];
  public childrenCount: number;
  public customData: any;
  public organizationalUnitTypes: DepartmentType[];
  public path: string;
  constructor(data?: Partial<Department>) {
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
    this.childrenCount = data.childrenCount ? data.childrenCount : 0;
    this.customData = data.customData ? data.customData : '';
    this.organizationalUnitTypes = data.organizationalUnitTypes
      ? data.organizationalUnitTypes
      : [];
    this.path = data.path ? data.path : '';
  }
}
