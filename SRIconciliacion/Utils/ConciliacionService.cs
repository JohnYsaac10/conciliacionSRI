using SRIconciliacion.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace SRIconciliacion.Utils
{
    public class ConciliacionService
    {
        private SqlConnection sqlConnection = new SqlConnection(@"Data Source=DESKTOP-QTOUJLK\SQLEXPRESS;Initial Catalog=task;Integrated Security=True");

        public ConciliacionService()
        {
            if(sqlConnection.State == ConnectionState.Open)
            {
                sqlConnection.Close();
            }
            sqlConnection.Open();
        }

        public string ConsultaPathFilesSRIConciliacionFacilitoSwitch()
        {
            return "";
        }

        public Result ConsultaReversosRiseCepMat(string fechaPago, string producto)
        {
            using (var cmd = new SqlCommand("Spf_consulta_reversos_MAT_RISE_CEP", sqlConnection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fechaPago", fechaPago);
                cmd.Parameters.AddWithValue("@producto", producto);

                DataTable dt = new DataTable { TableName = "Table" };

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dt);

                var res = ConvertDatatableToXML(dt);

                res = res.Replace("DocumentElement", "Datos");

                return new Result { Data = res };
            }
        }
        public string ConvertDatatableToXML(DataTable dt)
        {
            string result;
            using (StringWriter sw = new StringWriter())
            {
                dt.WriteXml(sw);
                result = sw.ToString();
            }
            return result;
        }

        //store procedure
        /*
            Alter procedure Spf_consulta_reversos_MAT_RISE_CEP
														@fechaPago Varchar(10),
														@producto varchar(20)
AS
BEGIN
	IF @producto = '0010181010' --MATRICULACION
	BEGIN
		select lg_type_trn, lg_fecha_trn, trim(lg_direccion) AS codLogMon, trim(lg_referencia) as lg_referencia
		from sb_log
		where lg_fecha_trn in (select * from STRING_SPLIT(@fechaPago, ',')) AND lg_producto=@producto  
		and lg_cod_resultado='15' and lg_indicador_reverso is not null
	END

	IF @producto = '0010011007' --RISE     php(cep)
	BEGIN
		select lg_type_trn, lg_fecha_trn, trim(lg_referencia) as lg_referencia, convert(numeric,lg_numero_factura) as lg_numero_factura
		from sb_log 
		where lg_fecha_compensa in (select * from STRING_SPLIT(@fechaPago, ','))
		--and fecha
		--and lg_type_trn='".$tipo."' 
		--and lg_referencia='".$valorReferencia."' 
		--and convert(numeric,lg_numero_factura)= '".$numCompSRI."' 
		and lg_producto=@producto  and lg_cod_resultado='00' and lg_indicador_reverso is not null
	END

	IF @producto = '0010191011' --CEP       php(sri)
	BEGIN
		select lg_fecha_trn, lg_type_trn, lg_new_cedruc, lg_referencia, RTRIM(LTRIM(SUBSTRING(LG_DAT_FACTURA,6,25))) as codLogMon
		from sb_log 
		where   lg_fecha_trn in (select * from STRING_SPLIT(@fechaPago, ','))
		--and lg_type_trn='" . $tipo . "' 
		--and lg_new_cedruc='" . $numeroRuc . "' 
		--and  lg_referencia='" . $adhesivo . "' 
		--and RTRIM(LTRIM(SUBSTRING(LG_DAT_FACTURA,6,25)))='" . $codigoLogMonitor . "' 
		and lg_producto=@producto and lg_cod_resultado='01' and lg_indicador_reverso is not null
	END

	
END

         */
    }
}