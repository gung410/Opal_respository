import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { POCDynamicInfo, POCLocalStorageInfo, POCSessionInfo } from './poc-app-Info.models';

import { Component } from '@angular/core';

@Component({
  selector: 'translation',
  template: `
    <h3>App Info Component</h3>
    Session Storage: {{ session }}
    <br />
    <input [(ngModel)]="sessionModel" />
    <button (click)="changeSessionStorage()">Change</button>
    <button (click)="resetSessionStorage()">Reset</button>
    <br />
    Local Storage: {{ storage }}
    <br />
    <input [(ngModel)]="storageModel" />
    <button (click)="changeLocalStorage()">Change</button>
    <button (click)="resetLocalStorage()">Reset</button>
    <br />
    InMemory Storage: {{ memory }}
    <br />
    <input [(ngModel)]="memoryModel" />
    <button (click)="changeMemoryStorage()">Change</button>
    <button (click)="resetMemoryStorage()">Reset</button>
    <br />
  `
})
export class POCAppInfoComponent extends BasePageComponent {
  public session: string = '';
  public sessionModel: string = '';
  public storage: string = '';
  public storageModel: string = '';
  public memory: string = '';
  public memoryModel: string = '';

  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.subscribe(this.getAppInfo(POCSessionInfo), data => (this.session = JSON.stringify(data)));
    this.subscribe(this.getAppInfo(POCLocalStorageInfo), data => (this.storage = JSON.stringify(data)));
    this.subscribe(this.getAppInfo(POCDynamicInfo), data => (this.memory = JSON.stringify(data)));
  }

  public changeSessionStorage(): void {
    this.setAppInfo(POCSessionInfo, { value: this.sessionModel });
  }

  public resetSessionStorage(): void {
    this.resetAppInfo(POCSessionInfo);
  }

  public changeLocalStorage(): void {
    this.setAppInfo(POCLocalStorageInfo, { value: this.storageModel });
  }

  public resetLocalStorage(): void {
    this.resetAppInfo(POCLocalStorageInfo);
  }

  public changeMemoryStorage(): void {
    this.setAppInfo(POCDynamicInfo, { value: this.memoryModel });
  }

  public resetMemoryStorage(): void {
    this.resetAppInfo(POCDynamicInfo);
  }
}
