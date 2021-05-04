import { HttpErrorResponse } from '@angular/common/http';
import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  OnInit,
  ViewChild,
  ViewEncapsulation
} from '@angular/core';
import {
  CxConfirmationDialogComponent,
  CxExtensiveTreeComponent,
  CxFormModal,
  CxGlobalLoaderService,
  CxSlidebarComponent,
  CxSurveyjsFormModalOptions,
  CxSurveyjsVariable,
  CxTreeButtonCondition,
  CxTreeText
} from '@conexus/cx-angular-common';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'app-auth/auth.service';
import { User } from 'app-models/auth.model';
import { DepartmentType } from 'app-models/department-type.model';
import { ToastrAdapterService } from 'app-services/toastr-adapter.service';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { SurveyUtils } from 'app-utilities/survey-utils';
import { GlobalKeySearchStoreService } from 'app/core/store-services/search-key-store.service';
import { Constant } from 'app/shared/app.constant';
import { BaseSmartComponent } from 'app/shared/components/component.abstract';
import { findIndexCommon } from 'app/shared/constants/common.const';
import { GroupFilterConst } from 'app/shared/constants/filter-group.constant';
import { SurveyVariableEnum } from 'app/shared/constants/survey-variable.enum';
import * as _ from 'lodash';

import { LocalizedDataItem } from 'app-models/localized-data-item.model';
import { RunOnNextRender } from 'app-utilities/run-on-next-render';
import { Utils } from 'app-utilities/utils';
import { UserAccountsHelper } from 'app/user-accounts/user-accounts.helper';
import { SAM_PERMISSIONS } from '../shared/constants/sam-permission.constant';
import { DepartmentFormJSON } from './department-form';
import { DepartmentHierarchicalService } from './department-hierarchical.service';
import { FilterDepartmentComponent } from './filter-department/filter-department.component';
import { organizationUnitLevelsConst } from './models/department-level.model';
import { Department } from './models/department.model';
import {
  DepartmentFilterGroupModel,
  DepartmentFilterOption,
  DepartmentQueryModel
} from './models/filter-params.model';
import { SearchDepartmentHierarchicalComponent } from './search-department-hierarchical-dialog/search-department-hierarchical.component';
import { DepartmentSurveyJSFormHelper } from './utilities/department-surveyjs-form.helper';

