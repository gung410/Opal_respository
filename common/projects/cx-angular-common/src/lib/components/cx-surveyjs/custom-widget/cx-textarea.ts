export const cxTextarea = {
  name: 'cxteatarea',
  title: 'Textarea with counter',
  iconName: '',
  widgetIsLoaded() {
    return typeof $ === 'function';
  },
  isFit(question) {
    return (
      question.getType() === 'comment'
    );
  },
  activatedByChanged() {
  },
  isDefaultRender: true,
  afterRender(question, textareaElement) {
    if (!textareaElement || !question) { return; }

    const textAreaJquery = $(textareaElement);
    // Get current max length config then remove current max length on textarea
    const maxLength: number = +textAreaJquery.attr('maxlength');
    if (!(maxLength > 0)) { return; }
    textAreaJquery.removeAttr('maxlength');

    const textAreaInvalidClass = 'cx-textarea-invalid';
    const counterInvalidClass = 'counter-invalid';
    const counterId = 'text-area-counter-' + new Date().getTime();
    const textAreaLength = textareaElement.value ? textareaElement.value.length : 0;

    // Insert counter after textarea
    const counterHTML = `<div id="${counterId}" class="cx-textarea-counter">${textAreaLength}/${maxLength}</div>`;
    textAreaJquery.after(counterHTML);
    const counterJQuery = $('#' + counterId);

    $(textareaElement).on('change keyup paste', () => {
      const length = textareaElement.value ? textareaElement.value.length : 0;
      const valid = length <= maxLength;
      const counterValue = `${length}/${maxLength}`;
      counterJQuery.text(counterValue);

      const hasClassInvalid = counterJQuery.hasClass(counterInvalidClass);
      if (valid && hasClassInvalid) {
        counterJQuery.removeClass(counterInvalidClass);
        textAreaJquery.removeClass(textAreaInvalidClass);
      } else if (!valid && !hasClassInvalid) {
        counterJQuery.addClass(counterInvalidClass);
        textAreaJquery.addClass(textAreaInvalidClass);
      }
    });
  }
};

