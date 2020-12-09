using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Courier.Models;
using Autenticacion.Controllers;
using System.Transactions;
using System.Web.Security;

namespace Courier.Controllers
{
    public class ImportController : Controller
    {
        //
        // GET: /Import/

        public ActionResult Index()
        {
            Linq_BD_IMPDataContext oImp = new Linq_BD_IMPDataContext();

            var oDatos = from imp in oImp.IMPORT_MOVISTAR
                         select imp;

            ValidationController oValidation = new ValidationController();
            foreach (var oFile in oDatos)
            {
                //AccountController oAC = new AccountController();

                AdministracionUsuario model = new AdministracionUsuario{
                    tipoUsuario="1",
                    Empresa="96786140",
                    SucursalEmpresa="5",
                    UserName=oFile.RUT.ToString() + "-" + oFile.DV.ToString(),
                    Password=oFile.CLAVE.ToString(),
                    ConfirmPassword=oFile.CLAVE.ToString(),
                    Apellidos="",
                    Nombres=oFile.NOMBRE,
                    Sucursal="3"
                };


                try
                {
                    //using (TransactionScope scope = new TransactionScope())
                    //{
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
                            foreach (var oFile2 in oRoles)
                            {
                                //oResult = oForm[oFile.RoleId.ToString()];
                                if (oFile2.RoleName=="KAM Empresa")
                                {
                                    bdRoles.Add(new aspnet_UsersInRoles { UserId = oUser, RoleId = oFile2.RoleId });
                                }
                            }

                            oSeg.aspnet_UsersInRoles.InsertAllOnSubmit(bdRoles);

                            
                            

                            oDC.SubmitChanges();
                            oSeg.SubmitChanges();



                            //FormsAuthentication.SetAuthCookie(model.RegistroUsuario.UserName, false /* createPersistentCookie */);

                            //scope.Complete();                            
                        }
                        else
                        {
                            
                        }
                    }
                //}
                catch (Exception e)
                {

                }
                
            }


            return View();
        }

    }
}
