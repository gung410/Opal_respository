import { CxObjectUtil } from './object.util';

export default class CxTreeUtil {
  public static getChildrenIdsMap(
    flatObjectsArray: any[],
    object: any,
    idFieldRoute: string,
    parentIdFieldRoute: string
  ): any {
    const childrenIdsMap = {};
    flatObjectsArray.forEach(obj => {
      if (CxObjectUtil.getPropertyValue(obj, parentIdFieldRoute)
        !== CxObjectUtil.getPropertyValue(object, idFieldRoute)) { return; }
      childrenIdsMap[CxObjectUtil.getPropertyValue(obj, idFieldRoute)] = obj;
    });
    return childrenIdsMap;
  }

  public static getObjectsExceptCurrentObjectAndDirectChildren(flatObjectsArray: any[], rootObject: any,
    idFieldRoute: string, parentIdFieldRoute: string): any[] {
    const childrenIdsMap = this.getChildrenIdsMap(flatObjectsArray,
      rootObject, idFieldRoute, parentIdFieldRoute);
    return flatObjectsArray.filter(obj =>
      CxObjectUtil.getPropertyValue(rootObject, idFieldRoute) !== CxObjectUtil.getPropertyValue(obj, idFieldRoute)
      && childrenIdsMap[CxObjectUtil.getPropertyValue(obj, idFieldRoute)] === undefined);
  }
}
