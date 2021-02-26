import { Dictionary, Utils } from 'app-utilities/utils';
import { MetadataTagGroupCode } from '../constant/metadata-tag-group-code.enum';

export const NOT_APPLICABLE_ITEM_DISPLAY_TEXT: string = 'Not Applicable';

export enum MetadataTagType {
  TeacherOutcome = 'Teacher Outcome',
  LearningDimension = 'Learning Dimension',
  LearningArea = 'Learning Area',
  LearningFramework = 'Learning Framework'
}

export interface IMetadataTagModel {
  tagId: string;
  parentTagId?: string;
  fullStatement: string;
  displayText: string;
  groupCode?: MetadataTagGroupCode;
  codingScheme?: string;
  note?: string;
  type?: MetadataTagType;
  childs?: IMetadataTagModel[];
}

export class MetadataTagModel implements IMetadataTagModel {
  get id(): string {
    return this.tagId;
  }

  static filterIdsInTree(
    tagIds: string[],
    tagsTree: MetadataTagModel[]
  ): string[] {
    const flatedTagsTreeDic = Utils.toDictionary(
      MetadataTagModel.flatTree(tagsTree),
      (p) => p.tagId
    );

    return tagIds.filter((p) => flatedTagsTreeDic[p] !== undefined);
  }

  static flatTree(tagsTree: MetadataTagModel[]): MetadataTagModel[] {
    let childResult: MetadataTagModel[] = [];
    // tslint:disable-next-line:prefer-for-of
    for (let i = 0; i < tagsTree.length; i++) {
      if (tagsTree[i].childs !== undefined) {
        childResult = childResult.concat(
          MetadataTagModel.flatTree(tagsTree[i].childs)
        );
      }
    }

    return tagsTree.concat(childResult);
  }

  static buildTree(
    flatTags: MetadataTagModel[],
    rootNodePredicate: (node: MetadataTagModel) => boolean,
    childNodePredicates?: Array<(node: MetadataTagModel) => boolean>,
    applyFirstChidNodePredicateForAll: boolean = false
  ): MetadataTagModel[] {
    const fullPathTagList = FullPathMetadataTagModel.buildFullPathMetadataTag(
      flatTags,
      Utils.toDictionary(flatTags, (p) => p.id)
    );

    return MetadataTagModel._buildTree(
      fullPathTagList,
      rootNodePredicate,
      childNodePredicates,
      1,
      applyFirstChidNodePredicateForAll
    );
  }

  static filterSelectedTreeNodes(
    tagsTree: MetadataTagModel[],
    selectedTagIds: string[]
  ): MetadataTagModel[] {
    return MetadataTagModel._filterSelectedTreeNodes(
      tagsTree,
      Utils.toDictionary(selectedTagIds, (p) => p)
    );
  }

  static filterTagWithChildsByTopLevelGroupCode(
    tags: MetadataTagModel[],
    groupCode: MetadataTagGroupCode,
    metadataTagsDicByParent: Dictionary<MetadataTagModel[]>
  ): MetadataTagModel[] {
    let result: MetadataTagModel[] = [];
    const topLevelTags = tags.filter((p) => p.groupCode === groupCode);
    const remainingTags = tags.filter((p) => p.groupCode !== groupCode);
    result = result.concat(topLevelTags);
    // tslint:disable-next-line:prefer-for-of
    for (let i = 0; i < topLevelTags.length; i++) {
      const topLevelTag = topLevelTags[i];
      result = result.concat(
        MetadataTagModel.getChildsTagRecursively(
          topLevelTag,
          remainingTags,
          metadataTagsDicByParent
        )
      );
    }

    return result;
  }

  static getChildsTagRecursively(
    topLevelTag: MetadataTagModel,
    allTags: MetadataTagModel[],
    metadataTagsDicByParent: Dictionary<MetadataTagModel[]>
  ): MetadataTagModel[] {
    let result: MetadataTagModel[] = [];
    const topLevelTagChilds = metadataTagsDicByParent[topLevelTag.tagId];
    if (topLevelTagChilds) {
      const topLevelTagChildsDic = Utils.toDictionary(
        topLevelTagChilds,
        (p) => p.id
      );
      const selectedChilds = allTags.filter(
        (selectedTags) => topLevelTagChildsDic[selectedTags.tagId] != null
      );
      result = result.concat(selectedChilds);
      const remainingChilds = allTags.filter(
        (p) =>
          selectedChilds.map((child) => child.tagId).indexOf(p.tagId) === -1 &&
          p.tagId !== topLevelTag.tagId
      );
      topLevelTagChilds.forEach((child) => {
        result = result.concat(
          MetadataTagModel.getChildsTagRecursively(
            child,
            remainingChilds,
            metadataTagsDicByParent
          )
        );
      });
    }

    return result;
  }

