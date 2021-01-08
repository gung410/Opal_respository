import { ConfigurationType, ILearningFeedbacksConfiguration } from '../models/learning-feedbacks.model';

export class LearningFeedbacksConfig {
  public static microlearning: ILearningFeedbacksConfiguration = {
    rating: 'mandatory',
    review: 'optional',
    evaluation: 'none',
    allowToEdit: true,
    isShow: LearningFeedbacksConfig.isShow
  };
  public static digitalContent: ILearningFeedbacksConfiguration = {
    rating: 'optional',
    review: 'optional',
    evaluation: 'none',
    allowToEdit: false,
    isShow: LearningFeedbacksConfig.isShow
  };
  public static faceToFace: ILearningFeedbacksConfiguration = {
    rating: 'none',
    review: 'mandatory',
    evaluation: 'mandatory',
    allowToEdit: true,
    isShow: LearningFeedbacksConfig.isShow
  };
  public static online: ILearningFeedbacksConfiguration = {
    rating: 'none',
    review: 'mandatory',
    evaluation: 'mandatory',
    allowToEdit: true,
    isShow: LearningFeedbacksConfig.isShow
  };

  private static isShow(configType: ConfigurationType): boolean {
    return configType === 'mandatory' || configType === 'optional';
  }
}
