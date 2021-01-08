import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CxSelectComponent } from './cx-select.component';
import { NgSelectModule } from '@ng-select/ng-select';
import { FormsModule } from '@angular/forms';

@NgModule({
  imports: [CommonModule, FormsModule, NgSelectModule],
  exports: [CxSelectComponent],
  declarations: [CxSelectComponent],
})
export class CxSelectModule {}
