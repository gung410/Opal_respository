import { AdvanceSearchCatalogModel } from '../models/catalogue-adv-search-filter.model';
import { CourseDataService } from './course-data.service';
import { INavigationMenuItem } from '@opal20/domain-components';
import { Injectable } from '@angular/core';
import { MetadataTagModel } from '@opal20/domain-api';
import { Subject } from 'rxjs';

@Injectable()
export class MegaMenuDataService {
  public serviceSchemeSelectItems: MetadataTagModel[] = [];
  public megaMenuData: INavigationMenuItem[] = [];
  public onServiceSchemeChange = new Subject<INavigationMenuItem[]>();

  private _showingModel: AdvanceSearchCatalogModel;

  private readonly NOT_APPLICABLE_ITEM_DISPLAY_TEXT: string = 'not applicable';

  constructor(private courseDataSvc: CourseDataService) {
    this.courseDataSvc.getCourseMetadata().subscribe(tags => {
      this._showingModel = new AdvanceSearchCatalogModel(tags);

      const serviceSchemeSelectItems = this._showingModel.serviceSchemeSelectItems;
      this.megaMenuData = this.createMegaMenuData(serviceSchemeSelectItems);
      this.onServiceSchemeChange.next(this.megaMenuData);
    });
  }
  private createMegaMenuData(serviceSchemeData: MetadataTagModel[]): INavigationMenuItem[] {
    const serviceScheme: INavigationMenuItem[] = [];
    serviceSchemeData.forEach(item => {
      if (item.displayText.toLocaleLowerCase() !== this.NOT_APPLICABLE_ITEM_DISPLAY_TEXT) {
        const menuItemModel: INavigationMenuItem = {
          id: item.id,
          name: item.displayText
        };
        if (item.childs) {
          const subjectData: INavigationMenuItem[] = [];
          item.childs.map(child => {
            const childData: INavigationMenuItem = {
              id: child.id,
              parentId: item.id,
              name: child.displayText
            };
            subjectData.push(childData);
          });

          menuItemModel.data = subjectData;

          serviceScheme.push(menuItemModel);
        }
      }
    });
    return serviceScheme;
  }
}
