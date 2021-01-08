import {
  Component,
  OnInit,
  Input,
  ViewEncapsulation,
  Output,
  EventEmitter,
  TemplateRef,
  ChangeDetectionStrategy,
  ElementRef,
  ChangeDetectorRef,
  OnChanges,
  SimpleChanges,
  Renderer2
} from "@angular/core";
import {
  CxTableContainersIcon
} from "./cx-table-containers.model";
import * as _ from "lodash";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { MediaObserver } from "@angular/flex-layout";
import { CxObjectUtil } from "../../../utils/object.util";
import { CxAbstractTableComponent } from '../cx-abstract-table.component';
import { CxColumnSortType } from '../cx-table.model';
import { CxAnimations } from '../../../constants/cx-animation.constant';

@Component({
  selector: "cx-table-containers",
  templateUrl: "./cx-table-containers.component.html",
  styleUrls: ["../cx-abstract-table.component.scss",
    "./cx-table-containers.component.scss"],
  encapsulation: ViewEncapsulation.None,
  animations: [CxAnimations.smoothAppendRemove],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CxTableContainersComponent<TItem, TContainer>
  extends CxAbstractTableComponent<TItem>
  implements OnInit, OnChanges {
  @Input() public get containers(): TContainer[] {
    return this._containers;
  }
  @Input() containerChildrenCountRoute: string;
  @Input() containerTemplate: TemplateRef<TContainer>;
  @Input() noContainerText = 'No item';
  @Input() noItemText = 'No Item';
  @Input() noRecordText = 'No Item';
  @Input() icon: CxTableContainersIcon = new CxTableContainersIcon();
  @Input() isDisplayContainer = true;
  @Output() moveItem = new EventEmitter<TItem>();
  @Output() containerClick = new EventEmitter<TContainer>();

  public set containers(containers: TContainer[]) {
    this._containers = containers;
  }

  private _containers: TContainer[] = [];

  private containersUnsorted: TContainer[] = [];
  constructor(
    public changeDetectorRef: ChangeDetectorRef,
    public elementRef: ElementRef,
    public media: MediaObserver,
    public ngbModal: NgbModal,
    renderer: Renderer2
  ) {
    super(changeDetectorRef, elementRef, media, ngbModal, renderer);
  }

  ngOnInit() {
    super.ngOnInit();
  }

  ngOnChanges(changes: SimpleChanges) {
    let isChanged = false;
    if (
      changes.containers &&
      changes.containers.currentValue !== changes.containers.previousValue
    ) {
      this.containersUnsorted = this.containers;
      isChanged = true;
    }

    if (
      changes.items &&
      changes.items.currentValue !== changes.items.previousValue
    ) {
      this.itemsUnsorted = this.items;
      this.updateSelectedItemsMapBaseOnItemsList(this.items);
      isChanged = true;
    }

    if (isChanged && !this.isDataLazyLoad) {
      this.sortDataByHeader(this.currentFieldSort, this.currentSortType);
    }
  }

  public sortByFieldClick(fieldSort: string, currentSort: CxColumnSortType) {
    this.currentFieldSort = fieldSort;
    this.currentSortType = currentSort;
    this.sortDataByHeader(fieldSort, currentSort);
  }

  protected sortDataByHeader(fieldSort: string, currentSortType: CxColumnSortType) {
    const nextSortType = this.changeSortInHeaders(
      this.headers,
      fieldSort,
      currentSortType
    );

    if (this.isDataLazyLoad) {
      this.sortTypeChange.emit({ fieldSort, sortType: nextSortType });
      return;
    } else {
      if (nextSortType === CxColumnSortType.UNSORTED) {
        this.containers = this.containersUnsorted;
        this.items = this.itemsUnsorted;
        return;
      }

      const sort = nextSortType === CxColumnSortType.ASCENDING;
      const containerData = this.containersUnsorted;
      if (!fieldSort) {
        return;
      }
      const listFieldSort = fieldSort.split(';');
      if (listFieldSort.length === 0) {
        return;
      }

      const itemData = this.itemsUnsorted;
      this.items = sort
        ? _.orderBy(itemData, [listFieldSort[1]], [sort])
        : _.orderBy(itemData, [listFieldSort[1]], [sort]).reverse();

      this.containers = sort
        ? _.orderBy(containerData, [listFieldSort[0]])
        : _.orderBy(containerData, [listFieldSort[0]]).reverse();
    }
  }

  onMoveItemClicked(item: TItem) {
    this.moveItem.emit(item);
  }

  getContainerIcon(container: TContainer) {
    if (!this.containerChildrenCountRoute) {
      return this.icon.containerHasChildClass;
    }
    const icon =
      CxObjectUtil.getPropertyValue(
        container,
        this.containerChildrenCountRoute
      ) === 0
        ? this.icon.containerEmptyClass
        : this.icon.containerHasChildClass;
    return icon;
  }

  public containerInfoClick(container: any) {
    this.containerClick.emit(container);
  }
}
