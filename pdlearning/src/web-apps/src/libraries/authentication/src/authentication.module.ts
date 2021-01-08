import { AuthDataService } from './services/auth-data.service';
import { AuthService } from './services/auth.service';
import { HttpHelpers } from './helpers/httpHelpers';
import { NgModule } from '@angular/core';
import { OAuthService } from './services/oauth-service';
import { PermissionService } from './services/permission.service';
import { UrlHelperService } from './services/url-helper.service';

@NgModule({
  providers: [AuthService, OAuthService, AuthDataService, UrlHelperService, HttpHelpers, PermissionService]
})
export class AuthenticationModule {}