@Component({
  selector: 'department-hierarchical',
  templateUrl: './department-hierarchical.component.html',
  styleUrls: ['./department-hierarchical.component.scss'],
  providers: [DepartmentHierarchicalService],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DepartmentHierarchicalComponent
  extends BaseSmartComponent
  implements OnInit {
  flatDepartmentsArray: Department[] = [];
  currentDepartmentId: number;
  selectedDepartmentId: number;
  departmentFilterOptions: DepartmentFilterGroupModel[];
  departmentFilterOptionsId: number[] = [];
  countDepartmentFilterChoice: number = 0;
  clearDepartmentFilterOptions: DepartmentFilterGroupModel[];
  departmentQueryObject: DepartmentQueryModel = new DepartmentQueryModel({});
  departmentTagData: DepartmentFilterGroupModel[];
  buttonCondition: CxTreeButtonCondition<any>;
  departmentTypeTitle: string;
  currentLanguageCode: string;
  departmentTypes: DepartmentType[];
  text: CxTreeText = new CxTreeText();
  havingExtensiveArea: boolean = true;
  isFiltering: boolean = false;
  isLoadingDepartment: boolean = false;
  levelOfEducationMap: object = {};
  isNoDepartment: boolean = false;
  departmentTypeList: DepartmentType[];
  departmentPathMap: any = {};
  searchDepartmentResult: any = [];
  departmentRouteMap: any = {};
  isDetectExpandTree: boolean;
  typesOfOrganisation: [] = [];

  @ViewChild(CxExtensiveTreeComponent)
  extensiveTreeComponent: CxExtensiveTreeComponent<any>;
  private currentUser: User;
  private departmentForm: any = DepartmentFormJSON;
  private currentSelectedDepartment: Department;
  private departmentSurveyJSHelper: DepartmentSurveyJSFormHelper = new DepartmentSurveyJSFormHelper();
  @ViewChild(CxSlidebarComponent)
  private slidebar: CxSlidebarComponent;

  public slidebarPosition: string = 'left';
  public height: string = '70vh';
  constructor(
    changeDetectorRef: ChangeDetectorRef,
    private departmentHierarchicalService: DepartmentHierarchicalService,
    private authService: AuthService,
    private cxGlobalLoaderService: CxGlobalLoaderService,
    private translateAdapterService: TranslateAdapterService,
    private formModal: CxFormModal,
    private toastr: ToastrAdapterService,
    private globalKeySearchStoreService: GlobalKeySearchStoreService,
    public ngbModal: NgbModal,
    private translateService: TranslateService
  ) {
    super(changeDetectorRef);
  }

  ngOnInit(): void {
    this.text.header = 'Organisation';
    this.initButtonCondition();
    this.cxGlobalLoaderService.showLoader();
    this.subscription.add(
      this.authService.userData().subscribe((user) => {
        if (user) {
          this.currentUser = user;
          this.currentLanguageCode = localStorage.getItem('language-code');
          this.currentDepartmentId = user.departmentId;
          this.selectedDepartmentId = user.departmentId;
          this.initFilterObject();
          this.getDepartments(
            UserAccountsHelper.getDefaultRootDepartment(this.currentUser)
          );
          this.getEducationLevels();
          this.getDepartmentTypeList();

          this.subscription.add(
            this.departmentHierarchicalService
              .getTypeOfOrganisation()
              .subscribe((response) => {
                response.forEach((organisation) => {
                  this.typesOfOrganisation[organisation.id] = organisation;
                });
              })
          );
        }
      })
    );
    this.subscription.add(
      this.globalKeySearchStoreService.get().subscribe((result: any) => {
        if (result) {
          if (result.isSearch && result.searchKey) {
            this.onSearchDepartment(result.searchKey);
          }
        }
      })
    );
  }

  get isUserHasRightsToAccessCRUDOrganisationManagement(): boolean {
    return this.currentUser.hasPermission(
      SAM_PERMISSIONS.CRUDinOrganisationManagement
    );
  }

  displayText(department: any): string {
    if (
      !department ||
      !department.jsonDynamicAttributes ||
      !department.jsonDynamicAttributes.typeOfOrganizationUnits
    ) {
      return '';
    }
    const typeOfOrg: any = this.typesOfOrganisation[
      department.jsonDynamicAttributes.typeOfOrganizationUnits
    ];

    return typeOfOrg ? typeOfOrg.displayText : '';
  }
  onSearchDepartment(searchKey: string): void {
    this.cxGlobalLoaderService.showLoader();
    this.departmentQueryObject.searchText = searchKey;
    this.departmentQueryObject.maxChildrenLevel = undefined;

    this.departmentHierarchicalService
      .getHierarchy(
        UserAccountsHelper.getDefaultRootDepartment(this.currentUser),
        this.departmentQueryObject
      )
      .subscribe(
        (response: Department[]) => {
          this.flatDepartmentsArray = _.uniqBy(
            this.flatDepartmentsArray.concat(response),
            'identity.id'
          );
          this.searchDepartmentResult = UserAccountsHelper.searchDepartments(
            response,
            searchKey
          );
          this.searchDepartmentResult.map((element: any) => {
            this.departmentPathMap[
              element.identity.id
            ] = this.getPathOfDepartment(response, element);
            const routeDepartment = {};
            this.getRouteOfDepartment(response, element, routeDepartment);
            this.departmentRouteMap[element.identity.id] = routeDepartment;
          });

          this.openDialog(this.searchDepartmentResult);
          this.cxGlobalLoaderService.hideLoader();
        },
        () => {
          this.toastr.error('There is something wrong, Please try again.');
          this.cxGlobalLoaderService.hideLoader();
        }
      );
  }
  onOpenAddDepartmentForm(newDepartmentData: {
    parentObject: Department;
    level: number;
  }): void {
    if (!this.isUserHasRightsToAccessCRUDOrganisationManagement) {
      return;
    }

    this.subscription.add(
      this.departmentHierarchicalService
        .getDepartmentInfo(newDepartmentData.parentObject)
        .subscribe((response: Department) => {
          if (response) {
            this.initializeNewDepartment(response);
          }
        })
    );
  }
  initializeNewDepartment(parentDepartmentInfo: Department): void {
    if (!this.formModal.hasOpenModals()) {
      const surveyJsFormOptionObject: any = this.departmentSurveyJSHelper.openAddDepartmentSurveyJSForm(
        parentDepartmentInfo,
        this.departmentTypeList
      );

      if (surveyJsFormOptionObject === null) {
        this.toastr.error(
          'Cannot initialize new department belong to current department selected'
        );

        return;
      }
      const modalRef = this.formModal.openSurveyJsForm(
        this.departmentForm,
        surveyJsFormOptionObject.dataJson,
        [],
        surveyJsFormOptionObject.options,
        { size: 'lg', backdrop: 'static', centered: true }
      );
      const submitObserver = this.formModal.submit.subscribe(
        (submitData: any) => {
          if (submitData) {
            const submittedDataConverted = SurveyUtils.mapSurveyJSResultToObject(
              submitData
            );
            const newDepartment = new Department(submittedDataConverted);
            if (
              newDepartment &&
              newDepartment.jsonDynamicAttributes &&
              newDepartment.jsonDynamicAttributes.showInSignUpForm &&
              newDepartment.jsonDynamicAttributes.showInSignUpForm instanceof
                Array
            ) {
              newDepartment.jsonDynamicAttributes.showInSignUpForm =
                newDepartment.jsonDynamicAttributes.showInSignUpForm[0];
            }
            newDepartment.entityStatus = {
              ...newDepartment.entityStatus,
              lastUpdated: new Date(),
              lastUpdatedBy: this.currentUser.identity.id,
              statusId: 'Active',
              statusReasonId: 'Unknown'
            };
            this.onAddDepartment(newDepartment);
            modalRef.close();
          }
        },
        (error) => {
          this.toastr.error(
            error.error.Message,
            'An error occurred when updating department.'
          );
        }
      );
      modalRef.result.finally(() => {
        submitObserver.unsubscribe();
      });
    }
  }

  onAddDepartment(newDepartment: Department): void {
    this.cxGlobalLoaderService.showLoader();
    this.subscription.add(
      this.departmentHierarchicalService
        .addNewOrganizationUnit(newDepartment)
        .subscribe((department) => {
          if (department) {
            department.departmentName = department.name;
            const childs = this.flatDepartmentsArray.filter(
              (x) => x.parentDepartmentId === department.parentDepartmentId
            );
            if (childs && childs.length > 0) {
              const newDepartmentArray = _.cloneDeep(this.flatDepartmentsArray);
              const parentDepartmentIndex = newDepartmentArray.findIndex(
                (parentDepartment) =>
                  parentDepartment.identity.id === department.parentDepartmentId
              );
              newDepartmentArray[parentDepartmentIndex].childrenCount += 1;
              newDepartmentArray.push(department);
              this.flatDepartmentsArray = newDepartmentArray;
              this.cxGlobalLoaderService.hideLoader();
            }
            if (typeof department.childrenCount === 'undefined') {
              department.childrenCount = 0;
              department.userCount = 0;
            }
            this.updateLevelOfEducationMap(department);
            this.changeDetectorRef.detectChanges();
            this.extensiveTreeComponent.executeAddItem(department, 0);
            this.toastr.success(
              `Organisation unit ${department.departmentName} has been added`
            );
          }
        }, this.handleErrorWhenAddDepartment)
    );
  }

  onUpdateDepartment(editedDepartmentData: Department): void {
    if (!this.isUserHasRightsToAccessCRUDOrganisationManagement) {
      return;
    }

    if (editedDepartmentData) {
      this.subscription.add(
        this.departmentHierarchicalService
          .getDepartmentInfo(editedDepartmentData)
          .subscribe((response: Department) => {
            if (response) {
              this.currentSelectedDepartment = response;
              this.openUpdateDepartmentForm();
            }
          })
      );
    }
  }

  onDeleteItem(departmentData: any): void {
    if (!this.isUserHasRightsToAccessCRUDOrganisationManagement) {
      return;
    }

    const modalRef = this.ngbModal.open(CxConfirmationDialogComponent, {
      size: 'lg',
      centered: true,
      windowClass: 'delete-modal'
    });
    const modalComponent = modalRef.componentInstance as CxConfirmationDialogComponent;
    modalComponent.cancelButtonText = this.translateService.instant(
      'Common.Button.Cancel'
    );
    modalComponent.confirmButtonText = this.translateService.instant(
      'Common.Button.Confirm'
    );
    modalComponent.header = this.translateService.instant(
      'Common.Label.ConfirmationDialog'
    );
    modalComponent.content = this.translateService.instant(
      'Department_Page.Department_Delete.Deactivate',
      { departmentName: departmentData.object.departmentName }
    );

    modalComponent.confirm.subscribe(() => {
      modalRef.close();
      this.departmentHierarchicalService
        .deleteHierarchy(departmentData)
        .subscribe(
          () => {
            const newDepartmentArray = _.cloneDeep(this.flatDepartmentsArray);
            const parentDepartmentIndex = newDepartmentArray.findIndex(
              (parentDepartment) =>
                parentDepartment.identity.id ===
                departmentData.object.parentDepartmentId
            );
            newDepartmentArray[parentDepartmentIndex].childrenCount -= 1;

            _.remove(newDepartmentArray, (department) => {
              return (
                department.identity.id === departmentData.object.identity.id
              );
            });
            this.flatDepartmentsArray = newDepartmentArray;
            this.changeDetectorRef.detectChanges();
            this.toastr.success(
              this.translateService.instant(
                'Department_Page.Department_Delete.Successfully',
                { departmentName: departmentData.object.departmentName }
              )
            );
          },
          (error) => {
            this.toastr.error(
              this.translateService.instant(
                'Department_Page.Department_Delete.Error'
              )
            );
          }
        );
    });

    modalComponent.cancel.subscribe(() => {
      modalRef.close();
    });
  }

  openUpdateDepartmentForm(): void {
    const departmentTypeObject = this.checkDepartmentType(
      this.currentSelectedDepartment
    );
    if (!this.formModal.hasOpenModals()) {
      const surveyjsVariables = [
        new CxSurveyjsVariable({
          name: SurveyVariableEnum.currentObject_isExternallyMastered,
          value: this.currentSelectedDepartment.entityStatus.externallyMastered
        }),
        new CxSurveyjsVariable({
          name: SurveyVariableEnum.formDisplayMode,
          value: 'edit'
        }),
        new CxSurveyjsVariable({
          name: SurveyVariableEnum.isClusterDepartment,
          value: departmentTypeObject.isClusterDepartment
        }),
        new CxSurveyjsVariable({
          name: SurveyVariableEnum.isZoneDepartment,
          value: departmentTypeObject.isZoneDepartment
        }),
        new CxSurveyjsVariable({
          name: SurveyVariableEnum.isSchoolDepartment,
          value: departmentTypeObject.isSchoolDepartment
        }),
        new CxSurveyjsVariable({
          name: SurveyVariableEnum.isBranchDepartment,
          value: departmentTypeObject.isBranchDepartment
        })
      ];
      const options = {
        showModalHeader: true,
        fixedButtonsFooter: true,
        modalHeaderText: 'Edit Organisation Unit',
        cancelName: 'Cancel',
        submitName: 'Save',
        variables: surveyjsVariables
      } as CxSurveyjsFormModalOptions;
      const dataJson: any = {
        organizationalUnitTypes: this.currentSelectedDepartment
          .organizationalUnitTypes,
        'jsonDynamicAttributes.typeOfOrganizationUnits':
          this.currentSelectedDepartment.jsonDynamicAttributes &&
          this.currentSelectedDepartment.jsonDynamicAttributes
            .typeOfOrganizationUnits
            ? this.currentSelectedDepartment.jsonDynamicAttributes
                .typeOfOrganizationUnits
            : '',
        name: this.currentSelectedDepartment.name,
        organizationNumber: this.currentSelectedDepartment.organizationNumber,
        'identity.extId': this.currentSelectedDepartment.identity.extId,
        'jsonDynamicAttributes.clusterSuperintendent':
          this.currentSelectedDepartment.jsonDynamicAttributes &&
          this.currentSelectedDepartment.jsonDynamicAttributes
            .clusterSuperintendent
            ? this.currentSelectedDepartment.jsonDynamicAttributes
                .clusterSuperintendent
            : '',
        'jsonDynamicAttributes.zoneDirector':
          this.currentSelectedDepartment.jsonDynamicAttributes &&
          this.currentSelectedDepartment.jsonDynamicAttributes.zoneDirector
            ? this.currentSelectedDepartment.jsonDynamicAttributes.zoneDirector
            : '',
        levels: this.currentSelectedDepartment.levels
          ? this.currentSelectedDepartment.levels
          : [],
        'jsonDynamicAttributes.showInSignUpForm':
          this.currentSelectedDepartment.jsonDynamicAttributes &&
          this.currentSelectedDepartment.jsonDynamicAttributes.showInSignUpForm,
        'entityStatus.externallyMastered': this.currentSelectedDepartment
          .entityStatus.externallyMastered,
        address: this.currentSelectedDepartment.address
      };
      const modalRef = this.formModal.openSurveyJsForm(
        this.departmentForm,
        dataJson,
        [],
        options,
        { size: 'lg', backdrop: 'static', centered: true }
      );
      const submitObserver = this.formModal.submit.subscribe(
        (submitData: any) => {
          if (submitData) {
            const submittedDataConverted = SurveyUtils.mapSurveyJSResultToObject(
              submitData
            );
            this.updateDepartment(submittedDataConverted as Department);
          }
          modalRef.close();
        },
        (error) => {
          this.toastr.error(
            error.error.Message,
            'An error occurred when updating department.'
          );
        }
      );
      modalRef.result.finally(() => {
        submitObserver.unsubscribe();
      });
    }
  }

  updateDepartment(submitData: Department): void {
    const departmentDTO: Department = this.buildDepartmentDtoFromSubmittedForm(
      submitData
    );
    this.cxGlobalLoaderService.showLoader();
    this.subscription.add(
      this.departmentHierarchicalService
        .updateOrganizationUnit(departmentDTO)
        .subscribe(
          (departmentResponse: Department) => {
            if (departmentResponse) {
              const departmentIndex = this.flatDepartmentsArray.findIndex(
                (item) => item.identity.id === departmentResponse.identity.id
              );
              if (departmentIndex > findIndexCommon.notFound) {
                let departmentChanged = {
                  ...this.flatDepartmentsArray[departmentIndex]
                };
                departmentChanged = this.convertDepartment(
                  departmentResponse,
                  departmentChanged
                );
                this.updateLevelOfEducationMap(departmentChanged);
                this.flatDepartmentsArray[departmentIndex] = departmentChanged;
                this.extensiveTreeComponent.executeEditItem(departmentChanged);
                this.toastr.success('Organisation unit has been updated');
                this.cxGlobalLoaderService.hideLoader();
              }
            }
          },
          (error) => {
            this.toastr.error('An error occurred when updating department.');
            this.cxGlobalLoaderService.hideLoader();
          }
        )
    );
  }

  loadMoreDepartments(departments: Department[]): void {
    const parentItem = this.flatDepartmentsArray.find(
      (x) => x.identity.id === this.selectedDepartmentId
    );
    const newParentItem = departments.find(
      (x) => x.identity.id === this.selectedDepartmentId
    );
    const parentIdex = this.flatDepartmentsArray.indexOf(parentItem);
    if (parentIdex > findIndexCommon.notFound) {
      this.flatDepartmentsArray[parentIdex] = newParentItem;
    }
    departments = departments.filter(
      (x) => x.identity.id !== this.selectedDepartmentId
    );
    this.flatDepartmentsArray = departments
      ? this.flatDepartmentsArray.concat(departments)
      : this.flatDepartmentsArray;
    this.isLoadingDepartment = false;
  }

  onCloseTagGroup(data: DepartmentFilterGroupModel): void {
    switch (data.groupConstant) {
      case GroupFilterConst.DEPARTMENT_TYPE:
        this.departmentTagData = this.departmentTagData.filter(
          (tagData) =>
            tagData.groupConstant !== GroupFilterConst.DEPARTMENT_TYPE
        );
        this.departmentFilterOptions.forEach((filterOption) => {
          if (filterOption.groupConstant !== data.groupConstant) {
            return;
          } else {
            return filterOption.options.forEach((option) => {
              return (option.isSelected = false);
            });
          }
        });
        this.departmentQueryObject.departmentTypeIds = [];
        this.selectedDepartmentId = this.currentDepartmentId;
        this.flatDepartmentsArray = [];
        this.getDepartments(this.selectedDepartmentId);
        break;
      default:
        return;
    }
  }

  onCloseTag(tag: any, groupConst: any): void {
    switch (groupConst) {
      case GroupFilterConst.DEPARTMENT_TYPE:
        this.departmentTagData.forEach((tagData) => {
          if (tagData.groupConstant === groupConst) {
            tagData.options.forEach((option) => {
              if (option.objectId === tag.objectId) {
                return (option.isSelected = false);
              }
            });
          }
        });
        this.departmentTagData = this.departmentTagData.filter(
          (data) => data.options.some((item) => item.isSelected) === true
        );

        this.departmentFilterOptions.forEach((filterOption) => {
          if (filterOption.groupConstant === groupConst) {
            filterOption.options.forEach((option) => {
              if (option.objectId === tag.objectId) {
                return (option.isSelected = false);
              }
            });
          }
        });
        const departmentTypeIds = this.departmentQueryObject.departmentTypeIds.filter(
          (departmentTypeId) => departmentTypeId !== tag.objectId
        );
        this.selectedDepartmentId = this.currentDepartmentId;
        this.flatDepartmentsArray = [];
        this.departmentQueryObject.departmentTypeIds = [];
        this.getDepartments(this.selectedDepartmentId);
        this.departmentQueryObject.departmentTypeIds = departmentTypeIds;
        break;
      default:
        return;
    }
  }

  onClickFilter(): void {
    const departmentFilterGroup = Utils.cloneDeep(
      this.departmentFilterOptions[0]
    );
    const modalRef = this.ngbModal.open(FilterDepartmentComponent, {
      size: 'lg',
      backdrop: 'static',
      centered: false,
      windowClass: 'filter-slidebar'
    });

    const instanceComponent = modalRef.componentInstance as FilterDepartmentComponent;
    instanceComponent.departmentFilterOptions = this.departmentFilterOptions;
    this.subscription.add(
      instanceComponent.done.subscribe(
        (departmentFilterOptions: DepartmentFilterGroupModel[]) => {
          this.changeOrganisationWithFilterData(departmentFilterOptions);
          modalRef.close();
        }
      )
    );
    this.subscription.add(
      instanceComponent.cancel.subscribe(() => {
        this.departmentFilterOptions[0] = departmentFilterGroup;
        modalRef.close();
      })
    );
  }
  changeOrganisationWithFilterData(
    departmentFilterOptions: DepartmentFilterGroupModel[]
  ): void {
    const departmentTypeFilterOption = departmentFilterOptions.find(
      (filterOption) => filterOption.name === this.departmentTypeTitle
    );
    RunOnNextRender(() => {
      const selectedOption = this.departmentFilterOptions[0].options.filter(
        (option) => option.isSelected
      );

      this.countDepartmentFilterChoice = selectedOption.length
        ? selectedOption.length
        : 0;
      this.changeDetectorRef.detectChanges();
    });

    this.cxGlobalLoaderService.showLoader();
    this.departmentQueryObject.departmentTypeIds = departmentTypeFilterOption.options
      .filter((option) => option.isSelected)
      .map((option) => option.objectId);
    this.departmentQueryObject.maxChildrenLevel = undefined;

    this.departmentHierarchicalService
      .getHierarchy(
        UserAccountsHelper.getDefaultRootDepartment(this.currentUser),
        this.departmentQueryObject
      )
      .subscribe(
        (response: Department[]) => {
          this.cxGlobalLoaderService.hideLoader();
          this.flatDepartmentsArray = _.uniqBy(
            this.flatDepartmentsArray.concat(response),
            'identity.id'
          );
          const filterResult = response.filter((department) => {
            if (department.levels) {
              return (
                department.levels.findIndex((level) =>
                  this.departmentQueryObject.departmentTypeIds.includes(
                    level.identity.id
                  )
                ) > -1
              );
            }

            return false;
          });

          filterResult.forEach((element: any) => {
            this.departmentPathMap[element.identity.id] = element.pathName
              ? element.pathName
              : this.getPathOfDepartment(response, element);
          });

          const isSearchAction = false;
          this.openDialog(filterResult, isSearchAction);
        },
        () => {
          this.toastr.error('There is something wrong, Please try again.');
          this.cxGlobalLoaderService.hideLoader();
        }
      );
  }

  onClearAllFilter(): void {
    this.departmentTagData = [];
    this.departmentFilterOptions.forEach((filterOption) => {
      filterOption.options.forEach((option) => {
        option.isSelected = false;
      });
    });
    this.initFilterObject();
    this.selectedDepartmentId = this.currentDepartmentId;
    this.flatDepartmentsArray = [];
    this.getDepartments(
      UserAccountsHelper.getDefaultRootDepartment(this.currentUser)
    );
  }

  loadChildren(selectedItem: Department): void {
    if (selectedItem && selectedItem.identity) {
      this.isLoadingDepartment = true;
      this.selectedDepartmentId = selectedItem.identity.id;
      this.getDepartments(this.selectedDepartmentId);
    }
  }

  onClickItemResult(department: any): void {
    this.searchDepartmentResult = [];
    this.departmentQueryObject.departmentTypeIds = [];
    this.ngbModal.dismissAll();
    this.currentDepartmentId = department.identity.id;
    this.changeDetectorRef.detectChanges();
    this.isFiltering = true;
    this.getDepartments(this.currentDepartmentId);
    this.isDetectExpandTree = true;
  }

  openDialog(departments: Department[], isSearchAction: boolean = true): void {
    if (!this.ngbModal.hasOpenModals()) {
      const modalRef = this.ngbModal.open(
        SearchDepartmentHierarchicalComponent,
        {
          centered: true,
          size: 'lg',
          backdrop: 'static'
        }
      );
      const searchDepartmentDialog = modalRef.componentInstance as SearchDepartmentHierarchicalComponent;
      searchDepartmentDialog.searchDepartmentResult = departments;
      searchDepartmentDialog.isSearchAction = isSearchAction;
      searchDepartmentDialog.dialogHeaderText = this.translateAdapterService.getValueImmediately(
        isSearchAction
          ? `Department_Page.Department_Search.Dialog_Header`
          : `Department_Page.Department_Filter.Dialog_Header`
      );
      searchDepartmentDialog.departmentPathMap = this.departmentPathMap;
      searchDepartmentDialog.isNoSearchResult = departments.length === 0;
      this.subscription.add(
        searchDepartmentDialog.cancel.subscribe(() => {
          modalRef.close();
        })
      );

      this.subscription.add(
        searchDepartmentDialog.clickItemResult.subscribe((department) => {
          modalRef.close();
          this.onClickItemResult(department);
        })
      );
    }
  }

  private addIdToMeatballActionButton(): void {
    const meatBallBtnElementList = document.querySelectorAll(
      '.icon-meatball.meatball-action'
    );

    if (!meatBallBtnElementList) {
      return;
    }

    meatBallBtnElementList.forEach((element: HTMLElement, index: number) => {
      element.id = `meatball`;
    });
  }

  private convertDepartment(
    department: Department,
    destinationDepartment: any
  ): any {
    destinationDepartment.departmentName = department.name;
    destinationDepartment.identity = department.identity;
    destinationDepartment.parentDepartmentId = department.parentDepartmentId;
    destinationDepartment.organizationNumber = department.organizationNumber;
    destinationDepartment.address = department.address;
    destinationDepartment.jsonDynamicAttributes =
      department.jsonDynamicAttributes;
    destinationDepartment.levels = department.levels;

    return destinationDepartment;
  }

  private buildDepartmentDtoFromSubmittedForm(
    submittedData: Department
  ): Department {
    let dataChanged = _.cloneDeep(this.currentSelectedDepartment);

    dataChanged = {
      ...dataChanged,
      organizationNumber: submittedData.organizationNumber,
      address: submittedData.address,
      name: submittedData.name,
      identity: {
        extId:
          submittedData && submittedData.identity
            ? submittedData.identity.extId
            : null,
        ownerId: this.currentSelectedDepartment.identity.ownerId,
        customerId: this.currentSelectedDepartment.identity.customerId,
        archetype: this.currentSelectedDepartment.identity.archetype,
        id: this.currentSelectedDepartment.identity.id
      },
      jsonDynamicAttributes: {
        cluster:
          submittedData.jsonDynamicAttributes &&
          submittedData.jsonDynamicAttributes.cluster
            ? submittedData.jsonDynamicAttributes.cluster
            : '',
        clusterSuperintendent:
          submittedData.jsonDynamicAttributes &&
          submittedData.jsonDynamicAttributes.clusterSuperintendent
            ? submittedData.jsonDynamicAttributes.clusterSuperintendent
            : '',
        zone:
          submittedData.jsonDynamicAttributes &&
          submittedData.jsonDynamicAttributes.zone
            ? submittedData.jsonDynamicAttributes.zone
            : '',
        zoneDirector:
          submittedData.jsonDynamicAttributes &&
          submittedData.jsonDynamicAttributes.zoneDirector
            ? submittedData.jsonDynamicAttributes.zoneDirector
            : '',
        typeOfOrganizationUnits:
          submittedData.jsonDynamicAttributes &&
          submittedData.jsonDynamicAttributes.typeOfOrganizationUnits
            ? submittedData.jsonDynamicAttributes.typeOfOrganizationUnits
            : ''
      },
      levels: submittedData.levels
    };

    if (
      submittedData.jsonDynamicAttributes &&
      submittedData.jsonDynamicAttributes.showInSignUpForm &&
      submittedData.jsonDynamicAttributes.showInSignUpForm.length > 0
    ) {
      dataChanged.jsonDynamicAttributes.showInSignUpForm =
        submittedData.jsonDynamicAttributes.showInSignUpForm[0];
    }

    return dataChanged;
  }

  private initFilterOptions(departmentTypes: DepartmentType[]): void {
    this.departmentTypes = departmentTypes;
    this.departmentTypeTitle = this.translateAdapterService.getValueImmediately(
      `Department_Page.Filter_Section.Filter_Options.Education_Level`
    );

    this.departmentFilterOptions = [
      new DepartmentFilterGroupModel({
        name: this.departmentTypeTitle,
        options: this.departmentTypes.map((departmentType) => {
          return new DepartmentFilterOption({
            isSelected: false,
            value: this.getLocalizedText(departmentType.localizedData),
            objectId: departmentType.identity.id
          });
        }),
        groupConstant: GroupFilterConst.DEPARTMENT_TYPE
      })
    ];
  }

  private initFilterObject(): void {
    this.departmentQueryObject = new DepartmentQueryModel({
      includeParent: false,
      includeChildren: true,
      maxChildrenLevel: 1,
      countChildren: true,
      countUser: true,
      includeDepartmentType: true
    });
  }

  private getLocalizedText(localizedData: LocalizedDataItem[]): string {
    const index = localizedData.findIndex(
      (item) => item.languageCode === this.currentLanguageCode
    );
    if (index > findIndexCommon.notFound) {
      return localizedData[index].fields[0].localizedText;
    }
  }

  private initButtonCondition(): void {
    this.buttonCondition = new CxTreeButtonCondition({
      enableAdd: (department: Department) => {
        const departmentRootId = 1;
        if (
          department &&
          department.entityStatus &&
          this.hasPermissionToActionsOnDepartmentHierachy &&
          !department.entityStatus.externallyMastered &&
          department.parentDepartmentId === departmentRootId
        ) {
          return !department.entityStatus.externallyMastered;
        }

        return false;
      },
      enableEdit: (department: Department) => {
        return (
          department &&
          this.hasPermissionToActionsOnDepartmentHierachy &&
          department.identity &&
          department.identity.archetype &&
          department.identity.archetype !== 'Unknown'
        );
      },
      enableMove: () => false,
      enableRemove: (department: Department) => {
        if (
          department.entityStatus &&
          !department.entityStatus.externallyMastered &&
          this.hasPermissionToActionsOnDepartmentHierachy &&
          department.userCount === 0 &&
          department.childrenCount === 0
        ) {
          return !department.entityStatus.externallyMastered;
        }

        return false;
      }
    });
  }

  private get hasPermissionToActionsOnDepartmentHierachy(): boolean {
    return this.currentUser.hasPermission(
      SAM_PERMISSIONS.CRUDinOrganisationManagement
    );
  }

  private getDepartments(fromDepartmentId: number): void {
    this.cxGlobalLoaderService.showLoader();
    const departmentQueryCloned = _.clone(this.departmentQueryObject);
    this.subscription.add(
      this.departmentHierarchicalService
        .getHierarchy(fromDepartmentId, departmentQueryCloned)
        .subscribe(
          (departments: any) => {
            if (departments) {
              departments.map((department) => {
                return new Department(department);
              });
            }
            if (this.isLoadingDepartment) {
              this.loadMoreDepartments(departments);
            } else {
              this.flatDepartmentsArray = _.uniqBy(
                this.flatDepartmentsArray.concat(departments),
                'identity.id'
              );
            }
            this.flatDepartmentsArray.forEach((item) => {
              this.updateLevelOfEducationMap(item);
            });
            this.cxGlobalLoaderService.hideLoader();
            this.isNoDepartment = !departments || departments.length === 0;
            this.changeDetectorRef.detectChanges();

            this.addIdToMeatballActionButton();

            if (this.isFiltering) {
              this.scrollToSelectedDepartment();
            }
          },
          (error) => {
            this.cxGlobalLoaderService.hideLoader();
          },
          () => {
            this.isDetectExpandTree = false;
          }
        )
    );
  }

  private updateLevelOfEducationMap(department: Department): void {
    this.levelOfEducationMap[department.identity.id] =
      department.levels && department.levels.length > 0
        ? this.getPropLocalizedData(department.levels, this.currentLanguageCode)
        : '';
  }

  private getPropLocalizedData(
    levels: any[],
    languageCode: string,
    field: string = 'Name'
  ): string[] {
    return levels
      .map((data) => {
        return SurveyUtils.getPropLocalizedData(
          data.localizedData,
          field,
          languageCode
        );
      })
      .map((stringData: string) => {
        return stringData
          .trim()
          .toLowerCase()
          .replace(/\w\S*/g, (word) =>
            word.replace(/^\w/, (character) => ' ' + character.toUpperCase())
          );
      });
  }

  private scrollToSelectedDepartment(): void {
    const selectedDepartmentElement = document.querySelector(
      '.cx-extensive-tree__display-area--selected'
    );

    if (selectedDepartmentElement) {
      this.isFiltering = false;

      selectedDepartmentElement.scrollIntoView({
        behavior: 'smooth',
        block: 'end',
        inline: 'nearest'
      });
    }
  }

  private checkEmptyFilter(): boolean {
    const notHaveFilter = this.departmentFilterOptions[0].options.filter(
      (item) => item.isSelected
    );

    return notHaveFilter.length > 0;
  }

  private checkDepartmentType(
    department: Department
  ): {
    isSchoolDepartment: boolean;
    isZoneDepartment: boolean;
    isClusterDepartment: boolean;
    isBranchDepartment: boolean;
  } {
    const departmentUnitType =
      department.organizationalUnitTypes &&
      department.organizationalUnitTypes.length > 0
        ? department.organizationalUnitTypes.map((item) => item.identity.extId)
        : '';
    const isSchoolDepartment = departmentUnitType.includes(
      organizationUnitLevelsConst.School
    );
    const isZoneDepartment = departmentUnitType.includes(
      organizationUnitLevelsConst.Branch_Zone
    );
    const isClusterDepartment = departmentUnitType.includes(
      organizationUnitLevelsConst.Cluster
    );

    const isBranchDepartment = departmentUnitType.includes(
      organizationUnitLevelsConst.Branch_Zone
    );

    return {
      isSchoolDepartment,
      isZoneDepartment,
      isClusterDepartment,
      isBranchDepartment
    };
  }

  private getEducationLevels(): void {
    const educationLevelArchetype = 'Level';
    this.subscription.add(
      this.departmentHierarchicalService
        .getDepartmentTypes(educationLevelArchetype)
        .subscribe((departmentTypes: DepartmentType[]) => {
          if (departmentTypes) {
            this.initFilterOptions(departmentTypes);
          }
        })
    );
  }

  private getDepartmentTypeList(): void {
    this.subscription.add(
      this.departmentHierarchicalService
        .getDepartmentTypes('OrganizationalUnitType')
        .subscribe((departmentTypes: DepartmentType[]) => {
          if (departmentTypes) {
            this.departmentTypeList = departmentTypes;
          }
        })
    );
  }
  private getPathOfDepartment(departmentArray: any[], department: any): any {
    const parentDepartment = departmentArray.find(
      (element: any) => element.identity.id === department.parentDepartmentId
    );
    if (!parentDepartment) {
      return department.departmentName;
    } else {
      return this.getPathOfDepartment(departmentArray, parentDepartment).concat(
        `/${department.departmentName}`
      );
    }
  }
  private getRouteOfDepartment(
    departmentArray: any[],
    department: any,
    newObject: any
  ): void {
    const parentDepartment = departmentArray.find(
      (element: any) => element.identity.id === department.parentDepartmentId
    );
    if (!parentDepartment) {
      newObject[department.identity.id] = department.identity.id;
    } else {
      newObject[parentDepartment.identity.id] = parentDepartment.identity.id;
      this.getRouteOfDepartment(departmentArray, parentDepartment, newObject);
    }
  }

  private handleErrorWhenAddDepartment = (
    httpErrorResponse: HttpErrorResponse
  ) => {
    this.cxGlobalLoaderService.hideLoader();
    if (
      httpErrorResponse &&
      httpErrorResponse.error &&
      httpErrorResponse.error.errorCode ===
        Constant.VALIDATION_DEPARTMENT_EXTID_EXISTS
    ) {
      this.toastr.error(
        `Failed to add Organization Unit, duplicated Organisation Code`
      );
    } else {
      this.toastr.error(`Failed to add Organization Unit`);
    }
  };
}
