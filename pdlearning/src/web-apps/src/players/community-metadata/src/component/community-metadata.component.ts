import { AuthService, OAuthService, UserInfo } from '@opal20/authentication';
import { BaseFormComponent, FragmentRegistry, IFormBuilderDefinition, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import {
  BaseUserInfo,
  CommunityTaggingApiService,
  DepartmentInfoModel,
  IQuerySearchTagRequest,
  ISaveResourceMetadataRequest,
  MetadataCodingScheme,
  MetadataId,
  OrganizationRepository,
  OrganizationUnitLevelEnum,
  PlaceOfWorkType,
  SearchTag,
  SystemRoleEnum,
  TaggingApiService,
  UserRepository,
  UserUtils
} from '@opal20/domain-api';
import { Component, HostListener } from '@angular/core';
import { requiredForListValidator, requiredIfValidator } from '@opal20/common-components';

import { CommunityInfo } from '../models/community-info.model';
import { CommunityIntergrationService } from '../services/community-intergration.service';
import { CommunityMetadataBuilderService } from '../services/communitty-metadata-builder.service';
import { CommunityMetadataListingModel } from '../models/community-metadata-listing.model';
import { CommunityMetadataViewModel } from '../models/community-metadata.model';
import { CourseDetailViewModel } from '@opal20/domain-components';
import { Observable } from 'rxjs';
import { Validators } from '@angular/forms';
import { map } from 'rxjs/operators';

@Component({
  selector: 'community-metadata',
  templateUrl: './community-metadata.component.html',
  styleUrls: ['./community-metadata.component.scss']
})
export class CommunityMetadataComponent extends BaseFormComponent {
  public communityMetadataModel: CommunityMetadataViewModel = new CommunityMetadataViewModel();
  public viewOnly: boolean = false;

  public metadataList: CommunityMetadataListingModel = new CommunityMetadataListingModel();
  public communityInfo: CommunityInfo = new CommunityInfo();

  public fetchOwnerDivisionFn = this.createFetchDivisionFn();
  public fetchBranchFn = this.createFetchBranchFn();
  public fetchZoneFn = this.createFetchZoneFn();
  public fetchClusterFn = this.createClusterFn();
  public fetchSchoolFn = this.createSchoolFn();
  public fetchDepartmentByIdsFn = this.createFetchDepartmentByIdsFn();
  public ignoreModeratorItemFn: (item: BaseUserInfo) => boolean;
  public ignoreCoModeratorItemFn: (item: BaseUserInfo) => boolean;
  public fetchUserItemsByIdsFn = this._createFetchUserItemsByIdsFn();
  public fetchModeratorItemsFn = this._createFetchUserSelectItemFn(CourseDetailViewModel.courseFacilitatorItemsRoles);
  public fetchCoModeratorItemsFn = this._createFetchUserSelectItemFn(CourseDetailViewModel.courseFacilitatorItemsRoles);
  public fetchMoeOfficerItemsFn = this._createFetchUserSelectItemFn();

  public placeOfWorkTitleDic = {
    [PlaceOfWorkType.ApplicableForEveryone]: 'Applicable for everyone',
    [PlaceOfWorkType.ApplicableForUsersInSpecificOrganisation]: 'Applicable for users in specific organisation(s)'
  };
  public PlaceOfWorkType: typeof PlaceOfWorkType = PlaceOfWorkType;
  public MetadataCodingScheme: typeof MetadataCodingScheme = MetadataCodingScheme;
  public customSearchTagAddedFn: (searchTag: string) => Promise<SearchTag>;
  public fetchSearchTagsFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<SearchTag[]>;
  public fetchSearchTagsByNamesFn: (names: string[]) => Observable<SearchTag[]>;

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public fragmentRegistry: FragmentRegistry,
    private organizationRepository: OrganizationRepository,
    protected communityIntergrationSrv: CommunityIntergrationService,
    protected metadataBuilderSrv: CommunityMetadataBuilderService,
    private taggingApiService: CommunityTaggingApiService,
    private authService: AuthService,
    private userRepository: UserRepository,
    private oAuthService: OAuthService,
    private resourceApiService: TaggingApiService
  ) {
    super(moduleFacadeService);

    // we are not using oidc but just oauth2 password flow so we bypass validations for oidc
    this.oAuthService.skipSubjectCheck = true;
    Promise.resolve()
      .then(() => this.communityIntergrationSrv.verifyIntergrationUrl())
      .then(() => this.oAuthService.loadDiscoveryDocument())
      .then(() => this.oAuthService.loadUserProfile())
      .then(user => {
        const userInfo = user as UserInfo;
        this.communityInfo = this.communityIntergrationSrv.getCommunityInfo();
        this.initMetadataModel(userInfo.sub);
        this.metadataList = new CommunityMetadataListingModel();
        this.setupGlobalIntergrations();
        this.ignoreModeratorItemFn = x =>
          x.id === this.communityMetadataModel.moderatorId || x.id === this.communityMetadataModel.currentUserId;
        this.ignoreCoModeratorItemFn = x =>
          x.id === this.communityMetadataModel.coModeratorId || x.id === this.communityMetadataModel.currentUserId;
      });

    this.customSearchTagAddedFn = (searchTag: string) => {
      this.communityMetadataModel.searchTags.push(searchTag);
      return new Promise(resolve => {
        resolve(
          new SearchTag({
            name: searchTag
          })
        );
      });
    };
    this.fetchSearchTagsByNamesFn = (searchTags: string[]) => {
      return this.resourceApiService.getSearchTagByNames(searchTags);
    };
    this.fetchSearchTagsFn = (searchText: string, skipCount: number, maxResultCount: number) => {
      const request: IQuerySearchTagRequest = {
        searchText: searchText,
        pagedInfo: {
          skipCount: skipCount,
          maxResultCount: maxResultCount
        }
      };
      return this.resourceApiService.querySearchTag(request).pipe(map(response => response.items));
    };
  }

  public get isCourseAssociated(): boolean {
    return !!this.communityInfo.courseId;
  }
  //#region init data

  public async initMetadataModel(currentUserId: string): Promise<void> {
    try {
      const metadata = await this.taggingApiService.getCommunityMetaData(this.communityInfo.communityId).toPromise();
      if (metadata && metadata.resource.dynamicMetaData) {
        this.communityMetadataModel = new CommunityMetadataViewModel(metadata.resource);
      }
      this.communityMetadataModel.currentUserId = currentUserId;
      this.subscribeMetadataListing();
    } catch (error) {
      this.raiseError('metadataLoad', error.status, error.error);
    }
  }

  public async subscribeMetadataListing(): Promise<void> {
    const subject = await this.metadataBuilderSrv.buildMetadataListingSubject(this.communityMetadataModel);

    this.subscribe(subject, metadataListingModel => {
      this.metadataList = metadataListingModel;
    });
  }

  //#endregion init data

  //#region validate and save

  public async saveMetadata(): Promise<void> {
    const dynamicMetaData: Dictionary<unknown> = (Utils.cloneDeep(this.communityMetadataModel) as unknown) as Dictionary<unknown>;

    // Remove all null or undefined fields
    Object.keys(dynamicMetaData).map(key => {
      if (dynamicMetaData[key] === null || dynamicMetaData[key] === undefined || key === 'searchTags') {
        delete dynamicMetaData[key];
      }
    });

    const request: ISaveResourceMetadataRequest = {
      tagIds: this.getAllMetadataTagIds(),
      searchTags: this.communityMetadataModel.searchTags,
      dynamicMetaData
    };
    return this.taggingApiService.saveCommunityMetadata(this.communityInfo.communityId, request, true).toPromise();
  }

  public getAllMetadataTagIds(): string[] {
    return []
      .concat(this.communityMetadataModel.pdActivityType ? [this.communityMetadataModel.pdActivityType] : [])
      .concat(this.communityMetadataModel.serviceSchemeIds)
      .concat(this.communityMetadataModel.subjectAreaIds)
      .concat(this.communityMetadataModel.learningFrameworkIds)
      .concat(this.communityMetadataModel.learningDimensionIds)
      .concat(this.communityMetadataModel.learningAreaIds)
      .concat(this.communityMetadataModel.learningSubAreaIds)
      .concat(this.communityMetadataModel.trackIds)
      .concat(this.communityMetadataModel.categoryIds)
      .concat(this.communityMetadataModel.teachingCourseStudyIds)
      .concat(this.communityMetadataModel.teachingLevels)
      .concat(this.communityMetadataModel.developmentalRoleIds)
      .concat(this.communityMetadataModel.learningMode ? [this.communityMetadataModel.learningMode] : []);
  }

  public onSaveButtonClick(): void {
    this.validateCommunityMetadata()
      .then((isValid: boolean) => {
        if (isValid) {
          return this.saveMetadata();
        }

        return Promise.resolve();
      })
      .catch(error => {
        this.raiseError('metadataUpdate', error.status, error.error);
      });
  }

  //#endregion validate and save

  //#region intergration listerners

  public setupGlobalIntergrations(): void {
    AppGlobal.communityMetadataIntergrations.validateMetadataForm = () => {
      return this.validateCommunityMetadata();
    };
    AppGlobal.communityMetadataIntergrations.saveMetadataForm = () => {
      this.saveCommunityMetadata();
    };
    AppGlobal.communityMetadataIntergrations.submitMetadataForm = () => {
      this.saveCommunityMetadata();
    };
    AppGlobal.communityMetadataIntergrations.refreshAccessToken = (accessToken: string) => {
      this.authService.setAccessToken(accessToken);
    };
    AppGlobal.communityMetadataIntergrations.setViewMode = (viewOnly: boolean) => (this.viewOnly = viewOnly);
  }

  @HostListener('window:submitCommunityMetadata', ['$event'])
  public async submitMetadata(event?: unknown): Promise<void> {
    const isValid = await this.validate();
    if (isValid) {
      this.saveMetadata();
    }
  }

  @HostListener('window:saveCommunityMetadata', ['$event'])
  public saveCommunityMetadata(event?: CustomEvent): void {
    this.saveMetadata();
  }

  @HostListener('window:validateCommunityMetadata', ['$event'])
  public async validateCommunityMetadata(event?: CustomEvent): Promise<boolean> {
    const isValid = await this.validate();
    const dispatchEvent = new CustomEvent('validateCommunityMetadataResult', { detail: { isValid } });
    try {
      if (window.parent) {
        window.dispatchEvent(dispatchEvent);
        if (window.parent) {
          window.parent.dispatchEvent(dispatchEvent);
        }
      }
    } catch (e) {
      console.log(e);
      // Ignore exception
    } finally {
      return isValid;
    }
  }

  @HostListener('window:refreshToken', ['$event'])
  public refreshToken(event: CustomEvent): void {
    if (event && event.detail && event.detail.accessToken) {
      this.authService.setAccessToken(event.detail.accessToken);
    }
  }
  //#endregion intergration listerners

  //#region model change callback
  public onSchemaChange(serviceSchemeIds: string[]): void {
    this.metadataBuilderSrv.buildSubjectSelectItems(serviceSchemeIds);
    this.metadataBuilderSrv.buildLearningFrameworksSelectItems(serviceSchemeIds);
    this.metadataBuilderSrv.buildDevelopmentalRoleSelectItems(serviceSchemeIds);

    this.communityMetadataModel.subjectAreaIds = [];
    this.communityMetadataModel.learningFrameworkIds = [];
    this.communityMetadataModel.developmentalRoleIds = [];
    this.onLearningFrameworkChange(this.communityMetadataModel.learningFrameworkIds);
  }

  public onLearningFrameworkChange(learningFrameworkIds: string[]): void {
    this.metadataBuilderSrv.buildLearningDimensionSelectItems(learningFrameworkIds);

    this.communityMetadataModel.learningDimensionIds = [];
    this.onLearningDimensionChange(this.communityMetadataModel.learningDimensionIds);
  }

  public onLearningDimensionChange(learningDimensionIds: string[]): void {
    this.metadataBuilderSrv.buildLearningAreaSelectItems(learningDimensionIds);

    this.communityMetadataModel.learningAreaIds = [];
    this.onLearningAreaChange(this.communityMetadataModel.learningAreaIds);
  }

  public onLearningAreaChange(learningAreaIds: string[]): void {
    this.metadataBuilderSrv.buildLearningSubAreaSelectItems(learningAreaIds);

    this.communityMetadataModel.learningSubAreaIds = [];
  }

  public onMOEOfficerChange(moeOfficerId: string): void {
    const officerInfo = this.metadataList.moeOfficerDic[moeOfficerId];
    if (officerInfo) {
      this.communityMetadataModel.moeOfficerEmail = officerInfo.emailAddress;
      this.communityMetadataModel.ownerDivisionIds = [];
      this.communityMetadataModel.ownerBranchIds = [];
      this.setOwnerDepartmentRecursive(officerInfo.departmentId);
    }
  }

  public setOwnerDepartmentRecursive(departmentId: number): void {
    const department = this.metadataList.departmentsDic[departmentId];
    if (department) {
      if (department.departmentType === OrganizationUnitLevelEnum.Division) {
        this.communityMetadataModel.ownerDivisionIds = [department.id];
        return;
      } else if (department.departmentType === OrganizationUnitLevelEnum.Branch) {
        this.communityMetadataModel.ownerBranchIds = [department.id];
      }
      this.setOwnerDepartmentRecursive(department.parentDepartmentId);
    }
  }

  public onPdActivityTypeChange(id: string): void {
    if (id === MetadataId.Microlearning) {
      this.communityMetadataModel.learningMode = MetadataId.ELearning;
    }
  }
  //#endregion model change callback

  public serviceSchemesContains(codingScheme: MetadataCodingScheme): boolean {
    return (
      this.communityMetadataModel.serviceSchemeIds &&
      this.communityMetadataModel.serviceSchemeIds.find(
        serviceId =>
          this.metadataList.metadataTagsDic[serviceId] && this.metadataList.metadataTagsDic[serviceId].codingScheme === codingScheme
      ) != null
    );
  }

  //#region create fetch function
  protected createFetchDivisionFn(): (searchText: string, skipCount: number, maxResultCount: number) => Observable<DepartmentInfoModel[]> {
    return (searchText, skipCount, maxResultCount) =>
      this.organizationRepository.loadOrganizationalUnits(
        searchText,
        [OrganizationUnitLevelEnum.Division],
        null,
        true,
        null,
        null,
        false,
        maxResultCount === 0 ? 1 : skipCount / maxResultCount + 1,
        maxResultCount,
        false
      );
  }

  protected createFetchBranchFn(): (searchText: string, skipCount: number, maxResultCount: number) => Observable<DepartmentInfoModel[]> {
    return (searchText, skipCount, maxResultCount) =>
      this.organizationRepository.loadOrganizationalUnits(
        searchText,
        [OrganizationUnitLevelEnum.Branch],
        null,
        true,
        null,
        null,
        false,
        maxResultCount === 0 ? 1 : skipCount / maxResultCount + 1,
        maxResultCount,
        false
      );
  }

  protected createFetchZoneFn(): (searchText: string, skipCount: number, maxResultCount: number) => Observable<DepartmentInfoModel[]> {
    return (searchText, skipCount, maxResultCount) =>
      this.organizationRepository.loadOrganizationalUnits(
        searchText,
        [OrganizationUnitLevelEnum.Branch],
        null,
        true,
        null,
        '10000666',
        false,
        maxResultCount === 0 ? 1 : skipCount / maxResultCount + 1,
        maxResultCount,
        false
      );
  }

  protected createClusterFn(): (searchText: string, skipCount: number, maxResultCount: number) => Observable<DepartmentInfoModel[]> {
    return (searchText, skipCount, maxResultCount) =>
      this.organizationRepository.loadOrganizationalUnits(
        searchText,
        [OrganizationUnitLevelEnum.Cluster],
        null,
        true,
        null,
        null,
        false,
        maxResultCount === 0 ? 1 : skipCount / maxResultCount + 1,
        maxResultCount,
        false
      );
  }

  protected createSchoolFn(): (searchText: string, skipCount: number, maxResultCount: number) => Observable<DepartmentInfoModel[]> {
    return (searchText, skipCount, maxResultCount) =>
      this.organizationRepository.loadOrganizationalUnits(
        searchText,
        [OrganizationUnitLevelEnum.School],
        null,
        true,
        null,
        null,
        false,
        maxResultCount === 0 ? 1 : skipCount / maxResultCount + 1,
        maxResultCount,
        false
      );
  }

  protected createFetchDepartmentByIdsFn(): (ids: number[]) => Observable<DepartmentInfoModel[]> {
    return ids => this.organizationRepository.loadOrganizationalUnitsByIds(ids, false, false);
  }

  //#endregion create fetch function

  //#region define FormBuilder
  protected createFormBuilderDefinition(): IFormBuilderDefinition | undefined {
    return {
      formName: 'form',
      controls: {
        pdActivityType: {
          defaultValue: this.communityMetadataModel.pdActivityType,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required'
            }
          ]
        },
        categoryIds: {
          defaultValue: undefined,
          validators: [
            {
              validator: requiredForListValidator(),
              validatorType: 'required'
            }
          ]
        },
        ownerDivisionIds: {
          defaultValue: undefined,
          validators: null
        },
        ownerBranchIds: {
          defaultValue: undefined,
          validators: null
        },

        moeOfficerId: {
          defaultValue: undefined,
          validators: [
            {
              validator: requiredIfValidator(p => !this.communityMetadataModel.isMicrolearning),
              validatorType: 'required'
            }
          ]
        },
        moeOfficerEmail: {
          defaultValue: undefined,
          validators: [
            {
              validator: requiredIfValidator(p => !this.communityMetadataModel.isMicrolearning),
              validatorType: 'required'
            }
          ]
        },
        serviceSchemeIds: {
          defaultValue: undefined,
          validators: [
            {
              validator: requiredForListValidator(),
              validatorType: 'required'
            }
          ]
        },
        subjectAreaIds: {
          defaultValue: undefined,
          validators: [
            {
              validator: requiredForListValidator(),
              validatorType: 'required'
            }
          ]
        },
        learningFrameworkIds: {
          defaultValue: undefined,
          validators: [
            {
              validator: requiredForListValidator(),
              validatorType: 'required'
            }
          ]
        },
        learningDimensionIds: {
          defaultValue: undefined,
          validators: [
            {
              validator: requiredForListValidator(
                () => this.metadataList.learningDimensionSelectItems && this.metadataList.learningDimensionSelectItems.length > 0
              ),
              validatorType: 'required'
            }
          ]
        },
        learningAreaIds: {
          defaultValue: undefined,
          validators: [
            {
              validator: requiredForListValidator(
                () => this.metadataList.learningAreaSelectItems && this.metadataList.learningAreaSelectItems.length > 0
              ),
              validatorType: 'required'
            }
          ]
        },
        learningSubAreaIds: {
          defaultValue: undefined,
          validators: null
        },
        placeOfWork: {
          defaultValue: undefined,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required'
            }
          ]
        },
        applicableDivisionIds: {
          defaultValue: undefined,
          validators: null
        },
        applicableBranchIds: {
          defaultValue: undefined,
          validators: null
        },
        applicableZoneIds: {
          defaultValue: undefined,
          validators: null
        },
        applicableClusterIds: {
          defaultValue: undefined,
          validators: null
        },
        applicableSchoolIds: {
          defaultValue: undefined,
          validators: null
        },
        trackIds: {
          defaultValue: undefined,
          validators: [
            {
              validator: requiredForListValidator(),
              validatorType: 'required'
            }
          ]
        },
        developmentalRoleIds: {
          defaultValue: undefined,
          validators: [
            {
              validator: requiredForListValidator(
                () => this.metadataList.developmentalRoleSelectItems && this.metadataList.developmentalRoleSelectItems.length > 0
              ),
              validatorType: 'required'
            }
          ]
        },
        teachingLevels: {
          defaultValue: undefined,
          validators: [
            {
              validator: requiredForListValidator(
                () =>
                  this.metadataList.teachingLevelsSelectItems &&
                  this.metadataList.teachingLevelsSelectItems.length > 0 &&
                  (!this.communityMetadataModel.hasOnlyOneServiceSchemesChecked() ||
                    !this.communityMetadataModel.serviceSchemesContains(
                      MetadataCodingScheme.ExecutiveAndAdministrativeStaff,
                      this.metadataList.metadataTagsDic
                    ))
              ),
              validatorType: 'required'
            }
          ]
        },
        teachingCourseStudyIds: {
          defaultValue: undefined,
          validators: [
            {
              validator: requiredForListValidator(
                () =>
                  this.metadataList.teachingCourseStudySelectItems &&
                  this.metadataList.teachingCourseStudySelectItems.length > 0 &&
                  (!this.communityMetadataModel.hasOnlyOneServiceSchemesChecked() ||
                    !this.communityMetadataModel.serviceSchemesContains(
                      MetadataCodingScheme.ExecutiveAndAdministrativeStaff,
                      this.metadataList.metadataTagsDic
                    ))
              ),
              validatorType: 'required'
            }
          ]
        },
        learningMode: {
          defaultValue: this.communityMetadataModel.learningMode,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required'
            }
          ]
        },
        moderatorId: {
          defaultValue: this.communityMetadataModel.moderatorId,
          validators: [
            {
              validator: requiredIfValidator(p => !this.communityMetadataModel.isMicrolearning),
              validatorType: 'required'
            }
          ]
        },
        coModeratorId: {
          defaultValue: null,
          validators: null
        },
        searchTags: {
          defaultValue: this.communityMetadataModel.searchTags,
          validators: null
        }
      }
    };
  }
  //#endregion define FormBuilder

  private raiseError(key: 'metadataLoad' | 'metadataUpdate', statusCode: number, error: { error: string }): void {
    const msg = JSON.stringify({
      key,
      statusCode,
      error
    });
    window.parent.postMessage(msg, '*');
  }

  private _createFetchUserSelectItemFn(
    inRoles?: SystemRoleEnum[],
    mapFn?: (data: BaseUserInfo[]) => BaseUserInfo[]
  ): (searchText: string, skipCount: number, maxResultCount: number) => Observable<BaseUserInfo[]> {
    const createFetchUsersFn = UserUtils.createFetchUsersFn(inRoles, this.userRepository);
    return (searchText: string, skipCount: number, maxResultCount: number) =>
      createFetchUsersFn(searchText, skipCount, maxResultCount).pipe(
        map(users => {
          if (mapFn) {
            return mapFn(users);
          }
          return users;
        })
      );
  }

  private _createFetchUserItemsByIdsFn(): (ids: string[]) => Observable<BaseUserInfo[]> {
    return UserUtils.createFetchUsersByIdsFn(this.userRepository, false, ['All']);
  }
}
