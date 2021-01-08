import { AfterViewInit, ChangeDetectorRef, OnChanges, OnInit, SimpleChanges, Type } from '@angular/core';
import { MonoTypeOperatorFunction, Observable, Subject, Subscription, of } from 'rxjs';
import { NAVIGATION_PARAMETERS_KEY, TIME_HIDDEN_NOTIFICATION } from '../constants';

import { Align } from '@progress/kendo-angular-popup';
import { BaseAppInfoModel } from '../app-info/app-info.models';
import { ICanDeactivateComponent } from '../form/guards/form-guard';
import { ITranslationParams } from '../translation/translation.models';
import { ModalService } from '../services/modal.service';
import { ModuleFacadeService } from '../services/module-facade.service';
import { Subscribable } from '../subscribable';
import { Utils } from '../utils/utils';
import { takeUntil } from 'rxjs/operators';

export enum NotificationType {
  None = 'none',
  Success = 'success',
  Warning = 'warning',
  Error = 'error',
  Info = 'info'
}

export abstract class BaseComponent extends Subscribable implements OnInit, AfterViewInit, OnChanges, ICanDeactivateComponent {
  get modalService(): ModalService {
    return this.moduleFacadeService.modalService;
  }
  public static defaultDetectChangesDelay: number = 100;

  public onDestroy$: Subject<unknown> = new Subject<unknown>();
  public initiated: boolean = false;
  public viewInitiated: boolean = false;
  public defaultContextMenuAnchorAlign: Align = { horizontal: 'center', vertical: 'bottom' };
  public defaultContextMenuPopupAlign: Align = { horizontal: 'center', vertical: 'top' };

  protected destroyed: boolean = false;

  private _detectChangesDelaySubs: Subscription = new Subscription();

