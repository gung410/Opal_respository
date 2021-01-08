import { DepartmentType } from 'app-models/department-type.model';
import { OrganizationalUnitTypeEnum } from '../constants/organizational-unit-type.enum';
import { findIndexCommon } from '../constants/common.const';
import { Injectable } from '@angular/core';

@Injectable()
export class OrganizationalUnitHelpers {
  static hasOrganizationalUnitType(
    organizationalUnitTypes: DepartmentType[],
    lookingType: OrganizationalUnitTypeEnum
  ): boolean {
    return (
      organizationalUnitTypes.findIndex(
        (role) => lookingType.toString() === role.identity.extId
      ) !== findIndexCommon.notFound
    );
  }

  static hasAnyOrganizationalUnitType(
    organizationalUnitTypes: DepartmentType[],
    lookingTypes: (OrganizationalUnitTypeEnum | string)[],
    acceptLookingTypesEmpty: boolean = false
  ): boolean {
    if (lookingTypes && lookingTypes.length === 0) {
      return acceptLookingTypesEmpty;
    }

    const lookingRoleExtIds =
      typeof lookingTypes[0] === 'string'
        ? lookingTypes
        : lookingTypes.map((r) => r.toString());

    return (
      organizationalUnitTypes.findIndex((role) =>
        lookingRoleExtIds.includes(role.identity.extId)
      ) !== findIndexCommon.notFound
    );
  }
}
