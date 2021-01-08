export interface RouterPageInputData<TData, TActiveTab, TSubActiveTab> {
  activeTab?: TActiveTab;
  subActiveTab?: TSubActiveTab;
  data?: TData;
}

export interface RouterPageInput<T, U, P> extends RouterPageInputData<T, U, P> {
  path: string;
  activeMenu?: string;
  parent?: RouterPageInput<unknown, unknown, unknown>;
}

export class RouterPageInputExt {
  public static getRootRoute(route: RouterPageInput<unknown, unknown, unknown>): RouterPageInput<unknown, unknown, unknown> {
    let currentRoute = route;
    while (currentRoute.parent != null) {
      currentRoute = currentRoute.parent;
    }
    return currentRoute;
  }

  public static findRouteInTreeByPath(
    currentRoute: RouterPageInput<unknown, unknown, unknown>,
    path: string
  ): RouterPageInput<unknown, unknown, unknown> | null {
    let result: RouterPageInput<unknown, unknown, unknown> | null = currentRoute.path === path ? currentRoute : null;
    while (currentRoute.parent != null && result == null) {
      if (currentRoute.parent.path === path) {
        result = currentRoute.parent;
        break;
      }
      currentRoute = currentRoute.parent;
    }
    return result;
  }

  public static buildRouterPageInput<T, U, P>(
    path: string,
    activeMenu?: string,
    data?: RouterPageInputData<T, U, P>,
    parent?: RouterPageInput<unknown, unknown, unknown>
  ): RouterPageInput<T, U, P> {
    return {
      activeTab: data != null ? data.activeTab : null,
      path: path,
      activeMenu: activeMenu,
      subActiveTab: data != null ? data.subActiveTab : null,
      data: data != null ? data.data : null,
      parent: parent
    };
  }

  public static extractRouterPageInputData<T, U, P>(pageInput: RouterPageInput<T, U, P>): RouterPageInputData<T, U, P> {
    return {
      activeTab: pageInput.activeTab,
      subActiveTab: pageInput.subActiveTab,
      data: pageInput.data
    };
  }

  public static flatRouteTree(pageInput: RouterPageInput<unknown, unknown, unknown>): RouterPageInput<unknown, unknown, unknown>[] {
    const childToParentOrderRoutes: RouterPageInput<unknown, unknown, unknown>[] = [pageInput];

    let currentPageInput = pageInput;
    while (currentPageInput.parent != null) {
      childToParentOrderRoutes.push(currentPageInput.parent);
      currentPageInput = currentPageInput.parent;
    }

    return childToParentOrderRoutes.reverse();
  }

  public static decodePageInputData(encodedData: string | null): RouterPageInputData<unknown, unknown, unknown> | null {
    try {
      const parsedData = JSON.parse(decodeURIComponent(encodedData));
      if (typeof parsedData === 'object') {
        return parsedData;
      }
      return null;
    } catch (error) {
      return null;
    }
  }

  public static encodePageInputData(data: RouterPageInputData<unknown, unknown, unknown>): string {
    const dataJson = JSON.stringify(data);
    if (dataJson == null || dataJson === '' || dataJson === '{}') {
      // Return "_" when data is empty. "_" represent for empty page input data in url in web app client
      return '_';
    }
    const encodedData = encodeURIComponent(JSON.stringify(data));

    return encodedData;
  }

  public static pageInputToNavigatePath(pageInput: RouterPageInput<unknown, unknown, unknown>): string {
    const parentToChildOrderRoutes = RouterPageInputExt.flatRouteTree(pageInput);

    let path = '';
    parentToChildOrderRoutes.forEach(p => {
      if (p.path != null && p.path !== '') {
        path += '/' + p.path + '/' + RouterPageInputExt.encodePageInputData(RouterPageInputExt.extractRouterPageInputData(p));
      }
    });
    if (path.endsWith('/')) {
      path = path.substr(0, path.length - 1);
    }
    return path;
  }
}
