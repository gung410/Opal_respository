import { MetadataTagGroupCode } from './metadata-tag-group-code.enum';
import { Utils } from '@opal20/infrastructure';

export const NOT_APPLICABLE_ITEM_DISPLAY_TEXT: string = 'Not Applicable';

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
  public tagId: string;
  public parentTagId?: string;
  public fullStatement: string;
  public displayText: string;
  public groupCode?: MetadataTagGroupCode;
  public codingScheme?: string;
  public note?: string;
  public type?: MetadataTagType;
  public childs?: MetadataTagModel[];
  public level?: number;
  public get id(): string {
    return this.tagId;
  }

  public static filterIdsInTree(tagIds: string[], tagsTree: MetadataTagModel[]): string[] {
    const flatedTagsTreeDic = Utils.toDictionary(MetadataTagModel.flatTree(tagsTree), p => p.tagId);
    return tagIds.filter(p => flatedTagsTreeDic[p] !== undefined);
  }

  public static flatTree(tagsTree: MetadataTagModel[]): MetadataTagModel[] {
    let childResult: MetadataTagModel[] = [];
    for (let i = 0; i < tagsTree.length; i++) {
      if (tagsTree[i].childs !== undefined) {
        childResult = childResult.concat(MetadataTagModel.flatTree(tagsTree[i].childs));
      }
    }
    return tagsTree.concat(childResult);
  }

  public static buildTree(
    flatTags: MetadataTagModel[],
    rootNodePredicate: (node: MetadataTagModel) => boolean,
    childNodePredicates?: ((node: MetadataTagModel) => boolean)[],
    applyFirstChidNodePredicateForAll: boolean = false
  ): MetadataTagModel[] {
    return MetadataTagModel._buildTree(flatTags, rootNodePredicate, childNodePredicates, 1, applyFirstChidNodePredicateForAll);
  }

  public static filterSelectedTreeNodes(tagsTree: MetadataTagModel[], selectedTagIds: string[]): MetadataTagModel[] {
    return MetadataTagModel._filterSelectedTreeNodes(tagsTree, Utils.toDictionary(selectedTagIds, p => p));
  }

  public static filterTagWithChildsByTopLevelGroupCode(
    tags: MetadataTagModel[],
    groupCode: MetadataTagGroupCode,
    metadataTagsDicByParent: Dictionary<MetadataTagModel[]>
  ): MetadataTagModel[] {
    let result: MetadataTagModel[] = [];
    const topLevelTags = tags.filter(p => p.groupCode === groupCode);
    const remainingTags = tags.filter(p => p.groupCode !== groupCode);
    result = result.concat(topLevelTags);
    for (let i = 0; i < topLevelTags.length; i++) {
      const topLevelTag = topLevelTags[i];
      result = result.concat(MetadataTagModel.getChildsTagRecursively(topLevelTag, remainingTags, metadataTagsDicByParent));
    }
    return result;
  }

  public static getChildsTagRecursively(
    topLevelTag: MetadataTagModel,
    allTags: MetadataTagModel[],
    metadataTagsDicByParent: Dictionary<MetadataTagModel[]>
  ): MetadataTagModel[] {
    let result: MetadataTagModel[] = [];
    const topLevelTagChilds = metadataTagsDicByParent[topLevelTag.tagId];
    if (topLevelTagChilds) {
      const topLevelTagChildsDic = Utils.toDictionary(topLevelTagChilds, p => p.id);
      const selectedChilds = allTags.filter(selectedTags => topLevelTagChildsDic[selectedTags.tagId] != null);
      result = result.concat(selectedChilds);
      const remainingChilds = allTags.filter(
        p => selectedChilds.map(child => child.tagId).indexOf(p.tagId) === -1 && p.tagId !== topLevelTag.tagId
      );
      topLevelTagChilds.forEach(child => {
        result = result.concat(MetadataTagModel.getChildsTagRecursively(child, remainingChilds, metadataTagsDicByParent));
      });
    }
    return result;
  }

  public static selectAllTreeItemIds(treeRootItem: MetadataTagModel, selectedIds: string[]): string[] {
    const allTreeItemIds = MetadataTagModel.flatTree([treeRootItem]).map(p => p.id);
    return Utils.distinct(selectedIds.concat(allTreeItemIds));
  }

  public static deselectAllTreeItemIds(treeRootItem: MetadataTagModel, selectedIds: string[]): string[] {
    const allTreeItemIds = MetadataTagModel.flatTree([treeRootItem]).map(p => p.id);
    return Utils.removeAlls(selectedIds, allTreeItemIds);
  }

  private static _filterSelectedTreeNodes(tagsTree: MetadataTagModel[], selectedTagIdsDic: Dictionary<string>): MetadataTagModel[] {
    return tagsTree
      .map(p => {
        p = Utils.clone(p, clonedP => {
          clonedP.childs =
            clonedP.childs && clonedP.childs.length > 0
              ? clonedP.childs.filter(p1 => {
                  if (p1.childs == null || p1.childs.length === 0) {
                    return selectedTagIdsDic[p1.tagId] != null;
                  } else {
                    p1 = Utils.clone(p1, clonedP1 => {
                      clonedP1.childs = MetadataTagModel._filterSelectedTreeNodes(p1.childs, selectedTagIdsDic);
                    });
                    return selectedTagIdsDic[p1.tagId] != null || p1.childs.length > 0;
                  }
                })
              : clonedP.childs;
        });
        return p;
      })
      .filter(p => selectedTagIdsDic[p.tagId] != null || (p.childs != null && p.childs.length > 0));
  }

  private static _buildTree(
    flatTags: MetadataTagModel[],
    rootNodePredicate: (node: MetadataTagModel) => boolean | null,
    childNodePredicates: ((node: MetadataTagModel) => boolean)[],
    currentLevel: number,
    applyFirstChidNodePredicateForAll: boolean
  ): MetadataTagModel[] {
    const rootNodes = flatTags.filter(p => rootNodePredicate == null || rootNodePredicate(p));
    if (rootNodes.length === 0) {
      return rootNodes;
    }

    const otherTags = flatTags.filter(p => rootNodePredicate == null || !rootNodePredicate(p));
    const childNodePredicate =
      childNodePredicates != null
        ? applyFirstChidNodePredicateForAll === true
          ? childNodePredicates[0]
          : childNodePredicates[currentLevel - 1]
        : null;
    for (let i = 0; i < rootNodes.length; i++) {
      rootNodes[i] = Utils.clone(rootNodes[i], clonedRootNode => {
        clonedRootNode.level = currentLevel;
        clonedRootNode.childs = MetadataTagModel._buildTree(
          otherTags,
          p => p.parentTagId === clonedRootNode.tagId,
          childNodePredicates,
          currentLevel + 1,
          applyFirstChidNodePredicateForAll
        ).filter(p => childNodePredicate == null || childNodePredicate(p));
      });
    }

    return rootNodes;
  }

  constructor(data?: IMetadataTagModel) {
    if (data != null) {
      this.tagId = data.tagId != null ? data.tagId.toLowerCase() : null;
      this.parentTagId = data.parentTagId != null ? data.parentTagId.toLowerCase() : null;
      this.fullStatement = data.fullStatement;
      this.displayText = data.displayText;
      this.groupCode = data.groupCode;
      this.codingScheme = data.codingScheme;
      this.note = data.note;
      this.type = data.type;
      this.childs = data.childs !== undefined ? data.childs.map(p => new MetadataTagModel(p)) : undefined;
    }
  }

  public filterChildItems(collection: MetadataTagModel[] | undefined): MetadataTagModel[] | undefined {
    return collection !== undefined ? collection.filter(p => p.parentTagId === this.tagId) : undefined;
  }

  public allChildsExistedIn(tagIdsDic: Dictionary<string>): boolean | undefined {
    return this.childs != null && this.childs.length > 0 ? this.childs.every(p => tagIdsDic[p.id] != null) : null;
  }

  public anyChildsExistedIn(tagIds: string[]): boolean | undefined {
    return this.childs != null && this.childs.length > 0 ? Utils.includesAny(this.childs.map(p => p.id), tagIds) : null;
  }

  public findWholeTreeTagsWithAllChildsExistedIn(
    tagIdsDic: Dictionary<string>,
    onFound?: (item: MetadataTagModel) => void
  ): MetadataTagModel[] {
    const result: MetadataTagModel[] = [];
    if (this.allChildsExistedIn(tagIdsDic)) {
      result.push(this);
      if (onFound) {
        onFound(this);
      }
    }
    if (this.childs) {
      this.childs.forEach(p => p.findWholeTreeTagsWithAllChildsExistedIn(tagIdsDic, i => result.push(i)));
    }
    return result;
  }
}

export enum MetadataTagType {
  TeacherOutcome = 'Teacher Outcome',
  LearningDimension = 'Learning Dimension',
  LearningArea = 'Learning Area',
  LearningFramework = 'Learning Framework'
}
