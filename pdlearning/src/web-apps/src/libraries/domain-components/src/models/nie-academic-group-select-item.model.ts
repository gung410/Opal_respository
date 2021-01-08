import { IOpalSelectDefaultItem } from '@opal20/common-components';
import { NieAcademicGroup } from '@opal20/domain-api';

export function buildNieAcademicGroupSelectItems(): IOpalSelectDefaultItem<string>[] {
  return [
    {
      value: NieAcademicGroup.AsianLanguagesAndCultures,
      label: 'Asian Languages and Cultures'
    },
    {
      value: NieAcademicGroup.EnglishLanguageAndLiterature,
      label: 'English Language and Literature'
    },
    {
      value: NieAcademicGroup.HumanitiesAndSocialStudiesEducation,
      label: 'Humanities and Social Studies Education'
    },
    {
      value: NieAcademicGroup.LearningSciencesAndAssessment,
      label: 'Learning Sciences and Assessment'
    },
    {
      value: NieAcademicGroup.MathematicsAndMathematicsEducation,
      label: 'Mathematics and Mathematics Education'
    },
    {
      value: NieAcademicGroup.NaturalSciencesAndScienceEducation,
      label: 'Natural Sciences and Science Education'
    },
    {
      value: NieAcademicGroup.PhysicalEducationAndSportsScience,
      label: 'Physical Education and Sports Science'
    },
    {
      value: NieAcademicGroup.PolicyCurriculumAndLeadership,
      label: 'Policy, Curriculum and Leadership'
    },
    {
      value: NieAcademicGroup.PsychologyAndChildHumanDevelopment,
      label: 'Psychology and Child & Human Development'
    },
    {
      value: NieAcademicGroup.VisualAndPerformingArts,
      label: 'Visual and Performing Arts'
    }
  ];
}
