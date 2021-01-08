import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';

import { Component } from '@angular/core';
import { VersionTrackingViewModel } from '@opal20/domain-api';

@Component({
  selector: 'compare-version-content-dialog',
  templateUrl: './compare-version-content-dialog.component.html'
})
export class CompareVersionContentDialogComponent extends BaseComponent {
  public oldVersionTrackingVm: VersionTrackingViewModel | undefined;
  public newVersionTrackingVm: VersionTrackingViewModel | undefined;

  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public getVersionString(version: string): string {
    return version.replace('v', 'Version ');
  }
}
