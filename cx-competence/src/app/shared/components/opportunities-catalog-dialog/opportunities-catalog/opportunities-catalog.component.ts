import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewEncapsulation,
} from '@angular/core';
import { PDCatalogCourseModel } from 'app/individual-development/models/opportunity.model';
import { isEmpty } from 'lodash';

@Component({
  selector: 'opportunities-catalog',
  templateUrl: './opportunities-catalog.component.html',
  styleUrls: ['./opportunities-catalog.component.scss'],
  styles: [
    `
      :host {
        display: block;
      }
    `,
  ],
  encapsulation: ViewEncapsulation.None,
})
export class OpportunitiesCatalogComponent implements OnInit {
  @Input() enableBookmark: boolean = true;
  @Input() searchCourses: PDCatalogCourseModel[];
  @Input() recommendedCourses: PDCatalogCourseModel[];
  @Input() isSearching: boolean;
  @Input() pickCourseMode: boolean = false;
  @Input() showLoadMoreButton: boolean = false;
  @Input() hasMoreRecommendCourse: boolean = true;
  @Input() hasMoreSearchCourse: boolean = true;
  @Input() addText: string;

  @Output() loadMoreSearchCourses: EventEmitter<void> = new EventEmitter();
  @Output() loadMoreRecommendCourses: EventEmitter<void> = new EventEmitter();
  @Output() selected: EventEmitter<PDCatalogCourseModel> = new EventEmitter();
  @Output() bookmark: EventEmitter<PDCatalogCourseModel> = new EventEmitter();

  // Secondary variables
  pageSize: number = 12;
  pageIndex: number = 0;
  isLoaded: boolean = false;
  constructor() {}

  ngOnInit(): void {}

  onSelectOpportunity(courseModel: PDCatalogCourseModel): void {
    this.selected.emit(courseModel);
  }

  onSelectBookmark(courseModel: PDCatalogCourseModel): void {
    this.bookmark.emit(courseModel);
  }

  get noRecommendedCourse(): boolean {
    return isEmpty(this.recommendedCourses);
  }

  get noSearchCourse(): boolean {
    return isEmpty(this.searchCourses);
  }

  get showPDCatalogueCourses(): boolean {
    return !this.isSearching && isEmpty(this.recommendedCourses);
  }

  get showLoadMoreSearchCourse(): boolean {
    return (
      !this.noSearchCourse &&
      this.showLoadMoreButton &&
      this.hasMoreSearchCourse
    );
  }

  get showLoadMoreRecommendCourse(): boolean {
    return (
      !this.noRecommendedCourse &&
      this.showLoadMoreButton &&
      this.hasMoreRecommendCourse
    );
  }
}
