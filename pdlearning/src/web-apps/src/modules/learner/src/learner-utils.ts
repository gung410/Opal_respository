import * as moment from 'moment';

import { DigitalContent, DigitalContentType, MyCourseDisplayStatus, MyRegistrationStatus, RegistrationType } from '@opal20/domain-api';

import { IFormBuilderDefinition } from '@opal20/infrastructure';
import { ILearningFeedbacksConfiguration } from './models/learning-feedbacks.model';
import { Validators } from '@angular/forms';
import { requiredAndNoWhitespaceValidator } from '@opal20/common-components';

export function isClassRunStarted(startDate: Date): boolean {
  if (!startDate) {
    return false;
  }
  return moment().isSameOrAfter(startDate, 'day');
}

/**
 * To check if the class run is ended or not.
 * Ex: endDate: 19/08/2020 => true if today is 20/08/2020 or after, ortherwise false.
 * @param endDate
 * @returns True if class is ended
 */
export function isClassRunEnded(endDate: Date): boolean {
  if (!endDate) {
    return false;
  }
  return moment(endDate)
    .add(1, 'day')
    .isSameOrBefore(moment(), 'day');
}

export function isClassRunApplicable(startDate: Date, endDate: Date): boolean {
  if (!startDate) {
    return false;
  }
  return moment().isSameOrAfter(startDate, 'day') && moment().isSameOrBefore(endDate, 'day');
}

export function canProcessClassRun(startDate: Date, workingDayChecking: number): boolean {
  if (!startDate) {
    return false;
  }
  return moment()
    .add(workingDayChecking, 'day')
    .isSameOrBefore(startDate, 'day');
}

export function getDigitalContenType(dc: DigitalContent): string {
  if (dc.type === DigitalContentType.LearningContent) {
    return 'document';
  }
  return dc.fileExtension;
}

export function generateMaskedSessionCodeFormat(courseCode: string): string | null {
  if (!courseCode) {
    return null;
  }
  const courseCodeCharacter = 'A';
  const wordsOfCodeRegex = new RegExp(/^(.*?)-/);
  const matches = courseCode.match(wordsOfCodeRegex);
  if (!matches || !matches[1]) {
    return null;
  }
  const numberOfWordsInCourseCode = matches[1].length;
  return `${courseCodeCharacter.repeat(numberOfWordsInCourseCode)}-AAAAAA-AA-AAA`;
}

export function toDisplayStatusFromRegistrationStatus(
  registrationStatus: MyRegistrationStatus,
  registrationType: RegistrationType
): MyCourseDisplayStatus {
  // prefer to the prefix of MyCourseDisplayStatus
  let displayStatusPrefix = '';
  switch (registrationType) {
    case RegistrationType.Nominated:
      displayStatusPrefix = 'Nominated';
      break;
    case RegistrationType.AddedByCA:
      displayStatusPrefix = 'AddedByCA';
      break;
    default:
      break;
  }
  return MyCourseDisplayStatus[`${displayStatusPrefix}${registrationStatus}`];
}

// This function will have in R3.0 (master branch) => we're only temporary use for fix issue
export function copyTextToClipboard(text: string): void {
  const clipboard = navigator.clipboard;
  if (!clipboard) {
    fallbackCopyTextToClipboard();
    return;
  }
  clipboard.writeText(text);

  function fallbackCopyTextToClipboard(): void {
    const dummyElement = document.createElement('span');
    dummyElement.style.whiteSpace = 'pre';
    dummyElement.textContent = sessionStorage.heavensDoor;
    document.body.appendChild(dummyElement);

    const selection = window.getSelection();
    selection.removeAllRanges();
    const range = document.createRange();
    range.selectNode(dummyElement);
    selection.addRange(range);

    document.execCommand('copy');

    selection.removeAllRanges();
    document.body.removeChild(dummyElement);
  }
}

export function getLearnerReviewForm(configuration: ILearningFeedbacksConfiguration): IFormBuilderDefinition {
  if (!configuration) {
    return;
  }
  const formBuilder: IFormBuilderDefinition = {
    formName: 'form',
    controls: {}
  };

  if (configuration.isShow(configuration.rating)) {
    formBuilder.controls.rating = {};
    if (configuration.rating === 'mandatory') {
      formBuilder.controls.rating = { validators: [{ validator: Validators.required }] };
    }
  }
  if (configuration.isShow(configuration.review)) {
    formBuilder.controls.comment = {};
    if (configuration.review === 'mandatory') {
      formBuilder.controls.comment = {
        validators: [{ validator: requiredAndNoWhitespaceValidator() }]
      };
    }

    if (formBuilder.controls.comment.validators) {
      formBuilder.controls.comment.validators.push({
        validator: Validators.maxLength(2000)
      });
    } else {
      formBuilder.controls.comment = {
        validators: [{ validator: Validators.maxLength(2000) }]
      };
    }
  }

  return formBuilder;
}
