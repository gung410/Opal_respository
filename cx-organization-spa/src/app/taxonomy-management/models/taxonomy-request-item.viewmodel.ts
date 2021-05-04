import { UserManagement } from 'app/user-accounts/models/user-management.model';
import { combineLatest, Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { Utils } from '../../shared/utilities/utils';
import { TaxonomyRequestActionEnum } from '../constant/taxonomy-request-action-type.enum';
import { TaxonomyRequestItem } from './taxonomy-request-item.model';
import { TaxonomyRequest } from './taxonomy-request.model';

export class TaxonomyRequestItemViewModel {
  get action(): TaxonomyRequestActionEnum {
    return this.data.type;
  }
  set action(newAction: TaxonomyRequestActionEnum) {
    if (newAction == null || !Utils.isDifferent(this.data.type, newAction)) {
      return;
    }

    this.data.type = newAction;
  }

  get serviceScheme(): string {
    return this._serviceScheme;
  }
  set serviceScheme(serviceScheme: string) {
    if (serviceScheme == null) {
      return;
    }

    this._serviceScheme = serviceScheme;
  }

  get metadataType(): string {
    return this.data.metadataType;
  }
  set metadataType(metadataType: string) {
    if (metadataType == null) {
      return;
    }

    this.data.metadataType = metadataType;
  }

  get metadata(): string {
    return this.data.nodeId;
  }
  set metadata(metadata: string) {
    if (metadata == null) {
      return;
    }

    this.data.nodeId = metadata;
  }

  get path(): string {
    return this.data.pathName;
  }
  set path(path: string) {
    if (path == null) {
      return;
    }

    this.data.pathName = path;
  }

  get approvingOfficer(): string {
    return this._approvingOfficer;
  }
  set approvingOfficer(approvingOfficerId: string) {
    if (approvingOfficerId == null) {
      return;
    }

    this._approvingOfficer = approvingOfficerId;
  }

  get metadataName(): string {
    return this.data.metadataName;
  }
  set metadataName(metadataName: string) {
    if (metadataName == null) {
      return;
    }

    this.data.metadataName = metadataName;
  }

  get abbreviation(): string {
    return this.data.abbreviation;
  }
  set abbreviation(abbreviation: string) {
    if (abbreviation == null) {
      return;
    }

    this.data.abbreviation = abbreviation;
  }

  get reason(): string {
    return this.data.reason;
  }
  set reason(reason: string) {
    if (reason == null) {
      return;
    }

    this.data.reason = reason;
  }

  get level1ApprovalOfficerComment(): string {
    return this._level1ApprovalOfficerComment;
  }
  set level1ApprovalOfficerComment(level1ApprovalOfficerComment: string) {
    if (level1ApprovalOfficerComment == null) {
      return;
    }

    this._level1ApprovalOfficerComment = level1ApprovalOfficerComment;
  }

  get level2ApprovalOfficerComment(): string {
    return this._level2ApprovalOfficerComment;
  }
  set level2ApprovalOfficerComment(level2ApprovalOfficerComment: string) {
    if (level2ApprovalOfficerComment == null) {
      return;
    }

    this._level2ApprovalOfficerComment = level2ApprovalOfficerComment;
  }

  get selectedApprovingOfficer(): UserManagement[] {
    return this._selectedApprovingOfficer;
  }
  set selectedApprovingOfficer(newSelectedApprovingOfficer: UserManagement[]) {
    if (newSelectedApprovingOfficer == null) {
      return;
    }

    this._selectedApprovingOfficer = newSelectedApprovingOfficer;
  }

  get isDataDifferentForSuggestion(): boolean {
    return Utils.isDifferent(this.data, this.originalData);
  }

  static create(
    getUsersByIdsFn: (userIds: string[]) => Observable<UserManagement[]>,
    taxonomyRequest: TaxonomyRequest
  ): Observable<TaxonomyRequestItemViewModel> {
    return combineLatest(
      taxonomyRequest.level1ApprovalOfficerId ||
        taxonomyRequest.level2ApprovalOfficerId
        ? getUsersByIdsFn([
            taxonomyRequest.level2ApprovalOfficerId
              ? taxonomyRequest.level2ApprovalOfficerId
              : taxonomyRequest.level1ApprovalOfficerId
          ])
        : of(null)
    ).pipe(
      map(([users]) => {
        return new TaxonomyRequestItemViewModel(
          taxonomyRequest,
          users ? users : null
        );
      })
    );
  }

  currentMetadataAndAbbreviation: string;
  originLevel1ApprovalOfficerComment: string;
  originLevel2ApprovalOfficerComment: string;

  data: TaxonomyRequestItem = new TaxonomyRequestItem();
  originalData: TaxonomyRequestItem = new TaxonomyRequestItem();

  private _serviceScheme: string;
  private _approvingOfficer: string;
  private _selectedApprovingOfficer: UserManagement[];
  private _level1ApprovalOfficerComment: string;
  private _level2ApprovalOfficerComment: string;

  constructor(taxonomyRequest?: TaxonomyRequest, user?: UserManagement[]) {
    if (!taxonomyRequest) {
      this._setDefaultDataValue();

      return;
    }

    this.buildDataValue(taxonomyRequest, user);
  }

  isDataDifferentForDetail(isCommentIncluded: boolean = true): boolean {
    if (isCommentIncluded) {
      const viewData: unknown = {
        ...this.originalData,
        level1ApprovalOfficerComment: this.originLevel1ApprovalOfficerComment,
        level2ApprovalOfficerComment: this.originLevel2ApprovalOfficerComment
      };

      const originalData: unknown = {
        ...this.data,
        level1ApprovalOfficerComment: this.level1ApprovalOfficerComment,
        level2ApprovalOfficerComment: this.level2ApprovalOfficerComment
      };

      return Utils.isDifferent(viewData, originalData);
    }

    return this.isDataDifferentForSuggestion;
  }

  private _setDefaultDataValue(): void {
    this.data.type = this.originalData.type = TaxonomyRequestActionEnum.Update;
  }

  private buildDataValue(
    taxonomyRequest: TaxonomyRequest,
    user?: UserManagement[]
  ): void {
    const metadataRequest = Utils.cloneDeep(
      taxonomyRequest.taxonomyRequestItems[0]
    );

    if (user && user.length) {
      this._selectedApprovingOfficer = user;
      this._approvingOfficer = user[0].identity.extId;
    }

    this.data = Utils.cloneDeep(metadataRequest);
    this.originalData = Utils.cloneDeep(metadataRequest);
    this.level1ApprovalOfficerComment = this.originLevel1ApprovalOfficerComment =
      taxonomyRequest.level1ApprovalOfficerComment;
    this.level2ApprovalOfficerComment = this.originLevel2ApprovalOfficerComment =
      taxonomyRequest.level2ApprovalOfficerComment;
  }
}
