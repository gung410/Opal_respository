import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { noContentWhiteSpaceValidator } from 'app/shared/validators/no-content-white-space-validator';

@Component({
  selector: 'metadata-confirm-dialog',
  templateUrl: './metadata-confirm-dialog.component.html',
  styleUrls: ['./metadata-confirm-dialog.component.scss']
})
export class MetadataConfirmDialogComponent implements OnInit {
  get comment(): string {
    return this._comment;
  }
  set comment(comment: string) {
    if (comment == null) {
      return;
    }

    this._comment = comment;
  }

  get opalTextAreaStyle(): unknown {
    return {
      border: '1x solid #d8dce6',
      boxSizing: 'border-box',
      borderRadius: '5px',
      width: '100%',
      padding: '10px'
    };
  }

  metadataConfirmForm: FormGroup;

  @Input() action: string = '';

  private _comment: string = '';

  constructor(private fb: FormBuilder, public dialogRef: MatDialogRef<any>) {}

  ngOnInit(): void {
    this.createFormBuilderDefinition();
  }

  createFormBuilderDefinition(): void {
    this.metadataConfirmForm = this.fb.group({
      comment: ['', [Validators.required, noContentWhiteSpaceValidator]]
    });
  }

  onCancelClicked(): void {
    this.dialogRef.close({});
  }

  onConfirmClicked(): void {
    this.dialogRef.close({
      comment: this.comment
    });
  }
}
