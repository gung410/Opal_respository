// tslint:disable:max-line-length
// tslint:disable:only-arrow-functions
// tslint:disable:radix
import * as Survey from 'survey-angular';
import { CxObjectUtil } from '../../../utils/object.util';

export const cxSelect2TagBox = {
    name: 'cxtagbox',
    title: 'CX tag box',
    iconName: 'icon-tagbox',
    widgetIsLoaded() {
        return typeof $ === 'function'; // && (!!$.fn as any).select2; TODO: We need to also check if the select2 library is loaded but not sure why the commented-out code doesn't work - Will fix later.
    },
    defaultJSON: {
        choices: ['Item 1', 'Item 2', 'Item 3']
    },
    htmlTemplate: '<select multiple=\'multiple\' style=\'width: 100%;\'></select>',
    isFit(question) {
        return question.getType() === 'cxtagbox';
    },
    activatedByChanged(activatedBy) {
        Survey.JsonObject.metaData.addClass(
            'cxtagbox',
            [
                {
                    name: 'hasOther',
                    visible: false
                }
            ],
            null,
            'checkbox'
        );
        Survey.JsonObject.metaData.addProperty('cxtagbox', {
            name: 'select2Config',
            default: null
        });
        Survey.JsonObject.metaData.addProperty('cxtagbox', {
            name: 'noResultText',
            default: ''
        });
        Survey.JsonObject.metaData.addProperty('cxtagbox', {
            name: 'storeWholeObject',
            default: false
        });
        Survey.JsonObject.metaData.addProperty('cxtagbox', {
            name: 'keyName',
            default: ''
        });
    },
    fixStyles(el) {
        el.parentElement.querySelector('.select2-search__field').style.border =
            'none';
    },
    afterRender(question, el) {
        // Ensure that if the property 'storeWholeObject' is true then the property 'keyName' is also required.
        //  Otherwise, the question will be removed from the DOM.
        if (question.storeWholeObject && !question.keyName) {
            console.warn(`Question '${question.name}' is configured to store whole object as an item in the array but missing definition of 'question.keyName'`);
            $(`#${question.id}`).remove();
            return;
        }

        const self = this;
        const select2Config = question.select2Config;
        const noResultText = question.noResultText;
        let settings = select2Config && typeof select2Config === 'string' ? JSON.parse(select2Config) : select2Config;

        // Add config to set no result text
        if (noResultText && noResultText !== '') {
          settings = settings || {};
          settings.language = { noResults: () => noResultText };
        }

        const $el: any = $(el).is('select') ? $(el) : $(el).find('select');

        self.willUnmount(question, el);

        $el.select2({
            tags: 'true',
            disabled: question.isReadOnly,
            theme: 'classic'
        });

        self.fixStyles(el);

        const updateValueHandler = function() {
            if (question.storeWholeObject) {
                const values = question.value.map(value => CxObjectUtil.getPropertyValue(value, question.keyName));
                $el.val(values).trigger('change');
            } else {
                $el.val(question.value).trigger('change');
            }
            self.fixStyles(el);
        };
        const buildChoice = function(choice) {
            return {
                id: question.storeWholeObject ? CxObjectUtil.getPropertyValue(choice.value, question.keyName) : choice.value,
                text: choice.text,
                object: question.storeWholeObject ? choice.value : {}   // Keep the whole object for later usage.
            };
        };
        const updateChoices = function() {
            $el.select2().empty();

            if (settings) {
                if (settings.ajax) {
                    $el.select2(settings);
                } else {
                    settings.data = question.visibleChoices.map(function(choice) {
                        return buildChoice(choice);
                    });
                    $el.select2(settings);
                }
            } else {
                $el.select2({
                    data: question.visibleChoices.map(function(choice) {
                        return buildChoice(choice);
                    })
                });
            }

            updateValueHandler();
        };

        question._propertyValueChangedFnSelect2 = function() {
            updateChoices();
        };

        question.readOnlyChangedCallback = function() {
            $el.prop('disabled', question.isReadOnly);
        };
        question.registerFunctionOnPropertyValueChanged(
            'visibleChoices',
            question._propertyValueChangedFnSelect2
        );
        question.valueChangedCallback = updateValueHandler;
        const getDataId = (id) => {
            // Try to convert the 'id' into the same type as the choice value in order
            //  to fix the selected values that are not updated when selecting/unselecting the default items.
            // See more detail by looking into the issue #2 which is mentioned here
            //  https://surveyjs.answerdesk.io/ticket/details/T2515/tagbox-selected-values-doesn-t-work-correctly-if-the-value-is-either-number-or-an-object
            if (question.visibleChoices && typeof question.visibleChoices[0].value === 'number') {
                const firstChoice = question.visibleChoices[0];
                if (firstChoice.value && firstChoice.value.toString().indexOf('.') > -1) { return parseFloat(id); }
                return parseInt(id);
            }
            return id;
        };
        const findIndex = function(arr, obj, keyName) {
            let index = 0;
            while (index < arr.length && CxObjectUtil.getPropertyValue(arr[index], keyName) !== CxObjectUtil.getPropertyValue(obj, keyName)) {
                index++;
            }
            if (index === arr.length) {
                index = -1;
            }
            return index;
        };
        $el.on('select2:select', function(e) {
            question.value = (question.value || [])
                .concat(question.storeWholeObject ? e.params.data.object : getDataId(e.params.data.id));
        });
        $el.on('select2:unselect', function(e) {
            const index = question.storeWholeObject
                ? findIndex(question.value || [], e.params.data.object, question.keyName)   // Find the index of the object in the array which matching with the property 'keyName'.
                : (question.value || []).indexOf(getDataId(e.params.data.id));
            if (index !== -1) {
                const val = [].concat(question.value);
                val.splice(index, 1);
                question.value = val;
            }
        });
        updateChoices();
    },
    willUnmount(question, el) {
        if (!question._propertyValueChangedFnSelect2) { return; }

        ($(el)
            .find('select')
            .off('select2:select') as any)
            .select2('destroy');
        question.readOnlyChangedCallback = null;
        question.valueChangedCallback = null;
        question.unRegisterFunctionOnPropertyValueChanged(
            'visibleChoices',
            question._propertyValueChangedFnSelect2
        );
        question._propertyValueChangedFnSelect2 = undefined;
    }
};
