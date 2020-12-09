using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;


namespace Courier.Models
{

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña Actual")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La contraseña debe ser de al menos 6 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña Nueva")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirme nueva contraseña")]
        [Compare("NewPassword", ErrorMessage = "Las Contraseñas no son identicas.")]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required]
        [Display(Name = "Rut Usuario")]
        [RegularExpression("\\d*[0-9]-([0-9]|k|K)$", ErrorMessage = "Campo Rut Formato incorrecto, ejemplo: 12345678-9")]  
        [Remote("ValidarUsuarioSesion", "Validation")]        
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contaseña")]
        public string Password { get; set; }

        [Display(Name = "¿Recordar?")]
        public bool RememberMe { get; set; }
    }

    //public class RegisterModel
    //{
    //    [Required]        
    //    [Display(Name = "Rut")]
    //    [RegularExpression("\\d*[0-9]-([0-9]|k|K)$", ErrorMessage = "Campo Rut Formato incorrecto, ejemplo: 12345678-9")]
    //    public string UserName { get; set; }        
    //    //[Remote("ValidarUsuario", "Validation")]
     

    //    [Required]
    //    [Display(Name = "Nombres")]    
    //    public string Nombres { get; set; }

    //    [Required]
    //    [Display(Name = "Apellidos")]
    //    public string Apellidos { get; set; }

    //    [Required]
    //    [DataType(DataType.EmailAddress)]
    //    [Display(Name = "Correo Electrónico")]
    //    public string Email { get; set; }

    //    [Required]
    //    [StringLength(100, ErrorMessage = "La contraseña debe ser de al menos 6 caracteres.", MinimumLength = 6)]
    //    [DataType(DataType.Password)]
    //    [Display(Name = "Contraseña")]
    //    public string Password { get; set; }

    //    [DataType(DataType.Password)]
    //    [Display(Name = "Confirme Contraseña")]
    //    [Compare("Password", ErrorMessage = "Las contraseñas deben ser iguales.")]
    //    public string ConfirmPassword { get; set; }
    //}
}
