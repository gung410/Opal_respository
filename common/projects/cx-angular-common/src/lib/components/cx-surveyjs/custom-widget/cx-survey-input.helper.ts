import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CxConfirmationDialogComponent } from '../../cx-confirmation-dialog/cx-confirmation-dialog.component';

export class CxSurveyInputHelper {
  static onRemoveItem(ngbModal: NgbModal, question: any, removingItem) {
    const showConfirmBox =
      question.confirmDeleteText && question.confirmDeleteText.length > 0;
    if (!showConfirmBox) {
      CxSurveyInputHelper.removeItem(question, removingItem);
      return;
    }
    const confirmDeleteText = question.confirmDeleteText.replace(
      '{0}',
      CxSurveyInputHelper.getDisplayText(removingItem, question)
    );
    const modalRef = ngbModal.open(CxConfirmationDialogComponent, { centered: true, backdrop: 'static' });
    const componentInstance = modalRef.componentInstance as CxConfirmationDialogComponent;
    componentInstance.header = 'Confirmation';
    componentInstance.cancelButtonText = 'No';
    componentInstance.confirmButtonText = 'Yes';
    componentInstance.isDanger = true;
    componentInstance.content = confirmDeleteText;
    componentInstance.cancel.subscribe(() => modalRef.close());
    componentInstance.confirm.subscribe(() => {
      CxSurveyInputHelper.removeItem(question, removingItem);
      modalRef.close();
    });
  }

  static removeItem(question, removingItem) {
    question.value = question.value.filter(
      quesValue =>
        !CxSurveyInputHelper.isTheSameItems(quesValue, removingItem, question.keyName)
    );
    CxSurveyInputHelper.onValueChanged(question);
  }


  /**
   * Gets deep value.
   * e.g: obj = { country: { name: 'Norway', code: '+47' } }
   * selector = 'country'        => result = { name: 'Norway', code: '+47' }
   * selector =  'country.name'  => result = 'Norway'.
   */
  static getDeepValue(obj: any, selector: string) {
    if (!obj) {
      return '';
    }
    if (!selector || selector.length === 0) {
      return obj;
    }
    const indexOfSeparator = selector.indexOf('.');
    const hasChildSegment = indexOfSeparator > -1;
    if (hasChildSegment) {
      const currentNode = selector.split('.')[0];
      const deeperSelector = selector.substring(indexOfSeparator + 1);
      return CxSurveyInputHelper.getDeepValue(obj[currentNode], deeperSelector);
    }
    return obj[selector];
  }

  static getDisplayText(objValue, question) {
    return CxSurveyInputHelper.getDeepValue(objValue, question.fieldToDisplay);
  }

  static isTheSameItems(objA: any, objB: any, selector: string) {
    const objAValue = CxSurveyInputHelper.getDeepValue(objA, selector);
    const objBValue = CxSurveyInputHelper.getDeepValue(objB, selector);
    if (typeof objAValue !== typeof objBValue) {
      return false;
    } else {
      return JSON.stringify(objAValue) === JSON.stringify(objBValue);
    }
  }

  static onValueChanged(question) {
    question.rowCount = question.value ? question.value.length : 0;
    question.koValue(question.value); // Trigger to update UI.
  }
}
