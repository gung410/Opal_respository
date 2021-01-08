import * as Survey from 'survey-angular';

export const cxInputmask = {
  name: 'cxinputmask',
  title: 'Text with mask',
  iconName: '',
  widgetIsLoaded() {
    return typeof $ === 'function';
  },
  isFit(question) {
    return (
      question.getType() === 'text' && question.inputType === 'cxinputmask'
    );
  },
  activatedByChanged() {
    Survey.JsonObject.metaData.addClass('cxinputmask', [], null, 'text');
    Survey.JsonObject.metaData.addProperty('text', {
      name: 'config',
      default: null
    });
    Survey.JsonObject.metaData.addProperty('text', {
      name: 'datePickerConfig',
      default: null
    });
    Survey.JsonObject.metaData.addProperty('text', {
      name: 'datePickerFormatLocate',
      default: 'en-GB'
    });
    Survey.JsonObject.metaData.addProperty('text', {
      name: 'datePickerInvalidMinDateText',
      default: 'Provided date is not in valid range. Min date is: '
    });
    Survey.JsonObject.metaData.addProperty('text', {
      name: 'datePickerInvalidMinDateText',
      default: 'Provided date is not in valid range. Max date is: '
    });
  },
  isDefaultRender: true,
  afterRender(question, inputElement) {
    if (!inputElement || !question) {
      return;
    }

    // Use current value to check backup value of question, use to check need to update value of question when value change
    let currentValue;
    // Init inputmask by jquery with question's config
    if (question.config) {
      const $el: any = $(inputElement);
      const alias = question.config.alias;

      // Set placeholder
      if (question.config.placeholder) {
        $el.attr('placeholder', question.config.placeholder);
      }

      // Set inputmask
      $el.inputmask(alias, question.config ? question.config : {});

      // Set datepicker if that is date
      if (question.config.alias === 'datetime') {
        const defaultDatePickerConfig = {
          changeMonth: true,
          changeYear: true,
          dateFormat: 'dd/mm/yy'
        };

        const datePickerConfig = question.datePickerConfig || defaultDatePickerConfig;
        $el.datepicker(datePickerConfig);

        setTimeout(() => {
          const minDate = _getMinMaxDate($el, 'min');
          const maxDate = _getMinMaxDate($el, 'max');
          $el.change(() => { _onCxInputDateChanged($el, question, minDate, maxDate); });
        });
      }
    }

    const onValueChangedCallback = () => {
      const newValue = question.value;

      // Set value for el.value if needed
      if (inputElement.value !== '' + newValue) {
        inputElement.value = '' + newValue;
      }

      // Check current question data type to parse to correctly type (text, number)
      if (currentValue !== question.value) {
        const isNumber = question.config && question.config.alias &&
          (question.config.alias === 'decimal' || question.config.alias === 'numeric');
        currentValue = isNumber && newValue !== undefined ? +newValue : newValue;
        question.value = currentValue;
      }
    };

    // On base text question it already has onValueChangedCallback func
    // we handle it here to convert question value to correctly type if needed
    question.valueChangedCallback = onValueChangedCallback;
    onValueChangedCallback();
  },
  willUnmount(question, el: any) {
    const $el: any = $(el).find('input');
    $el.inputmask('remove');
  }
};

function _onCxInputDateChanged($el, question, minDate, maxDate) {
  if (!$el) { return; }

  const currentDate = $el.datepicker('getDate');
  const formatLocate = question.datePickerFormatLocate;

  if (!!minDate && !!currentDate  && currentDate.setHours(0, 0, 0, 0) < minDate.setHours(0, 0, 0, 0)) {
    const datePickerInvalidMinDateText = question.datePickerInvalidMinDateText;
    const invalidText = datePickerInvalidMinDateText + minDate.toLocaleDateString(formatLocate);
    _onInvalidDate($el, question, invalidText);
    return;
  }

  if (!!maxDate && !!currentDate && currentDate.setHours(0, 0, 0, 0) > maxDate.setHours(0, 0, 0, 0)) {
    const datePickerInvalidMinDateText = question.datePickerInvalidMinDateText;
    const invalidText = datePickerInvalidMinDateText + maxDate.toLocaleDateString(formatLocate);
    _onInvalidDate($el, question, invalidText);
    return;
  }
}

function _onInvalidDate($el, question: any, invalidText: string) {
  $el.datepicker('setDate', undefined);
  question.clearValue();
  question.addError(new Survey.SurveyError(invalidText));
  question.survey.focusOnFirstError = true;
}

function _getMinMaxDate($el, type: 'min' | 'max') {
  if (!$el || !type) { return; }

  const jqueryObj: any = $;
  const elementContext = $el.context;

  if (!elementContext) { return; }

  const currentDatePickerInstance = $.data(elementContext, 'datepicker');

  if (!currentDatePickerInstance) { return; }

  if (type === 'min' && !currentDatePickerInstance.settings.minDate) {
    return;
  }

  if (type === 'max' && !currentDatePickerInstance.settings.maxDate) {
    return;
  }

  return jqueryObj.datepicker._getMinMaxDate(currentDatePickerInstance, type);
}
