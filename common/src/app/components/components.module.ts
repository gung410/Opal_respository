import { ComponentsRoutingModule } from './components-routing.module';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { ComponentsContainerComponent } from './components-page/components-container.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
@NgModule({
    imports: [
        CommonModule,
        ComponentsRoutingModule
    ],
    declarations: [
        ComponentsContainerComponent,
    ],
    providers: [
    ],
    exports: [
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA]
})

export class ComponentsModule { }
