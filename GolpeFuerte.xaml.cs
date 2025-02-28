using AplicacionAuto.Clases;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace AplicacionAuto
{
    public partial class GolpeFuerte : ContentPage
    {
        private List<Image> selectedImages;

        DatosAuto datosAuto1;
        String prioridad1;
        String servicio1;
        String tipoGolpe1;
        String paquete1;
        String opcionTodo1;

        List<GolpeMedida> datosGolpes1;
        List<Imagen> imagenesGolpeFuerte = new List<Imagen>();
        List<Imagen> imagenesTodoElVehiculo;
        private List<string> partesSeleccionadasPE;


        public GolpeFuerte(DatosAuto datosAuto, String prioridad, String servicio, String tipoGolpe)
        {
            InitializeComponent();
            datosAuto1 = datosAuto;
            prioridad1 = prioridad;
            servicio1 = servicio;
            tipoGolpe1 = tipoGolpe;

            selectedImages = new List<Image>();
        }

        private async void selectedImage_Clicked(object sender, EventArgs e)
        {
            var options = new PickOptions
            {
                FileTypes = FilePickerFileType.Images,
                PickerTitle = "Seleccionar imágenes"
            };

            var result = await FilePicker.PickMultipleAsync(options);
            byte[] imageData;
            String sourceImagen;
            if (result != null)
            {
                selectedImage1.Source = null;
                selectedImage2.Source = null;
                selectedImage3.Source = null;
                selectedImages.Clear();

                int imagesCount = result.Count() > 3 ? 3 : result.Count();

                for (int i = 0; i < imagesCount; i++)
                {
                    var image = new Image
                    {
                        Source = ImageSource.FromFile(result.ElementAt(i).FullPath),
                        WidthRequest = 100,
                        HeightRequest = 100
                    };

                    sourceImagen = result.ElementAt(i).FullPath;
                    using (FileStream stream = new FileStream(sourceImagen, FileMode.Open, FileAccess.Read))
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

                    var imagen = new Imagen
                    {
                        DatoImagen = imageData
                    };

                    imagenesGolpeFuerte.Add(imagen);
                    selectedImages.Add(image);

                    if (i == 0)
                        selectedImage1.Source = image.Source;
                    else if (i == 1)
                        selectedImage2.Source = image.Source;
                    else if (i == 2)
                        selectedImage3.Source = image.Source;
                }

                continueButton.IsEnabled = imagesCount > 0;

                // En MAUI ya no es necesario establecer la posición del botón con Grid.SetRow
                // Ya que estamos usando VerticalStackLayout
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Ocultar la barra de navegación programáticamente
            if (Navigation.NavigationStack.Count > 0)
            {
                Microsoft.Maui.Controls.NavigationPage.SetHasNavigationBar(this, false);
            }
            continueButton.IsEnabled = true;
        }

        private async void ButtonContinuar(object sender, EventArgs e)
        {
            continueButton.IsEnabled = false;
            await Navigation.PushAsync(new Mapa(datosAuto1, prioridad1, servicio1, opcionTodo1, tipoGolpe1, datosGolpes1, paquete1, imagenesGolpeFuerte, partesSeleccionadasPE, imagenesTodoElVehiculo));
        }

        private async void OnImageTapped(object sender, EventArgs e)
        {
            string descripcion = "Hola! En esta sección, simplemente adjunta las fotografías de las partes afectadas de tu vehículo. Tu colaboración al proporcionar imágenes claras nos ayudará a comprender mejor el daño y ofrecerte un servicio de hojalatería y pintura más preciso. ¡Gracias por tu participación";
            await DisplayAlert("Ayuda", descripcion, "Aceptar");
        }
    }
}