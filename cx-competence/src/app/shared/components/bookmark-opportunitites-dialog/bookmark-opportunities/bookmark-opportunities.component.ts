import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewEncapsulation,
} from '@angular/core';
import { CxGlobalLoaderService } from '@conexus/cx-angular-common';
import { PdCatalogueService } from 'app-services/idp/pd-catalogue/pd-catalogue.service';
import { PDCatalogCourseModel } from 'app/individual-development/models/opportunity.model';
import { remove } from 'lodash';

@Component({
  selector: 'bookmark-opportunities',
  templateUrl: './bookmark-opportunities.component.html',
  styleUrls: ['./bookmark-opportunities.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class BookmarkOpportunitiesComponent implements OnInit {
  @Input() selectedCourseIds: string[];
  @Output() selected: EventEmitter<PDCatalogCourseModel> = new EventEmitter();
  @Output() checkBookmark: EventEmitter<
    PDCatalogCourseModel
  > = new EventEmitter();

  bookmarkedCourseModels: PDCatalogCourseModel[];

  constructor(
    private pdCatalogueService: PdCatalogueService,
    private globalLoader: CxGlobalLoaderService
  ) {}

  ngOnInit(): void {
    this.getLearningOpportunities();
  }

  onSelectOpportunity(courseModel: PDCatalogCourseModel): void {
    this.selected.emit(courseModel);
  }

  onSelectBookmark(courseModel: PDCatalogCourseModel): void {
    this.checkBookmark.emit(courseModel);
    if (courseModel.isBookmarked) {
      courseModel.isBookmarked = !courseModel.isBookmarked;
      remove(this.bookmarkedCourseModels, (element: PDCatalogCourseModel) => {
        return element.course.id === courseModel.course.id;
      });
    }
  }

  private async getLearningOpportunities(): Promise<void> {
    this.globalLoader.showLoader();
    this.bookmarkedCourseModels = await this.pdCatalogueService.getBookmarkedPDOsAsync(
      this.selectedCourseIds
    );
    this.globalLoader.hideLoader();
  }
}
