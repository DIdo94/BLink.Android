using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace BLink.Business.Common
{
    public static class AppConstants
    {
        public static string UserImagesPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        public static string MainPhotoFormat = "main_Photo.jpeg";
    }
}