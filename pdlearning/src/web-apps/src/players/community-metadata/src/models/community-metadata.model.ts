import { MetadataCodingScheme, MetadataId, MetadataTagModel, PlaceOfWorkType, ResourceModel } from '@opal20/domain-api';

export class CommunityMetadataViewModel {
  //#region Metadata properties
  public pdActivityType: string;
  public categoryIds: string[] = [];
  public ownerDivisionIds: number[] = [];
  public ownerBranchIds: number[] = [];
  public get isMicrolearning(): boolean {
    return this.pdActivityType && this.pdActivityType === MetadataId.Microlearning;
  }

  public moeOfficerId: string;
  public moeOfficerEmail: string;

  public serviceSchemeIds: string[] = [];

  public subjectAreaIds: string[] = [];
  public learningFrameworkIds: string[] = [];
  public learningDimensionIds: string[] = [];
  public learningAreaIds: string[] = [];
  public learningSubAreaIds: string[] = [];

  public placeOfWork: PlaceOfWorkType;

  public applicableDivisionIds: number[] = [];
  public applicableBranchIds: number[] = [];
  public applicableZoneIds: number[] = [];
  public applicableClusterIds: number[] = [];
  public applicableSchoolIds: number[] = [];

  public trackIds: string[] = [];
  public developmentalRoleIds: string[] = [];
  public teachingLevels: string[] = [];
  public teachingCourseStudyIds: string[] = [];
  public learningMode: string;
  public moderatorId: string;
  public coModeratorId: string;
  public currentUserId: string;
  public searchTags: string[] = [];

  //#endregion Metadata properties
  constructor(data?: ResourceModel) {
    if (data && data.dynamicMetaData) {
      for (const key in data.dynamicMetaData) {
        if (data.dynamicMetaData[key]) {
          this[key] = data.dynamicMetaData[key];
        }
      }
      this.searchTags = data.searchTags;
    }
  }

  public setPlaceOfWork(value: PlaceOfWorkType): void {
    this.placeOfWork = value;

    if (this.placeOfWork === PlaceOfWorkType.ApplicableForEveryone) {
      this.applicableDivisionIds = null;
      this.applicableBranchIds = null;
      this.applicableZoneIds = null;
      this.applicableClusterIds = null;
      this.applicableSchoolIds = null;
    }
  }

  public hasOnlyOneServiceSchemesChecked(): boolean {
    return this.serviceSchemeIds && this.serviceSchemeIds.length === 1;
  }

  public serviceSchemesContains(codingScheme: MetadataCodingScheme, metadataTagsDic: Dictionary<MetadataTagModel>): boolean {
    return (
      this.serviceSchemeIds &&
      this.serviceSchemeIds.find(serviceId => metadataTagsDic[serviceId] && metadataTagsDic[serviceId].codingScheme === codingScheme) !=
        null
    );
  }
}
