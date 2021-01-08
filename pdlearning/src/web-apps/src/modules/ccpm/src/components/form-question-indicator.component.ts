import { BaseComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'form-question-indicator',
  templateUrl: './form-question-indicator.component.html'
})
export class FormQuestionIndicatorComponent extends BaseComponent {
  public questionNumberList: number[] = [];
  public _totalQuestionCount: number = 0;

  @Input() public selectedQuestionNumber: number | undefined;
  @Input() public set totalQuestionCount(v: number) {
    this._totalQuestionCount = v;
    this.questionNumberList = Utils.range(1, this.totalQuestionCount);
  }
  public get totalQuestionCount(): number {
    return this._totalQuestionCount;
  }
  @Output() public selectedQuestionNumberChange: EventEmitter<number> = new EventEmitter<number>();

  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public isActive(questionNumber: number): boolean {
    return this.selectedQuestionNumber === questionNumber;
  }

  public onIndicatorBtnClicked(questionNumber: number): void {
    this.selectedQuestionNumber = questionNumber;
    this.selectedQuestionNumberChange.emit(questionNumber);
  }
}
