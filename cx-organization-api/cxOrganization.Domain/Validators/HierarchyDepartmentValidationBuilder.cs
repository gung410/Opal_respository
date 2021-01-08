using System.Collections.Generic;
using System.Linq;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Validators
{
    public class HierarchyDepartmentValidationBuilder
    {
        private readonly HierarchyDepartmentValidationSpecification _dto;

        public HierarchyDepartmentValidationBuilder()
        {
            _dto = new HierarchyDepartmentValidationSpecification
            {
                IsDirectParent = true,
                HierarchyDepartments = new List<KeyValuePair<int, ArchetypeEnum>>(),
                IsNullArchetype=false,
                IsNotInArchetypes = new List<ArchetypeEnum>()
            };
        }

        /// <summary>
        /// Add a department to the list need for validation, make sure the child be added after the parent
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="departmentArchetype"></param>
        /// <returns></returns>
        public HierarchyDepartmentValidationBuilder ValidateDepartment(int departmentId, ArchetypeEnum departmentArchetype)
        {
            _dto.HierarchyDepartments.Add(new KeyValuePair<int, ArchetypeEnum>(departmentId, departmentArchetype));
            return this;
        }


        /// <summary>
        /// Allow status to check
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public HierarchyDepartmentValidationBuilder WithStatus(params EntityStatusEnum[] filters)
        {
            if (filters == null)
            {
                return this;
            }
            _dto.EntityStatusAllow = filters.ToList();
            return this;
        }

        /// <summary>
        /// The child department just need to belong to the path, no matter it have a direct department or not in the list
        /// </summary>
        /// <returns></returns>
        public HierarchyDepartmentValidationBuilder IsNotDirectParent()
        {
            _dto.IsDirectParent = false;
            return this;
        }

        /// <summary>
        /// The previous department is a direct department of the next one
        /// </summary>
        /// <returns></returns>
        public HierarchyDepartmentValidationBuilder IsDirectParent()
        {
            _dto.IsDirectParent = true;
            return this;
        }

        /// <summary>
        /// Skip checking archetype
        /// </summary>
        /// <returns></returns>
        public HierarchyDepartmentValidationBuilder SkipCheckingArchetype()
        {
            _dto.SkipCheckingArchetype = true;
            return this;
        }
        /// <summary>
        /// Archetype is null
        /// </summary>
        /// <returns></returns>
        public HierarchyDepartmentValidationBuilder IsNullArchetype()
        {
            _dto.IsNullArchetype = true;
            return this;
        }
        /// <summary>
        /// Return the validation specification 
        /// </summary>
        /// <returns></returns>
        public HierarchyDepartmentValidationSpecification Create()
        {
            return _dto;
        }
        public HierarchyDepartmentValidationBuilder IsNotInArchetypes(List<ArchetypeEnum> ListOfArchetype)
        {
            _dto.IsNotInArchetypes = ListOfArchetype;
            return this;
        }
    }
}
