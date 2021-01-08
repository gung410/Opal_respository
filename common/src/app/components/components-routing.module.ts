import { CxUniversalToolbarDocComponent } from './cx-universal-toolbar-doc/cx-universal-toolbar-doc.component';
import { CxNavbarDocComponent } from './cx-navbar-doc/cx-navbar-doc.component';
import { CxSidebarDropdownMenuDocComponent } from './cx-sidebar-dropdown-menu/cx-sidebar-dropdown-menu.component';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CxButtonDocComponent } from './button/cx-button-doc.component';
import { ComponentsContainerComponent } from './components-page/components-container.component';
import { CxTreeDocComponent } from './cx-tree/cx-tree-doc.component';
import { CxTreeDropdownDocComponent } from './cx-tree-dropdown-doc/cx-tree-dropdown-doc.component';
import { CxExpandableListDocComponent } from './cx-expandable-list-doc/cx-expandable-list-doc.component';
import { CxCheckboxDocComponent } from './cx-checkbox-doc/cx-checkbox-doc.component';
import { CxBreadcrumbDocComponent } from './cx-breadcrumb-doc/cx-breadcrumb-doc.component';
import { CxTableContainersDocComponent } from './cx-table-containers-doc/cx-table-containers-doc.component';
import { CxSurveyDocComponent } from './cx-survey-doc/cx-survey-doc.component';
import { CxTableDocComponent } from './cx-table-doc/cx-table-doc.component';
import { HeaderComponent } from './header/header.component';
import { CxDatetimeDocComponent } from './cx-datetime-doc/cx-datetime-doc.component';
import { CxNarrowTreeDocComponent } from './cx-narrow-tree-doc/cx-narrow-tree-doc.component';
import { CxTagGroupDocComponent } from './cx-tag-group-doc/cx-tag-group-doc.component';
import { CxNodeDocComponent } from './cx-node-doc/cx-node-doc.component';
import { CxSlidebarComponent } from 'projects/cx-angular-common/src/lib/components/cx-slidebar/cx-slidebar.component';
import { CxSlidebarDocComponent } from './cx-slidebar-doc/cx-slidebar-doc.component';

const routes: Routes = [
    {
        path: 'components',
        component: ComponentsContainerComponent,
        children: [
            { path: '', component: CxButtonDocComponent },
            { path: 'button', component: CxButtonDocComponent },
            { path: 'items-table', component: CxTableContainersDocComponent },
            { path: 'datetime-picker', component: CxDatetimeDocComponent },
            { path: 'table', component: CxTableDocComponent },
            { path: 'tree', component: CxTreeDocComponent },
            { path: 'narrow-tree', component: CxNarrowTreeDocComponent},
            { path: 'node', component: CxNodeDocComponent},
            { path: 'expandable-list', component: CxExpandableListDocComponent },
            { path: 'check-box', component: CxCheckboxDocComponent },
            { path: 'tree-dropdown', component: CxTreeDropdownDocComponent },
            { path: 'bread-crumb', component: CxBreadcrumbDocComponent },
            { path: 'sidebar-menu', component: CxSidebarDropdownMenuDocComponent },
            { path: 'surveyjs', component: CxSurveyDocComponent },
            { path: 'header', component: HeaderComponent},
            { path: 'navbar', component: CxNavbarDocComponent},
            { path: 'tag-group', component: CxTagGroupDocComponent},
            { path: 'universal-toolbar', component: CxUniversalToolbarDocComponent},
            { path: 'slidebar', component: CxSlidebarDocComponent},
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
    schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class ComponentsRoutingModule { }
