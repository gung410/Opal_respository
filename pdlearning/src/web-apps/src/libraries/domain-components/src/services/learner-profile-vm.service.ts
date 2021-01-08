import { LearningCatalogRepository, PublicUserInfo, TaggingRepository, UserRepository } from '@opal20/domain-api';
import { Observable, combineLatest, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { Injectable } from '@angular/core';
import { LearnerProfileViewModel } from '../view-models/learner-profile-view.model';
import { Utils } from '@opal20/infrastructure';

@Injectable()
export class LearnerProfileVmService {
  constructor(
    private userRepository: UserRepository,
    private learningRepository: LearningCatalogRepository,
    private taggingRepository: TaggingRepository
  ) {}

  public loadLearnerProfile(userId: string): Observable<LearnerProfileViewModel> {
    return this.processLoadLearnerProfile(this.userRepository.loadPublicUserInfoList({ userIds: [userId] }));
  }

  private processLoadLearnerProfile(userObs: Observable<PublicUserInfo[]>): Observable<LearnerProfileViewModel> {
    return userObs.pipe(
      switchMap(users => {
        if (users == null || users.length === 0) {
          return of(null);
        }
        const user = users[0];
        const registersAllMetadataIds = Utils.distinct(user.getAllMetadataIds());
        const metadatasObs = Utils.isEmpty(registersAllMetadataIds)
          ? of([])
          : this.taggingRepository.loadMetaDataTagsByIds(registersAllMetadataIds);
        return combineLatest(
          metadatasObs,
          this.learningRepository.loadUserDesignationList(),
          this.learningRepository.loadUserPortfolioList(),
          this.learningRepository.loadUserTypeOfOrganizationList(),
          this.learningRepository.loadUserRoleSpecificProficiencyList(),
          this.learningRepository.loadUserAreaOfProfessionalInterestList()
        ).pipe(
          map(([metadatas, designations, portfolios, typeOfOrganizations, roleSpecificProficiencies, areasOfProfessionalInterest]) => {
            return LearnerProfileViewModel.createFromModel(
              user,
              Utils.toDictionary(metadatas, p => p.id),
              Utils.toDictionary(designations, p => p.id),
              Utils.toDictionary(portfolios, p => p.id),
              Utils.toDictionary(typeOfOrganizations, p => p.id),
              Utils.toDictionary(roleSpecificProficiencies, p => p.id),
              Utils.toDictionary(areasOfProfessionalInterest, p => p.id)
            );
          })
        );
      })
    );
  }
}
