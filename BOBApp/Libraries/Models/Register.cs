using Libraries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Libraries.Models
{
    public class Register
    {
        [Required(ErrorMessage= "Gelieve een naam in te geven")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "De naam moet tussen 2 - 50 tekens bevatten")]
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Cellphone { get; set; }
        private string _Password;

        public string Password
        {
            get { return _Password; }
            set { _Password =md5.Create(value); }
        }

        public string FacebookID { get; set; }
        public bool? IsBob { get; set; }
        public double? PricePerKm { get; set; }
        public int? BobsType_ID { get; set; }
        public string LicensePlate { get; set; }
        public int? AutoType_ID { get; set; }


        //IDataErrorInfo hier uitvoeren (voor de error te laten zien)
        public string Error
        {
            get { return null; }
        }

        public bool IsValid()
        {
            return Validator.TryValidateObject(this, new ValidationContext(this, null, null), null, true);
        }

        public string this[string columnName]
        {
            get
            {
                try
                {
                    object value = this.GetType().GetProperty(columnName).GetValue(this);
                    Validator.ValidateProperty(value, new ValidationContext(this, null, null) { MemberName = columnName });
                }
                catch (ValidationException ex)
                {
                    return ex.Message;
                }
                return String.Empty;
            }
        }
    }
}
