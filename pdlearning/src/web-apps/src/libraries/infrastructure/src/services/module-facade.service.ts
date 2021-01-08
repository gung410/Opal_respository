import { ContextDataService, ModuleDataService } from './context-data.service';
import { DialogService, WindowService } from '@progress/kendo-angular-dialog';
import { Inject, Injectable } from '@angular/core';

import { APP_BASE_HREF } from '@angular/common';
import { AppInfoService } from '../app-info/app-info.service';
import { FormBuilderService } from '../form/form-builder.service';
import { GlobalScheduleService } from './global-schedule.service';
import { GlobalSpinnerService } from '../spinner/global-spinner.service';
import { GlobalTranslatorService } from '../translation/global-translator.service';
import { LocalScheduleService } from './local-schedule.service';
import { LocalTranslatorService } from '../translation/local-translator.service';
import { ModalService } from './modal.service';
import { ModuleInstance } from '../shell/shell.models';
import { NavigationService } from './navigation.service';
import { NotificationService } from '@progress/kendo-angular-notification';
import { ShellManager } from '../shell/shell-manager';
import { SpinnerService } from '../spinner/spinner.service';

@Injectable()
export class ModuleFacadeService {
  constructor(
    // Global services
    public appInfoService: AppInfoService,
    public moduleInstance: ModuleInstance,
    public formBuilder: FormBuilderService,
    public modalService: ModalService,
    public notificationService: NotificationService,
    public shellManager: ShellManager,
    public globalDialogService: DialogService,
    public globalWindowService: WindowService,
    public globalTranslator: GlobalTranslatorService,
    public globalSpinnerService: GlobalSpinnerService,
    public globalScheduleService: GlobalScheduleService,
    @Inject(APP_BASE_HREF)
    public baseHref: string,
    // Local services
    public dialogService: DialogService,
    public windowService: WindowService,
    public translator: LocalTranslatorService,
    public spinnerService: SpinnerService,
    public navigationService: NavigationService,
    public moduleDataService: ModuleDataService,
    public contextDataService: ContextDataService,
    public localScheduleService: LocalScheduleService
  ) {}
}
