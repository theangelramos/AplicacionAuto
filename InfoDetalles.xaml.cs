using AplicacionAuto.Clases;
using HTTPupt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Microsoft.Maui;

namespace AplicacionAuto
{
    public partial class InfoDetalles : ContentPage
    {
        PeticionHTTP peticion = new PeticionHTTP(Urls.UrlServer());

        DatosAuto datosAuto1;
        String prioridad1;
        String servicio1;
        String tipoGolpe1;
        List<GolpeMedida> datosGolpes1;
        String paquete1;
        TallerDTO taller1;
        String fecha1;
        String hora1;
        String opcionTodo1;
        private List<string> partesSeleccionadasPE1;

        public List<MarcaDTO> marcas;
        public List<SubmarcaDTO> submarcas;
        public List<ModeloDTO> modelos;
        public List<TipoDTO> tipos;
        public List<ColorDTO> colors;

        public List<AcabadoDTO> acabados;

        public List<PrioridadDTO> prioridads;

        public List<TipoServicioDTO> tipoServicios;

        public List<TipoGolpeDTO> tipoGolpes;
        public List<PiezaDTO> piezas;
        public List<FactorDTO> factors;
        public List<TipoServicioPaqueteDTO> tipoServicioPaquetes;

        public List<MedidaDTO> medidas;
        List<Imagen> imagenesGolpeFuerte1;
        List<Imagen> imagenesTodoElVehiculo1;
        public InfoDetalles(DatosAuto datosAuto, String prioridad, String servicio, String tipoGolpe, List<GolpeMedida> datosGolpes, String paquete, TallerDTO taller, String fecha, String hora, List<string> partesSeleccionadasPE, String opcionTodo, List<Imagen> imagenesGolpeFuerte, List<Imagen> imagenesTodoElVehiculo)
        {

            InitializeComponent();
            datosAuto1 = datosAuto;
            prioridad1 = prioridad;
            servicio1 = servicio;
            tipoGolpe1 = tipoGolpe;
            datosGolpes1 = datosGolpes;
            paquete1 = paquete;
            taller1 = taller;
            fecha1 = fecha;
            hora1 = hora;
            opcionTodo1 = opcionTodo;
            partesSeleccionadasPE1 = partesSeleccionadasPE;
            imagenesGolpeFuerte1 = imagenesGolpeFuerte;
            imagenesTodoElVehiculo1 = imagenesTodoElVehiculo;

            if (datosAuto != null)
            {
                marcaLabel.Text = datosAuto.Marca;
                submarcaLabel.Text = datosAuto.Submarca;
                modeloLabel.Text = datosAuto.Modelo;
                versionLabel.Text = datosAuto.Version;
                tipoLabel.Text = datosAuto.Tipo;
                colorLabel.Text = datosAuto.Color;
                acabadoLabel.Text = datosAuto.Acabado;
            }
            else
            {
                marcaLabel.IsVisible = false;
                marcaL.IsVisible = false;

                submarcaLabel.IsVisible = false;
                submarcaL.IsVisible = false;

                modeloLabel.IsVisible = false;
                modeloL.IsVisible = false;

                versionLabel.IsVisible = false;
                versionL.IsVisible = false;

                tipoLabel.IsVisible = false;
                tipoL.IsVisible = false;

                colorLabel.IsVisible = false;
                colorL.IsVisible = false;

                acabadoLabel.IsVisible = false;
                acabadoL.IsVisible = false;
            }
            if (prioridad == null)
            {
                prioridadLabel.IsVisible = false;
                prioridadL.IsVisible = false;
            }
            prioridadLabel.Text = prioridad;

            if (servicio == null)
            {
                servicioLabel.IsVisible = false;
                servicioL.IsVisible = false;
            }
            servicioLabel.Text = servicio;

            if (tipoGolpe == null)
            {
                tipoGolpeLabel.IsVisible = false;
                tipoGolpeL.IsVisible = false;
            }
            tipoGolpeLabel.Text = tipoGolpe;

            if (paquete == null)
            {
                paqueteLabel.IsVisible = false;
                paqueteL.IsVisible = false;
            }
            paqueteLabel.Text = paquete;

            if (taller == null)
            {
                tallerLabel.IsVisible = false;
                tallerL.IsVisible = false;
            }
            tallerLabel.Text = taller.Nombre;

            if (fecha == null)
            {
                fechaLabel.IsVisible = false;
                fechaL.IsVisible = false;
            }
            fechaLabel.Text = fecha;
            if (hora == null)
            {
                horaLabel.IsVisible = false;
                horaL.IsVisible = false;
            }
            horaLabel.Text = hora;

            if (tipoGolpe != null)
            {
                if (tipoGolpe.ToLower() == "Fuerte".ToLower())
                {
                    presupustoL.IsVisible = false;
                    presupuestoLabel.IsVisible = false;
                }
            }


            _ = obtenerDatos();
            obtenerPresuspuesto();


        }


