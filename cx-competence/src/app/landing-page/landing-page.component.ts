import {
  Component,
  ViewEncapsulation,
  OnInit,
  ChangeDetectorRef,
} from '@angular/core';
import { AuthService } from 'app-auth/auth.service';
import { User } from 'app-models/auth.model';
import { BaseScreenComponent } from 'app/shared/components/component.abstract';

@Component({
  selector: 'landing-page',
  templateUrl: 'landing-page.component.html',
  styleUrls: ['./landing-page.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class LandingPageComponent extends BaseScreenComponent
  implements OnInit {
  public currentUser: User;

  constructor(
    changeDetectorRef: ChangeDetectorRef,
    public authService: AuthService
  ) {
    super(changeDetectorRef, authService);
  }

  ngOnInit(): void {
    super.ngOnInit();
  }
}
