import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import {
  MAT_DIALOG_DATA,
  MatDialog,
  MatDialogRef
} from '@angular/material/dialog';
import {
  CxConfirmationDialogComponent,
  CxGlobalLoaderService
} from '@conexus/cx-angular-common';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { User } from 'app-models/auth.model';
import { FormBuilderService } from 'app-services/form-builder.service';
import { StatusTypeEnum } from 'app/shared/constants/user-status-type.enum';
import { ifValidator } from 'app/shared/validators/if-validator';
import { noContentWhiteSpaceValidator } from 'app/shared/validators/no-content-white-space-validator';
import { TaxonomyRequestActionEnum } from 'app/taxonomy-management/constant/taxonomy-request-action-type.enum';
import { TaxonomyRequestStatus } from 'app/taxonomy-management/constant/taxonomy-request-status.enum';
import {
  FullPathMetadataTagModel,
  MetadataTagModel
} from 'app/taxonomy-management/models/metadata-tag.model';
import { MetadataTypeNode } from 'app/taxonomy-management/models/metadata-type-node.model';
import { TaxonomyRequestItem } from 'app/taxonomy-management/models/taxonomy-request-item.model';
import { TaxonomyRequestItemViewModel } from 'app/taxonomy-management/models/taxonomy-request-item.viewmodel';
import { TaxonomyRequest } from 'app/taxonomy-management/models/taxonomy-request.model';
import { MetadataRequestApiService } from 'app/taxonomy-management/services/metadata-request-api.services';
import { MetadataRequestDialogService } from 'app/taxonomy-management/services/metadata-request-dialog.service';
import {
  UserManagement,
  UserManagementQueryModel
} from 'app/user-accounts/models/user-management.model';
import { UserAccountsDataService } from 'app/user-accounts/user-accounts-data.service';
import { ToastrService } from 'ngx-toastr';
import { Observable, Subscription } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';
import { SAM_PERMISSIONS } from '../../../../shared/constants/sam-permission.constant';
import { Utils } from '../../../../shared/utilities/utils';
import { MetadataTypeDialogComponent } from '../../metadata-type-dialog/metadata-type-dialog.component';
import { UpdateTaxonomyRequestRequest } from './../../../dtos/update-taxonomy-request-request.dto';

export type MetadataDialogMode = 'detail' | 'suggestion' | 'readonly';
@Component({
  selector: 'taxonomy-request-dialog',
  templateUrl: './taxonomy-request-dialog.component.html',
  styleUrls: ['./taxonomy-request-dialog.component.scss']
})
export class TaxonomyRequestDialogComponent implements OnInit, OnDestroy {
  readonly ABBREVIATION_MAX_LENGTH: number = 100;
  readonly REASON_MAX_LENGTH: number = 1000;
  readonly COMMENT_MAX_LENGTH: number = 1000;

  subscription: Subscription = new Subscription();

  get opalTextAreaStyle(): unknown {
    return {
      border: '1x solid #d8dce6',
      boxSizing: 'border-box',
      borderRadius: '5px',
      width: '100%',
      padding: '10px'
    };
  }

  get selectedMetadataType(): MetadataTypeNode {
    return this._selectedMetadataType;
  }
  set selectedMetadataType(metadataTypeNode: MetadataTypeNode) {
    if (metadataTypeNode == null) {
      return;
    }

    this._selectedMetadataType = metadataTypeNode;
    this.processChangesDependingOnMetadataType(metadataTypeNode);
  }

  get selectedMetadata(): MetadataTagModel {
    return this._selectedMetadata;
  }
  set selectedMetadata(metadata: MetadataTagModel) {
    if (metadata == null) {
      return;
    }

    this._selectedMetadata = metadata;
  }

  get metadataRequest(): TaxonomyRequestItemViewModel {
    return this._metadataRequest;
  }
  set metadataRequest(metadataRequest: TaxonomyRequestItemViewModel) {
    if (metadataRequest == null) {
      return;
    }

    this._metadataRequest = metadataRequest;
  }

