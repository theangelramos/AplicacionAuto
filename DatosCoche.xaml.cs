using AplicacionAuto.Clases;
using HTTPupt;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// CAMBIO 1: Reemplazar los using de Xamarin por los de MAUI
using Microsoft.Maui.Networking;    // Antes: Xamarin.Essentials
using Microsoft.Maui.Storage;       // Antes: Xamarin.Essentials.Preferences
using Microsoft.Maui.Devices;       // Para DeviceInfo
using Microsoft.Maui.Controls;      // Antes: Xamarin.Forms
using Microsoft.Maui.Controls.Xaml; // Antes: Xamarin.Forms.Xaml

namespace AplicacionAuto
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DatosCoche : ContentPage
    {
        DatosAuto datosAuto1;
        String prioridad1;
        String servicio1;
        String tipoGolpe1;
        String paquete1;
        String opcionTodo1;
        List<GolpeMedida> datosGolpes1;
        PeticionHTTP peticion = new PeticionHTTP(Urls.UrlServer());

        public List<SubmarcaDTO> submarcas;
        public List<MarcaDTO> marcas;
        public List<CocheDTO> coches;
        public List<ColorDTO> colores;
        public List<AcabadoDTO> acabados;
        private List<string> partesSeleccionadasPE;
        List<Imagen> imagenesTodoElVehiculo1;

        DatosAuto datosAuto;

        List<Imagen> imagenesGolpeFuerte = new List<Imagen>();

        static Boolean alerta = false;
        public DatosCoche()
        {
            InitializeComponent();


            LoginInicio login = new LoginInicio
            {
                CorreoElectronico = "appMovil@gmail.com",
                Password = "AutoUniverseUpt2023Obb"
            };

            // CAMBIO 2: Actualizar verificación de conexión a internet
            bool tieneConexionInternet = NetworkAccess.Internet == NetworkAccess.Internet;

            if (!tieneConexionInternet)
            {
                CerrarApp("Sin conexión", "Conéctate a una red para obtener los datos");
            }
            else
            {
                String jsonRecibir;
                try
                {
                    String json = JsonConvertidor.Objeto_Json(login);
                    peticion.PedirComunicacion("Authorization/PedirToken", MetodoHTTP.POST, TipoContenido.JSON);
                    peticion.enviarDatos(json);
                    json = peticion.ObtenerJson();

                    if (json != null)
                    {
                        // CAMBIO 3: Actualizar uso de Preferences
                        Preferences.Default.Set("token", json.Trim('\"'));
                    }

                    // CAMBIO 3: Actualizar uso de Preferences en todas las peticiones
                    peticion.PedirComunicacion("marca/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Default.Get("token", ""));
                    jsonRecibir = peticion.ObtenerJson();
                    marcas = JsonConvertidor.Json_ListaObjeto<MarcaDTO>(jsonRecibir);

                    foreach (var marca in marcas)
                    {
                        MarcaPicker.Items.Add(marca.Nombre);
                    }

                    peticion.PedirComunicacion("Submarca/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Default.Get("token", ""));
                    jsonRecibir = peticion.ObtenerJson();
                    submarcas = JsonConvertidor.Json_ListaObjeto<SubmarcaDTO>(jsonRecibir);

                    peticion.PedirComunicacion("Coche/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Default.Get("token", ""));
                    jsonRecibir = peticion.ObtenerJson();
                    coches = JsonConvertidor.Json_ListaObjeto<CocheDTO>(jsonRecibir);

                    peticion.PedirComunicacion("Color/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Default.Get("token", ""));
                    jsonRecibir = peticion.ObtenerJson();
                    colores = JsonConvertidor.Json_ListaObjeto<ColorDTO>(jsonRecibir);

                    peticion.PedirComunicacion("Acabado/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Default.Get("token", ""));
                    jsonRecibir = peticion.ObtenerJson();
                    acabados = JsonConvertidor.Json_ListaObjeto<AcabadoDTO>(jsonRecibir);
                }
                catch (Exception)
                {
                    CerrarApp("Error en la petición al servidor", "\n\nAsegúrate de tener una conexión estable a internet.");
                }
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            DatosAutoSiguiente.IsEnabled = true;
            Noexiste.IsEnabled = true;
        }

        public async void CerrarApp(String titulo, String mensaje)
        {
            if (alerta == false)
            {
                await DisplayAlert(titulo, mensaje, "Aceptar");
                alerta = true;
            }

            // CAMBIO 4: Actualizar verificación de plataforma
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
            }
        }

        private async void DatosAutoSiguiente_Clicked(object sender, EventArgs e)
        {
            if (MarcaPicker.SelectedItem != null && SubmarcaPicker.SelectedItem != null && ModeloPicker.SelectedItem != null && TipoPicker.SelectedItem != null
                && VersionPicker.SelectedItem != null && ColorPicker.SelectedItem != null && AcabadoPicker.SelectedItem != null)
            {
                datosAuto = new DatosAuto
                {
                    Marca = MarcaPicker.SelectedItem.ToString(),
                    Submarca = SubmarcaPicker.SelectedItem.ToString(),
                    Modelo = ModeloPicker.SelectedItem.ToString(),
                    Tipo = TipoPicker.SelectedItem.ToString(),
                    Version = VersionPicker.SelectedItem.ToString(),
                    Color = ColorPicker.SelectedItem.ToString(),
                    Acabado = AcabadoPicker.SelectedItem.ToString()
                };
                DatosAutoSiguiente.IsEnabled = false;

                await Navigation.PushAsync(new TipoPrioridad(datosAuto));
            }
            else
            {
                string descripcion = "¡Debes llenar todos los campos!";
                await DisplayAlert("Datos incompletos", descripcion, "Aceptar");
            }
        }

        private async void NoexisteClicked(object sender, EventArgs e)
        {
            Noexiste.IsEnabled = false;
            await Navigation.PushAsync(new Mapa(datosAuto1, prioridad1, servicio1, opcionTodo1, tipoGolpe1, datosGolpes1, paquete1, imagenesGolpeFuerte, partesSeleccionadasPE, imagenesTodoElVehiculo1));
        }

        private async void OnImageTapped(object sender, EventArgs e)
        {
            string descripcion = "Si no encuentras los datos de tu carro en las opciones , es recomendable que acudas a un taller automotriz. Los profesionales de servicio pueden ayudarte a identificar y proporcionar la información correcta sobre tu vehículo. ¡No dudes en obtener asesoramiento experto para garantizar un mantenimiento adecuado y seguro de tu carro!";
            await DisplayAlert("Ayuda", descripcion, "Aceptar");
        }

        private void MarcaPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            ColorPicker.Items.Clear();
            AcabadoPicker.Items.Clear();
            SubmarcaPicker.Items.Clear();
            ModeloPicker.Items.Clear();

            // CAMBIO 3: Actualizar uso de Preferences
            var dato = Preferences.Default.Get("token", "");

            var submarcasID = submarcas.FindAll(x => x.MarcaNombre.ToLower() == MarcaPicker.SelectedItem.ToString().ToLower());

            foreach (var submarca in submarcasID)
            {
                SubmarcaPicker.Items.Add(submarca.Nombre);
            }
        }

        List<string> modelos = new List<string>();
        private void SubmarcaPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            ColorPicker.Items.Clear();
            AcabadoPicker.Items.Clear();
            ModeloPicker.Items.Clear();
            TipoPicker.Items.Clear();
            modelos.Clear();

            var cochesFiltrados = coches.FindAll(x => x.MarcaNombre.ToLower() == MarcaPicker.SelectedItem?.ToString().ToLower() && x.SubmarcaNombre.ToLower() == SubmarcaPicker.SelectedItem?.ToString().ToLower());

            foreach (var coche in cochesFiltrados)
            {
                modelos.Add($"{coche.ModeloNombre}");
            }
            modelos = modelos.Distinct().ToList();
            foreach (var modelo in modelos)
            {
                ModeloPicker.Items.Add(modelo);
            }
        }

        List<string> tipos = new List<string>();
        private void ModeloPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            ColorPicker.Items.Clear();
            AcabadoPicker.Items.Clear();
            TipoPicker.Items.Clear();
            VersionPicker.Items.Clear();
            tipos.Clear();

            var cochesFiltrados = coches.FindAll(x => x.MarcaNombre.ToLower() == MarcaPicker.SelectedItem?.ToString().ToLower() && x.SubmarcaNombre.ToLower() == SubmarcaPicker.SelectedItem?.ToString().ToLower() && x.ModeloNombre == ModeloPicker.SelectedItem?.ToString().ToLower());

            foreach (var coche in cochesFiltrados)
            {
                tipos.Add($"{coche.TipoNombre}");
            }

            tipos = tipos.Distinct().ToList();

            foreach (var tipo in tipos)
            {
                TipoPicker.Items.Add(tipo);
            }
        }

        private void TipoPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            ColorPicker.Items.Clear();
            AcabadoPicker.Items.Clear();
            VersionPicker.Items.Clear();
            var cochesFiltrados = coches.FindAll(x => x.MarcaNombre.ToLower() == MarcaPicker.SelectedItem?.ToString().ToLower() && x.SubmarcaNombre.ToLower() == SubmarcaPicker.SelectedItem?.ToString().ToLower() && x.ModeloNombre == ModeloPicker.SelectedItem?.ToString().ToLower() && x.TipoNombre == TipoPicker.SelectedItem?.ToString());

            foreach (var coche in cochesFiltrados)
            {
                VersionPicker.Items.Add(coche.Version);
            }
        }

        private void VersionPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            AcabadoPicker.Items.Clear();
            ColorPicker.Items.Clear();

            foreach (var color in colores)
            {
                ColorPicker.Items.Add(color.Nombre);
            }
        }

        private void ColorPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            AcabadoPicker.Items.Clear();

            foreach (var acabado in acabados)
            {
                AcabadoPicker.Items.Add(acabado.Nombre);
            }
        }

        private void AcabadoPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ColorPicker.SelectedItem != null)
            {
                DatosAutoSiguiente.IsEnabled = true;
            }
            else
            {
                DatosAutoSiguiente.IsEnabled = false;
            }
        }
    }
}