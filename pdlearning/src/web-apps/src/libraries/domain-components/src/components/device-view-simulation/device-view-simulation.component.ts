import { Component, ContentChild, Directive, Input, TemplateRef } from '@angular/core';

import { BaseComponent } from '@opal20/infrastructure';
import { PreviewMode } from '../../models/preview-mode.model';

@Directive({
  selector: '[webTemplate]'
})
export class WebTemplateDirective {
  constructor(public tpl: TemplateRef<unknown>) {}
}

@Directive({
  selector: '[mobileTemplate]'
})
export class MobileTemplateDirective {
  constructor(public tpl: TemplateRef<unknown>) {}
}

@Component({
  selector: 'device-view-simulation',
  templateUrl: './device-view-simulation.component.html'
})
export class DeviceViewSimulationComponent extends BaseComponent {
  @Input() public previewMode: PreviewMode = PreviewMode.Web;
  @ContentChild(WebTemplateDirective, { read: TemplateRef, static: false }) public webTemplateRef: TemplateRef<unknown>;
  @ContentChild(MobileTemplateDirective, { read: TemplateRef, static: false }) public mobileTemplateRef: TemplateRef<unknown>;
  public PreviewMode: typeof PreviewMode = PreviewMode;
}
