import { CommonModule } from '@angular/common';
import { DocumentViewerComponent } from './document-viewer.component';
import { FunctionModule } from '../function.module';
import { NgModule } from '@angular/core';

@NgModule({
  imports: [CommonModule, FunctionModule],
  declarations: [DocumentViewerComponent],
  exports: [DocumentViewerComponent]
})
export class DocumentViewerModule {}
