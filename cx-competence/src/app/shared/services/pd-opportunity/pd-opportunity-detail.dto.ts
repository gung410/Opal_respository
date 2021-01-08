export interface PDOpportunityDetailGetDTO {
  id: string;
  thumbnailUrl?: string;
  courseName?: string;
  durationHours?: number;
  durationMinutes?: number;
  pdActivityType?: string;
  courseCode?: string;
  learningMode?: string;
  description?: string;
  categoryIds?: string[];
  notionalCost?: number;
  courseFee?: number;
  subjectAreaIds?: string[];
  courseObjective?: string;
  natureOfCourse?: string;
  courseLevel?: string;
  externalCode?: string;
  courseOutlineStructure?: string;
  status?: string;
  registrationMethod?: string;
}
