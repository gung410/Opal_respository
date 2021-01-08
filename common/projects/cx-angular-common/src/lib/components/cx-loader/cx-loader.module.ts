import { NgModule, ModuleWithProviders, InjectionToken } from "@angular/core";
import { CommonModule } from '@angular/common';
import { CxLoaderModuleConfig } from './cx-loader.model';
import { CxLoaderComponent } from './cx-loader.component';
import { CxGlobalLoaderService } from './cx-global-loader.service';

@NgModule({
    imports: [CommonModule],
    declarations: [CxLoaderComponent],
    providers: [CxGlobalLoaderService],
    entryComponents: [CxLoaderComponent],
    exports: [CxLoaderComponent]
})
export class CxLoaderModule {
    static forRoot(config: CxLoaderModuleConfig): ModuleWithProviders<CxLoaderModule> {
        return {
            ngModule: CxLoaderModule,
            providers: [{
                provide: CxLoaderModuleConfig,
                useValue: config
            }
        ]};
    }
}
