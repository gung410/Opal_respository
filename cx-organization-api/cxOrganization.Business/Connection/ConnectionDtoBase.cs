using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Business.Connection
{
    [Serializable]
    public class ConnectionDtoBase:IConexusBaseDto
    {
        /// <summary>
        /// Source of connection which is member belong to
        /// </summary>
       
        [Required]
        public ConnectionSourceDto Source { get; set; }
        /// <summary>
        /// The identity of user who do insert or update connections. This user might be belong to other system.
        /// If this is not set value, we will use LastUpdatedBy in EntityStatus of source , member
        /// </summary>
        public IdentityDto UpdatedByIdentity { get; set; }

        public List<Version> GetSupportedVersions()
        {
            return new List<Version>() { new Version("1.0") };
        }

        public Version GetDtoVersion()
        {
            return new Version("1.0");
        }

        public virtual Type GetDtoListType()
        {
            return typeof(List<ConnectionDtoBase>);
        }

        public virtual Type GetDtoListTypeByVersion(Version version)
        {
            if (new Version("1.0").Equals(version))
                return typeof(List<ConnectionDtoBase>);
            throw new NotImplementedException("ConnectionDtoBase version lower than verson 1.0 is not supported");
        }

        public virtual IConexusBaseDto MapToLatest(Version fromVersion, IConexusBaseDto oldVersionDto)
        {
            if (new Version("1.0").Equals(fromVersion))
                return oldVersionDto;
            throw new NotImplementedException("ConnectionDtoBase version lower than verson 1.0 is not supported");
        }

        public virtual IConexusBaseDto MapToVersion(IConexusBaseDto latestDto, Version toVersion)
        {
            if (toVersion.Equals(new Version("1.0")))
                return (IConexusBaseDto) latestDto;
            throw new NotImplementedException("ConnectionDtoBase version lower than verson 1.0 is not supported");
        }
    }
}
