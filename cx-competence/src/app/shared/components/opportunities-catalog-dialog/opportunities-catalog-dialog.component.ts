import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
  ViewEncapsulation,
} from '@angular/core';
import { CxGlobalLoaderService } from '@conexus/cx-angular-common';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from 'app-auth/auth.service';
import { IDictionary } from 'app-models/dictionary';
import { PDOpportunityDTO } from 'app-models/mpj/pdo-action-item.model';
import { ExternalPDOService } from 'app-services/idp/pd-catalogue/external-pdo.service';
import { PdCatalogueService } from 'app-services/idp/pd-catalogue/pd-catalogue.service';
import { PDPlannerHelpers } from 'app-services/idp/pd-planner/pd-planner-helpers';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { CourseDetailModalComponent } from 'app/approval-page/modals/course-detail-modal/course-detail-modal.component';
import { CoursepadNoteDto } from 'app/individual-development/models/course-note.model';
import { PDCatalogCourseModel } from 'app/individual-development/models/opportunity.model';
import { isEmpty } from 'lodash';
import { fromEvent, Subject } from 'rxjs';
import { BookmarkOpportunititesDialogComponent } from '../bookmark-opportunitites-dialog/bookmark-opportunitites-dialog.component';
import { FilterCatalogSlidebarService } from '../catalog-filter-form/services/filter-catalog-slidebar.service';
import { BaseScreenComponent } from '../component.abstract';

