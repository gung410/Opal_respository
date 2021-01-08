import {
  ChangeDetectorRef,
  Component,
  OnInit,
  ViewEncapsulation
} from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from 'app-auth/auth.service';
import { AppConstant } from 'app/shared/app.constant';
import { BaseScreenComponent } from 'app/shared/components/component.abstract';

import { ApiDetailComponent } from './api-detail/api-detail.component';

@Component({
  selector: 'monitor',
  templateUrl: './monitor.component.html',
  styleUrls: ['./monitor.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class MonitorComponent extends BaseScreenComponent implements OnInit {
  apiOrigins: string[] = Object.values(AppConstant.api);
  constructor(
    private ngbModal: NgbModal,
    public authService: AuthService,
    changeDetectorRef: ChangeDetectorRef
  ) {
    super(changeDetectorRef, authService);
  }

  ngOnInit(): void {
    super.ngOnInit();
  }

  onOriginClicked(origin: any): void {
    const modalRef = this.ngbModal.open(ApiDetailComponent, { centered: true });
    (modalRef.componentInstance as ApiDetailComponent).originResource = origin;
  }
}
