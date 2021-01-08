import {
  HttpClientTestingModule,
  HttpTestingController
} from '@angular/common/http/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import {
  async,
  ComponentFixture,
  fakeAsync,
  flush,
  TestBed,
  tick
} from '@angular/core/testing';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { Router } from '@angular/router';
import {
  CxExtensiveTreeComponent,
  CxFormModal,
  CxGlobalLoaderService
} from '@conexus/cx-angular-common';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { TranslateModule } from '@ngx-translate/core';
import { OAuthAdapterModule } from 'app-auth/auth-adapter.module';
import { AuthService } from 'app-auth/auth.service';
import { Identity } from 'app-models/identity.model';
import { NotificationDataService } from 'app-services/notification-data.service';
import { NotificationService } from 'app-services/notification.service';
import { ToastrAdapterService } from 'app-services/toastr-adapter.service';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { HttpHelpers } from 'app-utilities/http-helpers';
import { AppConstant } from 'app/shared/app.constant';
import { ToastrService } from 'ngx-toastr';
import { of } from 'rxjs';

import { DepartmentHierarchicalComponent } from './department-hierarchical.component';
import { DepartmentHierarchicalService } from './department-hierarchical.service';
import { DepartmentHierarchicalMockData } from './mock-data/department-hierarchical-mock-data';
import { Department } from './models/department.model';
import {
  DepartmentFilterGroupModel,
  DepartmentFilterOption,
  DepartmentQueryModel
} from './models/filter-params.model';

export class TranslateAdapterServiceStub {
  getValueImmediately(): string {
    return '';
  }
}

