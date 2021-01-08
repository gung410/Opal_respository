import {
  AnnouncementRepository,
  AnnouncementTemplate,
  ISaveAnnouncementDto,
  ISendAnnouncementRequest,
  SearchRegistrationsType,
  UserInfoModel
} from '@opal20/domain-api';
import {
  BaseFormComponent,
  ComponentType,
  IFormBuilderDefinition,
  IGridFilter,
  ModuleFacadeService,
  NotificationType,
  TranslationMessage,
  Utils
} from '@opal20/infrastructure';
import { Component, HostBinding, Input } from '@angular/core';
import {
  ListRegistrationGridDisplayColumns,
  RegistrationFilterComponent,
  RegistrationFilterModel,
  RegistrationViewModel
} from '@opal20/domain-components';
import {
  OpalDialogService,
  futureDateValidator,
  ifValidator,
  requiredAndNoWhitespaceValidator,
  requiredIfValidator,
  validateFutureDateType
} from '@opal20/common-components';
import { filter, map } from 'rxjs/operators';

import { Observable } from 'rxjs';
import { SendAnnouncementViewModel } from '../models/send-announcement-view.model';

@Component({
  selector: 'send-announcement-page',
  templateUrl: './send-announcement-page.component.html'
})
export class SendAnnouncementPageComponent extends BaseFormComponent {
  public filterPopupContent: ComponentType<RegistrationFilterComponent> = RegistrationFilterComponent;
  public get courseId(): string | undefined {
    return this._courseId;
  }

  @Input()
  public set courseId(v: string | undefined) {
    if (Utils.isDifferent(this._courseId, v)) {
      this._courseId = v;
      if (this.initiated) {
        this.announcementVm.courseId = v;
      }
    }
  }

  public get classRunId(): string | undefined {
    return this._classRunId;
  }

  @Input()
  public set classRunId(v: string | undefined) {
    if (Utils.isDifferent(this._classRunId, v)) {
      this._classRunId = v;
      if (this.initiated) {
        this.announcementVm.classrunId = v;
      }
    }
  }

  public fetchAnnouncementTemplatesFn: (
    searchText: string,
    skipCount: number,
    maxResultCount: number
  ) => Observable<AnnouncementTemplate[]>;
  public SearchRegistrationsType: typeof SearchRegistrationsType = SearchRegistrationsType;
  public participantsGridDisplayColumns: ListRegistrationGridDisplayColumns[] = [
    ListRegistrationGridDisplayColumns.selected,
    ListRegistrationGridDisplayColumns.name,
    ListRegistrationGridDisplayColumns.organisation,
    ListRegistrationGridDisplayColumns.learningContentProgress,
    ListRegistrationGridDisplayColumns.noOfAssignmentDone,
    ListRegistrationGridDisplayColumns.attendanceRatioOfPresent,
    ListRegistrationGridDisplayColumns.status
  ];
  public participantSelectedItems: RegistrationViewModel[] = [];
  public announcementVm: SendAnnouncementViewModel = new SendAnnouncementViewModel();
  public currentUser = UserInfoModel.getMyUserInfo();
  public filter: IGridFilter = {
    search: '',
    filter: null
  };
  public searchText: string = '';
  public filterData: RegistrationFilterModel = null;

