import { AppModule } from '../src/app.module';
import { REGISTRATION_MODULES } from './registration-modules';
import { enableProdMode } from '@angular/core';
import { initializeWindowContext } from './window-context';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

initializeWindowContext();

AppGlobal.registrationModules = REGISTRATION_MODULES.filter(m => !AppGlobal.environment.production || !m.development);

if (AppGlobal.environment.production) {
  enableProdMode();
}

platformBrowserDynamic()
  .bootstrapModule(AppModule)
  .catch(err => console.error(err));
