using System.Collections.Generic;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Validators
{
    public class HierarchyDepartmentValidationSpecification
    {
        public bool IsDirectParent { get; set; }
        public bool SkipCheckingArchetype { get; set; }
        public List<EntityStatusEnum> EntityStatusAllow { get; set; }
        public List<KeyValuePair<int, ArchetypeEnum>> HierarchyDepartments { get; set; }
        public bool IsNullArchetype { get; set; }
        public List<ArchetypeEnum> IsNotInArchetypes { get; set; }
    }
}
