import { Component } from '@angular/core';
import { ModuleFacadeService } from '@opal20/infrastructure';

@Component({
  selector: 'poc-app',
  template: `
    <div class="module-navigation">
      Features:
      <a
        class="module-navigation-item"
        *ngFor="let config of moduleFacadeService.moduleInstance.router.config"
        (click)="moduleFacadeService.navigationService.navigateTo(config.path)"
      >
        {{ config.path.toUpperCase() }}
      </a>
    </div>
    <router-outlet></router-outlet>
  `
})
export class POCComponent {
  constructor(public moduleFacadeService: ModuleFacadeService) {}
}
