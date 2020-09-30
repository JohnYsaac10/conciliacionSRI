<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConciliacionSRI.aspx.cs" Inherits="SRIconciliacion.Views.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>ConciliaciónSRI</title>

    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" 
    integrity="sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u" crossorigin="anonymous">
    <style>
        .tableFixHead          { overflow-y: auto; height: 150px; }
        .tableFixHead thead th { position: sticky; top: 0; }

        /* Just common table stuff. Really. */
        table  { border-collapse: collapse; width: 100%; }
        th, td { padding: 8px 16px; }
        th     { background:#eee; }
    </style>
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
                                <option>CEP</option>
                                <option>RISE</option>
                                <option>MATRICULA</option>
                            </select>
                        </div>
                    </div>
                </div>
            </div>
          </div>
          <div class="col-sm-4">
              <div class="row">
                <button type="button" class="btn btn-warning btn-sm">CONSULTAR</button>
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
                                    <option>INTERAGUA</option>
                                    <option>COMISION TRANSITO</option>
                                    <option>FULL RECARGA</option>
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



        <!-- table -->
        <!--<table class="table">
            <thead>
            <tr>
            <th>#</th>
            <th>First Name</th>
            <th>Last Name</th>
            <th>Username</th>
            </tr>
            </thead>
            <tbody>
            <tr>
            <td>1</td>
            <td>Mark</td>
            <td>Otto</td>
            <td>@mdo</td>
            </tr>
            <tr>
            <td>2</td>
            <td>Jacob</td>
            <td>Thornton</td>
            <td>@fat</td>
            </tr>
            <tr>
            <td>3</td>
            <td>Larry</td>
            <td>the Bird</td>
            <td>@twitter</td>
            </tr>
            </tbody>
            </table>  -->
    
            <div class="tableFixHead">
                <table class="table">
                  <thead>
                    <tr><th>COD. IFI.</th><th>Institución</th><th>Servicio</th><th>Archivo</th><th>User</th><th>Fecha</th><th>Acción</th></tr>
                  </thead>
                  <tbody>
                    <tr><td>079</td><td>COAD</td><td>MAT</td><td>54-02-03-2020.xml</td><td>admin</td><td>12/03/2020</td><td><button class="btn btn-info btn-sm">Descargar</button></td></tr>
                    <tr><td>079</td><td>COAD</td><td>MAT</td><td>54-02-03-2020.xml</td><td>admin</td><td>12/03/2020</td><td><button class="btn btn-info btn-sm">Descargar</button></td></tr>
                    <tr><td>079</td><td>COAD</td><td>MAT</td><td>54-02-03-2020.xml</td><td>admin</td><td>12/03/2020</td><td><button class="btn btn-info btn-sm">Descargar</button></td></tr>
                    <tr><td>079</td><td>COAD</td><td>MAT</td><td>54-02-03-2020.xml</td><td>admin</td><td>12/03/2020</td><td><button class="btn btn-info btn-sm">Descargar</button></td></tr>
                    <tr><td>079</td><td>COAD</td><td>MAT</td><td>54-02-03-2020.xml</td><td>admin</td><td>12/03/2020</td><td><button class="btn btn-info btn-sm">Descargar</button></td></tr>
                
                  </tbody>
                </table>
              </div>
    </div>
</body>
</html>
