import { BasePortalOutlet, ComponentPortal, Portal, TemplatePortal } from './portal';
import {
  ComponentFactory,
  ComponentFactoryResolver,
  ComponentRef,
  Directive,
  EmbeddedViewRef,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
  TemplateRef,
  ViewContainerRef
} from '@angular/core';
/**
 * @license
 * Copyright Google LLC All Rights Reserved.
 *
 * Use of this source code is governed by an MIT-style license that can be
 * found in the LICENSE file at https://angular.io/license
 */

@Directive({
  selector: '[template-container], [templateContainer]',
  exportAs: 'templateContainer'
})
/**
 * Directive for TemplateContainer
 */
export class CustomPortal extends TemplatePortal {
  constructor(templateRef: TemplateRef<unknown>, viewContainerRef: ViewContainerRef) {
    super(templateRef, viewContainerRef);
  }
}

/**
 * Possible attached references to the MjsContainerHost.
 */
export type PortalOutletAttachedRef = ComponentRef<unknown> | EmbeddedViewRef<unknown> | null;

/**
 * Directive version of a ContainerHost
 *
 * Usage:
 * `<ng-template [mjsContainerHost]="greeting"></ng-template>`
 */
@Directive({
  selector: '[portalOutlet], [portal-outlet]',
  exportAs: 'portalOutlet'
})
// tslint:disable-next-line:directive-class-suffix
export class PortalOutlet extends BasePortalOutlet implements OnInit, OnDestroy {
  @Output()
  public attached: EventEmitter<PortalOutletAttachedRef> = new EventEmitter();

  /**
   * Whether the container component is initialized
   */
  private _isInitialized: boolean = false;

  /**
   * Reference to the currently-attached component/view ref
   */
  private _attachedRef: PortalOutletAttachedRef;

  constructor(private _componentFactoryResolver: ComponentFactoryResolver, private _viewContainerRef: ViewContainerRef) {
    super();
  }

  /**
   * Container associated with the container host
   */
  get container(): Portal<unknown> | null {
    return this._attachedContainer;
  }

  /**
   * Set container to this host
   */
  @Input('portalOutlet')
  set container(container: Portal<unknown> | null) {
    if (this.hasAttached() && !container && !this._isInitialized) {
      return;
    }

    if (this.hasAttached()) {
      super.detach();
    }

    if (container) {
      super.attach(container);
    }

    this._attachedContainer = container;
  }

  /**
   * Component or view reference that is attached to the container
   */
  get attachedRef(): PortalOutletAttachedRef {
    return this._attachedRef;
  }

  public ngOnInit(): void {
    this._isInitialized = true;
  }

  public ngOnDestroy(): void {
    super.dispose();
    this._attachedContainer = null;
    this._attachedRef = null;
  }

  /**
   * Attach the given ComponentContainer to this ContainerHost using the ComponentFactoryResolver.
   *
   * @param container Container to be attached to the container host
   * @returns Reference to the created component.
   */
  public attachComponentContainer<T>(container: ComponentPortal<T>): ComponentRef<T> {
    container.setAttachedHost(this);

    // If the container specifies an origin, use that as the logical location of the component
    // in the application tree. Otherwise use the location of this ContainerHost.
    const viewContainerRef: ViewContainerRef = container.viewContainerRef != null ? container.viewContainerRef : this._viewContainerRef;

    const componentFactory: ComponentFactory<T> = this._componentFactoryResolver.resolveComponentFactory(container.component);
    const ref: ComponentRef<T> = viewContainerRef.createComponent(
      componentFactory,
      viewContainerRef.length,
      container.injector || viewContainerRef.parentInjector
    );

    super.setDisposeFn(() => ref.destroy());
    this._attachedContainer = container;
    this._attachedRef = ref;
    this.attached.emit(ref);

    return ref;
  }

  /**
   * Attach the given TemplateContainer to this ContainerHost as an embedded View.
   * @param container Container to be attached.
   * @returns Reference to the created embedded view.
   */
  public attachTemplateContainer<C>(container: TemplatePortal<C>): EmbeddedViewRef<C> {
    container.setAttachedHost(this);
    const viewRef: EmbeddedViewRef<C> = this._viewContainerRef.createEmbeddedView(container.templateRef, container.context);
    super.setDisposeFn(() => this._viewContainerRef.clear());

    this._attachedContainer = container;
    this._attachedRef = viewRef;
    this.attached.emit(viewRef);

    return viewRef;
  }
}
