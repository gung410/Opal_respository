import { DialogModule, WindowModule } from '@progress/kendo-angular-dialog';
import { ModuleWithProviders, NgModule, Type } from '@angular/core';

import { AmazonCloudfrontIframeComponent } from './amazon-s3-uploader/amazon-cloudfront-iframe.component';
import { AngularModule } from './angular.module';
import { AppInfoService } from './app-info/app-info.service';
import { AutoFocusDirective } from './directives/auto-focus.directive';
import { AutoTrimDirective } from './directives/auto-trim.directive';
import { CommonFacadeService } from './services/common-facade.service';
import { DurationInHAndM } from './pipes/duration-in-H-and-M.pipe';
import { DurationInHoursAndMinutes } from './pipes/duration-in-hours-and-minutes.pipe';
import { ErrorHightLightDirective } from './directives/error-hightlight.directive';
import { ErrorTooltipDirective } from './directives/error-tooltip.directive';
import { FileSizeDisplayPipe } from './pipes/file-size-display.pipe';
import { FormBuilderModule } from './form/form-builder.module';
import { FormGuard } from './form/guards/form-guard';
import { GlobalScheduleService } from './services/global-schedule.service';
import { GlobalTranslatorService } from './translation/global-translator.service';
import { HttpClientModule } from '@angular/common/http';
import { InterceptorModule } from './backend-service/interceptor.module';
import { InternalTranslationModule } from './translation/translation.module';
import { ModalService } from './services/modal.service';
import { NotificationModule } from '@progress/kendo-angular-notification';
import { PopupModule } from '@progress/kendo-angular-popup';
import { ReactiveFocusIntegrationDirective } from './directives/reactive-focus-integration.directive';
import { RelativeTimespanPipe } from './pipes/relative-timespan.pipe';
import { SpinnerModule } from './spinner/spinner.module';

const INFRASTRUCTURE_DIRECTIVES: Type<unknown>[] = [
  ErrorHightLightDirective,
  ErrorTooltipDirective,
  ReactiveFocusIntegrationDirective,
  AutoFocusDirective,
  AutoTrimDirective
];
const INFRASTRUCTURE_COMPONENTS: Type<unknown>[] = [AmazonCloudfrontIframeComponent];
const INFRASTRUCTURE_PIPES: Type<unknown>[] = [RelativeTimespanPipe, DurationInHoursAndMinutes, DurationInHAndM, FileSizeDisplayPipe];
const INFRASTRUCTURE_SERVICES: Type<unknown>[] = [
  AppInfoService,
  GlobalTranslatorService,
  GlobalScheduleService,
  CommonFacadeService,
  ModalService,
  FormGuard
];
const INFRASTRUCTURE_MODULES: Type<unknown>[] = [
  AngularModule,
  HttpClientModule,
  InterceptorModule,
  FormBuilderModule,
  DialogModule,
  WindowModule,
  NotificationModule,
  PopupModule
];

@NgModule({
  imports: [...INFRASTRUCTURE_MODULES, SpinnerModule.forChild()],
  declarations: [...INFRASTRUCTURE_DIRECTIVES, ...INFRASTRUCTURE_PIPES, ...INFRASTRUCTURE_COMPONENTS],
  providers: [],
  exports: [...INFRASTRUCTURE_DIRECTIVES, ...INFRASTRUCTURE_PIPES, ...INFRASTRUCTURE_COMPONENTS, ...INFRASTRUCTURE_MODULES, SpinnerModule]
})
export class InfrastructureModule {
  public static forRoot(): ModuleWithProviders {
    return {
      ngModule: RootInfrastructureModule,
      providers: [...INFRASTRUCTURE_SERVICES]
    };
  }
}

@NgModule({
  imports: [InfrastructureModule, SpinnerModule.forRoot(), InternalTranslationModule.forRoot([{ moduleId: 'common' }])],
  exports: [InfrastructureModule]
})
export class RootInfrastructureModule {}
