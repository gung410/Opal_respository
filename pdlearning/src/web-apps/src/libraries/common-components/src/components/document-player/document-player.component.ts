import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'document-player',
  templateUrl: './document-player.component.html'
})
export class DocumentPlayerComponent extends BaseComponent {
  @Input() public url: string;

  constructor(moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }
}
