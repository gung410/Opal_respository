export interface IMetadataTypeNode {
  nodeId: number;
  metadataTypeId: string;
  code: string;
  name: string;
  parentNodeId?: number;
  children?: IMetadataTypeNode[];
}

export class MetadataTypeNode implements IMetadataTypeNode {
  nodeId: number;
  metadataTypeId: string;
  code: string;
  name: string;
  parentNodeId?: number;
  children?: IMetadataTypeNode[];

  constructor(data?: IMetadataTypeNode) {
    if (data == null) {
      return;
    }

    this.nodeId = data.nodeId;
    this.metadataTypeId = data.metadataTypeId;
    this.code = data.code;
    this.name = data.name;
    this.parentNodeId = data.parentNodeId;
    this.children = data.children;
  }
}
