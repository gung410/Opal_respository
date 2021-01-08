import {
  Component,
  EventEmitter,
  Input,
  Output,
  ViewEncapsulation,
} from '@angular/core';
import { PDCatalogCourseModel } from 'app/individual-development/models/opportunity.model';

@Component({
  selector: 'opportunity-detail-dialog',
  templateUrl: './opportunity-detail-dialog.component.html',
  styleUrls: ['./opportunity-detail-dialog.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class OpportunityDetailDialogComponent {
  @Input() courseModel: PDCatalogCourseModel;
  @Input() navigationHeader: string;
  @Input() addText: string;
  @Output() addToPlan: EventEmitter<PDCatalogCourseModel> = new EventEmitter();
  @Output() bookmark: EventEmitter<PDCatalogCourseModel> = new EventEmitter();
  @Output() cancel: EventEmitter<void> = new EventEmitter();

  onAddToPlanClicked(courseModel: PDCatalogCourseModel): void {
    this.addToPlan.emit(courseModel);
  }

  onBookmarkClicked(courseModel: PDCatalogCourseModel): void {
    this.bookmark.emit(courseModel);
  }

  onCancel(): void {
    this.cancel.emit();
  }
}
