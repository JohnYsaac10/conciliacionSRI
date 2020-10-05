<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConciliacionSRI.aspx.cs" Inherits="SRIconciliacion.Views.ConciliacionSRI" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>ConciliaciónSRI</title>

    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" 
    integrity="sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u" crossorigin="anonymous">
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
        <div class="row">
          <div class="col-sm-8">
            <div class="row">
                <div class="col-sm-6">
                    <div class="form-group row">
                        <label for="input_fecha" class="col-sm-3 col-form-label">FECHA:</label>
                        <div class="col-sm-9">
                          <input type="text" class="form-control-plaintext" id="input_fecha">
                        </div>
                    </div>
                </div>
                <div class="col-sm-6">
                    <div class="form-group row">
                        <label for="select_servicio" class="col-sm-4 col-form-label">SERVICIO:</label>
                        <div class="col-sm-8">
                            <select class="form-control" id="select_servicio">
                                <option selected="selected" value="">TODOS</option>
                                <option value="CEP">CEP</option>
                                <option value="RISE">RISE</option>
                                <option value="MAT">MATRICULA</option>
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
                                    <option value="132">INSTITUCION 1</option>
                                    <option value="141">INSTITUCION 2</option>
                                    <option value="304">INSTITUCION 3</option>
                                    <option value="076">INSTITUCION 4</option>
                                    <option value="074">INSTITUCION 5</option>
                                </select>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-sm-4">
                <div class="row">
                    <button type="button" class="btn btn-success btn-sm">PROCESAR</button>
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
                    <!--<tr><td>079</td><td>COAD</td><td>MAT</td><td>54-02-03-2020.xml</td><td>admin</td><td>12/03/2020</td><td><button class="btn btn-info btn-sm">Descargar</button></td></tr>
                    <tr><td>079</td><td>COAD</td><td>MAT</td><td>54-02-03-2020.xml</td><td>admin</td><td>12/03/2020</td><td><button class="btn btn-info btn-sm">Descargar</button></td></tr>
                    <tr><td>079</td><td>COAD</td><td>MAT</td><td>54-02-03-2020.xml</td><td>admin</td><td>12/03/2020</td><td><button class="btn btn-info btn-sm">Descargar</button></td></tr>
                    <tr><td>079</td><td>COAD</td><td>MAT</td><td>54-02-03-2020.xml</td><td>admin</td><td>12/03/2020</td><td><button class="btn btn-info btn-sm">Descargar</button></td></tr>
                    <tr><td>079</td><td>COAD</td><td>MAT</td><td>54-02-03-2020.xml</td><td>admin</td><td>12/03/2020</td><td><button class="btn btn-info btn-sm">Descargar</button></td></tr>  -->
                
                  </tbody>
                </table>
              </div>
    </div>

    <script>
        $(document).ready(function () {

            getInitialFiles();

            $("#btn_consultar").click(function () {
                getFilesFilter();
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

        function populateTable(result) {
            if (result.Data.length == 0) {
                alert("no hay datos que mostrar")
            } else {
                var table = $(".table tbody"); table.empty();
                $.each(result.Data, function (idx, elem) {
                    table.append("<tr><td>" + elem.CodIFI + "</td><td>" + elem.Institucion + "</td>   <td>" + "SRI - " +
                        elem.Servicio + "</td><td>" + elem.NombreArchivo + "</td>   <td>" + elem.Usuario + "</td><td>" +
                        elem.Fecha + "</td><td> <button class=" + '"btn btn-info btn-sm" onclick=' + '"downloadFile(' + elem.NombreArchivo + "-" + elem.Servicio + ')"' + '>Descargar</button>'  + "</td></tr>");
                });
            }

            //table.empty().append();
        }

        function getFilesFilter() {
            var servicio = $('select[id=select_servicio] option').filter(':selected').val();
            var institucion = $('select[id=select_institucion] option').filter(':selected').val();
            var fecha = $("#input_fecha").val();

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
    </script>
</body>
</html>
