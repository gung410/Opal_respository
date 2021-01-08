import { Compiler, Injectable, NgModuleFactory, Type } from '@angular/core';

import { ModuleInfo } from './shell.models';

@Injectable()
export class ModuleCompiler {
  constructor(private compiler: Compiler) {}

  public compileModule(moduleInfo: ModuleInfo): Promise<NgModuleFactory<unknown>> {
    if (AppGlobal.mode === 'aot') {
      return moduleInfo
        .loadNgModule()
        .then((loadedModule: System.Module) => <NgModuleFactory<unknown>>this.extractModule(loadedModule, 'ModuleNgFactory'));
    }

    return moduleInfo.loadNgModule().then((loadedModule: System.Module) => {
      const extractedModule: Type<unknown> = <Type<unknown>>this.extractModule(loadedModule, 'Module');

      return this.compiler.compileModuleAsync(extractedModule);
    });
  }

  private extractModule(loadedModule: System.Module, modulePattern: string): Type<unknown> | NgModuleFactory<unknown> {
    /// Extract module containing keyword
    const keys: string[] = Object.keys(loadedModule).filter(item => item.indexOf(modulePattern) >= 0);

    if (keys.length === 0) {
      throw new Error('Please check angular.manifest.json and export for module. E.g: export class ABCModule {}');
    }

    return loadedModule[keys[0]];
  }
}
