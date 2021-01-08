export interface MetadataObject {
  id: string;
  fullStatement: string;
  type: string;
  displayText: string;
  codingScheme: string;
  typeId: string;
}

export interface LearningAreaFormSelectorResultModel {
  allLearningFrameWork: MetadataObject;
  learningFrameWorkByServiceSchemes: MetadataObject;
  learningDimensionByServiceSchemes: MetadataObject;
  learningDimensionByAllLearningFramework: MetadataObject;
  listLearningArea: MetadataObject[];
}

export class KLPPlannedAreaModel {
  framework: MetadataObject;
  dimension: MetadataObject;
  area: MetadataObject;
  constructor(
    framework: MetadataObject,
    dimension: MetadataObject,
    area: MetadataObject
  ) {
    this.framework = framework;
    this.dimension = dimension;
    this.area = area;
  }
}
