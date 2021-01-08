import {
  AreaOfProfessionalInterest,
  Designation,
  MetadataTagModel,
  Portfolio,
  PublicUserInfo,
  RoleSpecificProficiency,
  TypeOfOrganization
} from '@opal20/domain-api';

export interface ILearnerProfileViewModel {
  user: PublicUserInfo;
  registerAllMetadataDic: Dictionary<MetadataTagModel>;
  registerDesginationDic: Dictionary<Designation>;
  registerPortfolioDic: Dictionary<Portfolio>;
  registerTypeOfOrganizationDic: Dictionary<TypeOfOrganization>;
  registerRoleSpecificProficiencyDic: Dictionary<RoleSpecificProficiency>;
  registerAreaOfProfessionalInterestDic: Dictionary<AreaOfProfessionalInterest>;
}

export class LearnerProfileViewModel implements ILearnerProfileViewModel {
  public user: PublicUserInfo = new PublicUserInfo();
  public registerAllMetadataDic: Dictionary<MetadataTagModel> = {};
  public registerDesginationDic: Dictionary<Designation> = {};
  public registerPortfolioDic: Dictionary<Portfolio> = {};
  public registerTypeOfOrganizationDic: Dictionary<TypeOfOrganization> = {};
  public registerRoleSpecificProficiencyDic: Dictionary<RoleSpecificProficiency> = {};
  public registerAreaOfProfessionalInterestDic: Dictionary<AreaOfProfessionalInterest> = {};
  public designationDisplayText: string = '';
  public teachingLevelDisplayTexts: string[] = [];
  public portfoliosDisplayTexts: string[] = [];
  public serviceSchemeDisplayText: string = '';
  public teachingSubjectsDisplayTexts: string[] = [];
  public coCurricularActivitiesDisplayTexts: string[] = [];
  public teachingCourseOfStudyDisplayTexts: string[] = [];
  public typeOfOrganizationDisplayText: string = '';
  public areasOfProfessionalInterestDisplayTexts: string[] = [];
  public roleSpecificProficienciesDisplayTexts: string[] = [];
  private _registerDesginationDic: Dictionary<Designation> = {};
  private _registerAllMetadataDic: Dictionary<MetadataTagModel> = {};
  private _registerPortfolioDic: Dictionary<Portfolio> = {};
  private _registerTypeOfOrganizationDic: Dictionary<TypeOfOrganization> = {};
  private _registerRoleSpecificProficiencyDic: Dictionary<RoleSpecificProficiency> = {};
  private _registerAreaOfProfessionalInterestDic: Dictionary<AreaOfProfessionalInterest> = {};

  public static createFromModel(
    user?: PublicUserInfo,
    registerAllMetadataDic: Dictionary<MetadataTagModel> = null,
    registerDesginationDic: Dictionary<Designation> = null,
    registerPortfolioDic: Dictionary<Portfolio> = null,
    registerTypeOfOrganizationDic: Dictionary<TypeOfOrganization> = null,
    registerRoleSpecificProficiencyDic: Dictionary<RoleSpecificProficiency> = null,
    registerAreaOfProfessionalInterestDic: Dictionary<AreaOfProfessionalInterest> = null
  ): LearnerProfileViewModel {
    return new LearnerProfileViewModel({
      user: user,
      registerAllMetadataDic: registerAllMetadataDic,
      registerDesginationDic: registerDesginationDic,
      registerPortfolioDic: registerPortfolioDic,
      registerTypeOfOrganizationDic: registerTypeOfOrganizationDic,
      registerRoleSpecificProficiencyDic: registerRoleSpecificProficiencyDic,
      registerAreaOfProfessionalInterestDic: registerAreaOfProfessionalInterestDic
    });
  }

