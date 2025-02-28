using AplicacionAuto.Clases;
using HTTPupt;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Networking;
using Microsoft.Maui.Storage;

namespace AplicacionAuto
{
    public partial class GolpeLeveMedio : ContentPage
    {
        PeticionHTTP peticion = new PeticionHTTP(Urls.UrlServer());
        static Boolean alerta = false;
        private ObservableCollection<GolpeMedida> medidasList;

        private int golpeCounter = 1;

        private string selectedImagePath;

        DatosAuto datosAuto1;
        String prioridad1;
        String servicio1;
        String tipoGolpe1;

        public byte[] imageData;
        public GolpeLeveMedio(DatosAuto datosAuto, String prioridad, String servicio, String tipoGolpe)
        {
            InitializeComponent();
            datosAuto1 = datosAuto;
            prioridad1 = prioridad;
            servicio1 = servicio;
            tipoGolpe1 = tipoGolpe;

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
                    peticion.PedirComunicacion("Pieza/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Get("token", ""));
                    jsonRecibir = peticion.ObtenerJson();
                    List<PiezaDTO> piezas = JsonConvertidor.Json_ListaObjeto<PiezaDTO>(jsonRecibir);

                    var piezasFiltradas = piezas.FindAll(x => x.TipoPiezaNombre.ToLower() == "Hojalatería y pintura".ToLower());

                    foreach (var pieza in piezasFiltradas)
                    {
                        pickerPartes.Items.Add(pieza.Nombre);
                    }
                }
                catch (Exception)
                {
                    CerrarApp("Error en la petición al servidor", "\n\nAsegúrate de tener una conexión estable a internet.");
                }

                try
                {
                    peticion.PedirComunicacion("Medida/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Get("token", ""));
                    jsonRecibir = peticion.ObtenerJson();
                    List<MedidaDTO> medidas = JsonConvertidor.Json_ListaObjeto<MedidaDTO>(jsonRecibir);

                    var medidasFiltradas = medidas.FindAll(x => x.TipoMedidaNombre.ToLower() == "Diámetro horizontal hojalatería".ToLower());

                    foreach (var medida in medidasFiltradas)
                    {
                        pickerMedidaHorizontal.Items.Add(medida.Descripcion);
                    }
                }
                catch (Exception)
                {
                    throw;
                }

                try
                {
                    peticion.PedirComunicacion("Medida/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Get("token", ""));
                    jsonRecibir = peticion.ObtenerJson();
                    List<MedidaDTO> medidas = JsonConvertidor.Json_ListaObjeto<MedidaDTO>(jsonRecibir);

                    var medidasFiltradas = medidas.FindAll(x => x.TipoMedidaNombre.ToLower() == "Diámetro vertical hojalatería".ToLower());

                    foreach (var medida in medidasFiltradas)
                    {
                        pickerMedidaVertical.Items.Add(medida.Descripcion);
                    }
                }
                catch (Exception)
                {
                    throw;
                }

                try
                {
                    peticion.PedirComunicacion("Medida/Obtener", MetodoHTTP.GET, TipoContenido.JSON, Preferences.Get("token", ""));
                    jsonRecibir = peticion.ObtenerJson();
                    List<MedidaDTO> medidas = JsonConvertidor.Json_ListaObjeto<MedidaDTO>(jsonRecibir);

                    var medidasFiltradas = medidas.FindAll(x => x.TipoMedidaNombre.ToLower() == "Profundidad".ToLower());

                    foreach (var medida in medidasFiltradas)
                    {
                        pickerMedidaProfundidad.Items.Add(medida.Descripcion);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }

            medidasList = new ObservableCollection<GolpeMedida>();
            collectionView.ItemsSource = medidasList;
        }

        public async void CerrarApp(String titulo, String mensaje)
        {
            if (alerta == false)
            {
                await DisplayAlert(titulo, mensaje, "Aceptar");
                alerta = true;
            }
        }

        private async void selectedImage_Clicked(object sender, EventArgs e)
        {
            var options = new PickOptions
            {
                FileTypes = FilePickerFileType.Images,
                PickerTitle = "Seleccionar imagen"
            };

            var result = await FilePicker.PickAsync(options);

            if (result != null)
            {
                selectedImagePath = result.FullPath;
                selectedImage.Source = ImageSource.FromFile(result.FullPath);

                using (FileStream stream = new FileStream(selectedImagePath, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        imageData = reader.ReadBytes((int)stream.Length);

                        String imageDataString = imageData.ToString();
                        String imageDataString2 = Convert.ToBase64String(imageData);
                        Console.WriteLine(imageDataString);
                        Console.WriteLine(imageDataString2);
                    }
                }
            }
        }

        private void AgregarGolpe_Clicked(object sender, EventArgs e)
        {
            string opcionSeleccionada = pickerPartes.SelectedItem as string;
            string opcionSeleccionada2 = pickerMedidaHorizontal.SelectedItem as string;
            string opcionSeleccionada3 = pickerMedidaVertical.SelectedItem as string;
            string opcionSeleccionada4 = pickerMedidaProfundidad.SelectedItem as string;

            if (!string.IsNullOrEmpty(opcionSeleccionada) &&
                !string.IsNullOrEmpty(opcionSeleccionada2) &&
                !string.IsNullOrEmpty(opcionSeleccionada3) &&
                !string.IsNullOrEmpty(opcionSeleccionada4) &&
                !string.IsNullOrEmpty(selectedImagePath))
            {
                string medida = $"Diámetro horizontal: {opcionSeleccionada2}  \nDiámetro vertical: {opcionSeleccionada3} \nProfundidad: {opcionSeleccionada4}\n";

                GolpeMedida golpeMedida = new GolpeMedida
                {
                    ImagenPath = selectedImagePath,
                    DatoImagen = imageData,
                    PiezaNombre = opcionSeleccionada,
                    EtiquetaPiezaNombre = $"Pieza: {opcionSeleccionada}",
                    DiametroH = opcionSeleccionada2,
                    DiametroV = opcionSeleccionada3,
                    ProfundidadGolpe = opcionSeleccionada4,
                    EtiquetaDatos = medida
                };

                medidasList.Add(golpeMedida);

                pickerPartes.SelectedItem = null;
                pickerMedidaHorizontal.SelectedItem = null;
                pickerMedidaVertical.SelectedItem = null;
                pickerMedidaProfundidad.SelectedItem = null;
                selectedImage.Source = null;
                selectedImagePath = null;

                siguienteButton.IsVisible = medidasList.Count > 0;
            }
            else
            {
                DisplayAlert("Datos incompletos", "Ingresa todas las medidas y selecciona una imagen", "OK");
            }
        }

        private async void OnImageTapped(object sender, EventArgs e)
        {
            string descripcion = "¡Importante! Para un registro preciso, asegúrate de tener fotos claras del golpe en tu dispositivo. Esto nos ayudará a evaluar mejor el daño. Además, proporciona las medidas en centímetros del golpe, tanto horizontal como verticalmente. ¡Tu colaboración nos permite brindarte un servicio más preciso y eficiente en la hojalatería y pintura de tu vehículo!";

            await DisplayAlert("Ayuda", descripcion, "Aceptar");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Ocultar la barra de navegación programáticamente
            if (Navigation.NavigationStack.Count > 0)
            {
                Microsoft.Maui.Controls.NavigationPage.SetHasNavigationBar(this, false);
            }
            siguienteButton.IsEnabled = true;

            // Configurar el CollectionView programáticamente para evitar problemas con DataTemplate
            CustomizeCollectionView();
        }

        private void CustomizeCollectionView()
        {
            // Crear un DataTemplate programáticamente para evitar problemas con XAML
            collectionView.ItemTemplate = new DataTemplate(() =>
            {
                var stack = new StackLayout
                {
                    Spacing = 10,
                    Padding = new Thickness(5)
                };

                var image = new Image
                {
                    HeightRequest = 150,
                    Aspect = Aspect.AspectFit
                };
                image.SetBinding(Image.SourceProperty, "ImagenPath");

                var labelPieza = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Center,
                    TextColor = Colors.Red
                };
                labelPieza.SetBinding(Label.TextProperty, "EtiquetaPiezaNombre");

                var labelDatos = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Center,
                    TextColor = Colors.Black
                };
                labelDatos.SetBinding(Label.TextProperty, "EtiquetaDatos");

                stack.Add(image);
                stack.Add(labelPieza);
                stack.Add(labelDatos);

                return stack;
            });
        }

        private async void Button_Guardar(object sender, EventArgs e)
        {
            siguienteButton.IsEnabled = false;
            await Navigation.PushAsync(new PaqueteHojalateriaPintura(datosAuto1, prioridad1, servicio1, tipoGolpe1, medidasList));
        }
    }
}