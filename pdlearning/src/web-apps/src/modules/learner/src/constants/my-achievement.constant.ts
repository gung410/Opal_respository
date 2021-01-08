export enum MY_ACHIEVEMENT_TYPE_ENUM {
  ECertificates = 'e-certificates',
  DigitalBadges = 'digitalbadges'
}

export const MY_ACHIEVEMENT_TYPE_MAPPING_TEXT_CONST: Map<MY_ACHIEVEMENT_TYPE_ENUM, string> = new Map<MY_ACHIEVEMENT_TYPE_ENUM, string>([
  [MY_ACHIEVEMENT_TYPE_ENUM.ECertificates, 'E-Certificates'],
  [MY_ACHIEVEMENT_TYPE_ENUM.DigitalBadges, 'Badges']
]);
