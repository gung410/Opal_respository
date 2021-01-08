import * as Survey from 'survey-angular';
// Reference https://plnkr.co/edit/fXsLf1R88WxxDFaFEnYx?p=preview

/**
 * People picker using for picking up persons from a dialog.
 */
export const cxPeoplePicker = {
  // the widget name. It should be unique and written in lowcase.
  name: 'cxpeoplepicker',
  // the widget title. It is how it will appear on the toolbox of the SurveyJS Editor/Builder
  title: 'CX People Picker',
  // the name of the icon on the toolbox. We will leave it empty to use the standard one
  iconName: '',
  // If the widgets depends on third-party library(s) then here you may check if this library(s) is loaded
  widgetIsLoaded() {
    return true;
  },
  // SurveyJS library calls this function for every question to check, if it should use this widget instead of default rendering/behavior
  isFit(question) {
    // we return true if the type of question is cxpeoplepicker
    return question.getType() === 'cxpeoplepicker';
  },
  // Use this function to create a new class or add new properties or remove unneeded properties from your widget
  // activatedBy tells how your widget has been activated by: property, type or customType
  activatedByChanged(activatedBy) {
    // We do not need to check acticatedBy parameter, since we will use our widget for customType only
    // We are creating a new class and derived it from text question type. It means
    //  that text model (properties and fuctions) will be available to us
    Survey.JsonObject.metaData.addClass('cxpeoplepicker', [], null, 'text');
    // Add new property(s)
    // For more information go to https://surveyjs.io/Examples/Builder/?id=addproperties#content-docs
    Survey.JsonObject.metaData.addProperties('cxpeoplepicker', [
      {
        name: 'maxRowCount:number',
        default: 999999
      },
      { name: 'emptyText:text', default: 'No items.' },
      { name: 'rowCount:number', default: 0 },
      { name: 'addRowText:text', default: 'Add' },
      { name: 'addRowLocation:text', default: 'top' },
      { name: 'addRowDialogTitle:text', default: 'People Picker' },
      { name: 'numberOfSelectedPeopleTextPrefix:text', default: '' },
      { name: 'numberOfSelectedPeopleText:text', default: ' learner(s)' },
      { name: 'numberOfSelectedPeopleTextSuffix:text', default: ' selected.' },
      { name: 'showDuplicatedWarning:boolean', default: true },
      {
        name: 'successMessage:text',
        default: '{addedCount} {singular/plural} been added successfully.'
      },
      {
        name: 'duplicatedMessage:text',
        default: '{duplicatedCount} {singular/plural} been added previously.'
      }
    ]);
  },
  // If you want to use the default question rendering then set this property to true.
  //  We do not need any default rendering, we will use our our htmlTemplate
  isDefaultRender: false,
  // You should use it if your set the isDefaultRender to false
  htmlTemplate: `
        <div>
            <div data-bind="if: question.addRowLocation === 'top' || question.addRowLocation !== 'bottom'">
                <button type="button"
                    class="cx-btn add-item"
                    data-bind="click: question.onOpenPeoplePickerClicked, text: question.addRowText,
                        visible: question.rowCount < question.maxRowCount && (!question.isReadOnly || question.isReadOnly == false)">
                </button>
            </div>
            <div class="empty-people-list"
              data-bind="if: question.emptyText && (!question.value || !question.rowCount)">
              <div data-bind="html: question.emptyText"></div>
            </div>
            <div class="number-of-selected-people-container"
              data-bind="if: (question.value && question.rowCount > 0)">
              <span data-bind="html: question.numberOfSelectedPeopleTextPrefix"></span>
              <span class="number-of-selected-people cx-link"
                data-bind="click: question.onOpenSelectedPeopleClicked,
                  html: question.rowCount + ' ' + question.numberOfSelectedPeopleText"></span>
              <span data-bind="html: question.numberOfSelectedPeopleTextSuffix"></span>
            </div>
            <div data-bind="if: question.addRowLocation === 'bottom'">
                <button type="button"
                    class="cx-btn add-item"
                    data-bind="click: question.onOpenPeoplePickerClicked, text: question.addRowText,
                        visible: question.rowCount < question.maxRowCount && (!question.isReadOnly || question.isReadOnly == false)">
                </button>
            </div>
        </div>`,
  // The main function, rendering and two-way binding
  afterRender(question, el) {
    // el is our root element in htmlTemplate, is "div" in our case
    // get button and set some properties
    const button = el.getElementsByClassName('add-item')[0];
    question.onOpenPeoplePickerClicked = () => {
      alert(`Not implemented yet! Please override this 'onOpenPeoplePickerClicked' event by your own!`);
    };
    question.onOpenSelectedPeopleClicked = () => {
      alert(`Not implemented yet! Please override this 'onOpenSelectedPeopleClicked' event by your own!`);
    };

    const onValueChangedCallback = () => {
      question.rowCount = question.value ? question.value.length : 0;
    };
    // if the question value changed in the code, for example you have changed it in JavaScript
    question.valueChangedCallback = onValueChangedCallback;
    // set initial value
    onValueChangedCallback();

    const onReadOnlyChangedCallback = () => {
      if (question.isReadOnly) {
        button.setAttribute('disabled', 'disabled');
      } else {
        button.removeAttribute('disabled');
      }
    };
    // if question becomes readonly/enabled add/remove disabled attribute
    question.readOnlyChangedCallback = onReadOnlyChangedCallback;
    // set initial readOnly if needed
    onReadOnlyChangedCallback();
  },
  // Use it to destroy the widget. It is typically needed by jQuery widgets
  willUnmount(question, el) {
  }
};
