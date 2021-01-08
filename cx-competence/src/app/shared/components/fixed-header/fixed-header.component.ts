import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
  ViewEncapsulation,
} from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { Router } from '@angular/router';
import {
  CxHeaderComponent,
  NotificationList,
} from '@conexus/cx-angular-common';
import { environment } from 'app-environments/environment';
import { User } from 'app-models/auth.model';
import { BreadcrumbSettingService } from 'app-services/bread-crumb-setting.service';
import { StaffListService } from 'app/staff/staff.container/staff-list.service';
import { BasePresentationComponent } from '../component.abstract';

@Component({
  selector: 'fixed-header',
  templateUrl: './fixed-header.component.html',
  styleUrls: ['./fixed-header.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class FixedHeaderComponent
  extends BasePresentationComponent
  implements OnInit, AfterViewInit {
  @Input() userData: User;
  @Input() headerAction: any;
  @Input() multipleLanguages: any;
  @Input() invalidUser: boolean;
  @Input() notifications: NotificationList;
  @Input() isShowNotificationBell: boolean = false;
  @Input()
  notificationBellUrl: SafeResourceUrl = this.domSanitizer.bypassSecurityTrustResourceUrl(
    environment.notification.bellUrl
  );
  @Output() logout: EventEmitter<void> = new EventEmitter();
  @Output() changePassword: EventEmitter<void> = new EventEmitter();
  @Output() editProfile: EventEmitter<void> = new EventEmitter();
  @Output() searchOnSearchBox: EventEmitter<string> = new EventEmitter<
    string
  >();
  @Output() clickSupport: EventEmitter<void> = new EventEmitter();
  @Output() clickSettings: EventEmitter<void> = new EventEmitter();
  @ViewChild('appHeader') appHeaderRef: ElementRef;
  @ViewChild('cxHeader') cxHeader: CxHeaderComponent;

  currentSite: string;
  isAtStaffList: boolean;
  searchValue: string;

  constructor(
    private changeDectectorRef: ChangeDetectorRef,
    private router: Router,
    private breadcrumbSettingService: BreadcrumbSettingService,
    private domSanitizer: DomSanitizer,
    private staffListService: StaffListService
  ) {
    super(changeDectectorRef);

    // Angular-common library are checking active menu item by currentSite.startWith(menuUrl),
    // passing pathname also incase SPA deploy as virtualpath
    // example: https://www.uat.opal2.conexus.net/pdplanner/
    this.currentSite = `${location.origin}${location.pathname}` || undefined;
  }

  ngOnInit(): void {
    const currentUrl = this.router.url;
    this.isAtStaffList = currentUrl.includes('/employee');
    this.breadcrumbSettingService.changeBreadcrumb({ route: currentUrl });
  }

  ngAfterViewInit(): void {}

  public onLogout(): void {
    this.logout.emit();
  }

  public onChangePassword(): void {
    this.changePassword.emit();
  }

  public onEditProfile(): void {
    this.editProfile.emit();
  }

  public onSupport(): void {
    this.clickSupport.emit();
  }

  public onClickedSettings(): void {
    this.clickSettings.emit();
  }

  public onSearch(searchTerm: string): void {
    this.searchOnSearchBox.emit(searchTerm);
  }

  public onClickedHamburger(): void {
    // TODO: handle event clicked hamburger button
  }

  public onChangeMenu(isAtStaffList: boolean): void {
    if (isAtStaffList) {
      this.staffListService.searchingBehaviorSubject.next('');
      this.cxHeader.searchValue = '';
      this.cxHeader.changeDetectorRef.detectChanges();
    }
    this.isAtStaffList = isAtStaffList;
  }
}
