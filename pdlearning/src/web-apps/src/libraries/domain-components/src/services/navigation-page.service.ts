import { BehaviorSubject, Observable } from 'rxjs';
import { DialogAction, OpalDialogService, RootElementScrollableService } from '@opal20/common-components';
import { NavigationService, Utils } from '@opal20/infrastructure';
import { RouterPageInput, RouterPageInputData, RouterPageInputExt } from '../models/router-info.model';
import { delay, map } from 'rxjs/operators';

import { Injectable } from '@angular/core';

@Injectable()
export class NavigationPageService {
  public get currentRoute$(): Observable<RouterPageInput<unknown, unknown, unknown> | null> {
    return this._currentRoute$.asObservable().pipe(
      delay(500), // Delay to ensure route has been activated
      map(_ => Utils.cloneDeep(_))
    );
  }

  private _currentRoute$: BehaviorSubject<RouterPageInput<unknown, unknown, unknown> | null> = new BehaviorSubject(null);
  private _navigators: Dictionary<RouterPageInput<unknown, unknown, unknown>> = {};
  private _navigationService: NavigationService = null;
  private _opalDialogService: OpalDialogService = null;
  private _rootElementScrollableService: RootElementScrollableService = null;
  private _rootPath: string = '';
  private _defaultRoute: string = '';

  public static setNavigationDataActiveMenu(
    navigationData: RouterPageInput<unknown, unknown, unknown>,
    activeMenu: string
  ): RouterPageInput<unknown, unknown, unknown> {
    navigationData.activeMenu = activeMenu;

    let currentRouteNode = navigationData;
    while (currentRouteNode.parent != null) {
      currentRouteNode.parent.activeMenu = activeMenu;
      currentRouteNode = currentRouteNode.parent;
    }
    return navigationData;
  }

  public static updateDeeplink(path: string): void {
    if (path.endsWith('/')) {
      path = path.substr(0, path.length - 1);
    }

    if (path === AppGlobal.router.getPath()) {
      return;
    }

    const popStateFn = window.onpopstate;

    window.onpopstate = () => (window.onpopstate = popStateFn);
    AppGlobal.router.setRoute(path);
  }

  public init(
    rootPath: string,
    navigationService: NavigationService,
    rootElementScrollableService: RootElementScrollableService,
    navigators: { [id: string]: RouterPageInput<unknown, unknown, unknown> },
    currentRoutePageInput: RouterPageInput<unknown, unknown, unknown> | null,
    opalDialogService: OpalDialogService,
    defaultRoute: string
  ): void {
    this._rootPath = (AppGlobal.getBaseHref() + rootPath + '/').replace('//', '/');
    this._navigationService = navigationService;
    this._navigators = navigators;
    this._opalDialogService = opalDialogService;
    this._rootElementScrollableService = rootElementScrollableService;
    this._defaultRoute = defaultRoute;
    this.setCurrentRoute(currentRoutePageInput);
  }

  public setCurrentRoute(currentRoutePageInput: RouterPageInput<unknown, unknown, unknown>): void {
    currentRoutePageInput = Utils.clone(currentRoutePageInput);
    currentRoutePageInput.path = this.correctToDefaultRoute(currentRoutePageInput.path);

    this._currentRoute$.next(currentRoutePageInput);
    this._navigators[currentRoutePageInput.path] = currentRoutePageInput;
    if (currentRoutePageInput != null) {
      NavigationPageService.updateDeeplink(this._pageInputToNavigateFullPath(currentRoutePageInput));
    }
  }

  public navigateTo<TData, TActiveTab, TSubActiveTab>(
    path: string,
    pageInputData?: RouterPageInputData<TData, TActiveTab, TSubActiveTab>,
    parent?: RouterPageInput<unknown, unknown, unknown>
  ): void {
    path = this.correctToDefaultRoute(path);
    const currentRoute = this._currentRoute$.value;

    let currentRoutePageInput = this._navigators[path];
    if (currentRoutePageInput == null) {
      currentRoutePageInput = { path: path };
      this._navigators[path] = currentRoutePageInput;
    }
    currentRoutePageInput.parent = parent;
    if (pageInputData != null) {
      currentRoutePageInput.data = pageInputData.data;
      currentRoutePageInput.activeTab = pageInputData.activeTab;
      currentRoutePageInput.subActiveTab = pageInputData.subActiveTab;
    }
    if (currentRoutePageInput.activeMenu == null) {
      currentRoutePageInput.activeMenu = currentRoute.activeMenu;
    }
    this._navigationService.navigateTo(this._navigators[path].path, this._navigators[path]);
    this._rootElementScrollableService.scrollToTop();

    NavigationPageService.updateDeeplink(this._pageInputToNavigateFullPath(currentRoutePageInput));
    this._currentRoute$.next(this._navigators[path]);
  }

