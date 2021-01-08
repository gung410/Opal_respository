import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CxCommonModule } from '@conexus/cx-angular-common';
import { TranslateModule } from '@ngx-translate/core';
import { CxCommentComponent } from './cx-comment.component';

@NgModule({
  declarations: [CxCommentComponent],
  exports: [CxCommentComponent],
  imports: [CommonModule, TranslateModule, CxCommonModule, FormsModule],
})
export class CxCommentModule {}
