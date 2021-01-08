import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgModule, Type } from '@angular/core';

import { CommonModule } from '@angular/common';

const ANGULAR_MODULES: Type<unknown>[] = [CommonModule, FormsModule, ReactiveFormsModule];

@NgModule({
  imports: [...ANGULAR_MODULES],
  exports: [...ANGULAR_MODULES]
})
export class AngularModule {}