  static selectAllTreeItemIds(
    treeRootItem: MetadataTagModel,
    selectedIds: string[]
  ): string[] {
    const allTreeItemIds = MetadataTagModel.flatTree([treeRootItem]).map(
      (p) => p.id
    );

    return Utils.distinct(selectedIds.concat(allTreeItemIds));
  }

  static deselectAllTreeItemIds(
    treeRootItem: MetadataTagModel,
    selectedIds: string[]
  ): string[] {
    const allTreeItemIds = MetadataTagModel.flatTree([treeRootItem]).map(
      (p) => p.id
    );

    return Utils.removeAlls(selectedIds, allTreeItemIds);
  }

  private static _filterSelectedTreeNodes(
    tagsTree: MetadataTagModel[],
    selectedTagIdsDic: Dictionary<string>
  ): MetadataTagModel[] {
    return tagsTree
      .map((p) => {
        p = Utils.clone(p, (clonedP) => {
          clonedP.childs =
            clonedP.childs && clonedP.childs.length > 0
              ? clonedP.childs.filter((p1) => {
                  if (p1.childs == null || p1.childs.length === 0) {
                    return selectedTagIdsDic[p1.tagId] != null;
                  } else {
                    p1 = Utils.clone(p1, (clonedP1) => {
                      clonedP1.childs = MetadataTagModel._filterSelectedTreeNodes(
                        p1.childs,
                        selectedTagIdsDic
                      );
                    });

                    return (
                      selectedTagIdsDic[p1.tagId] != null ||
                      p1.childs.length > 0
                    );
                  }
                })
              : clonedP.childs;
        });

        return p;
      })
      .filter(
        (p) =>
          selectedTagIdsDic[p.tagId] != null ||
          (p.childs != null && p.childs.length > 0)
      );
  }

  private static _buildTree(
    flatTags: FullPathMetadataTagModel[],
    rootNodePredicate: (node: MetadataTagModel) => boolean | null,
    childNodePredicates: Array<(node: MetadataTagModel) => boolean>,
    currentLevel: number,
    applyFirstChidNodePredicateForAll: boolean
  ): MetadataTagModel[] {
    let rootNodes = flatTags;
    if (rootNodePredicate != null) {
      rootNodes = flatTags.filter((p) => rootNodePredicate(p));
    }
    if (rootNodes.length === 0) {
      return rootNodes;
    }

    const childNodePredicate =
      childNodePredicates != null
        ? applyFirstChidNodePredicateForAll === true
          ? childNodePredicates[0]
          : childNodePredicates[currentLevel - 1]
        : null;

    // Build rootNodesAllLeafChildsDic
    const rootNodesAllLeafChildsDic: Dictionary<
      FullPathMetadataTagModel[]
    > = Utils.toDictionarySelect(
      rootNodes,
      (p) => p.id,
      (p) => [] as FullPathMetadataTagModel[]
    );
    flatTags.forEach((currentItem) => {
      const currentAncestorNodes = currentItem.getAncestorsNodes();
      for (let i = currentLevel - 1; i < currentAncestorNodes.length; i++) {
        const currentAncestorNode = currentAncestorNodes[i];
        if (
          currentAncestorNode != null &&
          rootNodesAllLeafChildsDic[currentAncestorNode.id] != null
        ) {
          rootNodesAllLeafChildsDic[currentAncestorNode.id].push(currentItem);

          return;
        }
      }
    });

    for (let i = 0; i < rootNodes.length; i++) {
      rootNodes[i] = Utils.clone(rootNodes[i], (clonedRootNode) => {
        clonedRootNode.level = currentLevel;
        clonedRootNode.childs = MetadataTagModel._buildTree(
          rootNodesAllLeafChildsDic[clonedRootNode.id],
          (p) =>
            p.parentTagId === clonedRootNode.tagId &&
            (childNodePredicate == null || childNodePredicate(p)),
          childNodePredicates,
          currentLevel + 1,
          applyFirstChidNodePredicateForAll
        );
      });
    }

    return rootNodes;
  }

