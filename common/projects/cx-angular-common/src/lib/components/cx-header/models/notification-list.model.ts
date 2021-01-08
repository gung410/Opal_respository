import { NotificationItem } from "./notification-item.model"

export class NotificationList {
    totalCount : number;
    totalNewCount: number;
    totalUnreadCount: number;
    items : NotificationItem[];
    constructor(data: Partial<NotificationList>) {
        if (!data) { return; }
        this.totalNewCount = data.totalNewCount ? data.totalNewCount : 0;
        this.totalCount = data.totalCount ? data.totalCount : 0;
        this.totalUnreadCount = data.totalUnreadCount ? data.totalUnreadCount : 0;
        this.items = data.items ? data.items : [];
    }
}
