import { DateUtils, IContainFilter, IFilter, IFromToFilter } from '@opal20/infrastructure';

import { UserInfoModel } from '@opal20/domain-api';

export enum CourseFilterOwnBy {
  All = 'all',
  MeOnly = 'me-only',
  OtherUsers = 'other-users'
}

export class CourseFilterModel {
  public ownBy?: CourseFilterOwnBy;
  public fromOrganisation?: number[];
  public createdDateFrom?: Date;
  public createdDateTo?: Date;
  public changedDateFrom?: Date;
  public changedDateTo?: Date;
  public status?: string[];
  public contentStatus?: string[];
  public pdType?: string[];
  public category?: string[];
  public serviceScheme?: string[];
  public teachingLevel?: string[];
  public learningFrameworks?: string[];
  public learningDimension?: string[];
  public developmentalRole?: string[];
  public learningArea?: string[];
  public learningSubArea?: string[];
  public subject?: string[];
  public courseLevel?: string[];

  public convert(): IFilter {
    const currentUser: UserInfoModel = UserInfoModel.getMyUserInfo();
    const containFilters: IContainFilter[] = [];
    const fromToFilters: IFromToFilter[] = [];

    containFilters.push(
      {
        field: 'CreatedBy',
        values: this.ownBy === CourseFilterOwnBy.All || this.ownBy == null ? [] : [currentUser.id],
        notContain: this.ownBy === CourseFilterOwnBy.OtherUsers
      },
      {
        field: 'DepartmentId',
        values: this.fromOrganisation ? this.fromOrganisation.map(p => p.toString()) : null,
        notContain: false
      },
      {
        field: 'Status',
        values: this.status,
        notContain: false
      },
      {
        field: 'ContentStatus',
        values: this.contentStatus,
        notContain: false
      },
      {
        field: 'PDActivityType',
        values: this.pdType,
        notContain: false
      },
      {
        field: 'CategoryIds',
        values: this.category,
        notContain: false
      },
      {
        field: 'ServiceSchemeIds',
        values: this.serviceScheme,
        notContain: false
      },
      {
        field: 'DevelopmentalRoleIds',
        values: this.developmentalRole,
        notContain: false
      },
      {
        field: 'TeachingLevels',
        values: this.teachingLevel,
        notContain: false
      },
      {
        field: 'CourseLevel',
        values: this.courseLevel,
        notContain: false
      },
      {
        field: 'SubjectAreaIds',
        values: this.subject,
        notContain: false
      },
      {
        field: 'LearningFrameworkIds',
        values: this.learningFrameworks,
        notContain: false
      },
      {
        field: 'LearningDimensionIds',
        values: this.learningDimension,
        notContain: false
      },
      {
        field: 'LearningAreaIds',
        values: this.learningArea,
        notContain: false
      },
      {
        field: 'LearningSubAreaIds',
        values: this.learningSubArea,
        notContain: false
      }
    );

    fromToFilters.push(
      {
        field: 'CreatedDate',
        fromValue: this.createdDateFrom ? DateUtils.removeTime(this.createdDateFrom).toLocaleString() : null,
        toValue: this.createdDateTo ? DateUtils.setTimeToEndInDay(this.createdDateTo).toLocaleString() : null,
        equalFrom: true,
        equalTo: true
      },
      {
        field: 'ChangedDate',
        fromValue: this.changedDateFrom ? DateUtils.removeTime(this.changedDateFrom).toLocaleString() : null,
        toValue: this.changedDateTo ? DateUtils.setTimeToEndInDay(this.changedDateTo).toLocaleString() : null,
        equalFrom: true,
        equalTo: true
      }
    );

    return {
      containFilters: containFilters,
      fromToFilters: fromToFilters
    };
  }
}
