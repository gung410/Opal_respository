import {
  Component,
  Input,
  TemplateRef,
  ViewChild,
  ViewEncapsulation,
} from '@angular/core';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'cx-slidebar',
  templateUrl: './cx-slidebar.component.html',
  styleUrls: ['./cx-slidebar.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class CxSlidebarComponent {
  @Input()
  public height: string = '90vh';

  @Input()
  public position: 'left' | 'right' = 'right';

  @ViewChild('slidebarContent')
  private slibarContent: TemplateRef<any>;

  private modalRef: NgbModalRef;

  constructor(
    private ngbModal: NgbModal) {}

  public openSlidebar(allowOpenMultiModals: boolean = false): NgbModalRef {
    if (!this.ngbModal.hasOpenModals() || allowOpenMultiModals) {
      this.modalRef = this.ngbModal.open(this.slibarContent, {
        centered: false,
        size: 'sm',
        windowClass: ('cx-slidebar-modal cx-slidebar-modal--' + this.position),
        backdropClass: 'cx-slidebar-backdrop',
      });

      setTimeout(() => {
        document
        .querySelectorAll('.cx-slidebar-modal .modal-dialog')
        .forEach((item) => {
          item.classList.add('openned');
        });
      });
    }
    return this.modalRef;
  }

  public closeSlidebar(): void {
    document
      .querySelectorAll('.cx-slidebar-modal .modal-dialog')
      .forEach((item) => {
        item.classList.add('closing');
      });

    setTimeout(() => {
      if (this.modalRef) {
        this.modalRef.close();
      }
    }, 300);
  }
}
