export interface ILearningFeedbacksConfiguration {
  rating: ConfigurationType;
  review: ConfigurationType;
  evaluation: ConfigurationType;
  allowToEdit: boolean;
  isShow: (configType: ConfigurationType) => boolean;
}

export type ConfigurationType = 'none' | 'mandatory' | 'optional';
