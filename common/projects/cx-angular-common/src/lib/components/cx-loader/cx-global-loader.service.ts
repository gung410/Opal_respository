import { Injectable, ComponentRef, ComponentFactoryResolver, ApplicationRef, Injector, EmbeddedViewRef } from "@angular/core";
import { CxLoaderComponent } from './cx-loader.component';
import { CxLoaderUI, CxLoaderModuleConfig } from './cx-loader.model';


@Injectable({
    providedIn: 'root'
})

export class CxGlobalLoaderService {
    private loaderComponentRef: ComponentRef<CxLoaderComponent>;
    constructor(private componentFactoryResolver: ComponentFactoryResolver,
                private applicationRef: ApplicationRef,
                private injector: Injector,
                private config: CxLoaderModuleConfig) {
        const componentFactory = this.componentFactoryResolver.resolveComponentFactory(CxLoaderComponent);
        this.loaderComponentRef = componentFactory.create(this.injector);
        this.applicationRef.attachView(this.loaderComponentRef.hostView);

        const domElem = (this.loaderComponentRef.hostView as EmbeddedViewRef<any>).rootNodes[0] as HTMLElement;
        document.body.appendChild(domElem);
        this.loaderComponentRef.instance.setUpConfig(this.config);
    }

    // TODO: need to provide overriden config later
    showLoader() {
        this.loaderComponentRef.instance.loading = true;
    }

    hideLoader() {
        this.loaderComponentRef.instance.loading = false;
    }
}
