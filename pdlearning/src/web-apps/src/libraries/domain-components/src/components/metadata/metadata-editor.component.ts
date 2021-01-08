import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import { MetadataTagGroupCode, MetadataTagModel, ResourceModel, SearchTag } from '@opal20/domain-api';
import { Observable, combineLatest } from 'rxjs';

import { CheckedState } from '@progress/kendo-angular-treeview';
import { DigitalContentDetailMode } from '../../models/digital-content-detail-mode.model';
import { MetadataEditorService } from '../../services/metadata-editor.service';
import { ResourceMetadataFormModel } from '../../models/resource-metadata-form.model';
import { Validators } from '@angular/forms';
import { mainSubjectRequireValidator } from '../../validators/main-subject-require-validator';
import { map } from 'rxjs/operators';
import { requiredForListValidator } from '@opal20/common-components';

@Component({
  selector: 'metadata-editor',
  templateUrl: './metadata-editor.component.html'
})
export class MetadataEditorComponent extends BaseFormComponent {
  public DigitalContentDetailMode: typeof DigitalContentDetailMode = DigitalContentDetailMode;
  public subjectAreaSelectItems: MetadataTagModel[] | undefined;
  @Input()
  public isFromDigitalContent: boolean = false;
  @Input()
  public mode: DigitalContentDetailMode = DigitalContentDetailMode.Edit;

  public metadataTags: MetadataTagModel[] | undefined;
  public resource: ResourceModel | undefined;
  public resourceId: string | undefined;
  public metadataTagGroupCode: typeof MetadataTagGroupCode = MetadataTagGroupCode;
  public resourceMetadataForm: ResourceMetadataFormModel | undefined;
  public developmentalRolesItemIsCheckedFn: (dataItem: MetadataTagModel, index: string) => CheckedState;
  public subjectAreasAndKeywordsItemIsCheckedFn: (dataItem: MetadataTagModel, index: string) => CheckedState;
  public dimensionsAndAreasItemIsCheckedFn: (dataItem: MetadataTagModel, index: string) => CheckedState;
  public customSearchTagAddedFn: (searchTag: string) => Promise<SearchTag>;
  public syncResourceMetadataForm: () => void = Utils.debounce(() => {
    this.metadataEditorSvc.updateResourceMetadataForm(this.resourceMetadataForm);
  }, 300);
  public fetchSearchTagsFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<SearchTag[]>;
  public fetchSearchTagsByNamesFn: (names: string[]) => Observable<SearchTag[]>;
  constructor(moduleFacadeService: ModuleFacadeService, private metadataEditorSvc: MetadataEditorService) {
    super(moduleFacadeService);
    this.metadataEditorSvc.resourceId$.pipe(this.untilDestroy()).subscribe(data => {
      this.resourceId = data;
      if (this.resource == null || this.resource.resourceId !== this.resourceId) {
        this.metadataEditorSvc
          .loadResource()
          .pipe(this.untilDestroy())
          .subscribe();
      }
    });
    this.metadataEditorSvc.metadataTags$.pipe(this.untilDestroy()).subscribe(data => {
      this.metadataTags = data;
    });

    combineLatest([
      this.metadataEditorSvc.resource$.pipe(this.untilDestroy()),
      this.metadataEditorSvc.digitalContentAutoSaveInformer$.pipe(this.untilDestroy())
    ]).subscribe(([resourceData, isAutoSaveRequest]) => {
      if (!isAutoSaveRequest) {
        this.resource = resourceData;
      }
    });
    this.metadataEditorSvc.resourceMetadataForm$.pipe(this.untilDestroy()).subscribe(data => {
      const clonedData = Utils.clone(data, _ => {
        _.resource = Utils.cloneDeep(_.resource);
      });
      this.resourceMetadataForm = clonedData;
      if (data && (!this.subjectAreaSelectItems || (this.subjectAreaSelectItems && this.subjectAreaSelectItems.length === 0))) {
        this.subjectAreaSelectItems = this.resourceMetadataForm.subjectAreaSelectItems;
      }
    });
    this.developmentalRolesItemIsCheckedFn = this.tagTvItemIsCheckedFnFactory(() => {
      return this.resourceMetadataForm.developmentalRoles;
    });
    this.subjectAreasAndKeywordsItemIsCheckedFn = this.tagTvItemIsCheckedFnFactory(() => {
      return this.resourceMetadataForm.subjectAreasAndKeywords;
    });
    this.dimensionsAndAreasItemIsCheckedFn = this.tagTvItemIsCheckedFnFactory(() => {
      return this.resourceMetadataForm.dimensionsAndAreas;
    });
    this.customSearchTagAddedFn = (searchTag: string) => {
      this.resourceMetadataForm.resource.searchTags.push(searchTag);
      this.metadataEditorSvc.updateResourceMetadataForm(this.resourceMetadataForm);
      return new Promise(resolve => {
        resolve(
          new SearchTag({
            name: searchTag
          })
        );
      });
    };
    this.fetchSearchTagsByNamesFn = (searchTags: string[]) => {
      return this.metadataEditorSvc.getSearchTagByNames(searchTags);
    };
    this.fetchSearchTagsFn = (searchText: string, skipCount: number, maxResultCount: number) => {
      return this.metadataEditorSvc.querySearchTag(searchText, skipCount, maxResultCount).pipe(map(response => response.items));
    };

    this.metadataEditorSvc
      .loadMetadataTags()
      .pipe(this.untilDestroy())
      .subscribe();
    this.metadataEditorSvc.setValidateFormFn(() => this.validate());
  }

