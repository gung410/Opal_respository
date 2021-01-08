using System;
using System.Collections.Generic;
using System.Linq;
using Thunder.Platform.Core.Context;

namespace Thunder.Platform.EntityFrameworkCore.Logging
{
    public class ThunderQueryTrackingSource : IQueryTrackingSource
    {
        public readonly List<string> _trackingInformation = new List<string>();

        public ThunderQueryTrackingSource(IUserContext context)
        {
            var requestId = context.GetValue<string>(CommonUserContextKeys.RequestId);
            requestId = !string.IsNullOrEmpty(requestId) ? requestId : Guid.NewGuid().ToString();
            this.PushTrackingInformation($"Tracking Id (requestId): {requestId}");
        }

        public void PushTrackingInformation(string trackingInformation)
        {
            _trackingInformation.Add(trackingInformation);
        }

        public string PopTrackingInformation()
        {
            var item = _trackingInformation[^1]; // ^1 = _trackingInformation.length - 1
            _trackingInformation.Remove(item);
            return item;
        }

        public string GetAllTrackingInformation()
        {
            return string.Join('\0', _trackingInformation.Select(i => $"-- {i} \n").ToArray());
        }
    }
}
