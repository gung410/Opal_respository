
import {Md5} from 'md5-typescript';
export class CurrentUser {
    public givenName?: string;
    public familyName?: string;
    public emails?: string;
    public id?: string;
    public avatarUrl?: string;
    constructor(identityClaims: any) {
        this.id = identityClaims['sub'];
        this.givenName = identityClaims['given_name'] || identityClaims.givenName;
        this.familyName = identityClaims['family_name'] || identityClaims.familyName;
        this.emails = identityClaims.emails;
        this.avatarUrl = this.GetGravatarImageUrl(identityClaims.emails);
    }

    private GetGravatarImageUrl(email: string,
                                imageSize: number = 80,
                                gavatarTypeD: string = 'mm'): string {
        let imageURL = '';
        const gravataBaseUrl = 'http://www.gravatar.com';
        const hash = Md5.init(email.toLowerCase()).toLowerCase();
        imageURL = gravataBaseUrl + '/avatar/' + hash + '.jpg?s=' + imageSize + '&d=' + gavatarTypeD;
        return imageURL;
    }
}
