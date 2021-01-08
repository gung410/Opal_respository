import {
  Component,
  EventEmitter,
  HostListener,
  Input,
  OnInit,
  Output,
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CxNavbarItemModel } from '@conexus/cx-angular-common';
import { BreadcrumbSettingService } from 'app-services/bread-crumb-setting.service';
import { StaffListService } from 'app/staff/staff.container/staff-list.service';

@Component({
  selector: 'sticky-navbar',
  templateUrl: './sticky-navbar.component.html',
  styleUrls: ['./sticky-navbar.component.scss'],
})
export class StickyNavbarComponent implements OnInit {
  @Input() roles: string[];
  @Input() menuData: Array<CxNavbarItemModel>;
  @Output() clickItem: EventEmitter<any> = new EventEmitter();
  @Output() changeMenu: EventEmitter<boolean> = new EventEmitter();
  listMenu: Array<CxNavbarItemModel> = [];
  innerWidth = 0;
  maxWidth = 768;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private breadcrumbSettingService: BreadcrumbSettingService,
    private staffListService: StaffListService
  ) {}

  ngOnInit() {
    this.staffListService.resetSearchValueSubject.subscribe((res) => {
      if (res) {
        this.changeMenu.emit(true);
      }
    });
  }

  @HostListener('window:resize', ['$event'])
  onResize() {}

  ngAfterViewInit() {}

  public clickMenuItem(menuItem: any) {
    if (menuItem && menuItem.children && menuItem.children.length <= 0) {
      this.breadcrumbSettingService.changeBreadcrumb({ route: menuItem.route });
    }
    this.clickItem.emit(menuItem);
    this.changeMenu.emit(menuItem.route.includes('/employee'));
  }
}
