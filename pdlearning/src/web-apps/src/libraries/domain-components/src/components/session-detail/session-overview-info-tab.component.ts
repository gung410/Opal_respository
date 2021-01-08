import { BaseComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import {
  BlockoutDateRepository,
  Course,
  DigitalContent,
  DigitalContentStatus,
  GetBlockoutDateDependenciesModel,
  IGetBlockoutDateDependenciesRequest,
  MetadataTagModel,
  SearchClassRunType,
  TaggingRepository
} from '@opal20/domain-api';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Subscription, combineLatest } from 'rxjs';

import { BlockOutDateDependenciesDetailDialog } from '../blockout-date-dependencies-detail-dialog/blockout-date-dependencies-detail-dialog.component';
import { BlockoutDateViewModel } from './../../models/blockout-date-view.model';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { DigitalContentReferenceDialog } from '../digital-content-reference-dialog/digital-content-reference-dialog.component';
import { FormGroup } from '@angular/forms';
import { OpalDialogService } from '@opal20/common-components';
import { SessionDetailMode } from './../../models/session-detail-mode.model';
import { SessionDetailViewModel } from './../../view-models/session-detail-view.model';
@Component({
  selector: 'session-overview-info-tab',
  templateUrl: './session-overview-info-tab.component.html'
})
export class SessionOverviewInfoTabComponent extends BaseComponent {
  @Input() public form: FormGroup;
  @Input() public mode: SessionDetailMode | undefined;
  @Input() public classRunSearchType: SearchClassRunType;
  @Input() public isRescheduleMode: boolean = false;
  @Input() public forModule: 'lmm' | 'cam' | string;
  @Input() public set course(v: Course) {
    this._course = v;
    if (this.initiated) {
      this.checkBlockoutDateDependencies();
    }
  }

  public get course(): Course {
    return this._course;
  }

  @Input() public session: SessionDetailViewModel;

  @Output('preRecordClipChange') public preRecordClipChangeEvent: EventEmitter<DigitalContent | null> = new EventEmitter();

  @Output('usePreRecordClipChange') public usePreRecordClipChangeEvent: EventEmitter<boolean> = new EventEmitter();

  public SessionDetailMode: typeof SessionDetailMode = SessionDetailMode;
  public SearchClassRunType: typeof SearchClassRunType = SearchClassRunType;
  public blockOutDates: BlockoutDateViewModel[] = [];

  private _course: Course = new Course();
  private metadataTagDict: Dictionary<MetadataTagModel> = {};
  private _loadDataSubs = new Subscription();
  private _blockOutDateSubs = new Subscription();
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private blockOutDateRepository: BlockoutDateRepository,
    private taggingRepository: TaggingRepository,
    private opalDialogService: OpalDialogService
  ) {
    super(moduleFacadeService);
  }

  public onSessionDateChanged(sessionDate: Date): void {
    if (Utils.isDifferent(this.session.sessionDate, sessionDate)) {
      this.session.sessionDate = sessionDate;
      this.checkBlockoutDateDependencies();
    }
  }

  public viewDetailBlockoutDateDependencies(): void {
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(BlockOutDateDependenciesDetailDialog, {
      availableBlockOutDates: this.blockOutDates
    });
  }

  public displaySelectDigitalContentDialog(): void {
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(
      DigitalContentReferenceDialog,
      {
        withinDownloadableContent: true,
        filterByExtensions: ['mp4'],
        filterByStatuses: [DigitalContentStatus.Published]
      },
      {
        maxWidth: '95vw',
        maxHeight: '95vh',
        width: '80vw',
        height: '80vh',
        borderRadius: '0'
      }
    );
    this.subscribe(dialogRef.result, (data: DigitalContent) => {
      if (data && data instanceof DigitalContent) {
        this.preRecordClipChangeEvent.emit(data);
      }
    });
  }

  public onUsePreRecordClipChanged(value: boolean): void {
    this.usePreRecordClipChangeEvent.emit(value);
  }

  public removePreRecordClip(): void {
    this.session.preRecordClip = null;
    this.preRecordClipChangeEvent.emit(this.session.preRecordClip);
  }

  protected onInit(): void {
    this.loadData();
  }

  private loadData(): void {
    this._loadDataSubs.unsubscribe();
    const taggingObs = this.taggingRepository.loadAllMetaDataTags();
    this._loadDataSubs = combineLatest(taggingObs)
      .pipe(this.untilDestroy())
      .subscribe(([metadataTags]) => {
        this.metadataTagDict = Utils.toDictionary(metadataTags, p => p.id);
        this.checkBlockoutDateDependencies();
      });
  }

  private checkBlockoutDateDependencies(): void {
    if (!this.session.sessionDate || this.course.serviceSchemeIds.length === 0) {
      return;
    }
    this._blockOutDateSubs.unsubscribe();
    this._blockOutDateSubs = this.blockOutDateRepository
      .loadBlockoutDateDependencies(<IGetBlockoutDateDependenciesRequest>{
        fromDate: this.session.sessionDate,
        serviceSchemes: this.course.serviceSchemeIds
      })
      .pipe(this.untilDestroy())
      .subscribe((blockOutDateDependencies: GetBlockoutDateDependenciesModel) => {
        this.blockOutDates = blockOutDateDependencies.matchedBlockoutDates.map(p =>
          BlockoutDateViewModel.createFromModel(p, null, {}, this.metadataTagDict)
        );
      });
  }
}
