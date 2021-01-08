import { CxNavbarItemModel } from './../../../../projects/cx-angular-common/src/lib/components/cx-navbar/models/cx-navbar-item-model';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-cx-navbar-doc',
  templateUrl: './cx-navbar-doc.component.html',
  styleUrls: ['./cx-navbar-doc.component.scss']
})
export class CxNavbarDocComponent implements OnInit {

  listMenu: Array<CxNavbarItemModel>;

  constructor() { }

  ngOnInit() {
    this.listMenu = [
      new CxNavbarItemModel({
        content: 'Training Calendar',
        icon: 'icon-calendar',
        iconActive: 'icon-calendar-selected',
        isCollapsed: true,
        children: [
          new CxNavbarItemModel({
            content: 'My Calendar',
            route: '/my-calendar'
          }),
          new CxNavbarItemModel({
            content: 'Team Calendar',
            route: '/team-calendar'
          })
        ]
      }),
      new CxNavbarItemModel({
        content: 'Staff Development',
        icon: 'icon-growth',
        iconActive: 'icon-growth-selected',
        isCollapsed: true,
        children: [
          new CxNavbarItemModel({
            content: 'My Calendar',
            route: '/my-calendar'
          }),
          new CxNavbarItemModel({
            content: 'MPJ',
            route: '/team-calendar'
          })
        ]
      }),
      new CxNavbarItemModel({
        content: 'Organisational Development',
        icon: 'icon-staff',
        iconActive: 'icon-staff-selected',
      }),
      new CxNavbarItemModel({
        content: 'Professional Growth',
        icon: 'icon-pie-chart',
        iconActive: 'icon-pie-chart-selected',
        children: [
          new CxNavbarItemModel({
            content: 'My Calendar',
            route: '/my-calendar'
          }),
          new CxNavbarItemModel({
            content: 'Team Calendar',
            route: '/team-calendar'
          })
        ]
      })
    ];
  }
}