  fetchUsersFn: (
    searchText: string,
    skipCount: number,
    maxResultCount: number
  ) => Observable<UserManagement[]> = null;

  actionItems: unknown = [
    {
      action: TaxonomyRequestActionEnum.Create,
      actionLabel: 'Add new metadata item'
    },
    {
      action: TaxonomyRequestActionEnum.Update,
      actionLabel: 'Edit selected metadata item'
    },
    {
      action: TaxonomyRequestActionEnum.Delete,
      actionLabel: 'Delete selected metadata item'
    }
  ];

  currentMetadataRequestId: string;

  serviceSchemeItems: MetadataTagModel[] = [];

  metadataItems: MetadataTagModel[] = [];

  metadataRequestForm: FormGroup;

  updateTaxonomyRequestItemRequest: UpdateTaxonomyRequestRequest;

  originTaxonomyRequest: TaxonomyRequest;

  @Input() mode: MetadataDialogMode;

  @Input() currentUser: User;

  private _metadataRequest: TaxonomyRequestItemViewModel = new TaxonomyRequestItemViewModel();
  private _selectedMetadataType: MetadataTypeNode | null;
  private _selectedMetadata: MetadataTagModel | null;

  constructor(
    public dialogRef: MatDialogRef<any>,
    private ngbModal: NgbModal,
    private userAccountsDataService: UserAccountsDataService,
    private fb: FormBuilder,
    private toastrService: ToastrService,
    private dialog: MatDialog,
    private formBuilderSvc: FormBuilderService,
    private cxGlobalLoaderService: CxGlobalLoaderService,
    private metadataReqApiSvc: MetadataRequestApiService,
    private metadataReqDialogSvc: MetadataRequestDialogService,
    @Inject(MAT_DIALOG_DATA) public metadataRequestId: string
  ) {
    this.fetchUsersFn = this.createFetchUsersFn();
    if (metadataRequestId) {
      this.currentMetadataRequestId = metadataRequestId;
    }
  }

