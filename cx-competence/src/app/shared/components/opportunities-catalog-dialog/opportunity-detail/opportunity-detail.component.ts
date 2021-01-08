import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewEncapsulation,
} from '@angular/core';
import { PDOpportunityDetailModel } from 'app-services/pd-opportunity/pd-opportunity-detail.model';
import { PDCatalogCourseModel } from 'app/individual-development/models/opportunity.model';

@Component({
  selector: 'opportunity-detail',
  templateUrl: './opportunity-detail.component.html',
  styleUrls: ['./opportunity-detail.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class OpportunityDetailComponent implements OnInit {
  @Input() courseModel: PDCatalogCourseModel;
  @Input() addText: string;

  @Output() addToPlan: EventEmitter<PDCatalogCourseModel> = new EventEmitter();
  @Output() bookmark: EventEmitter<PDCatalogCourseModel> = new EventEmitter();
  addTextButton = 'Add to plan';
  courseDetail: PDOpportunityDetailModel;

  ngOnInit(): void {
    this.courseDetail = this.courseModel ? this.courseModel.course : undefined;
    this.addTextButton = this.addText;
  }

  onAddToPlanClicked(): void {
    this.addToPlan.emit(this.courseModel);
  }

  onBookmarkClicked(): void {
    this.bookmark.emit(this.courseModel);
  }
}
