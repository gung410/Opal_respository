import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'alert-dialog',
  templateUrl: './alert-dialog.component.html',
  styleUrls: ['./alert-dialog.component.scss'],
})
export class AlertDialogComponent implements OnInit {
  @Input() message: string;
  @Input() icon: string[] = ['icon-check-mark', 'icon-success'];
  constructor() {}

  ngOnInit() {}
}
