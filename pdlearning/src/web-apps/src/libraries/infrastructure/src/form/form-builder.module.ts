import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { FormBuilderService } from './form-builder.service';
import { NgModule } from '@angular/core';

@NgModule({
  exports: [FormsModule, ReactiveFormsModule],
  providers: [FormBuilderService]
})
export class FormBuilderModule {}
