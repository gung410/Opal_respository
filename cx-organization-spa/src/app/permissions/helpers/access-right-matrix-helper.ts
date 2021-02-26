import { Dictionary, Utils } from 'app-utilities/utils';
import { findIndexCommon } from 'app/shared/constants/common.const';
import { GrantedType } from '../enum/granted-type.enum';
import { ObjectType } from '../enum/object-type.enum';
import { AccessRightsLevel } from '../models/access-rights-level.model';
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

  static sortAccessRightsHierarchically(
    accessRights: AccessRightsModel[]
  ): AccessRightsLevel[] {
    const originalNodes = {}; // the original values
    const nodeIndex = {}; // tree nodes

    let i: number;
    for (i = 0; i < accessRights.length; i++) {
      const accessRightId = accessRights[i].id;
      const unsortedNode = {
        id: accessRightId,
        level: 1,
        children: [],
        sorted: false
      };
      originalNodes[accessRightId] = accessRights[i];
      nodeIndex[accessRightId] = unsortedNode;
    }

    // populate tree
    for (i = 0; i < accessRights.length; i++) {
      const node = nodeIndex[accessRights[i].id];
      let pNode = node;
      let j: number;
      let nextId = originalNodes[pNode.id].parentId;
      for (j = 0; nextId in nodeIndex; j++) {
        pNode = nodeIndex[nextId];
        if (j === 0) {
          pNode.children.push(node.id);
        }
        node.level++;
        nextId = originalNodes[pNode.id].parentId;
      }
    }

    // extract nodes then sort by level
    const nodes = [];
    for (const key in nodeIndex) {
      if (nodeIndex[key]) {
        nodes.push(nodeIndex[key]);
      }
    }
    nodes.sort((nodeA, nodeB) => {
      return nodeA.level - nodeB.level;
    });

    // sort array hierarchically
    const hierarchicalNodes = [];

    nodes.forEach((node) => {
      this.sortNodeHierarchically(node, nodes, hierarchicalNodes);
    });

    // sort the nodes hierarchically
    const finalSortedNotes: AccessRightsLevel[] = new Array();
    hierarchicalNodes.forEach((indexedNode) => {
      const originalNode = accessRights.find(
        (node: AccessRightsModel) => node.id === indexedNode.id
      );
      const originalAccessRightsLevel = new AccessRightsLevel({
        ...originalNode,
        level: indexedNode.level
      });
      finalSortedNotes.push(originalAccessRightsLevel);
    });

    return finalSortedNotes;
  }

  private static sortNodeHierarchically(
    node: any,
    originalNodes: any[],
    sortedNode: any[]
  ): any[] {
    // STOP CONDITION
    if (!node.children.length) {
      if (!sortedNode.includes(node)) {
        sortedNode.push(node);
      }

      return;
    }

    if (
      !sortedNode.includes(node) ||
      !node.children.some((childNode: any) => !sortedNode.includes(childNode))
    ) {
      sortedNode.push(node);
    }

    node.children.forEach((childNodeId) => {
      const originalNode = originalNodes.find(
        (existingNode: any) => existingNode.id === childNodeId
      );
      this.sortNodeHierarchically(originalNode, originalNodes, sortedNode);
    });
  }
}
