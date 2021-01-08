import { Component, OnInit } from '@angular/core';
import { IdentityClaims } from '../data/identityClaims-data';
import { CurrentUser, HeaderLogo } from 'projects/cx-angular-common/src/public_api';

@Component({
    selector: 'cx-docs-view',
    templateUrl: './docs-view.component.html',
    styleUrls: ['./docs-view.component.scss']
})
export class DocsViewComponent implements OnInit {
    public currentUser = new CurrentUser(IdentityClaims);
    public logo: HeaderLogo = {
        imageUrl: 'assets/images/logo-vip-24.png',
        imageAlt: 'PD Planner',
        routeLink: '/components/button'
    };
    title = 'PROFESSIONAL DEVELOPMENT (PD) ADMIN';

    constructor() { }

    ngOnInit() {
    }

    public onLogout(signal: any): void {
        console.log(signal);
    }

    public onChangePassword(signal: any): void {
        console.log(signal);
    }
    public onEditProfile(signal: any): void {
        console.log(signal);
    }
    public onSearch(signal: any): void {
        console.log(signal);
    }
}