describe('DepartmentHierarchicalComponent: ', () => {
  let departmentHierarchicalComponent: DepartmentHierarchicalComponent;
  let departmentHierarchicalFixture: ComponentFixture<DepartmentHierarchicalComponent>;
  let cxExtensiveTreeComponent: CxExtensiveTreeComponent<any>;
  let cxExtensiveTreeFixture: ComponentFixture<CxExtensiveTreeComponent<any>>;
  let departmentHierarchicalService: DepartmentHierarchicalService;
  let httpMock: HttpTestingController;
  let mockData: any;
  let spy: jasmine.Spy;
  mockData = DepartmentHierarchicalMockData.mockDepartmentData;
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [DepartmentHierarchicalComponent, CxExtensiveTreeComponent],
      imports: [
        HttpClientTestingModule,
        OAuthAdapterModule.forRoot(),
        TranslateModule.forRoot(),
        BrowserAnimationsModule,
        NgbModule
      ],
      providers: [
        DepartmentHierarchicalService,
        {
          provide: Router,
          // tslint:disable-next-line:max-classes-per-file
          useClass: class {
            navigate: jasmine.Spy = jasmine.createSpy('navigate');
          }
        },
        CxFormModal,
        ToastrAdapterService,
        {
          provide: CxGlobalLoaderService,
          useValue: {
            showLoader: () => {
              return;
            },
            hideLoader: () => {
              return;
            }
          }
        },
        {
          provide: ToastrService,
          useValue: {
            error: (param?, error?) => {
              return;
            },
            warning: (param?) => {
              return;
            }
          }
        },
        NotificationDataService,
        NotificationService,
        AuthService,
        HttpHelpers,
        [
          {
            provide: TranslateAdapterService,
            useClass: TranslateAdapterServiceStub
          }
        ]
      ],
      schemas: [NO_ERRORS_SCHEMA]
    })
      .compileComponents()
      .then(() => {
        departmentHierarchicalFixture = TestBed.createComponent(
          DepartmentHierarchicalComponent
        );
        departmentHierarchicalService = TestBed.get(
          DepartmentHierarchicalService
        );
        httpMock = TestBed.get(HttpTestingController);
        cxExtensiveTreeFixture = TestBed.createComponent(
          CxExtensiveTreeComponent
        );
        departmentHierarchicalComponent =
          departmentHierarchicalFixture.componentInstance;
        cxExtensiveTreeComponent = cxExtensiveTreeFixture.componentInstance;
        departmentHierarchicalComponent.flatDepartmentsArray = mockData;
        departmentHierarchicalComponent.extensiveTreeComponent = cxExtensiveTreeComponent;
      });
  }));

  beforeEach(() => {
    departmentHierarchicalFixture.detectChanges();
  });

  it('should create', () => {
    expect(departmentHierarchicalComponent).toBeTruthy();
    expect(departmentHierarchicalService).toBeTruthy();
  });

  it('should call API and get hierarchical data', () => {
    const departmentId = 1;
    const testParam = new DepartmentQueryModel({
      includeParent: true,
      includeChildren: true,
      departmentTypeIds: [1],
      departmentEntityStatuses: ['Active']
    });
    departmentHierarchicalService
      .getHierarchy(departmentId, testParam)
      .subscribe((data: any) => {
        expect(data[0].departmentName).toBe('Sub Department A.2.1');
      });
    const remainingURLPath =
      `hierarchydepartmentidentifiers?includeChildren=true&includeParent=true&departmentTypeIds` +
      `=${testParam.departmentTypeIds[0]}&departmentEntityStatuses=${testParam.departmentEntityStatuses[0]}`;
    const mockRequest = httpMock.expectOne(
      `${AppConstant.api.organization}/departments/${departmentId}/${remainingURLPath}`,
      'call to api'
    );
    expect(mockRequest.request.method).toBe('GET');
    expect(mockRequest.cancelled).toBeFalsy();
    expect(mockRequest.request.url).toBe(
      `${AppConstant.api.organization}/departments/${departmentId}/hierarchydepartmentidentifiers`
    );
    expect(mockRequest.request.responseType).toEqual('json');
    mockRequest.flush(mockData);
    httpMock.verify();
  });

  describe('Update department: ', () => {
    const editedDepartment = {
      parentDepartmentId: 1,
      identity: {
        extId: 'HRMS00111',
        ownerId: 3001,
        customerId: 2052,
        archetype: 'OrganizationalUnit',
        id: 14298
      },
      departmentName: 'MOE test 33',
      departmentDescription: '',
      organizationNumber: '12221',
      address: '1131',
      postalCode: '55555',
      city: 'ddd',
      tag: '',
      languageId: 2,
      countryCode: 65,
      entityStatus: {
        externallyMastered: true,
        lastExternallySynchronized: '2019-03-25T11:13:35.9466667',
        entityVersion: 'AAAAAAALVW0=',
        lastUpdated: '2019-07-18T04:47:01.6877684',
        lastUpdatedBy: 3,
        statusId: 'Active',
        statusReasonId: 'Unknown',
        deleted: false
      }
    } as Department;

    let formModal: CxFormModal;
    beforeEach(() => {
      (departmentHierarchicalComponent as any).currentSelectedDepartment = editedDepartment;
      formModal = departmentHierarchicalFixture.debugElement.injector.get(
        CxFormModal
      );
      spyOn(formModal, 'openSurveyJsForm').and.returnValue({
        result: new Promise(() => {
          return;
        }),
        close: () => {
          return;
        }
      });
    });

    it('should edit selected department and call api to get information for selected department', fakeAsync(() => {
      const fakeResult = {};
      spy = spyOn(
        (departmentHierarchicalComponent as any).departmentHierarchicalService,
        'getDepartmentInfo'
      ).and.returnValue(of(fakeResult));
      spyOn(departmentHierarchicalComponent, 'openUpdateDepartmentForm');
      departmentHierarchicalComponent.onUpdateDepartment(editedDepartment);

      expect(spy).toHaveBeenCalled();
      expect(
        departmentHierarchicalComponent.openUpdateDepartmentForm
      ).toHaveBeenCalled();
      expect(
        (departmentHierarchicalComponent as any).currentSelectedDepartment
      ).toBe(fakeResult);
    }));

    it('should edit selected department and open dialog', fakeAsync(() => {
      departmentHierarchicalComponent.openUpdateDepartmentForm();
      expect(formModal.openSurveyJsForm).toHaveBeenCalled();
    }));

    it('should edit selected department and submit form to called updateDepartment function', fakeAsync(() => {
      spyOn<any>(departmentHierarchicalComponent, 'updateDepartment');
      departmentHierarchicalComponent.openUpdateDepartmentForm();
      formModal.submit.next(editedDepartment);

      expect(
        (departmentHierarchicalComponent as any).updateDepartment
      ).toHaveBeenCalled();
      expect(
        (departmentHierarchicalComponent as any).updateDepartment
      ).toHaveBeenCalledWith(editedDepartment as Department);
    }));

    it('should edit selected department and process submitted data', fakeAsync(() => {
      const afterChangeData = {
        ...editedDepartment,
        organizationNumber: '123123',
        address: '123456 st',
        name: 'Updated department name',
        levels: []
      };
      spyOn<any>(
        departmentHierarchicalComponent,
        'buildDepartmentDtoFromSubmittedForm'
      ).and.returnValue(afterChangeData);
      spyOn<any>(
        (departmentHierarchicalComponent as any).extensiveTreeComponent,
        'executeEditItem'
      );
      spyOn<any>((departmentHierarchicalComponent as any).toastr, 'success');
      spyOn<any>(
        (departmentHierarchicalComponent as any).departmentHierarchicalService,
        'updateOrganizationUnit'
      ).and.returnValue(of(afterChangeData));
      departmentHierarchicalComponent.updateDepartment(editedDepartment);

      expect(
        (departmentHierarchicalComponent as any)
          .buildDepartmentDtoFromSubmittedForm
      ).toHaveBeenCalled();
      expect(
        (departmentHierarchicalComponent as any)
          .buildDepartmentDtoFromSubmittedForm
      ).toHaveBeenCalledWith(editedDepartment as Department);
      expect(
        (departmentHierarchicalComponent as any).departmentHierarchicalService
          .updateOrganizationUnit
      ).toHaveBeenCalled();
      expect(
        (departmentHierarchicalComponent as any).departmentHierarchicalService
          .updateOrganizationUnit
      ).toHaveBeenCalledWith(afterChangeData as Department);
      const departmentIndex = departmentHierarchicalComponent.flatDepartmentsArray.findIndex(
        (item) => item.identity.id === afterChangeData.identity.id
      );
      expect(
        departmentHierarchicalComponent.flatDepartmentsArray[departmentIndex]
          .departmentName
      ).toEqual(afterChangeData.name);
      departmentHierarchicalFixture.destroy();
      flush();
    }));
  });

  //TODO: modify when implement function add new department
  xit('should add new department with new title', fakeAsync(() => {
    const tickSecond = 100;
    const departmentLenght = 100;
    const parentDepartment = {
      parentDepartmentId: 13738,
      identity: {
        extId: '',
        ownerId: 2001,
        customerId: 1792,
        archetype: 'OrganizationalUnit',
        id: 14282
      },
      departmentName: 'Sub Department A.2.1',
      departmentDescription: ''
    };
    const newName = 'New Department';
    spy = spyOn(
      departmentHierarchicalComponent.extensiveTreeComponent,
      'executeAddItem'
    ).and.returnValue(null);
    tick(tickSecond);
    departmentHierarchicalFixture.whenStable().then(() => {
      expect(
        departmentHierarchicalComponent.extensiveTreeComponent.executeAddItem
      ).toHaveBeenCalled();
      expect(departmentHierarchicalComponent.flatDepartmentsArray.length).toBe(
        departmentLenght
      );
      const parentDepartmentOfAddedDepartment = departmentHierarchicalComponent.flatDepartmentsArray.filter(
        (x) => x.parentDepartmentId === parentDepartment.identity.id
      );
      expect(parentDepartmentOfAddedDepartment.length).toBeGreaterThan(1);
      expect(
        parentDepartmentOfAddedDepartment.find(
          (x) => x.departmentName === newName
        )
      ).not.toBeUndefined();
    });
  }));

  it('should clear current filter when calling onClearAllFilter()', () => {
    departmentHierarchicalComponent.departmentTagData = [
      new DepartmentFilterGroupModel()
    ];
    departmentHierarchicalComponent.departmentTagData[0].options = [
      new DepartmentFilterOption({
        isSelected: true
      }),
      new DepartmentFilterOption({
        isSelected: true
      })
    ];
    departmentHierarchicalComponent.departmentFilterOptions =
      departmentHierarchicalComponent.departmentTagData;
    departmentHierarchicalComponent.onClearAllFilter();
    expect(departmentHierarchicalComponent.departmentTagData.length).toBe(0);
    const isAnyOptionSelected = departmentHierarchicalComponent.departmentFilterOptions[0].options.some(
      (option) => option.isSelected === true
    );
    expect(isAnyOptionSelected).toBe(false);
  });

  it('should call api to get more departments when calling loadChildren() function', () => {
    const departmentId = 123;
    const department = new Department({
      parentDepartmentId: 13738,
      identity: new Identity({
        extId: '',
        ownerId: 2001,
        customerId: 1792,
        archetype: 'OrganizationalUnit',
        id: 14282
      }),
      departmentName: 'Sub Department A.2.1',
      departmentDescription: ''
    });
    const childrenDepartment = { ...department };
    childrenDepartment.identity.id = departmentId;
    departmentHierarchicalService = departmentHierarchicalFixture.debugElement.injector.get(
      DepartmentHierarchicalService
    );
    spy = spyOn(departmentHierarchicalService, 'getHierarchy').and.returnValue(
      of([childrenDepartment])
    );
    departmentHierarchicalComponent.loadChildren(department);
    expect(departmentHierarchicalService.getHierarchy).toHaveBeenCalled();
  });
});
