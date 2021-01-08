import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, ViewChild } from '@angular/core';

import { DialogRef } from '@progress/kendo-angular-dialog';
import { DropDownListComponent } from '@progress/kendo-angular-dropdowns';

@Component({
  selector: 'learning-package-confirmation-dialog',
  templateUrl: './learning-package-confirmation-dialog.component.html'
})
export class PackageConfirmDialog extends BaseComponent {
  @ViewChild('zipPackageControl', { static: false })
  public zipPackageControl: DropDownListComponent;
  public zipPackages: IDataItem[] = [
    {
      text: 'SCORM',
      value: 'scorm'
    }
  ];
  public fileName: string = '';

  constructor(public moduleFacadeService: ModuleFacadeService, public dialogRef: DialogRef) {
    super(moduleFacadeService);
  }

  public chooseZipPackage(): void {
    const selectedZipPackage = this.zipPackageControl.value;
    this.dialogRef.close(selectedZipPackage);
  }
}
