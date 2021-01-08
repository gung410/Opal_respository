import { ColDef, ColumnApi, GridOptions } from '@ag-grid-community/core';
import { LPLNameRendererComponent } from 'app/organisational-development/learning-plan-list/name-renderer.component';
// tslint:disable-next-line:max-line-length
import { LinkViewButtonRendererComponent } from 'app/shared/components/ag-grid-renderer/link-view-button/link-view-button-renderer.component';
import { LNAStatusRendererComponent } from 'app/shared/components/ag-grid-renderer/lna-status-renderer/lna-status-renderer.component';
import { PdPlanStatusRendererComponent } from 'app/shared/components/ag-grid-renderer/pd-plan-status/pdplan-status-renderer.component';
import { ApplicationDateCellRendererComponent } from '../ag-grid-renderer/application-date-cell-renderer.component';
import { AssessmentStatusCellRendererComponent } from '../ag-grid-renderer/assessment-status-cell-renderer.component';
import { ClassRunCellRendererComponent } from '../ag-grid-renderer/class-run-cell-renderer.component';
import { CourseCellRendererComponent } from '../ag-grid-renderer/course-cell-renderer.component';
import { DepartmentCellRendererComponent } from '../ag-grid-renderer/department-cell-renderer.component';
import { GroupActionRendererComponent } from '../ag-grid-renderer/group-action-renderer.component';
import { GroupCellRendererComponent } from '../ag-grid-renderer/group-cell-renderer.component';
import { LearnerCellRendererComponent } from '../ag-grid-renderer/learner-cell-renderer.component';
import { MassNominationCellRendererComponent } from '../ag-grid-renderer/mass-nomination-cell-renderer.component';
import { ReasonCellRendererComponent } from '../ag-grid-renderer/reason-cell-renderer.component';
import { StatusCellRendererComponent } from '../ag-grid-renderer/status-cell-renderer.component';
import { ApprovalTargetEnum } from '../models/approval.enum';
import { ApprovalColumnDefConstants } from './approval-grid-column.constant';
import { ApprovalConstants } from './approval-page.constant';

export class ApprovalAgGridHelper {
  static getGridOptions(
    approvalTarget: ApprovalTargetEnum,
    parentComponent: any
  ): GridOptions {
    const columnDefs = ApprovalAgGridHelper.getColDef(approvalTarget);
    const defaultColDef = ApprovalAgGridHelper.getDefaultColDef(approvalTarget);
    const frameworkComponents = ApprovalAgGridHelper.getFrameworkComponents();
    const rowSelection = 'multiple';
    const overlayNoRowsTemplate = `<div class="grid-nodata">No data</div>`;
    const rowHeight = 90;
    const rowGroupPanelShow = 'always';
    const suppressRowClickSelection = true;
    const context = { componentParent: parentComponent };

    return {
      context,
      rowSelection,
      columnDefs,
      defaultColDef,
      frameworkComponents,
      overlayNoRowsTemplate,
      suppressRowClickSelection,
      rowHeight,
      rowGroupPanelShow,
    };
  }

  private static getColDef(approvalTarget: ApprovalTargetEnum): ColDef[] {
    return ApprovalColumnDefConstants.COL_DEFS_MAP[approvalTarget];
  }

  private static getDefaultColDef(approvalTarget: ApprovalTargetEnum): ColDef {
    const isFirstColumn = (params: {
      columnApi: ColumnApi;
      column: any;
    }): boolean =>
      params.columnApi.getAllDisplayedColumns()[0] === params.column;

    return {
      headerCheckboxSelection: ApprovalConstants.RESTRICT_SINGLE_SELECT_TARGET_ENUMS.includes(
        approvalTarget
      )
        ? false
        : isFirstColumn,
      checkboxSelection: isFirstColumn,
      resizable: true,
    };
  }

  private static getFrameworkComponents(): any {
    return {
      learnerCellRenderer: LearnerCellRendererComponent,
      groupCellRenderer: GroupCellRendererComponent,
      departmentCellRenderer: DepartmentCellRendererComponent,
      courseCellRenderer: CourseCellRendererComponent,
      classRunCellRenderer: ClassRunCellRendererComponent,
      applicationDateCellRenderer: ApplicationDateCellRendererComponent,
      reasonCellRenderer: ReasonCellRendererComponent,
      learningPlanNameRenderer: LPLNameRendererComponent,
      actionGroupRenderer: GroupActionRendererComponent,
      massNominationCellRenderer: MassNominationCellRendererComponent,
      statusCellRenderer: StatusCellRendererComponent,
      assessmentStatusCellRenderer: AssessmentStatusCellRendererComponent,
      LNAStatusRenderer: LNAStatusRendererComponent,
      PDPStatusRenderer: PdPlanStatusRendererComponent,
      linkViewButtonRenderer: LinkViewButtonRendererComponent,
    };
  }
}
