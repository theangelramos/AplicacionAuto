using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AplicacionAuto.Clases
{
    class Datos
    {
    }

    public class Urls
    {
        private static string urlServidor = "https://auto-universe.com.mx";



        public static string UrlServer()
        {
            return urlServidor;
        }
    }

    public class LoginInicio
    {
        public String CorreoElectronico { get; set; }
        public String Password { get; set; }
    }

    public class MarcaDTO
    {
        public int ID { get; set; }
        public String Nombre { get; set; }
        public double Coeficiente { get; set; }
    }
    public class FactorDTO
    {
        public int ID { get; set; }
        public double Valor { get; set; }
    }

    public class SubmarcaDTO
    {
        public int ID { get; set; }
        public String Nombre { get; set; }
        public double Coeficiente { get; set; }
        public int MarcaID { get; set; }
        public String MarcaNombre { get; set; }

    }
    public class ModeloDTO
    {
        public int ID { get; set; }
        public String Nombre { get; set; }
        public double Coeficiente { get; set; }
    }
    public class CocheDTO
    {
        public int ID { get; set; }

        public int SubmarcaID { get; set; }

        public String SubmarcaNombre { get; set; }

        public int MarcaID { get; set; }
        public String MarcaNombre { get; set; }

        public int ModeloID { get; set; }
        public String ModeloNombre { get; set; }

        public int TipoID { get; set; }
        public String TipoNombre { get; set; }
        public String Version { get; set; }


    }

    public class ColorDTO
    {
        public int ID { get; set; }
        public String Nombre { get; set; }
        public int CategoriaID { get; set; }
        public String CategoriaNombre { get; set; }
        public double CategoriaCoeficiente { get; set; }

    }

    public class AcabadoDTO
    {
        public int ID { get; set; }
        public String Nombre { get; set; }
        public double Coeficiente { get; set; }
    }

    public class TipoDTO
    {
        public int ID { get; set; }
        public String Nombre { get; set; }

    }

    public class TipoServicioDTO
    {
        public int ID { get; set; }
        public String Nombre { get; set; }
    }

    public class PrioridadDTO
    {
        public int ID { get; set; }
        public String Nombre { get; set; }
        public double Coeficiente { get; set; }
    }

    public class TipoGolpeDTO
    {
        public int ID { get; set; }
        public String Nombre { get; set; }
        public double Coeficiente { get; set; }

    }
    public class TipoServicioPaqueteDTO
    {
        public int ID { get; set; }
        public int TipoServicioID { get; set; }
        public String TipoServicioNombre { get; set; }
        public int PaqueteID { get; set; }
        public String PaqueteNombre { get; set; }
        public double Precio { get; set; }
        public String Descripcion { get; set; }
    }

    public class DatosAuto
    {
        public String Marca { get; set; }
        public String Submarca { get; set; }
        public String Modelo { get; set; }
        public String Tipo { get; set; }
        public String Version { get; set; }
        public String Color { get; set; }
        public String Acabado { get; set; }

    }

    public class PiezaDTO
    {
        public int ID { get; set; }
        public String Nombre { get; set; }
        public double Coeficiente { get; set; }
        public int TipoPiezaID { get; set; }
        public String TipoPiezaNombre { get; set; }



    }

    public class DatosPiezaDTO
    {
        public int ID { get; set; }
        public int? PiezaID { get; set; }
        public int? ImagenID { get; set; }
        public string DiametroH { get; set; }
        public string DiametroV { get; set; }
        public string Profundidad { get; set; }
    }
    public class SolicitudDatosPiezaDTO
    {
        public int ID { get; set; }

        public int? DatosPiezaID { get; set; }

        public int? SolicitudID { get; set; }
    }


    public class MedidaDTO
    {
        public int ID { get; set; }
        public String Descripcion { get; set; }
        public double Coeficiente { get; set; }
        public int TipoMedidaID { get; set; }
        public String TipoMedidaNombre { get; set; }
    }

    public class TallerDTO
    {
        public int ID { get; set; }
        public String Nombre { get; set; }
        public String Direccion { get; set; }
        public String Telefono { get; set; }
        public String CorreoElectronico { get; set; }
        public String Encargado { get; set; }
        public String HorarioServicio { get; set; }
        public String Longitud { get; set; }
        public String Latitud { get; set; }

    }
    public class ImagenDTO
    {
        public int ID { get; set; }
        public byte[] DatoImagen { get; set; }
    }
    public class ImagenDTOB
    {
        public int ID { get; set; }
        public byte[] DatoImagen { get; set; }
        public string Nombre { get; set; }
    }
    public class Imagen
    {
        public byte[] DatoImagen { get; set; }
    }



    public class GolpeMedida
    {
        public string ImagenPath { get; set; }
        public byte[] DatoImagen { get; set; }
        public string PiezaNombre { get; set; }
        public string EtiquetaPiezaNombre { get; set; }

        public string DiametroH { get; set; }
        public string DiametroV { get; set; }

        public string ProfundidadGolpe { get; set; }

        public string EtiquetaDatos { get; set; }
    }

    public class UsuarioDTO
    {
        public int ID { get; set; }
        public String Estado { get; set; }
        public String Municipio { get; set; }
        public String NombreCompleto { get; set; }
        public String Telefono { get; set; }
        public String CorreoElectronico { get; set; }
        public Boolean Estatus { get; set; }

    }

    public class MedidadDTO
    {
        public string MedidaH { get; set; }
        public string MedidaV { get; set;}
        public string profundidad { get; set;}
        public int? ImagenID { get; set; }
        public int? PiezaID { get; set; }

    }

    public class SolicitudDTO
    {
        public int ID { get; set; }
        public int? UsuarioID { get; set; }
        public int? CocheID { get; set; }
        public int? AcabadoID { get; set; }
        public int? ColorID { get; set; }
        public int? TipoServicioPaqueteID { get; set; }
        public int? TallerID { get; set; }
        public int? PrioridadID { get; set; }
        public int? TipoGolpeID { get; set; }
        public string FechaEmision { get; set; }
        public String FechaCita { get; set; }
        public String HoraCita { get; set; }
        public Double Presupuesto { get; set; }


    }

    public class Coche
    {
        public int ID { get; set; }
        public int SubmarcaID { get; set; }
        public int ModeloID { get; set; }
        public int TipoID { get; set; }
        public String Version { get; set; }
    }

    public class Acabado
    {
        public int ID { get; set; }
        public String Nombre { get; set; }
        public double Coeficiente { get; set; }
    }

    public class TipoServicioPaquete
    {
        public int ID { get; set; }
        public int TipoServicioID { get; set; }
        public int PaqueteID { get; set; }
        public double Precio { get; set; }
        public String Descripcion { get; set; }
    }

    public class Taller
    {
        public int ID { get; set; }
        public String Nombre { get; set; }
        public String Direccion { get; set; }
        public String Telefono { get; set; }
        public String CorreoElectronico { get; set; }
        public String Encargado { get; set; }
        public String HorarioServicio { get; set; }
        public String Longitud { get; set; }
        public String Latitud { get; set; }
    }

    public class Prioridad
    {
        public int ID { get; set; }
        public String Nombre { get; set; }
        public double Coeficiente { get; set; }
    }

    public class TipoGolpe
    {
        public int ID { get; set; }
        public String Nombre { get; set; }
        public double Coeficiente { get; set; }
    }

}
