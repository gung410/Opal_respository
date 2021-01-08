import {
  ComponentFactory,
  ComponentRef,
  Injector,
  NgModuleFactory,
  NgModuleRef,
  Renderer2,
  StaticProvider,
  Type,
  ViewChild,
  ViewContainerRef
} from '@angular/core';
import { ContextDataService, ModuleDataService } from '../services/context-data.service';
import { DialogService, WindowService } from '@progress/kendo-angular-dialog';
import {
  FakeMissingTranslationHandler,
  MissingTranslationHandler,
  TranslateCompiler,
  TranslateDefaultParser,
  TranslateFakeCompiler,
  TranslateLoader,
  TranslateParser,
  TranslateService,
  TranslateStore,
  USE_DEFAULT_LANG,
  USE_STORE
} from '@ngx-translate/core';
import { IModuleInfo, LOCAL_MODULE_INFO_COLLECTION } from '../translation/translation.models';
import { LocalizationService, TranslationLoader, createTranslationLoader } from '../translation/translation-loader';

import { APP_BASE_HREF } from '@angular/common';
import { AppInfoService } from '../app-info/app-info.service';
import { BaseModuleOutlet } from './outlets/base-module-outlet';
import { BrowserIdleHandler } from '../specials/browser-idle.handler';
import { ComponentPortal } from '../portal/portal';
import { FormBuilderService } from '../form/form-builder.service';
import { Fragment } from './fragment';
import { FragmentRegistry } from './fragment-registry';
import { FunctionModule } from '../function.module';
import { GlobalScheduleService } from '../services/global-schedule.service';
import { GlobalSpinnerService } from '../spinner/global-spinner.service';
import { GlobalTranslatorService } from '../translation/global-translator.service';
import { LocalScheduleService } from '../services/local-schedule.service';
import { LocalTranslatorService } from '../translation/local-translator.service';
import { ModalService } from '../services/modal.service';
import { ModuleCompiler } from './module-compiler';
import { ModuleFacadeService } from '../services/module-facade.service';
import { ModuleInstance } from './shell.models';
import { ModuleOutletComponent } from './outlets/module-outlet.component';
import { NavigationService } from '../services/navigation.service';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Router } from '@angular/router';
import { ScheduleService } from '../services/schedule.service';
import { ShellManager } from './shell-manager';
import { SpinnerService } from '../spinner/spinner.service';
import { SystemError } from '../errors';

// tslint:disable:no-any

/**
 * This is how we built the web application. Everything begins from this class!
 * Using dynamic bootstrapping module designed by ourself, we enhanced Angular framework the whole new level!
 * Thanks ThongNH7, KhanhBC, ToanNM4 for your contribution to this awesome architecture.
 *
 * The application overview:
 * _______________________________________________________________________
 * |______________________________________________________________________|
 * | FRAGMENT (Header)                                                    |
 * |______________________________________________________________________|
 * |______________________________________________________________________|
 * | FRAGMENT (Processing bar)                                            |
 * |______________________________________________________________________|
 * |______________________________________________________________________|
 * | FRAGMENT    |               FRAGMENT               |    FRAGMENT    |
 * | (Left Menu) |             (App toolbar)            | (Right Content) |
 * |_____________|______________________________________|                 |
 * |             |                                      |                 |
 * |             |               FRAGMENT               |                 |
 * |             | (Function Module will be loaded here)|                 |
 * |             |                                      |                 |
 * |             |                                      |                 |
 * |_____________|______________________________________|_________________|
 * |                     FRAGMENT (Bottom Content)                        |
 * |______________________________________________________________________|
 *
 * The application was composed by many pieces that we called FRAGMENT(s).
 * The most important fragment is the place we load Function Module into. Before loading module, we do some generic processes like:
 *  - Preparing translation resources.
 *  - Preparing common services that will be used by the function.
 *  - And more...
 *
 * Basically, when the Function (e.g., ModuleA) was loaded, everything was ready for the developer!
 * They can get multilang through methods provided by our base classes, they can get domain types information instantly, etc.
 */
export class AppShell {
  @ViewChild('mainFragment', { read: ViewContainerRef, static: false })
  public mainFragment: ViewContainerRef;

  constructor(
    public renderer: Renderer2,
    public injector: Injector,
    public fragmentRegistry: FragmentRegistry,
    public shellManager: ShellManager,
    public compiler: ModuleCompiler
  ) {
    this.shellManager.init(this);
  }

  public registerFragment(position: string, type: Type<Fragment>): void {
    this.fragmentRegistry.remove(position);
    this.fragmentRegistry.register(position, new ComponentPortal(type));
  }

  public unregisterFragment(position: string): void {
    this.fragmentRegistry.remove(position);
  }

  public unregisterAllFragments(): void {
    this.fragmentRegistry.empty();
  }

