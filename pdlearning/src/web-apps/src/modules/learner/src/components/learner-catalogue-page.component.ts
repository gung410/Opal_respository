import { BasePageComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { CATALOGUE_MAPPING_TYPE_CONST, CATALOGUE_TYPE_ENUM } from '../constants/catalogue-type.constant';
import { Component, ElementRef, ViewChild } from '@angular/core';
import { ILearningItemModel, LearningItemModel } from '../models/learning-item.model';
import { INavigationFilterCriteria, LearnerRoutePaths } from '@opal20/domain-components';
import { NavigationEnd, Router, RouterEvent } from '@angular/router';
import { filter, map } from 'rxjs/operators';

import { CatalogResourceType } from '@opal20/domain-api';
import { CatalogueDataService } from '../services/catalogue-data.service';
import { CourseDataService } from '../services/course-data.service';
import { LearningCardListComponent } from './learning-card-list.component';
import { Observable } from 'rxjs';
import { SECTION_WIDGET } from '../constants/section-widget';
import { StandaloneFormItemModel } from '../models/standalone-form-item.model';

@Component({
  selector: 'learner-catalogue-page',
  templateUrl: './learner-catalogue-page.component.html'
})
export class LearnerCataloguePageComponent extends BasePageComponent {
  public suggestedLearningItems: ILearningItemModel[] = [];
  public organisationSuggestedLearningItems: ILearningItemModel[] = [];
  public newlyAddedLearningItems: ILearningItemModel[] = [];
  public sharedToMeItems: ILearningItemModel[] = [];

  public currentCourseId: string | undefined;
  public currentLearningItem: ILearningItemModel | undefined;

  public catalogueSearchTypes: CATALOGUE_SEARCH_TYPE[] = [
    { name: 'All', value: 'all', total: 0 },
    { name: 'Course', value: 'course', total: 0 },
    { name: 'Microlearning', value: 'microlearning', total: 0 },
    { name: 'Digital content', value: 'content', total: 0 },
    { name: 'Learning paths', value: 'learningpath', total: 0 },
    { name: 'Community', value: 'community', total: 0 },
    { name: 'Form', value: 'form', total: 0 }
  ];
  public activeSearchType: CatalogResourceType = 'all';
  public total: number = 0;

  public suggestedLearningItemsTotalCount: number = 0;
  public organisationSuggestedLearningItemsTotalCount: number = 0;
  public newlyAddedLearningItemsTotalCount: number = 0;
  public sharedToMeItemsTotalCount: number = 0;
  public catalogueByTypeItemsTotalCount: number = 0;

  public navigateData: { catalogueType: CATALOGUE_TYPE_ENUM; displayText: string; filterCriteria: INavigationFilterCriteria };

  @ViewChild('searchInput', { static: false })
  public searchInput: ElementRef;

  public showingCatalogueSearch: boolean = true;
  public showingCourseSearchResult: boolean = false;
  public searchText: string = '';
  public searchTypeByParamInNavigation: CatalogResourceType;

  public get isShowClearText(): boolean {
    return !Utils.isNullOrEmpty(this.searchText);
  }
  public showAdvFilter: boolean = false;

  public currentCardListType: CATALOGUE_TYPE_ENUM;
  public cardListCallback: CardListCallbackType;
  public cardListTitle: string;
  public showingCardList: boolean = false;

  public previousSearchText: string = '';
  public filterNumber: number = 0;
  @ViewChild('searchResultList', { static: false })
  private searchResultListComponent: LearningCardListComponent;

  @ViewChild('cardListDetail', { static: false })
  private cardListDetailComponent: LearningCardListComponent;

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private courseDataService: CourseDataService,
    private catalogDataService: CatalogueDataService,
    private subNavigateRouter: Router
  ) {
    super(moduleFacadeService);
    this.catalogDataService.clearFilter();
    this.currentLearningItem = undefined;
    this.loadDataDefaultPage();
  }

  public onInit(): void {
    this.updateDeeplink(`learner/${LearnerRoutePaths.Catalogue}`);
    this.updateDeeplinkWithParameterInNavigation();
    this.subNavigateRouter.events
      .pipe(
        filter((event: RouterEvent) => event instanceof NavigationEnd),
        this.untilDestroy()
      )
      .subscribe(() => {
        this.currentLearningItem = undefined;
        this.updateDeeplinkWithParameterInNavigation();
      });
  }

  public onCardListBack(): void {
    this.backToDefaultPage();
  }

  public getPagedSearchCoursesCallBack(
    maxResultCount: number,
    skipCount: number
  ): Observable<{
    total: number;
    items: ILearningItemModel[];
  }> {
    const allResourceTypes: CatalogResourceType[] = this.catalogueSearchTypes.map(p => p.value);
    return this.catalogDataService.searchCourse(this.searchText, this.activeSearchType, allResourceTypes, maxResultCount, skipCount).pipe(
      map(_ => {
        this.searchInput.nativeElement.blur();
        this.total = _.total;
        _.resourceStatistics.forEach(resourceStatistic => {
          const catalogueSearchType = this.catalogueSearchTypes.find(p => p.value === resourceStatistic.type);
          catalogueSearchType.total = resourceStatistic.total;
        });

        return {
          total: _.resourceStatistics.find(e => e.type === this.activeSearchType).total,
          items: _.items
        };
      })
    );
  }

  public getPagedCatalogueByTypeCallBack(
    maxResultCount: number,
    skipCount: number
  ): Observable<{
    total: number;
    items: ILearningItemModel[];
  }> {
    return this.catalogDataService
      .searchPDCatalogueByType(this.searchText, this.searchTypeByParamInNavigation, maxResultCount, skipCount)
      .pipe(
        map(_ => {
          return {
            total: _.total,
            items: _.items
          };
        })
      );
  }

  public getSuggestedCoursesCallBack(
    maxResultCount: number,
    skipCount: number
  ): Observable<{
    total: number;
    items: ILearningItemModel[];
  }> {
    return this.catalogDataService.getSuggestedCourses(maxResultCount, skipCount).pipe(
      map(result => {
        const items = result.items;
        return {
          total: result.total,
          items: items
        };
      })
    );
  }

  public getOrganisationSuggestedCoursesCallBack(
    maxResultCount: number,
    skipCount: number
  ): Observable<{
    total: number;
    items: ILearningItemModel[];
  }> {
    return this.courseDataService.getOrganisationSuggestedCourses(maxResultCount, skipCount).pipe(
      map(result => {
        return {
          total: result.total,
          items: result.items
        };
      })
    );
  }

  public getNewlyAddedCoursesCallBack(
    maxResultCount: number,
    skipCount: number
  ): Observable<{
    total: number;
    items: ILearningItemModel[];
  }> {
    return this.catalogDataService.getNewlyAddedCourses(true, maxResultCount, skipCount).pipe(
      map(result => {
        return {
          total: result.total,
          items: result.items
        };
      })
    );
  }

  public getSharedToMeCallBack(
    maxResultCount: number,
    skipCount: number
  ): Observable<{
    total: number;
    items: ILearningItemModel[];
  }> {
    return this.catalogDataService.getSharedToMe(maxResultCount, skipCount).pipe(
      map(result => {
        const items = result.items;
        return {
          total: result.total,
          items: items
        };
      })
    );
  }

  public searchOnType(type: CATALOGUE_SEARCH_TYPE): void {
    this.activeSearchType = type.value;
    this.searchResultListComponent.triggerDataChange(true);
  }

  public onSearchText(): void {
    if (this.searchText === this.previousSearchText) {
      return;
    }
    this.searchCourse();
  }

  public searchCourse(): void {
    this.showingCourseSearchResult = this.searchText !== '' || this.filterNumber > 0;

    if (this.showingCourseSearchResult && this.searchResultListComponent !== undefined) {
      this.searchResultListComponent.triggerDataChange(true);
    }

    if (this.searchText === '' && this.filterNumber === 0) {
      this.backToDefaultPage();
    }
    this.previousSearchText = this.searchText;
  }

  public resetFilter(): void {
    if (this.searchResultListComponent !== undefined) {
      this.catalogDataService.clearFilter();
      this.filterNumber = this.catalogDataService.model.count;
      this.showingCourseSearchResult = false;
      this.searchResultListComponent.triggerDataChange(true);
    }
    this.searchText = '';
    this.previousSearchText = '';
  }

  public showAdvSearchFilter(): void {
    this.showAdvFilter = true;
  }

  public hideAdvSearchFilter(): void {
    this.showAdvFilter = false;
  }

  public applyAdvSearchFilter(): void {
    this.filterNumber = this.catalogDataService.model.count;
    this.searchCourse();
  }

  public onLearningCardClick(event: ILearningItemModel): void {
    this.currentLearningItem = event;
    if (event instanceof LearningItemModel) {
      this.onMicrolearningClick(event);
      return;
    }

    if (event instanceof StandaloneFormItemModel) {
      return;
    }

    this.currentCourseId = event.id;

    this.updateDeeplink(`learner/${LearnerRoutePaths.Detail}/${event.type.toLocaleLowerCase()}/${this.currentCourseId}`);
  }

  public onMicrolearningClick(event: LearningItemModel): void {
    this.updateDeeplink(`learner/${LearnerRoutePaths.Detail}/${event.type.toLocaleLowerCase()}/${event.id}`);
    if (event.isExpiredCourse) {
      return;
    }

    this.currentCourseId = event.id;
  }

  public onShowCourseRecommendationsClick(): void {
    this.showCardList(CATALOGUE_TYPE_ENUM.RecommendedForYou);
  }

  public onShowOrgCourseRecommendationsClick(): void {
    this.showCardList(CATALOGUE_TYPE_ENUM.RecommendedByYourOrganisation);
  }

  public onShowNewlyAddedCoursesClick(): void {
    this.showCardList(CATALOGUE_TYPE_ENUM.NewlyAdded);
  }

  public onShowSharedToMeItemsClick(): void {
    this.showCardList(CATALOGUE_TYPE_ENUM.SharedToMe);
  }

  public showCardList(catalogueType: CATALOGUE_TYPE_ENUM): void {
    this.currentCardListType = catalogueType;
    this.showingCardList = true;
    this.updateDeeplink(`learner/${LearnerRoutePaths.Catalogue}/${catalogueType}`);
    switch (catalogueType) {
      case CATALOGUE_TYPE_ENUM.SharedToMe:
        this.showingCatalogueSearch = false;
        this.cardListTitle = this.translate(SECTION_WIDGET.Shared);
        this.cardListCallback = this.getSharedToMeCallBack;
        break;
      case CATALOGUE_TYPE_ENUM.RecommendedForYou:
        this.showingCatalogueSearch = false;
        this.cardListTitle = this.translate(SECTION_WIDGET.RecommendedForYou);
        this.cardListCallback = this.getSuggestedCoursesCallBack;
        break;
      case CATALOGUE_TYPE_ENUM.RecommendedByYourOrganisation:
        this.showingCatalogueSearch = false;
        this.cardListTitle = this.translate(SECTION_WIDGET.RecommendedByYourOrganisation);
        this.cardListCallback = this.getOrganisationSuggestedCoursesCallBack;
        break;
      case CATALOGUE_TYPE_ENUM.NewlyAdded:
        this.showingCatalogueSearch = false;
        this.cardListTitle = this.translate(SECTION_WIDGET.NewlyAdded);
        this.cardListCallback = this.getNewlyAddedCoursesCallBack;
        break;
      case CATALOGUE_TYPE_ENUM.Courses:
      case CATALOGUE_TYPE_ENUM.Microlearning:
      case CATALOGUE_TYPE_ENUM.DigitalContent:
        this.searchTypeByParamInNavigation = CATALOGUE_MAPPING_TYPE_CONST.get(catalogueType);

        this.showingCatalogueSearch = false;
        this.cardListTitle = this.translate(this.navigateData.displayText);
        this.cardListCallback = this.getPagedCatalogueByTypeCallBack;
        break;
      case CATALOGUE_TYPE_ENUM.AllCourses:
        this.onCardListBack();
        break;
    }
  }

  public onCourseDetailBackClick(): void {
    this.currentCourseId = undefined;
    this.currentLearningItem = undefined;
    this.loadDataDefaultPage();

    if (this.showingCourseSearchResult && this.searchResultListComponent != null) {
      this.updateDeeplink(`learner/${LearnerRoutePaths.Catalogue}`);
      this.searchResultListComponent.triggerDataChange();
    } else if (this.currentCardListType && this.cardListDetailComponent != null) {
      this.updateDeeplink(`learner/${LearnerRoutePaths.Catalogue}/${this.currentCardListType}`);
      this.cardListDetailComponent.triggerDataChange();
    } else {
      this.backToDefaultPage();
    }
  }

  public getSearchCoursesListTitleHtml(): string {
    const translatedText = this.moduleFacadeService.translator.translate('Results for');
    return `<div>${translatedText} <strong>${Utils.escapeHtml(this.previousSearchText)}</strong></div>`;
  }

  public get sectionWidget(): typeof SECTION_WIDGET {
    return SECTION_WIDGET;
  }

  public get showDefaultPage(): boolean {
    return !this.showingCardList && !this.showingCourseSearchResult;
  }

  private loadSuggestedLearningItems(showSpinner: boolean = true): void {
    this.catalogDataService
      .getSuggestedCourses(10, 0, showSpinner)
      .pipe(this.untilDestroy())
      .subscribe(result => {
        this.suggestedLearningItems = result.items;
        this.suggestedLearningItemsTotalCount = result.total;
      });
  }

  private loadOrganisationSuggestedLearningItems(showSpinner: boolean = true): void {
    this.courseDataService
      .getOrganisationSuggestedCourses(10, 0, showSpinner)
      .pipe(this.untilDestroy())
      .subscribe(result => {
        this.organisationSuggestedLearningItems = result.items;
        this.organisationSuggestedLearningItemsTotalCount = result.total;
      });
  }

  private loadAddedLearningItems(showSpinner: boolean = true): void {
    this.catalogDataService
      .getNewlyAddedCourses(showSpinner)
      .pipe(this.untilDestroy())
      .subscribe(result => {
        this.newlyAddedLearningItems = result.items;
        this.newlyAddedLearningItemsTotalCount = result.total;
      });
  }

  private loadSharedToMe(): void {
    this.catalogDataService
      .getSharedToMe()
      .pipe(this.untilDestroy())
      .subscribe(result => {
        this.sharedToMeItems = result.items;
        this.sharedToMeItemsTotalCount = result.total;
      });
  }

  private updateDeeplinkWithParameterInNavigation(): void {
    this.navigateData = this.getNavigateData();
    if (this.navigateData == null) {
      return;
    }

    if (this.navigateData.catalogueType) {
      this.showCardList(this.navigateData.catalogueType);
    }

    if (this.navigateData.filterCriteria) {
      const filterCriteria = this.navigateData.filterCriteria;
      this.filteredByFilterCriteria(filterCriteria);
    }
  }

  private filteredByFilterCriteria(filterCriteria: INavigationFilterCriteria): void {
    this.catalogDataService.clearFilter();
    if (filterCriteria) {
      this.catalogDataService.model.serviceSchemeIds = [filterCriteria.serviceScheme];
      this.catalogDataService.model.subjectAreaIds = [filterCriteria.subjectArea];
    }
    this.filterNumber = this.catalogDataService.model.count;
    this.searchText = '';
    this.previousSearchText = '';
    this.showingCourseSearchResult = true;
    this.catalogDataService.applyFilter();
    if (this.searchResultListComponent !== undefined) {
      this.searchResultListComponent.triggerDataChange(true);
    }
  }

  private backToDefaultPage(): void {
    this.currentCourseId = null;
    this.currentLearningItem = null;
    this.showingCardList = false;
    this.currentCardListType = null;
    this.cardListCallback = null;
    this.cardListTitle = null;
    this.showingCatalogueSearch = true;
    this.moduleFacadeService.navigationService.navigateTo(LearnerRoutePaths.Catalogue);
    this.updateDeeplink(`learner/${LearnerRoutePaths.Catalogue}`);
    this.loadDataDefaultPage();
  }

  private loadDataDefaultPage(): void {
    if (!this.showDefaultPage) {
      return;
    }
    this.loadSuggestedLearningItems();
    this.loadOrganisationSuggestedLearningItems();
    this.loadAddedLearningItems();
    this.loadSharedToMe();
  }
}

type CardListCallbackType = (
  maxResultCount: number,
  skipCount: number
) => Observable<{
  total: number;
  items: ILearningItemModel[];
}>;

type CATALOGUE_SEARCH_TYPE = {
  name: string;
  value: CatalogResourceType;
  total: number;
};
