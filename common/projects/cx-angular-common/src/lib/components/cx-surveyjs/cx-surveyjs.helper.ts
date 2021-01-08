import { Question, QuestionPanelDynamic } from 'survey-angular';
import { uniqueId } from 'lodash';
import { CxConfirmationDialogComponent } from '../cx-confirmation-dialog/cx-confirmation-dialog.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

export class CxSurveyjsHelper {
  static removeAutocomplete(htmlElement: HTMLElement, question: Question) {
    const questionInput = htmlElement.querySelector('input');
    if (questionInput) {
      questionInput.setAttribute('autocomplete', 'off');
    }
  }

  static disableDatePickerInput(htmlElement: HTMLElement, question: Question) {
    const questionInput = htmlElement.querySelector('input');
    if (questionInput) {
      questionInput.setAttribute('readonly', 'true');
    }
  }

  static addPopOverButton(htmlElement: HTMLElement, question: Question) {
    const btn = document.createElement('a');
    btn.tabIndex = 0;
    btn.id = 'question-popover-' + uniqueId();
    btn.className = 'popover-btn';
    btn.innerHTML = 'i';
    btn.dataset.role = 'button';
    btn.dataset.toggle = 'popover';
    btn.dataset.trigger = 'focus';
    btn.dataset.placement = 'right';
    btn.dataset.container = 'body';
    btn.dataset.html = 'true';
    btn.dataset.content = question.popoverText;
    btn.onclick = (() => { $('#' + btn.id).popover('toggle'); });
    const header = htmlElement.querySelector('h5');
    header.appendChild(btn);
  }

  static removePanelUI(ngbModal: NgbModal, questionPanelDynamic: QuestionPanelDynamic, panel: any) {
    if (!questionPanelDynamic.canRemovePanel) { return; }
    if (!questionPanelDynamic.confirmDelete) {
      questionPanelDynamic.removePanel(panel);
      return;
    }
    const modalRef = ngbModal.open(CxConfirmationDialogComponent, { centered: true, backdrop: 'static' });
    const componentInstance = modalRef.componentInstance as CxConfirmationDialogComponent;
    componentInstance.cancelButtonText = 'No';
    componentInstance.confirmButtonText = 'Yes';
    componentInstance.isDanger = true;
    componentInstance.content = questionPanelDynamic.confirmDeleteText;
    componentInstance.cancel.subscribe(() => modalRef.close());
    componentInstance.confirm.subscribe(() => {
      questionPanelDynamic.removePanel(panel);
      modalRef.close();
    });
  }

  static moveSelectAllToTop(htmlElement): void {
    if (htmlElement.querySelector('.sv-selectbase__item.sv-q-col-1 [aria-label="Select all"]')) {
      const fieldset = htmlElement.querySelector('fieldset');
      if (fieldset) {
        const checkboxSelectAll = fieldset.querySelector('div');
        if (!checkboxSelectAll) { return; }

        fieldset.classList.add('select-all');
        checkboxSelectAll.classList.add('checkbox-select-all');
      }
    }
  }

  /**
   * By default, the matrix single choice or the matrix dropdown doesn't have the heading text of the first column of the table.
   * This method will add the heading text of the first column.
   * @param htmlElement The html element of the matrix.
   * @param question The matrix question.
   */
  static addFirstHeadingTextToMatrixTable(htmlElement: HTMLElement, question: Question) {
    if (question.firstHeadingText) {
      const firstCellInMatrixSingleChoiceSelector = 'table > thead > tr > td:nth-child(1)';
      let firstCell = htmlElement.querySelector(firstCellInMatrixSingleChoiceSelector);
      if (!firstCell) {
        const firstCellInMatrixDropdownSelector = 'table > thead > tr > th:nth-child(1)';
        firstCell = htmlElement.querySelector(firstCellInMatrixDropdownSelector);
      }
      firstCell.innerHTML = question.firstHeadingText;
    }
  }
}
