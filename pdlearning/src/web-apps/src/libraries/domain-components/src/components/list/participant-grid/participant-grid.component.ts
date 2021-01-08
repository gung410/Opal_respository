import { BaseGridComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FORM_PARTICIPANT_STATUS_COLOR_MAP, FormParticipantStatus, FormParticipantViewModel } from '@opal20/domain-api';

import { CellClickEvent } from '@progress/kendo-angular-grid';
import { FormParticipantListService } from '../../../services/form-participant-list.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'list-participant-grid',
  templateUrl: './participant-grid.component.html'
})
export class ListParticipantGridComponent extends BaseGridComponent<FormParticipantViewModel> {
  @Input() public formOriginalObjectId: string;
  @Output('viewParticipant') public viewParticipantEvent: EventEmitter<FormParticipantViewModel> = new EventEmitter<
    FormParticipantViewModel
  >();

  public readonly FORM_PARTICIPANT_STATUS = FormParticipantStatus;

  private _loadDataSub: Subscription = new Subscription();

  constructor(public moduleFacadeService: ModuleFacadeService, public formParticipantListService: FormParticipantListService) {
    super(moduleFacadeService);
  }

  public get isAllParticipantCompleted(): boolean {
    if (this.gridData && this.gridData.data) {
      const listUserCompleted = this.gridData.data.filter(user => user.status === FormParticipantStatus.Completed);
      return listUserCompleted.length === this.gridData.data.length;
    }
  }

  public onGridCellClick(event: CellClickEvent): void {
    if (event.dataItem instanceof FormParticipantViewModel && !this.indexActionColumns.includes(event.columnIndex)) {
      this.viewParticipantEvent.emit(event.dataItem);
    }
  }

  public refreshList(): void {
    this.refreshData();
  }

  public displayStatus(status: FormParticipantStatus): string {
    return FORM_PARTICIPANT_STATUS_COLOR_MAP[status].text;
  }

  public onCheckAll(checked: boolean): void {
    this.checkAll = checked;

    // Clear list selected
    if (checked === false) {
      this.selectedItems = [];
      this.selecteds = {};
    }
    this.gridData.data.forEach(item => {
      if (item.status !== FormParticipantStatus.Completed) {
        if (checked) {
          this.selectedItems.push(item);
        }
        this.selecteds[item.id] = checked;
        item.selected = checked;
      }
    });
    this.selectedItemsChangeEvent.emit(this.selectedItems);
  }

  public onCheckItem(checked: boolean, dataItem: FormParticipantViewModel): void {
    this.selecteds[dataItem.id] = checked;
    if (checked) {
      if (!this.selectedItems.includes(dataItem)) {
        this.selectedItems.push(dataItem);
      }
    } else {
      this.selectedItems = Utils.removeFirst(this.selectedItems, dataItem) as FormParticipantViewModel[];
    }

    const listUserCompleted = this.gridData.data.filter(user => user.status === FormParticipantStatus.Completed);

    this.checkAll = this.selectedItems.length === this.gridData.total - listUserCompleted.length;
    this.selectedItemsChangeEvent.emit(this.selectedItems);
  }

  protected onInit(): void {
    super.onInit();
  }

  protected loadData(): void {
    this._loadDataSub.unsubscribe();
    this._loadDataSub = this.formParticipantListService
      .loadFormParticipants(this.formOriginalObjectId, this.state.skip, this.state.take)
      .pipe(this.untilDestroy())
      .subscribe(data => {
        this.gridData = data;
        this.updateSelectedsAndGridData();
      });
  }
}
