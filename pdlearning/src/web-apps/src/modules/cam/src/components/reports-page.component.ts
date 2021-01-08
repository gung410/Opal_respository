import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { CAM_PERMISSIONS, SystemRoleEnum, UserInfoModel } from '@opal20/domain-api';
import { Component, HostBinding } from '@angular/core';
import { IOpalReportDynamicParams, OpalReportDynamicComponent } from '@opal20/domain-components';

@Component({
  selector: 'reports-page',
  templateUrl: './reports-page.component.html'
})
export class ReportsPageComponent extends BasePageComponent {
  public paramsReportDynamic: IOpalReportDynamicParams | null;

  public static hasViewPermissions(currentUser: UserInfoModel): boolean {
    return (
      currentUser.hasPermissionPrefix(CAM_PERMISSIONS.Reports) ||
      currentUser.hasAdministratorRoles() ||
      currentUser.hasRole(SystemRoleEnum.CourseAdministrator) ||
      currentUser.hasRole(SystemRoleEnum.CourseContentCreator) ||
      currentUser.hasRole(SystemRoleEnum.SchoolContentApprovingOfficer) ||
      currentUser.hasRole(SystemRoleEnum.CourseApprovingOfficer) ||
      currentUser.hasRole(SystemRoleEnum.MOEHQContentApprovingOfficer)
    );
  }

  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  protected onInit(): void {
    this.paramsReportDynamic = OpalReportDynamicComponent.buildCamReportList();
  }
}
