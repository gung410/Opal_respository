import { Injectable } from '@angular/core';
import { ArchetypeEnum } from 'app-enums/archetypeEnum';
import { PDPlanDto } from 'app-models/pdplan.model';
import { UserCountingParameter } from 'app-models/user-counting-parameter.model';
import { UserCounting } from 'app-models/user-counting.model';
import { CommentEventEntity } from 'app-services/comment-event.constant';
import { UserService } from 'app-services/user.service';
import {
  OdpActivity,
  OdpStatusCode,
} from 'app/organisational-development/learning-plan-detail/odp.constant';
import { IdpDto } from 'app/organisational-development/models/idp.model';
import { groupBy, maxBy } from 'lodash';

@Injectable()
export class KeyLearningProgramHelper {
  readonly learningAreaListSurveyName: string = 'listLearningArea';
  readonly learningOpportunityListSurveyName: string =
    'listLearningOpportunity';

  constructor(private userService: UserService) {}

  hasParentLearningDirectionApproved(parentDto: IdpDto): boolean {
    if (!parentDto || !parentDto.assessmentStatusInfo) {
      return false;
    }

    const statusCode = parentDto.assessmentStatusInfo.assessmentStatusCode;

    return statusCode === OdpStatusCode.Approved;
  }

  initDataForKLPWhenCreate(newPdplanDto: IdpDto): void {
    if (newPdplanDto.answer[this.learningAreaListSurveyName] === undefined) {
      newPdplanDto.answer[this.learningAreaListSurveyName] = [];
    }

    if (
      newPdplanDto.answer[this.learningOpportunityListSurveyName] === undefined
    ) {
      newPdplanDto.answer[this.learningOpportunityListSurveyName] = [];
    }
  }

  getCommentEventEntity(pdplanDto: PDPlanDto): CommentEventEntity {
    if (pdplanDto.pdPlanActivity === OdpActivity.Plan) {
      return CommentEventEntity.OdpLearningPlan;
    }
    if (pdplanDto.pdPlanActivity === OdpActivity.Direction) {
      return CommentEventEntity.OdpLearningDirection;
    }

    return CommentEventEntity.OdpKeyLearningProgramme;
  }

  async getTaggingByTargetAudienceOfUserGroupOrIndividualUsers(
    userGroupIds?: number[],
    userIds?: number[],
    userTypeArchetypes?: ArchetypeEnum[],
    getMostPopularInEachGroup?: boolean
  ): Promise<any> {
    const userCountingParameter: UserCountingParameter = {
      userTypeArchetypes,
      userGroupIds,
      userIds,
    };
    const userCountingByUserTypes = await this.userService.getUserCountingByUserTypesAsync(
      userCountingParameter
    );

    const groupsByArchetype = groupBy(
      userCountingByUserTypes,
      (userCounting) => {
        return userCounting.archetype.toString();
      }
    );
    if (getMostPopularInEachGroup === true) {
      // Find the most popular in each group.
      // Each group might have more than one popular subject if there is no most popular one.
      return this.getMostPopularUserTypeInEachGroup(
        groupsByArchetype,
        userCountingByUserTypes
      );
    }

    // Return the all user types in the grouping even though they are not the most popular one.
    const elementsInArchetype = {};
    for (const key in groupsByArchetype) {
      if (Object.prototype.hasOwnProperty.call(groupsByArchetype, key)) {
        const elements = groupsByArchetype[key];
        elementsInArchetype[key] = elements.map(
          (element) => element.userTypeExtId
        );
      }
    }

    return elementsInArchetype;
  }

  private getMostPopularUserTypeInEachGroup(
    groupsByArchetype: any,
    userCountingByUserTypes: UserCounting[]
  ): any {
    const maxCountInEachArchetype = {};
    for (const key in groupsByArchetype) {
      if (Object.prototype.hasOwnProperty.call(groupsByArchetype, key)) {
        const elements = groupsByArchetype[key];
        const maxCountInGroup: any = maxBy(elements, 'count');
        maxCountInEachArchetype[key] = maxCountInGroup.count;
      }
    }
    const mostPopularInEachArchetype = {};
    for (const key in maxCountInEachArchetype) {
      if (Object.prototype.hasOwnProperty.call(maxCountInEachArchetype, key)) {
        const maxCount = maxCountInEachArchetype[key];
        mostPopularInEachArchetype[key] = userCountingByUserTypes
          .filter(
            (element) =>
              element.archetype.toString() === key && element.count === maxCount
          )
          .map((element) => element.userTypeExtId);
      }
    }

    return mostPopularInEachArchetype;
  }
}
