import { RegistrationType } from '@opal20/domain-api';

export const COURSE_REGISTRATION_TYPE_MAP = {
  [RegistrationType.None]: {
    text: 'None'
  },
  [RegistrationType.Manual]: {
    text: 'Self-Registration'
  },
  [RegistrationType.Application]: {
    text: RegistrationType.Application
  },
  [RegistrationType.Nominated]: {
    text: RegistrationType.Nominated
  },
  [RegistrationType.AddedByCA]: {
    text: 'Adding Participant'
  }
};

export const COURSE_REGISTRATION_STATUS_PREFIX_MAP = {
  [RegistrationType.None]: 'Registration',
  [RegistrationType.Manual]: 'Registration',
  [RegistrationType.Application]: 'Registration',
  [RegistrationType.Nominated]: 'Nomination',
  [RegistrationType.AddedByCA]: 'Adding Participant'
};
