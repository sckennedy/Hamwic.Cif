using System.ComponentModel.DataAnnotations;

namespace Hamwic.Cif.Core.Entities
{
    /// <summary>
    /// The School database table representational model
    /// </summary>
    public class School
    {
        /// <summary>
        /// The primary key for the entity
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The name of school
        /// </summary>
        [Required]
        public string Name { get; set; }
    }
}