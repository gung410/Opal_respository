import { Md5 } from 'md5-typescript';
import { Injectable } from '@angular/core';
import { environment } from 'app-environments/environment';
import { AppConstant } from '../app.constant';

@Injectable()
export class ImageHelpers {
  public static getAvatarFromEmail(
    email: string,
    imageSize: number = 80,
    gravatarTypeD: string = 'mm'
  ): string {
    if (!email) {
      return AppConstant.defaultAvatar;
    }
    const hash = Md5.init(email.toLowerCase()).toLowerCase();

    return `${environment.gravatarUrl}/${hash}.jpg?s=${imageSize}&d=${gravatarTypeD}`;
  }
}