  async ngOnInit(): Promise<void> {
    this.createFormBuilderDefinition();
    this.serviceSchemeItems = await this.metadataReqDialogSvc.getServiceScheme();
    if (this.currentMetadataRequestId) {
      this.buildMetadataRequestViewModel(this.currentMetadataRequestId);
    }
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  createFormBuilderDefinition(): void {
    this.metadataRequestForm = this.fb.group({
      action: [''],
      serviceScheme: [
        '',
        [
          ifValidator(
            (control) => this.mode === 'suggestion',
            () => Validators.required
          )
        ]
      ],
      metadataType: [
        '',
        [
          ifValidator(
            (control) => this.mode === 'suggestion',
            () => Validators.required
          )
        ]
      ],
      metadata: [
        '',
        [
          ifValidator(
            (control) => this.mode === 'suggestion',
            () => Validators.required
          )
        ]
      ],
      path: [''],
      approvingOfficer: [
        '',
        [
          ifValidator(
            (control) => !this.currentUser.hasAdminRole(),
            () => Validators.required
          )
        ]
      ],
      metadataName: [
        '',
        [
          ifValidator(
            (control) =>
              this.metadataRequest.action !== TaxonomyRequestActionEnum.Delete,
            () => Validators.required
          ),
          noContentWhiteSpaceValidator
        ]
      ],
      abbreviation: [
        '',
        [
          ifValidator(
            (control) =>
              this.metadataRequest.action !== TaxonomyRequestActionEnum.Delete,
            () => Validators.required
          ),
          noContentWhiteSpaceValidator,
          Validators.maxLength(this.ABBREVIATION_MAX_LENGTH)
        ]
      ],
      reason: ['', [Validators.maxLength(this.REASON_MAX_LENGTH)]],
      level1ApprovalOfficerComment: [
        '',
        [
          ifValidator(
            (control) =>
              this.mode === 'detail' &&
              !this.isDisabledLevel1ApprovalOfficerComment,
            () => Validators.required
          ),
          Validators.maxLength(this.REASON_MAX_LENGTH)
        ]
      ],
      level2ApprovalOfficerComment: [
        '',
        [
          ifValidator(
            (control) =>
              this.mode === 'detail' &&
              !this.isDisabledLevel2ApprovalOfficerComment,
            () => Validators.required
          ),
          Validators.maxLength(this.REASON_MAX_LENGTH)
        ]
      ]
    });
  }

  onActionChanged(action: TaxonomyRequestActionEnum): void {
    this.metadataRequest.action = action;
    this.formBuilderSvc.updateAllValidators(this.metadataRequestForm);
  }

  onServiceSchemeChanged(tagId: string): void {
    if (tagId == null) {
      return;
    }

    if (Utils.isDifferent(this.metadataRequest.serviceScheme, tagId)) {
      this._selectedMetadataType = null;
      this.metadataRequest.metadata = '';
      this.metadataRequest.metadataType = '';
    }
  }

  onMetadataChanged(tagId: string): void {
    if (tagId == null) {
      return;
    }

    this.metadataRequest.path = this.metadataReqDialogSvc.getPathByMetadataId(
      tagId,
      this.metadataRequest.serviceScheme
    );
  }

  onChooseMetadataTypeBtnClicked(): void {
    this.onMetadataTypeDialogOpened();
  }

  onSaveClicked(): void {
    this.updateMetadataRequest();
  }

  onSubmitClicked(): void {
    this.saveMetadataRequest();
  }

  saveMetadataRequest(): void {
    if (!this.isMetadataRequestFormValid()) {
      this.toastrService.error('Please fill in all required fields');

      return;
    }

    const metadataRequestResult = this.standardizedMetadataRequestValue(
      this.metadataRequest.data
    );
    this.dialogRef.close({
      metadataRequest: metadataRequestResult
    });

    // BEGIN DEBUG SECTION (Open comment below for debugging, below code will be removed afterwards)
    // console.log(this.metadataRequest.data);
    // if (this.isMetadataRequestFormValid()) {
    //   console.log('ALLOW TO SUBMIT');

    //   return;
    // }
    // console.log('SUBMIT FAIL');
    // this.formBuilderSvc.getFormValidationErrors(this.metadataRequestForm);
    // END DEBUG SECTION
  }

  updateMetadataRequest(): void {
    if (!this.isMetadataRequestFormValid()) {
      this.toastrService.error('Please fill in all required fields');

      return;
    }

    this.dialogRef.close({
      updateTaxonomyRequestResult: this.getdUpdateTaxonomyRequestItemRequestResult()
    });

    // BEGIN DEBUG SECTION (Open comment below for debugging, below code will be removed afterwards)
    // console.log(this.metadataRequest.data);
    // if (this.isMetadataRequestFormValid()) {
    //   console.log('ALLOW TO SUBMIT');

    //   return;
    // }
    // console.log('SUBMIT FAIL');
    // this.formBuilderSvc.getFormValidationErrors(this.metadataRequestForm);
    // END DEBUG SECTION
  }

  get isDisabledLevel1ApprovalOfficerComment(): boolean {
    return (
      !this.currentUser.hasSecondaryAdminRole() ||
      !this.isDisabledLevel2ApprovalOfficerComment
    );
  }

  get isDisabledLevel2ApprovalOfficerComment(): boolean {
    return !(
      this.currentUser.hasUserAccountAdministrator() ||
      this.currentUser.hasOverallSystemAdministrator()
    );
  }

  onCancelClicked(): void {
    if (
      (this.mode === 'suggestion' &&
        !this.metadataRequest.isDataDifferentForSuggestion) ||
      (this.mode === 'detail' &&
        !this.metadataRequest.isDataDifferentForDetail()) ||
      this.mode === 'readonly'
    ) {
      this.dialogRef.close({});

      return;
    }

    this.showConfirmationDialog(
      () => {
        if (this.mode === 'suggestion') {
          this.saveMetadataRequest();
        }

        if (this.mode === 'detail') {
          this.updateMetadataRequest();
        }
      },
      'Yes',
      'No',
      'Warning',
      'You have unsaved changes, would you like to save it now?',
      () => {
        this.dialogRef.close({});
      }
    );
  }

  onApproveClicked(): void {
    let approveConfirmationContent: string =
      'Are you sure you want to approve this request?';

    if (this.metadataRequest.isDataDifferentForDetail(false)) {
      approveConfirmationContent =
        'You have made some changes. Do you want to approve the request with current changes?';
    }

    this.showConfirmationDialog(
      () => {
        if (!this.isMetadataRequestFormValid()) {
          this.toastrService.error('Please fill in all required fields');

          return;
        }

        this.dialogRef.close({
          approveTaxonomyRequestResult: this.getdUpdateTaxonomyRequestItemRequestResult()
        });
      },
      'Confirm',
      'Cancel',
      'Confirmation',
      approveConfirmationContent
    );
  }

  onRejectClicked(): void {
    this.showConfirmationDialog(
      () => {
        if (!this.isMetadataRequestFormValid()) {
          this.toastrService.error('Please fill in all required fields');

          return;
        }

        this.dialogRef.close({
          rejectTaxonomyRequestResult: this.getdUpdateTaxonomyRequestItemRequestResult()
        });
      },
      'Confirm',
      'Cancel',
      'Confirmation',
      'Are you sure you want to reject this request?'
    );
  }

  get isEnableToSubmit(): boolean {
    return this.isMetadataRequestFormValid();
  }

  get isEnableToSave(): boolean {
    return this.isMetadataRequestFormValid();
  }

  get isEnableToApprove(): boolean {
    return this.isMetadataRequestFormValid();
  }

  get isVisibleToApprove(): boolean {
    return (
      (this.originTaxonomyRequest?.status ===
        TaxonomyRequestStatus.PendingLevel1 &&
        this.currentUser.hasPermission(
          SAM_PERMISSIONS.MetadataApprovePending1st
        )) ||
      (this.originTaxonomyRequest?.status ===
        TaxonomyRequestStatus.PendingLevel2 &&
        this.currentUser.hasPermission(
          SAM_PERMISSIONS.MetadataApprovePending2nd
        ))
    );
  }

  get isEnableToReject(): boolean {
    return this.isMetadataRequestFormValid();
  }

  get isVisibleToReject(): boolean {
    return (
      (this.originTaxonomyRequest?.status ===
        TaxonomyRequestStatus.PendingLevel1 &&
        this.currentUser.hasPermission(
          SAM_PERMISSIONS.MetadataRejectPending1st
        )) ||
      (this.originTaxonomyRequest?.status ===
        TaxonomyRequestStatus.PendingLevel2 &&
        this.currentUser.hasPermission(
          SAM_PERMISSIONS.MetadataRejectPending2nd
        ))
    );
  }

  get isShowCurrentMetadataInfo(): boolean {
    return (
      this.metadataRequest.metadata &&
      (this.metadataRequest.action === TaxonomyRequestActionEnum.Update ||
        this.metadataRequest.action === TaxonomyRequestActionEnum.Delete)
    );
  }

  get isDeleteAction(): boolean {
    return this.metadataRequest.action === TaxonomyRequestActionEnum.Delete;
  }

  private buildMetadataRequestViewModel(taxonomyRequestId: string): void {
    this.cxGlobalLoaderService.showLoader();
    const metadataReqSub = this.metadataReqApiSvc
      .getMetadataRequestById(taxonomyRequestId)
      .pipe(
        switchMap((metadataResponse) => {
          this.originTaxonomyRequest = Utils.cloneDeep(metadataResponse);
          this.initUpdateTaxonomyRequestItemRequest(metadataResponse);

          return TaxonomyRequestItemViewModel.create(
            (ids) =>
              this.userAccountsDataService
                .getUsers(
                  new UserManagementQueryModel({
                    extIds: ids,
                    orderBy: 'firstName asc',
                    userEntityStatuses: [
                      StatusTypeEnum.All.code,
                      StatusTypeEnum.Deactive.code
                    ],
                    pageIndex: 0,
                    pageSize: 0
                  })
                )
                .pipe(map((res) => res.items)),
            metadataResponse as TaxonomyRequest
          );
        })
      )
      .subscribe(
        (taxonomyRequestItemViewModel: TaxonomyRequestItemViewModel) => {
          this.metadataRequest = Utils.cloneDeep(taxonomyRequestItemViewModel);
          if (this.isShowCurrentMetadataInfo) {
            this.metadataRequest.currentMetadataAndAbbreviation = this.metadataReqDialogSvc.getMetadataInfo(
              taxonomyRequestItemViewModel.data.nodeId
            );
          }

          this.formBuilderSvc.updateAllValidators(this.metadataRequestForm);

          this.cxGlobalLoaderService.hideLoader();
        }
      );

    this.subscription.add(metadataReqSub);
  }

  private initUpdateTaxonomyRequestItemRequest(
    taxonomyRequest: TaxonomyRequest
  ): void {
    this.updateTaxonomyRequestItemRequest = new UpdateTaxonomyRequestRequest({
      taxonomyRequestId: taxonomyRequest.id,
      item: {
        taxonomyRequestItemId: taxonomyRequest.taxonomyRequestItems[0].id,
        nodeId: taxonomyRequest.taxonomyRequestItems[0].nodeId,
        metadataName: taxonomyRequest.taxonomyRequestItems[0].metadataName,
        reason: taxonomyRequest.taxonomyRequestItems[0].reason,
        abbreviation: taxonomyRequest.taxonomyRequestItems[0].abbreviation
      },
      comment: ''
    });
  }

  private getdUpdateTaxonomyRequestItemRequestResult(): UpdateTaxonomyRequestRequest {
    return Utils.clone(this.updateTaxonomyRequestItemRequest, (clonedValue) => {
      clonedValue.item.abbreviation = this.metadataRequest.abbreviation;
      clonedValue.item.metadataName = this.metadataRequest.metadataName;
      clonedValue.item.reason = this.metadataRequest.reason;
      // clonedValue.comment =
      //   this.currentUser.hasUserAccountAdministrator ||
      //   this.currentUser.hasOverallSystemAdministrator
      //     ? this.metadataRequest.level2ApprovalOfficerComment
      //     : this.metadataRequest.level1ApprovalOfficerComment;
      clonedValue.comment = this.getCurrentCommentValue;
    });
  }

  private get getCurrentCommentValue(): string {
    return this.isDisabledLevel2ApprovalOfficerComment
      ? this.metadataRequest.level1ApprovalOfficerComment
      : this.metadataRequest.level2ApprovalOfficerComment;
  }

  private standardizedMetadataRequestValue(
    metadataRequest: TaxonomyRequestItem
  ): TaxonomyRequestItem {
    const metadataRequestResult = Utils.cloneDeep(metadataRequest);

    switch (this.metadataRequest.action) {
      case TaxonomyRequestActionEnum.Create:
        return metadataRequestResult;
      case TaxonomyRequestActionEnum.Update:
        return metadataRequestResult;
      case TaxonomyRequestActionEnum.Delete:
        return metadataRequestResult;
      default:
        return metadataRequestResult;
    }
  }

  private isMetadataRequestFormValid(): boolean {
    if (this.mode === 'suggestion') {
      return this.metadataRequestForm.valid;
    }

    if (this.mode === 'detail') {
      return (
        this.metadataRequestForm.valid &&
        this.getCurrentCommentValue &&
        !!this.getCurrentCommentValue.trim().length
      );
    }

    return false;
  }

  private processChangesDependingOnMetadataType(
    newMetadataTypeNode: MetadataTypeNode
  ): void {
    if (
      !Utils.isDifferent(
        this.metadataRequest.data.metadataType,
        newMetadataTypeNode.name
      )
    ) {
      return;
    }

    this.metadataRequest.data.metadataType = newMetadataTypeNode.name;
    this.metadataItems = this.metadataReqDialogSvc.allMetadatas.filter(
      (metadata) =>
        metadata?.groupCode === newMetadataTypeNode.code &&
        metadata?.parentTagId === this.metadataRequest.serviceScheme
    );

    this.metadataItems = FullPathMetadataTagModel.buildFullPathMetadataTag(
      this.metadataItems,
      Utils.toDictionary(
        this.metadataReqDialogSvc.allMetadatas,
        (metadata) => metadata.tagId
      )
    );

    this.metadataRequest.metadata = '';
  }

  private async onMetadataTypeDialogOpened(): Promise<void> {
    const currentServiceSchemeInfo = await this.metadataReqDialogSvc.getServiceSchemeInfoById(
      this.metadataRequest.serviceScheme
    );
    const metadataTypeDialog = this.dialog.open(MetadataTypeDialogComponent, {
      width: '500px',
      data: currentServiceSchemeInfo,
      hasBackdrop: true
    });

    const metadataTypeDialogInstance = metadataTypeDialog.componentInstance;
    metadataTypeDialogInstance.selectedMetadataType = this.selectedMetadataType;

    this.subscription.add(
      metadataTypeDialog.afterClosed().subscribe((result: any) => {
        if (result.metadataType) {
          this.selectedMetadataType = result.metadataType as MetadataTypeNode;
        }
      })
    );
  }

  private createFetchUsersFn(): (
    searchText: string,
    skipCount: number,
    maxResultCount: number
  ) => Observable<UserManagement[]> {
    return (searchText, skipCount, maxResultCount) =>
      this.userAccountsDataService
        .getUsers(
          new UserManagementQueryModel({
            searchKey: searchText,
            orderBy: 'firstName asc',
            parentDepartmentId: [this.currentUser.departmentId],
            userEntityStatuses: [StatusTypeEnum.Active.code],
            pageIndex:
              maxResultCount === 0 ? 1 : skipCount / maxResultCount + 1,
            pageSize: maxResultCount,
            filterOnSubDepartment: true
          })
        )
        .pipe(
          map((usersPaging) => {
            return usersPaging.items;
          })
        );
  }

  private showConfirmationDialog(
    onConfirmed: () => void,
    confirmButton: string,
    cancelButton: string,
    headerName: string,
    content: string,
    onCanceled?: () => void
  ): void {
    const cxConfirmationDialogModalRef = this.ngbModal.open(
      CxConfirmationDialogComponent,
      {
        size: 'sm',
        centered: true
      }
    );
    const cxConfirmationDialogModal = cxConfirmationDialogModalRef.componentInstance as CxConfirmationDialogComponent;
    cxConfirmationDialogModal.cancelButtonText = cancelButton;
    cxConfirmationDialogModal.confirmButtonText = confirmButton;
    cxConfirmationDialogModal.content = content;
    cxConfirmationDialogModal.header = headerName;
    cxConfirmationDialogModal.showCancelButton = true;
    cxConfirmationDialogModal.showCloseButton = false;
    this.subscription.add(
      cxConfirmationDialogModal.confirm.subscribe(() => {
        onConfirmed();
        cxConfirmationDialogModalRef.close();
      })
    );
    this.subscription.add(
      cxConfirmationDialogModal.cancel.subscribe(() => {
        if (onCanceled) {
          onCanceled();
        }
        cxConfirmationDialogModalRef.close();
      })
    );
  }
}
