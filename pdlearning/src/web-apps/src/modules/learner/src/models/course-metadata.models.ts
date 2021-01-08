import { GetResourceWithMetadataResult, MetadataTagModel } from '@opal20/domain-api';

import { ResourceMetadataFormModel } from '@opal20/domain-components';
import { Utils } from '@opal20/infrastructure';

export class CourseMetadataModel {
  public mainSubjectArea: string | undefined;
  public serviceSchemes: string[] = [];
  public developmentalRoles: string[] = [];
  public courseLevels: string[] = [];
  public preRequisites: string | undefined;
  public learningFrameworks: string[] = [];
  public objectiveOutcomes: string | undefined;
  public subjectAreasAndKeywordsMetadata: MetadataTagModel[] = [];
  public dimensionsAndAreasTree: MetadataTagModel[] = [];

  constructor(data?: GetResourceWithMetadataResult) {
    if (data == null) {
      return;
    }
    const resourceForm = new ResourceMetadataFormModel(data.resource, data.metadataTags);
    this.mainSubjectArea = !Utils.isEmpty(resourceForm.mainSubjectArea)
      ? resourceForm.metadataTagsDic[resourceForm.mainSubjectArea].displayText
      : undefined;
    this.serviceSchemes = resourceForm.serviceSchemes.map(p => resourceForm.metadataTagsDic[p].displayText);
    this.developmentalRoles = resourceForm.developmentalRoles.map(p => resourceForm.metadataTagsDic[p].displayText);
    this.courseLevels = resourceForm.courseLevels.map(p => resourceForm.metadataTagsDic[p].displayText);
    this.preRequisites = resourceForm.resource.preRequisties;
    this.learningFrameworks = resourceForm.learningFrameworks.map(p => resourceForm.metadataTagsDic[p].displayText);
    this.objectiveOutcomes = resourceForm.resource.objectivesOutCome;
    this.subjectAreasAndKeywordsMetadata = resourceForm.subjectAreasAndKeywordsTree;
    this.dimensionsAndAreasTree = resourceForm.dimensionsAndAreasTree;
  }
}
