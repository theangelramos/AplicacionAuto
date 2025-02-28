using AplicacionAuto.Clases;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace AplicacionAuto
{
    public partial class PulirTodoVehiculo : ContentPage
    {
        private List<Image> selectedImages;

        DatosAuto datosAuto1;
        String prioridad1;
        String servicio1;
        String tipoGolpe1;
        String opcionTodo1;
        private List<string> partesSeleccionadasPE;

        List<Imagen> imagenesTodoElVehiculo = new List<Imagen>();

        public PulirTodoVehiculo(DatosAuto datosAuto, String prioridad, String servicio, String opcionTodo)
        {
            datosAuto1 = datosAuto;
            prioridad1 = prioridad;
            servicio1 = servicio;
            opcionTodo1 = opcionTodo;
            InitializeComponent();
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

                    imagenesTodoElVehiculo.Add(imagen);
                    selectedImages.Add(image);

                    if (i == 0)
                        selectedImage1.Source = image.Source;
                    else if (i == 1)
                        selectedImage2.Source = image.Source;
                    else if (i == 2)
                        selectedImage3.Source = image.Source;
                }

                continueButton.IsEnabled = imagesCount > 0;

                // En MAUI ya no necesitamos usar Grid.SetRow con VerticalStackLayout
                // Las imágenes ya están en el orden correcto en el contenedor
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Ocultar la barra de navegación desde el código
            if (Navigation.NavigationStack.Count > 0)
            {
                Microsoft.Maui.Controls.NavigationPage.SetHasNavigationBar(this, false);
            }
            continueButton.IsEnabled = true;
        }

        private async void ButtonPulirContinuar(object sender, EventArgs e)
        {
            continueButton.IsEnabled = false;
            await Navigation.PushAsync(new PaquetePulidoEncerado(datosAuto1, prioridad1, servicio1, tipoGolpe1, partesSeleccionadasPE, opcionTodo1, imagenesTodoElVehiculo));
        }
    }
}