  private _courseId: string;
  private _classRunId: string;

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private announcementRepository: AnnouncementRepository,
    private opalDialogService: OpalDialogService
  ) {
    super(moduleFacadeService);
    this.fetchAnnouncementTemplatesFn = this.buildFetchAnnouncementTemplatesFn();
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  public onClickReset(): void {
    this.modalService.showConfirmMessage(
      new TranslationMessage(
        this.moduleFacadeService.globalTranslator,
        "You're about to reset this send announcements. Do you want to proceed?"
      ),
      () => {
        this.announcementVm.clearData();
      }
    );
  }

  public onClickPreview(): void {
    this.opalDialogService.openConfirmDialog({
      confirmTitle: this.announcementVm.title,
      confirmMsgHtml: this.announcementVm.message,
      hideNoBtn: true,
      yesBtnText: 'Close'
    });
  }

  public onClickSend(): void {
    this.saveAnnouncement(false);
  }

  public onClickSendAndSave(): void {
    this.saveAnnouncement(true);
  }

  public onDeleteAnnouncementTemplate(id: string): void {
    this.modalService.showConfirmMessage(
      new TranslationMessage(
        this.moduleFacadeService.globalTranslator,
        "You're about to delete this send announcements. Do you want to proceed?"
      ),
      () => {
        this.announcementRepository.deleteAnnouncementTemplate(id).then(_ => {
          this.showNotification(`"${this.announcementVm.data.title}" is successfully deleted`, NotificationType.Success);
          this.announcementVm.selectedAnnouncementTemplateId = null;
        });
      }
    );
  }
  public hasOwnerAnnouncementTemplate(createdBy: string): boolean {
    return createdBy === this.currentUser.id;
  }

  public onSubmitSearch(): void {
    this.filter = {
      ...this.filter,
      search: this.searchText
    };
  }

  public onApplyFilter(data: RegistrationFilterModel): void {
    this.filterData = data;

    this.filter = {
      ...this.filter,
      filter: this.filterData ? this.filterData.convert() : null
    };
  }

  public resetFilter(): void {
    this.filter = {
      ...this.filter,
      search: this.searchText,
      filter: this.filterData ? this.filterData.convert() : null
    };
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition | undefined {
    return {
      formName: 'announcement',
      controls: {
        announcementTitle: {
          defaultValue: null,
          validators: [
            {
              validator: requiredAndNoWhitespaceValidator(),
              validatorType: 'required',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Announcement title must be filled')
            }
          ]
        },
        announcementMessage: {
          defaultValue: null,
          validators: [
            {
              validator: requiredAndNoWhitespaceValidator(),
              validatorType: 'required',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Announcement message must be filled')
            }
          ]
        },
        scheduleDate: {
          defaultValue: new Date(),
          validators: [
            {
              validator: requiredIfValidator(() => !this.announcementVm.isSentNow),
              validatorType: 'required'
            },
            {
              validator: ifValidator(p => !this.announcementVm.isSentNow, () => futureDateValidator(false)),
              validatorType: validateFutureDateType,
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Schedule Date cannot be in the past')
            }
          ]
        }
      }
    };
  }

  private saveAnnouncement(isSaveTemplate: boolean): void {
    this.validate().then(valid => {
      if (valid) {
        if (!this.announcementVm.isSentToAllParticipants && this.announcementVm.participants.length === 0) {
          this.opalDialogService.openConfirmDialog({
            confirmTitle: 'Error',
            confirmMsg: 'You cannot send this announcement. Please choose at least one receiver.',
            hideNoBtn: true,
            yesBtnText: 'OK'
          });
          return;
        }

        this.announcementRepository
          .sendAnnouncement(<ISendAnnouncementRequest>{
            data: <ISaveAnnouncementDto>{
              id: this.announcementVm.id,
              title: this.announcementVm.title,
              base64Message: Utils.encodeBase64(this.announcementVm.message),
              scheduleDate: this.announcementVm.isSentNow ? null : this.announcementVm.scheduleDate,
              registrationIds: this.announcementVm.participants,
              courseId: this.courseId,
              classrunId: this.classRunId,
              saveTemplate: isSaveTemplate,
              isSentToAllParticipants: this.announcementVm.isSentToAllParticipants
            }
          })
          .then(_ => this.showNotification(this.translate('Course Announcement is sent successfully.')));
      }
    });
  }

  private buildFetchAnnouncementTemplatesFn(): (
    searchText: string,
    skipCount: number,
    maxResultCount: number
  ) => Observable<AnnouncementTemplate[]> {
    return (searchText: string, skipCount: number, maxResultCount: number) => {
      return this.announcementRepository.searchAnnouncementTemplate(searchText, skipCount, maxResultCount, false).pipe(map(p => p.items));
    };
  }
}
