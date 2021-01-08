import {
  AfterViewInit,
  ChangeDetectorRef,
  OnChanges,
  OnDestroy,
  OnInit,
  SimpleChanges
} from '@angular/core';
import { AuthService } from 'app-auth/auth.service';
import { User } from 'app-models/auth.model';
import { Utils } from 'app-utilities/utils';
import { GrantedType } from 'app/permissions/enum/granted-type.enum';
import { AccessRightsModel } from 'app/permissions/models/access-rights.model';
import { AppConstant } from 'app/shared/app.constant';
import { UserManagement } from 'app/user-accounts/models/user-management.model';
import { MonoTypeOperatorFunction, Subject, Subscription } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { Subscribable } from '../subscribable';

// This OpalBaseComponent is copied from BaseClass of WebApp from Opal Thunder project
export abstract class OpalBaseComponent
  extends Subscribable
  implements OnInit, AfterViewInit, OnChanges {
  public static defaultDetectChangesDelay: number = 100;

  public onDestroy$: Subject<unknown> = new Subject<unknown>();
  public initiated: boolean = false;
  public viewInitiated: boolean = false;

  protected destroyed: boolean = false;

  private _detectChangesDelaySubs: Subscription = new Subscription();

  protected get canDetectChanges(): boolean {
    return this.initiated && !this.destroyed;
  }

  constructor() {
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
      typeof immediateOrDelay === 'number'
        ? immediateOrDelay
        : immediateOrDelay
        ? 0
        : OpalBaseComponent.defaultDetectChangesDelay;
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

  protected onChanges(changes: SimpleChanges): void {
    // Virtual method
  }

  protected onInit(): void {
    // Virtual method
  }

  protected onAfterViewInit(): void {
    // Virtual method
  }

  /**
   * This function is for internal use only, please don't override it
   * @internal FW
   */
  protected internalAfterViewInit(): void {
    // Virtual method
  }
}

// tslint:disable-next-line: max-classes-per-file
export class BaseComponent {
  protected getUserImage(user: UserManagement) {
    return user &&
      user.jsonDynamicAttributes &&
      user.jsonDynamicAttributes.avatarUrl
      ? user.jsonDynamicAttributes.avatarUrl
      : AppConstant.defaultAvatar;
  }
}

// tslint:disable:max-classes-per-file
export class BaseSmartComponent extends BaseComponent implements OnDestroy {
  protected subscription: Subscription = new Subscription();
  constructor(protected changeDetectorRef: ChangeDetectorRef) {
    super();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}

export class BasePresentationComponent extends BaseComponent {
  constructor(protected changeDetectorRef: ChangeDetectorRef) {
    super();
  }
}

export class BaseScreenComponent extends BaseSmartComponent implements OnInit {
  currentUser: User;

  constructor(
    protected changeDetectorRef: ChangeDetectorRef,
    protected authService: AuthService
  ) {
    super(changeDetectorRef);
    this.getCurrentUser();
  }

  ngOnInit(): void {
    // not implement
  }

  onLogout(): void {
    this.authService.logout();
  }

  protected currentUserPermissionDic(): IPermissionDictionary {
    // Virtual method
    return {};
  }

  protected hasPermission(permissionKey: string): boolean {
    if (!this.currentUser) {
      return false;
    }

    const userPermissionDic = this.currentUserPermissionDic();
    const permission: AccessRightsModel = userPermissionDic[permissionKey];

    return permission ? permission.grantedType === GrantedType.Allow : false;
  }

  protected hasPermissions(permissionKeys: string[]): boolean {
    return permissionKeys.every((key) => this.hasPermission(key));
  }

  private getCurrentUser(): void {
    this.subscription.add(
      this.authService.userData().subscribe(async (user: User) => {
        this.currentUser = user;
      })
    );
  }
}

export declare interface IPermissionDictionary {
  [permission: string]: AccessRightsModel;
}
