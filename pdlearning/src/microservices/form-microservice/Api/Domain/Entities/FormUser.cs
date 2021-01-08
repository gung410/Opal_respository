using System.Globalization;
using Conexus.Opal.AccessControl.Entities;
using Microservice.Form.Domain.Interfaces;

namespace Microservice.Form.Domain.Entities
{
    public class FormUser : UserEntity, IFullTextSearchable
    {
        // Technical Columns

        /// <summary>
        /// This column to support search by text function in UI. This is a computed data column from FirstName, LastName, Email.
        /// </summary>
        public string FullTextSearch
        {
            get => $"{FirstName ?? string.Empty}  {LastName ?? string.Empty}  {Email ?? string.Empty}";
            set { }
        }

        /// <summary>
        /// This column is a unique full-text search key column. It also support OrderBy. For now we order by Email.
        /// </summary>
        public string FullTextSearchKey
        {
            get => $"{Email?.ToString(CultureInfo.InvariantCulture) ?? string.Empty}_{Id}";
            set { }
        }
    }
}
