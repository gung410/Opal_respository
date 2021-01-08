import {
  Component,
  OnInit,
  ViewEncapsulation,
  ChangeDetectorRef,
} from '@angular/core';
import { AuthService } from 'app-auth/auth.service';
import { UserService } from 'app-services/user.service';
import { BaseScreenComponent } from 'app/shared/components/component.abstract';
import { IDPMode } from 'app/individual-development/idp.constant';
import { Staff } from 'app/staff/staff.container/staff-list/models/staff.model';

@Component({
  selector: 'my-pd-journey',
  templateUrl: './my-pd-journey.component.html',
  styleUrls: ['./my-pd-journey.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class MyPdJourneyComponent extends BaseScreenComponent
  implements OnInit {
  mode: string = IDPMode.Learner;
  user: Staff;
  constructor(
    changeDetectorRef: ChangeDetectorRef,
    authService: AuthService,
    private userService: UserService
  ) {
    super(changeDetectorRef, authService);
  }

  async ngOnInit(): Promise<void> {
    this.user = await this.userService.getStaffProfile(this.currentUser.id);
  }
}
