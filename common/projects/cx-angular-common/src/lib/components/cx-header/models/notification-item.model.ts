export class NotificationItem {
    avatarUrl?: string;
    senderName: string;
    subject?: string;
    message: string;
    sentDate: Date;
    id: string | number;
    messageId: string | number;
    hyperLink?: string;
    dateReadUtc?: Date;
    constructor(data: Partial<NotificationItem>) {
        if (!data) { return; }
        this.id = data.id;
        this.messageId = data.messageId;
        this.avatarUrl = data.avatarUrl;
        this.senderName = data.senderName;
        this.subject = data.subject;
        this.message = data.message;
        this.hyperLink = data.hyperLink;
        this.dateReadUtc = data.dateReadUtc;
    }
}
