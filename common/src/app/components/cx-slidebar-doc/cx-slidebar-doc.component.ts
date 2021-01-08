import { Component, OnInit, ViewChild } from '@angular/core';
import { CxSlidebarComponent } from 'projects/cx-angular-common/src';

@Component({
  selector: 'cx-slidebar-doc',
  templateUrl: './cx-slidebar-doc.component.html',
  styleUrls: ['./cx-slidebar-doc.component.scss'],
})
export class CxSlidebarDocComponent implements OnInit {
  @ViewChild(CxSlidebarComponent)
  private slidebar: CxSlidebarComponent;

  public slidebarPosition: 'left' | 'right' = 'right';
  public height: string = '70vh';

  constructor() {}

  ngOnInit() {}

  public openSlidebar(): void {
    this.slidebar.openSlidebar();
  }
}
