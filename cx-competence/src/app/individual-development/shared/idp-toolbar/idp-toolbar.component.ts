import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';
import { CxBreadCrumbItem } from '@conexus/cx-angular-common/lib/components/cx-breadcrumb-simple/model/breadcrumb.model';

@Component({
  selector: 'idp-toolbar',
  templateUrl: './idp-toolbar.component.html',
  styleUrls: ['./idp-toolbar.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class IdpToolbarComponent implements OnInit {
  @Input() user: any;
  breadcrumb: CxBreadCrumbItem[] = [];

  constructor(private router: Router) {}

  ngOnInit(): void {
    this.breadcrumb = [
      { name: this.user.department.name, identity: null },
      { name: this.user.firstName, identity: null },
    ];
  }

  onClickBreadcrumbItem(breadcrumbItem: CxBreadCrumbItem): void {
    if (breadcrumbItem && breadcrumbItem.name === this.user.department.name) {
      this.router.navigateByUrl('/');
    }
  }
}
