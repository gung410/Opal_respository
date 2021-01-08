import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'pd-planner-page-header',
  templateUrl: './pd-planner-page-header.component.html',
  styleUrls: ['./pd-planner-page-header.component.scss'],
})
export class PDPlannerPageHeaderComponent implements OnInit {
  @Input() backRoute: string;
  @Input() title: string;
  @Input() isPopup = false;
  @Output() closePopup = new EventEmitter<any>();
  constructor(private router: Router) {}

  ngOnInit() {}

  goBack() {
    if (this.isPopup) {
      this.closePopup.emit();
    }
    if (this.backRoute) {
      this.router.navigate([this.backRoute]);
    }
  }
}
