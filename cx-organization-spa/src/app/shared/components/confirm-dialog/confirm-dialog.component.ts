import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

@Component({
  selector: 'app-confirm-dialog',
  templateUrl: './confirm-dialog.component.html',
  styleUrls: ['./confirm-dialog.component.scss']
})
export class ConfirmDialogComponent implements OnInit {
  title: string;
  content: string;
  confirmButtonText: string;
  cancelButtonText: string;

  get convertedContent(): SafeHtml {
    return this.domSanitizer.bypassSecurityTrustHtml(this.content);
  }

  constructor(
    private domSanitizer: DomSanitizer,
    public dialogRef: MatDialogRef<ConfirmDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ConfirmDialogModel
  ) {
    this.title = data.title;
    this.content = data.content;
    this.confirmButtonText = data.confirmButtonText || 'Yes';
    this.cancelButtonText = data.cancelButtonText || 'No';
  }

  ngOnInit() {}

  onConfirm(): void {
    this.dialogRef.close(true);
  }

  onDismiss(): void {
    this.dialogRef.close(false);
  }
}

/**
 * Class to represent confirm dialog model.
 *
 * It has been kept here to keep it as part of shared component.
 */
// tslint:disable-next-line:max-classes-per-file
export class ConfirmDialogModel {
  title: string;
  content: string;
  cancelButtonText?: string;
  confirmButtonText?: string;
  constructor(data?: ConfirmDialogModel) {
    if (!data) {
      return;
    }

    this.title = data.title;
    this.content = data.content;
    this.confirmButtonText = data.confirmButtonText;
    this.cancelButtonText = data.cancelButtonText;
  }
}