  protected get canDetectChanges(): boolean {
    return this.initiated && !this.destroyed;
  }

  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super();
  }

  public ngOnChanges(changes: SimpleChanges): void {
    this.onChanges(changes);
  }

  /**
   * This function is for internal use only, please don't override it
   * @internal FW
   */
  public ngOnInit(): void {
    this.internalInit();
    this.onInit();
    this.initiated = true;
  }

  /**
   * This function is for internal use only, please don't override it
   * @internal FW
   */
  public ngOnDestroy(): void {
    super.ngOnDestroy();
    this.onDestroy$.next();
    this.onDestroy$.complete();
    this.initiated = false;
    this.destroyed = true;
  }

  /**
   * This function is for internal use only, please don't override it
   * @internal FW
   */
  public ngAfterViewInit(): void {
    this.internalAfterViewInit();
    this.onAfterViewInit();
    this.viewInitiated = true;
  }

  public canDeactivate(): Observable<boolean> {
    return of(true);
  }

  public getNavigateData<T>(): T {
    return <T>this.moduleFacadeService.contextDataService.getData(NAVIGATION_PARAMETERS_KEY);
  }

  public clearNavigateData(): void {
    return this.moduleFacadeService.contextDataService.clearAll();
  }

  public returnHome(): void {
    this.moduleFacadeService.navigationService.returnHome();
  }

  public navigateTo<T>(path: string, parameters?: T): void {
    this.moduleFacadeService.navigationService.navigateTo(path, parameters);
  }

  public updateDeeplink(path: string): void {
    if (AppGlobal.router.getPath() === `${this.moduleFacadeService.baseHref}${path}`) {
      return;
    }

    this.moduleFacadeService.navigationService.updateDeeplink(path);
  }

  public translate(key: string | Array<string>, interpolateParams?: ITranslationParams): string {
    return this.moduleFacadeService.translator.translate(key, interpolateParams);
  }

  public translateCommon(key: string | Array<string>, interpolateParams?: ITranslationParams): string {
    return this.moduleFacadeService.translator.translateCommon(key, interpolateParams);
  }

  public getAppInfo<T extends BaseAppInfoModel>(type: Type<T>): Observable<T> {
    return this.moduleFacadeService.appInfoService.get(type);
  }

  public getAppInfoSync<T extends BaseAppInfoModel>(type: Type<T>): T {
    return this.moduleFacadeService.appInfoService.getSync(type);
  }

  public setAppInfo<T extends BaseAppInfoModel>(type: Type<T>, data: Partial<T>, replaceable: boolean = true): void {
    return this.moduleFacadeService.appInfoService.set(type, data, replaceable);
  }

  public resetAppInfo<T extends BaseAppInfoModel>(type: Type<T>): void {
    return this.moduleFacadeService.appInfoService.reset(type);
  }

  public untilDestroy<T>(): MonoTypeOperatorFunction<T> {
    return takeUntil(this.onDestroy$);
  }

  public detectChanges(
    changeDetectorRef: ChangeDetectorRef,
    immediateOrDelay?: boolean | number,
    onDone?: () => unknown,
    checkParentForHostbinding: boolean = false
  ): void {
    this._detectChangesDelaySubs.unsubscribe();
    if (!this.canDetectChanges) {
      return;
    }

    const delayTime =
      typeof immediateOrDelay === 'number' ? immediateOrDelay : immediateOrDelay ? 0 : BaseComponent.defaultDetectChangesDelay;
    this._detectChangesDelaySubs = Utils.delay(() => {
      if (this.canDetectChanges) {
        changeDetectorRef.detectChanges();
        if (checkParentForHostbinding) {
          changeDetectorRef.markForCheck();
        }
        if (onDone != null) {
          onDone();
        }
      }
    }, delayTime);
  }

  public showNotification(
    content: string = this.translateCommon('The record has been updated successfully.'),
    type: NotificationType | string = NotificationType.Success
  ): void {
    this.moduleFacadeService.notificationService.show({
      content: content,
      hideAfter: TIME_HIDDEN_NOTIFICATION,
      position: { horizontal: 'right', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: type as ('none' | 'success' | 'warning' | 'error' | 'info'), icon: true }
    });
  }

  /**
   * This function is for internal use only, please don't override it
   * @internal FW
   */
  protected internalInit(): void {
    const navigationParameters: unknown =
      this.moduleFacadeService.contextDataService && this.moduleFacadeService.contextDataService.getData(NAVIGATION_PARAMETERS_KEY);

    if (navigationParameters) {
      this.onNavigatedTo(navigationParameters);
    }
  }

  /**
   * This function is for internal use only, please don't override it
   * @internal FW
   */
  protected internalAfterViewInit(): void {
    // Virtual method
  }

  /**
   * This function is for internal use only, please don't override it
   * @internal FW
   */
  protected internalDestroy(): void {
    // Virtual method
  }

  protected onChanges(changes: SimpleChanges): void {
    // Virtual method
  }

  protected onInit(): void {
    // Virtual method
  }

  protected onAfterViewInit(): void {
    // Virtual method
  }

  protected onDestroy(): void {
    // Virtual method
    this.moduleFacadeService.contextDataService.removeData(this.moduleFacadeService.moduleInstance.contextDataKey);
  }

  /**
   * This method is executed when another page navigates to us.
   */
  protected onNavigatedTo<T>(parameters: T): void {
    // Virtual method
  }

  /**
   * This method is suppport component to implement return current loging user permission to help check user permission
   */
  protected currentUserPermissionDic(): IPermissionDictionary {
    // Virtual method
    return {};
  }

  /**
   * This method check the current user has a permission by providing permissionKey
   */
  protected hasPermission(permissionKey: string): boolean {
    const userPermissionDic = this.currentUserPermissionDic();
    const permission: IModulePermission = userPermissionDic[permissionKey];

    return permission ? permission.grantedType === 'Allow' : false;
  }

  protected hasPermissions(permissionKeys: string[]): boolean {
    return permissionKeys.every(key => this.hasPermission(key));
  }

  protected hasPermissionPrefix(permissionKeyPrefix: string): boolean {
    const userPermissionKeys = Object.keys(this.currentUserPermissionDic());
    for (let i = 0; i < userPermissionKeys.length; i++) {
      const userPermissionKey = userPermissionKeys[i];
      if (userPermissionKey.startsWith(permissionKeyPrefix)) {
        return this.hasPermission(userPermissionKey);
      }
    }
    return false;
  }
}
