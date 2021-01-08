import { AreaOfProfessionalInterest } from '../../share/models/area-of-professional-interest.model';
import { BaseRepository } from '@opal20/infrastructure';
import { Designation } from '../../share/models/designation.model';
import { Injectable } from '@angular/core';
import { JobFamily } from '../../share/models/job-family.model';
import { LearningCatalogApiService } from '../services/learning-catalog-api.service';
import { Observable } from 'rxjs';
import { Portfolio } from '../../share/models/portfolio.model';
import { RoleSpecificProficiency } from '../../share/models/role-specific-proficiency.model';
import { TeachingLevel } from '../../share/models/teaching-level.model';
import { TeachingSubject } from '../../share/models/teaching-subject.model';
import { TypeOfOrganization } from '../../share/models/type-of-organization.model';
import { UserRepositoryContext } from '../user-repository-context';

@Injectable()
export class LearningCatalogRepository extends BaseRepository<UserRepositoryContext> {
  constructor(context: UserRepositoryContext, private apiSvc: LearningCatalogApiService) {
    super(context);
  }

  public loadUserDesignationList(showSpinner: boolean = true): Observable<Designation[]> {
    return this.processUpsertData(
      this.context.designationSubject,
      implicitLoad => this.apiSvc.getUserDesignation(!implicitLoad && showSpinner),
      'loadUserDesignationList',
      undefined,
      'implicitReload',
      (repoData, apiResult) => apiResult.map(item => repoData[item.id]).filter(_ => _ != null),
      apiResult => apiResult,
      x => x.id,
      null
    );
  }

  public loadUserPortfolioList(showSpinner: boolean = true): Observable<Portfolio[]> {
    return this.processUpsertData(
      this.context.portfolioSubject,
      implicitLoad => this.apiSvc.getUserPortfolios(!implicitLoad && showSpinner),
      'loadUserPortfolioList',
      undefined,
      'implicitReload',
      (repoData, apiResult) => apiResult.map(item => repoData[item.id]).filter(_ => _ != null),
      apiResult => apiResult,
      x => x.id,
      null
    );
  }

  public loadUserTypeOfOrganizationList(showSpinner: boolean = true): Observable<TypeOfOrganization[]> {
    return this.processUpsertData(
      this.context.typeOfOrganizationSubject,
      implicitLoad => this.apiSvc.getUserTypeOfOrganizations(!implicitLoad && showSpinner),
      'loadUserTypeOfOrganizationList',
      undefined,
      'implicitReload',
      (repoData, apiResult) => apiResult.map(item => repoData[item.id]).filter(_ => _ != null),
      apiResult => apiResult,
      x => x.id,
      null
    );
  }

  public loadUserRoleSpecificProficiencyList(showSpinner: boolean = true): Observable<RoleSpecificProficiency[]> {
    return this.processUpsertData(
      this.context.roleSpecificProficiencySubject,
      implicitLoad => this.apiSvc.getUserRoleSpecificProficiencies(!implicitLoad && showSpinner),
      'loadUserRoleSpecificProficiencyList',
      undefined,
      'implicitReload',
      (repoData, apiResult) => apiResult.map(item => repoData[item.id]).filter(_ => _ != null),
      apiResult => apiResult,
      x => x.id,
      null
    );
  }

  public loadUserAreaOfProfessionalInterestList(showSpinner: boolean = true): Observable<AreaOfProfessionalInterest[]> {
    return this.processUpsertData(
      this.context.areaOfProfessionalInterestSubject,
      implicitLoad => this.apiSvc.getUserAreasOfProfessionalInterest(!implicitLoad && showSpinner),
      'loadUserAreaOfProfessionalInterestList',
      undefined,
      'implicitReload',
      (repoData, apiResult) => apiResult.map(item => repoData[item.id]).filter(_ => _ != null),
      apiResult => apiResult,
      x => x.id,
      null
    );
  }

  public loadUserTeachingSubjects(showSpinner: boolean = true): Observable<TeachingSubject[]> {
    return this.processUpsertData(
      this.context.teachingSubjectSubject,
      implicitLoad => this.apiSvc.getUserTeachingSubject(!implicitLoad && showSpinner),
      'loadUserTeachingSubjects',
      undefined,
      'implicitReload',
      (repoData, apiResult) => apiResult.map(item => repoData[item.id]).filter(teachingSubject => teachingSubject),
      apiResult => apiResult,
      x => x.id,
      null
    );
  }

  public loadUserTeachingLevels(showSpinner: boolean = true): Observable<TeachingLevel[]> {
    return this.processUpsertData(
      this.context.teachingLevelSubject,
      implicitLoad => this.apiSvc.getUserTeachingLevel(!implicitLoad && showSpinner),
      'loadUserTeachingLevels',
      undefined,
      'implicitReload',
      (repoData, apiResult) => apiResult.map(item => repoData[item.id]).filter(teachingLevel => teachingLevel),
      apiResult => apiResult,
      x => x.id,
      null
    );
  }

  public loadUserJobFamiles(showSpinner: boolean = true): Observable<JobFamily[]> {
    return this.processUpsertData(
      this.context.jobFamilySubject,
      implicitLoad => this.apiSvc.getUserJobFamily(!implicitLoad && showSpinner),
      'loadUserJobFamiles',
      undefined,
      'implicitReload',
      (repoData, apiResult) => apiResult.map(item => repoData[item.id]).filter(jobFamily => jobFamily),
      apiResult => apiResult,
      x => x.id,
      null
    );
  }
}
