﻿using System;
using System.Collections.Generic;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Client
{
    public class MemberDto : ConexusBaseDto
    {
        /// <summary>
        /// Valid to
        /// </summary>
        public DateTime? ValidTo { get; set; }

        /// <summary>
        /// Valid from
        /// </summary>
        public DateTime? validFrom { get; set; }

        /// <summary>
        /// member role id
        /// </summary>
        public int? MemberRoleId { get; set; }

        /// <summary>
        /// Created date time
        /// </summary>
        public DateTime? Created { get; set; }

        /// <summary>
        /// Created by user id
        /// </summary>
        public int? CreatedBy { get; set; }

        /// <summary>
        /// Membership role (not in use)
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Membership period (not in use)
        /// </summary>
        public PeriodDto Period { get; set; }

        /// <summary>
        ///DisplayName
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Referrer Token of external system
        /// </summary>
        public string ReferrerToken { get; set; }

        /// <summary>
        /// ReferrerResource link to external instance
        /// </summary>
        public string ReferrerResource { get; set; }

        /// <summary>
        /// Archetype of external instance
        /// </summary>
        public int? ReferrerArchetypeId { get; set; }
        /// <summary>
        /// Usergroup MemberId
        /// </summary>
        public long? UserGroupMemberId { get; set; }
        /// <summary>
        /// Period Id
        /// </summary>
        public int? PeriodId { get; set; }
    }
}
