import { GlobalTranslatorService } from './global-translator.service';
import { InjectionToken } from '@angular/core';
import { LocalTranslatorService } from './local-translator.service';
import { TranslationLoader } from './translation-loader';

export interface IModuleInfo {
  moduleId: string;
  moduleType?: 'Function' | 'Shared';
}

export interface IResource {
  [key: string]: string;
}

export interface ITranslationParams {
  [key: string]: string | number;
}

export class TranslationMessage {
  constructor(
    public translator: GlobalTranslatorService | LocalTranslatorService,
    public key: string,
    public params?: ITranslationParams
  ) {}

  public toString(): string {
    return this.translator.translate(this.key, this.params);
  }
}

export const TRANSLATION_LOADER: InjectionToken<TranslationLoader> = new InjectionToken('fw.translator-loader');
export const GLOBAL_MODULE_INFO_COLLECTION: InjectionToken<IModuleInfo[][]> = new InjectionToken('fw.global-module-info-collection');
export const LOCAL_MODULE_INFO_COLLECTION: InjectionToken<IModuleInfo[][]> = new InjectionToken('fw.local-module-info-collection');
