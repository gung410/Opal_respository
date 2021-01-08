import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output, ViewChild, ViewEncapsulation } from '@angular/core';
import { IQuestionGroupSearchRequest, QuestionBankApiService, QuestionGroup } from '@opal20/domain-api';

import { Observable } from 'rxjs';
import { OpalSelectComponent } from '@opal20/common-components';
import { map } from 'rxjs/operators';

export enum QuestionGroupDisplayMode {
  DropDownList = 'DropDownList',
  ComboBox = 'ComboBox'
}

@Component({
  selector: 'question-bank-group',
  templateUrl: './question-bank-group.component.html',
  encapsulation: ViewEncapsulation.None
})
export class QuestionBankGroupComponent extends BaseComponent {
  @Input() public displayMode: QuestionGroupDisplayMode = QuestionGroupDisplayMode.ComboBox;
  @Input() public questionGroupName: string;
  @Input() public dropdownPosition: string = 'bottom';
  @Input() public isFilterByUsing: boolean = false;
  @Input() public multiple: boolean = false;

  @Output() public questionGroupNameChange: EventEmitter<string> = new EventEmitter<string>();
  @Output() public questionGroupChange: EventEmitter<QuestionGroup[]> = new EventEmitter<QuestionGroup[]>();

  @ViewChild('groupSelect', { static: false }) public groupSelectElm: OpalSelectComponent;

  public questionGroupDisplayMode = QuestionGroupDisplayMode;

  public customQuestionGroupAddedFn: (searchTag: string) => Promise<QuestionGroup>;
  public fetchQuestionGroupsFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<QuestionGroup[]>;

  constructor(public moduleFacadeService: ModuleFacadeService, private questionBankApiService: QuestionBankApiService) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.customQuestionGroupAddedFn = (questionGroupName: string) => {
      this.questionGroupName = questionGroupName;
      return Promise.resolve(
        new QuestionGroup({
          name: questionGroupName,
          id: null
        })
      );
    };

    this.fetchQuestionGroupsFn = (searchText: string, skipCount: number, maxResultCount: number) => {
      const request: IQuestionGroupSearchRequest = {
        name: searchText,
        isFilterByUsing: this.isFilterByUsing,
        pagedInfo: {
          skipCount: skipCount,
          maxResultCount: maxResultCount
        }
      };

      return this.questionBankApiService.searchQuestionGroups(request, false).pipe(map(response => response.items));
    };
  }

  public onQuestionGroupChange(data: QuestionGroup[] | QuestionGroup): void {
    if (data instanceof Array) {
      this.questionGroupChange.emit(data);
    } else {
      this.questionGroupChange.emit([data]);
      if (data && data.name) {
        this.questionGroupNameChange.emit(data.name);
      }
    }
  }
}
