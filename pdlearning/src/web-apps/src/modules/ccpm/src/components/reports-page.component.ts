import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, HostBinding } from '@angular/core';
import { IOpalReportDynamicParams, OpalReportDynamicComponent } from '@opal20/domain-components';

@Component({
  selector: 'reports-page',
  templateUrl: './reports-page.component.html',
  styles: [
    `
      .content-management-container {
        flex-grow: 1;
      }
    `
  ]
})
export class ReportsPageComponent extends BasePageComponent {
  public paramsReportDynamic: IOpalReportDynamicParams | null;

  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  @HostBinding('class.home-module')
  public getContentClass(): boolean {
    return true;
  }

  public ngOnInit(): void {
    this.paramsReportDynamic = OpalReportDynamicComponent.buildCCPMReportList();
  }
}
