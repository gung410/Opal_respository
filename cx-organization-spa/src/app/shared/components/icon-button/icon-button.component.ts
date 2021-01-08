import {
  ChangeDetectorRef,
  Component,
  Input,
  ViewEncapsulation
} from '@angular/core';

import { BasePresentationComponent } from '../component.abstract';

@Component({
  selector: 'icon-button',
  templateUrl: './icon-button.component.html',
  styleUrls: ['./icon-button.component.scss'],
  encapsulation: ViewEncapsulation.Emulated
})
export class IconButtonComponent extends BasePresentationComponent {
  @Input() visible: boolean = true;
  @Input() text: string;
  @Input() haveNotification: boolean;
  @Input() iconClass: string;
  @Input() backgroundColor: string;
  @Input() alignElementCenter: boolean = true;
  constructor(changeDetectorRef: ChangeDetectorRef) {
    super(changeDetectorRef);
  }
}
