import { ICellRendererAngularComp } from '@ag-grid-community/angular';
import { ChangeDetectorRef, Component } from '@angular/core';
import { AuthService } from 'app-auth/auth.service';
import { User } from 'app-models/auth.model';
import { BaseScreenComponent } from 'app/shared/components/component.abstract';

@Component({
  selector: 'name-renderer',
  templateUrl: './name-renderer.component.html',
  styleUrls: ['./name-renderer.component.scss'],
})
export class NameRendererComponent
  extends BaseScreenComponent
  implements ICellRendererAngularComp {
  params: any;
  email: any;
  fullName: any;
  departmentName: any;
  id: string;
  avatarUrl: string;
  detailLink: string;
  constructor(
    changeDetectorRef: ChangeDetectorRef,
    protected authService: AuthService
  ) {
    super(changeDetectorRef, authService);
  }

  // called on init
  agInit(params: any): void {
    this.processParam(params);
  }

  // called when the cell is refreshed
  refresh(params: any): boolean {
    this.processParam(params);

    return true;
  }

  getAvatar(): string {
    return this.getUserImage(new User({ avatarUrl: this.avatarUrl }));
  }

  processParam(params: any): void {
    if (!params || !params.value) {
      return;
    }
    this.params = params;
    this.fullName = this.params.value.fullName;
    this.email = this.params.value.email;
    this.departmentName = this.params.value.departmentName;
    this.id = this.params.value.id;
    this.avatarUrl = params.value.avatarUrl;
    this.detailLink = `/employee/detail/${this.id}`;
  }
}
