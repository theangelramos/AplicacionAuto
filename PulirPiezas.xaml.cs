using AplicacionAuto.Clases;
using HTTPupt;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Networking;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using Microsoft.Maui.Storage;

namespace AplicacionAuto
{
    public partial class PulirPiezas : ContentPage
    {
        PeticionHTTP peticion = new PeticionHTTP(Urls.UrlServer());
        static Boolean alerta = false;
        private List<string> partesSeleccionadasPE; 

        DatosAuto datosAuto1;
        String prioridad1;
        String servicio1;
        String tipoGolpe1;
        String opcionTodo1;
        List<Imagen> imagenesTodoElVehiculo;
        
        public PulirPiezas(DatosAuto datosAuto, String prioridad, String servicio)
        {
            InitializeComponent();
            datosAuto1 = datosAuto;
            prioridad1 = prioridad;
            servicio1 = servicio;
            
            // En .NET MAUI, la verificación de conectividad es diferente
            bool tieneConexionInternet = Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

            if (!tieneConexionInternet)
            {
                CerrarApp("Sin conexión", "Conéctate a una red para obtener los datos");
            }
            else
            {
                String jsonRecibir;
                try
                {
                    // Asumiendo que tu clase PeticionHTTP ya ha sido migrada a .NET MAUI
                    peticion.PedirComunicacion("Pieza/Obtener", MetodoHTTP.GET, TipoContenido.JSON, 
                        Preferences.Default.Get("token", ""));
                    jsonRecibir = peticion.ObtenerJson();
                    List<PiezaDTO> piezas = JsonConvertidor.Json_ListaObjeto<PiezaDTO>(jsonRecibir);

                    var piezasFiltradas = piezas.FindAll(x => x.TipoPiezaNombre.ToLower() == "Pulido y encerado".ToLower());

                    piezas = piezasFiltradas;

                    piezas.RemoveAll(p => p.Nombre == "Todo el vehículo");

                    BindingContext = new { piezas };
                }
                catch (Exception)
                {
                    CerrarApp("Error en la petición al servidor", "\n\nAsegúrate de tener una conexión estable a internet.");
                }
            }

            partesSeleccionadasPE = new List<string>(); 
        }

        public async void CerrarApp(String titulo, String mensaje)
        {
            if (alerta == false)
            {
                await DisplayAlert(titulo, mensaje, "Aceptar");
                alerta = true;
            }
        }

        private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            string parteCarro = GetCheckBoxText(checkBox); 

            if (e.Value) 
            {
                if (!partesSeleccionadasPE.Contains(parteCarro)) 
                {
                    partesSeleccionadasPE.Add(parteCarro); 
                }
            }
            else 
            {
                if (partesSeleccionadasPE.Contains(parteCarro)) 
                {
                    partesSeleccionadasPE.Remove(parteCarro); 
                }
            }
        }

        private string GetCheckBoxText(CheckBox checkBox)
        {
            // En MAUI, accedemos al padre usando Parent
            var horizontalStackLayout = checkBox.Parent as HorizontalStackLayout; 
            if (horizontalStackLayout != null)
            {
                // El Label es el primer elemento del HorizontalStackLayout
                var label = horizontalStackLayout.Children[0] as Label;
                if (label != null)
                {
                    return label.Text;
                }
            }
            return string.Empty;
        }

        private void ButtonGuardar_Clicked(object sender, EventArgs e)
        {
            // En MAUI, actualizamos la fuente de datos del CollectionView
            lstPartesSeleccionadasListView.ItemsSource = null; 
            lstPartesSeleccionadasListView.ItemsSource = partesSeleccionadasPE; 

            btnSiguiente.IsVisible = true; 
        }

        private async void ButtonSiguiente_Clicked(object sender, EventArgs e)
        {
            btnSiguiente.IsEnabled = false;
            await Navigation.PushAsync(new PaquetePulidoEncerado(datosAuto1, prioridad1, 
                servicio1, tipoGolpe1, partesSeleccionadasPE, opcionTodo1, imagenesTodoElVehiculo));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            btnSiguiente.IsEnabled = true;
        }
    }
}