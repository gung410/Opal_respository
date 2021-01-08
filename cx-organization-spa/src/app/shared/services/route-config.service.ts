import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class RouteConfigService {
  // List of route which support to send in redirecturl when redirect to IDM for login
  // If routes have 'sendInReturnUrlOidc' configured to true, that routes will be added
  // by SelectiveStrategyService to this list when SPA starting
  SendInRedirectUrlRoutePaths: string[];

  constructor() {
    this.SendInRedirectUrlRoutePaths = [];
  }
}
