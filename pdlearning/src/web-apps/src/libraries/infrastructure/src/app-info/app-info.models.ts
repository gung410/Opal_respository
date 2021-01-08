export type StorageType = 'InMemory' | 'LocalStorage' | 'SessionStorage';

/**
 * This is a workaround  for getting the class name when minify javascript.
 * __IS_DYNAMIC_INFO: use in dynamic module loader and this info will be stored in notifiable collection of dynamic service.
 */
export abstract class BaseAppInfoModel {
  public abstract readonly __NAME?: string;
  public abstract readonly __STORAGE_TYPE?: StorageType;
}

export class User extends BaseAppInfoModel {
  public __NAME: string = 'User';
  public __STORAGE_TYPE: StorageType = 'LocalStorage';
}

export class Menu extends BaseAppInfoModel {
  public __NAME: string = 'Menu';
  public __STORAGE_TYPE: StorageType = 'SessionStorage';
}
