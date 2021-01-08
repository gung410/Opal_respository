import { IOpalSelectDefaultItem } from '@opal20/common-components';
import { PrerequisiteCertificateType } from '@opal20/domain-api';

export function buildPrerequisiteEcertificateTypeSelectItems(): IOpalSelectDefaultItem<string>[] {
  return [
    { value: PrerequisiteCertificateType.CompletionCourse, label: 'Completion of course' },
    {
      value: PrerequisiteCertificateType.CompletionCourseEvaluation,
      label: 'Completion of course evaluation form'
    }
  ];
}
