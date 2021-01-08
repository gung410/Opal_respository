import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppConstant } from 'app/shared/app.constant';
@Component({
  selector: 'step-submitted',
  templateUrl: './step-submitted.component.html',
  styleUrls: ['./step-submitted.component.scss'],
})
export class StepSubmittedComponent implements OnInit {
  constructor(private router: Router) {}

  ngOnInit(): void {}

  onOpenPDPlan(): void {
    const route = `/${AppConstant.mobileUrl.pdPlanner}/${AppConstant.mobileUrl.plannedActivities}`;
    this.router.navigate([route]);
  }
}
