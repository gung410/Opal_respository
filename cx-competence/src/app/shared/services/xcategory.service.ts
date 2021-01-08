import {
  XCategoryType,
  XCategoryTypeQueryModel,
} from '../models/xcategory.model';
import { Injectable } from '@angular/core';
import { HttpHelpers } from 'app-utilities/httpHelpers';
import { Observable } from 'rxjs';
import { AppConstant } from '../app.constant';

@Injectable({
  providedIn: 'root',
})
export class XCategoryService {
  constructor(private http: HttpHelpers) {}

  public getXCategoryTypes(
    filterParamModel: XCategoryTypeQueryModel
  ): Observable<XCategoryType[]> {
    return this.http.get<XCategoryType[]>(
      `${AppConstant.api.assessment}/xcategorytypes`,
      filterParamModel
    );
  }
}