  public ngOnDestroy(): void {
    super.ngOnDestroy();
    this.metadataEditorSvc.resetValidateFormFn();
  }

  public tagTvItemIsCheckedFnFactory(checkedKeysFn: () => string[]): (dataItem: MetadataTagModel, index: string) => CheckedState {
    return (dataItem: MetadataTagModel, index: string) => {
      if (
        checkedKeysFn().indexOf(dataItem.tagId) > -1 ||
        (dataItem.childs !== undefined &&
          dataItem.childs.length > 0 &&
          Utils.includesAll(checkedKeysFn(), dataItem.childs.map(p => p.tagId)))
      ) {
        return 'checked';
      }

      if (this.tagTvItemIsIndeterminate(dataItem.childs, checkedKeysFn)) {
        return 'indeterminate';
      }

      return 'none';
    };
  }
  public handleMainSubjectAreaFilter(value: string): void {
    this.subjectAreaSelectItems = value
      ? this.resourceMetadataForm.subjectAreaSelectItems.filter(s => s.displayText.toLowerCase().indexOf(value.toLowerCase()) !== -1)
      : this.resourceMetadataForm.subjectAreaSelectItems;
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'form',
      controls: {
        pdOpportunityType: {
          defaultValue: undefined,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required'
            }
          ]
        },
        serviceSchemes: {
          defaultValue: [],
          validators: [
            {
              validator: requiredForListValidator(),
              validatorType: 'required'
            }
          ]
        },
        mainSubjectArea: {
          defaultValue: undefined,
          validators: [
            {
              validator: mainSubjectRequireValidator(
                () => this.resourceMetadataForm.subjectAreaSelectItems.length > 0,
                Validators.required
              ),
              validatorType: 'required'
            }
          ]
        },
        courseLevels: {
          defaultValue: []
        },
        developmentalRoles: {
          defaultValue: []
        },
        preRequisites: {
          defaultValue: undefined
        },
        learningFrameworks: {
          defaultValue: []
        },
        searchTags: {
          defaultValue: []
        },
        objectivesOutCome: {
          defaultValue: undefined
        }
      }
    };
  }

  private tagTvItemIsIndeterminate(itemChilds: MetadataTagModel[] | undefined, checkedKeysFn: () => string[]): boolean {
    if (itemChilds === undefined) {
      return false;
    }
    let idx = 0;
    let item: MetadataTagModel;

    while ((item = itemChilds[idx])) {
      if (this.tagTvItemIsIndeterminate(item.childs, checkedKeysFn) || checkedKeysFn().indexOf(item.tagId) > -1) {
        return true;
      }

      idx += 1;
    }

    return false;
  }
}