        private async Task obtenerDatos()
        {
            String jsonRecibir;
            try
            {
                peticion.PedirComunicacion("Marca/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Default.Get("token", ""));
                jsonRecibir = await peticion.ObtenerJson();

                if (jsonRecibir != null)
                {
                    marcas = JsonConvertidor.Json_ListaObjeto<MarcaDTO>(jsonRecibir);
                }

                peticion.PedirComunicacion("Submarca/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Default.Get("token", ""));
                jsonRecibir = await peticion.ObtenerJson();

                if (jsonRecibir != null)
                {
                    submarcas = JsonConvertidor.Json_ListaObjeto<SubmarcaDTO>(jsonRecibir);
                }
                peticion.PedirComunicacion("Modelo/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Default.Get("token", ""));
                jsonRecibir = await peticion.ObtenerJson();

                if (jsonRecibir != null)
                {
                    modelos = JsonConvertidor.Json_ListaObjeto<ModeloDTO>(jsonRecibir);
                }



                peticion.PedirComunicacion("Tipo/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Default.Get("token", ""));
                jsonRecibir = await peticion.ObtenerJson();

                if (jsonRecibir != null)
                {
                    tipos = JsonConvertidor.Json_ListaObjeto<TipoDTO>(jsonRecibir);
                }

                peticion.PedirComunicacion("Color/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Default.Get("token", ""));
                jsonRecibir = await peticion.ObtenerJson();

                if (jsonRecibir != null)
                {

                    colors = JsonConvertidor.Json_ListaObjeto<ColorDTO>(jsonRecibir);
                }

                peticion.PedirComunicacion("Acabado/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Default.Get("token", ""));
                jsonRecibir = await peticion.ObtenerJson();

                if (jsonRecibir != null)
                {
                    acabados = JsonConvertidor.Json_ListaObjeto<AcabadoDTO>(jsonRecibir);
                }


                peticion.PedirComunicacion("Prioridad/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Default.Get("token", ""));
                jsonRecibir = await peticion.ObtenerJson();

                if (jsonRecibir != null)
                {
                    prioridads = JsonConvertidor.Json_ListaObjeto<PrioridadDTO>(jsonRecibir);
                }

                peticion.PedirComunicacion("TipoServicio/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Default.Get("token", ""));
                jsonRecibir = await peticion.ObtenerJson();

                if (jsonRecibir != null)
                {
                    tipoServicios = JsonConvertidor.Json_ListaObjeto<TipoServicioDTO>(jsonRecibir);
                }

                peticion.PedirComunicacion("TipoGolpe/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Default.Get("token", ""));
                jsonRecibir = await peticion.ObtenerJson();

                if (jsonRecibir != null)
                {
                    tipoGolpes = JsonConvertidor.Json_ListaObjeto<TipoGolpeDTO>(jsonRecibir);
                }

                peticion.PedirComunicacion("Pieza/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Default.Get("token", ""));
                jsonRecibir = await peticion.ObtenerJson();

                if (jsonRecibir != null)
                {
                    piezas = JsonConvertidor.Json_ListaObjeto<PiezaDTO>(jsonRecibir);
                }

                peticion.PedirComunicacion("Factor/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Default.Get("token", ""));
                jsonRecibir = await peticion.ObtenerJson();

                if (jsonRecibir != null)
                {
                    factors = JsonConvertidor.Json_ListaObjeto<FactorDTO>(jsonRecibir);
                }

                peticion.PedirComunicacion("TipoServicioPaquete/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Default.Get("token", ""));
                jsonRecibir = await peticion.ObtenerJson();

                if (jsonRecibir != null)
                {
                    tipoServicioPaquetes = JsonConvertidor.Json_ListaObjeto<TipoServicioPaqueteDTO>(jsonRecibir);
                }

                peticion.PedirComunicacion("Medida/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Default.Get("token", ""));
                jsonRecibir = await peticion.ObtenerJson();

                if (jsonRecibir != null)
                {
                    medidas = JsonConvertidor.Json_ListaObjeto<MedidaDTO>(jsonRecibir);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public Double factor;
        Double precioCotizacion = 0;
        public void obtenerPresuspuesto()
        {

            if (factors != null)
            {
                FactorDTO objetoFactor = factors.FirstOrDefault();
                factor = objetoFactor.Valor;


            }

            if (datosAuto1 != null)
            {
                if (marcas != null)
                {
                    var objetoObtenido = marcas.FindAll(x => x.Nombre.ToLower() == datosAuto1.Marca.ToLower());
                    MarcaDTO objeto = objetoObtenido.FirstOrDefault();

                    precioCotizacion = objeto.Coeficiente * factor;

                }




                if (submarcas != null)
                {
                    var objetoObtenido = submarcas.FindAll(x => x.Nombre.ToLower() == datosAuto1.Submarca.ToLower());
                    SubmarcaDTO objeto = objetoObtenido.FirstOrDefault();

                    precioCotizacion = precioCotizacion + objeto.Coeficiente * factor;

                }

                if (modelos != null)
                {
                    var objetoObtenido = modelos.FindAll(x => x.Nombre.ToLower() == datosAuto1.Modelo.ToLower());
                    ModeloDTO objeto = objetoObtenido.FirstOrDefault();

                    precioCotizacion = precioCotizacion + objeto.Coeficiente * factor;

                }

                if (colors != null)
                {
                    var objetoObtenido = colors.FindAll(x => x.Nombre.ToLower() == datosAuto1.Color.ToLower());
                    ColorDTO objeto = objetoObtenido.FirstOrDefault();

                    precioCotizacion = precioCotizacion + objeto.CategoriaCoeficiente * factor;

                }

                if (acabados != null)
                {
                    var objetoObtenido = acabados.FindAll(x => x.Nombre.ToLower() == datosAuto1.Acabado.ToLower());
                    AcabadoDTO objeto = objetoObtenido.FirstOrDefault();

                    precioCotizacion = precioCotizacion + objeto.Coeficiente * factor;

                }


            }

            if (prioridad1 != null)
            {
                var objetoObtenido = prioridads.FindAll(x => x.Nombre.ToLower() == prioridad1.ToLower());
                PrioridadDTO objeto = objetoObtenido.FirstOrDefault();

                precioCotizacion = precioCotizacion + objeto.Coeficiente * factor;

            }

            if (tipoGolpe1 != null)
            {
                var objetoObtenido = tipoGolpes.FindAll(x => x.Nombre.ToLower() == tipoGolpe1.ToLower());
                TipoGolpeDTO objeto = objetoObtenido.FirstOrDefault();

                precioCotizacion = precioCotizacion + objeto.Coeficiente * factor;
            }

            if (datosGolpes1 != null)
            {
                foreach (var golpe in datosGolpes1)
                {
                    var objetoObtenidoPieza = piezas.FindAll(x => x.Nombre.ToLower() == golpe.PiezaNombre.ToLower());
                    if (objetoObtenidoPieza.Count != 0)
                    {
                        PiezaDTO objetoPieza = objetoObtenidoPieza.FirstOrDefault();
                        precioCotizacion = precioCotizacion + objetoPieza.Coeficiente * factor;
                    }





                    var objetoObtenidoMedidaHorizontal = medidas.FindAll(x => x.TipoMedidaNombre.ToLower() == "Diámetro horizontal".ToLower() && x.Descripcion.ToLower() == golpe.DiametroH.ToLower());
                    if (objetoObtenidoMedidaHorizontal.Count != 0)
                    {
                        MedidaDTO objetoMedidaHorizontal = objetoObtenidoMedidaHorizontal.FirstOrDefault();
                        precioCotizacion = precioCotizacion + objetoMedidaHorizontal.Coeficiente * factor;

                    }


                    var objetoObtenidoMedidaVertical = medidas.FindAll(x => x.TipoMedidaNombre.ToLower() == "Diámetro vertical".ToLower() && x.Descripcion.ToLower() == golpe.DiametroV.ToLower());
                    if (objetoObtenidoMedidaVertical.Count != 0)
                    {
                        MedidaDTO objetoMedidaVerical = objetoObtenidoMedidaVertical.FirstOrDefault();
                        precioCotizacion = precioCotizacion + objetoMedidaVerical.Coeficiente * factor;

                    }


                    if (golpe.ProfundidadGolpe != null)
                    {
                        var objetoObtenidoMedidaProfundidad = medidas.FindAll(x => x.TipoMedidaNombre.ToLower() == "Profundidad".ToLower() && x.Descripcion.ToLower() == golpe.ProfundidadGolpe.ToLower());
                        if (objetoObtenidoMedidaProfundidad.Count != 0)
                        {
                            MedidaDTO objetoMedidaProfundidad = objetoObtenidoMedidaProfundidad.FirstOrDefault();
                            precioCotizacion = precioCotizacion + objetoMedidaProfundidad.Coeficiente * factor;

                        }
                    }


                }
            }

            if (partesSeleccionadasPE1 != null)
            {
                foreach (var piezaPE in partesSeleccionadasPE1)
                {
                    var objetoObtenido = piezas.FindAll(x => x.Nombre.ToLower() == piezaPE.ToLower());
                    PiezaDTO objeto = objetoObtenido.FirstOrDefault();

                    precioCotizacion = precioCotizacion + objeto.Coeficiente * factor;

                }
            }
            if (tipoServicioPaquetes != null)
            {
                if (tipoServicioPaquetes.Count != 0)
                {
                    if (paquete1 != null)
                    {
                        var objetoObtenido = tipoServicioPaquetes.FindAll(x => x.TipoServicioNombre.ToLower() == servicio1.ToLower() && x.PaqueteNombre.ToLower() == paquete1.ToLower());

                        if (objetoObtenido.Count != 0)
                        {
                            TipoServicioPaqueteDTO objeto = objetoObtenido.FirstOrDefault();
                            precioCotizacion = precioCotizacion + objeto.Precio;
                        }

                    }





                }
            }
            if (opcionTodo1 != null || opcionTodo1 != "")
            {

                if (opcionTodo1 == "Pintura")
                {
                    var objetoObtenidoPieza = piezas.FindAll(x => x.Nombre.ToLower() == "Todo el vehículo".ToLower() && x.TipoPiezaNombre.ToLower() == "Pintura".ToLower());
                    if (objetoObtenidoPieza.Count != 0)
                    {
                        PiezaDTO objetoPieza = objetoObtenidoPieza.FirstOrDefault();
                        precioCotizacion = precioCotizacion + objetoPieza.Coeficiente * factor;
                    }

                }
                else if (opcionTodo1 == "PulidoEncerado")
                {
                    var objetoObtenidoPieza = piezas.FindAll(x => x.Nombre.ToLower() == "Todo el vehículo".ToLower() && x.TipoPiezaNombre.ToLower() == "Pulido y encerado".ToLower());
                    if (objetoObtenidoPieza.Count != 0)
                    {
                        PiezaDTO objetoPieza = objetoObtenidoPieza.FirstOrDefault();
                        precioCotizacion = precioCotizacion + objetoPieza.Coeficiente * factor;
                    }
                }
            }

            if (precioCotizacion == 0)
            {
                presupustoL.IsVisible = false;
                presupuestoLabel.IsVisible = false;
            }

            presupuestoLabel.Text = "$ " + precioCotizacion.ToString() + " MX";

        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            confirmarBtn.IsEnabled = true;
        }
        private async void ButtonConfirmar(object sender, EventArgs e)
        {
            confirmarBtn.IsEnabled = false;
            await Navigation.PushAsync(new DatosUsuario(datosAuto1, prioridad1, servicio1, tipoGolpe1, datosGolpes1, paquete1, taller1, fecha1, hora1, partesSeleccionadasPE1, precioCotizacion.ToString(), opcionTodo1, imagenesGolpeFuerte1, imagenesTodoElVehiculo1));
        }

        private async void ButtonCancelar(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
    }
}