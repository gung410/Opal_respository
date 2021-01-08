import { CxNavbarComponent } from './components/cx-navbar/cx-navbar.component';
import { CxNavbarItemComponent } from './components/cx-navbar/cx-navbar-item/cx-navbar-item.component';
import { CxMenuItemComponent } from './components/cx-sidebar-dropdown-menu/cx-menu-item/cx-menu-item.component';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CxCommonComponent } from './cx-angular-common.component';
import { CxButtonComponent } from './components/cx-button/cx-button.component';
import { CxTreeComponent } from './components/cx-tree/cx-tree.component';
import { CxTreeNodeComponent } from './components/cx-tree/cx-tree-node/cx-tree-node.component';
import { CxNodeComponent } from './components/cx-node/cx-node.component';
import { CxAppendPopoverDirective } from './directives/append-popover.directive';
import { CxButtonDirective } from './directives/button.directive';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import { MatTabsModule } from '@angular/material/tabs';
import { NgbModule, NgbDatepickerModule } from '@ng-bootstrap/ng-bootstrap';
import { CxInputComponent } from './components/cx-input/cx-input.component';
import { CxCollapsibleInputComponent } from './components/cx-collapsible-input/cx-collapsible-input.component';
import { CxTreeDialogComponent } from './components/cx-tree/cx-tree-dialog/cx-tree-dialog.component';
import { CxTreeDropdownComponent } from './components/cx-tree/cx-tree-dropdown/cx-tree-dropdown.component';
import { CxBreadcrumbComponent } from './components/cx-breadcrumb/cx-breadcrumb.component';
import { CxCheckboxComponent } from './components/cx-checkbox/cx-checkbox.component';
import { CxExpandableListComponent } from './components/cx-expandable-list/cx-expandable-list.component';
import { CxDialogTemplateComponent } from './components/cx-dialog-template/cx-dialog-template.component';
import { CxCustomizedCheckboxComponent } from './components/cx-customized-checkbox/cx-customized-checkbox.component';
import { CxHeaderComponent } from './components/cx-header/cx-header.component';
import { CxSidebarDropdownMenuComponent } from './components/cx-sidebar-dropdown-menu/cx-sidebar-dropdown-menu.component';
import { CxSurveyjsComponent } from './components/cx-surveyjs/cx-surveyjs.component';
import { CxFormModal } from './modals/cx-form.modal';
import { CxDropdownMenuComponent } from './components/cx-sidebar-dropdown-menu/cx-dropdown-menu/cx-dropdown-menu.component';
import {
    CxRemoveItemsConfirmDialogComponent, CxItemCustomActionComponent, CxTableContainersComponent
} from './components/cx-table/cx-table.public';
import { CxTableComponent } from './components/cx-table/cx-table/cx-table.component';
import { CxPagingComponent } from './components/cx-paging/cx-paging.component';
import { CxClickOutsideDirective } from './components/cx-navbar/directives/events/cx-click-outside.directive';
import { CxSurveyjsModalTemplateComponent } from './components/cx-surveyjs/cx-surveyjs-modal-template/cx-surveyjs-modal-template.component';
import { CxDatePickerComponent } from './components/cx-date-picker/cx-date-picker.component';
import { CxNarrowTreeComponent } from './components/cx-tree/cx-narrow-tree/cx-narrow-tree.component';
import { CxNarrowTreeNodeComponent } from './components/cx-tree/cx-narrow-tree/cx-narrow-tree-node/cx-narrow-tree-node.component';
import { CxConfirmationDialogComponent } from './components/cx-confirmation-dialog/cx-confirmation-dialog.component';
import { CxTagGroupComponent } from './components/cx-tag-group/cx-tag-group.component';
import { CxBreadcrumbSimpleComponent } from './components/cx-breadcrumb-simple/cx-breadcrumb-simple.component';
import { MomentModule } from 'angular2-moment';
import { CxExtensiveTreeComponent } from './components/cx-extensive-tree/cx-extensive-tree.component';
import { CxExtensiveTreeDialogComponent } from './components/cx-extensive-tree/cx-extensive-tree-dialog/cx-extensive-tree-dialog.component';
import { CxTagGroupSimpleComponent } from './components/cx-tag-group-simple/cx-tag-group-simple.component';
import { RouterModule } from '@angular/router';
import { CxMandatoryTextComponent } from './components/cx-mandatory-text/cx-mandatory-text.component';
import {CxActionToolbarComponent} from './components/cx-action-toolbar/cx-action-toolbar.component';
import { DefaultImageDirective } from './directives/default-image.directive';
import {CxUniversalToolbarComponent} from './components/cx-universal-toolbar/cx-universal-toolbar.component';
import { FooterComponent } from './components/cx-footer/cx-footer.component';
import { CxFloatingToolbarDirective } from './directives/floating-toolbar';
import { CxSlidebarComponent } from './components/cx-slidebar/cx-slidebar.component';
@NgModule({
    declarations: [
        CxCommonComponent,
        CxButtonComponent,
        CxTreeComponent,
        CxTreeNodeComponent,
        CxNodeComponent,
        CxAppendPopoverDirective,
        CxButtonDirective,
        CxTreeDialogComponent,
        CxInputComponent,
        CxCollapsibleInputComponent,
        CxTreeDropdownComponent,
        CxBreadcrumbComponent,
        CxBreadcrumbSimpleComponent,
        CxTableContainersComponent,
        CxCheckboxComponent,
        CxExpandableListComponent,
        CxItemCustomActionComponent,
        CxDialogTemplateComponent,
        CxConfirmationDialogComponent,
        CxRemoveItemsConfirmDialogComponent,
        CxCustomizedCheckboxComponent,
        CxHeaderComponent,
        CxSurveyjsComponent,
        CxMenuItemComponent,
        CxSidebarDropdownMenuComponent,
        CxTableComponent,
        CxPagingComponent,
        CxDropdownMenuComponent,
        CxSlidebarComponent,
        CxNavbarItemComponent,
        CxNavbarComponent,
        CxClickOutsideDirective,
        CxSurveyjsModalTemplateComponent,
        CxDatePickerComponent,
        CxNarrowTreeComponent,
        CxNarrowTreeNodeComponent,
        CxTagGroupComponent,
        CxExtensiveTreeComponent,
        CxExtensiveTreeDialogComponent,
        CxTagGroupSimpleComponent,
        CxMandatoryTextComponent,
        CxActionToolbarComponent,
        DefaultImageDirective,
        CxFloatingToolbarDirective,
        CxUniversalToolbarComponent,
        FooterComponent,
    ],
    imports: [
        CommonModule,
        FormsModule,
        MatIconModule,
        RouterModule,
        MatTabsModule,
        NgbModule,
        NgbDatepickerModule,
        MomentModule
    ],
    exports: [
        CommonModule,
        CxCommonComponent,
        CxButtonComponent,
        CxTreeComponent,
        CxTreeNodeComponent,
        CxNarrowTreeComponent,
        CxNarrowTreeNodeComponent,
        CxNodeComponent,
        CxAppendPopoverDirective,
        CxButtonDirective,
        CxInputComponent,
        CxCollapsibleInputComponent,
        CxTreeDropdownComponent,
        CxBreadcrumbComponent,
        CxBreadcrumbSimpleComponent,
        CxTableContainersComponent,
        CxCheckboxComponent,
        CxExpandableListComponent,
        CxItemCustomActionComponent,
        CxDialogTemplateComponent,
        CxCustomizedCheckboxComponent,
        CxHeaderComponent,
        CxMenuItemComponent,
        CxSidebarDropdownMenuComponent,
        CxTableComponent,
        CxPagingComponent,
        CxDropdownMenuComponent,
        CxSlidebarComponent,
        CxNavbarItemComponent,
        CxSurveyjsComponent,
        CxNavbarComponent,
        CxDatePickerComponent,
        CxTagGroupComponent,
        CxExtensiveTreeComponent,
        CxTagGroupSimpleComponent,
        CxMandatoryTextComponent,
        CxActionToolbarComponent,
        DefaultImageDirective,
        CxFloatingToolbarDirective,
        CxUniversalToolbarComponent,
        FooterComponent
    ],
    entryComponents: [CxTreeDialogComponent,
        CxRemoveItemsConfirmDialogComponent,
        CxSurveyjsModalTemplateComponent,
        CxConfirmationDialogComponent,
        CxExtensiveTreeDialogComponent
    ],
    providers: [CxFormModal],
    schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class CxCommonModule { }
