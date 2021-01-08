import { AreaOfProfessionalInterest, IAreaOfProfessionalInterest } from '../../share/models/area-of-professional-interest.model';
import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { Designation, IDesignation } from '../../share/models/designation.model';
import { IJobFamily, JobFamily } from '../../share/models/job-family.model';
import { IPortfolio, Portfolio } from '../../share/models/portfolio.model';
import { IRoleSpecificProficiency, RoleSpecificProficiency } from '../../share/models/role-specific-proficiency.model';
import { ITeachingLevel, TeachingLevel } from '../../share/models/teaching-level.model';
import { ITeachingSubject, TeachingSubject } from '../../share/models/teaching-subject.model';
import { ITypeOfOrganization, TypeOfOrganization } from '../../share/models/type-of-organization.model';

import { AuthService } from '@opal20/authentication';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class LearningCatalogApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.learningCatalogUrl + '/catalogentries/';
  }

  constructor(protected commonFacadeService: CommonFacadeService, private authService: AuthService) {
    super(commonFacadeService);
  }

  public getUserDesignation(showSpinner: boolean = true): Observable<Designation[]> {
    return this.get<IDesignation[]>(`explorer/DESIGNATION`, undefined, showSpinner).pipe(
      map(users => (users ? users.map(user => new Designation(user)) : []))
    );
  }

  public getUserPortfolios(showSpinner: boolean = true): Observable<Portfolio[]> {
    return this.get<IPortfolio[]>(`explorer/PORTFOLIOS`, undefined, showSpinner).pipe(
      map(users => (users ? users.map(user => new Portfolio(user)) : []))
    );
  }

  public getUserTypeOfOrganizations(showSpinner: boolean = true): Observable<TypeOfOrganization[]> {
    return this.get<ITypeOfOrganization[]>(`explorer/OU-TYPES`, undefined, showSpinner).pipe(
      map(users => (users ? users.map(user => new TypeOfOrganization(user)) : []))
    );
  }

  public getUserRoleSpecificProficiencies(showSpinner: boolean = true): Observable<RoleSpecificProficiency[]> {
    return this.get<IRoleSpecificProficiency[]>(`explorer/RSPS`, undefined, showSpinner).pipe(
      map(users => (users ? users.map(user => new RoleSpecificProficiency(user)) : []))
    );
  }

  public getUserAreasOfProfessionalInterest(showSpinner: boolean = true): Observable<AreaOfProfessionalInterest[]> {
    return this.get<IAreaOfProfessionalInterest[]>(`explorer/7415c00a-0f29-11ea-be55-0242ac120003`, undefined, showSpinner).pipe(
      map(users => (users ? users.map(user => new AreaOfProfessionalInterest(user)) : []))
    );
  }

  public getUserTeachingLevel(showSpinner: boolean = true): Observable<TeachingLevel[]> {
    return this.get<ITeachingLevel[]>(`explorer/TEACHING-LEVELS`, undefined, showSpinner).pipe(
      map(teachingLevels => (teachingLevels ? teachingLevels.map(teachingLevel => new TeachingLevel(teachingLevel)) : []))
    );
  }

  public getUserTeachingSubject(showSpinner: boolean = true): Observable<TeachingSubject[]> {
    return this.get<ITeachingSubject[]>(`explorer/TEACHING-SUBJECTS`, undefined, showSpinner).pipe(
      map(teachingSubjects => (teachingSubjects ? teachingSubjects.map(teachingSubject => new TeachingSubject(teachingSubject)) : []))
    );
  }

  public getUserJobFamily(showSpinner: boolean = true): Observable<JobFamily[]> {
    return this.get<IJobFamily[]>(`explorer/JOB-FAMILIES`, undefined, showSpinner).pipe(
      map(jobFamilies => (jobFamilies ? jobFamilies.map(jobFamily => new JobFamily(jobFamily)) : []))
    );
  }
}
