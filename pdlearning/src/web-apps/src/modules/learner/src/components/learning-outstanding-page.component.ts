import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { IGetMyOutstandingTaskRequest, MyOutstandingTaskApiService, OutstandingTask } from '@opal20/domain-api';

const pageSize = 10;
@Component({
  selector: 'learning-outstanding-page',
  templateUrl: './learning-outstanding-page.component.html'
})
export class LearningOutstandingPageComponent extends BaseComponent implements OnInit {
  @Output()
  public outStandingTaskClick: EventEmitter<OutstandingTask> = new EventEmitter<OutstandingTask>();
  @Output()
  public outStandingTaskBackClick: EventEmitter<void> = new EventEmitter<void>();

  public outstandingTasks: OutstandingTask[] = [];
  public totalCount: number = 0;

  constructor(protected moduleFacadeService: ModuleFacadeService, private myOutstandingTaskApiService: MyOutstandingTaskApiService) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.myOutstandingTaskApiService
      .getOutstandingTasks(<IGetMyOutstandingTaskRequest>{
        maxResultCount: pageSize,
        skipCount: 0
      })
      .then(result => {
        this.totalCount = result.totalCount;
        this.outstandingTasks = result.items;
      });
  }

  public onBackClicked(): void {
    this.outStandingTaskBackClick.emit();
  }

  public onOutStandingClicked(outstandingTask: OutstandingTask): void {
    this.outStandingTaskClick.emit(outstandingTask);
  }
}