  tagId: string;
  parentTagId?: string;
  fullStatement: string;
  displayText: string;
  groupCode?: MetadataTagGroupCode;
  codingScheme?: string;
  note?: string;
  type?: MetadataTagType;
  childs?: MetadataTagModel[];
  level?: number;

  constructor(data?: IMetadataTagModel) {
    if (data != null) {
      this.tagId = data.tagId != null ? data.tagId.toLowerCase() : null;
      this.parentTagId =
        data.parentTagId != null ? data.parentTagId.toLowerCase() : null;
      this.fullStatement = data.fullStatement;
      this.displayText = data.displayText;
      this.groupCode = data.groupCode;
      this.codingScheme = data.codingScheme;
      this.note = data.note;
      this.type = data.type;
      this.childs =
        data.childs !== undefined
          ? data.childs.map((p) => new MetadataTagModel(p))
          : undefined;
    }
  }
}

export interface IFullPathMetadataTagModel extends IMetadataTagModel {
  parent?: IFullPathMetadataTagModel;
}

// tslint:disable-next-line:max-classes-per-file
export class FullPathMetadataTagModel
  extends MetadataTagModel
  implements IFullPathMetadataTagModel {
  static buildFullPathMetadataTag(
    items: MetadataTagModel[],
    allMetadataTagsDic: Dictionary<MetadataTagModel>
  ): FullPathMetadataTagModel[] {
    return items.map((p) => {
      return FullPathMetadataTagModel.create(p, allMetadataTagsDic);
    });
  }

  static create(
    leafNode: MetadataTagModel,
    allMetadataTagsDic: Dictionary<MetadataTagModel>
  ): FullPathMetadataTagModel {
    if (
      leafNode.parentTagId == null ||
      allMetadataTagsDic[leafNode.parentTagId] == null
    ) {
      return new FullPathMetadataTagModel(leafNode);
    }

    const parentLeafNode = allMetadataTagsDic[leafNode.parentTagId];
    const parent = FullPathMetadataTagModel.create(
      parentLeafNode,
      allMetadataTagsDic
    );

    return new FullPathMetadataTagModel(leafNode, parent);
  }

  parent?: FullPathMetadataTagModel;
  _fullPathName: string;
  get fullPathName(): string {
    if (this._fullPathName == null) {
      this._fullPathName = this.getFullPathName();
    }

    return this._fullPathName;
  }

  constructor(data?: IMetadataTagModel, parent?: IFullPathMetadataTagModel) {
    super(data);
    this.parent =
      parent != null
        ? new FullPathMetadataTagModel(parent, parent.parent)
        : null;
  }

  getFullPathName(): string {
    const nodeNameList: string[] = [this.displayText];

    let currentParentNode = this.parent;
    while (currentParentNode != null) {
      nodeNameList.push(currentParentNode.displayText);
      currentParentNode = currentParentNode.parent;
    }

    return nodeNameList.join(' - ');
  }

  isLeafChildOf(parentTagId: string): boolean {
    const currentParentNode = this.parent;
    while (currentParentNode != null) {
      if (this.parent.id === parentTagId) {
        return true;
      }
    }

    return false;
  }

  getRootNode(fromLevel: number = 0): FullPathMetadataTagModel {
    const reversedParentNodeList: FullPathMetadataTagModel[] = [];

    let currentParentNode = this.parent;
    while (currentParentNode != null) {
      reversedParentNodeList.push(currentParentNode);
      currentParentNode = currentParentNode.parent;
    }

    const parentNodeList = reversedParentNodeList.reverse();

    return parentNodeList[fromLevel];
  }

  getAncestorsNodes(): FullPathMetadataTagModel[] {
    const parentNodeList: FullPathMetadataTagModel[] = [];

    let currentParentNode = this.parent;
    while (currentParentNode != null) {
      parentNodeList.push(currentParentNode);
      currentParentNode = currentParentNode.parent;
    }

    return parentNodeList;
  }
}
