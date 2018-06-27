using System.ComponentModel.DataAnnotations;

namespace Hamwic.Cif.Web.Models.Account
{
    public class ForgotPasswordModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress{get; set;}
    }
}