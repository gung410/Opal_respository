import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { Utils } from '../utils/utils';

/**
 * This class is used for Function scope only.
 */
@Injectable()
export class ContextDataService {
  public onContextDataChanged: Subject<{ [key: string]: unknown }> = new Subject();

  private contextData: { [key: string]: unknown } = {};

  public clearAllExcept(keptKeys?: string[]): void {
    if (keptKeys) {
      const newContextData: { [key: string]: unknown } = {};
      keptKeys.forEach(key => {
        if (this.hasKey(key)) {
          newContextData[key] = this.contextData[key];
        }
      });
      this.contextData = newContextData;
    } else {
      this.contextData = {};
    }

    this.notifyContextDataChanged();
  }

  public clearAll(): void {
    this.contextData = {};
    this.notifyContextDataChanged();
  }

  public setData<T>(key: string, data: T): void {
    this.contextData[key] = data;
    this.notifyContextDataChanged();
  }

  public removeData(key: string): void {
    delete this.contextData[key];
    this.notifyContextDataChanged();
  }

  public isEmpty(): boolean {
    return Utils.isEmpty(this.contextData);
  }

  public hasData(key: string): boolean {
    const found: unknown = this.contextData[key];

    return found !== null && found !== undefined;
  }

  public hasKey(key: string): boolean {
    return this.contextData.hasOwnProperty(key);
  }

  public getData<T>(key: string): T {
    return <T>this.contextData[key];
  }

  public appendData(data: { [key: string]: unknown }): void {
    for (const propertyName in data) {
      if (data.hasOwnProperty(propertyName)) {
        this.setData(propertyName, data[propertyName]);
      }
    }
    this.notifyContextDataChanged();
  }

  private notifyContextDataChanged(): void {
    this.onContextDataChanged.next(this.contextData);
  }
}

/**
 * This class is used for other dynamic module scope only
 */
@Injectable()
export class ModuleDataService extends ContextDataService {}
