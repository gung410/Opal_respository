using System.Collections.Generic;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Commands
{
    public class InitPredefineUserPreferenceForUserCommand : BaseThunderCommand
    {
        public InitPredefineUserPreferenceForUserCommand(List<string> keys)
        {
            Keys = keys;
        }

        /// <summary>
        /// If keys has value (not null or empty). Then init preferences only for these keys.
        /// </summary>
        public List<string> Keys { get; set; }
    }
}
