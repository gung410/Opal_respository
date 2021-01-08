using System;
using System.Collections.Generic;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Business.Connection
{
    [Serializable]
    public class ConnectionDto : ConnectionDtoBase
    {
        /// <summary>
        /// List of members of connection
        /// </summary>
        public List<ConnectionMemberDto> Members { get; set; }

      
        public override Type GetDtoListTypeByVersion(Version version)
        {
            if (new Version("1.0").Equals(version))
                return new List<ConnectionDto>().GetType();
            throw new NotImplementedException("ConnectionDto version lower than verson 1.0 is not supported");
        }

        public override IConexusBaseDto MapToLatest(Version fromVersion, IConexusBaseDto oldVersionDto)
        {
            if (new Version("1.0").Equals(fromVersion))
                return oldVersionDto;
            throw new NotImplementedException("ConnectionDto version lower than verson 1.0 is not supported");
        }

        public override IConexusBaseDto MapToVersion(IConexusBaseDto latestDto, Version toVersion)
        {
            if (toVersion.Equals(new Version("1.0")))
                return (IConexusBaseDto)latestDto;
            throw new NotImplementedException("ConnectionDto version lower than verson 1.0 is not supported");
        }
        public override Type GetDtoListType()
        {
            return typeof(List<ConnectionDto>);
        }

    }
}
