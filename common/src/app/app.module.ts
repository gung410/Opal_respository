import { CxUniversalToolbarDocComponent } from './components/cx-universal-toolbar-doc/cx-universal-toolbar-doc.component';
import { CxNavbarDocComponent } from './components/cx-navbar-doc/cx-navbar-doc.component';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatIconModule } from '@angular/material/icon';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MatButtonModule } from '@angular/material/button';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatMenuModule } from '@angular/material/menu';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatTabsModule } from '@angular/material/tabs';

import { AppComponent } from './app.component';
import { NavLeftComponent } from './nav-left/nav-left.component';
import { DocsViewerComponent } from './docs-viewer/docs-viewer.component';
import { NavHeaderComponent } from './nav-header/nav-header.component';
import { NavFooterComponent } from './nav-footer/nav-footer.component';
import { AppRoutingModule } from './app-routing.module';
import { ComponentsModule } from './components/components.module';
import { DocsViewComponent } from './docs-view/docs-view.component';
import { CxCommonModule, CxSurveyjsService } from 'projects/cx-angular-common/src';
import { CxTreeDocComponent } from './components/cx-tree/cx-tree-doc.component';
import { CxButtonDocComponent } from './components/button/cx-button-doc.component';
import { CxTreeDropdownDocComponent } from './components/cx-tree-dropdown-doc/cx-tree-dropdown-doc.component';
import { CxExpandableListDocComponent } from './components/cx-expandable-list-doc/cx-expandable-list-doc.component';
import { CxCheckboxDocComponent } from './components/cx-checkbox-doc/cx-checkbox-doc.component';
import { MomentModule } from 'angular2-moment';
import { CxBreadcrumbDocComponent } from './components/cx-breadcrumb-doc/cx-breadcrumb-doc.component';
import { CxSidebarDropdownMenuDocComponent } from './components/cx-sidebar-dropdown-menu/cx-sidebar-dropdown-menu.component';
import { CxSurveyDocComponent } from './components/cx-survey-doc/cx-survey-doc.component';
import { HeaderComponent } from './components/header/header.component';
import { CxTableContainersDocComponent } from './components/cx-table-containers-doc/cx-table-containers-doc.component';
import { CxTableDocComponent } from './components/cx-table-doc/cx-table-doc.component';
import { CxDatetimeDocComponent } from './components/cx-datetime-doc/cx-datetime-doc.component';
import { CxNarrowTreeDocComponent } from './components/cx-narrow-tree-doc/cx-narrow-tree-doc.component';
import { CxNodeDocComponent } from './components/cx-node-doc/cx-node-doc.component';
import { CxTagGroupDocComponent } from './components/cx-tag-group-doc/cx-tag-group-doc.component';
import { CxLoaderModule } from 'projects/cx-angular-common/src/lib/components/cx-loader/cx-loader.module';
import { CxLoaderUI } from 'projects/cx-angular-common/src/lib/components/cx-loader/cx-loader.model';
import { FormsModule } from '@angular/forms';
import { CxSlidebarDocComponent } from './components/cx-slidebar-doc/cx-slidebar-doc.component';

@NgModule({
  declarations: [
    CxSidebarDropdownMenuDocComponent,
    AppComponent,
    NavLeftComponent,
    NavLeftComponent,
    DocsViewerComponent,
    DocsViewComponent,
    NavHeaderComponent,
    NavFooterComponent,
    CxButtonDocComponent,
    CxTableContainersDocComponent,
    CxTableDocComponent,
    CxTreeDocComponent,
    CxNarrowTreeDocComponent,
    CxNodeDocComponent,
    CxTreeDropdownDocComponent,
    CxExpandableListDocComponent,
    CxCheckboxDocComponent,
    CxBreadcrumbDocComponent,
    HeaderComponent,
    CxSurveyDocComponent,
    CxNavbarDocComponent,
    CxDatetimeDocComponent,
    CxTagGroupDocComponent,
    CxUniversalToolbarDocComponent,
    CxSlidebarDocComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    CxCommonModule,
    MatButtonModule,
    MatMenuModule,
    MatSidenavModule,
    MatIconModule,
    NgbModule,
    MatTabsModule,
    MatExpansionModule,
    ComponentsModule,
    MomentModule,
    CxLoaderModule.forRoot({
      loaderUi: new CxLoaderUI({
        circleBackgroundColor: 'green',
        overlayTopPositionInPx: 0
      })
    }),
    FormsModule
  ],
  providers: [CxSurveyjsService],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppModule { }
