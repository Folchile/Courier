using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Courier.Models;
using System.Web.Security;
using System.Web.Routing;
using Courier.Controllers;
using System.Xml.Linq;

namespace Courier.Controllers
{
    public class AdministracionController : Controller
    {

        #region Representa la carga inicial de CAR, SBC, LST
        public ActionResult Carpeta()
        {
            LinqBD_PARDataContext oDC = new LinqBD_PARDataContext();
            var oDatos = from CAR in oDC.TB_PAR_CAR_CARPETA
                         orderby CAR.FL_PAR_CAR_ORDEN
                         select CAR;

            var oSubCarpetas = from SUB in oDC.VI_PAR_SUBCARPETA
                               orderby SUB.PK_PAR_CAR_ID,SUB.FL_PAR_SBC_ORDEN
                               select SUB;

            var oListaItems = from ITM in oDC.VI_PAR_LIST_ITEM_FOR_TABLE       
                              orderby ITM.FL_PAR_CAR_ORDEN,ITM.FL_PAR_SBC_ORDEN,ITM.FL_PAR_LST_ORDEN
                              select ITM;

            var oModel = new AdministracionCarpeta()
            {
                oListaCarpetas = oDatos.ToList(),
                oListaSubCarpetas = oSubCarpetas.ToList(),
                oListaItemsForTable=oListaItems.ToList()
            };
            return View(oModel);
        }
        #endregion

        #region subcarpeta
        public ActionResult SubCarpeta()
        {
            LinqBD_PARDataContext oDC = new LinqBD_PARDataContext();
            

            var oSubCarpetas = from SUB in oDC.VI_PAR_SUBCARPETA
                               orderby SUB.PK_PAR_CAR_ID,SUB.FL_PAR_SBC_ORDEN
                               select SUB;


            var oModel = new AdministracionCarpeta()
            {
               
                oListaSubCarpetas = oSubCarpetas.ToList()
               
            };
            return PartialView("_ListaSubCarpetas",oModel);
        }
        #endregion

        #region item
        public ActionResult Item()
        {
            LinqBD_PARDataContext oDC = new LinqBD_PARDataContext();


            var oItems = from LST in oDC.VI_PAR_LIST_ITEM_FOR_TABLE
                         orderby LST.FL_PAR_CAR_ORDEN, LST.FL_PAR_SBC_ORDEN, LST.FL_PAR_LST_ORDEN
                               select LST;


            var oModel = new AdministracionCarpeta()
            {

                oListaItemsForTable = oItems.ToList()

            };
            return PartialView("_ListaItems", oModel);
        }
        #endregion

        #region Seguridad
        public ActionResult DropDownListaSucursalEmpresa(AdministracionUsuario oModel)
        {                    
            oModel.GetListaSucursalEmpresa();
            return PartialView("_DropDownListaSucursalEmpresa", oModel);
        }

        public ActionResult ActualizaUsuario(string filtro)
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            if (filtro == null || filtro == "")
            {
                var oDatos = from Usu in oPar.VI_PAR_USUARIO
                             select Usu;

                var oModel = new AdministracionUsuario()
                {
                    oListaUsuarios = oDatos.ToList()

                };

                return PartialView("_ListaUsuarios", oModel);
            }
            else
            {
                var oDatos = from Usu in oPar.VI_PAR_USUARIO
                             where 
                                Usu.FL_PAR_USU_NOMBRES.Contains(filtro)
                                || Usu.FL_PAR_USU_APELLIDOS.Contains(filtro)
                                || Usu.PK_PAR_USU_RUT.ToString().Contains(filtro)
                                || Usu.PK_CLI_EMP_RUT.ToString().Contains(filtro)
                                || Usu.FL_CLI_EMP_FANTASIA.Contains(filtro)
                                || Usu.Email.Contains(filtro)
                             select Usu;

                var oModel = new AdministracionUsuario()
                {
                    oListaUsuarios = oDatos.ToList()

                };

                return PartialView("_ListaUsuarios", oModel);
            }
        }
        #region _ListaRoles
        public ActionResult _ListaRoles(AdministracionUsuario oModel)
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            int? iSistema=null;
            if (oModel.Sistema != null)
                iSistema = Convert.ToInt32(oModel.Sistema);
            var oDatos = (from rol in oPar.PR_PAR_MAN_SEG_LISTA_ROLES(iSistema)
                          select rol).ToList() ;
            oModel.oListaRolesFiltro = oDatos;
            return PartialView("_ListaRoles", oModel);
        }
        #endregion

        [Authorize]
        public ActionResult Usuario()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            LinqBD_SEGDataContext oSeg = new LinqBD_SEGDataContext();

            var oDatos = from Usu in oPar.VI_PAR_USUARIO
                         select Usu;

            var oRoles = from Rol in oSeg.aspnet_Roles
                         select Rol;

            //var oFunciones = from Fun in oPar.VI_PAR_FUNCIONES
            //                 select Fun;

            var oSis = from pr in oPar.TB_PAR_SIS_SISTEMA
                       select new SelectListItem
                       {
                           Text = pr.FL_PAR_SIS_NOMBRE.ToUpper(),
                           Value = pr.PK_PAR_SIS_ID.ToString()
                       };

            var oModel = new AdministracionUsuario()
            {
                oListaUsuarios=oDatos.ToList(),
                oListaRoles=oRoles.ToList(),
                oListaSistema=oSis
            };

            return View(oModel);
        }

        [Authorize]
        public ActionResult FormularioUsuario(string UserName)
        {
            LinqBD_SEGDataContext oSeg = new LinqBD_SEGDataContext();
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();            

            AdministracionUsuario oModel = new AdministracionUsuario();

            var oSucursal = from suc in oPar.TB_PAR_SUC_SUCURSAL
                            orderby suc.FL_PAR_SUC_NOMBRE
                            select new SelectListItem
                            {
                                Text = suc.FL_PAR_SUC_NOMBRE,
                                Value = suc.PK_PAR_SUC_ID.ToString()
                            };

            oModel.oListaSucursal = oSucursal;


            //USUARIO EXISTENTE
            if (UserName != "" && UserName != null)
            {                
                var oUsuario = (from usu in oPar.VI_PAR_USUARIO
                                where usu.UserName==UserName
                               select usu).Single();
                

                oModel.UserName = UserName;
                oModel.Nombres = oUsuario.FL_PAR_USU_NOMBRES;
                oModel.Apellidos = oUsuario.FL_PAR_USU_APELLIDOS;
                oModel.Email = oUsuario.Email;
                oModel.Sucursal = oUsuario.PK_PAR_SUC_ID.ToString();

                List<BoolSetting> oListaCheck = new List<BoolSetting>();

                var oRoles = from rol in oSeg.aspnet_Roles                             
                             select new
                             {
                                 rol.RoleId,
                                 rol.RoleName
                             };

                var oRolesUsuario = from rol in oSeg.aspnet_Roles
                                    join roluser in oSeg.aspnet_UsersInRoles on rol.RoleId equals roluser.RoleId
                                    join user in oSeg.aspnet_Users on roluser.UserId equals user.UserId
                                    where user.UserName == UserName
                                    select rol;

                bool oExiste = false;
                foreach (var oFila in oRoles)
                {
                    oExiste = false;
                    foreach (var oF in oRolesUsuario)
                    {
                        if (oF.RoleId == oFila.RoleId)
                            oExiste = true;
                    }
                    
                    
                    oListaCheck.Add(new BoolSetting { Text = oFila.RoleId.ToString(), Value = oExiste, DisplayName = oFila.RoleName });
                    
                }

                oModel.Roles = oListaCheck;
                oModel.UserNameEdit = oModel.UserName;

                return PartialView("_FormularioEditarUsuario", oModel);                
            }
            else //USUARIO NUEVO
            {

                List<BoolSetting> oListaCheck = new List<BoolSetting>();

                var oRoles = from rol in oSeg.aspnet_Roles select rol;

                foreach (var oFila in oRoles)
                {
                    oListaCheck.Add(new BoolSetting { Text = oFila.RoleId.ToString(), Value = false , DisplayName=oFila.RoleName});
                }

                oModel.Roles = oListaCheck;
                oModel.GetListaTipoUsuario();
                oModel.GetListaEmpresa();

                List<SelectListItem> oLstItem = new List<SelectListItem>();
                oModel.oListaSucursal = oLstItem;

                oModel.GetListaSucursalEmpresa();

               

                return PartialView("_FormularioUsuario", oModel);
            }
        }

        #endregion        
       
        #region Carga Formularios
        public ActionResult formularios(AdministracionCarpeta oModel)
        {
            oModel.Tipo = 1;
            return  View(oModel);
        }
        #endregion

        #region Carga datos antes de visualizar Agregar Sub Carpeta
        public ActionResult formulariosSubCarpeta(AdministracionCarpeta oModel)
        {


            LinqBD_PARDataContext oDC = new LinqBD_PARDataContext();
            var oCarpetas = from CAR in oDC.TB_PAR_CAR_CARPETA
                            select new SelectListItem
                            {
                                Text=CAR.FL_PAR_CAR_NOMBRE,
                                Value=CAR.PK_PAR_CAR_ID.ToString()
                            };


            
            oModel.oListaCarpetasDropdown = oCarpetas;
                   


            oModel.Tipo = 3;
            return PartialView("formularios", oModel);
        }
        #endregion        

        #region Guardar Editado de Carpeta
        [HttpPost][Authorize]
        public string SaveEditarCarpeta(AdministracionCarpeta oModel)
        {
            if (oModel != null)
            {
                LinqBD_PARDataContext oDc = new LinqBD_PARDataContext();

                var oCarpeta = (from oCar in oDc.TB_PAR_CAR_CARPETA
                                where oCar.PK_PAR_CAR_ID == oModel.Id
                                select oCar).Single();

                oCarpeta.FL_PAR_CAR_NOMBRE = oModel.NombreCarpeta;
                oCarpeta.FL_PAR_CAR_CLASS = oModel.Clase;
                oCarpeta.FL_PAR_CAR_ORDEN = oModel.Orden;
                

                try
                {
                    oDc.SubmitChanges();
                    return ("Registro Actualizado");
                }
                catch (Exception e)
                {
                    return ("<p>No fue posible Actualizar la información</p>" + e.Message);
                }

            }
            else
            {
                return ("Error en el modelo");

            }
        }
        #endregion

        #region Guardar Editado de SUB Carpeta
        [HttpPost]
        [Authorize]
        public string SaveEditarSubCarpeta(AdministracionCarpeta oModel)
        {
            if (oModel != null)
            {
                LinqBD_PARDataContext oDc = new LinqBD_PARDataContext();

                var oCarpeta = (from oCar in oDc.TB_PAR_SBC_SUB_CARPETA
                                where oCar.PK_PAR_SBC_ID == oModel.Id
                                select oCar).Single();

                oCarpeta.FL_PAR_SBC_CLASS = oModel.Clase;
                oCarpeta.FL_PAR_SBC_NOMBRE = oModel.NombreSubCarpeta;
                oCarpeta.FL_PAR_SBC_ORDEN = oModel.Orden;
                oCarpeta.PK_PAR_CAR_ID = Convert.ToInt32(oModel.IdPadre);


                try
                {
                    oDc.SubmitChanges();
                    return ("Registro Actualizado");
                }
                catch (Exception e)
                {
                    return ("<p>No fue posible Actualizar la información</p>" + e.Message);
                }

            }
            else
            {
                return ("Error en el modelo");

            }
        }
        #endregion

        #region Carga ID sub carpeta, listado de un dropdownlistbox
        [HttpPost]
        public ActionResult cargaidSubCarpeta(FormCollection oForm)
        {
            var oValor=0;
            if (oForm["id_load_sbc"] != null)
            {
                if (ValidationController.IsNumeric(oForm["id_load_sbc"]) == true)
                    oValor = Convert.ToInt32(oForm["id_load_sbc"]);
            }

            LinqBD_PARDataContext oDC = new LinqBD_PARDataContext();

            var oLista = from SBC in oDC.TB_PAR_SBC_SUB_CARPETA
                         where SBC.PK_PAR_CAR_ID == oValor
                         select new SelectListItem
                         {
                             Value=SBC.PK_PAR_SBC_ID.ToString(),
                             Text=SBC.FL_PAR_SBC_NOMBRE
                         };

            AdministracionCarpeta oModel = new AdministracionCarpeta();

            oModel.oListaSubCarpetasDropdown = oLista;

            return PartialView("_CargaControl", oModel);
        }
        #endregion

        #region Carga datos de formulario para edicion de carpeta
        [HttpPost]
        public ActionResult EditarCarpeta(FormCollection oForm)
        {
            var oValor=0;
            if (ValidationController.IsNumeric(oForm["carpeta_edit_id"]))
                oValor = Convert.ToInt32(oForm["carpeta_edit_id"]);
            
            AdministracionCarpeta oModel = new AdministracionCarpeta();
            if (oValor!=0)
            {
                LinqBD_PARDataContext oDC = new LinqBD_PARDataContext();

                var oDatos = from Car in oDC.TB_PAR_CAR_CARPETA
                             where Car.PK_PAR_CAR_ID == oValor
                             select Car;

                oModel.Id = oValor;
                oModel.Tipo = 2;//determina que se trata de una edicion en el view formularios
                foreach (var oFile in oDatos)
                {
                    oModel.NombreCarpeta = oFile.FL_PAR_CAR_NOMBRE;
                    oModel.Orden = oFile.FL_PAR_CAR_ORDEN;
                    oModel.Clase = oFile.FL_PAR_CAR_CLASS;
                    oModel.Sistema = Convert.ToInt32(oFile.PK_PAR_SIS_ID);
                }
            }           

            return PartialView("formularios",oModel);
        }
        #endregion

        #region Carga datos de formulario para edicion de SUB carpeta
        [HttpPost]
        public ActionResult EditarSubCarpeta(FormCollection oForm)
        {
            var oValor = 0;
            if (ValidationController.IsNumeric(oForm["subcarpeta_edit_id"]))
                oValor = Convert.ToInt32(oForm["subcarpeta_edit_id"]);

            AdministracionCarpeta oModel = new AdministracionCarpeta();
            if (oValor != 0)
            {

                LinqBD_PARDataContext oDC = new LinqBD_PARDataContext();
                               

                var oDatos = (from Car in oDC.TB_PAR_SBC_SUB_CARPETA
                             where Car.PK_PAR_SBC_ID == oValor
                             select Car).Single();

                oModel.Id = oValor;
                oModel.Tipo = 4;//determina que se trata de una edicion de subcarpeta en el view formularios                
                oModel.NombreSubCarpeta = oDatos.FL_PAR_SBC_NOMBRE;
                oModel.Orden = oDatos.FL_PAR_SBC_ORDEN;
                oModel.Clase = oDatos.FL_PAR_SBC_CLASS;
                oModel.IdPadre = oDatos.PK_PAR_CAR_ID.ToString();                    
                

                var oCarpetas = from CAR in oDC.TB_PAR_CAR_CARPETA
                                select new SelectListItem
                                {
                                    Text = CAR.FL_PAR_CAR_NOMBRE,
                                    Value = CAR.PK_PAR_CAR_ID.ToString(),
                                    Selected = (CAR.PK_PAR_CAR_ID == oDatos.PK_PAR_CAR_ID)
                                };

                oModel.oListaCarpetasDropdown = oCarpetas; 
            }

            return PartialView("formularios", oModel);
        }
        #endregion

        #region Carga datos Antes de Agregar ITEM
        public ActionResult formulariosItem(AdministracionCarpeta oModel)
        {
            LinqBD_PARDataContext oDC = new LinqBD_PARDataContext();


            var oSistema = from sis in oDC.TB_PAR_SIS_SISTEMA
                           select new SelectListItem
                           {
                               Text = sis.FL_PAR_SIS_NOMBRE,
                               Value = sis.PK_PAR_SIS_ID.ToString()
                           };

            var oCarpetas = from CAR in oDC.TB_PAR_CAR_CARPETA
                            select new SelectListItem
                            {
                                Text = CAR.FL_PAR_CAR_NOMBRE,
                                Value = CAR.PK_PAR_CAR_ID.ToString()
                            };


            var oSubCarpetas = from SBC in oDC.TB_PAR_SBC_SUB_CARPETA
                               where SBC.PK_PAR_SBC_ID == 0
                               select new SelectListItem
                               {
                                   Text = SBC.FL_PAR_SBC_NOMBRE.ToString(),
                                   Value = SBC.PK_PAR_SBC_ID.ToString()

                               };


            var oClases = new List<SelectListItem>();

            oClases.Add(new SelectListItem { Text = "manual", Value = "Manual" });
            oClases.Add(new SelectListItem { Text = "automatico", Value = "automatico" });
            oClases.Add(new SelectListItem { Text = "programado", Value = "programado" });
            oClases.Add(new SelectListItem { Text = "anular", Value = "anular" });
            oClases.Add(new SelectListItem { Text = "box", Value = "box" });
            oClases.Add(new SelectListItem { Text = "foward", Value = "foward" });
            oClases.Add(new SelectListItem { Text = "backward", Value = "backward" });
            oClases.Add(new SelectListItem { Text = "find", Value = "find" });
            oClases.Add(new SelectListItem { Text = "service", Value = "service" });
            oClases.Add(new SelectListItem { Text = "direction", Value = "direction" });
            oClases.Add(new SelectListItem { Text = "serviceclient", Value = "serviceclient" });
            oClases.Add(new SelectListItem { Text = "editar", Value = "editar" });
            oClases.Add(new SelectListItem { Text = "columna", Value = "columna" });
            oClases.Add(new SelectListItem { Text = "excel", Value = "excel" });
            oClases.Add(new SelectListItem { Text = "tiempocalendario", Value = "tiempocalendario" });
            oClases.Add(new SelectListItem { Text = "login", Value = "login" });
            oClases.Add(new SelectListItem { Text = "tractor", Value = "tractor" });
            oClases.Add(new SelectListItem { Text = "entrar", Value = "entrar" });
            oClases.Add(new SelectListItem { Text = "salir", Value = "salir" });
            oClases.Add(new SelectListItem { Text = "copy", Value = "copy" });

            ViewBag.Clases = oClases;



            var oRoles = from Rol in oDC.PR_PAR_ROL_LIST_ITEM(0)
                         select new List<BoolSetting>
                         {
                            new BoolSetting
                            {
                                DisplayName=Rol.RoleId.ToString(),
                                Value=Convert.ToBoolean(Rol.VALUE),
                                Text=Rol.RoleName.ToString()
                            }
                         };

            oModel.oListaSistema=oSistema;
            oModel.oListaCarpetasDropdown = oCarpetas;
            oModel.oListaSubCarpetasDropdown = oSubCarpetas;    //van vacias, es solo para mostrar el --seleccione--  
            oModel.oCkeckBoxRoles = oRoles;

            oModel.Tipo = 5;
            return PartialView("formularios", oModel);
        }
        #endregion

        #region Eliminar Carpeta
        [HttpPost][Authorize]
        public string EliminarCarpeta(FormCollection oForm)
        {
            var oValor=0;

            LinqBD_PARDataContext oDc = new LinqBD_PARDataContext();

            if (ValidationController.IsNumeric(oForm["carpeta_id"]))
                oValor = Convert.ToInt32(oForm["carpeta_id"]);




            oDc.PR_PAR_ELIMINAR_CARPETA(oValor);
            

            try
            {
                oDc.SubmitChanges();
                return ("Registro Eliminado");
            }
            catch (Exception e)
            {
                return ("<p>No fue posible eliminar la información</p><p>" + e.Message +"</p>");
            }
           
        }
        #endregion

        #region Eliminar Sub Carpeta
        [HttpPost]
        [Authorize]
        public string EliminarSubCarpeta(FormCollection oForm)
        {
            var oValor = 0;

            LinqBD_PARDataContext oDc = new LinqBD_PARDataContext();

            if (ValidationController.IsNumeric(oForm["subcarpeta_id"]))
                oValor = Convert.ToInt32(oForm["subcarpeta_id"]);




            var oQuery = from SBC in oDc.TB_PAR_SBC_SUB_CARPETA
                             where SBC.PK_PAR_SBC_ID==oValor
                             select SBC;

            oDc.TB_PAR_SBC_SUB_CARPETA.DeleteAllOnSubmit(oQuery);

            try
            {
                oDc.SubmitChanges();
                return ("Registro Eliminado");
            }
            catch (Exception e)
            {
                return ("<p>No fue posible eliminar la información</p><p>" + e.Message + "</p>");
            }

        }
        #endregion

        #region Agregar Carpeta
        [HttpPost][Authorize]
        public string AgregarCarpeta(AdministracionCarpeta oModel)
        {
            if (oModel != null)
            {
                LinqBD_PARDataContext oDc = new LinqBD_PARDataContext();

                TB_PAR_CAR_CARPETA oCar = new TB_PAR_CAR_CARPETA
                {
                    FL_PAR_CAR_NOMBRE = oModel.NombreCarpeta,
                    FL_PAR_CAR_ORDEN = Convert.ToInt32(oModel.Orden),
                    FL_PAR_CAR_CLASS=   oModel.Clase,
                    PK_PAR_SIS_ID=1
                };
                oDc.TB_PAR_CAR_CARPETA.InsertOnSubmit(oCar);

                try
                {
                    oDc.SubmitChanges();          
                    return("Registro Guardado");
                }
                catch (Exception e)
                {
                    return ("No fue posible guardar la información" + e.Message);
                }

            }
            else
            {
                return ("Error en el modelo");
            
            }
            
        }
        #endregion

        #region Agregar Sub Carpeta
        [HttpPost][Authorize]
        public string AgregarSubCarpeta(AdministracionCarpeta oModel)
        {
            if (oModel != null)
            {
                LinqBD_PARDataContext oDc = new LinqBD_PARDataContext();

                TB_PAR_SBC_SUB_CARPETA oCar = new TB_PAR_SBC_SUB_CARPETA
                {
                    FL_PAR_SBC_NOMBRE = oModel.NombreSubCarpeta,
                    FL_PAR_SBC_ORDEN = Convert.ToInt32(oModel.Orden),
                    FL_PAR_SBC_CLASS = oModel.Clase,
                    PK_PAR_CAR_ID = Convert.ToInt32(oModel.IdPadre)
                };
                oDc.TB_PAR_SBC_SUB_CARPETA.InsertOnSubmit(oCar);

                try
                {
                    oDc.SubmitChanges();
                    return ("Registro Guardado");
                }
                catch (Exception e)
                {
                    return ("No fue posible guardar la información" + e.Message);
                }

            }
            else
            {
                return ("Error en el modelo");

            }

        }
        #endregion

        #region Agregar Item
        public string AgregarItem(FormCollection oForm)
        {

            LinqBD_PARDataContext oDc = new LinqBD_PARDataContext();
            LinqBD_PARDataContext oDc2 = new LinqBD_PARDataContext();

            TB_PAR_LST_LISTA_ITEM oItem = new TB_PAR_LST_LISTA_ITEM();


            if (oForm["idSubCarpeta"] != null && oForm["idSubCarpeta"] != "")
                oItem.PK_PAR_SBC_ID = Convert.ToInt32(oForm["idSubCarpeta"]);
            else if (oForm["idPadre"] != null && oForm["idPadre"] != "")
                oItem.PK_PAR_CAR_ID = Convert.ToInt32(oForm["idPadre"]);




            oItem.PK_PAR_SIS_ID = Convert.ToInt32(oForm["sSistema"].ToString());
            oItem.FL_PAR_LST_NOMBRE = oForm["NombreItem"].ToString();
            oItem.FL_PAR_LST_ORDEN = Convert.ToInt32(oForm["Orden"]);
            oItem.FL_PAR_LST_CONTROLLER = oForm["Controlador"].ToString();
            oItem.FL_PAR_LST_ACTION = oForm["Accion"].ToString();
            oItem.FL_PAR_LST_CLASS = oForm["ClaseItem"].ToString();            



            var oQuery = from Rol in oDc2.PR_PAR_ROL_LIST_ITEM(0)
                         select Rol;

            var oRoles = from Rol in oDc.TB_PAR_FUN_FUNCION
                         select Rol;

            oDc.TB_PAR_LST_LISTA_ITEM.InsertOnSubmit(oItem);

            try
            {
                oDc.SubmitChanges();

                var oRoleId = "";
                var oResultadoRol = "";


                foreach (var oFile in oQuery)
                {
                    oRoleId = oFile.RoleId.ToString();
                    oResultadoRol = oForm[oRoleId][0].ToString();
                    if (oResultadoRol != null)
                    {
                        if (oResultadoRol == "t")
                        {
                            oDc2.TB_PAR_FUN_FUNCION.InsertOnSubmit(new TB_PAR_FUN_FUNCION
                            {
                                PK_PAR_LST_ID = oItem.PK_PAR_LST_ID,
                                RoleId = oFile.RoleId
                            });
                        }
                    }
                }

                try
                {
                    oDc2.SubmitChanges();
                    return ("Registro Guardado");
                }
                catch (Exception e)
                {
                    return ("No fue posible guardar los Roles" + e.Message);
                }
            }
            catch (Exception e)
            {
                return ("No fue posible guardar la información" + e.Message);
            }

        }
        #endregion

        #region Eliminar Item
        public string EliminarItem(int oitem)
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oRegistroItem=(from lst in oPar.TB_PAR_LST_LISTA_ITEM
                       where lst.PK_PAR_LST_ID==oitem
                       select lst).Single();

            var oRegistroFuncion = (from fun in oPar.TB_PAR_FUN_FUNCION
                                    where fun.PK_PAR_LST_ID == oitem
                                    select fun).Single();

            oPar.TB_PAR_LST_LISTA_ITEM.DeleteOnSubmit(oRegistroItem);
            oPar.TB_PAR_FUN_FUNCION.DeleteOnSubmit(oRegistroFuncion);

            try
            {
                oPar.SubmitChanges();
                return ("Registro Eliminado Exitosamente");
            }
            catch (Exception e)
            {
                return ("No fue posible eliminar el registro, " + e.Message);
            }
        }
        #endregion

        #region Lista Columnas
        public ActionResult ListaColumnas()
        {
            AdministracionExcel oModel = new AdministracionExcel();
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            var oQuery = from Col in oPar.TB_PAR_COL_COLUMNA
                         select Col;

            oModel.oListaColumna = oQuery;

            return View(oModel);
        }
        #endregion


        //Administración Excel

        #region Definicion Archivo Carga
        [Authorize]
        public ActionResult DefinicionArchivoCarga()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            var oQuery = from Result in oPar.PR_PAR_LISTA_COLUMNAS_SERVICIO(0)
                         orderby Result.FL_PAR_COL_NOMBRE
                         select Result;
                       


            AdministracionExcel oModel = new AdministracionExcel();

            oModel.GetListaCliente();
            oModel.GetListaServicio(0);


            var oIndice = 1;
            foreach (var oFile in oQuery)
            {
                oModel.oListaSerCol.Add(new ColumnasExcel
                    {                  
                        Id=Convert.ToInt32(oFile.PK_PAR_EXC_ID),
                        Indice=oIndice,
                        Letra = FunLetra((int) oFile.FL_PAR_EXC_LETRA),
                        Nombre = oFile.FL_PAR_COL_NOMBRE,
                        Descripcion = oFile.FL_PAR_COL_DESCRIPCION,
                        Obligatoria = FunObligatoria((bool) oFile.FL_PAR_EXC_OBLIGATORIA)
                    });
                oIndice += 1;
            }

            #region Carga Lista Letras
            var arrLetras = FunRetornaLetras();            
            var oValores = from datos in arrLetras
                           select new SelectListItem
                           {
                               Text = datos.Valor,
                               Value = datos.Id.ToString()
                           };

            oModel.oListaLetra = oValores;
            #endregion

            #region Carga Lista Columnas 


            var oDDColumnas = from datos in oPar.TB_PAR_COL_COLUMNA
                           select new SelectListItem
                           {
                               Text = datos.FL_PAR_COL_NOMBRE,
                               Value = datos.PK_PAR_COL_ID.ToString()
                           };

            oModel.oListaLetra = oValores;
            oModel.oListaColumnasforDD = oDDColumnas;

            #endregion

            #region CargaDatosObligatorios

            var oDDObligatorio = from datos in FunRetornaObligatorio()
                                 select new SelectListItem
                                 {
                                     Value = datos.Id.ToString(),
                                     Text = datos.Valor
                                 };
            oModel.oListaObligatorioforDD = oDDObligatorio;
            #endregion

            return View(oModel);
        }
        #endregion

        #region DropDownServicio
        public ActionResult DropDownServicio(string CLI_RUT)
        {
            int RutCliente = 0;
            ValidationController oValidation = new ValidationController();
            if (oValidation.IsNumeric2(CLI_RUT))
            {
                RutCliente=Convert.ToInt32(CLI_RUT);
            }
            AdministracionExcel oModel = new AdministracionExcel();
            oModel.GetListaServicio(RutCliente);
            return PartialView("_DropDownServicio", oModel);
        }
        #endregion

        #region CargaLetraServicio
        public ActionResult CargaLetraServicio(string Servicio)
        {
            int iServicio =0;
            ValidationController oValidation = new ValidationController();
            if (oValidation.IsNumeric2(Servicio)==true)
                iServicio=Convert.ToInt32(Servicio);
            AdministracionExcel oModel = new AdministracionExcel();
            oModel.GetListaLetra(iServicio);
            return PartialView("_CargaLetraServicio", oModel);
        }
        #endregion

        #region CargaColumnaServicio
        public ActionResult CargaColumnaServicio(string Servicio)
        {
            int iServicio = 0;
            ValidationController oValidation = new ValidationController();
            if (oValidation.IsNumeric2(Servicio) == true)
                iServicio = Convert.ToInt32(Servicio);
            AdministracionExcel oModel = new AdministracionExcel();
            oModel.GetListaColumna(iServicio);
            return PartialView("_CargaColumnaServicio", oModel);
        }
        #endregion

        #region Actualiza Lista Columnas Servicio
        public ActionResult ActualizaListaSerCol(AdministracionExcel oModel)
        {
            
            int oValor=0;

            if (ValidationController.IsNumeric(oModel.Servicio.ToString()))
                oValor=oModel.Servicio;

            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            var oQuery = from Result in oPar.PR_PAR_LISTA_COLUMNAS_SERVICIO(oValor)
                         select Result;

            ColumnasExcel oExcel = new ColumnasExcel();
            List<ColumnasExcel> oListaExcel = new List<ColumnasExcel>();



            var oIndice = 1;
            foreach (var oFile in oQuery)
            {
                oListaExcel.Add(new ColumnasExcel{
                    Id = Convert.ToInt32(oFile.PK_PAR_EXC_ID),
                    Indice=oIndice,
                    Letra = FunLetra(Convert.ToInt32(oFile.FL_PAR_EXC_LETRA)),
                    Nombre = oFile.FL_PAR_COL_NOMBRE,
                    Descripcion = oFile.FL_PAR_COL_DESCRIPCION,
                    Obligatoria = FunObligatoria((bool)oFile.FL_PAR_EXC_OBLIGATORIA)
                });
                oIndice += 1;
                                
            }


            #region Carga Lista Letras
            var arrLetras = FunRetornaLetras();
            var oValores = from datos in arrLetras
                           select new SelectListItem
                           {
                               Text = datos.Valor,
                               Value = datos.Id.ToString()
                           };

            oModel.oListaLetra = oValores;
            #endregion

            #region Carga Lista Columnas


            var oDDColumnas = from datos in oPar.TB_PAR_COL_COLUMNA
                              select new SelectListItem
                              {
                                  Text = datos.FL_PAR_COL_NOMBRE,
                                  Value = datos.PK_PAR_COL_ID.ToString()
                              };

            oModel.oListaLetra = oValores;
            oModel.oListaColumnasforDD = oDDColumnas;

            #endregion

            #region CargaDatosObligatorios

            var oDDObligatorio = from datos in FunRetornaObligatorio()
                                 select new SelectListItem
                                 {
                                     Value=datos.Id.ToString(),
                                     Text=datos.Valor
                                 };

            oModel.oListaObligatorioforDD = oDDObligatorio;



            #endregion

            oModel.oListaSerCol = oListaExcel;
            

            return PartialView("_CargaListaColumnasExcel",oModel);
        }
        public string FunObligatoria(bool Valor)
        {
            List<idBoolValorString> oVerdadero = new List<idBoolValorString>();

            oVerdadero.Add(new idBoolValorString { Id = true, Valor = "Sí" });
            oVerdadero.Add(new idBoolValorString { Id = false, Valor = "No" });

            foreach (var Item in oVerdadero)
            {
                if (Item.Id == Valor)
                    return Item.Valor;
            }

            return "";

        }
        public string FunLetra(int Valor)
        {
            //Entregada una int, devuelve la letra que lo representa, función utilizada en para determinar que el valor 1
            //representa la letra A (Columnas Excel)

            List<idIntValorString> oVerdadero = new List<idIntValorString>();

            oVerdadero = FunRetornaLetras();


            foreach (var Item in oVerdadero)
            {
                if (Item.Id == Valor)
                    return Item.Valor;
            }

            return "";

        }
        public List<idIntValorString> FunRetornaLetras()
        {
            List<idIntValorString> oVerdadero = new List<idIntValorString>();

            oVerdadero.Add(new idIntValorString { Id = 1, Valor = "A" });
            oVerdadero.Add(new idIntValorString { Id = 2, Valor = "B" });
            oVerdadero.Add(new idIntValorString { Id = 3, Valor = "C" });
            oVerdadero.Add(new idIntValorString { Id = 4, Valor = "D" });
            oVerdadero.Add(new idIntValorString { Id = 5, Valor = "E" });
            oVerdadero.Add(new idIntValorString { Id = 6, Valor = "F" });
            oVerdadero.Add(new idIntValorString { Id = 7, Valor = "G" });
            oVerdadero.Add(new idIntValorString { Id = 8, Valor = "H" });
            oVerdadero.Add(new idIntValorString { Id = 9, Valor = "I" });
            oVerdadero.Add(new idIntValorString { Id = 10, Valor = "J" });
            oVerdadero.Add(new idIntValorString { Id = 11, Valor = "K" });
            oVerdadero.Add(new idIntValorString { Id = 12, Valor = "L" });
            oVerdadero.Add(new idIntValorString { Id = 13, Valor = "M" });
            oVerdadero.Add(new idIntValorString { Id = 14, Valor = "N" });
            oVerdadero.Add(new idIntValorString { Id = 15, Valor = "O" });
            oVerdadero.Add(new idIntValorString { Id = 16, Valor = "P" });
            oVerdadero.Add(new idIntValorString { Id = 17, Valor = "Q" });
            oVerdadero.Add(new idIntValorString { Id = 18, Valor = "R" });
            oVerdadero.Add(new idIntValorString { Id = 19, Valor = "S" });
            oVerdadero.Add(new idIntValorString { Id = 20, Valor = "T" });
            oVerdadero.Add(new idIntValorString { Id = 21, Valor = "U" });
            oVerdadero.Add(new idIntValorString { Id = 22, Valor = "V" });
            oVerdadero.Add(new idIntValorString { Id = 23, Valor = "W" });
            oVerdadero.Add(new idIntValorString { Id = 24, Valor = "X" });
            oVerdadero.Add(new idIntValorString { Id = 25, Valor = "Y" });
            oVerdadero.Add(new idIntValorString { Id = 26, Valor = "Z" });
            oVerdadero.Add(new idIntValorString { Id = 27, Valor = "AA" });
            oVerdadero.Add(new idIntValorString { Id = 28, Valor = "AB" });


            return oVerdadero;
        }
        public List<idBoolValorString> FunRetornaObligatorio()
        {
            List<idBoolValorString> oDatos = new List<idBoolValorString>();
            oDatos.Add(new idBoolValorString { Id = true, Valor="Sí" });
            oDatos.Add(new idBoolValorString { Id = false, Valor = "No" });
            return oDatos;
        }
        #endregion

        #region Actualiza después de guardar
        public ActionResult UpdateAfterSave (string idServicioVigente)
        {
            AdministracionExcel oModel = new AdministracionExcel();
            oModel.Servicio = Convert.ToInt32(idServicioVigente);
            return ActualizaListaSerCol(oModel);
        }
        #endregion

        #region Guarda registro en tabla TB_PAR_EXC_EXCEL
        [HttpPost][Authorize]
        public string AgregarColumnaServicio(AdministracionExcel oModel, string ServicioRequerido)
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            TB_PAR_EXC_EXCEL oExcel = new TB_PAR_EXC_EXCEL();

            oExcel.FL_PAR_EXC_LETRA = Convert.ToInt32(oModel.Letra);
            oExcel.FL_PAR_EXC_OBLIGATORIA = Convert.ToBoolean(oModel.Obligatoria);
            oExcel.PK_PAR_COL_ID = Convert.ToInt32(oModel.Columna);
            oExcel.PK_PAR_SER_ID = Convert.ToInt32(ServicioRequerido);

            oPar.TB_PAR_EXC_EXCEL.InsertOnSubmit(oExcel);

            try
            {
                oPar.SubmitChanges();
                return (ErrorAgregarColumnaServicio(0));
            }
            catch (Exception e)
            {
                return (ErrorAgregarColumnaServicio(1) +"<p>"+  e.Message + "</p>");
            }
        }
        public string ErrorAgregarColumnaServicio(int oId)
        {
            switch (oId)
            {
                case 0: return ("Registro Guardado Exitosamente");
                case 1: return ("No fue posible guardar la información");
                default: return("Error desconocido");
            }
        }
        #endregion

        #region Elimina registro de la tabla TB_PAR_EXC_EXCEL
        public string EliminarColumnaExcel(string idFilaBorrar)
        {

            if (ValidationController.IsNumeric(idFilaBorrar))
            {
                var oValor=Convert.ToInt32(idFilaBorrar);
                LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

                var oElemento = (from Fila in oPar.TB_PAR_EXC_EXCEL
                                where Fila.PK_PAR_EXC_ID==oValor
                                select Fila).Single();

                oPar.TB_PAR_EXC_EXCEL.DeleteOnSubmit(oElemento);
                try
                {
                    oPar.SubmitChanges();
                    return (ErrorEliminarColumnaServicio(0));
                }
                catch (Exception e)
                {
                    return (ErrorEliminarColumnaServicio(1) + "<p>" + e.Message + "</p>");
                }
            }
            else
            {
                return (ErrorEliminarColumnaServicio(2));
            }
        }
        public string ErrorEliminarColumnaServicio(int oId)
        {
            switch (oId)
            {
                case 0: return ("Registro Eliminado Exitosamente");
                case 1: return ("No fue posible eliminar la información");
                case 2: return ("El indice de la tabla debe ser númerico");
                default: return ("Error desconocido");
            }
        }
        #endregion

        #region Mueve registro de la tabla TB_PAR_EXC_EXCE, de posición ejemplo A-->B ó B-->A (cambia la Letra)
        [Authorize][HttpPost]
        public string MueveColumnaExcel(string idServicioMover, string idFilaModificar, string idTipo)
        {        
            if (ValidationController.IsNumeric(idFilaModificar))
            {
                LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

                IQueryable<TB_PAR_EXC_EXCEL> oDatos;
                if (idTipo == "baja") //posibles valores Baja Sube definidos en un JavaScript de "_CargaListaColumnasExcel"
                {
                    oDatos = from Dat in oPar.TB_PAR_EXC_EXCEL
                             where Dat.PK_PAR_SER_ID == Convert.ToInt32(idServicioMover)
                             orderby Dat.FL_PAR_EXC_LETRA ascending
                             select Dat;
                }
                else
                {
                    oDatos = from Dat in oPar.TB_PAR_EXC_EXCEL
                             where Dat.PK_PAR_SER_ID == Convert.ToInt32(idServicioMover)
                             orderby Dat.FL_PAR_EXC_LETRA descending
                             select Dat;
                }

                var oEnc = 0;
                var oCol=0;
                var oNext = 0;
                var oCol2 = 0;
                foreach (var oFile in oDatos)
                {
                    if (oEnc == 1)
                    {
                        oCol2 = oFile.FL_PAR_EXC_LETRA;
                        oNext = Convert.ToInt32(oFile.PK_PAR_EXC_ID);
                        oFile.FL_PAR_EXC_LETRA = oCol;
                        oEnc = 0;
                        
                    }
                    if (oFile.PK_PAR_EXC_ID == Convert.ToInt32(idFilaModificar))
                    {
                        oEnc = 1;
                        oCol = oFile.FL_PAR_EXC_LETRA;                        
                    }                   
                }

                foreach (var oFile2 in oDatos)
                {
                    if (oFile2.PK_PAR_EXC_ID == Convert.ToInt32(idFilaModificar))
                    {
                        oFile2.FL_PAR_EXC_LETRA = oCol2;
                    }
                }

                try
                {
                    oPar.SubmitChanges();
                    return ("Elemento Actualizado");
                }
                catch (Exception e)
                {
                    return ("Error :" + e.Message);
                }
                                
            }
            else
	        {
                return ("Id Fila no válido");
	        }
        }
        #endregion

        #region Comuna

        public ActionResult Comuna()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            var oComunas = from com in oPar.TB_PAR_COM_COMUNA
                           join prv in oPar.TB_PAR_PRV_PROVINCIA on com.PK_PAR_PRV_ID equals prv.PK_PAR_PRV_ID
                           join reg in oPar.TB_PAR_REG_REGION on prv.PK_PAR_REG_ID equals reg.PK_PAR_REG_ID                                                                                 
                           join cla in oPar.TB_PAR_CLA_CLASIFICACION on com.PK_PAR_CLA_ID equals cla.PK_PAR_CLA_ID into joincomcla
                           from cla in joincomcla.DefaultIfEmpty()
                           orderby reg.PK_PAR_REG_ID, prv.PK_PAR_PRV_ID,com.FL_PAR_COM_NOMBRE
                           select new valoresListaComuna { 
                            comuna_id= com.PK_PAR_COM_ID,
                            comuna_nombre= com.FL_PAR_COM_NOMBRE,
                            comuna_provincia=prv.FL_PAR_PRV_NOMBRE,
                            comuna_region=reg.FL_PAR_REG_NOMBRE,
                            comuna_latitud = Convert.ToDouble(com.FL_PAR_COM_LATITUD),
                            comuna_longitud = Convert.ToDouble(com.FL_PAR_COM_LONGITUD),
                            clasificacion=cla.FL_PAR_CLA_NOMBRE
                           };

            var oSinClasificacion = from com in oPar.TB_PAR_COM_COMUNA
                           join prv in oPar.TB_PAR_PRV_PROVINCIA on com.PK_PAR_PRV_ID equals prv.PK_PAR_PRV_ID
                           join reg in oPar.TB_PAR_REG_REGION on prv.PK_PAR_REG_ID equals reg.PK_PAR_REG_ID
                           where com.PK_PAR_CLA_ID == null                           
                           orderby reg.PK_PAR_REG_ID, prv.PK_PAR_PRV_ID, com.FL_PAR_COM_NOMBRE
                           select new valoresListaComuna
                           {
                               comuna_id = com.PK_PAR_COM_ID,
                               comuna_nombre = com.FL_PAR_COM_NOMBRE,
                               comuna_provincia = prv.FL_PAR_PRV_NOMBRE,
                               comuna_region = reg.FL_PAR_REG_NOMBRE,
                               comuna_latitud = Convert.ToDouble(com.FL_PAR_COM_LATITUD),
                               comuna_longitud = Convert.ToDouble(com.FL_PAR_COM_LONGITUD),                               
                           };

            ComunaModels oModel = new ComunaModels ();
            oModel.ListaComunas = oComunas;
            oModel.ListaComunasSC = oSinClasificacion;

            return View(oModel);
        }

        private string reemplaza(string val)
        {

            var oResult = val.ToLower();
            oResult= val.Replace("á","a");
            oResult = val.Replace("é","e");
            oResult = val.Replace("í","i");
            oResult = val.Replace("ó","o");
            oResult = val.Replace("ú","u");

            return oResult;
        }

        public string  BuscarPosicion(int ComunaId)
        {

            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            var oDatos = (from com in oPar.TB_PAR_COM_COMUNA
                         join prv in oPar.TB_PAR_PRV_PROVINCIA on com.PK_PAR_PRV_ID equals prv.PK_PAR_PRV_ID
                         join reg in oPar.TB_PAR_REG_REGION on prv.PK_PAR_REG_ID equals reg.PK_PAR_REG_ID
                         join pai in oPar.TB_PAR_PAI_PAIS on reg.PK_PAR_PAI_ID equals pai.PK_PAR_PAI_ID
                         where com.PK_PAR_COM_ID == ComunaId
                         select  new { 
                            com.FL_PAR_COM_NOMBRE,
                            prv.FL_PAR_PRV_NOMBRE,
                            reg.FL_PAR_REG_NOMBRE,
                            pai.FL_PAR_PAI_NOMBRE
                         }).Single();

            string oDireccion = reemplaza(oDatos.FL_PAR_COM_NOMBRE) + "," + reemplaza(oDatos.FL_PAR_PRV_NOMBRE) + "," + reemplaza(oDatos.FL_PAR_REG_NOMBRE) + "," + reemplaza(oDatos.FL_PAR_PAI_NOMBRE);

            XDocument xDoc = XDocument.Load("http://maps.googleapis.com/maps/api/geocode/xml?address=" + oDireccion + "&sensor=false");           

            var oLine1 = xDoc.Element("GeocodeResponse");
            var oEstado = oLine1.Element("status").Value;

            var oResult = "0|0";

            if (oEstado == "OK")
            {
                var oResultado = oLine1.Element("result");
                var oGeometry = oResultado.Element("geometry");
                var oLocation = oGeometry.Element("location");
                var oLat = oLocation.Element("lat").Value;
                var oLon = oLocation.Element("lng").Value;

                oResult=oLat +"|" + oLon;                
            }
                       
            return (oResult);
        }

        #endregion

        #region Localidad
        public ActionResult Localidad()
        {
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            var oLocalidades = from loc in oPar.TB_PAR_LOC_LOCALIDAD
                           join com in oPar.TB_PAR_COM_COMUNA on loc.PK_PAR_COM_ID equals com.PK_PAR_COM_ID
                           join prv in oPar.TB_PAR_PRV_PROVINCIA on com.PK_PAR_PRV_ID equals prv.PK_PAR_PRV_ID
                           join reg in oPar.TB_PAR_REG_REGION on prv.PK_PAR_REG_ID equals reg.PK_PAR_REG_ID                                                                                 
                           orderby reg.PK_PAR_REG_ID, prv.PK_PAR_PRV_ID, com.FL_PAR_COM_NOMBRE
                           select new valoresListaLocalidades
                           {
                               id = Convert.ToInt32(loc.PK_PAR_LOC_ID),
                               nombre=loc.FL_PAR_LOC_NOMBRE,
                               comuna = com.FL_PAR_COM_NOMBRE,
                               provincia = prv.FL_PAR_PRV_NOMBRE,
                               region = reg.FL_PAR_REG_NOMBRE,
                               latitud = Convert.ToDouble(loc.FL_PAR_LOC_LATITUD),
                               longitud = Convert.ToDouble(loc.FL_PAR_LOC_LONGITUD),
                               codigo = loc.FL_PAR_LOC_CODIGO
                           };

            LocalidadModels oModel = new LocalidadModels();
            oModel.ListaLocalidades = oLocalidades;

            return View(oModel);
        }

        public string BuscarPosicionLocalidad(int LocalidadId)
        {

            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            var oDatos = (from loc in oPar.TB_PAR_LOC_LOCALIDAD
                          join com in oPar.TB_PAR_COM_COMUNA on loc.PK_PAR_COM_ID equals com.PK_PAR_COM_ID
                          join prv in oPar.TB_PAR_PRV_PROVINCIA on com.PK_PAR_PRV_ID equals prv.PK_PAR_PRV_ID
                          join reg in oPar.TB_PAR_REG_REGION on prv.PK_PAR_REG_ID equals reg.PK_PAR_REG_ID
                          join pai in oPar.TB_PAR_PAI_PAIS on reg.PK_PAR_PAI_ID equals pai.PK_PAR_PAI_ID
                          where loc.PK_PAR_LOC_ID == LocalidadId
                          select new
                          {
                              loc.FL_PAR_LOC_NOMBRE,
                              com.FL_PAR_COM_NOMBRE,
                              prv.FL_PAR_PRV_NOMBRE,
                              reg.FL_PAR_REG_NOMBRE,
                              pai.FL_PAR_PAI_NOMBRE
                          }).Single();

            string oDireccion = reemplaza(oDatos.FL_PAR_LOC_NOMBRE) +","+ reemplaza(oDatos.FL_PAR_COM_NOMBRE) + "," + reemplaza(oDatos.FL_PAR_PRV_NOMBRE) + "," + reemplaza(oDatos.FL_PAR_REG_NOMBRE) + "," + reemplaza(oDatos.FL_PAR_PAI_NOMBRE);

            XDocument xDoc = XDocument.Load("http://maps.googleapis.com/maps/api/geocode/xml?address=" + oDireccion + "&sensor=false");

            var oLine1 = xDoc.Element("GeocodeResponse");
            var oEstado = oLine1.Element("status").Value;

            var oResult = "0|0";

            if (oEstado == "OK")
            {
                var oResultado = oLine1.Element("result");
                var oGeometry = oResultado.Element("geometry");
                var oLocation = oGeometry.Element("location");
                var oLat = oLocation.Element("lat").Value;
                var oLon = oLocation.Element("lng").Value;

                oResult = oLat + "|" + oLon;
            }

            return (oResult);
        }

        #endregion

        #region ListaLocalidades
        public ActionResult ListaLocalidades(int idComuna)
        {
            LocalidadModels oModel = new LocalidadModels();
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            var oLocalidades = from loc in oPar.TB_PAR_LOC_LOCALIDAD
                               where loc.PK_PAR_COM_ID == idComuna
                               select new valoresListaLocalidades
                               {
                                   nombre=loc.FL_PAR_LOC_NOMBRE,
                                   id=Convert.ToInt32(loc.PK_PAR_LOC_ID)
                               };

            oModel.ListaLocalidades = oLocalidades;

            return PartialView("_ListaLocalidades",oModel);
        }
        #endregion

        #region UpdateRolFuncion
        [HttpPost]
        public string UpdateRolFuncion(AdministracionCarpeta model, FormCollection oForm)
        {
            try
            {                
                LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
                LinqBD_SEGDataContext oSeg = new LinqBD_SEGDataContext();

                var oRoles = from m in oPar.TB_PAR_FUN_FUNCION
                             where m.PK_PAR_LST_ID== model.IdItem
                             select m;

                oPar.TB_PAR_FUN_FUNCION.DeleteAllOnSubmit(oRoles);//Borro para después insertar

                List<TB_PAR_FUN_FUNCION> bdRoles = new List<TB_PAR_FUN_FUNCION>();
                
                var oRolesExistentes = from rol in oSeg.aspnet_Roles select rol;

                string oResult;
                foreach (var oFile in oRolesExistentes)
                {
                    oResult = oForm[oFile.RoleId.ToString()];
                    if (oResult == "true,false")
                    {
                        bdRoles.Add(new TB_PAR_FUN_FUNCION { PK_PAR_LST_ID = model.IdItem,RoleId=oFile.RoleId });
                    }
                }

                oPar.TB_PAR_FUN_FUNCION.InsertAllOnSubmit(bdRoles);

                oPar.SubmitChanges();

                return ("<p>La información fue actualizada exitosamente!</p>");
            }

            catch (Exception e)
            {
                return ("No fue posible actualizar los Roles " + e.Message);
            }
        }
        #endregion

        #region EditarRolFuncion
        public ActionResult EditarRolFuncion(int oItem)
        {

            AdministracionCarpeta oModel = new AdministracionCarpeta();

            LinqBD_SEGDataContext oSeg = new LinqBD_SEGDataContext();
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();

            var oRoles = from rol in oSeg.aspnet_Roles
                         select new
                         {
                             rol.RoleId,
                             rol.RoleName
                         };

            var oRolesItem = from m in oPar.TB_PAR_FUN_FUNCION
                             where m.PK_PAR_LST_ID==oItem
                             select m;
            List<BoolSetting> oListaCheck = new List<BoolSetting>();
            bool oExiste = false;
            foreach (var oFila in oRoles)
            {
                oExiste = false;
                foreach (var oF in oRolesItem)
                {
                    if (oF.RoleId == oFila.RoleId)
                        oExiste = true;
                }


                oListaCheck.Add(new BoolSetting { Text = oFila.RoleId.ToString(), Value = oExiste, DisplayName = oFila.RoleName });

            }

            oModel.IdItem = oItem;
            oModel.Roles = oListaCheck;

            return PartialView("_EditarRolFuncion", oModel);
        }
        #endregion


        //CAMBIA ESTADO BULTO

        #region CambiaEstadoBulto

        public ActionResult CambiaEstadoBulto()
        {
            CambiaEstadoBultoModels oModel = new CambiaEstadoBultoModels();
            return View(oModel);
        }
        [HttpPost]
        public ActionResult BuscarBulto(CambiaEstadoBultoModels oModel)
        {
            ValidationController oValidation = new ValidationController();
            var oResult = oValidation.TransformCodigoGenericoOTPOTD(oModel.OT);
            oModel.GetDatosAnular(oResult.OTP, oResult.OTD);
            oModel.GetBultos(oResult.OTP, oResult.OTD);
            oModel.GetReferencias(Convert.ToInt32(oModel.DatosAnular.PK_PAR_SER_ID));
            oModel.GetListaESA();
            oModel.GetListaEEX();

            return PartialView("_CargaDatosBulto", oModel);
        }
        public ActionResult FormCambiaEstadoBulto(CambiaEstadoBultoModels oModel)
        {
            oModel.GetListaEstado();
            return PartialView("_CambiaEstadoBulto",oModel);
        }
        public ActionResult FormRollBack(CambiaEstadoBultoModels oModel)
        {
            oModel.GetListaEstadoRoll();
            oModel.GetListaSucursal();
            return PartialView("_CambiaEstadoRollBack", oModel);
        }
        public class ResultadoGuardar
        {
            public bool Ok { get; set; }
            public string Error { get; set; }
            public int Valor { get; set; }
        }
        public ActionResult GuardaCambiaBulto(CambiaEstadoBultoModels oModel)
        {
            ResultadoGuardar oResult = new ResultadoGuardar();
            ValidationController oValidation = new ValidationController();
            try
            {  
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                int suc = 0;
                
                var cSuc = (from uab in oFol.TB_FOL_UAB_UBICACION_ACTUAL_BULTO
                            where uab.PK_FOL_BLT_ID == oModel.Bulto
                            select uab).Single();

                suc = cSuc.PK_PAR_SUC_ID;

                if (oModel.Sucursal != null && oModel.Sucursal != string.Empty)
                {
                    suc = Convert.ToInt32(oModel.Sucursal);                   
                }

                if (suc == 0)
                {
                    suc = oValidation.GetSucursalIDfromActiveUser();
                }

                var oDatos = (from blt in oFol.TB_FOL_BLT_BULTO
                              where blt.PK_FOL_BLT_ID == oModel.Bulto
                              select blt).Single();

                oDatos.PK_FOL_EST_ID = Convert.ToInt32(oModel.Estado);
                oDatos.PK_PAR_SUC_ID = suc;
                oDatos.PK_PAR_USU_RUT = oValidation.GetRutActiveUser();

                oFol.SubmitChanges();
                oResult.Ok = true;
                oResult.Error = "<p>Información guardada exitosamente</p>";
            }
            catch (Exception e)
            {
                oResult.Ok = false;
                oResult.Error = "<p>No fue posible guardar la información</p>" + e.Message;
            }
            return Json(oResult);
        }
        #endregion

        //REENVIAR PEDIDO
        #region ReenviarPedido
        public ActionResult ReenviarPedido()
        {
            ReenviarPedidoModels oModel = new ReenviarPedidoModels();
            oModel.GetDatosOTD_DATOS();
            List<SelectListItem> oLista = new List<SelectListItem>();
            oModel.oListaServicios = oLista;
            oModel.ListaTipoReferencia = oLista;
            return View(oModel);
        }
        #endregion
        #region RP001_BuscarDatosOT
        public ActionResult RP001_BuscarDatosOT(ReenviarPedidoModels oModel)
        {
            RetornoModels oRetorno = new RetornoModels();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            RetornoModels oRetono = new RetornoModels();

            oRetorno=oModel.ValidarRutCliente();
            if (oRetorno.Ok == true)
            {
                oRetorno=oModel.ValidarTipoReferencia();
            }

            if (oRetorno.Ok == true)
            {
                oRetorno = oModel.ValidarValorReferencia();
            }


            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region RP001_ActualizaLista
        public ActionResult RP001_ActualizaLista(ReenviarPedidoModels oModel)
        {
            oModel.R001_GetDatosOTD_DATOS();
            return PartialView("RP001_ActualizaLista", oModel);
        }
        #endregion
        #region RP001_AplicarQuitarDevolucion
        public ActionResult RP001_AplicarQuitarDevolucion(decimal OTP, decimal OTD)
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                var oDatos = (from otd in oFol.TB_FOL_OTD_OT_DETALLE
                              where otd.PK_FOL_OTP_ID == OTP &&
                              otd.PK_FOL_OTD_ID == OTD
                              select otd).Single();

                oDatos.FL_FOL_OTD_DEVOLUCION = false;
                oFol.SubmitChanges();
                oRetorno.Ok = true;
            }
            catch (Exception e)
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = "No fue posible modificar la información " + e.Message;
            }
            return Json(oRetorno,JsonRequestBehavior.AllowGet);
        }
        #endregion

        //CREAT OT MANUAL
        #region CrearOT
        public ActionResult CrearOT()
        {
            CrearOTManualModels oModel = new CrearOTManualModels();
            oModel.GetListaCantidad();
            oModel.GetListaOMA();
            return View(oModel);
        }
        public ActionResult GuardarIngreso(CrearOTManualModels oModel)
        {
            var oResult=oModel.Guardar();
            return Json(oResult);
        }
        #endregion

        //ELIMINAR ESTADO
        #region EliminarEstado
        public ActionResult EliminarEstado()
        {
            return View();
        }        
        #endregion
        #region ReversarEstado
        [HttpPost]
        public ActionResult ReversarEstado(EliminarEstadoModels oModel)
        {
            ValidationController oValidation = new ValidationController();
            var oOT = oValidation.TransformCodigoGenericoOTPOTD(oModel.OT);
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            RetornoModels oResult = new RetornoModels();
            try
            {
                oFol.PR_FOL_OTD_REGRESAR_ESTADO(oOT.OTP, oOT.OTD);
                oResult.Ok = true;                
            }
            catch (Exception e)
            {
                oResult.Ok = false;
                oResult.Mensaje = string.Format("No fue posible guardar la información: <p>{0}</p>", e.Message);
            }

            return Json(oResult);
        }
        #endregion


        
        #region SucursalesUsuario
        public ActionResult SucursalesUsuario(string UserName)
        {
            
            ValidationController oValidation = new ValidationController();


            SucursalesUsuarioModels oModel = new SucursalesUsuarioModels();


            oModel.Rut= Convert.ToInt32(UserName.Substring(0, UserName.Length - 2));
            oModel.RutVE = oModel.Rut;
            oModel.GetListaSucursales();
            oModel.GetListaSucursalTipoUsr();
            oModel.GetListaTipoSucursal();
            oModel.GetListaCliente();


            return PartialView("SucursalesUsuario",oModel);
        }
        #endregion
        #region UpdListaCliente
        public ActionResult UpdListaCliente(SucursalesUsuarioModels oModel)
        {
            oModel.GetListaCliente();           
            return PartialView("_UpdListaCliente",oModel);
        }
        #endregion
        #region UpdListaSucursal
        public ActionResult UpdListaSucursal(SucursalesUsuarioModels oModel)
        {
            oModel.GetListaSucursalTipoUsr();
            return PartialView("_UpdListaServicios", oModel);
        }
        #endregion
        #region GuardaIngresoSucursal
        public ActionResult GuardaIngresoSucursal(SucursalesUsuarioModels oModel)
        {
            RetornoModels oRetono = new RetornoModels();
            if (oModel.Sucursal == null || oModel.Sucursal == "")
            {
                oRetono.Mensaje = "Debe seleccionar una Sucursal";
                oRetono.Ok = false;
            }
            else
            {
                oModel.Rut = oModel.RutVE;
                oModel.GetListaSucursales();
                int oSuc = Convert.ToInt32(oModel.Sucursal);
                bool TieneSucursal = false;
                foreach (var oFila in oModel.oListaSucursales)
                {
                    if (oFila.PK_PAR_SUC_ID == oSuc)
                    {
                        TieneSucursal = true;
                    }
                }
                if (TieneSucursal == true)
                {
                    oRetono.Ok = false;
                    oRetono.Mensaje = "La sucursal ya se encuentra ingresada para el usuario";
                }
                else
                {
                    try
                    {
                        LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
                        var oRes = oPar.PR_PAR_INSERT_SUS(oSuc, oModel.RutVE);
                        oRetono.Ok = true;
                    }
                    catch (Exception e)
                    {
                        oRetono.Ok = false;
                        oRetono.Mensaje = "<p>No fue posible almacenar la información</p>" + e.Message;
                    }
                }
            }

            return Json(oRetono, JsonRequestBehavior.AllowGet);

        }
        #endregion
        #region EliminaSucursalUsuario
        public ActionResult EliminaSucursalUsuario(SucursalesUsuarioModels oModel)
        {
            oModel.GetListaSucursales();
            RetornoModels oRetorno = new RetornoModels();
            LinqBD_PARDataContext oPar = new LinqBD_PARDataContext();
            if (oModel.oListaSucursales.Count() > 1)
            {
                try
                {
                    var oResult = oPar.PR_PAR_SUS_ELIMINA(Convert.ToInt32(oModel.Sucursal), oModel.Rut);
                    oModel.GetListaSucursales();
                    var oResult2= oPar.PR_PAR_CAMBIA_SUC_USUARIO(oModel.Rut, oModel.oListaSucursales.ToList()[0].PK_PAR_SUC_ID);
                    oRetorno.Ok = true;                    
                }
                catch (Exception e)
                {
                    oRetorno.Ok = false;
                    oRetorno.Mensaje="No fue posible eliminar la sucursal " + e.Message;
                }
            }            
            else
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = "No puede eliminar todas las sucursales de un usuario, debe tener al menos 1";
            }
            return Json(oRetorno,JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region CambiaVia
        public ActionResult CambiarVia(string sOTP, string sOTD)
        {
            decimal OTP = Convert.ToDecimal(sOTP);
            decimal OTD = Convert.ToDecimal(sOTD);

            RetornoModels oRetorno = new RetornoModels();

            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();

            try
            {
                oFol.PR_FOL_CAMBIA_VIA_DEN(OTP, OTD);
                oRetorno.Ok = true;
            }
            catch (Exception e)
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = "No fue posible cambiar la Vía" + e.Message;
            }
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }
        #endregion



        #region AdministrarOT
        [Authorize]
        public ActionResult AdministrarOT()
        {
            return View("CambiaDireccion");
        }
        #endregion
        #region BuscarOTCambioDireccion
        public ActionResult BuscarOTCambioDireccion(CambiaDireccionModels oModel)
        {
            ValidationController oValidation = new ValidationController();
            var oResult=oValidation.TransformCodigoGenericoOTPOTD(oModel.OT);
            return Json(oResult, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region OpcionesCambioDireccion
        public ActionResult OpcionesCambioDireccion()
        {
            RecepcionOKManualModels oModel = new RecepcionOKManualModels();
            ValidationController oValidation = new ValidationController();
            oModel.PuedeModificarDireccion = oValidation.TieneRolByName("Puede modificar dirección");
            oModel.PuedeRecibirRechazarManual = oValidation.TieneRolByName("Puede Recibir/Rechazar Manual");
            oModel.PuedeSiniestrarOT = oValidation.TieneRolByName("Puede siniestrar OT");
            oModel.PuedeCambiarServicio = oValidation.TieneRolByName("Puede cambiar servicio");
            oModel.PuedeModificarObservacion = oValidation.TieneRolByName("Puede modificar observación");
            oModel.PuedeRectificarUbicaciones = oValidation.TieneRolByName("Rectifica Ubicaciones");
            return PartialView("OpcionesCambioDireccion",oModel); 
        }
        #endregion
        #region FormularioCambiaDireccion
        public ActionResult FormularioCambiaDireccion(FormularioCambiaDireccionModels oModel)
        {
            oModel.GetDatosDireccion();           
            ValidationController oValidation = new ValidationController();
            if (oValidation.IsNumeric2(oModel.Region))
                oModel.GetDatosComuna(Convert.ToInt32(oModel.Region));
            else
                oModel.GetDatosComuna(0);
            if (oValidation.IsNumeric2(oModel.Comuna))
                oModel.GetDatosLocalidad(Convert.ToInt32(oModel.Comuna));
            else
                oModel.GetDatosLocalidad(0);

            oModel.GetDatosRegion();
            oModel.GetDatosVia();
            return PartialView("_FormularioIngresoDireccion",oModel);
        }
        #endregion

        #region GuardarDireccion
        public ActionResult GuardarDireccion(FormularioCambiaDireccionModels oModel)
        {
            RetornoModels oRetorno = new RetornoModels();
            ValidationController oValidation = new ValidationController();

            decimal decDEN;
            int? intComuna=null,intLocalidad=null, intVia=null;

            oRetorno.Ok = true;

            if (oValidation.IsNumeric2(oModel.Comuna))
                intComuna = Convert.ToInt32(oModel.Comuna);
            else
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = "Debe seleccionar una Comuna";
            }

            if (oValidation.IsNumeric2(oModel.Localidad))
                intLocalidad = Convert.ToInt32(oModel.Localidad);

            if (oValidation.IsNumeric2(oModel.Via))
                intVia = Convert.ToInt32(oModel.Via);
            else
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = "Debe seleccionar una Vía";
            }

            if (oValidation.isDecimal(oModel.DEN_ID.ToString()))
                decDEN = Convert.ToInt32(oModel.DEN_ID);
            else
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = "Ocurrio un error";
            }

            if (oRetorno.Ok == true)
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                var oDEN = (from den in oFol.TB_FOL_DEN_DIRECCION_ENTREGA
                            where den.PK_FOL_DEN_ID == oModel.DEN_ID
                            select den).Single();
                var oCantidad = from den in oFol.TB_FOL_DEN_DIRECCION_ENTREGA
                                where den.PK_FOL_OTP_ID == oDEN.PK_FOL_OTP_ID
                                && den.PK_FOL_OTD_ID == oDEN.PK_FOL_OTD_ID
                                select den;
                if (oCantidad.Count() == 1 && Convert.ToInt32(oModel.TDE_ID) == 2)
                {
                    oRetorno.Ok = false;
                    oRetorno.Mensaje = "Solo existe una dirección de entrega, ingrese una nueva dirección de devolución en el módulo Operación > Devolución";
                }
            }

            if (oRetorno.Ok == true)
            {
                try
                {
                    LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                    var oDEN = (from den in oFol.TB_FOL_DEN_DIRECCION_ENTREGA
                                where den.PK_FOL_DEN_ID == oModel.DEN_ID
                                select den).Single();

                    oDEN.PK_PAR_COM_ID = intComuna;
                    oDEN.PK_PAR_LOC_ID = intLocalidad;
                    oDEN.PK_PAR_VIA_ID = intVia;
                    oDEN.PK_PAR_USU_RUT = oValidation.GetRutActiveUser();
                    oDEN.FL_FOL_DEN_DIRECCION = oModel.Direccion;
                    oDEN.FL_FOL_DEN_NUMERO = oModel.Numero2;
                    oDEN.PK_FOL_TDE_ID = Convert.ToInt32(oModel.TDE_ID);

                    oFol.SubmitChanges();
                    oRetorno.Ok = true;
                }
                catch (Exception e)
                {
                    oRetorno.Ok = false;
                    oRetorno.Mensaje = string.Format("Ha ocurrido un error {0}", e.Message);
                }
            }
            return Json(oRetorno);
        }
        #endregion
        #region GuardarSiniestrarOT
        [HttpPost]
        public ActionResult GuardarSiniestrarOT(string OT)
        {
            ValidationController oValidation = new ValidationController();
            RetornoModels oRetorno = new RetornoModels();
            var oDatosOT = oValidation.TransformCodigoGenericoOTPOTD(OT);
            if (oDatosOT.Error == 0)
            {
                try
                {

                    LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                    oFol.PR_FOL_OTD_SINIESTRAR(oDatosOT.OTP, oDatosOT.OTD, oValidation.GetRutActiveUser());
                    oRetorno.Ok = true;
                }
                catch (Exception e)
                {
                    oRetorno.Ok = false;
                    oRetorno.Mensaje = string.Format("No fue posible guardar la información:{0}", e.Message);
                }

            }
            else
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = oDatosOT.ErrorMensaje;
            }
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region FormularioRecepcionOK
        public ActionResult FormularioRecepcionOK(RecepcionOKManualModels oModel)
        {
            ValidationController oValidation = new ValidationController();
            oModel.GetHora();
            var oDatosOT = oValidation.TransformCodigoGenericoOTPOTD(oModel.ROK_OT);
            if (oDatosOT.Error==0)
            {
                oModel.OTP=oDatosOT.OTP;
                oModel.OTD=oDatosOT.OTD;

                oModel.FOTP = oDatosOT.OTP;
                oModel.FOTD = oDatosOT.OTD;

                oModel.GetDiasDiferencia();
                oModel.GetListaEstado();
                oModel.GetListaObservaciones();
                return PartialView("FormularioRecepcionOK", oModel);
            }
            else
            {
                return PartialView("FormularioRecepcionOKError", oModel);
            }
        }
        #endregion

        #region ActualizaListaObservaciones
        public ActionResult ActualizaListaObservaciones(RecepcionOKManualModels oModel)
        {
            oModel.GetListaObservaciones();
            return PartialView("_ActualizaListaObservacion", oModel);
        }
        #endregion
        #region GuardaCambioDireccion
        [Authorize]
        public ActionResult GuardaCambioDireccion(RecepcionOKManualModels oModel)
        {
            RetornoModels oRetorno = new RetornoModels();
            ValidationController oValidation = new ValidationController();
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            if (ModelState.IsValid)
            {
                var FechaFormato = oModel.FechaRecepcionOK.Substring(6, 4) + oModel.FechaRecepcionOK.Substring(3, 2) + oModel.FechaRecepcionOK.Substring(0, 2);
                try
                {
                    oFol.PR_FOL_OTD_RECEPCION_MANUAL(
                         oModel.FOTP
                        , oModel.FOTD
                        , oValidation.GetRutActiveUser()
                        , Convert.ToInt32(oModel.Observacion)
                        , Convert.ToInt32(oModel.Estado)
                        , FechaFormato
                        , oModel.Nombre
                        , oModel.Rut
                        , oModel.HoraRecepcion + ":" + oModel.MinutoRecepcion
                        );
                    oRetorno.Ok = true;
                }
                catch (Exception e)
                {
                    oRetorno.Ok = false;
                    oRetorno.Mensaje = string.Format("No fue posible guardar la información, {0}", e.Message);
                }
            }
            else
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = "El formulario tiene errores de validación";
            }
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region AdministrarBultos
        public ActionResult AdministrarBultos(AdministracionBultoModels oModel)
        {
            oModel.GetListaBultos();
            ValidationController oValidation = new ValidationController();
            oModel.PuedeReImprimirEtiqueta = oValidation.TieneRolByName("Puede re-imprimir etiqueta");
            oModel.PuedeModificarPesoBulto = oValidation.TieneRolByName("Puede modificar peso bulto");
            return PartialView("_ListaBultos", oModel);
        }
        #endregion

        #region FormularioPesoBulto
        [HttpPost]
        public ActionResult FormularioPesoBulto(ModificaPesoBultoModels oModel)
        {
            oModel.GetKGBulto();

            return PartialView("_FormularioPesoBulto",oModel);
        }
        #endregion

        [HttpPost]
        #region GuardarCambioPeso
        public ActionResult GuardarCambioPeso(ModificaPesoBultoModels oModel)
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {
                oModel.GuardaCambios();
                oRetorno.Ok = true;
            }
            catch (Exception ex)
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = "No fue posible guardar la información " + ex.Message;
            }
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }
        #endregion

        //Enviar COrreos
        #region FormularioCorreo
        public ActionResult FormularioCorreo(string Rut)
        {
                      

            CorreoModels oModel = new CorreoModels();
            oModel.RutCorreo = Convert.ToInt32(Rut.Substring(0, Rut.Length - 2));

            oModel.GetListaCorreo();
            oModel.GetListaEmpresas();
            oModel.GetListaServicios();
            oModel.GetListaEstados();
            return PartialView("_FormularioCorreo",oModel);
        }
        #endregion

        #region GuardaFormularioCorreo
        public ActionResult GuardaFormularioCorreo(CorreoModels oModel)
        {
            RetornoModels oRetorno = new RetornoModels();
            if (ModelState.IsValid)
            {
                ValidationController oValidation = new ValidationController();
                int oServicio = 0;
                int oEstado = 0;
                int oRut = 0 ;

                if (!oValidation.IsNumeric2(oModel.RutCorreo.ToString()) || !oValidation.IsNumeric2(oModel.SUE_SER_ID) || !oValidation.IsNumeric2(oModel.SUE_EST_ID))
                {
                    oRetorno.Ok=false;
                    oRetorno.Mensaje="Faltan Datos";
                }
                else
                {
                    try
                    {
                        oRut = Convert.ToInt32(oModel.RutCorreo);
                        oServicio = Convert.ToInt32(oModel.SUE_SER_ID);
                        oEstado = Convert.ToInt32(oModel.SUE_EST_ID);

                        LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                        oFol.PR_FOL_SUE_INSERT(oServicio, oEstado, oRut);

                        oRetorno.OTD = oRut;
                        oRetorno.Ok = true;
                    }
                    catch (Exception e)
                    {
                        oRetorno.Ok = false;
                        oRetorno.Mensaje=string.Format("Se ha producido un error al guardar la información:{0}",e.Message);
                    }
                }
            }
            else
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = "El formulario tiene errores de validación";
            }
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region EliminarCorreoUsuario
        public ActionResult EliminarCorreoUsuario(CorreoModels oModel)
        {

            int oServicio = 0;
            int oEstado = 0;
            int oRut = 0;

            RetornoModels oRetorno = new RetornoModels();

            try
            {
                oRut = Convert.ToInt32(oModel.RutCorreo);
                oServicio = Convert.ToInt32(oModel.SUE_SER_ID);
                oEstado = Convert.ToInt32(oModel.SUE_EST_ID);
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                oFol.PR_FOL_SUE_DELETE(oServicio, oEstado, oRut);

                oRetorno.OTD = oRut;
                oRetorno.Ok = true;
            }
            catch (Exception e)
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = string.Format("No fue posible eliminar el registro {0}",e.Message);
            }
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region FormularioCS
        public ActionResult FormularioCS(FormularioCSModels oModel)
        {
            oModel.GetListaServicios();
            oModel.csOTP = oModel.OTP;
            return PartialView("_FormularioCS", oModel);
        }
        #endregion
        #region FormularioOBS
        public ActionResult FormularioOBS(FormularioOBSModels oModel)
        {
            oModel.GetDatosObservacion();            
            return PartialView("_FormularioOBS", oModel);
        }
        #endregion

        #region FormularioRectifica
        public ActionResult FormularioRectifica(decimal OTP, decimal OTD)
        {
            BuscarOTModels oModel = new BuscarOTModels();
            oModel.OTP = OTP;
            oModel.OTD = OTD;
            oModel.GetListaEntrega(OTP, OTD);
            return PartialView("_FormularioRectifica", oModel);
        }
        #endregion

        #region GuardarOBS
        [Authorize]
        public ActionResult GuardarOBS(FormularioOBSModels oModel)
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                oFol.PR_FOL_OCA_MODIFICA_INSERTA(oModel.obsOTP, oModel.obsOTD, oModel.obsObservacion);
                oRetorno.Ok = true;
            }
            catch (Exception e)
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = "No fue posible cambiar la observación " + e.Message;
            }
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region GuardaCambioDireccion
        [Authorize]
        public ActionResult GuardarCS(FormularioCSModels oModel)
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                var oOTP = (from otp in oFol.TB_FOL_OTP_OT_PRINCIPAL
                            where otp.PK_FOL_OTP_ID == oModel.csOTP
                            select otp).Single();
                oOTP.PK_PAR_SER_ID = Convert.ToInt32(oModel.Servicio);
                oFol.SubmitChanges();
                oRetorno.Ok = true;
            }
            catch (Exception e)
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = "No fue posible cambiar el servicio " + e.Message;
            }
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region fnBloquea
        public ActionResult fnBloquea(AdministracionUsuario oModel)
        {
            RetornoModels oRetorno = new RetornoModels();
            try
            {
                LinqBD_PARDataContext oBPR = new LinqBD_PARDataContext();
                oBPR.PR_PAR_MAN_USU_BLOQUEA(oModel.USU_RUT, oModel.BLOQUEA);
                oRetorno.Ok = true;
            }
            catch (Exception Ex)
            {
                oRetorno.Ok = false;
                oRetorno.Mensaje = "No fue posible actualizar la información, " + Ex.Message;
            }            
            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }
        #endregion        


        //RESTRICCIONES
        #region _FormularioRestricciones
        public ActionResult _FormularioRestricciones(RestriccionesModels oModel)
        {
            oModel.GetListaEmpresas();
            oModel.ListaServicios = new List<SelectListItem>();            
            return PartialView("Restriccion/_FormularioRestricciones",oModel);
        }
        #endregion
        #region _ListaRestricciones
        public ActionResult _ListaRestricciones(RestriccionesModels oModel)
        {
            oModel.GetListaRestricciones();
            return PartialView("Restriccion/_ListaRestricciones", oModel);
        }
        #endregion
        #region _GuardaRestriccion
        public ActionResult _GuardaRestriccion(RestriccionesModels oModel)
        {
            RetornoModels oResultado = new RetornoModels();
            oResultado = oModel.GuardaRestriccion();
            return Json(oResultado,JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region _EliminaRestriccion
        public ActionResult _EliminaRestriccion(RestriccionesModels oModel)
        {
            RetornoModels oResultado = new RetornoModels();
            oResultado = oModel.EliminaRestriccion();
            return Json(oResultado, JsonRequestBehavior.AllowGet);
        }
        #endregion

        [Authorize]
        public ActionResult EditaEntregaUbicacion(string ent_id)
        {
            
            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var miRegistro = oFol.PR_FOL_UBICACION_POR_ENTREGA(Decimal.Parse(ent_id)).FirstOrDefault();

            RetornoDireccionEntregaModels miRetorno = new RetornoDireccionEntregaModels(miRegistro.LAT_OT, miRegistro.LNG_OT, miRegistro.LAT_ENTREGA, miRegistro.LNG_ENTREGA, miRegistro.ENT_ID.ToString());
            return PartialView("_EditaDireccionEntrega", miRetorno);//agregar model o clase para pasar a vista
        }

        [Authorize]
        public ActionResult ActualizaEntrega(string EntId, string Lat, string Lng)
        {
            RetornoBoolModels oRetorno = new RetornoBoolModels();
            oRetorno.Ok = true;
            oRetorno.Mensaje = "Datos Actualizados!";

            try
            {
                LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
                oFol.PR_FOL_RECTIFICA_UBICACION( decimal.Parse(EntId), Lat, Lng);

            }
            catch (Exception e)
            {
                oRetorno.Ok = true;
                oRetorno.Mensaje = "Error: " + e.Message;
                throw;
            }

            return Json(oRetorno, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult VerMapa(string ent)
        {

            LinqBD_FOLDataContext oFol = new LinqBD_FOLDataContext();
            var miOT = oFol.PR_FOL_LOCALIZACION_ENTREGA_SIMPLE(decimal.Parse(ent)).Single();
            MapOtModels.RetornoDireccion miDireccion = new MapOtModels.RetornoDireccion(miOT.OT, miOT.DIRECCION, miOT.NUMERO, miOT.LATITUD.ToString(), miOT.LONGITUD.ToString());

            return PartialView("_MapLocation", miDireccion);//agregar model o clase para pasar a vista
        }

    }
}
