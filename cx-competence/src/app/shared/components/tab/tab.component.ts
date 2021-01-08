import {
  Component,
  OnInit,
  Input,
  ContentChild,
  TemplateRef,
} from '@angular/core';

@Component({
  selector: 'app-tab',
  templateUrl: './tab.component.html',
  styleUrls: ['./tab.component.scss'],
})
export class TabComponent implements OnInit {
  @Input() tabs: any[];
  @Input() containerClass: string;

  constructor() {}

  ngOnInit() {}
}