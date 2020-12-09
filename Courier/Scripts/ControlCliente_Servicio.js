function ChangeRutCliente(Rut) {
    $("#divservicio").hide();
    $("#imgCargandoServicio").show();
    var randomnumber = Math.floor(Math.random() * 11111111);
    $("#Servicio").load('@Url.Action("GetListaServicios", "Generico")?r=' + randomnumber + '&RutEmpresa=' + Rut + ' #Servicio option',
                function (response, status, xhr) {
                    $("#imgCargandoServicio").hide();
                    $("#divservicio").show();
                    if (status == 'error') {
                        alert("No fue obtener los servicios del cliente");
                    }
                });
}