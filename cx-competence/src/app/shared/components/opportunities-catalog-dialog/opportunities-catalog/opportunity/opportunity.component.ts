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
  selector: 'opportunity-card',
  templateUrl: './opportunity.component.html',
  styleUrls: ['./opportunity.component.scss'],
  styles: [
    `
      :host {
        display: block;
      }
    `,
  ],
  encapsulation: ViewEncapsulation.None,
})
export class OpportunityComponent implements OnInit {
  @Input() enableBookmark: boolean = true;
  @Input() courseModel: PDCatalogCourseModel;
  @Output() selected: EventEmitter<PDCatalogCourseModel> = new EventEmitter();
  @Output() bookmark: EventEmitter<PDCatalogCourseModel> = new EventEmitter();

  courseDetail: PDOpportunityDetailModel;

  ngOnInit(): void {
    this.courseDetail = this.courseModel ? this.courseModel.course : undefined;
  }

  onClick(): void {
    this.selected.emit(this.courseModel);
  }

  onBookmarkClicked(): void {
    this.bookmark.emit(this.courseModel);
  }
}
