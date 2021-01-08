import { BaseComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import { CopyrightFormModel, MetadataEditorService, ResourceMetadataFormModel } from '@opal20/domain-components';
import { MetadataTagModel, ResourceType } from '@opal20/domain-api';

import { CheckedState } from '@progress/kendo-angular-treeview';
import { MyDigitalContentDetail } from '../models/my-digital-content-detail.model';

@Component({
  selector: 'learner-digital-content-more-detail',
  templateUrl: './learner-digital-content-more-detail.component.html'
})
export class DigitalContentMoreDetailComponent extends BaseComponent {
  public metadataDigitalContent: ResourceMetadataFormModel | undefined;
  public copyrightDigitalContent: CopyrightFormModel | undefined;
  public dimensionsAndAreasItemIsCheckedFn: (dataItem: MetadataTagModel, index: string) => CheckedState;
  public developmentalRolesItemIsCheckedFn: (dataItem: MetadataTagModel, index: string) => CheckedState;
  public subjectAreasAndKeywordsItemIsCheckedFn: (dataItem: MetadataTagModel, index: string) => CheckedState;

  @Input()
  public digitalContent: MyDigitalContentDetail | undefined;
  constructor(protected moduleFacadeService: ModuleFacadeService, private metadataEditorSvc: MetadataEditorService) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.metadataEditorSvc.setResourceInfo(this.digitalContent.digitalContentId, ResourceType.Content);
    this.metadataEditorSvc
      .loadResource()
      .pipe(this.untilDestroy())
      .subscribe();
    this.metadataEditorSvc
      .loadMetadataTags()
      .pipe(this.untilDestroy())
      .subscribe();

    this.metadataEditorSvc.resourceMetadataForm$.pipe(this.untilDestroy()).subscribe(data => {
      const clonedData = Utils.clone(data, _ => {
        _.resource = Utils.cloneDeep(_.resource);
      });
      this.metadataDigitalContent = clonedData;
    });

    this.copyrightDigitalContent = new CopyrightFormModel(this.digitalContent.digitalContent);
    this.checkedFnc();
  }

  public checkedFnc(): void {
    this.dimensionsAndAreasItemIsCheckedFn = this.tagTvItemIsCheckedFnFactory(() => {
      return this.metadataDigitalContent.dimensionsAndAreas;
    });
    this.developmentalRolesItemIsCheckedFn = this.tagTvItemIsCheckedFnFactory(() => {
      return this.metadataDigitalContent.developmentalRoles;
    });
    this.subjectAreasAndKeywordsItemIsCheckedFn = this.tagTvItemIsCheckedFnFactory(() => {
      return this.metadataDigitalContent.subjectAreasAndKeywords;
    });
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
