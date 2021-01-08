export class CxPeoplePickerHelper {

  static onValueChanged(question) {
    question.rowCount = question.value ? question.value.length : 0;
    question.koValue(question.value); // Trigger to update UI.
  }
}
