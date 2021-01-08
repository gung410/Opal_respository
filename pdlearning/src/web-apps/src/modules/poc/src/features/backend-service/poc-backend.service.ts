import { BaseBackendService, CommonFacadeService, InterceptorRegistry, InterceptorType } from '@opal20/infrastructure';

import { Injectable } from '@angular/core';

export interface IIpAddress {
  ip: string;
}

@Injectable()
export class POCBackendService extends BaseBackendService {
  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public getIpAddress(): Promise<IIpAddress> {
    return this.get<IIpAddress>('https://cors-anywhere.herokuapp.com/https://api.ipify.org?format=json').toPromise();
  }

  public getWithHttpError(): Promise<IIpAddress> {
    return this.get<IIpAddress>('https://cors-anywhere.herokuapp.com/').toPromise();
  }

  protected onFilterInterceptors(registry: InterceptorRegistry): InterceptorRegistry {
    return registry.replace(InterceptorType.Authentication, {
      key: 'EXTERNAL_AUTHENTICATION_INTERCEPTOR',
      type: InterceptorType.Authentication
    });
  }
}
