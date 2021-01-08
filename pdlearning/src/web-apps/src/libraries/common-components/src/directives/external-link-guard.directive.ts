import { AfterViewInit, Directive, ElementRef, Input } from '@angular/core';
import { DomUtils, ModalService, ModuleFacadeService, Utils } from '@opal20/infrastructure';
@Directive({
  selector: '[externalLinkGuard]'
})
export class ExternalLinkGuardDirective implements AfterViewInit {
  @Input() public href: string;

  constructor(private modalService: ModalService, private ref: ElementRef, private facadeService: ModuleFacadeService) {}

  public ngAfterViewInit(): void {
    const anchors: NodeList = this.ref.nativeElement.querySelectorAll('a');
    anchors.forEach((el: HTMLAnchorElement) => el.addEventListener('click', this.handleClick.bind(this)));
  }

  public handleClick(event: MouseEvent): void {
    const el: HTMLAnchorElement = event.currentTarget as HTMLAnchorElement;
    const href = Utils.getHrefFromAnchor(el);
    if (href) {
      const url: string = this.validateAndCorrectUrlToAbsolutePath(href);
      Utils.isInternalUrl(url) ? window.open(url, '_blank') : this.confirmToOpenSite(url);
    }
    DomUtils.preventDefaultEvent(event);
  }

  private confirmToOpenSite(url: string): void {
    const width = 500;
    this.modalService.showConfirmMessage(
      this.facadeService.globalTranslator.instant('ExternalLinkGuard.warning'),
      () => {
        window.open(url, '_blank');
      },
      null,
      null,
      width
    );
  }

  private validateAndCorrectUrlToAbsolutePath(href: string): string {
    return !Utils.isAbsoluteUrl(href) ? `//${href}` : href;
  }
}
