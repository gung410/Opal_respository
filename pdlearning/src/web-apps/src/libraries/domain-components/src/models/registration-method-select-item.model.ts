import { IOpalSelectDefaultItem } from '@opal20/common-components';
import { RegistrationMethod } from '@opal20/domain-api';

export function buildRegistrationMethodSelectItems(): IOpalSelectDefaultItem<string>[] {
  return [
    {
      value: RegistrationMethod.Private,
      label: 'By nomination only'
    },
    {
      value: RegistrationMethod.Public,
      label: 'No registration required'
    },
    {
      value: RegistrationMethod.Restricted,
      label: 'By registration/nomination'
    }
  ];
}