  public navigateBack(hasDataChangedFn: () => boolean = null, dataChangedDialogOkActionFn: () => Observable<unknown> = null): void {
    if (hasDataChangedFn && hasDataChangedFn() === true) {
      this._opalDialogService
        .openConfirmDialog({
          confirmTitle: 'Warning',
          confirmMsg: 'You have unsaved changes, would you like to save it now?'
        })
        .subscribe(action => {
          if (action === DialogAction.Cancel) {
            this._navigateBack();
          } else if (action === DialogAction.OK) {
            dataChangedDialogOkActionFn().subscribe();
          }
        });
    } else {
      this._navigateBack();
    }
  }

  public returnRoot(): void {
    this._navigationService.returnRoot();
  }

  public returnHome(): void {
    this._navigationService.returnHome();
  }

  public redirectByRouter(router: RouterPageInput<unknown, unknown, unknown> | null): void {
    setTimeout(() => {
      this.navigateByRouter(router);
    });
  }

  public navigateByRouter(
    router: RouterPageInput<unknown, unknown, unknown> | null,
    hasDataChangedFn: () => boolean = null,
    dataChangedDialogOkActionFn: () => Observable<unknown> = null
  ): void {
    if (router == null) {
      return;
    }
    if (hasDataChangedFn != null && hasDataChangedFn() === true) {
      this._opalDialogService
        .openConfirmDialog({
          confirmTitle: 'Warning',
          confirmMsg: 'You have unsaved changes, would you like to save it now?'
        })
        .subscribe(action => {
          if (action === DialogAction.Cancel) {
            this._navigateByRouter(router);
          } else if (action === DialogAction.OK && dataChangedDialogOkActionFn != null) {
            dataChangedDialogOkActionFn().subscribe(() => {
              this._navigateByRouter(router);
            });
          }
        });
    } else {
      this._navigateByRouter(router);
    }
  }

  public findParentOfCurrentRouter(parentPath: string): RouterPageInput<unknown, unknown, unknown> {
    const currentRoutePath = this._getCurrentRoutePath();
    if (currentRoutePath == null) {
      return;
    }

    const currentRouter = this._navigators[currentRoutePath];

    const parentToChildOrderRoutes = RouterPageInputExt.flatRouteTree(currentRouter);

    return parentToChildOrderRoutes.find(x => x.path === parentPath);
  }

  private _pageInputToNavigateFullPath(pageInput: RouterPageInput<unknown, unknown, unknown>): string {
    return this._combineWithRootPath(RouterPageInputExt.pageInputToNavigatePath(pageInput));
  }

  private _navigateBack(): void {
    const currentRoutePath = this._getCurrentRoutePath();
    if (currentRoutePath == null) {
      return;
    }
    const parentRouter = this._navigators[currentRoutePath].parent;
    if (parentRouter) {
      this._navigateByRouter(parentRouter);
    } else {
      this.navigateTo(this._defaultRoute);
    }
  }

  private _getCurrentRoutePath(): string | null {
    return this._currentRoute$.getValue() != null ? this._currentRoute$.getValue().path : null;
  }

  private _navigateByRouter(router: RouterPageInput<unknown, unknown, unknown>): void {
    if (router) {
      this._navigators[router.path] = router;
      this._navigationService.navigateTo(router.path, router);
      this._rootElementScrollableService.scrollToTop();
      NavigationPageService.updateDeeplink(this._combineWithRootPath(RouterPageInputExt.pageInputToNavigatePath(router)));
      this._currentRoute$.next(router);
    }
  }

  private _combineWithRootPath(path: string): string {
    let result = `${this._rootPath}${path}`.replace('//', '/');
    if (result.endsWith('/')) {
      result = result.substr(0, result.length - 1);
    }
    return result;
  }

  private correctToDefaultRoute(path: string): string {
    if (path === '') {
      return this._defaultRoute;
    }
    return path;
  }
}
