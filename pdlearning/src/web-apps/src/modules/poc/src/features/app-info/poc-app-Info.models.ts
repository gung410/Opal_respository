import { BaseAppInfoModel, StorageType } from '@opal20/infrastructure';

export class POCSessionInfo extends BaseAppInfoModel {
  public __NAME: string = 'POCSessionInfo';
  public __STORAGE_TYPE: StorageType = 'SessionStorage';

  public value: string = '1';
}

export class POCLocalStorageInfo extends BaseAppInfoModel {
  public __NAME: string = 'POCLocalStorageInfo';
  public __STORAGE_TYPE: StorageType = 'LocalStorage';

  public value: string = '2';
}

export class POCDynamicInfo extends BaseAppInfoModel {
  public __NAME: string = 'POCDynamicInfo';
  public __STORAGE_TYPE: StorageType = 'InMemory';

  public value: string = '3';
}
