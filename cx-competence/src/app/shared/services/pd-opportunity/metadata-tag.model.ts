export interface MetadataTagGetDTO {
  id: number;
  parentTagId?: string;
  tagId: string;
  fullStatement: string;
  displayText: string;
  groupCode: string;
  codingScheme: string;
  childs?: MetadataTagGetDTO[];
}

export class MetadataTagModel {
  id: string;
  parentTagId?: string;
  tagId: string;
  fullStatement: string;
  name: string;
  groupCode: string;
  codingScheme: string;
  childs?: MetadataTagModel[];
  constructor(metadataTagGetDTO: MetadataTagGetDTO) {
    this.id = metadataTagGetDTO.tagId;
    this.parentTagId =
      metadataTagGetDTO.parentTagId != null
        ? metadataTagGetDTO.parentTagId.toLowerCase()
        : null;
    this.tagId =
      metadataTagGetDTO.tagId != null
        ? metadataTagGetDTO.tagId.toLowerCase()
        : null;
    this.fullStatement = metadataTagGetDTO.fullStatement;
    this.name = metadataTagGetDTO.displayText;
    this.groupCode = metadataTagGetDTO.groupCode;
    this.codingScheme = metadataTagGetDTO.codingScheme;
    this.childs =
      metadataTagGetDTO.childs !== undefined
        ? metadataTagGetDTO.childs.map((p) => new MetadataTagModel(p))
        : undefined;
  }
}
