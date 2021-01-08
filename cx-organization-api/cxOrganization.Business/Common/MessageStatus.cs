using cxOrganization.Business.Extensions;
using cxPlatform.Client.ConexusBase;
using System.Net;

namespace cxOrganization.Business.Common
{
    public class MessageStatus
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
       
        public bool IsOk()
        {
            return StatusCode == 200;
        }
        public bool IsNoContent()
        {
            return StatusCode == 204;
        }
        public static MessageStatus Create(int statusCode, string message)
        {
            return new MessageStatus {StatusCode = statusCode, Message = message};
        }

        public static MessageStatus CreateError(string message = null)
        {
            return Create((int) HttpStatusCode.InternalServerError, message);
        }
        public static MessageStatus CreateNoContent(string message = null)
        {
            return Create((int)HttpStatusCode.NoContent, message);
        }
        public static MessageStatus CreateSuccess(string message = null)
        {
            return Create((int)HttpStatusCode.OK, message);
        }
        public static MessageStatus CreateNotFound(string message = null)
        {
            return Create((int)HttpStatusCode.NotFound, message);
        }
        public static MessageStatus CreateNotFound(IdentityDto identityDto)
        {
            return Create((int)HttpStatusCode.NotFound, string.Format("{0} is not found", identityDto.ToStringInfo()));
        }
        public static MessageStatus CreateInvalid(string message = null)
        {
            return Create((int)HttpStatusCode.BadRequest, message);
        }
    }
}