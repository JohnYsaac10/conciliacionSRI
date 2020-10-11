<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConciliacionSRI.aspx.cs" Inherits="SRIconciliacion.Views.ConciliacionSRI" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>ConciliaciónSRI</title>

    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" 
    integrity="sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u" crossorigin="anonymous">

   <%-- <link href="../Assets/bootstrap-datetimepicker/css/bootstrap-datetimepicker.min.css" rel="stylesheet" />--%>

    <style>
        .tableFixHead          { overflow-y: auto; height: 450px; }
        .tableFixHead thead th { position: sticky; top: 0; }

        /* Just common table stuff. Really. */
        table  { border-collapse: collapse; width: 100%; }
        th, td { padding: 8px 16px; }
        th     { background:#eee; }
    </style>

    <script
            src="https://code.jquery.com/jquery-2.2.4.min.js"
            integrity="sha256-BbhdlvQf/xTY9gja0Dq3HiwQF8LaCRTXxZKRutelT44="
            crossorigin="anonymous"></script>

</head>
<body>
    <div class="container" style="margin-top: 3rem;">
        <!-- first row -->
                        <!-- first row -->
        <div class="row">
          <div class="col-sm-8">
            <div class="row">
                <div class="col-sm-6">
                    <div class="form-group row">

                        <label for="divDiaFin" class="col-sm-3 col-form-label">FECHA:</label>
                        <div class="col-sm-9">
                          <div id="divDiaFin" class="input-group date form_date_day_ini">
                            <input id="datDiaIniShow" class="form-control inputDate" size="16" type="text">
                            <span class="input-group-addon">
                                <span class="fa fa-calendar"></span>
                            </span>
                        </div>
                        <input type="hidden" id="datDiaIni"/>
                        </div>

                        
                    </div>
                </div>
                <div class="col-sm-6">
                    <div class="form-group row">
                        <label for="select_servicio" class="col-sm-4 col-form-label">SERVICIO:</label>
                        <div class="col-sm-8">
                            <select class="form-control" id="select_servicio">
                                <option selected="selected" value="">TODOS</option>
                            </select>
                        </div>
                    </div>
                </div>
            </div>
          </div>
          <div class="col-sm-4">
              <div class="row">
                <button type="button" id="btn_consultar" class="btn btn-warning btn-sm">CONSULTAR</button>
              </div>
          </div>
        </div>

        <!-- second row -->
        <div class="row">
            <div class="col-sm-8">
                <div class="row">
                    <div class="col-sm-6">

                    </div>
                    <div class="col-sm-6">
                        <div class="form-group row">
                            <label for="select_institucion" class="col-sm-4 col-form-label">INSTITUCIÓN:</label>
                            <div class="col-sm-8">
                                <select class="form-control" id="select_institucion">
                                    <option selected="selected" value="">TODOS</option>
                                </select>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-sm-4">
                <div class="row">
                    <button type="button" id="btn_procesar" class="btn btn-success btn-sm">PROCESAR</button>
                    <div class="form-check" style="display: inline-block; margin-left: 2rem;">
                        <input class="form-check-input" type="checkbox" value="" id="ENVIO_CORREO">
                        <label class="form-check-label" for="ENVIO_CORREO">
                          ENVIO CORREO
                        </label>
                    </div>
                </div>
            </div>
        </div>

    
            <div class="tableFixHead">
                <table class="table">
                  <thead>
                    <tr><th>COD. IFI.</th><th>Institución</th><th>Servicio</th><th>Archivo</th><th>User</th><th>Fecha</th><th>Acción</th></tr>
                  </thead>
                  <tbody>

                  </tbody>
                </table>
              </div>
    </div>

    <script>
        var instituciones = [];
        var loadInitialFiles = true;

        $(document).ready(function () {

            getInstituciones();   // get all insituciones for cb box

            getProductos();   //get sri productos

            //getInitialFiles();     //***consultar TODOS***

            $("#btn_consultar").click(function () {  //***consultar por filtros***
                getFilesFilter();
            });

            $("#btn_procesar").click(function () {  //***consultar por filtros***
                conciliar();
            });
        });

        function getInitialFiles() {
            var dataString = JSON.stringify({});
            $.ajax({
                type: "POST",
                url: "ConciliacionSRI.aspx/LoadFiles",
                data: dataString,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (result) {
                    var obj = JSON.parse(result.d);
                    console.log(obj);
                    populateTable(obj);
                }
            });
        }

        function populateTable(result) {             //***populate table***
            if (result.Data.length == 0) {
                var table2 = $(".table tbody"); table2.empty();
                alertify.alert("RED Facilito", "No hay datos que mostrar para el criterio de busqueda");
            } else {
                var table = $(".table tbody"); table.empty();
                $.each(result.Data, function (idx, elem) {
                    var servicioX = elem.Servicio === 'MAT' ? 'MATRICULACIÓN' : elem.Servicio;
                    table.append("<tr><td>" + elem.CodIFI + "</td><td>" + getInstitucion(elem.CodIFI) + "</td>   <td>" + "SRI - " +
                        servicioX + "</td><td>" + elem.NombreArchivo + "</td>   <td>" + elem.Usuario + "</td><td>" +
                        elem.Fecha + "</td><td> <a class=" + '"btn btn-info btn-sm" onclick=' + '"downloadFile(' + "'" + elem.NombreArchivo + "'" + "," + "'" + elem.Servicio + "'" + "," + "'" + elem.Fecha + "'" + ')"' + '>Descargar</a>' + "</td></tr>");
                });
            }
        }
        function getFilesFilter() {
            var servicio = $('select[id=select_servicio] option').filter(':selected').val();
            var institucion = $('select[id=select_institucion] option').filter(':selected').val();
            var fecha = $("#datDiaIniShow").val();
            var dataString = JSON.stringify({
                fecha: fecha,
                institucion: institucion,
                servicio: servicio
            });
            console.log(dataString);
            $.ajax({
                type: "POST",
                url: "ConciliacionSRI.aspx/GetFilesBy",
                data: dataString,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (result) {
                    var obj = JSON.parse(result.d);
                    console.log(obj);
                    populateTable(obj);
                }
            });
        }

        function conciliar() {

            var enviar_email = $('#ENVIO_CORREO').is(':checked');

            var servicio = $('select[id=select_servicio] option').filter(':selected').val();
            var institucion = $('select[id=select_institucion] option').filter(':selected').val();
            var fecha = $("#datDiaIniShow").val();

            var dataString = JSON.stringify({
                fecha: fecha,
                institucion: institucion,
                servicio: servicio,
                enviarEmail: enviar_email
            });
            console.log(dataString);
            $.ajax({
                type: "POST",
                url: "ConciliacionSRI.aspx/Conciliar",
                data: dataString,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (result) {
                    var obj = JSON.parse(result.d);
                    console.log(obj);
                    populateTable(obj);
                }
            });

        }



        function downloadFile(fileName, servicio, fecha) {

            console.log("posyy click!!!");

            var parametro = JSON.stringify({
                nombreArchivo: fileName,
                servicio: servicio,
                fecha: fecha,
            });
            var url = "ConciliacionSRI.aspx/Download";
            var xhr = new XMLHttpRequest();

            xhr.onreadystatechange = function () {
                var a;
                if (xhr.readyState === 4 && xhr.status === 200) {
                    a = document.createElement('a');
                    a.href = window.URL.createObjectURL(xhr.response);

                    a.download = fileName;
                    a.style.display = 'none';
                    document.body.appendChild(a);
                    a.click();

                }

            };

            xhr.open("POST", url, true);
            xhr.setRequestHeader("Content-Type", "application/json");
            xhr.responseType = 'blob';
            xhr.send(parametro);

        }

        function getInstituciones() {
            //GetInstituciones
            $.ajax({
                type: "POST",
                url: "ConciliacionSRI.aspx/GetInstituciones",
                data: "",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (result) {
                    //var obj = JSON.parse(result.d);

                    //var data = JSON.parse(obj.DataResult.Data).Datos.Table;

                    var data = [{ nombre: "Coop 29 DE OCTUBRE", fi_sri: "122" }, { nombre: "ANDALUCIA", fi_sri: "123" }, { nombre: "BANCO DESARROLLO DE LOS PUEBLOS S.A", fi_sri: "128" }, { nombre: "COTOCOLLAO", "fi_sri": "130" }, { nombre: "GUARANDA", fi_sri: "132" }, { nombre: "COOPROGRESO", fi_sri: "137" }, { nombre: "Coop SAN FRANCISCO", fi_sri: "139" }, { nombre: "SANTA ROSA", fi_sri: "141" }, { nombre: "Coop COOPCCP", fi_sri: "300" }, { nombre: "PADRE JULIAN LORENTE", fi_sri: "304" }, { nombre: "Coop SAN JOSE", fi_sri: "364" }, { nombre: "COAC JARDIN AZUAYO", fi_sri: "615" }, { nombre: "LA BENEFICA", fi_sri: "720" }, { nombre: "Coop MUTUALISTA IMBABURA", fi_sri: "74" }, { nombre: "Coop MUTUALISTA PICHINCHA", fi_sri: "76" }];

                    instituciones = data;
                    populateCbBox(data);
                    if (loadInitialFiles) {
                        getInitialFiles();
                        loadInitialFiles = false;
                    }
                }
            });


        }

        function populateCbBox(result) {
            var $dropdown = $("#select_institucion");
            $.each(result, function () {
                $dropdown.append($("<option/>").val(this.fi_sri).text(this.nombre));
            });
        }

        function getInstitucion(idInstitucion) {
            var institucion = instituciones.find(ele => ele.fi_sri == idInstitucion);
            return institucion ? institucion.nombre : "Desconocido";
        }

        async function getProductos() {
            //GetInstituciones

            $.ajax({
                type: "POST",
                url: "ConciliacionSRI.aspx/GetProductos",
                data: "",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (result) {
                    //var obj = JSON.parse(result.d);

                    //var data = JSON.parse(obj.DataResult.Data).Datos.Table;
                    var data = [{ producto: "0010011007", nombre: "SRI - RISE - GLOBAL" }, { producto: "0010181010", nombre: "SRI - MATRICULACION - TRANSFERENCIA DOMINIO" }, { producto: "0010191011", nombre: "SRI - CEP" }];

                    populateCbBoxProductos(data);
                }
            });

        }

        function populateCbBoxProductos(result) {
            var $dropdown = $("#select_servicio");
            $.each(result, function () {
                var splitedPro = this.nombre.split(" - ");
                var prod = splitedPro[1];
                $dropdown.append($("<option/>").val(prod).text(splitedPro[0] + ' - ' + splitedPro[1]));
            });
        }
    </script>

    <%--<script src="https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.29.1/moment.min.js"></script>
    <script src="../Assets/bootstrap-datetimepicker/js/bootstrap-datetimepicker.min.js"></script>

      <script type="text/javascript">
          //***config calendar***
          var fecha = new Date();

          var mm = fecha.getMonth() + 1;
          var dd = fecha.getDate();
          var yyyy = fecha.getFullYear();

          if (dd < 10) {
              dd = '0' + dd;
          }
          if (mm < 10) {
              mm = '0' + mm;
          }

          var strDateDiaMax = "{0}/{1}/{2}".format(dd, mm, yyyy);
          var strDateDiaMaxH = "{0}{1}{2}".format(yyyy, mm, dd);

          $('.form_date_day_ini').datetimepicker(
              {
                  format: "dd/mm/yyyy",
                  useCurrent: true,
                  language: 'es',
                  weekStart: 1,
                  autoclose: true,
                  todayBtn: true,
                  todayHighlight: 0,
                  startView: 2,
                  minView: 2,
                  forceParse: 0,
                  startDate: '-1m',
                  endDate: '-0d',
                  linkField: "datDiaIni",
                  linkFormat: "yyyymmdd"
              });



          document.getElementById("datDiaIniShow").value = strDateDiaMax;
          document.getElementById("datDiaIni").value = strDateDiaMaxH;



      </script>--%>
</body>
</html>