  public loadModule(
    moduleInstance: ModuleInstance,
    outletType: Type<BaseModuleOutlet>,
    providers?: StaticProvider[],
    parentInjector?: Injector,
    vCRef: ViewContainerRef = this.mainFragment,
    attachModuleInstanceId: boolean = true
  ): Promise<void> {
    return this.compiler.compileModule(moduleInstance.moduleInfo).then((ngModuleFactory: NgModuleFactory<any>) => {
      const translationProviders: StaticProvider[] = [
        {
          provide: LOCAL_MODULE_INFO_COLLECTION,
          useValue: []
        },
        {
          provide: TranslateLoader,
          useFactory: createTranslationLoader,
          deps: [LocalizationService, LOCAL_MODULE_INFO_COLLECTION]
        },
        { provide: TranslateCompiler, useClass: TranslateFakeCompiler, deps: [] },
        { provide: TranslateParser, useClass: TranslateDefaultParser, deps: [] },
        { provide: MissingTranslationHandler, useClass: FakeMissingTranslationHandler, deps: [] },
        { provide: USE_STORE, useValue: true },
        { provide: USE_DEFAULT_LANG, useValue: true },
        {
          provide: LocalTranslatorService,
          deps: [
            GlobalTranslatorService,
            TranslateStore,
            TranslateLoader,
            TranslateCompiler,
            TranslateParser,
            MissingTranslationHandler,
            USE_DEFAULT_LANG,
            USE_STORE
          ]
        },
        {
          provide: TranslateService,
          useExisting: LocalTranslatorService
        }
      ];

      const scheduleServiceProviders: StaticProvider[] = [
        {
          provide: LocalScheduleService,
          deps: [BrowserIdleHandler]
        },
        {
          provide: ScheduleService,
          useExisting: LocalScheduleService
        }
      ];
      const moduleInjector: Injector = Injector.create(
        [
          ...translationProviders,

          ...scheduleServiceProviders,

          // To add more providers when loading new module
          ...(providers || []),

          // The reason why we use forChild here because its scope isolated in every function/module
          // When module/function was destroyed, these providers should be detroyed as well
          ...(<StaticProvider[]>FunctionModule.forChild().providers),

          // Module instance was created using dynamic module loading
          // Therefor we need to provide dynamically as well
          { provide: ModuleInstance, useValue: moduleInstance },

          // This is very important to provide Facade Service here,
          // Because ModuleFacadeService must be resolved after translationProviders
          {
            provide: ModuleFacadeService,
            deps: [
              AppInfoService,
              ModuleInstance,
              FormBuilderService,
              ModalService,
              NotificationService,
              ShellManager,
              DialogService,
              WindowService,
              GlobalTranslatorService,
              GlobalSpinnerService,
              GlobalScheduleService,
              APP_BASE_HREF,
              DialogService,
              WindowService,
              LocalTranslatorService,
              SpinnerService,
              NavigationService,
              ModuleDataService,
              ContextDataService,
              LocalScheduleService
            ]
          }
        ],
        parentInjector || this.injector
      );
      const moduleRef: NgModuleRef<any> = ngModuleFactory.create(moduleInjector);

      moduleInstance.moduleRef = moduleRef;
      moduleInstance.facadeService = moduleRef.injector.get(ModuleFacadeService);
      moduleInstance.router = moduleRef.injector.get(Router, null);

      // Assign local services after module loaded.
      const moduleFacadeService: ModuleFacadeService = moduleRef.injector.get(ModuleFacadeService);
      moduleFacadeService.spinnerService = moduleRef.injector.get(SpinnerService);
      moduleFacadeService.dialogService = moduleRef.injector.get(DialogService);
      moduleFacadeService.windowService = moduleRef.injector.get(WindowService);

      return Promise.resolve()
        .then(() => this.initTranslation(moduleRef, moduleInstance))
        .then(() => this.bootstrapModule(moduleRef, moduleInstance, outletType, vCRef, attachModuleInstanceId));
    });
  }

  public destroyModule(instance: ModuleInstance): void {
    this.shellManager.moduleContext.clear();

    if (instance.outletComponentRef) {
      instance.outletComponentRef.destroy();
    }

    if (instance.moduleRef) {
      instance.moduleRef.destroy();
    }
  }

  private initTranslation(moduleRef: NgModuleRef<any>, instance: ModuleInstance): Promise<void> {
    const translateLoader: TranslationLoader = moduleRef.injector.get(TranslateLoader, null);
    const moduleInfoCollections: IModuleInfo[][] = moduleRef.injector.get(LOCAL_MODULE_INFO_COLLECTION, null);
    const localTranslator: LocalTranslatorService = moduleRef.injector.get(LocalTranslatorService);

    if (!translateLoader || !moduleInfoCollections || !localTranslator) {
      return Promise.resolve();
    }

    translateLoader.moduleInfoCollections = moduleInfoCollections;

    return localTranslator.init();
  }

  private bootstrapModule(
    moduleRef: NgModuleRef<any>,
    instance: ModuleInstance,
    outletType: Type<BaseModuleOutlet>,
    vCRef: ViewContainerRef = this.mainFragment,
    attachModuleInstanceId: boolean = true
  ): Promise<void> {
    const bootstrapComponents: Type<any>[] = (<any>moduleRef)._bootstrapComponents;

    if (bootstrapComponents.length !== 1) {
      throw new SystemError('Require one and only 1 bootstrap component per module!');
    }

    const bootstrapComponent: Type<any> = bootstrapComponents[0];
    const componentFactory: ComponentFactory<any> = moduleRef.componentFactoryResolver.resolveComponentFactory(
      moduleRef.instance.outletType || outletType || ModuleOutletComponent
    );
    const outletComponentRef: ComponentRef<any> = vCRef.createComponent(componentFactory);

    instance.outletComponentRef = outletComponentRef;
    instance.componentFactory = moduleRef.componentFactoryResolver.resolveComponentFactory(bootstrapComponent);

    /**
     * Fix the issue:
     * https://stackoverflow.com/questions/42387348/angular2-dynamic-content-loading-throws-expression-changed-exception
     */
    outletComponentRef.changeDetectorRef.detectChanges();

    if (attachModuleInstanceId) {
      this.renderer.setAttribute(outletComponentRef.location.nativeElement, 'data-module-instance-id', instance.id);
      this.shellManager.moduleContext.currentModuleInstance = instance;
    }

    instance.markAsLoaded();

    return Promise.resolve();
  }
}
