import {
  HttpClientTestingModule,
  HttpTestingController
} from '@angular/common/http/testing';
import { async, TestBed } from '@angular/core/testing';
import { AuthService } from 'app-auth/auth.service';
import { HttpHelpers } from 'app-utilities/http-helpers';
import { AppConstant } from 'app/shared/app.constant';

import { UserAccountMockData } from './mock-data/user-account-mock-data';
import { DepartmentQueryModel } from './models/filter-param.model';
import { UserManagementQueryModel } from './models/user-management.model';
import { UserAccountsDataService } from './user-accounts-data.service';

describe('UserAccountService', () => {
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [UserAccountsDataService, HttpHelpers, AuthService]
    });
  }));

  function setup(): any {
    const userAccountDataService = TestBed.get(UserAccountsDataService);
    const httpMock = TestBed.get(HttpTestingController);

    return { userAccountDataService, httpMock };
  }

  it('should get employee by department id when calling API', () => {
    const { userAccountDataService, httpMock } = setup();
    const params = new UserManagementQueryModel({
      pageIndex: 1,
      pageSize: 100,
      parentDepartmentId: [1]
    });

    userAccountDataService.getUsers(params).subscribe((data: any) => {
      expect(data.items.length).toBe(2);
    });
    const paramsUrl =
      `parentDepartmentId=${params.parentDepartmentId}&pageIndex=${params.pageIndex}&pageSize=${params.pageSize}` +
      `&multiUserTypefilters=&multiUserTypefilters=&multiUserTypeExtIdFilters=&multiUserTypeExtIdFilters=`;
    const mockRequest = httpMock.expectOne(
      `${AppConstant.api.organization}/usermanagement/users?${paramsUrl}`,
      'call to api'
    );
    expect(mockRequest.request.method).toBe('GET');
    expect(mockRequest.cancelled).toBeFalsy();
    expect(mockRequest.request.url).toBe(
      `${AppConstant.api.organization}/usermanagement/users`
    );
    expect(mockRequest.request.responseType).toEqual('json');
    mockRequest.flush(UserAccountMockData.mockEmployeesData);
    httpMock.verify();
  });

  it('should get departments by id when calling API', () => {
    const testParam = 1;
    const { userAccountDataService, httpMock } = setup();
    userAccountDataService
      .getHierarchicalDepartments(
        testParam,
        new DepartmentQueryModel({
          includeParent: false,
          includeChildren: true,
          countChildren: true,
          maxChildrenLevel: 1
        })
      )
      .subscribe((data: any) => {
        expect(data.length).toBe(2);
      });
    const mockRequest = httpMock.expectOne(
      `${AppConstant.api.organization}/departments/${testParam}/hierarchydepartmentidentifiers?includeChildren=true&includeParent=false&countChildren=true&maxChildrenLevel=1`,
      'call to api'
    );
    expect(mockRequest.request.method).toBe('GET');
    expect(mockRequest.cancelled).toBeFalsy();
    expect(mockRequest.request.url).toBe(
      `${AppConstant.api.organization}/departments/${testParam}/hierarchydepartmentidentifiers`
    );
    expect(mockRequest.request.responseType).toEqual('json');
    mockRequest.flush(UserAccountMockData.mockDepartmentData);
    httpMock.verify();
  });

  it('should return newly created user when calling create user API', () => {
    const { userAccountDataService, httpMock } = setup();
    const testUser = UserAccountMockData.mockCreateUserFormData;
    userAccountDataService.createUser(testUser).subscribe((newUser: any) => {
      expect(newUser.firstName).toBe('Tester Tung');
      expect(newUser.systemRoles.length).toBe(14);
    });
    const mockRequest = httpMock.expectOne(
      `${AppConstant.api.organization}/usermanagement/users`,
      'call to api'
    );
    expect(mockRequest.request.method).toBe('POST');
    expect(mockRequest.cancelled).toBeFalsy();
    expect(mockRequest.request.url).toBe(
      `${AppConstant.api.organization}/usermanagement/users`
    );
    expect(mockRequest.request.responseType).toEqual('json');
    mockRequest.flush(UserAccountMockData.mockCreatedUserData);
  });
});
