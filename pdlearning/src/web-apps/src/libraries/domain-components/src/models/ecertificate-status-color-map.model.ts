import { ECertificateTemplateStatus } from '@opal20/domain-api';

export const ECERTIFICATE_STATUS_COLOR_MAP = {
  [ECertificateTemplateStatus.Draft]: {
    text: 'Draft',
    color: '#D8DCE6'
  },
  [ECertificateTemplateStatus.Active]: {
    text: 'Active',
    color: '#3BDC87'
  }
};
