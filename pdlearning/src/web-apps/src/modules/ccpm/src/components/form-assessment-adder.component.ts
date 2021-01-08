import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'form-assessment-adder',
  templateUrl: './form-assessment-adder.component.html'
})
export class FormAssessmentAdderComponent extends BaseComponent {
  @Input() public disabled: boolean = false;
  @Input() public title: string = 'Add';
  @Output() public onAddNewClick: EventEmitter<void> = new EventEmitter<void>();

  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public onAddNewAssessment(): void {
    this.onAddNewClick.emit();
  }
}
