import {
  Component,
  EventEmitter,
  Input,
  Output,
  ViewEncapsulation,
} from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { PDCatalogCourseModel } from 'app/individual-development/models/opportunity.model';
// tslint:disable-next-line:max-line-length
import { OpportunityDetailDialogComponent } from '../opportunities-catalog-dialog/opportunity-detail-dialog/opportunity-detail-dialog.component';

@Component({
  selector: 'bookmark-opportunitites-dialog',
  templateUrl: './bookmark-opportunitites-dialog.component.html',
  styleUrls: ['./bookmark-opportunitites-dialog.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class BookmarkOpportunititesDialogComponent {
  @Input() navigationHeader: string;
  @Input() selectedCourseIds: string[];
  @Input() addText: string;
  @Output() addToPlan: EventEmitter<PDCatalogCourseModel> = new EventEmitter();
  @Output() bookmark: EventEmitter<PDCatalogCourseModel> = new EventEmitter();
  @Output() cancel: EventEmitter<void> = new EventEmitter();
  constructor(
    private ngbModal: NgbModal,
    private translateService: TranslateAdapterService
  ) {}

  onOpportunityClicked(courseModel: PDCatalogCourseModel): void {
    const modalRef = this.ngbModal.open(OpportunityDetailDialogComponent, {
      centered: true,
      windowClass: 'modal-size-xl',
    });
    const componentInstance = modalRef.componentInstance as OpportunityDetailDialogComponent;
    const currentNavigation = this.translateService.getValueImmediately(
      'MyPdJourney.Catalog.Titles.Bookmarks'
    );
    componentInstance.navigationHeader = this.translateService.getValueImmediately(
      'MyPdJourney.Catalog.Navigation',
      { title: currentNavigation }
    );
    componentInstance.courseModel = courseModel;
    componentInstance.addText = this.addText;
    componentInstance.cancel.subscribe(() => modalRef.close());
    componentInstance.addToPlan.subscribe(this.addToPlan.emit);
    componentInstance.bookmark.subscribe(() => {
      this.bookmark.emit(courseModel);
    });
  }

  onCancel(): void {
    this.cancel.emit();
  }

  onCheckBookmark(courseModel: PDCatalogCourseModel): void {
    this.bookmark.emit(courseModel);
  }
}
