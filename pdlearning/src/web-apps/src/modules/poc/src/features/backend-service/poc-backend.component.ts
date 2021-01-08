import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';

import { BehaviorSubject } from 'rxjs';
import { Component } from '@angular/core';
import { POCBackendService } from './poc-backend.service';

@Component({
  selector: 'backend-service',
  template: `
    <h3>Backend Service</h3>
    IP Address: {{ ip$ | async }}
    <br />
    <button (click)="getIpAddress()">Get IP address</button>
    <br />
    <button (click)="getWithError()">Get with error</button>
  `
})
export class POCBackendComponent extends BasePageComponent {
  public ip$: BehaviorSubject<string> = new BehaviorSubject('...');

  constructor(protected moduleFacadeService: ModuleFacadeService, private backendService: POCBackendService) {
    super(moduleFacadeService);
  }

  public getIpAddress(): void {
    this.backendService.getIpAddress().then(response => this.ip$.next(response.ip));
  }

  public getWithError(): void {
    this.backendService.getWithHttpError().then(response => this.ip$.next(response.ip));
  }
}
