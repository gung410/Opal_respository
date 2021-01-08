export enum AgeGroupEnum {
  UnderTwenty = 0,
  Twenties = 20,
  Thirties = 30,
  Forties = 40,
  FiftyAndGreater = 50,
}

export const AgeGroupTextConst = {
  [AgeGroupEnum.UnderTwenty]: '0-19',
  [AgeGroupEnum.Twenties]: '20-29',
  [AgeGroupEnum.Thirties]: '30-39',
  [AgeGroupEnum.Forties]: '40-49',
  [AgeGroupEnum.FiftyAndGreater]: '> 50',
};
