using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Courier.Models;
using System.Transactions;
using Courier.Controllers;

namespace Autenticacion.Controllers
{
    public class AccountController : Controller
    {

        //
        // GET: /Account/LogOnhhjkhhh

        public ActionResult LogOn()
        {
            return View();
        }

        //
        // POST: /Account/LogOn
        public HtmlString sConsultaMenu(string oRut, UrlHelper oUrl)
        {

            string oResult = "";
            //comentario


            LinqBD_PARDataContext oDataContext = new LinqBD_PARDataContext();
            var oMenu = oDataContext.PR_PAR_PERFIL_USUARIO(oRut,1);

            oResult += "<ul id=\"browser\" class=\"filetree\">";

            var oCarpetaName = "";
            var oAcumulaFinSubMenu = "";
            var oAcumulaFinMenu = "";
            var oCantidadMenu = 0;
            var oCantidadSubMenu = 0;
            var oSubCarpetaName = "";
            var oCantidadFiles = 0;
            foreach (var oFile in oMenu)
            {
                //Menu
                if (oCarpetaName != oFile.FL_PAR_CAR_NOMBRE)
                {
                    if (oCantidadFiles > 0)
                    {
                        oCantidadFiles = 0;
                        oResult += "</ul>";
                    }
                    if (oCantidadSubMenu > 0)
                    {
                        oCantidadSubMenu = 0;
                        oResult += oAcumulaFinSubMenu;
                        oAcumulaFinSubMenu = "";
                        oResult += "</ul>";
                    }
                    if (oCantidadMenu > 0)
                    {
                        oResult += oAcumulaFinMenu;
                        oAcumulaFinMenu = "";
                    }
                    oCarpetaName = oFile.FL_PAR_CAR_NOMBRE;
                    if (oFile.FL_PAR_CUS_CLASS != null && oFile.FL_PAR_CUS_CLASS != "")
                    {
                        if (oFile.FL_PAR_CUS_CLASS == "collapsable" || oFile.FL_PAR_CUS_CLASS == "collapsable lastCollapsable")
                        {
                            oResult += string.Format("<li id='C{2}' class=\"{1}\"><span class=\"folder\" onclick='JavaScript:CambiaMenu({2})'>{0}</span>", oFile.FL_PAR_CAR_NOMBRE, "closed", oFile.PK_PAR_CAR_ID);
                        }
                        else
                        {
                            oResult += string.Format("<li id='C{1}' ><span class=\"folder\" onclick='JavaScript:CambiaMenu({1})'>{0}</span>", oFile.FL_PAR_CAR_NOMBRE, oFile.PK_PAR_CAR_ID);
                        }
                    }
                    else if (oFile.FL_PAR_CAR_CLASS == "closed")
                        oResult += string.Format("<li id='C{2}' class=\"{1}\"><span class=\"folder\" onclick='JavaScript:CambiaMenu({2})'>{0}</span>", oFile.FL_PAR_CAR_NOMBRE, oFile.FL_PAR_CAR_CLASS, oFile.PK_PAR_CAR_ID);
                    else
                    {
                        oResult += string.Format("<li id='C{1}' ><span class=\"folder\" onclick='JavaScript:CambiaMenu({1})'>{0}</span>", oFile.FL_PAR_CAR_NOMBRE, oFile.PK_PAR_CAR_ID);
                    }
                    oAcumulaFinMenu += "</li>";
                    oCantidadMenu += 1;
                }
                //SubMenu
                if (oFile.FL_PAR_SBC_NOMBRE != null)
                {
                    if (oSubCarpetaName != oFile.FL_PAR_SBC_NOMBRE)
                    {
                        if (oCantidadFiles > 0)
                        {
                            oCantidadFiles = 0;
                            oResult += "</ul>";
                        }
                        if (oCantidadSubMenu > 0)
                        {
                            oResult += oAcumulaFinSubMenu;
                            oAcumulaFinSubMenu = "";
                        }
                        oSubCarpetaName = oFile.FL_PAR_SBC_NOMBRE;
                        if (oCantidadSubMenu == 0) oResult += "<ul>";

                        if (oFile.FL_PAR_CUB_CLASS != null && oFile.FL_PAR_CUB_CLASS != "")
                        {
                            if (oFile.FL_PAR_CUB_CLASS == "collapsable" || oFile.FL_PAR_CUB_CLASS == "collapsable lastCollapsable")
                            {
                                oResult += string.Format("<li  id='S{2}' class=\"{1}\"><span class=\"folder\" onclick=\"JavaScript:CambiaMenuS({2})\">{0}</span>", oFile.FL_PAR_SBC_NOMBRE, "closed", oFile.PK_PAR_SBC_ID);
                            }
                            else
                            {
                                oResult += string.Format("<li  id='S{1}'><span class=\"folder\" onclick=\"JavaScript:CambiaMenuS({1})\">{0}</span>", oFile.FL_PAR_SBC_NOMBRE, oFile.PK_PAR_SBC_ID);
                            }
                        }
                        else if (oFile.FL_PAR_SBC_CLASS == "closed")
                            oResult += string.Format("<li  id='S{2}' class=\"{1}\"><span class=\"folder\" onclick=\"JavaScript:CambiaMenuS({2})\">{0}</span>", oFile.FL_PAR_SBC_NOMBRE, oFile.FL_PAR_SBC_CLASS, oFile.PK_PAR_SBC_ID);
                        else
                        {
                            oResult += string.Format("<li  id='S{1}'><span class=\"folder\" onclick=\"JavaScript:CambiaMenuS({1})\">{0}</span>", oFile.FL_PAR_SBC_NOMBRE, oFile.PK_PAR_SBC_ID);
                        }
                        oAcumulaFinSubMenu += "</li>";
                        oCantidadSubMenu += 1;
                    }
                }
                //FILE
                if (oFile.FL_PAR_LST_NOMBRE != null)
                {
                    string sUrl = oUrl.Action(oFile.FL_PAR_LST_ACTION, oFile.FL_PAR_LST_CONTROLLER).Replace("%3f", "?").Replace("%3d", "=");
                    if (oCantidadFiles == 0) oResult += "<ul>";
                    oResult += string.Format("<li><span class=\"{1}\"><a href=\"{2}\">{0}</a></span></li>", oFile.FL_PAR_LST_NOMBRE, oFile.FL_PAR_LST_CLASS, sUrl);
                    //CargarPaginaContent
                    //oResult += string.Format("<li><span class=\"{1}\"><a href=\"#\" onclick=\"javascript:CargarPaginaContent({2},{3});\">{0}</a></span></li>", oFile.FL_PAR_LST_NOMBRE, oFile.FL_PAR_LST_CLASS, oFile.FL_PAR_LST_CONTROLLER, oFile.FL_PAR_LST_ACTION);

                    oCantidadFiles += 1;
                }
            }
            oResult += oAcumulaFinSubMenu;
            oResult += oAcumulaFinMenu;
            oResult += "</ul>";

            var oIhtml = new HtmlString(oResult);
            return (oIhtml);

        }
        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {

                    ValidationController oVal = new ValidationController();
                    var oSuc = oVal.Sucursal(model.UserName);
                    Session["SucName"] = oSuc.Nombre;
                    Session["SucName2"] = oSuc.Nombre;
                    Session["SucId"] = oSuc.id;                    
                    

                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);


