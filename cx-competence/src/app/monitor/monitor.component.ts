import {
  Component,
  ViewEncapsulation,
  OnInit,
  ChangeDetectorRef,
} from '@angular/core';
import { AppConstant } from 'app/shared/app.constant';
import { BaseScreenComponent } from 'app/shared/components/component.abstract';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ApiDetailComponent } from './api-detail/api-detail.component';
import { AuthService } from 'app-auth/auth.service';

@Component({
  selector: 'monitor',
  templateUrl: './monitor.component.html',
  styleUrls: ['./monitor.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class MonitorComponent extends BaseScreenComponent implements OnInit {
  public apiOrigins: string[] = Object.values(AppConstant.api);
  constructor(
    changeDetectorRef: ChangeDetectorRef,
    private ngbModal: NgbModal,
    public authService: AuthService
  ) {
    super(changeDetectorRef, authService);
  }

  ngOnInit(): void {
    super.ngOnInit();
  }

  onOriginClicked(origin) {
    const modalRef = this.ngbModal.open(ApiDetailComponent, { centered: true });
    (modalRef.componentInstance as ApiDetailComponent).originResource = origin;
  }
}
