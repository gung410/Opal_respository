import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewEncapsulation,
} from '@angular/core';
import {
  PDOpportunityAnswerDTO,
  PDOpportunityDTO,
  PDOSource,
} from 'app-models/mpj/pdo-action-item.model';

@Component({
  selector: 'pdo-long-card',
  templateUrl: './pdo-long-card.component.html',
  styleUrls: ['./pdo-long-card.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class PDOLongCardComponent implements OnInit {
  @Input('pdoAnswer')
  set pdoAnswer(pdoAnswer: PDOpportunityAnswerDTO) {
    this.processPDOModel(pdoAnswer);
  }
  @Input() allowRemove: boolean = false;
  @Output() clickedItem: EventEmitter<string> = new EventEmitter();
  @Output() clickedRemove: EventEmitter<string> = new EventEmitter();

  pdoDTO: PDOpportunityDTO;

  constructor() {}

  ngOnInit(): void {}

  onClickItem(): void {
    this.clickedItem.emit(this.pdoDTO.uri);
  }

  onRemoveItem(): void {
    this.clickedRemove.emit(this.pdoDTO.uri);
  }

  get isCoursePadPDO(): boolean {
    return this.pdoDTO.source === PDOSource.CoursePadPDO;
  }

  get isExternalPDO(): boolean {
    return this.pdoDTO.source === PDOSource.CustomPDO;
  }

  get imgPath(): string {
    return this.pdoDTO.thumbnailUrl;
  }

  private processPDOModel(pdoAnswerDTO: PDOpportunityAnswerDTO): void {
    if (!pdoAnswerDTO || !pdoAnswerDTO.learningOpportunity) {
      return;
    }

    this.pdoDTO = pdoAnswerDTO.learningOpportunity;
  }
}
