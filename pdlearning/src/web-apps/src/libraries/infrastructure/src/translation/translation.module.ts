import { GLOBAL_MODULE_INFO_COLLECTION, IModuleInfo, LOCAL_MODULE_INFO_COLLECTION, TRANSLATION_LOADER } from './translation.models';
import { LocalizationService, createTranslationLoader } from './translation-loader';
import { ModuleWithProviders, NgModule } from '@angular/core';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';

import { GlobalTranslatorPipe } from './pipes/global-translator.pipe';
import { GlobalTranslatorService } from './global-translator.service';
import { LocalTranslatorPipe } from './pipes/local-translator.pipe';

@NgModule({})
export class TranslationModule {
  public static registerCommons(moduleInfoCollection: IModuleInfo[] = []): ModuleWithProviders {
    return {
      ngModule: GlobalTranslationModule,
      providers: [
        {
          provide: GLOBAL_MODULE_INFO_COLLECTION,
          useValue: moduleInfoCollection,
          multi: true
        }
      ]
    };
  }

  public static registerModules(moduleInfoCollection: IModuleInfo[] = []): ModuleWithProviders {
    return {
      ngModule: LocalTranslationModule,
      providers: [
        {
          provide: LOCAL_MODULE_INFO_COLLECTION,
          useValue: moduleInfoCollection,
          multi: true
        }
      ]
    };
  }
}

@NgModule({})
export class InternalTranslationModule {
  public static forRoot(moduleInfoCollection: IModuleInfo[] = []): ModuleWithProviders {
    return {
      ngModule: GlobalTranslationModule,
      providers: [
        {
          provide: GLOBAL_MODULE_INFO_COLLECTION,
          useValue: moduleInfoCollection,
          multi: true
        },
        {
          provide: TRANSLATION_LOADER,
          useFactory: createTranslationLoader,
          deps: [LocalizationService, GLOBAL_MODULE_INFO_COLLECTION]
        },
        GlobalTranslatorService
      ]
    };
  }
}

@NgModule({
  declarations: [GlobalTranslatorPipe],
  providers: [LocalizationService],
  exports: [GlobalTranslatorPipe]
})
export class BaseTranslationModule {}

@NgModule({
  imports: [
    BaseTranslationModule,
    TranslateModule.forRoot({
      loader: { provide: TranslateLoader, useExisting: TRANSLATION_LOADER },
      isolate: false
    })
  ],
  exports: [BaseTranslationModule]
})
export class GlobalTranslationModule {}

@NgModule({
  imports: [BaseTranslationModule],
  declarations: [LocalTranslatorPipe],
  exports: [BaseTranslationModule, LocalTranslatorPipe]
})
export class LocalTranslationModule {}
