export enum InterceptorType {
  Authentication = 'AUTHENTICATION_INTERCEPTOR',
  HttpResponse = 'HTTP_RESPONSE_INTERCEPTOR',
  Spinner = 'SPINNER_INTERCEPTOR'
}

export interface Interceptor {
  key: string;
  type: InterceptorType;
}

export class InterceptorRegistry {
  private registry: Interceptor[] = [];

  constructor(interceptors: Interceptor[]) {
    this.registry = interceptors;
  }

  public register(interceptor: Interceptor): InterceptorRegistry {
    if (!interceptor) {
      return this;
    }

    this.registry.push(interceptor);

    return this;
  }

  public replace(type: InterceptorType, interceptor?: Interceptor): InterceptorRegistry {
    this.registry = this.registry.filter(i => i.type !== type);

    return this.register(interceptor);
  }

  public toJSON(): string {
    return JSON.stringify(this.registry.map(i => i.key));
  }
}

export const DEFAULT_INTERCEPTORS: Interceptor[] = [
  {
    key: InterceptorType.Authentication,
    type: InterceptorType.Authentication
  },
  {
    key: InterceptorType.HttpResponse,
    type: InterceptorType.HttpResponse
  },
  {
    key: InterceptorType.Spinner,
    type: InterceptorType.Spinner
  }
];
