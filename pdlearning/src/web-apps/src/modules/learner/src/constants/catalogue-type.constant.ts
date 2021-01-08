import { CatalogResourceType } from '@opal20/domain-api';

export enum CATALOGUE_TYPE_ENUM {
  AllCourses = 'allcourses',
  RecommendedForYou = 'recommendations',
  RecommendedByYourOrganisation = 'recommendations-organisation',
  NewlyAdded = 'newly-added',
  Courses = 'courses',
  Microlearning = 'microlearning',
  DigitalContent = 'digitalcontent',
  SharedToMe = 'sharedtome' // shared by other users, diff learningpath
}

export const CATALOGUE_TYPE_MAPPING_TEXT_CONST: Map<CATALOGUE_TYPE_ENUM, string> = new Map<CATALOGUE_TYPE_ENUM, string>([
  [CATALOGUE_TYPE_ENUM.AllCourses, 'All Courses'],
  [CATALOGUE_TYPE_ENUM.Courses, 'Courses'],
  [CATALOGUE_TYPE_ENUM.Microlearning, 'Microlearning'],
  [CATALOGUE_TYPE_ENUM.DigitalContent, 'Digital Content'],
  [CATALOGUE_TYPE_ENUM.SharedToMe, 'Shared by other users']
]);

export const CATALOGUE_MAPPING_TYPE_CONST: Map<CATALOGUE_TYPE_ENUM, CatalogResourceType> = new Map<
  CATALOGUE_TYPE_ENUM,
  CatalogResourceType
>([
  [CATALOGUE_TYPE_ENUM.Courses, 'course'],
  [CATALOGUE_TYPE_ENUM.Microlearning, 'microlearning'],
  [CATALOGUE_TYPE_ENUM.DigitalContent, 'content']
]);
