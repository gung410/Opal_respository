import { BaseUserInfo, IBaseUserInfo, PublicUserInfo, UserInfoModel } from '../share/models/user-info.model';

import { AreaOfProfessionalInterest } from './../share/models/area-of-professional-interest.model';
import { BaseRepositoryContext } from '@opal20/infrastructure';
import { BehaviorSubject } from 'rxjs';
import { Designation } from './../share/models/designation.model';
import { Injectable } from '@angular/core';
import { JobFamily } from '../share/models/job-family.model';
import { Portfolio } from './../share/models/portfolio.model';
import { RoleSpecificProficiency } from './../share/models/role-specific-proficiency.model';
import { TeachingLevel } from '../share/models/teaching-level.model';
import { TeachingSubject } from '../share/models/teaching-subject.model';
import { TypeOfOrganization } from './../share/models/type-of-organization.model';

@Injectable()
export class UserRepositoryContext extends BaseRepositoryContext {
  public usersSubject: BehaviorSubject<Dictionary<UserInfoModel>> = new BehaviorSubject({});
  public publicUsersSubject: BehaviorSubject<Dictionary<PublicUserInfo>> = new BehaviorSubject({});
  public baseUserInfoSubject: BehaviorSubject<Dictionary<BaseUserInfo>> = new BehaviorSubject({});
  public baseUserInfoResultSubject: BehaviorSubject<Dictionary<IBaseUserInfo>> = new BehaviorSubject({});
  public designationSubject: BehaviorSubject<Dictionary<Designation>> = new BehaviorSubject({});
  public portfolioSubject: BehaviorSubject<Dictionary<Portfolio>> = new BehaviorSubject({});
  public typeOfOrganizationSubject: BehaviorSubject<Dictionary<TypeOfOrganization>> = new BehaviorSubject({});
  public roleSpecificProficiencySubject: BehaviorSubject<Dictionary<RoleSpecificProficiency>> = new BehaviorSubject({});
  public areaOfProfessionalInterestSubject: BehaviorSubject<Dictionary<AreaOfProfessionalInterest>> = new BehaviorSubject({});
  public teachingSubjectSubject: BehaviorSubject<Dictionary<TeachingSubject>> = new BehaviorSubject({});
  public teachingLevelSubject: BehaviorSubject<Dictionary<TeachingLevel>> = new BehaviorSubject({});
  public jobFamilySubject: BehaviorSubject<Dictionary<JobFamily>> = new BehaviorSubject({});
}
