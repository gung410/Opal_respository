import { AuthDataService, AuthService, OAuthService, UrlHelperService } from '@opal20/authentication';
import {
  BaseModuleOutlet,
  BaseRoutingModule,
  Fragment,
  FunctionModule,
  MODULE_INPUT_DATA,
  ModuleDataService,
  ModuleFacadeService,
  NAVIGATION_PARAMETERS_KEY,
  TranslationModule
} from '@opal20/infrastructure';
import { CalendarDomainApiModule, CalendarIntergrationService } from '@opal20/domain-api';
import { DatePickerModule, TimePickerModule } from '@progress/kendo-angular-dateinputs';
import { DomainComponentsModule, NavigationMenuService } from '@opal20/domain-components';
import { LayoutModule, TabStripModule } from '@progress/kendo-angular-layout';
import { NgModule, Type } from '@angular/core';
import { Observable, of } from 'rxjs';
import { Router, RouterModule } from '@angular/router';

import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { CalendarComponent } from './components/calendar.component';
import { CalendarDialogRefService } from './services/calendar-dialog-ref.service';
import { CalendarPlayerComponent } from './components/calendar-player/calendar-player.component';
import { CalendarRoutePaths } from './calendar.config';
import { CalendarRoutingModule } from './calendar-routing.module';
import { CalendarViewTemplateComponent } from './components/personal-calendar-view-template/calendar-view-template.component';
import { CommonComponentsModule } from '@opal20/common-components';
import { CommunitiesCalendarComponent } from './components/communities-calendar/communities-calendar.component';
import { CommunityEventDetailFormComponent } from './components/community-event-detail-form/community-event-detail-form.component';
import { CommunityEventRegularTemplateComponent } from './components/community-event-regular-template/community-event-regular-template.component';
import { CommunityEventWebinarTemplateComponent } from './components/community-event-webinar-template/community-event-webinar-template.component';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { EventTooltipComponent } from './components/event-tooltip/event-tooltip.component';
import { GridModule } from '@progress/kendo-angular-grid';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { OpalCalendarOutletComponent } from './components/calendar-outlet/calendar-outlet.component';
import { PersonalCalendarComponent } from './components/personal-calendar/personal-calendar.component';
import { PersonalCalendarContainerComponent } from './components/personal-calendar-container/personal-calendar-container.component';
import { PersonalCalendarWidgetComponent } from './components/personal-calendar-widget/personal-calendar-widget.component';
import { PersonalEventDetailFormComponent } from './components/personal-event-detail-form/personal-event-detail-form.component';
import { SchedulerModule } from '@progress/kendo-angular-scheduler';
import { ShareCalendarComponent } from './components/share-calendar/share-calendar.component';
import { SpecificCommunityCalendarComponent } from './components/specific-community-calendar/specific-community-calendar.component';
import { TeamCalendarComponent } from './components/team-calendar/team-calendar.component';
import { TeamCalendarContainerComponent } from './components/team-calendar-container/team-calendar-container.component';
import { TeamCalendarViewTemplateComponent } from './components/team-calendar-view-template/team-calendar-view-template.component';
import { TooltipModule } from '@progress/kendo-angular-tooltip';
import { TreeViewModule } from '@progress/kendo-angular-treeview';
// TODO: Move calendar services/models/... to domain-api
@NgModule({
  imports: [
    FunctionModule,
    CalendarRoutingModule,
    RouterModule.forRoot([]),
    CommonComponentsModule.forRoot(),
    DomainComponentsModule.forRoot(),
    DropDownsModule,
    SchedulerModule,
    TimePickerModule,
    DatePickerModule,
    TabStripModule,
    ButtonsModule,
    LayoutModule,
    TreeViewModule,
    InputsModule,
    TooltipModule,
    GridModule,
    CalendarDomainApiModule,
    TranslationModule.registerModules([{ moduleId: 'calendar' }])
  ],
  providers: [AuthService, UrlHelperService, CalendarDialogRefService],
  declarations: [
    CalendarComponent,
    PersonalCalendarComponent,
    TeamCalendarComponent,
    CalendarPlayerComponent,
    CommunitiesCalendarComponent,
    SpecificCommunityCalendarComponent,
    PersonalEventDetailFormComponent,
    CommunityEventDetailFormComponent,
    EventTooltipComponent,
    CalendarViewTemplateComponent,
    CommunityEventRegularTemplateComponent,
    CommunityEventWebinarTemplateComponent,
    TeamCalendarContainerComponent,
    ShareCalendarComponent,
    OpalCalendarOutletComponent,
    TeamCalendarViewTemplateComponent,
    PersonalCalendarContainerComponent,
    PersonalCalendarWidgetComponent
  ],
  entryComponents: [
    PersonalCalendarComponent,
    TeamCalendarComponent,
    CalendarPlayerComponent,
    CommunitiesCalendarComponent,
    SpecificCommunityCalendarComponent,
    PersonalEventDetailFormComponent,
    CommunityEventDetailFormComponent,
    EventTooltipComponent,
    CalendarViewTemplateComponent,
    CommunityEventRegularTemplateComponent,
    CommunityEventWebinarTemplateComponent,
    TeamCalendarContainerComponent,
    ShareCalendarComponent,
    OpalCalendarOutletComponent,
    TeamCalendarViewTemplateComponent,
    PersonalCalendarContainerComponent,
    PersonalCalendarWidgetComponent
  ],
  bootstrap: [CalendarComponent],
  exports: [CalendarComponent, CalendarPlayerComponent, PersonalCalendarWidgetComponent]
})
export class CalendarModule extends BaseRoutingModule {
  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private oAuthService: OAuthService,
    private authDataService: AuthDataService,
    protected router: Router,
    protected navigationMenuService: NavigationMenuService,
    private authService: AuthService,
    private calendarIntergrationSrv: CalendarIntergrationService
  ) {
    super(moduleFacadeService, router);

    // we are not using oidc but just oauth2 password flow so we bypass validations for oidc
    let extId: string;
    this.oAuthService.skipSubjectCheck = true;
    Promise.resolve()
      .then(() => this.calendarIntergrationSrv.verifyIntergrationUrl())
      .then(() => this.oAuthService.loadDiscoveryDocument())
      .then(() => this.oAuthService.loadUserProfile())
      .then(userProfile => {
        // tslint:disable-next-line:no-string-literal
        extId = userProfile['sub'];
        return this.authDataService.getUserProfileAsync(extId);
      })
      .then(user => {
        AppGlobal.user = user;
        // tslint:disable-next-line:no-string-literal
        AppGlobal.user['extId'] = extId;
        this.calendarIntergrationSrv.currentUser.next(AppGlobal.user);
        this.setupGlobalIntergrations();
      });

    this.navigationMenuService.init(
      (menuId, parameters, skipLocationChange) =>
        this.moduleFacadeService.navigationService.navigateTo(menuId, parameters, skipLocationChange),
      []
    );
  }

  private setupGlobalIntergrations(): void {
    AppGlobal.calendarIntergrations.refreshAccessToken = (accessToken: string) => {
      this.authService.setAccessToken(accessToken);
    };
  }
  protected get outletType(): Type<BaseModuleOutlet> {
    return OpalCalendarOutletComponent;
  }

  protected get fragments(): { [position: string]: Type<Fragment> } {
    return {};
  }

  protected get defaultPath(): Observable<string> {
    const moduleData = this.getModuleData();
    this.moduleFacadeService.contextDataService.setData(NAVIGATION_PARAMETERS_KEY, moduleData);

    if (moduleData.createEvent || moduleData.editEvent) {
      return of(CalendarRoutePaths.CommunityEventDetail);
    }

    if (moduleData.teamCalendar) {
      return of(CalendarRoutePaths.TeamCalendar);
    }

    // Default as community player.
    return of(CalendarRoutePaths.CalendarPlayer);
  }

  private getModuleData(): {
    communityId?: string;
    eventType?: string;
    teamCalendar?: boolean;
    eventId?: string;
    editEvent?: boolean;
    createEvent?: boolean;
  } {
    const moduleDataService: ModuleDataService = this.moduleFacadeService.moduleDataService;
    return (moduleDataService && moduleDataService.getData(MODULE_INPUT_DATA)) || {};
  }
}
