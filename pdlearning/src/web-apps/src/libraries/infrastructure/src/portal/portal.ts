import { ComponentRef, ElementRef, EmbeddedViewRef, Injector, TemplateRef, ViewContainerRef } from '@angular/core';
// tslint:disable:interface-name
/**
 * @license
 * Copyright Google LLC All Rights Reserved.
 *
 * Use of this source code is governed by an MIT-style license that can be
 * found in the LICENSE file at https://angular.io/license
 */

/**
 * Interface that can be used to generically type a class
 */
export interface ComponentType<T> {
  new (...args: unknown[]): T;
}

/**
 * This can be attach to / detached from a `ContainerHost`.
 */
export abstract class Portal<T> {
  private _attachedHost: PortalOutlet | null;

  /**
   * Attach this container to a host
   */
  public attach(host: PortalOutlet): T {
    if (host == null) {
      throw Error('Attempting to attach a container to a null host');
    }

    if (host.hasAttached()) {
      throw Error('Host has already attached the container');
    }

    this._attachedHost = host;

    return host.attach(this) as T;
  }

  /**
   * Detach this container from its host
   */
  public detach(): void {
    const host: PortalOutlet = this._attachedHost;

    if (host == null) {
      throw Error('Attempting to attach a container to a null host');
    } else {
      this._attachedHost = null;
      host.detach();
    }
  }

  /**
   * Check if this container has attached to a host
   */
  get isAttached(): boolean {
    return this._attachedHost != null;
  }

  /**
   * Sets attached host
   */
  public setAttachedHost(host: PortalOutlet | null): void {
    this._attachedHost = host;
  }
}

/**
 * This is a container that instantiates some Component upon attachment.
 */
export class ComponentPortal<T> extends Portal<ComponentRef<T>> {
  /** The type of the component that will be instantiated for attachment. */
  public component: ComponentType<T>;

  /**
   * The view container reference
   */
  public viewContainerRef?: ViewContainerRef | null;

  /**
   * [Optional] Injector used for the instantiation of the component
   */
  public injector?: Injector | null;

  constructor(component: ComponentType<T>, viewContainerRef?: ViewContainerRef | null, injector?: Injector | null) {
    super();
    this.component = component;
    this.viewContainerRef = viewContainerRef;
    this.injector = injector;
  }
}

/**
 * This is a container that represents some embedded template (TemplateRef).
 */
export class TemplatePortal<C = unknown> extends Portal<C> {
  /**
   * The embedded template that will be used to instantiate an embedded View in the host
   */
  public templateRef: TemplateRef<C>;

  /**
   * Reference to the ViewContainer into which the template will be stamped out
   */
  public viewContainerRef: ViewContainerRef;

  /**
   * Contextual data to be passed in to the embedded view
   */
  public context: C | undefined;

  constructor(template: TemplateRef<C>, viewContainerRef: ViewContainerRef, context?: C) {
    super();
    this.templateRef = template;
    this.viewContainerRef = viewContainerRef;
    this.context = context;
  }

  get origin(): ElementRef {
    return this.templateRef.elementRef;
  }

  /**
   * Attach the container to the provided `ContainerHost`.
   */
  public attach(host: PortalOutlet, context: C | undefined = this.context): C {
    this.context = context;

    return super.attach(host);
  }

  /**
   * Detach the container for its host
   */
  public detach(): void {
    this.context = undefined;

    return super.detach();
  }
}

/**
 * This is a place can contain a single container
 */
export interface PortalOutlet {
  /**
   * Attaches a container to this host
   */
  attach(container: Portal<unknown>): unknown;

  /**
   * Detaches current attached container from this host
   */
  detach(): unknown;

  /**
   * Performs cleanup before the host is destroyed
   */
  dispose(): void;

  /**
   * Check if a container has attached to this host
   */
  hasAttached(): boolean;
}

/**
 * Partial implementation of ContainerHost that handles attaching
 * ComponentContainer and TemplateContainer
 */
export abstract class BasePortalOutlet implements PortalOutlet {
  /**
   * The container currently attached to the host
   */
  protected _attachedContainer: Portal<unknown> | null;

  /**
   * A function that will permanently dispose this host
   */
  private _disposeFn: (() => void) | null;

  /**
   * Whether this host has already been permanently disposed
   */
  private _isDisposed: boolean = false;

  /**
   * Whether this host has an attached container
   */
  public hasAttached(): boolean {
    return !!this._attachedContainer;
  }

  public attach<T>(container: ComponentPortal<T>): ComponentRef<T>;
  public attach<T>(container: TemplatePortal<T>): EmbeddedViewRef<T>;
  public attach(container: unknown): unknown;

  /**
   * Attaches a container
   */
  public attach(container: Portal<unknown>): unknown {
    if (!container) {
      throw Error('Must provide a container to attach');
    }

    if (this.hasAttached()) {
      throw Error('Host has already attached a container');
    }

    if (this._isDisposed) {
      throw Error('This ContainerHost has already been disposed');
    }

    if (container instanceof ComponentPortal) {
      this._attachedContainer = container;

      return this.attachComponentContainer(container);
    } else if (container instanceof TemplatePortal) {
      this._attachedContainer = container;

      return this.attachTemplateContainer(container);
    }

    throw Error(
      'Attempting to attach an unknown container type. BaseContainerHost accepts either ' + 'a ComponentContainer or a TemplateContainer'
    );
  }

  public abstract attachComponentContainer<T>(container: ComponentPortal<T>): ComponentRef<T>;

  public abstract attachTemplateContainer<C>(container: TemplatePortal<C>): EmbeddedViewRef<C>;

  /**
   * Detaches a previously attached container
   */
  public detach(): void {
    if (this._attachedContainer) {
      this._attachedContainer.setAttachedHost(null);
      this._attachedContainer = null;
    }

    this._invokeDisposeFn();
  }

  /**
   * Permanently dispose of this container host
   */
  public dispose(): void {
    if (this.hasAttached()) {
      this.detach();
    }

    this._invokeDisposeFn();
    this._isDisposed = true;
  }

  public setDisposeFn(fn: () => void): void {
    this._disposeFn = fn;
  }

  private _invokeDisposeFn(): void {
    if (this._disposeFn) {
      this._disposeFn();
      this._disposeFn = null;
    }
  }
}
