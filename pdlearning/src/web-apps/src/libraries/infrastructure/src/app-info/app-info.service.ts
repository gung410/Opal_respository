import { APP_LOCAL_STORAGE_KEY, APP_SESSION_STORAGE_KEY } from '../constants';
import { BaseAppInfoModel, StorageType } from './app-info.models';
import { BehaviorSubject, Observable } from 'rxjs';
import { Injectable, Type } from '@angular/core';
import { LocalStorageUtils, SessionStorageUtils } from '../utils/app-storage.utils';

@Injectable()
export class AppInfoService {
  private _notifiers: { [key: string]: BehaviorSubject<BaseAppInfoModel> } = {};
  private _appInfoCollection: Map<string, BaseAppInfoModel> = new Map();
  private _accessToken: string;

  /**
   * Return directly data from storage.
   * @param type Class type. E.g., Menu, User...
   */
  public getSync<T extends BaseAppInfoModel>(type: Type<T>): T | null {
    return this.getDataFromStorage(type);
  }

  /**
   * Return observable of data from storage.
   * @param type Class type. E.g., Menu, User...
   */
  public get<T extends BaseAppInfoModel>(type: Type<T>): Observable<T | null> {
    const typeName: string = this.getTypeName(type);
    const data: T = this.getDataFromStorage(type);

    if (!this._notifiers[typeName]) {
      this._notifiers[typeName] = new BehaviorSubject(data);
    }

    return <Observable<T>>this._notifiers[typeName].asObservable();
  }

  /**
   * Set data into storage or collection of dynamic service.
   * @param type Class type. E.g., Menu, User...
   * @param data The app info data.
   * @param replacale The system will update the data instead of new one.
   */
  public set<T extends BaseAppInfoModel>(type: Type<T>, data: Partial<T>, replacale: boolean = true): void {
    const typeName: string = this.getTypeName(type);
    const storageType: StorageType = this.getStorageType(type);

    switch (storageType) {
      case 'LocalStorage':
        LocalStorageUtils.set(APP_LOCAL_STORAGE_KEY, { [typeName]: data }, replacale);
        break;
      case 'SessionStorage':
        SessionStorageUtils.set(APP_SESSION_STORAGE_KEY, { [typeName]: data }, replacale);
        break;
      case 'InMemory':
      default:
        this._appInfoCollection.set(typeName, <T>data);
    }

    this.notifyDataChanged(typeName, <T>data);
  }

  public reset<T extends BaseAppInfoModel>(type: Type<T>): void {
    // tslint:disable-next-line:no-any
    const info: any = new type();

    delete info.__NAME;
    delete info.__STORAGE_TYPE;

    this.set(type, info);
  }

  public getAccessToken(): string {
    return this._accessToken;
  }

  public setAccessToken(accessToken: string): void {
    this._accessToken = accessToken;
  }

  private getDataFromStorage<T extends BaseAppInfoModel>(type: Type<T>): T | null {
    const typeName: string = this.getTypeName(type);
    const storageType: StorageType = this.getStorageType(type);

    switch (storageType) {
      case 'LocalStorage':
        return LocalStorageUtils.get(APP_LOCAL_STORAGE_KEY, typeName);
      case 'SessionStorage':
        return SessionStorageUtils.get(APP_SESSION_STORAGE_KEY, typeName);
      case 'InMemory':
      default:
        return <T>this._appInfoCollection.get(typeName);
    }
  }

  private notifyDataChanged<T extends BaseAppInfoModel>(dataType: string, value: T): void {
    if (this._notifiers.hasOwnProperty(dataType)) {
      this._notifiers[dataType].next(value);
    }
  }

  private getTypeName(type: Type<BaseAppInfoModel>): string {
    return new type().__NAME || type.name;
  }

  private getStorageType(type: Type<BaseAppInfoModel>): StorageType {
    return new type().__STORAGE_TYPE || 'InMemory';
  }
}
