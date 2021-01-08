import { Align, Collision, Margin, Offset, PopupAnimation, PositionMode } from '@progress/kendo-angular-popup';
import { ElementRef, TemplateRef, ViewContainerRef } from '@angular/core';

export class OpalPopupSettings {
  /**
   * Custom popup settings
   */
  public maxWidth?: string;
  public zIndex?: string;

  /**
   * Controls the Popup animation. By default, the open and close animations are enabled
   * ([see example]({% slug animations_popup %})).
   */
  public animate?: boolean | PopupAnimation;
  /**
   * Specifies the element which will be used as an anchor. The Popup opens next to that element.
   */
  public anchor?: ElementRef;
  /**
   * Defines the container to which the Popup will be appended.
   */
  public appendTo?: ViewContainerRef;
  /**
   * Specifies the anchor pivot point
   * ([see example]({% slug alignmentpositioning_popup %})).
   */
  public anchorAlign?: Align;
  /**
   * Defines the content of the Popup.
   */
  public content?: TemplateRef<unknown> | Function;
  /**
   * Configures the collision behavior of the Popup
   * ([see example]({% slug viewportboundarydetection_popup %}).
   */
  public collision?: Collision;
  /**
   * Configures the margin value that will be added to the popup dimensions
   * in pixels and leaves a blank space between the popup and the anchor.
   */
  public margin?: Margin;
  /**
   * Specifies the position mode of the component. By default, the Popup uses fixed positioning.
   * To make the Popup acquire absolute positioning, set this option to `absolute`.
   *
   * > If you need to support mobile browsers with the zoom option, use the `absolute` positioning of the Popup.
   */
  public positionMode?: PositionMode;
  /**
   * Specifies the pivot point of the Popup ([see example]({% slug alignmentpositioning_popup %})).
   */
  public popupAlign?: Align;
  /**
   * Specifies a list of CSS classes that will be added to the internal animated element
   * ([see example]({% slug appearance_popup %})).
   *
   * > To style the content of the Popup, use this property binding.
   */
  public popupClass?: string | Array<string> | Object;
  /**
   * Specifies the absolute position of the element
   * ([see example]({% slug alignmentpositioning_popup %}#toc-aligning-to-absolute-points)).
   * The Popup opens next to that point. The pivot point of the Popup is defined by the `popupAlign` configuration option.
   * The boundary detection is applied by using the window viewport.
   */
  public offset?: Offset;
}
