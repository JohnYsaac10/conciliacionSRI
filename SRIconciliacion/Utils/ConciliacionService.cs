using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SRIconciliacion.Utils
{
    public class ConciliacionService
    {
        public Fase1 Sp_consulta_conciliacion_mat_getAdquirente(string fecha, string tipo, string codLogMon, string producto)
        {
            /*
                CREATE PROCEDURE sp_consulta_conciliacion_mat_getAdquirente
														@fecha nvarchar(8),
														@tipo nvarchar(8),
														@codLogMon nvarchar(25),
														@producto nvarchar(15)
                AS
	                BEGIN
		                select count (1) as si,lg_sec_adquirente
		                from serbas..sb_log 
		                where   lg_fecha_trn in (@fecha) and lg_type_trn=@tipo and ltrim(lg_direccion)=@codLogMon and   
		                lg_producto=@producto  and lg_cod_resultado='15' and lg_indicador_reverso is null
		                group by lg_sec_adquirente
	                END
                GO;
             */


            if (codLogMon == "12229092020235580663")
            {
                return new Fase1
                {
                    OK = 0,
                    Secuencial = 123
                };
            }

            return new Fase1
            {
                OK = 1,
                Secuencial = 1111
            };
        }

        public int Sp_consult_conciliacion_mat_getExrev(string placaCpnCamv, string tipo, int secuencial)
        {
            /*
             CREATE PROCEDURE sp_consult_conciliacion_mat_getExrev
													@placaCpnCamv nvarchar(10),
													@tipo nvarchar(8),
													@secuencial INT
            AS
	            BEGIN
		            select count(1) as exrev
                    from serbas..sb_log
		            where lg_referencia = @placaCpnCamv and lg_cod_resultado = '15' AND lg_type_trn = @tipo
		            and lg_indicador_reverso is not null and lg_sec_adquirente = @secuencial
	            END
            GO;

            */

            if (placaCpnCamv == "IS268U")
            {
                return 1;
            }
            return 0;
        }


        public Fase1 Sp_consulta_conciliacion_rise_getAdquirente(string fecha, string tipo, string valorReferencia, string numCompSRI, string producto)
        {
            /*
             CREATE PROCEDURE Sp_consulta_conciliacion_rise_getAdquirente
													    @fecha nvarchar(8),
														@tipo nvarchar(8),
														@valorReferencia nvarchar(25),
														@numCompSRI INT,
                                                        @producto nvarchar(15)
            AS
	            BEGIN
		            select count(1) as si,lg_sec_adquirente
                    from serbas..sb_log
                    where lg_fecha_compensa in (@fecha) and lg_type_trn = @tipo and
                    lg_referencia = @valorReferencia and convert(numeric, lg_numero_factura)= @numCompSRI
                    and lg_producto = @producto  and lg_cod_resultado = '00' and lg_indicador_reverso is null
                    group by lg_sec_adquirente
	            END
            GO;
            */
            return new Fase1
            {
                OK = 1,
                Secuencial = 1111
            };

        }

        public int Sp_consulta_conciliacion_rise_getExrev(string fecha, string valorReferencia, int secuencial)
        {

            /*
             CREATE PROCEDURE sp_consult_conciliacion_rise_getExrev
                                                    @fecha nvarchar(8),
													@valorReferencia nvarchar(25),
													@secuencial INT
            AS
	            BEGIN
		            select count(1) as exrev
                    from serbas..sb_log
                    where lg_fecha_compensa in (@fecha) and
                    lg_referencia = @valorReferencia and lg_cod_resultado = '00' AND SUBSTRING(LG_TYPE_TRN,1,2)= '01'
                    and lg_indicador_reverso is not null and lg_sec_adquirente = @secuencial;
	            END
            GO;

            */

            return 0;
        }
    }

    public class Fase1
    {
        public int OK { get; set; }
        public int Secuencial { get; set; }
    }
}