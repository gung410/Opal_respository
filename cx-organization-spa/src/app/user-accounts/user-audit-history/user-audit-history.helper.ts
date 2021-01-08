import { ObjectUtilities } from 'app-utilities/object-utils';
import * as _ from 'lodash';
import * as moment from 'moment';

import { DatahubEventActionType } from '../constants/user-field-mapping.constant';
import { ApprovalGroup } from '../models/approval-group.model';
import { DatahubEventModel } from '../models/audit-history.model';
import { PDCatalogueEnumerationDto } from '../models/pd-catalogue.model';
import { findIndexCommon } from 'app/shared/constants/common.const';

export class UserAuditHistoryHelper {
  static getDiffs(
    objA: any,
    objB: any,
    fields: string[],
    metadatas: { [id: string]: PDCatalogueEnumerationDto[] }
  ): any {
    if (!objA || !objB) {
      return;
    }
    const temp = {};
    if (!fields || !fields.length) {
      return;
    }
    fields.forEach((field) => {
      let nestedA = _.get(objA, field);
      let nestedB = _.get(objB, field);
      if (Array.isArray(nestedA) || Array.isArray(nestedB)) {
        if (!_.isEqual(nestedA && nestedA.sort(), nestedB && nestedB.sort())) {
          nestedA = UserAuditHistoryHelper.mapMetadataIdToValue(
            field,
            nestedA,
            metadatas
          );
          nestedB = UserAuditHistoryHelper.mapMetadataIdToValue(
            field,
            nestedB,
            metadatas
          );
          temp[field] = { previous: nestedA, current: nestedB };

          return;
        }
      }
      if (typeof nestedA === 'object' && typeof nestedB === 'object') {
        const nestedDiff = UserAuditHistoryHelper.getDiffs(
          nestedA,
          nestedB,
          fields,
          metadatas
        );
        if (!ObjectUtilities.isEmpty(nestedDiff)) {
          temp[field] = nestedDiff;
        }

        return;
      }
      if (nestedA === nestedB) {
        return;
      }
      nestedA = UserAuditHistoryHelper.mapMetadataIdToValue(
        field,
        nestedA,
        metadatas
      );
      nestedB = UserAuditHistoryHelper.mapMetadataIdToValue(
        field,
        nestedB,
        metadatas
      );
      temp[field] = { previous: nestedA, current: nestedB };
    });

    return temp;
  }

  static mapMetadataIdToValue(
    metaField: string,
    metadataId: any,
    metadatas: { [id: string]: PDCatalogueEnumerationDto[] }
  ): any {
    const metadata = metadatas[metaField];
    if (!metadata) {
      return metadataId;
    }
    let metadataDto;
    if (Array.isArray(metadataId)) {
      metadataDto = metadata
        .filter(
          (data) => metadataId.indexOf(data.id) > findIndexCommon.notFound
        )
        .map((data) => data.displayText);
    } else {
      metadataDto = metadata.find((data) => metadataId === data.id);
      metadataDto = metadataDto ? metadataDto.displayText : metadataId;
    }

    return metadataDto ? metadataDto : metadataId;
  }

  static getApprovalGroupDiff(
    log: DatahubEventModel,
    nextLog: DatahubEventModel
  ): {} {
    const action = log.routing.action;
    const approvalGroup = log.approvalGroupInfo;
    if (nextLog) {
      const correlationId = log.payload.references.correlationId;
      const nextAction = nextLog.routing.action;
      const nextCorrelationId = nextLog.payload.references.correlationId;
      if (
        correlationId === nextCorrelationId &&
        action === DatahubEventActionType.ApprovalGroupDeleted &&
        nextAction === DatahubEventActionType.ApprovalGroupCreated
      ) {
        const nextApprovalGroup = nextLog.approvalGroupInfo;
        if (
          approvalGroup &&
          nextApprovalGroup &&
          approvalGroup.type === nextApprovalGroup.type
        ) {
          return { current: nextApprovalGroup, previous: approvalGroup };
        }
      }
    }
    const differ = {};
    const isCreated = action === DatahubEventActionType.ApprovalGroupCreated;
    differ[isCreated ? 'current' : 'previous'] = approvalGroup;

    return differ;
  }

  static getDateFormat(date): string {
    return moment(date).format('ll');
  }

  static getLocalizedTexts(roles: any[]): string[] {
    if (!roles || !roles.length) {
      return [];
    }

    return roles.map((role) => {
      if (
        role &&
        role.localizedData &&
        role.localizedData.length &&
        role.localizedData[0].fields.length
      ) {
        return role.localizedData[0].fields[0].localizedText;
      }

      return '';
    });
  }

  static mapArrayObjectToSimpleArray(
    log: DatahubEventModel
  ): DatahubEventModel {
    let userData = log.payload.body.userData;
    userData = {
      ...userData,
      systemRoles: UserAuditHistoryHelper.getLocalizedTexts(
        userData.systemRoles
      ),
      personnelGroups: UserAuditHistoryHelper.getLocalizedTexts(
        userData.personnelGroups
      ),
      careerPaths: UserAuditHistoryHelper.getLocalizedTexts(
        userData.careerPaths
      ),
      developmentalRoles: UserAuditHistoryHelper.getLocalizedTexts(
        userData.developmentalRoles
      ),
      learningFrameworks: UserAuditHistoryHelper.getLocalizedTexts(
        userData.learningFrameworks
      )
    };
    log.payload.body.userData = userData;

    return log;
  }
}
