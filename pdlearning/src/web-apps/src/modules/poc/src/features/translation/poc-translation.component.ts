import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';

import { Component } from '@angular/core';

@Component({
  selector: 'translation',
  template: `
    <h3>Translation Component</h3>
    <span>Global language: {{ 'OK' | globalTranslator }}</span>
    <br />
    <span>Local language: {{ 'poc.hello' | translator }}</span>
    <br />
    <span><button (click)="changeLanguage('vi')">Vietnamese</button></span>
    <span><button (click)="changeLanguage('en')">English</button></span>
  `
})
export class POCTranslationComponent extends BasePageComponent {
  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public changeLanguage(lang: string): void {
    this.moduleFacadeService.globalTranslator.use(lang);
  }
}
