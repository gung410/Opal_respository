import { Component, OnInit } from '@angular/core';
import { IdentityClaims } from 'src/app/data/identityClaims-data';
import {
  CurrentUser,
  MultipleLanguages,
  HeaderLogo,
  TopHeader,
  AppsSwitcherItem,
  NotificationItem,
  CxNavbarItemModel } from 'projects/cx-angular-common/src';
import { NotificationList } from 'projects/cx-angular-common/src/lib/components/cx-header/models/notification-list.model';
import { environment } from 'src/environments/environment';
import { SafeResourceUrl, DomSanitizer } from '@angular/platform-browser';

@Component({
    selector: 'header',
    templateUrl: './header.component.html',
    styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {
    public currentUser = new CurrentUser(IdentityClaims);
    public notificationBellUrl: SafeResourceUrl = this.domSanitizer.bypassSecurityTrustResourceUrl(environment.notification.bellUrl);
    public topHeader: TopHeader = {
        linkHref: 'http://www.gov.sg',
        linkAlt: 'Singapore Government',
        iconClass: 'icon-lion',
        text: 'A Singapore Government Agency Website'
    };
    public logo: HeaderLogo = {
      imageUrl: 'assets/images/opal-logo-slogan.png',
      imageAlt: 'PD Planner',
      routeLink: '/components/button',
      text: 'System Admin'
    };
    public placeholderText = 'Search';
    public multipleLanguages: MultipleLanguages = {
        notifications: 'Notifications  - Spanish',
        search: 'Search  - Spanish'
    };
    public applications: AppsSwitcherItem[] = [
        {
            label: 'PD Planner 1',
            mainUrl: 'https://development-competence-opal-spa.csc.cxs.cloud',
            logoUrl: 'assets/images/moe-logo.png'
        },
        {
            label: 'PROFESSIONAL DEVELOPMENT (PD) ADMIN',
            mainUrl: 'google.com'
        },
        {
            label: 'System Admin',
            mainUrl: 'google.com'
        },
        {
            label: 'Content Creator',
            mainUrl: 'google.com'
        },
        {
            label: 'Learner',
            mainUrl: 'google.com'
        }
    ];

    public notificationList: NotificationList = {
        totalNewCount : 3,
        totalCount : 4,
        totalUnreadCount : 4,
        items: [
            {
                id: 4,
                messageId: '4',
                avatarUrl: 'assets/images/avatar-1.png',
                senderName: 'Shane Nguyen',
                message: 'invites you to his store.',
                sentDate: new Date(),
                dateReadUtc: new Date()
            },
            {
                id: 2,
                messageId: '2',
                avatarUrl: 'assets/images/avatar-1.png',
                senderName: 'Kai Tran',
                message: 'reply to your comment in this post',
                sentDate: new Date(),
                dateReadUtc: new Date()
            },
            {
                id: 3,
                messageId: '3',
                avatarUrl: 'assets/images/avatar-1.png',
                senderName: 'TheO Nguyen',
                hyperLink: '/components/items-table',
                message: 'like your photo',
                sentDate: new Date(),
                dateReadUtc: new Date()
            },
            {
                id: 1,
                messageId: '1',
                avatarUrl: 'assets/images/avatar-1.png',
                senderName: 'Shane Nguyen',
                message: 'invites you to his store.',
                sentDate: new Date(),
                dateReadUtc: new Date()
            }
        ]
    };

    public currentSearchText: string;
    public notificationInterval: NodeJS.Timer;
    public isShowNotificationBell = environment.notification.enableShowBellIcon;

    public propApiData = [
        {
            name: 'currentUser',
            desc: '[CurrentUser] Current user object'
        },
        {
            name: 'placeholderText',
            desc: ''
        },
        {
            name: 'logo',
            desc: ''
        },
        {
            name: 'topHeader',
            desc: 'Configuration for Top Header'
        },
        {
            name: 'applications',
            desc: 'List of application objects'
        },
        {
            name: 'notifications',
            desc: 'List of notification objects'
        },
        {
            name: 'currentURL',
            desc: ''
        }
    ];
    public eventApiData = [
        {
            name: 'signOut',
            desc: ''
        },
        {
            name: 'editProfile',
            desc: ''
        },
        {
            name: 'clickSettings',
            desc: ''
        },
        {
            name: 'clickSupport',
            desc: ''
        },
        {
            name: 'openNotificationPopUp',
            desc: ''
        },
        {
            name: 'searchOnSearchBox',
            desc: ''
        }
    ];
    menus: CxNavbarItemModel[];
    constructor(private domSanitizer: DomSanitizer) { }

    ngOnInit() {
        setInterval(() => {
            this.notificationList.totalNewCount++;
            this.notificationList.items = [{
                id: Number.parseInt(this.notificationList.items[0].id.toString()) + 1,
                messageId: Number.parseInt(this.notificationList.items[0].id.toString()) + 1,
                avatarUrl: 'assets/images/avatar-1.png',
                senderName: 'Shane Nguyen ' + (Number.parseInt(this.notificationList.items[0].id.toString()) + 1),
                message: 'invites you to his store.',
                sentDate: new Date()
            }, ...this.notificationList.items];
        }, 10000);
    }

    public onLogout(): void {
        console.log('SignOut clicked');
    }

    public onEditProfile(): void {
        console.log('Profile clicked');
    }
    public onSearch(searchTerm: string): void {
        console.log(searchTerm);
    }

    public onClickSettings(): void {
        console.log('Settings clicked');
    }

    public onClickSupport(): void {
        console.log('Support clicked');
    }

    public onOpenNotificationPopUp(isOpen: boolean): void {
        if (!isOpen) { return; }
        this.notificationList.totalNewCount = 0;
    }

    public onClickedNotification(notification: NotificationItem): void {
      notification.dateReadUtc = new Date();
    }

    public onClickNotification(notification: NotificationItem): void {
        const { hyperLink } = notification;
        if (hyperLink && hyperLink.length > 0) {
            window.location.href = hyperLink;
        }
    }
}
