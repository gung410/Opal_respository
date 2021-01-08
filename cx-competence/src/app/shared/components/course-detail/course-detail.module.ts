import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { TranslateModule } from '@ngx-translate/core';
import { CommentService } from 'app-services/comment.service';
import { CxCommentModule } from 'app/individual-development/cx-comment/cx-comment.module';
import { PDOLongCardModule } from '../pdo-long-card/pdo-long-card.module';
import { ClassrunInfoComponent } from './classrun-info/classrun-info.component';
import { CourseDetailComponent } from './course-detail.component';
import { CourseInfoComponent } from './course-info/course-info.component';
import { CourseReviewItemComponent } from './course-review-item/course-review-item.component';
import { CourseReviewComponent } from './course-review/course-review.component';
import { TableOfContentComponent } from './table-of-content/table-of-content.component';

@NgModule({
  imports: [
    CommonModule,
    NgbModule,
    CxCommentModule,
    PDOLongCardModule,
    TranslateModule,
  ],
  declarations: [
    CourseInfoComponent,
    ClassrunInfoComponent,
    CourseDetailComponent,
    CourseReviewComponent,
    CourseReviewItemComponent,
    TableOfContentComponent,
  ],
  providers: [CommentService],
  exports: [
    CourseInfoComponent,
    CourseReviewComponent,
    ClassrunInfoComponent,
    CourseDetailComponent,
    CourseReviewItemComponent,
    TableOfContentComponent,
  ],
})
export class CourseDetailModule {}
