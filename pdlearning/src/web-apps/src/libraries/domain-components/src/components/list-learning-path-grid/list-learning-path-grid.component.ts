import { BaseGridComponent, ClipboardUtil, ModuleFacadeService } from '@opal20/infrastructure';
import { CellClickEvent, RowClassArgs } from '@progress/kendo-angular-grid';
import { Component, EventEmitter, Input, Output } from '@angular/core';

import { IRowCallbackModel } from './../../models/row-callback.model';
import { LEARNING_PATH_STATUS_COLOR_MAP } from './../../models/learning-path-status-color-map.model';
import { LearningPathViewModel } from './../../models/learning-path-view.model';
import { ListLearningPathGridComponentService } from './../../services/list-learning-path-grid-component.service';
import { OpalDialogService } from '@opal20/common-components';
import { Subscription } from 'rxjs';
import { WebAppLinkBuilder } from './../../helpers/webapp-link-builder.helper';
@Component({
  selector: 'list-learning-path-grid',
  templateUrl: './list-learning-path-grid.component.html'
})
export class ListLearningPathGridComponent extends BaseGridComponent<LearningPathViewModel> {
  public loading: boolean;
  @Input() public indexActionColumn: number = null;
  @Output('viewLearningPath')
  public viewLearningPathEvent: EventEmitter<LearningPathViewModel> = new EventEmitter<LearningPathViewModel>();

  private _loadDataSub: Subscription = new Subscription();
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public opalDialogService: OpalDialogService,
    public listLearningPathGridComponentService: ListLearningPathGridComponentService
  ) {
    super(moduleFacadeService);
  }

  public loadData(): void {
    this._loadDataSub.unsubscribe();
    this._loadDataSub = this.listLearningPathGridComponentService
      .loadLearningPaths('', this.state.skip, this.state.take, this.checkAll, () => this.selecteds)
      .pipe(this.untilDestroy())
      .subscribe(data => {
        this.gridData = data;
        this.updateSelectedsAndGridData();
      });
  }

  public rowCallback(context: RowClassArgs): IRowCallbackModel {
    return {
      selected: context.dataItem.selected
    };
  }

  public get statusColorMap(): unknown {
    return LEARNING_PATH_STATUS_COLOR_MAP;
  }

  public onGridCellClick(event: CellClickEvent): void {
    // columnIndex isn't action column
    // columnIndex isn't action column
    if (
      event.dataItem instanceof LearningPathViewModel &&
      (this.indexActionColumn == null || event.columnIndex !== this.indexActionColumn)
    ) {
      this.viewLearningPathEvent.emit(event.dataItem);
    }
  }

  public onCopyShareableLearningPathLinkBtnClicked(e: MouseEvent, item: LearningPathViewModel): void {
    e.stopImmediatePropagation();
    this.copyShareableLearningPathLink(item);
  }

  public copyShareableLearningPathLink(item: LearningPathViewModel): void {
    ClipboardUtil.copyTextToClipboard(WebAppLinkBuilder.buildLearningPathDetailUrl(item.id, true));
    this.showNotification('Copied shareable link successfully');
  }

  protected onInit(): void {
    super.onInit();
  }
}
