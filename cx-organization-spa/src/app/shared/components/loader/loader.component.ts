import {
  ChangeDetectorRef,
  Component,
  Input,
  OnInit,
  ViewEncapsulation
} from '@angular/core';

import { BasePresentationComponent } from '../component.abstract';

@Component({
  selector: 'loader',
  templateUrl: './loader.component.html',
  styleUrls: ['./loader.component.scss'],
  encapsulation: ViewEncapsulation.Emulated
})
export class LoaderComponent extends BasePresentationComponent {
  @Input() loading: boolean = false;
  @Input() failed: boolean = false;
  constructor(changeDetectorRef: ChangeDetectorRef) {
    super(changeDetectorRef);
  }
}
