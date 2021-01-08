import { IContainFilter, IFilter, IFromToFilter } from '@opal20/infrastructure';

export class RegistrationFilterModel {
  public department?: number[];
  public serviceScheme?: string[];
  public designation?: string[];
  public developmentalRole?: string[];
  public teachingLevel?: string[];
  public teachingCourseOfStudy?: string[];
  public teachingSubject?: string[];
  public learningFrameworks?: string[];

  public convert(): IFilter {
    const containFilters: IContainFilter[] = [];
    const fromToFilters: IFromToFilter[] = [];

    containFilters.push(
      {
        field: 'DepartmentId',
        values: this.department ? this.department.map(p => p.toString()) : null,
        notContain: false
      },
      {
        field: 'ServiceScheme',
        values: this.serviceScheme,
        notContain: false
      },
      {
        field: 'Designation',
        values: this.designation,
        notContain: false
      },
      {
        field: 'DevelopmentalRole',
        values: this.developmentalRole,
        notContain: false
      },
      {
        field: 'TeachingLevel',
        values: this.teachingLevel,
        notContain: false
      },
      {
        field: 'TeachingCourseOfStudy',
        values: this.teachingCourseOfStudy,
        notContain: false
      },
      {
        field: 'TeachingSubject',
        values: this.teachingSubject,
        notContain: false
      },
      {
        field: 'LearningFramework',
        values: this.learningFrameworks,
        notContain: false
      }
    );

    return {
      containFilters: containFilters,
      fromToFilters: fromToFilters
    };
  }
}
