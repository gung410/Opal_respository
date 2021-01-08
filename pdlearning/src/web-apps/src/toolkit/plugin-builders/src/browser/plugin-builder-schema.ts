import { Schema } from '@angular-devkit/build-angular/src/browser/schema';

export interface IPluginBuilderSchema extends Schema {
  /**
   * The entry path. E.g., @angular/core, ./app/main.
   */
  path: string;

  /**
   * The angular module path like loadChildren. E.g., app.module#AppModule
   */
  ngmodule: string;

  /**
   * The plugin name. E.g., ng.core.
   */
  name: string;
}
