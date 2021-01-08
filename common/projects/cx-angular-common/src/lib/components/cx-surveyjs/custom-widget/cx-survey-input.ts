import * as Survey from 'survey-angular';
import { SurveyModel } from 'survey-angular';
import { handleErrorWhenLoadChoicesByUrl } from '../surveyjs-function';
import { CxSurveyInputHelper } from './cx-survey-input.helper';

// Reference https://plnkr.co/edit/fXsLf1R88WxxDFaFEnYx?p=preview
export const cxSurveyInput = {
  // the widget name. It should be unique and written in lowcase.
  name: 'cxsurveyinput',
  // the widget title. It is how it will appear on the toolbox of the SurveyJS Editor/Builder
  title: 'CX Survey Input',
  // the name of the icon on the toolbox. We will leave it empty to use the standard one
  iconName: '',
  // If the widgets depends on third-party library(s) then here you may check if this library(s) is loaded
  widgetIsLoaded() {
    return true;
  },
  // SurveyJS library calls this function for every question to check, if it should use this widget instead of default rendering/behavior
  isFit(question) {
    // we return true if the type of question is cxsurveyinput
    return question.getType() === 'cxsurveyinput';
  },
  // Use this function to create a new class or add new properties or remove unneeded properties from your widget
  // activatedBy tells how your widget has been activated by: property, type or customType
  activatedByChanged(activatedBy) {
    // We do not need to check acticatedBy parameter, since we will use our widget for customType only
    // We are creating a new class and derived it from text question type. It means
    //  that text model (properties and fuctions) will be available to us
    Survey.JsonObject.metaData.addClass('cxsurveyinput', [], null, 'text');
    // Add new property(s)
    // For more information go to https://surveyjs.io/Examples/Builder/?id=addproperties#content-docs
    Survey.JsonObject.metaData.addProperties('cxsurveyinput', [
      {
        name: 'maxRowCount:number',
        default: Survey.settings ? Survey.settings.matrixMaximumRowCount : 100
      },
      { name: 'surveyJson', default: {} },
      { name: 'selectedItemDisplayType', default: 'list' },
      { name: 'selectedItemText', default: '' },
      { name: 'fieldToGetValue:string', default: '' },
      { name: 'fieldToDisplay:string', default: '' },
      { name: 'keyName:string', default: '' },
      { name: 'keyDuplicationError:string', default: '' },
      { name: 'rowCount:number', default: 0 },
      { name: 'addRowText:string', default: 'Add' },
      { name: 'addRowLocation:string', default: 'top' },
      { name: 'removeRowText:string', default: 'Remove' },
      { name: 'confirmDeleteText:string', default: '' },
      { name: 'popupHeaderText:string', default: '' },
      { name: 'popupCancelButtonText:string', default: 'Cancel' },
      { name: 'popupCompleteButtonText:string', default: 'OK' }
    ]);
  },
  // If you want to use the default question rendering then set this property to true.
  //  We do not need any default rendering, we will use our our htmlTemplate
  isDefaultRender: false,
  // You should use it if your set the isDefaultRender to false
  htmlTemplate: `
        <div class="cx-survey-input-container">
            <div data-bind="if: question.addRowLocation === 'top' || question.addRowLocation !== 'bottom'">
                <button type="button"
                    class="cx-btn add-item"
                    data-bind="text: question.addRowText,
                        visible: question.rowCount < question.maxRowCount && (!question.isReadOnly || question.isReadOnly == false)">
                </button>
            </div>

            <div class="cx-survey-input-selected-label" data-bind="
              if: question.selectedItemDisplayType !== '' && question.koValue() && question.koValue().length > 0">
              <h5 data-bind="html: question.selectedItemText"></h5>
            </div>

            <div class="cx-survey-input" data-bind="if: question.selectedItemDisplayType !== 'tagbox'">
              <div class="result" data-bind="foreach: { data: question.koValue(), as : 'valueObject' }">
                <div class="row">
                  <div class="col-sm-12 col-md-10" data-bind="text: valueObject.getValueBySelector(question.fieldToDisplay)"></div>
                  <div class="col-sm-12 col-md-2">
                      <button type="button" class="cx-btn remove-item"
                          data-bind="click: question.onRemoveItem, html: question.removeRowText,
                              visible: question.isReadOnly === false">
                      </button>
                  </div>
                </div>
              </div>
            </div>

            <div data-bind="if: question.selectedItemDisplayType === 'tagbox' && question.koValue() && question.koValue().length > 0">
                <div class="cx-survey-input-tagbox" data-bind="foreach: { data: question.koValue(), as : 'valueObject' }">
                  <div class="tag-item">
                    <div class="tag-item__text" data-bind="text: valueObject.getValueBySelector(question.fieldToDisplay)"></div>
                    <div class="tag-item__remove" data-bind="click: question.onRemoveItem,
                      visible: question.isReadOnly === false">
                      <i class="icon icon-close"></i>
                    </div>
                  </div>
                </div>
            </div>

            <div data-bind="if: question.addRowLocation === 'bottom'">
                <button type="button"
                    class="cx-btn add-item"
                    data-bind="text: question.addRowText,
                        visible: question.rowCount < question.maxRowCount && (!question.isReadOnly || question.isReadOnly == false)">
                </button>
            </div>
        </div>`,
  // The main function, rendering and two-way binding
  afterRender(question, el) {
    // el is our root element in htmlTemplate, is "div" in our case
    // get button and set some properties
    const button = el.getElementsByClassName('add-item')[0];
    let uniqueQuestionId = question.questionId
      ? question.questionId
      : `${question.name}-${new Date().getTime()}`;
    // We need remove the dots on id, because the data-toggle can't toggle modal if id contain dot symbol
    uniqueQuestionId = uniqueQuestionId.replace(/\./g, '');
    question.rowCount = question.value ? question.value.length : 0;
    question.fieldToGetValue = question.fieldToGetValue
      ? question.fieldToGetValue
      : question.name;

    button.onclick = () => {
      openSurvey(button, question, uniqueQuestionId);
    };
    const openSurvey = (
      triggeredButton: any,
      questionObj: any,
      questionId: string
    ) => {
      const surveyContainerId = buildSurveyContainerId(questionId);
      // Open modal.
      openModal(triggeredButton, questionObj, questionId, surveyContainerId);

      // When close modal the class 'modal-open' will be removed on the body element
      // But it incorrect if there still exist parent modal so we need to check and add that class again
      $(`#${buildModalId(uniqueQuestionId)}`).one('hidden.bs.modal', () => {
        const modalShowTotal = $('.modal.show').length;
        if (modalShowTotal > 0) {
          $('body').addClass('modal-open');
        }
      });

      // Init survey model.
      const json =
        typeof questionObj.surveyJson === 'string'
          ? JSON.parse(questionObj.surveyJson)
          : questionObj.surveyJson;
      const survey = new Survey.Model(json);
      survey.onComplete.add(onCompleteSurvey);
      survey.onLoadChoicesFromServer.add(handleErrorWhenLoadChoicesByUrl);

      // Clone the survey js variable from the parent survey form into the survey form in the popup.
      const parentFormVariables = questionObj.survey.variablesHash;
      for (const name in parentFormVariables) {
        if (parentFormVariables.hasOwnProperty(name)) {
          survey.setVariable(name, parentFormVariables[name]);
        }
      }

      survey.setVariable('parentObject', questionObj.survey.data);

      // Store the choice as a whole object instead of only one field of the object.
      (survey
        .getAllQuestions()
        .filter(
          (ques: any) => ques.choicesByUrl && !ques.choicesByUrl.valueName
        ) as Survey.Question[]).forEach(ques => {
        ques.choicesByUrl.getItemValueCallback = itemValue => itemValue;
      });

      // Render the survey js.
      Survey.SurveyNG.render(surveyContainerId, { model: survey });
      // Add cancel button into the popup.
      addCancelButton(surveyContainerId, question);
    };
    const onCompleteSurvey = (survey: SurveyModel) => {
      // Close the modal.
      $(`#${buildModalId(uniqueQuestionId)}`).modal('hide');

      // Bind new items into the value of the question.
      addNewItems(
        question,
        CxSurveyInputHelper.getDeepValue(survey.data, `${question.fieldToGetValue}`)
      );

      CxSurveyInputHelper.onValueChanged(question);
    };
    const addNewItems = (ques: any, newValue: any) => {
      if (!newValue) {
        return;
      }
      // Check to push the all items into the question.value if it's an array; otherwise, just push single item.
      if (newValue.push) {
        newValue.forEach(element => {
          pushNewQuestionValueItem(ques, element);
        });
      } else {
        pushNewQuestionValueItem(ques, newValue);
      }
    };
    const pushNewQuestionValueItem = (ques: any, newValue: any) => {
      // Init the value as an array if it's not.
      if (!ques.value || !ques.value.push) {
        ques.value = [];
      }
      // Only add new item if the number of the array is not greater than the defined maxRowCount.
      if (ques.value.length < ques.maxRowCount) {
        // Check duplication.
        if (ques.value.find(x => CxSurveyInputHelper.isTheSameItems(x, newValue, ques.keyName))) {
          alert(
            `Duplication of '${CxSurveyInputHelper.getDisplayText(newValue, question)}' has been ignored.`
          );
          return;
        }
        ques.value = ques.value.concat(newValue);
      }
    };
    const openModal = (
      triggeredButton: any,
      questionObj: any,
      questionId: string,
      surveyContainerId: string
    ) => {
      const modalId = buildModalId(questionId);
      if (triggeredButton.getAttribute('data-target') !== modalId) {
        triggeredButton.setAttribute('data-toggle', 'modal');
        triggeredButton.setAttribute('data-target', `#${modalId}`);
      }

      // Check to ensure the modal has not been rendered/appended into the DOM.
      if (!questionObj.renderedSurvey || questionObj.renderedSurvey === false) {

        const modalHeader = `
                    <div class="modal-header">
                        <h3 class="modal-title">${questionObj.popupHeaderText}</h3>
                        <div class="modal-close-button" data-dismiss="modal">
                          <i class="material-icons close-icon"></i>
                        </div>
                    </div>`;
        const modalHtml = `
                    <cx-surveyjs class="modal cx-survey-input-modal fade" role="dialog" id="${modalId}">
                        <div class="modal-dialog" role="document">
                            <div class="modal-content">
                                <div class="cx-dialog-template">
                                  ${
                                    questionObj.popupHeaderText ? modalHeader : ''
                                  }
                                  <div class="modal-body">
                                      <div class="surveyContainer" id="${surveyContainerId}"></div>
                                  </div>
                              </div>
                            </div>
                        </div>
                    </cx-surveyjs>`;
        const modalContainerId = modalId + '-container';
        const modalContainer = document.createElement('div');
        modalContainer.id = modalContainerId;
        modalContainer.innerHTML = modalHtml;
        document.body.appendChild(modalContainer);
        // Mark that the survey is rendered.
        questionObj.renderedSurvey = true;
        questionObj.questionId = questionId;
        questionObj.modalContainerId = modalContainerId; // Use when destroy
      }
    };
    const buildModalId = (questionId: string): string => {
      return `modal${questionId}`;
    };
    const buildSurveyContainerId = (questionId: string): string => {
      return `surveyContainer${questionId}`;
    };
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
    if (question.modalContainerId) {
      const addNewItemModel = document.getElementById(question.modalContainerId);
      if (!!addNewItemModel) { addNewItemModel.remove(); }
    }

    question.renderedSurvey = false;
  }
};

function addCancelButton(surveyContainerId: string, question: any) {
  const surveyContainer = document.getElementById(surveyContainerId);
  if (surveyContainer) {
    const completeButton = surveyContainer.getElementsByClassName(
      'sv_complete_btn'
    )[0] as HTMLInputElement;
    const cancelButton = createCancelButton(question);
    if (completeButton) {
      completeButton.value = `${
        question.popupCompleteButtonText
          ? question.popupCompleteButtonText
          : completeButton.value
      }`;
      completeButton.parentNode.insertBefore(cancelButton, completeButton);
    } else {
      const footerPanel = surveyContainer.getElementsByClassName(
        'panel-footer'
      )[0];
      if (footerPanel) {
        footerPanel.appendChild(cancelButton);
      }
    }
  }
}

function createCancelButton(question: any) {
  const cancelButton = document.createElement('input');
  cancelButton.type = 'button';
  cancelButton.className = 'cx-btn-secondary';
  cancelButton.setAttribute('data-dismiss', 'modal');
  cancelButton.value = `${question.popupCancelButtonText}`;
  return cancelButton;
}

Object.defineProperty(Object.prototype, 'getValueBySelector', {
  value(selector) {
    return CxSurveyInputHelper.getDeepValue(this, selector);
  }
});
