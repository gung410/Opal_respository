import { Dictionary, Utils } from 'app-utilities/utils';
import { findIndexCommon } from 'app/shared/constants/common.const';
import { GrantedType } from '../enum/granted-type.enum';
import { ObjectType } from '../enum/object-type.enum';
import { AccessRightsMatrixModel } from '../models/access-rights-matrix.model';
import { AccessRightsModel } from '../models/access-rights.model';
import { GrantedAccessRightsModel } from '../models/granted-access-rights.model';
import { SystemRoleModel } from '../models/system-role.model';
import { IUpdateAccessRightsRequest } from '../models/update-accessRights-request.model';

export class AccessRightMatrixHelper {
  static generateAccessRightsBySystemRoleId(
    accessRightsMatrixModel: AccessRightsMatrixModel,
    systemRoleId: number
  ): IUpdateAccessRightsRequest {
    const accessRights = accessRightsMatrixModel.grantedAccessRights.filter(
      (grantedAccessRight) => grantedAccessRight.systemRoleId === systemRoleId
    );

    // tslint:disable-next-line:no-angle-bracket-type-assertion
    const updateAccessRightsRequest = <IUpdateAccessRightsRequest>{
      systemRoleId,
      accessRights: []
    };

    const accessRightsDic = Utils.toDictionary(
      accessRightsMatrixModel.accessRights,
      (right) => right.id
    );

    const grantedAccessRightDic = Utils.toDictionary(
      accessRightsMatrixModel.grantedAccessRights,
      (grantedAccessRight) =>
        `${grantedAccessRight.accessRightId}-${grantedAccessRight.systemRoleId}`
    );

    accessRights.forEach((accessRight) => {
      const updatedAccessRight = Utils.clone(
        accessRightsDic[accessRight.accessRightId],
        (clonedAccessRight) => {
          clonedAccessRight.grantedType =
            grantedAccessRightDic[
              `${accessRight.accessRightId}-${systemRoleId}`
            ].grantedType;
        }
      );

      updateAccessRightsRequest.accessRights.push(updatedAccessRight);
    });

    return updateAccessRightsRequest;
  }

  static addModuleInfo(
    moduleId: number,
    updateAccessRightsRequest: IUpdateAccessRightsRequest
  ): IUpdateAccessRightsRequest {
    const hasPermissions = updateAccessRightsRequest.accessRights.some(
      (accessRight) => accessRight.grantedType === GrantedType.Allow
    );

    updateAccessRightsRequest.accessRights.push(
      new AccessRightsModel({
        grantedType: hasPermissions ? GrantedType.Allow : GrantedType.Deny,
        id: moduleId,
        objectType: ObjectType.Module
      })
    );

    return updateAccessRightsRequest;
  }

  static updateAccessRightsMatrix(
    accessRightsMatrixModel: AccessRightsMatrixModel,
    grantedAccessRights: GrantedAccessRightsModel[]
  ): AccessRightsMatrixModel {
    grantedAccessRights.forEach((accessRights) => {
      const grantedAccessRightIndex = accessRightsMatrixModel.grantedAccessRights.findIndex(
        (accessRight) =>
          accessRight.accessRightId === accessRights.accessRightId &&
          accessRight.systemRoleId === accessRights.systemRoleId
      );

      if (grantedAccessRightIndex === findIndexCommon.notFound) {
        return null;
      }

      accessRightsMatrixModel.grantedAccessRights[
        grantedAccessRightIndex
      ].grantedType = accessRights.grantedType;
    });

    return accessRightsMatrixModel;
  }

  // static reorderLevelsOfAccessRights(
  //   accessRights: AccessRightsModel[],
  //   moduleId: number
  // ): void {
  //   const orderedAccessRights: AccessRightsModel[] = [];

  //   const mainAccessRights = accessRights.filter(
  //     (accessRight) => accessRight.parentId === moduleId
  //   );

  //   mainAccessRights.forEach((accessRight) => {
  //     const hierarchyOfAccessRight = this.getHierarchyOfAccessRight(
  //       accessRights.slice(mainAccessRights.length),
  //       accessRight
  //     );

  //     orderedAccessRights.push(accessRight, ...hierarchyOfAccessRight);
  //   });

  //   // return orderedAccessRights;
  //   console.log('re-order:');
  //   // console.log(orderedAccessRights);
  //   this.printAccessRights(orderedAccessRights);
  // }

  // static getHierarchyOfAccessRight(
  //   accessRights: AccessRightsModel[],
  //   parentAccessRight: AccessRightsModel
  // ): AccessRightsModel[] {
  //   const orderedAccessRights: AccessRightsModel[] = [];

  //   const accessRightsByParentId = accessRights.filter(
  //     (accessRight) => accessRight.parentId === parentAccessRight.id
  //   );

  //   if (accessRightsByParentId.length > 0) {
  //     accessRightsByParentId.forEach((accessRight) => {
  //       const result = this.getHierarchyOfAccessRight(
  //         accessRights,
  //         accessRight
  //       );

  //       orderedAccessRights.push(...result);
  //     });

  //     return orderedAccessRights;
  //   }

  //   return [parentAccessRight];
  // }

  static printAccessRights(accessRights: AccessRightsModel[]): void {
    accessRights.forEach((r) => {
      console.log(
        '---------------------------------------------------------------------'
      );
      console.log('Id: ' + r.id);
      console.log('Name: ' + r.localizedData[0].fields[0].localizedText);
      console.log('Type: ' + r.objectType);
      console.log('No: ' + r.no);
      console.log('Parent id: ' + r.parentId);
    });
  }
}
