import {
  Component,
  OnInit,
  Input,
  ViewEncapsulation,
  Output,
  EventEmitter,
  ChangeDetectorRef,
} from '@angular/core';
import { BasePresentationComponent } from '../component.abstract';

@Component({
  selector: 'icon-button',
  templateUrl: './icon-button.component.html',
  styleUrls: ['./icon-button.component.scss'],
  encapsulation: ViewEncapsulation.Emulated,
})
export class IconButtonComponent extends BasePresentationComponent {
  @Input() visible = true;
  @Input() text: string;
  @Input() haveNotification: boolean;
  @Input() iconClass: string;
  @Input() backgroundColor: string;
  @Input() alignElementCenter = true;
  constructor(changeDectectorRef: ChangeDetectorRef) {
    super(changeDectectorRef);
  }
}
