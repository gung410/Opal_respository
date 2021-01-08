import * as Survey from 'survey-angular';

export class CxSurveyStylesManager {
  public static applyTheme(): void {
    Survey.StylesManager.applyTheme('bootstrap');
    Survey.defaultBootstrapCss.navigationButton = 'cx-btn';
    Survey.defaultBootstrapCss.matrixdynamic.button = 'cx-btn';
    Survey.defaultBootstrapCss.paneldynamic.button = 'cx-btn';
    Survey.defaultBootstrapCss.container = 'cx-surveyjs-container';
    Survey.defaultBootstrapCss.pageTitle = 'cx-surveyjs-page-title';
    Survey.defaultBootstrapCss.question.content = 'cx-question-content';
    this.applyModernCssClass();
  }

  private static applyModernCssClass(): void {
    Object.assign(Survey.defaultBootstrapCss.checkbox, this.buildCheckboxCss());
    Object.assign(Survey.defaultBootstrapCss.radiogroup, this.buildRadioGroupCss());
    Object.assign(Survey.defaultBootstrapCss.matrix, this.buildMatrixCss());
  }

  private static buildCheckboxCss(): object {
    return {
      root: 'sv-selectbase',
      item: 'sv-item sv-checkbox sv-selectbase__item',
      itemDisabled: 'sv-item--disabled sv-checkbox--disabled',
      itemChecked: 'sv-checkbox--checked',
      itemHover: 'sv-checkbox--allowhover',
      itemInline: 'sv-selectbase__item--inline',
      label: 'sv-selectbase__label',
      labelChecked: '',
      itemControl: 'sv-visuallyhidden sv-item__control',
      itemDecorator: 'sv-item__svg sv-checkbox__svg',
      controlLabel: 'sv-item__control-label',
      materialDecorator: 'sv-item__decorator sv-checkbox__decorator',
      other: 'sv-comment sv-question__other',
      column: 'sv-selectbase__column'
    };
  }

  private static buildRadioGroupCss(): object {
    return {
      root: 'sv-selectbase',
      item: 'sv-item sv-radio sv-selectbase__item',
      itemInline: 'sv-selectbase__item--inline',
      label: 'sv-selectbase__label',
      labelChecked: '',
      itemDisabled: 'sv-item--disabled sv-radio--disabled',
      itemChecked: 'sv-radio--checked',
      itemHover: 'sv-radio--allowhover',
      itemControl: 'sv-visuallyhidden sv-item__control',
      itemDecorator: 'sv-item__svg sv-radio__svg',
      controlLabel: 'sv-item__control-label',
      materialDecorator: 'sv-item__decorator sv-radio__decorator',
      other: 'sv-comment sv-question__other',
      clearButton: 'sv-btn sv-selectbase__clear-btn',
      column: 'sv-selectbase__column'
    };
  }

  private static buildMatrixCss(): object {
    return {
      tableWrapper: 'sv-matrix',
      root: 'table sv-table',
      cell: 'sv-table__cell sv-matrix__cell',
      headerCell: 'sv-table__cell sv-table__cell--header',
      label: 'sv-item sv-radio sv-matrix__label',
      itemValue: 'sv-visuallyhidden sv-item__control sv-radio__control',
      itemChecked: 'sv-radio--checked',
      itemDisabled: 'sv-item--disabled sv-radio--disabled',
      itemHover: 'sv-radio--allowhover',
      materialDecorator: 'sv-item__decorator sv-radio__decorator',
      itemDecorator: 'sv-item__svg sv-radio__svg',
      cellText: 'sv-matrix__text',
      cellTextSelected: 'sv-matrix__text--сhecked',
      cellTextDisabled: 'sv-matrix__text--disabled'
    };
  }
}
