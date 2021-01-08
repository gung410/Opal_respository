import {
  ApplicationRef,
  ComponentFactory,
  ComponentFactoryResolver,
  ComponentRef,
  EmbeddedViewRef,
  Injector,
  ViewContainerRef
} from '@angular/core';
import { BasePortalOutlet, ComponentPortal, TemplatePortal } from './portal';
/**
 * @license
 * Copyright Google LLC All Rights Reserved.
 *
 * Use of this source code is governed by an MIT-style license that can be
 * found in the LICENSE file at https://angular.io/license
 */

/**
 * Use this for attaching containers to an arbitrary DOM element outside of the Angular
 * application context.
 */
export class DomPortalOutlet extends BasePortalOutlet {
  constructor(
    /** Element into which the content is projected. */
    public outletElement: Element,
    private _componentFactoryResolver: ComponentFactoryResolver,
    private _appRef: ApplicationRef,
    private _defaultInjector: Injector
  ) {
    super();
  }

  /**
   * Attach the given ComponentContainer to DOM element using the ComponentFactoryResolver.
   * @param container Container to be attached
   * @returns Reference to the created component.
   */
  public attachComponentContainer<T>(container: ComponentPortal<T>): ComponentRef<T> {
    let componentRef: ComponentRef<T>;
    const componentFactory: ComponentFactory<T> = this._componentFactoryResolver.resolveComponentFactory(container.component);

    // If the container specifies a ViewContainerRef, we will use that as the attachment point
    // for the component (in terms of Angular's component tree, not rendering).
    // When the ViewContainerRef is missing, we use the factory to create the component directly
    // and then manually attach the view to the application.
    if (container.viewContainerRef) {
      componentRef = container.viewContainerRef.createComponent(
        componentFactory,
        container.viewContainerRef.length,
        container.injector || container.viewContainerRef.parentInjector
      );

      this.setDisposeFn(() => componentRef.destroy());
    } else {
      componentRef = componentFactory.create(container.injector || this._defaultInjector);
      this._appRef.attachView(componentRef.hostView);
      this.setDisposeFn(() => {
        this._appRef.detachView(componentRef.hostView);
        componentRef.destroy();
      });
    }
    // At this point the component has been instantiated, so we move it to the location in the DOM
    // where we want it to be rendered.
    this.outletElement.appendChild(this._getComponentRootNode(componentRef));

    return componentRef;
  }

  /**
   * Attaches a template container to the DOM as an embedded view.
   * @param container Container to be attached.
   * @returns Reference to the created embedded view.
   */
  public attachTemplateContainer<C>(container: TemplatePortal<C>): EmbeddedViewRef<C> {
    const viewContainer: ViewContainerRef = container.viewContainerRef;
    const viewRef: EmbeddedViewRef<C> = viewContainer.createEmbeddedView(container.templateRef, container.context);
    viewRef.detectChanges();

    // The method `createEmbeddedView` will add the view as a child of the viewContainer.
    // But for the DomContainerHost the view can be added everywhere in the DOM
    // (e.g Overlay Container) To move the view to the specified host element. We just
    // re-append the existing root nodes.
    viewRef.rootNodes.forEach(rootNode => this.outletElement.appendChild(rootNode));

    this.setDisposeFn(() => {
      const index: number = viewContainer.indexOf(viewRef);
      if (index !== -1) {
        viewContainer.remove(index);
      }
    });

    return viewRef;
  }

  /**
   * Clears out a container from the DOM.
   */
  public dispose(): void {
    super.dispose();
    if (this.outletElement.parentNode != null) {
      this.outletElement.parentNode.removeChild(this.outletElement);
    }
  }

  /** Gets the root HTMLElement for an instantiated component. */
  private _getComponentRootNode<T>(componentRef: ComponentRef<T>): HTMLElement {
    return (componentRef.hostView as EmbeddedViewRef<T>).rootNodes[0] as HTMLElement;
  }
}
