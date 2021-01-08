import { CourseFundingAndSubsidyReference, ICourseFundingAndSubsidyReference } from './course-funding-and-subsidy-reference.model';

export interface IFundingAndSubsidy {
  id: number;
  title: string;
  content: string;
  references: ICourseFundingAndSubsidyReference[];
}

export class FundingAndSubsidy implements FundingAndSubsidy {
  public id: number;
  public title: string;
  public content: string;
  public references: CourseFundingAndSubsidyReference[] = [];

  constructor(data?: IFundingAndSubsidy) {
    if (data) {
      this.id = data.id;
      this.title = data.title;
      this.content = data.content;
      this.title = data.title;
      this.references = data.references.map(x => new CourseFundingAndSubsidyReference(x));
    }
  }
}

export const FUNDING_AND_SUBSIDY_DATA: FundingAndSubsidy[] = [
  new FundingAndSubsidy({
    id: 1,
    title: '1.	School Professional Development Grant (SPDG) / Divisional Training Vote',
    content:
      'For funding of school/division initiated standalone courses that do not lead to a professional qualification, or courses that are ' +
      'modularised from existing degree/non-degree programmes from approved institutions.',
    references: []
  }),
  new FundingAndSubsidy({
    id: 2,
    title: '2.	Lifelong Learning Award (LLA)',
    content:
      'For funding of officer initiated standalone courses that do not lead to a professional qualification, or courses that are  ' +
      'modularised from existing degree/non-degree programmes from approved institutions, capped at a lifetime career cap of $5000.',
    references: [
      { title: 'EO:', link: 'http://intranet.moe.gov.sg/hronline/EO/ProfessionalDevelopment/Packages/Pages/PDP4.aspx' },
      {
        title: 'AED: ',
        link: 'http://intranet.moe.gov.sg/hronline/aedhome/ProfessionalDevelopment/ProfessionalDevelopmentPackages/Pages/PDP-4-LLA.aspx'
      },
      {
        title: 'EAS: ',
        link: 'http://intranet.moe.gov.sg/hronline/EAS/ProfessionalDevelopment/ProfessionalDevelopmentPackages/Pages/PDP4.aspx '
      },
      {
        title: 'MK: ',
        link: 'http://intranet.moe.gov.sg/hronline/MKEducator/ProfessionalDevelopment/ProfessionalDevelopmentPackages/Pages/PDP4.aspx '
      }
    ]
  }),
  new FundingAndSubsidy({
    id: 3,
    title: '3.	Skills Development Fund (SDF)',
    content: 'For funding of certain SkillsFuture (SSG) courses, schools may apply for grants via the SkillsConnect system. ',
    references: [{ title: 'SDF: ', link: 'http://intranet.moe.gov.sg/academy/Pages/professional-development/SDF.aspx' }]
  }),
  new FundingAndSubsidy({
    id: 4,
    title: '4.	SkillsFuture Credit (SFC)',
    content: 'For funding of SkillsFuture (SSG) courses, Singapore citizens aged 25 and above can apply to use their SFC. ',
    references: [{ title: 'SFC: ', link: 'https://www.skillsfuture.sg/credit' }]
  })
];
