import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService, TranslationMessage } from '@opal20/infrastructure';
import { Component, ElementRef, EventEmitter, Input, Output, ViewChild, ViewEncapsulation } from '@angular/core';
import {
  CopyrightFormComponent,
  DigitalContentDetailMode,
  DigitalContentDetailViewModel,
  MetadataEditorComponent,
  MetadataEditorService,
  PermissionsTermsOfUse,
  ResourceMetadataFormModel
} from '@opal20/domain-components';
import { Course, DigitalContentStatus, IResourceModel, MetadataTagModel, ResourceType } from '@opal20/domain-api';
import { OpalDialogService, ScrollableMenu, ifValidator, requiredIfValidator, startEndValidator } from '@opal20/common-components';

import { CopyMetadataDialogComponent } from './dialogs/copy-metadata-dialog.component';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { Validators } from '@angular/forms';

enum AdditionalInformationTab {
  General = 't-general',
  Metadata = 't-metadata',
  Rights = 't-rights'
}

@Component({
  selector: 'digital-additional-information-tab',
  templateUrl: './digital-additional-information-tab.component.html',
  encapsulation: ViewEncapsulation.None
})
export class DigitalAdditionalInformationTabComponent extends BaseFormComponent {
  @ViewChild('generalTab', { static: false })
  public generalTab: ElementRef;

  @ViewChild('metadataTab', { static: false })
  public metadataTab: ElementRef;

  @ViewChild('rightsTab', { static: false })
  public rightsTab: ElementRef;

  public tabs: ScrollableMenu[] = [
    {
      id: AdditionalInformationTab.General,
      title: 'General',
      elementFn: () => {
        return this.generalTab;
      }
    },
    {
      id: AdditionalInformationTab.Metadata,
      title: 'Metadata',
      elementFn: () => {
        return this.metadataTab;
      }
    },
    {
      id: AdditionalInformationTab.Rights,
      title: 'Rights ',
      elementFn: () => {
        return this.rightsTab;
      }
    }
  ];
  public DigitalContentDetailMode: typeof DigitalContentDetailMode = DigitalContentDetailMode;

  @Input() public contentViewModel: DigitalContentDetailViewModel;
  @Input() public mode: DigitalContentDetailMode;
  @Output() public onLoad: EventEmitter<{
    copyrightForm: CopyrightFormComponent;
    metadataEditor: MetadataEditorComponent;
  }> = new EventEmitter();

