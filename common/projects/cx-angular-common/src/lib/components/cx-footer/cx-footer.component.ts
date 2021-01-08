import { Component, OnInit, Input } from '@angular/core';
import { CxFooterData } from './cx-footer.model';

@Component({
  selector: 'cx-footer',
  templateUrl: './cx-footer.component.html',
  styleUrls: ['./cx-footer.component.scss']
})
export class FooterComponent implements OnInit {
  @Input() footerData: CxFooterData;

  constructor() { }

  ngOnInit(): void {}
}
