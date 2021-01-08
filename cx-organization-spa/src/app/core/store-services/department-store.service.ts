import { Injectable } from '@angular/core';
import { DepartmentType } from 'app-models/department-type.model';
import { HttpHelpers } from 'app-utilities/http-helpers';
import { Department } from 'app/department-hierarchical/models/department.model';
import { AppConstant } from 'app/shared/app.constant';
import { Observable } from 'rxjs';
import { shareReplay } from 'rxjs/operators';

@Injectable({
    providedIn: 'root'
})
export class DepartmentStoreService {
    responseCache: Map<string, Observable<any>> = new Map();

    constructor(private httpHelper: HttpHelpers) { }

    getDepartmentTypesByDepartmentId(
        departmentId: number
    ): Observable<DepartmentType[]> {
        const key = `getDepartmentTypesByDepartmentId:${departmentId}`;
        if (this.responseCache.has(key)) {
            return this.responseCache.get(key);
        }

        const response = this.httpHelper
            .get<DepartmentType[]>(
                `${AppConstant.api.organization}/departmenttypes`,
                {
                    departmentids: [departmentId]
                }
            )
            .pipe(shareReplay());

        return this.responseCache.set(key, response).get(key);
    }

    getDepartmentById(departmentId: number): Observable<Department> {
        const key = `getDepartmentById:${departmentId}`;
        if (this.responseCache.has(key)) {
            return this.responseCache.get(key);
        }
        const response = this.httpHelper
            .get<Department[]>(
                `${AppConstant.api.organization}/departments/${departmentId}/hierarchydepartmentidentifiers`,
                {
                    includeParent: false
                }
            )
            .pipe(shareReplay())
            .map((res) => {
                return res[0];
            });

        return this.responseCache.set(key, response).get(key);
    }

    getDepartmentExternalById(extIds: string, ownerId: number): Observable<any> {
        const key = `getDepartmentExternalById:${extIds}`;
        if (this.responseCache.has(key)) {
            return this.responseCache.get(key);
        }
        const response = this.httpHelper.get<Department>
            (`${AppConstant.api.organization}/owners/${ownerId}/departments?departmentStatusEnums=All&extIds=${extIds}`
            ).pipe(shareReplay()).map((res) => {
                return res[0];
            });

        return this.responseCache.set(key, response).get(key);
    }
    /**
     * Gets the top accessible department of the current logged-in user.
     */
    getMyTopDepartment(): Observable<Department> {
        const key = `getMyTopDepartment:`;
        if (this.responseCache.has(key)) {
            return this.responseCache.get(key);
        }
        const response = this.httpHelper
            .get<Department>(
                `${AppConstant.api.organization}/mytophierarchydepartment`
            )
            .pipe(shareReplay())
            .map((res) => {
                return res;
            });

        return this.responseCache.set(key, response).get(key);
    }

    async getDepartmentByIdToPromise(departmentId: number): Promise<Department> {
        try {
            const resp = await this.getDepartmentById(departmentId).toPromise();

            return resp;
        } catch (err) {
            console.error(
                `Error when getting department info for department id ${departmentId}`
            );

            return;
        }
    }

    async getDepartmentTypesByDepartmentIdToPromise(
        departmentId: number
    ): Promise<DepartmentType[]> {
        try {
            const resp = await this.getDepartmentTypesByDepartmentId(
                departmentId
            ).toPromise();

            return resp;
        } catch (err) {
            console.error(
                `Error when getting department type info for department id ${departmentId}`
            );

            return;
        }
    }
}