  constructor(data?: ILearnerProfileViewModel) {
    if (data != null) {
      this.user = data.user;
      this._registerAllMetadataDic = data.registerAllMetadataDic;
      this._registerDesginationDic = data.registerDesginationDic;
      this._registerPortfolioDic = data.registerPortfolioDic;
      this._registerTypeOfOrganizationDic = data.registerTypeOfOrganizationDic;
      this._registerRoleSpecificProficiencyDic = data.registerRoleSpecificProficiencyDic;
      this._registerAreaOfProfessionalInterestDic = data.registerAreaOfProfessionalInterestDic;
      this.designationDisplayText = this.getDesignationDisplayText();
      this.teachingLevelDisplayTexts = this.getTeachingLevelDisplayTexts();
      this.teachingSubjectsDisplayTexts = this.getTeachingSubjectsDisplayTexts();
      this.serviceSchemeDisplayText = this.getServiceSchemeDisplayText();
      this.teachingCourseOfStudyDisplayTexts = this.getTeachingCourseOfStudyDisplayTexts();
      this.coCurricularActivitiesDisplayTexts = this.getCoCurricularActivitiesDisplayTexts();
      this.portfoliosDisplayTexts = this.getPortfoliosDisplayTexts();
      this.typeOfOrganizationDisplayText = this.getTypeOfOrganizationDisplayText();
      this.roleSpecificProficienciesDisplayTexts = this.getRoleSpecificProficienciesDisplayTexts();
      this.areasOfProfessionalInterestDisplayTexts = this.getAreasOfProfessionalInterestDisplayTexts();
    }
  }

  public getDesignationDisplayText(): string {
    if (this.user == null) {
      return '';
    }

    return this.user.getDesignationDisplayText(this._registerDesginationDic);
  }

  public getTypeOfOrganizationDisplayText(): string {
    if (this.user == null) {
      return '';
    }

    const typeOfOrganization = this._registerTypeOfOrganizationDic[this.user.typeOfOrganization];
    return typeOfOrganization ? typeOfOrganization.displayText : '';
  }

  public getTeachingLevelDisplayTexts(): string[] {
    if (this.user == null) {
      return [];
    }

    return this.user.teachingLevels
      .map(p => this._registerAllMetadataDic[p])
      .filter(p => p != null)
      .map(p => p.displayText);
  }

  public getRoleSpecificProficienciesDisplayTexts(): string[] {
    if (this.user == null) {
      return [];
    }

    return this.user.roleSpecificProficiencies
      .map(p => this._registerRoleSpecificProficiencyDic[p])
      .filter(p => p != null)
      .map(p => p.displayText);
  }

  public getAreasOfProfessionalInterestDisplayTexts(): string[] {
    if (this.user == null) {
      return [];
    }

    return this.user.areasOfProfessionalInterest
      .map(p => this._registerAreaOfProfessionalInterestDic[p])
      .filter(p => p != null)
      .map(p => p.displayText);
  }

  public getTeachingCourseOfStudyDisplayTexts(): string[] {
    if (this.user == null) {
      return [];
    }

    return this.user.teachingCourseOfStudy
      .map(p => this._registerAllMetadataDic[p])
      .filter(p => p != null)
      .map(p => p.displayText);
  }

  public getPortfoliosDisplayTexts(): string[] {
    if (this.user == null) {
      return [];
    }

    return this.user.portfolios
      .map(p => this._registerPortfolioDic[p])
      .filter(p => p != null)
      .map(p => p.displayText);
  }

  public getCoCurricularActivitiesDisplayTexts(): string[] {
    if (this.user == null) {
      return [];
    }

    return this.user.coCurricularActivities
      .map(p => this._registerAllMetadataDic[p])
      .filter(p => p != null)
      .map(p => p.displayText);
  }

  public getServiceSchemeDisplayText(): string {
    if (this.user == null) {
      return '';
    }

    const serviceScheme = this._registerAllMetadataDic[this.user.serviceScheme];
    return serviceScheme ? serviceScheme.displayText : '';
  }

  public getTeachingSubjectsDisplayTexts(): string[] {
    if (this.user == null) {
      return [];
    }
    return []
      .concat(this.user.teachingSubjects, this.user.jobFamilies)
      .map(p => this._registerAllMetadataDic[p])
      .filter(p => p != null)
      .map(p => p.displayText);
  }
}