  @ViewChild(CopyrightFormComponent, { static: true })
  private copyrightForm: CopyrightFormComponent | undefined;
  @ViewChild(MetadataEditorComponent, { static: true })
  private metadataEditor: MetadataEditorComponent | undefined;

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private opalDialogService: OpalDialogService,
    private metadataEditorSvc: MetadataEditorService
  ) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.onLoad.emit({ copyrightForm: this.copyrightForm, metadataEditor: this.metadataEditor });
  }

  public patchAdditionalInfoFormValue(contentViewModel: DigitalContentDetailViewModel): void {
    this.patchInitialFormValue({
      description: contentViewModel.description,
      primaryApprovingOfficerId: contentViewModel.primaryApprovingOfficerId,
      alternativeApprovingOfficerId: contentViewModel.alternativeApprovingOfficerId,
      autoPublishDate: contentViewModel.autoPublishDate,
      isAutoPublish: contentViewModel.isAutoPublish
    });
  }

  public onArchiveDateInputChange(date: Date): void {
    this.contentViewModel.archiveDate = date;
  }

  public displaySearchCourseDialog(): void {
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(CopyMetadataDialogComponent);
    this.subscribe(dialogRef.result, (data: Course) => {
      if (data && data instanceof Course) {
        const resource: IResourceModel = {
          resourceId: this.contentViewModel.id,
          resourceType: ResourceType.Content,
          mainSubjectAreaTagId: data.pdAreaThemeId,
          tags: this.getMetadataTagsFromCourse(data, this.metadataEditor.resourceMetadataForm.metadataTagsDicByParentTagId),
          searchTags: data.metadataKeys,
          objectivesOutCome: data.courseObjective
        };
        const resourceFormMetadata = new ResourceMetadataFormModel(resource, this.metadataEditor.metadataTags);
        this.metadataEditorSvc.updateResourceMetadataForm(resourceFormMetadata);
        // reload main subject area items
        this.metadataEditor.handleMainSubjectAreaFilter(null);
        this.contentViewModel.data.startDate = undefined;
        this.contentViewModel.data.expiredDate = undefined;
        this.contentViewModel.data.publisher = data.copyrightOwner;
        this.contentViewModel.data.acknowledgementAndCredit = data.acknowledgementAndCredit;
        this.contentViewModel.data.remarks = data.copyrightOwner ? data.remarks : null;
        this.contentViewModel.data.termsOfUse = this.getTermOfUse(data);
        this.copyrightForm.updateCopyrightFormData(this.contentViewModel.data);
      }
    });
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'form',
      controls: {
        description: {
          defaultValue: null
        },
        primaryApprovingOfficerId: {
          defaultValue: null,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required'
            }
          ]
        },
        alternativeApprovingOfficerId: {
          defaultValue: null,
          validators: null
        },
        autoPublishDate: {
          defaultValue: null,
          validators: [
            {
              validator: ifValidator(
                () => {
                  return this.contentViewModel.status === DigitalContentStatus.Draft;
                },
                () => startEndValidator('validAutoPublishDate', p => new Date(), p => p.value, false, 'dateOnly')
              ),
              validatorType: 'validAutoPublishDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'The Publish Schedule Date cannot be in the past')
            },
            {
              validator: requiredIfValidator(p => !this.contentViewModel.isAutoPublish),
              validatorType: 'required'
            }
          ]
        },
        isAutoPublish: {
          defaultValue: null,
          validators: null
        },
        archiveDate: {
          defaultValue: null,
          validators: [
            {
              validator: ifValidator(
                () => {
                  return this.contentViewModel.status === DigitalContentStatus.Draft;
                },
                () => startEndValidator('validArchiveDate', p => new Date(), p => p.value, false, 'dateOnly')
              ),
              validatorType: 'validArchiveDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'The Archival Date cannot be in the past')
            }
          ]
        }
      }
    };
  }

  private getMetadataTagsFromCourse(course: Course, metadataTagsDicByParentTagId: Dictionary<MetadataTagModel[]>): string[] {
    let tags = [
      course.courseLevel,
      ...course.serviceSchemeIds,
      ...course.developmentalRoleIds,
      ...course.subjectAreaIds,
      ...course.learningFrameworkIds,
      ...course.learningSubAreaIds
    ];
    tags = tags.concat(this.getAllChildTags(course.subjectAreaIds, metadataTagsDicByParentTagId));
    const selectLearningAreaIds = [];
    course.learningAreaIds.forEach(id => {
      const childs = metadataTagsDicByParentTagId[id];
      if (childs === undefined || childs.every(child => course.learningSubAreaIds.includes(child.id))) {
        selectLearningAreaIds.push(id);
      }
    });
    tags = tags.concat(selectLearningAreaIds);
    course.learningDimensionIds.forEach(id => {
      const childs = metadataTagsDicByParentTagId[id];
      if (childs === undefined || childs.every(child => selectLearningAreaIds.includes(child.id))) {
        tags.push(id);
      }
    });
    return tags;
  }

  private getAllChildTags(tags: string[], metadataTagsDicByParentTagId: Dictionary<MetadataTagModel[]>): string[] {
    let result = [];
    tags.forEach(tag => {
      const childs = metadataTagsDicByParentTagId[tag];
      if (childs && childs.length > 0) {
        const childIds = childs.map(child => child.id);
        result = result.concat(childIds);
        result = result.concat(this.getAllChildTags(childIds, metadataTagsDicByParentTagId));
      }
    });
    return result;
  }

  private getTermOfUse(course: Course): string {
    if (course.allowPersonalDownload) {
      return PermissionsTermsOfUse.DownloadForPersonal;
    } else if (course.allowNonCommerReuseWithModification) {
      return PermissionsTermsOfUse.WithModification;
    } else if (course.allowNonCommerReuseWithoutModification) {
      return PermissionsTermsOfUse.WithoutModification;
    } else if (course.allowNonCommerInMOEReuseWithModification) {
      return PermissionsTermsOfUse.WithModificationInMOE;
    } else if (course.allowNonCommerInMoeReuseWithoutModification) {
      return PermissionsTermsOfUse.WithoutModificationInMOE;
    }
  }
}
