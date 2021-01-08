namespace Communication.Business.MailLog
{
    public interface IMailLogger
    {
        void WriteLog(MailLogMessage message);
    }
}
