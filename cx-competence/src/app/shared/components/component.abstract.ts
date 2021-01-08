import { ChangeDetectorRef, OnInit } from '@angular/core';
import { AuthService } from 'app-auth/auth.service';
import { User } from 'app-models/auth.model';
import { Subscription } from 'rxjs';
import { AppConstant } from '../app.constant';

export class BaseComponent {
  protected subscribers: Subscription[];

  protected set subscriptionAdder(subscriber: Subscription) {
    this.subscribers.push(subscriber);
  }

  constructor(protected changeDetectorRef: ChangeDetectorRef) {
    this.subscribers = [];
  }

  ngOnDestroy() {
    this.changeDetectorRef.detach();
    if (this.subscribers && this.subscribers.length) {
      for (const subscriber of this.subscribers) {
        subscriber.unsubscribe();
      }
    }
  }

  protected getUserImage(user: User) {
    return user && user.avatarUrl ? user.avatarUrl : AppConstant.defaultAvatar;
  }
}

export class BaseSmartComponent extends BaseComponent {
  protected subscription = new Subscription();
  constructor(changeDetectorRef: ChangeDetectorRef) {
    super(changeDetectorRef);
  }
  ngOnDestroy() {
    super.ngOnDestroy();
    this.subscription.unsubscribe();
  }
}

export class BasePresentationComponent extends BaseComponent {
  constructor(changeDectectorRef: ChangeDetectorRef) {
    super(changeDectectorRef);
  }
  ngOnDestroy() {
    super.ngOnDestroy();
  }
}

export class BaseScreenComponent extends BaseSmartComponent implements OnInit {
  private static _currentUser: User;

  constructor(
    changeDetectorRef: ChangeDetectorRef,
    protected authService: AuthService
  ) {
    super(changeDetectorRef);
  }

  ngOnInit(): void {}

  public get currentUser() {
    return BaseScreenComponent._currentUser;
  }

  public set currentUser(user: User) {
    BaseScreenComponent._currentUser = user;
  }

  onLogout() {
    this.authService.logout();
  }
}
