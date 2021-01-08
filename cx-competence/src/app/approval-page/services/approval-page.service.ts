import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'app-auth/auth.service';
import { IDictionary } from 'app-models/dictionary';
import { PagingResponseModel } from 'app-models/user-management.model';
import { CommentEventEntity } from 'app-services/comment-event.constant';
import { CommentService } from 'app-services/comment.service';
import {
  CommentResultDto,
  CommentTagTypeEnum,
} from 'app/individual-development/cx-comment/comment.model';
import { OdpActivity } from 'app/organisational-development/learning-plan-detail/odp.constant';
import {
  ApprovalTargetEnum,
  ChangeNominationStatusTargetEnum,
} from '../models/approval.enum';
import { IdpApprovalPageService } from './idp-approval-page.service';
import { OdpApprovalPageService } from './odp-approval-page.services';

@Injectable()
export class ApprovalPageService {
  constructor(
    private authService: AuthService,
    private idpApprovalPageService: IdpApprovalPageService,
    private odpApprovalPageService: OdpApprovalPageService,
    private commentService: CommentService,
    private translateService: TranslateService
  ) {}

  getGridDataAsync(
    approvalTarget: ApprovalTargetEnum,
    pageIndex: number = 0,
    pageSize: number = 10,
    queryParams: IDictionary<unknown> = {}
  ): Promise<PagingResponseModel<any>> {
    switch (approvalTarget) {
      // IDP cases
      case ApprovalTargetEnum.ClassRegistration:
        return this.idpApprovalPageService.getClassRegistrations(
          pageIndex,
          pageSize,
          queryParams
        );
      case ApprovalTargetEnum.ClassWidthdrawal:
        return this.idpApprovalPageService.getClassWithDrawals(
          pageIndex,
          pageSize,
          queryParams
        );
      case ApprovalTargetEnum.ClassChangeRequest:
        return this.idpApprovalPageService.getClassChangeRequests(
          pageIndex,
          pageSize,
          queryParams
        );
      case ApprovalTargetEnum.AdhocNominations: // TODO
      case ApprovalTargetEnum.AdhocNominationsLearner:
        return this.idpApprovalPageService.getAdhocNominateLearnerRequests(
          pageIndex,
          pageSize
        );
      case ApprovalTargetEnum.AdhocNominationsGroup:
        return this.idpApprovalPageService.getAdhocNominateGroupRequests(
          pageIndex,
          pageSize
        );
      case ApprovalTargetEnum.AdhocNominationDepartment:
        return this.idpApprovalPageService.getAdhocNominateDepartmentRequests(
          pageIndex,
          pageSize
        );
      case ApprovalTargetEnum.AdhocMassNomination:
        return this.idpApprovalPageService.getAdhocMassNominateRequests(
          pageIndex,
          pageSize
        );
      case ApprovalTargetEnum.LNA:
        return this.idpApprovalPageService.getLearningNeedRequests(
          this.authService.userDepartmentId,
          pageIndex,
          pageSize
        );
      case ApprovalTargetEnum.PDPlan:
        return this.idpApprovalPageService.getPdPlanRequests(
          this.authService.userDepartmentId,
          pageIndex,
          pageSize
        );
      case ApprovalTargetEnum.SelfAssignPDO:
        return this.idpApprovalPageService.getPendingPDORequests(
          pageIndex,
          pageSize
        );

      // ODP cases
      case ApprovalTargetEnum.LearningPlan:
        return this.odpApprovalPageService.getLearningPlanRequests(
          pageIndex,
          pageSize
        );
      case ApprovalTargetEnum.LearningDirection:
        return this.odpApprovalPageService.getLearningDirectionRequests(
          pageIndex,
          pageSize
        );
      case ApprovalTargetEnum.Nominations:
      case ApprovalTargetEnum.NominationsLearner:
        return this.odpApprovalPageService.getNominateLearnerRequests(
          pageIndex,
          pageSize
        );
      case ApprovalTargetEnum.NominationsGroup:
        return this.odpApprovalPageService.getNominateGroupRequests(
          pageIndex,
          pageSize
        );
      case ApprovalTargetEnum.NominationDepartment:
        return this.odpApprovalPageService.getNominateDepartmentRequests(
          pageIndex,
          pageSize
        );
      case ApprovalTargetEnum.MassNomination:
        return this.odpApprovalPageService.getMassNominateRequests(
          pageIndex,
          pageSize
        );
      default:
        return undefined;
    }
  }

