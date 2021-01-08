import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService } from '@opal20/infrastructure';
import {
  BrokenLinkContentType,
  BrokenLinkModuleIdentifier,
  BrokenLinkReportApiService,
  IReportBrokenLinkRequest
} from '@opal20/domain-api';
import { Component, Input } from '@angular/core';

import { DialogRef } from '@progress/kendo-angular-dialog';
import { Validators } from '@angular/forms';

@Component({
  selector: 'broken-link-report-dialog',
  templateUrl: './broken-link-report-dialog.component.html'
})
export class BrokenLinkReportDialogComponent extends BaseFormComponent {
  public isSubmit: boolean = false;

  @Input() public urlData: string[];
  @Input() public objectId: string;
  @Input() public originalObjectId: string;
  @Input() public module: BrokenLinkModuleIdentifier;
  @Input() public parentId: string;
  @Input() public objectDetailUrl: string;
  @Input() public objectOwnerId: string;
  @Input() public objectOwnerName: string;
  @Input() public objectTitle: string;
  @Input() public isPreviewMode: boolean = false;
  @Input() public contentType: BrokenLinkContentType;

  public description: string;
  public url: string;

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public dialogRef: DialogRef,
    public brokenLinkReportApiService: BrokenLinkReportApiService
  ) {
    super(moduleFacadeService);
  }

  public onCancel(): void {
    this.dialogRef.close();
  }

  public async onProceed(): Promise<void> {
    if (this.isPreviewMode) {
      this.isSubmit = true;
      return;
    }

    const formValid = await this.validate();
    if (formValid) {
      await this.brokenLinkReportApiService.reportBrokenLink(<IReportBrokenLinkRequest>{
        objectId: this.objectId,
        url: this.url,
        description: this.description,
        objectDetailUrl: this.objectDetailUrl,
        module: this.module,
        objectOwnerId: this.objectOwnerId,
        objectOwnerName: this.objectOwnerName,
        objectTitle: this.objectTitle,
        originalObjectId: this.originalObjectId,
        parentId: this.parentId,
        reporterName: this.getCurrentUserName(),
        contentType: this.contentType
      });
      this.isSubmit = true;
    }
  }

  public getCurrentUserName(): string {
    return `${AppGlobal.user.firstName || ''} ${AppGlobal.user.lastName || ''}`.trim();
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'form',
      controls: {
        url: {
          defaultValue: null,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required'
            }
          ]
        },
        description: {
          defaultValue: null
        }
      }
    };
  }
}
