import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuardService } from 'app-authguards/auth-guard.service';
import { SelectiveStrategyService } from 'app-services/selective-strategy.service';

import { ErrorPageComponent } from './error-page/error-page.component';
import { LoginComponent } from './login/login.component';
import { SessionTimeoutPageComponent } from './session-timeout-page/session-timeout-page.component';
import { AppConstant } from './shared/app.constant';

/**
 * Use this data :{sendInReturnUrlOidc : true  } setting to make the route include in return url when SPA redirect to IDP for login
 */
export const routes: Routes = [
  {
    path: '',
    redirectTo: AppConstant.siteURL.login,
    pathMatch: 'full'
  },
  {
    path: AppConstant.siteURL.login,
    component: LoginComponent
  },
  {
    path: AppConstant.siteURL.error,
    component: ErrorPageComponent
  },
  {
    path: AppConstant.siteURL.error + '/:id',
    component: ErrorPageComponent
  },
  {
    path: AppConstant.siteURL.monitor,
    canActivate: [AuthGuardService],
    loadChildren: 'app/monitor/monitor.module#MonitorModule'
  },
  {
    path: AppConstant.siteURL.sessionTimeout,
    component: SessionTimeoutPageComponent
  },
  {
    path: 'management',
    canActivate: [AuthGuardService],
    loadChildren: 'app/management/management.module#ManagementModule'
  },
  {
    path: AppConstant.siteURL.menus.organization,
    canActivate: [AuthGuardService],
    loadChildren:
      'app/department-hierarchical/department-hierarchical.module#DepartmentHierarchicalModule'
  },
  {
    path: AppConstant.siteURL.menus.userAccounts,
    canActivate: [AuthGuardService],
    loadChildren: 'app/user-accounts/user-accounts.module#UserAccountsModule'
  },
  {
    path: AppConstant.siteURL.menus.reports,
    canActivate: [AuthGuardService],
    loadChildren: 'app/reports/reports.module#ReportsModule'
  },
  {
    path: AppConstant.siteURL.menus.userGroups,
    canActivate: [AuthGuardService],
    loadChildren: 'app/user-groups/user-groups.module#UserGroupsModule'
  },
  {
    path: AppConstant.siteURL.menus.broadcastMessages,
    canActivate: [AuthGuardService],
    loadChildren:
      'app/broadcast-messages/broadcast-messages.module#BroadcastMessagesModule'
  },
  {
    path: AppConstant.siteURL.menus.permissions,
    canActivate: [AuthGuardService],
    loadChildren: 'app/permissions/permissions.module#PermissionsModule'
  },
  {
    path: AppConstant.siteURL.menus.systemAuditLog,
    canActivate: [AuthGuardService],
    loadChildren:
      'app/system-audit-log/system-audit-log.module#SystemAuditLogModule'
  },
  {
    path: '**',
    redirectTo: AppConstant.siteURL.login
  }
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes, {
      preloadingStrategy: SelectiveStrategyService,
      onSameUrlNavigation: 'reload'
    })
  ],
  providers: [SelectiveStrategyService],
  exports: [RouterModule]
})
export class AppRoutingModule {}
