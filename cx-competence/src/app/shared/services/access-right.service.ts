import { Injectable } from '@angular/core';
import { AccessRight } from 'app-models/access-right/access-right-model';
import { GetAccessRightParam } from 'app-models/access-right/get-access-right-param.model';
import { GrantedTypeEnum } from 'app-models/access-right/granted-type.enum';
import { HttpHelpers } from 'app-utilities/httpHelpers';
import { Observable } from 'rxjs/Observable';
import { map } from 'rxjs/operators';
import { AppConstant } from '../app.constant';

@Injectable({
  providedIn: 'root',
})
export class AccessRightService {
  private basePortalApiUrl: string = AppConstant.api.portal;

  constructor(private http: HttpHelpers) {}

  /**
   * Retrieves the list of permissions of the current logged-in user.
   * NOTE: It will only return the action keys which the user has permission.
   * @param getAccessRightParam The object parameters which is using for getting the permissions.
   */
  getMyPermissions(
    getAccessRightParam?: GetAccessRightParam
  ): Observable<string[]> {
    return this.http
      .get<AccessRight[]>(
        `${this.basePortalApiUrl}/me/accessRights`,
        getAccessRightParam
      )
      .pipe(
        map((accessRights) =>
          accessRights
            .filter(
              (a) =>
                GrantedTypeEnum[a.grantedType] === GrantedTypeEnum.Allow &&
                !!a.action &&
                a.action.length > 0
            )
            .map((a) => a.action.toLowerCase())
        )
      );
  }
}