  async approveRequest(
    ids: any[],
    approvalTarget: ApprovalTargetEnum
  ): Promise<boolean> {
    switch (approvalTarget) {
      // IDP
      case ApprovalTargetEnum.ClassRegistration:
        return await this.idpApprovalPageService.approveClassRegistration(ids);
      case ApprovalTargetEnum.ClassWidthdrawal:
        return await this.idpApprovalPageService.approveClassWithDrawal(ids);
      case ApprovalTargetEnum.ClassChangeRequest:
        return await this.idpApprovalPageService.approveClasChangeRequest(ids);
      case ApprovalTargetEnum.Nominations:
      case ApprovalTargetEnum.AdhocNominationsLearner:
        return await this.idpApprovalPageService.approveAdhocNominateRequest(
          ids,
          ChangeNominationStatusTargetEnum.Learner
        );
      case ApprovalTargetEnum.AdhocNominationsGroup:
        return await this.idpApprovalPageService.approveAdhocNominateRequest(
          ids,
          ChangeNominationStatusTargetEnum.Group
        );
      case ApprovalTargetEnum.AdhocNominationDepartment:
        return await this.idpApprovalPageService.approveAdhocNominateRequest(
          ids,
          ChangeNominationStatusTargetEnum.Department
        );
      case ApprovalTargetEnum.AdhocMassNomination:
        return await this.idpApprovalPageService.approveAdhocMassNominateRequest(
          ids,
          ChangeNominationStatusTargetEnum.AdHocMassNomination
        );
      case ApprovalTargetEnum.LNA:
        return await this.idpApprovalPageService.approveLearningNeedRequest(
          ids
        );

      case ApprovalTargetEnum.PDPlan:
        return await this.idpApprovalPageService.approvePdPlanRequest(ids);

      case ApprovalTargetEnum.SelfAssignPDO:
        return await this.idpApprovalPageService.approveSelfAssignPDORequest(
          ids
        );

      // ODP cases
      case ApprovalTargetEnum.LearningPlan:
        await this.odpApprovalPageService.approvePlanRequests(
          ids,
          OdpActivity.Plan
        );

        return true;
      case ApprovalTargetEnum.LearningDirection:
        await this.odpApprovalPageService.approvePlanRequests(
          ids,
          OdpActivity.Direction
        );

        return true;
      case ApprovalTargetEnum.Nominations:
      case ApprovalTargetEnum.NominationsLearner:
        return await this.odpApprovalPageService.approveNominateRequest(
          ids,
          ChangeNominationStatusTargetEnum.Learner
        );
      case ApprovalTargetEnum.NominationsGroup:
        return await this.odpApprovalPageService.approveNominateRequest(
          ids,
          ChangeNominationStatusTargetEnum.Group
        );
      case ApprovalTargetEnum.NominationDepartment:
        return await this.odpApprovalPageService.approveNominateRequest(
          ids,
          ChangeNominationStatusTargetEnum.Department
        );
      case ApprovalTargetEnum.MassNomination:
        return await this.odpApprovalPageService.approveMassNominateRequest(
          ids,
          ChangeNominationStatusTargetEnum.MassNomination
        );
      default:
        return false;
    }
  }

