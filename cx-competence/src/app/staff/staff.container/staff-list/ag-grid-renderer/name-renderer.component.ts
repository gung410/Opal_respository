import { ICellRendererAngularComp } from '@ag-grid-community/angular';
import { ChangeDetectorRef, Component } from '@angular/core';
import { AuthService } from 'app-auth/auth.service';
import { User } from 'app-models/auth.model';
import { BaseScreenComponent } from 'app/shared/components/component.abstract';
import { StaffListService } from '../../staff-list.service';

@Component({
  selector: 'name-cell',
  template: `<div *ngIf="email" class="staff-table__items--avatar">
      <a routerLink="{{ detailLink }}" (click)="resetSearchValue($event)"
        ><img class="staff-table__items--avatar-image" [src]="getAvatar()"
      /></a>
    </div>
    <div class="staff-table__items-info">
      <div
        class="staff-table__items-info-data staff-table__items-info-data--main"
        title="{{ fullName }}"
      >
        <a routerLink="{{ detailLink }}" (click)="resetSearchValue($event)">{{
          fullName
        }}</a>
      </div>
      <div
        class="staff-table__items-info-data staff-table__items-info-data--secondary"
        title="{{ email }}"
      >
        {{ email }}
      </div>
    </div>`,
  styleUrls: ['../staff-list.component.scss'],
})
export class SLNameRendererComponent
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
    private staffListService: StaffListService,
    protected authService: AuthService
  ) {
    super(changeDetectorRef, authService);
  }

  agInit(params: any): void {
    this.processParam(params);
  }

  refresh(params: any): boolean {
    this.processParam(params);

    return true;
  }

  getAvatar(): string {
    return this.getUserImage(new User({ avatarUrl: this.avatarUrl }));
  }

  resetSearchValue(event: any): void {
    this.staffListService.resetSearchValueSubject.next(true);
  }

  private processParam(params: any): void {
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
