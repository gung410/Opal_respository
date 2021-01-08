// tslint:disable-next-line: max-line-length
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuardService } from 'app-authguards/auth-guard.service';
import { SelectiveStrategyService } from 'app-services/selective-strategy.service';
import { ErrorPageComponent } from './error-page/error-page.component';
import { LoginComponent } from './login/login.component';
import { ReviewMyProfessionalDevelopmentJourneyComponent } from './professional-growth/review-my-professional-development-journey/review-my-professional-development-journey.component';
import { ProfileComponent } from './profile/profile.component';
import { SessionTimeoutPageComponent } from './session-timeout-page/session-timeout-page.component';
import { AppConstant } from './shared/app.constant';
import { MyCalendarComponent } from './training-calendar/my-calendar/my-calendar.component';

/**
 * Use this data :{sendInReturnUrlOidc : true  } setting to make the route include in return url when SPA redirect to IDP for login
 */
export const routes: Routes = [
  {
    path: '',
    redirectTo: AppConstant.siteURL.login,
    pathMatch: 'full',
  },
  {
    path: AppConstant.mobileUrl.pdPlanner,
    loadChildren: () => import('app/mobile/pd-planner/pd-planner.module').then(m => m.PDPlannerModule),
  },
  {
    path: `${AppConstant.mobileUrl.pdPlanner}/:target`,
    loadChildren: () =>
      import('app/mobile/pd-planner/pd-planner.module').then(
        (m) => m.PDPlannerModule
      ),
  },
  {
    path: AppConstant.siteURL.login,
    component: LoginComponent,
  },
  {
    path: AppConstant.siteURL.error + '/:id',
    component: ErrorPageComponent,
  },
  {
    path: AppConstant.siteURL.error,
    component: ErrorPageComponent,
  },
  {
    path: AppConstant.siteURL.monitor,
    canActivate: [AuthGuardService],
    loadChildren: () => import('app/monitor/monitor.module').then(m => m.MonitorModule),
  },
  {
    path: AppConstant.siteURL.sessionTimeout,
    component: SessionTimeoutPageComponent,
  },
  {
    path: AppConstant.siteURL.profile,
    canActivate: [AuthGuardService],
    component: ProfileComponent,
  },
  {
    path: AppConstant.siteURL.management,
    canActivate: [AuthGuardService],
    loadChildren: () => import('app/management/management.module').then(m => m.ManagementModule),
  },
  {
    path: AppConstant.siteURL.menus.listEmployee,
    canActivate: [AuthGuardService],
    loadChildren: () => import('app/staff/staff.module').then(m => m.StaffModule),
  },
  {
    path: AppConstant.siteURL.menus.pendingRequestIDP + '/:target/:subTarget',
    canActivate: [AuthGuardService],
    loadChildren: () => import('app/approval-page/approval-page.module').then(m => m.ApprovalPageModule),
  },
  {
    path: AppConstant.siteURL.menus.pendingRequestIDP + '/:target',
    canActivate: [AuthGuardService],
    loadChildren: () => import('app/approval-page/approval-page.module').then(m => m.ApprovalPageModule),
  },
  {
    path: AppConstant.siteURL.menus.pendingRequestIDP,
    canActivate: [AuthGuardService],
    loadChildren: () => import('app/approval-page/approval-page.module').then(m => m.ApprovalPageModule),
  },
  {
    path: AppConstant.siteURL.menus.pendingRequestODP + '/:target/:subTarget',
    canActivate: [AuthGuardService],
    loadChildren: () => import('app/approval-page/approval-page.module').then(m => m.ApprovalPageModule),
  },
  {
    path: AppConstant.siteURL.menus.pendingRequestODP + '/:target',
    canActivate: [AuthGuardService],
    loadChildren: () => import('app/approval-page/approval-page.module').then(m => m.ApprovalPageModule),
  },
  {
    path: AppConstant.siteURL.menus.pendingRequestODP,
    canActivate: [AuthGuardService],
    loadChildren: () => import('app/approval-page/approval-page.module').then(m => m.ApprovalPageModule),
  },
  {
    path: AppConstant.siteURL.menus.adhocNominations,
    canActivate: [AuthGuardService],
    loadChildren:
      // tslint:disable-next-line:max-line-length
      () => import('app/organisational-development/learning-plan-detail/learning-plan-content/key-learning-program/planned-pdo-detail/adhoc-nominations/adhoc-nominattions.module').then(m => m.AdhocNominationsModule),
  },
  {
    path: AppConstant.siteURL.menus.report,
    canActivate: [AuthGuardService],
    loadChildren: () => import('app/report-page/report-page.module').then(m => m.ReportPageModule),
  },
  {
    path: AppConstant.siteURL.menus.teamCalendar,
    canActivate: [AuthGuardService],
    loadChildren: () => import('app/team-calendar/team-calendar.module').then(m => m.TeamCalendarModule),
  },
  {
    path: AppConstant.siteURL.menus.myCalendar,
    canActivate: [AuthGuardService],
    component: MyCalendarComponent,
  },
  {
    path: AppConstant.siteURL.menus.reviewMPJ,
    canActivate: [AuthGuardService],
    component: ReviewMyProfessionalDevelopmentJourneyComponent,
  },
  {
    path: AppConstant.siteURL.menus.odp,
    canActivate: [AuthGuardService],
    loadChildren:
      () => import('app/organisational-development/organisational-development.module').then(m => m.OrganisationalDevelopmentModule),
  },
  {
    path: '**',
    redirectTo: AppConstant.siteURL.login,
  },
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes, {
      preloadingStrategy: SelectiveStrategyService,
      onSameUrlNavigation: 'reload',
    }),
  ],
  providers: [SelectiveStrategyService],
  exports: [RouterModule],
})
export class AppRoutingModule {}