  async rejectRequest(
    ids: any[],
    approvalTarget: ApprovalTargetEnum,
    reason: string
  ): Promise<boolean> {
    switch (approvalTarget) {
      // IDP cases
      case ApprovalTargetEnum.ClassRegistration:
        return await this.idpApprovalPageService.rejectClassRegistration(
          ids,
          reason
        );
      case ApprovalTargetEnum.ClassWidthdrawal:
        return await this.idpApprovalPageService.rejectClassWithDrawal(
          ids,
          reason
        );
      case ApprovalTargetEnum.ClassChangeRequest:
        return await this.idpApprovalPageService.rejectClassChangeRequest(
          ids,
          reason
        );
      case ApprovalTargetEnum.AdhocNominations:
      case ApprovalTargetEnum.AdhocNominationsLearner:
        return await this.idpApprovalPageService.rejectAdhocNominateRequest(
          ids,
          ChangeNominationStatusTargetEnum.Learner,
          reason
        );
      case ApprovalTargetEnum.AdhocNominationsGroup:
        return await this.idpApprovalPageService.rejectAdhocNominateRequest(
          ids,
          ChangeNominationStatusTargetEnum.Group,
          reason
        );
      case ApprovalTargetEnum.AdhocNominationDepartment:
        return await this.idpApprovalPageService.rejectAdhocNominateRequest(
          ids,
          ChangeNominationStatusTargetEnum.Department,
          reason
        );
      case ApprovalTargetEnum.AdhocMassNomination:
        return await this.idpApprovalPageService.rejectAdhocMassNominateRequest(
          ids,
          ChangeNominationStatusTargetEnum.AdHocMassNomination,
          reason
        );
      case ApprovalTargetEnum.LNA:
        const rejectLNACommentDtos = ids.map((p) => {
          return {
            resultExtId: p.extId,
            content: reason,
            tag: CommentTagTypeEnum.Rejection,
          } as CommentResultDto;
        });

        await this.commentService
          .saveComments(
            CommentEventEntity.IdpLearningNeedsAnalysis,
            rejectLNACommentDtos
          )
          .toPromise();

        return await this.idpApprovalPageService.rejectLearningNeedRequest(ids);

      case ApprovalTargetEnum.PDPlan:
        const rejectPdPlanCommentDtos = ids.map((p) => {
          return {
            resultExtId: p.extId,
            content: reason,
            tag: CommentTagTypeEnum.Rejection,
          } as CommentResultDto;
        });

        await this.commentService
          .saveComments(CommentEventEntity.IdpPlan, rejectPdPlanCommentDtos)
          .toPromise();

        return await this.idpApprovalPageService.rejectPdPlanRequest(ids);

      case ApprovalTargetEnum.SelfAssignPDO:
        const rejectPendingPDO = ids.map((p) => {
          return {
            resultExtId: p.extId,
            content: reason,
            tag: CommentTagTypeEnum.Rejection,
          } as CommentResultDto;
        });

        await this.commentService
          .saveComments(CommentEventEntity.IdpPdo, rejectPendingPDO)
          .toPromise();

        return await this.idpApprovalPageService.rejectSelfAssignPDORequest(
          ids
        );

      // ODP cases
      case ApprovalTargetEnum.LearningPlan:
        await this.odpApprovalPageService.rejectPlanRequests(
          ids,
          reason,
          OdpActivity.Plan
        );

        return true;
      case ApprovalTargetEnum.LearningDirection:
        await this.odpApprovalPageService.rejectPlanRequests(
          ids,
          reason,
          OdpActivity.Direction
        );

        return true;
      case ApprovalTargetEnum.Nominations:
      case ApprovalTargetEnum.NominationsLearner:
        return await this.odpApprovalPageService.rejectNominateRequest(
          ids,
          ChangeNominationStatusTargetEnum.Learner,
          reason
        );
      case ApprovalTargetEnum.NominationsGroup:
        return await this.odpApprovalPageService.rejectNominateRequest(
          ids,
          ChangeNominationStatusTargetEnum.Group,
          reason
        );
      case ApprovalTargetEnum.NominationDepartment:
        return await this.odpApprovalPageService.rejectNominateRequest(
          ids,
          ChangeNominationStatusTargetEnum.Department,
          reason
        );
      case ApprovalTargetEnum.MassNomination:
        return await this.odpApprovalPageService.rejectMassNominateRequest(
          ids,
          ChangeNominationStatusTargetEnum.MassNomination,
          reason
        );
      default:
        return false;
    }
  }

  getApproveSuccessMessage(target: ApprovalTargetEnum): string {
    switch (target) {
      case ApprovalTargetEnum.LNA:
      case ApprovalTargetEnum.PDPlan:
        return this.translateService.instant(
          'Common.Message.AcknowledgeSuccessfully'
        ) as string;
      default:
        return this.translateService.instant(
          'Common.Message.ApproveSuccessfully'
        ) as string;
    }
  }

  getApproveFailMessage(target: ApprovalTargetEnum): string {
    switch (target) {
      case ApprovalTargetEnum.LNA:
      case ApprovalTargetEnum.PDPlan:
        return this.translateService.instant(
          'Common.Message.AcknowledgeFail'
        ) as string;
      default:
        return this.translateService.instant(
          'Common.Message.ApproveFail'
        ) as string;
    }
  }

  getRejectSuccessMessage(target: ApprovalTargetEnum): string {
    return this.translateService.instant(
      'Common.Message.RejectSuccessfully'
    ) as string;
  }

  getRejectFailMessage(target: ApprovalTargetEnum): string {
    return this.translateService.instant('Common.Message.RejectFail') as string;
  }
}