                    Session["Token"] = "";                    

                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Rut o Contraseña incorrecto.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            ValidationController oValidation = new ValidationController();

            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            oPar.PR_PAR_USU_LOGIN_CLEAR(oValidation.GetRutActiveUser(), 1);

            Session["Token"] = null;
            Session["SucName"] = null;
            Session["SucName2"] = null;
            Session["Id"] = null;
            Session["Menu"] = null;
            Session["RutEmpresa"] = null;
            Session["Entrar"] = null;

            FormsAuthentication.SignOut();            

            return RedirectToAction("Index", "Home");
           
        }



        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [Authorize]
        public ActionResult EliminarUsuario(string UserName)
        {
            UserResponse oResult = new UserResponse();
            try
            {
                using (TransactionScope escope = new TransactionScope())
                {

                    Membership.DeleteUser(UserName);

                    LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

                    var oRut = UserName.Substring(0, UserName.Length - 2);

                    var oUsuario = (from m in oPar.TB_PAR_USU_USUARIO
                                    where m.PK_PAR_USU_RUT == Convert.ToDecimal(oRut)
                                    select m).Single();

                    oPar.TB_PAR_USU_USUARIO.DeleteOnSubmit(oUsuario);

                    oPar.SubmitChanges();

                    escope.Complete();

                    oResult.Ok = true;
                    oResult.Mensaje="Usuario Elminado Exitosamente";
                }
            }
            catch (TransactionAbortedException ex)
            {
                oResult.Ok = false;
                oResult.Mensaje = "No fue posible eliminar el usuario : " + ex.Message;                
            }
            catch (ApplicationException ex)
            {
                oResult.Ok = true;
                oResult.Mensaje = "No fue posible eliminar el usuario : " + ex.Message;                
            }
            return Json(oResult);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ActualizaPassword(AdministracionUsuario oModel)
        {
            UserResponse oResult = new UserResponse();
            try
            {
                MembershipUser oUser = Membership.GetUser(oModel.UserNameEdit);
                oUser.ChangePassword(oUser.ResetPassword(), oModel.Password);
                oResult.Ok = true;
                oResult.Mensaje="<p>Contraseña actualizada exitosamente</p>";                
            }
            catch (Exception e)
            {
                oResult.Ok = false;
                oResult.Mensaje="<p>No fue posible cambiar la contraseña:</p>" + e.Message;
            }
            return Json(oResult);
            
        }

        //[Authorize(Roles = "Administrador")]
        public ActionResult EditarClave(string UserName)
        {
            AdministracionUsuario oModel = new AdministracionUsuario { UserNameEdit=UserName };
            return PartialView("_FormularioCambioPassword",oModel);
        }
        [Authorize][HttpPost]
        public ActionResult UpdateRegister(AdministracionUsuario model, FormCollection oForm)
        {
            UserResponse oResultUP = new UserResponse();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
                    LinqBD_SEGDataContext oSeg = new LinqBD_SEGDataContext();


                    //USUARIO
                    var oUsu = (from usu in oPar.TB_PAR_USU_USUARIO
                                where usu.PK_PAR_USU_RUT == Convert.ToDecimal(model.UserNameEdit.Substring(0, model.UserNameEdit.Length - 2))
                               select usu).Single();

                    oUsu.FL_PAR_USU_NOMBRES = model.Nombres;
                    oUsu.FL_PAR_USU_APELLIDOS = model.Apellidos;
                    oUsu.PK_PAR_SUC_ID = Convert.ToInt32(model.Sucursal);


                    //EMAIL
                    MembershipUser oUsuario = Membership.GetUser(model.UserNameEdit);
                    oUsuario.Email = model.Email;
                    Membership.UpdateUser(oUsuario);
                    

                    //GET USUARIO

                    var oSegUsu = (from m in oSeg.aspnet_Users 
                               where m.UserName==model.UserNameEdit
                               select m).Single();


                    //ROLES
                    List<aspnet_UsersInRoles> bdRoles = new List<aspnet_UsersInRoles>();
                    var oRoles = from rol in oSeg.aspnet_Roles select rol;                    
                    
                    string oResult;
                    foreach (var oFile in oRoles)
                    {
                        oResult = oForm[oFile.RoleId.ToString()];
                        if (oResult == "true,false")
                        {
                            bdRoles.Add(new aspnet_UsersInRoles { UserId = oSegUsu.UserId, RoleId = oFile.RoleId });
                        }
                    }

                    //Borro todos los roles del usuario

                    var oRolesUsuario = from roles in oSeg.aspnet_UsersInRoles
                                        where roles.UserId==oSegUsu.UserId
                                        select roles;


                    oSeg.aspnet_UsersInRoles.DeleteAllOnSubmit(oRolesUsuario);
                    oSeg.aspnet_UsersInRoles.InsertAllOnSubmit(bdRoles);                    
                    oSeg.SubmitChanges();                        
                    oPar.SubmitChanges();

                    scope.Complete();

                    oResultUP.Ok = true;
                    oResultUP.Mensaje="<p>Registro Actualizado Exitosamente!</p>";
                }
            }
            catch (TransactionAbortedException ex) {
                oResultUP.Ok = false;
                oResultUP.Mensaje = "No fue posible actualizar la información : " + ex.Message;                
            }
            catch (ApplicationException ex) {
                oResultUP.Ok = false;
                oResultUP.Mensaje = "No fue posible actualizar la información : " + ex.Message;
            }
            return Json(oResultUP);
        }

        public class UserResponse
        {
            public bool Ok {get;set;}
            public string Mensaje {get;set;}
        }

        [HttpPost][Authorize]
        public ActionResult Register(AdministracionUsuario model, FormCollection oForm)
        {
            UserResponse oResultIn = new UserResponse();
            ValidationController oValidation = new ValidationController();
            if (model.tipoUsuario=="2")
            {
                if (oValidation.isDecimal(model.Empresa)==false)
                {
                    oResultIn.Ok = false;
                    oResultIn.Mensaje = "Para usuarios externos debe seleccionar una empresa";
                    return Json(oResultIn);
                }
                else if (oValidation.isDecimal(model.SucursalEmpresa) == false)
                {
                    oResultIn.Ok = false;
                    oResultIn.Mensaje = "Para usuarios externos debe seleccionar una sucursal empresa";
                    return Json(oResultIn);
                }
            }
            if (ModelState.IsValid)
            {
                try
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        // Attempt to register the user
                        MembershipCreateStatus createStatus;
                        Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

                        if (createStatus == MembershipCreateStatus.Success)
                        {

                            LinqBD_PARDataContext oDC = new LinqBD_PARDataContext();
                            LinqBD_SEGDataContext oSeg = new LinqBD_SEGDataContext();
                            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

                            TB_PAR_USU_USUARIO oUsuario = new TB_PAR_USU_USUARIO
                            {
                                PK_PAR_USU_RUT = Convert.ToInt32(model.UserName.Substring(0, model.UserName.Length - 2)),
                                FL_PAR_USU_APELLIDOS = model.Apellidos,
                                FL_PAR_USU_DV = model.UserName.Substring(model.UserName.Length - 1, 1),
                                FL_PAR_USU_NOMBRES = model.Nombres,
                                PK_PAR_SUC_ID = Convert.ToInt32(model.Sucursal) ,
                                PK_PAR_TUS_ID=Convert.ToInt32(model.tipoUsuario)
                            };

                            if (model.Empresa != null && model.Empresa != string.Empty)
                            {
                                if (oValidation.isDecimal(model.Empresa) && oValidation.isDecimal(model.SucursalEmpresa))
                                {
                                    System.Data.Linq.EntitySet<TB_PAR_USE_USU_SUC_EMP> oUSE = new System.Data.Linq.EntitySet<TB_PAR_USE_USU_SUC_EMP>();
                                    oUSE.Add( new TB_PAR_USE_USU_SUC_EMP
                                    {
                                        PK_CLI_EMP_RUT = Convert.ToInt32(model.Empresa),
                                        PK_CLI_SUC_ID = Convert.ToInt32(model.SucursalEmpresa),
                                        PK_PAR_USU_RUT = Convert.ToInt32(model.UserName.Substring(0, model.UserName.Length - 2))
                                    });
                                    oUsuario.TB_PAR_USE_USU_SUC_EMP = oUSE;
                                }

                                oFol.PR_FOL_ULM_INSERT(Convert.ToInt32(model.Empresa), Convert.ToInt32(model.UserName.Substring(0, model.UserName.Length - 2)));
                            }

                            oDC.TB_PAR_USU_USUARIO.InsertOnSubmit(oUsuario);
                            //debo validar al usuario.. verificar despues... controlar son 2 registros en bases distintas

                            var oRoles = from rol in oSeg.aspnet_Roles
                                         select rol;

                            List<aspnet_UsersInRoles> bdRoles = new List<aspnet_UsersInRoles>();

                            var oUser = (from user in oSeg.aspnet_Users
                                         where user.UserName == model.UserName
                                         select user.UserId).Single();

                            string oResult;
                            foreach (var oFile in oRoles)
                            {
                                oResult = oForm[oFile.RoleId.ToString()];
                                if (oResult == "true,false")
                                {
                                    bdRoles.Add(new aspnet_UsersInRoles { UserId = oUser, RoleId = oFile.RoleId });
                                }
                            }

                            oSeg.aspnet_UsersInRoles.InsertAllOnSubmit(bdRoles);

                            var oConsulta = from sus in oDC.TB_PAR_SUS_SUCURSAL_USUARIO
                                            where sus.PK_PAR_USU_RUT == Convert.ToInt32(model.UserName.Substring(0, model.UserName.Length - 2))
                                            && sus.PK_PAR_SUC_ID == Convert.ToInt32(model.Sucursal)
                                            select sus;

                            if (oConsulta.Count() == 0)
                            {
                                oDC.TB_PAR_SUS_SUCURSAL_USUARIO.InsertOnSubmit(new TB_PAR_SUS_SUCURSAL_USUARIO
                                {
                                    PK_PAR_SUC_ID = Convert.ToInt32(model.Sucursal),
                                    PK_PAR_USU_RUT = Convert.ToInt32(model.UserName.Substring(0, model.UserName.Length - 2))
                                });
                            }
                            

                            oDC.SubmitChanges();
                            oSeg.SubmitChanges();



                            //FormsAuthentication.SetAuthCookie(model.RegistroUsuario.UserName, false /* createPersistentCookie */);

                            scope.Complete();
                            oResultIn.Ok = true;
                            oResultIn.Mensaje = "Registro Almacenado Exitosamente";
                        }
                        else
                        {
                            ModelState.AddModelError("", ErrorCodeToString(createStatus));
                        }
                    }
                }
                catch (Exception e)
                {
                    oResultIn.Ok = false;
                    oResultIn.Mensaje = "<p>Ocurrio un error al guardar la información</p>" + e.Message;
                }

            }
            // If we got this far, something failed, redisplay form
            return Json(oResultIn);
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "La contraseña actual es incorrecta o la nueva es invalida.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        #region Status Codes
        public static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Usuario ya existe. Por favor ingrese un nombre de usuario distinto.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Un usuario ya tiene la cuenta de correo. For favor ingrese una cuenta de correo distinta.";

                case MembershipCreateStatus.InvalidPassword:
                    return "La contaseña ingresada es invalida. Por favor ingrese una contraseña valida.";

                case MembershipCreateStatus.InvalidEmail:
                    return "La cuenta de correo es invalida. Por favor ingrese un valor correcto e intente nuevamente.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
        
    }
}
