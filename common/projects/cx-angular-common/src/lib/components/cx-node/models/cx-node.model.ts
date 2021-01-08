export class CxNode {
    id?: string;
    name: string;
    iconClass?: string;
    subName?: string;
    dataObject: any;
    children?: CxNode[];
    parent?: CxNode;
    nodeName?: string;
    status?: NodeStatus;
    canCreateChildren?: boolean;
    createChildrenIcon?: string;
    addNodeText?: string;
    hideChildren = false;
    idFieldPath?: string;

    constructor(name) {
        this.name = name;
    }
}

export class NodeStatus {
    shortName: string;
    type: NodeStatusType;
    text: string;
}

export enum NodeStatusType {
    Draft = 'draft',
    Pending = 'pending',
    Approved = 'approved',
    Rejected = 'rejected'
}
