import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { PDOpportunityDTO } from 'app-models/mpj/pdo-action-item.model';
import { PDCatalogCourseModel } from 'app/individual-development/models/opportunity.model';

@Component({
  selector: 'catalogue-course-picker',
  templateUrl: './catalogue-course-picker.component.html',
  styleUrls: ['./catalogue-course-picker.component.scss'],
})
export class CatalogueCoursePickerComponent implements OnInit {
  @Input() dialogTitle: string = 'Add PD Opportunity';
  @Input() addText: string = 'Add to plan';
  @Input() selectedCourseIds: string[];
  @Input() tagIds: string[];
  @Input() personnelGroupsIdsForExternalPDOUsage: string[];
  @Input() enableBookmark: boolean = false;
  @Input() allowPickMultiplePDO: boolean = false;
  @Input() pickCourseMode: boolean = false;
  @Input() showLoadMoreButton: boolean = true;
  @Input() allowAddExternalPDOPermission: boolean = true;

  @Output() back: EventEmitter<void> = new EventEmitter();
  @Output()
  addToPlan: EventEmitter<PDCatalogCourseModel> = new EventEmitter<PDCatalogCourseModel>();
  @Output()
  addExternalPDO: EventEmitter<PDOpportunityDTO> = new EventEmitter<PDOpportunityDTO>();

  constructor() {}

  ngOnInit(): void {}

  onClickBack(): void {
    this.back.emit();
  }
}
