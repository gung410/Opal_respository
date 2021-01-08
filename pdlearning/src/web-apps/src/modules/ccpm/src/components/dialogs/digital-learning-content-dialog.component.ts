import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, ViewChild, ViewEncapsulation } from '@angular/core';
import { ContentApiService, DigitalLearningContentRequest, IDigitalContent, ResourceType } from '@opal20/domain-api';
import {
  ContentDialogComponent,
  ContentDialogComponentTabType,
  CopyrightFormComponent,
  MetadataEditorComponent,
  MetadataEditorService
} from '@opal20/domain-components';

import { DialogRef } from '@progress/kendo-angular-dialog';
import { Validators } from '@angular/forms';
import { XmlEntities } from 'html-entities';
import { validateExpiryDate } from '../../validators/expiry-date-validator';

@Component({
  selector: 'digital-learning-content-dialog',
  templateUrl: './digital-learning-content-dialog.component.html',
  encapsulation: ViewEncapsulation.None
})
export class DigitalLearningContentDialog extends BaseFormComponent {
  public get learningContentId(): string | undefined {
    return this.learningContent !== undefined ? this.learningContent.id : undefined;
  }
  @ViewChild(CopyrightFormComponent, { static: false })
  public copyrightForm: CopyrightFormComponent | undefined;
  @ViewChild(ContentDialogComponent, { static: true })
  public contentDialog: ContentDialogComponent;
  @ViewChild(MetadataEditorComponent, { static: false })
  public metadataEditor: MetadataEditorComponent | undefined;

  public learningContent: IDigitalContent | undefined;
  public activeTab: ContentDialogComponentTabType = ContentDialogComponentTabType.General;
  public hasErrorTabs: ContentDialogComponentTabType[] = [];

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private dialogRef: DialogRef,
    private contentApiService: ContentApiService,
    private metadataEditorSvc: MetadataEditorService
  ) {
    super(moduleFacadeService);
  }

  public initData(): void {
    if (this.learningContent) {
      this.patchInitialFormValue({
        title: this.learningContent.title,
        description: this.learningContent.description
      });
    }
    this.metadataEditorSvc.setResourceInfo(this.learningContentId, ResourceType.Content);
  }

  public checkAndCancel(): void {
    if (this.isFormDirty()) {
      this.performCheckAndCancel();

      return;
    }

    this.performCancel();
  }

  public disabledExpiryDate(value: Date): boolean {
    return !validateExpiryDate(value);
  }

  protected performCancel(): void {
    this.dialogRef.close();
  }

  protected additionalCanSaveCheck(controls?: string[]): Promise<boolean> {
    return Promise.all([this.copyrightForm.validate(), this.metadataEditor.validate()]).then(_ => _.findIndex(p => p !== true) < 0);
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    const result: IFormBuilderDefinition = {
      formName: 'form',
      controls: {
        title: {
          defaultValue: '',
          validators: [
            {
              validator: Validators.required
            },
            {
              validator: Validators.maxLength(255)
            }
          ]
        },
        description: {
          defaultValue: ''
        }
      }
    };
    return result;
  }

  protected saveData(): void {
    const copyRightData = this.copyrightForm.getCopyrightData();
    const model: IDigitalContent = {
      title: this.form.value.title,
      description: new XmlEntities().encode(this.form.value.description),
      ...copyRightData
    };

    if (!this.learningContent) {
      this.contentApiService
        .createDigitalContent(new DigitalLearningContentRequest(model))
        .then(_ => this.metadataEditorSvc.saveCurrentResourceMetadataForm(_.id, ResourceType.Content).toPromise())
        .then(response => {
          this.dialogRef.close(response.resourceId);
        });
    } else {
      model.id = this.learningContent.id;
      model.status = this.learningContent.status;
      model.htmlContent = new XmlEntities().encode(this.learningContent.htmlContent);

      this.contentApiService
        .updateDigitalContent(new DigitalLearningContentRequest(model))
        .then(_ => this.metadataEditorSvc.saveCurrentResourceMetadataForm().toPromise())
        .then(() => this.dialogRef.close(model));
    }
  }

  protected onCheckToSaveInvalid(): void {
    this.hasErrorTabs = this.getHasErrorTabs();
    this.contentDialog.showFirstErrorTab(this.hasErrorTabs);
  }

  private getHasErrorTabs(): ContentDialogComponentTabType[] {
    const result: ContentDialogComponentTabType[] = [];
    if (this.form && this.form.invalid) {
      result.push(ContentDialogComponentTabType.General);
    }
    if (this.metadataEditor && this.metadataEditor.form.invalid) {
      result.push(ContentDialogComponentTabType.MetaData);
    }
    if (this.copyrightForm && this.copyrightForm.form.invalid) {
      result.push(ContentDialogComponentTabType.DigitalRight);
    }
    return result;
  }
}
