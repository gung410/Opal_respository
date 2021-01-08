import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'course-criteria-max-participant-select',
  templateUrl: './course-criteria-max-participant-select.component.html'
})
export class CourseCriteriaMaxParticipantSelect extends BaseComponent {
  @Input() public items: object[] = [];
  @Input() public itemsTypeTitle: string = 'n/a';
  @Input() public labelField: string = 'label';
  @Input() public labelFn?: (item: object) => string;
  @Input() public maxParticipantsField: string = 'maxParticipant';
  @Input() public readonly: boolean = false;

  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }
}
