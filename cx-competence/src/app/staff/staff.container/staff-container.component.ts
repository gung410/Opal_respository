import { Component, ChangeDetectorRef, OnInit } from '@angular/core';
import { AuthService } from 'app-auth/auth.service';
import { BaseScreenComponent } from 'app/shared/components/component.abstract';

@Component({
  selector: 'staff-container',
  templateUrl: './staff-container.component.html',
  styleUrls: ['./staff-container.component.scss'],
})
export class StaffContainerComponent extends BaseScreenComponent
  implements OnInit {
  public staffList: any[] = [];
  constructor(changeDetectorRef: ChangeDetectorRef, authService: AuthService) {
    super(changeDetectorRef, authService);
  }

  ngOnInit() {
    this.authService.userData().subscribe((user) => {
      if (user) {
        this.currentUser = user;
      }
    });
  }
}
