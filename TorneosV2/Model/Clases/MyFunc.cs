namespace TorneosV2.Model.Clases
{
    public class MyFunc
    {
        public Z190_Bitacora MakeBitacora(string userId, string orgId, string desc,
            bool sistema)
        {
            Z190_Bitacora bitacora = new Z190_Bitacora();
            bitacora.UserId = userId;
            bitacora.OrgId = orgId;
            bitacora.Desc = desc;
            bitacora.Sistema = sistema;

            return bitacora;
        }

        public static string DiaTitulo(int dia, int completo = 0)
        {
            string valores = "Dom,Lun,Mar,Mie,Jue,Vie,Sab,Domingo,Lunes,Martes,Miercoles,Jueves,Viernes,Sabado";
            var arr = valores.Split(",");
            completo = completo > 1 ? 0 : completo * 7;

            return arr[(dia - 1 + completo)];
        }
        public static string MesTitulo(int mes, int completo = 0)
        {
            string valores = "Ene,Feb,Mar,Abr,May,Jun,Jul,Ago,Sep,Oct,Nov,Dic,Enero,Febrero,Marzo,Abril,Mayo,Junio,Julio,Agosto,Septiembre,Octubre,Noviembre,Diciembre";
            var arr = valores.Split(",");
            completo = completo > 1 ? 0 : completo * 12;

            return arr[(mes - 1 + completo)];
        }
        public static int Ejercicio(DateTime fecha)
        {
            int year = fecha.Year;
            return year < 2000 ? year - 1900 : year - 2000;
        }
        public static string LaHora(DateTime lahora, string formato)
        {
            switch (formato)
            {
                case "M":
                    string cero = lahora.Minute < 10 ? "0" : "";
                    return $"{lahora.Hour}:{cero}{lahora.Minute}";

                default:
                    string mincero = lahora.Minute < 10 ? "0" : "";
                    string segcero = lahora.Second < 10 ? "0" : "";
                    return $"{lahora.Hour}:{mincero}{lahora.Minute}:{segcero}{lahora.Second}";
            }
        }
        public static string FormatoFecha(string formato, DateTime lafecha)
        {
            string resultado = string.Empty;

            switch (formato)
            {
                case "DD/MMM/AA":
                    resultado = $"{lafecha.Day}/";
                    resultado += $"{MesTitulo(lafecha.Month, 0)}/";
                    resultado += $"{Ejercicio(lafecha)}";
                    break;

            }
            return resultado;
        }
        public static int DameRandom(int inicio, int final)
        {
            Random rnd = new Random();
            return rnd.Next(inicio, final);
        }

        public const string Repetitivo = "Ilimitado,Diario,Semanal,Quincenal,Mensual,Bimestral,Trimestral,Cuatrimestral,Semestral,Anual";

        public const string RepetitivoN = "9999,365,52,24,12,6,4,3,2,1";
    }
}