@Component({
  selector: 'opportunities-catalog-dialog',
  templateUrl: './opportunities-catalog-dialog.component.html',
  styleUrls: ['./opportunities-catalog-dialog.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class OpportunitiesCatalogDialogComponent
  extends BaseScreenComponent
  implements OnInit {
  @Input() enableBookmark: boolean = true;
  @Input() selectedCourseIds: string[];
  @Input() tagIds: string[];
  @Input() personnelGroupsIdsForExternalPDOUsage: string[];
  @Input() allowPickMultiplePDO: boolean = true;
  @Input() pickCourseMode: boolean = false;
  @Input() dialogTitle: string = 'Add PD Opportunity';
  @Input() addText: string = 'Add to plan';
  @Input() showLoadMoreButton: boolean = false;
  @Input() allowAddExternalPDOPermission: boolean = true;

  @Output() cancel: EventEmitter<any> = new EventEmitter();
  @Output()
  addToPlan: EventEmitter<PDCatalogCourseModel> = new EventEmitter<PDCatalogCourseModel>();
  @Output()
  addExternalPDO: EventEmitter<PDOpportunityDTO> = new EventEmitter<PDOpportunityDTO>();
  @Output() addToBookmark: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('cxDialogTemplateElement', { read: ElementRef })
  set _cxDialogTemplateElement(elementRef: ElementRef) {
    const modalBodyElemenRef = elementRef.nativeElement.querySelector(
      '.modal-body'
    );
    this.modalBodyElemenRef = modalBodyElemenRef;
    fromEvent(modalBodyElemenRef, 'scroll')
      .throttleTime(100)
      .subscribe(this.onModalBodyScroll);
  }
  modalBodyElemenRef: any;

  navigationHeader: string;
  catalogTitle: string;
  bookmarkedCourses: Map<string, CoursepadNoteDto>;
  currentCatalogueCourse: PDCatalogCourseModel;

  // Catalogue Input
  searchCourses: PDCatalogCourseModel[];
  recommendedCourses: PDCatalogCourseModel[];
  isSearching: boolean = false;
  hasMoreDataSearchCourse: boolean = true;
  hasMoreDataRecommendCourse: boolean = true;

  private pageIndexSearchCourse: number = 0;
  private pageIndexRecommendCourse: number = 0;
  private pageSize: number = 12;
  private searchText: string = '';
  private filterTagIds: string[] = [];

  private getMoreSearchCourses$: Subject<void> = new Subject();
  private getMoreRecommendCourses$: Subject<void> = new Subject();

  constructor(
    protected authService: AuthService,
    protected changeDetectorRef: ChangeDetectorRef,
    private pdCatalogueService: PdCatalogueService,
    private externalPDOService: ExternalPDOService,
    private translateService: TranslateAdapterService,
    private filterSlidebarService: FilterCatalogSlidebarService,
    private ngbModal: NgbModal,
    private globalLoader: CxGlobalLoaderService
  ) {
    super(changeDetectorRef, authService);
  }

  ngOnInit(): void {
    const currentNavigation = this.translateService.getValueImmediately(
      'MyPdJourney.Catalog.Titles.AddCatalog'
    );
    this.navigationHeader = this.translateService.getValueImmediately(
      'MyPdJourney.Catalog.Navigation',
      { title: currentNavigation }
    );
    this.getMoreSearchCourses$.subscribe(this.getCourseSearch);
    this.getMoreRecommendCourses$.subscribe(this.getCourseRecommended);
    this.getData();
    this.subscribeFilterEvent();
  }

  subscribeFilterEvent(): void {
    this.subscriptionAdder = this.filterSlidebarService.onSubmitFilter.subscribe(
      async (params) => {
        this.onFilter(params as IDictionary<string[]>);
      }
    );
  }

  onOpportunityClicked(pdCatalogCourse: PDCatalogCourseModel): void {
    const ngbModal = this.ngbModal.open(CourseDetailModalComponent, {
      centered: true,
      size: 'lg',
      windowClass: 'mobile-dialog-slide-right',
    });

    const modalRef = ngbModal.componentInstance as CourseDetailModalComponent;
    modalRef.courseId = pdCatalogCourse.course.id;
    modalRef.selectCourseMode = true;
    modalRef.selectCourseModeDisplayText = this.addText;
    modalRef.selectCourseModeSelected = pdCatalogCourse.isSelected;
    modalRef.addToPlan.subscribe((courseId) => {
      this.onAddingToPlanFromCourseId(courseId);
      if (!this.allowPickMultiplePDO) {
        ngbModal.close();
      }
    });
    modalRef.close.subscribe(() => ngbModal.close(false));
  }

  onCancel = (): void => {
    this.cancel.emit();
  };

  onBookmarkClicked(): void {
    const modalRef = this.ngbModal.open(BookmarkOpportunititesDialogComponent, {
      windowClass: 'modal-size-xl mobile-dialog-slide-right',
      centered: true,
    });
    const componentInstance = modalRef.componentInstance as BookmarkOpportunititesDialogComponent;
    componentInstance.navigationHeader = this.navigationHeader;
    componentInstance.selectedCourseIds = this.selectedCourseIds;
    componentInstance.addText = this.addText;
    componentInstance.cancel.subscribe(() => {
      this.getMoreSearchCourses$.next();
      modalRef.close();
    });
    componentInstance.addToPlan.subscribe(this.onAddingToPlan);
    componentInstance.bookmark.subscribe(this.onBookmarked);
  }

  onAddingToPlanFromCourseId = (courseId: string): void => {
    let pdCatalogCourseModel;
    if (!isEmpty(this.searchCourses)) {
      pdCatalogCourseModel = this.searchCourses.filter(
        (p) => p.course.id === courseId
      );
    } else {
      pdCatalogCourseModel = this.recommendedCourses.filter(
        (p) => p.course.id === courseId
      );
    }
    if (!isEmpty(pdCatalogCourseModel)) {
      this.onAddingToPlan(pdCatalogCourseModel[0]);
    }
  };

  onAddingToPlan = (pdCatalogCourseModel: PDCatalogCourseModel): void => {
    this.addToPlan.emit(pdCatalogCourseModel);
    if (!this.allowPickMultiplePDO) {
      this.cancel.emit();
    }
  };

  onBookmarked = (courseModel: PDCatalogCourseModel): void => {
    this.addToBookmark.emit(courseModel);
  };

  onAddExternalClicked(): void {
    this.showExternalPDOForm();
  }

  onSearch(searchText: string): void {
    if (this.searchText === searchText) {
      return;
    }
    this.isSearching = !isEmpty(searchText);

    if (!this.isSearching && !this.showPDCatalogueCourses) {
      return;
    }

    this.searchText = searchText;
    this.pageIndexSearchCourse = 0;
    this.modalBodyElemenRef.scrollTo(0, 0);
    this.getMoreSearchCourses$.next();
  }

  onFilter(params: IDictionary<string[]>): void {
    const tagIds = this.getTagIdFromParams(params);
    if (this.filterTagIds === tagIds) {
      return;
    }
    this.isSearching = !isEmpty(tagIds);

    if (!this.isSearching && !this.showPDCatalogueCourses) {
      return;
    }

    this.filterTagIds = tagIds;
    this.pageIndexSearchCourse = 0;
    this.modalBodyElemenRef.scrollTo(0, 0);
    this.getMoreSearchCourses$.next();
  }

  onClickLoadMoreButton(): void {
    this.getMoreCourses();
  }

  get showPDCatalogueCourses(): boolean {
    return !this.isSearching && isEmpty(this.recommendedCourses);
  }

  private async getData(): Promise<void> {
    if (this.enableBookmark) {
      this.bookmarkedCourses = await this.pdCatalogueService.getBookmarkedPDOsMapAsync();
    }
    this.getMoreRecommendCourses$.next();
  }

  private getCourseSearch = async (): Promise<void> => {
    this.globalLoader.showLoader();
    this.changeDetectorRef.detectChanges();
    let results: PDCatalogCourseModel[] = [];

    const responseData = await this.pdCatalogueService.searchPDCatalogue(
      this.searchText,
      this.filterTagIds,
      this.pageIndexSearchCourse,
      this.pageSize
    );
    if (!responseData || isEmpty(responseData.items)) {
      this.searchCourses =
        this.pageIndexSearchCourse > 0 ? this.searchCourses : [];
      this.hasMoreDataSearchCourse = false;
      this.globalLoader.hideLoader();
      this.changeDetectorRef.detectChanges();

      return;
    }

    this.hasMoreDataSearchCourse = responseData.hasMoreData;
    results = this.pdCatalogueService.updatePDOSelectBookmarkInfo(
      responseData.items,
      this.bookmarkedCourses,
      this.selectedCourseIds
    );

    this.searchCourses = this.searchCourses || [];
    this.searchCourses =
      this.pageIndexSearchCourse > 0
        ? this.searchCourses.concat(results)
        : results;
    this.globalLoader.hideLoader();
    this.changeDetectorRef.detectChanges();
  };

  private getCourseRecommended = async (): Promise<void> => {
    this.globalLoader.showLoader();

    let results: PDCatalogCourseModel[] = [];

    const responseData = await this.pdCatalogueService.getRecommendFromPDCatalogueAsync(
      this.tagIds,
      this.pageIndexRecommendCourse,
      this.pageSize
    );

    if (!responseData || isEmpty(responseData.items)) {
      // Case no any recommend course, show pd catalogue courses
      if (this.pageIndexRecommendCourse === 0) {
        this.getCourseSearch();
      }

      this.recommendedCourses =
        this.pageIndexRecommendCourse > 0 ? this.recommendedCourses : [];
      this.hasMoreDataRecommendCourse = false;
      this.globalLoader.hideLoader();
      this.changeDetectorRef.detectChanges();

      return;
    }

    this.hasMoreDataRecommendCourse = responseData.hasMoreData;
    results = this.pdCatalogueService.updatePDOSelectBookmarkInfo(
      responseData.items,
      this.bookmarkedCourses,
      this.selectedCourseIds
    );
    this.recommendedCourses = this.recommendedCourses || [];
    this.recommendedCourses =
      this.pageIndexRecommendCourse > 0
        ? this.recommendedCourses.concat(results)
        : results;
    this.globalLoader.hideLoader();
    this.changeDetectorRef.detectChanges();
  };

  private async showExternalPDOForm(): Promise<void> {
    this.globalLoader.showLoader();

    const modalRef = await this.externalPDOService.showExternalPDOFormAsync(
      this.personnelGroupsIdsForExternalPDOUsage
    );
    if (!modalRef) {
      this.globalLoader.hideLoader();

      return;
    }

    const modalRefComponent = modalRef.componentInstance;
    modalRefComponent.changeValue.subscribe(() => {});

    modalRefComponent.submitting.subscribe((event) => {
      const pdoDTO = PDPlannerHelpers.externalToPDOpportunityDTO(
        event.survey.data
      );
      this.addExternalPDO.emit(pdoDTO);
      modalRef.close();
      if (!this.allowPickMultiplePDO) {
        this.cancel.emit();
      }
    });

    this.globalLoader.hideLoader();
  }

  private onModalBodyScroll = (event: any): void => {
    if (!this.hasMoreDataSearchCourse && !this.hasMoreDataRecommendCourse) {
      return;
    }
    const itemHeight = 160;
    const target = event.target;
    const scrollHeight: number = target.scrollHeight;
    const scrollTop: number = target.scrollTop;
    const offsetHeight: number = target.offsetHeight;
    if (scrollHeight - scrollTop - offsetHeight <= itemHeight) {
      this.getMoreCourses();
    }
  };

  private getMoreCourses(): void {
    if (
      (this.isSearching && this.hasMoreDataSearchCourse) ||
      this.showPDCatalogueCourses
    ) {
      this.pageIndexSearchCourse += 1;
      this.getMoreSearchCourses$.next();
    } else if (!this.isSearching && this.hasMoreDataRecommendCourse) {
      this.pageIndexRecommendCourse += 1;
      this.getMoreRecommendCourses$.next();
    }
  }

  private getTagIdFromParams(params: IDictionary<string[]>): string[] {
    let tagIds = [];
    tagIds = tagIds
      .concat(params.pdActivityType ? params.pdActivityType : [])
      .concat(params.learningMode ? params.learningMode : [])
      .concat(params.categorySelectedItems ? params.categorySelectedItems : [])
      .concat(
        params.serviceSchemeSelectedItems
          ? params.serviceSchemeSelectedItems
          : []
      )
      .concat(
        params.developmentalRoleSelectedItems
          ? params.developmentalRoleSelectedItems
          : []
      )
      .concat(
        params.teachingLevelSelectedItems
          ? params.teachingLevelSelectedItems
          : []
      )
      .concat(params.courseLevel ? params.courseLevel : [])
      .concat(
        params.subjectAreaSelectedItems ? params.subjectAreaSelectedItems : []
      )
      .concat(
        params.learningFrameworkSelectedItems
          ? params.learningFrameworkSelectedItems
          : []
      )
      .concat(
        params.learningDimensionSelectedItems
          ? params.learningDimensionSelectedItems
          : []
      )
      .concat(
        params.learningAreaSelectedItems ? params.learningAreaSelectedItems : []
      )
      .concat(
        params.learningSubAreaSelectedItems
          ? params.learningSubAreaSelectedItems
          : []
      )
      .concat(params.natureOfCourse ? params.natureOfCourse : []);

    return tagIds;
  }
}
