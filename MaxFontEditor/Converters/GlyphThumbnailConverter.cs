using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Data;
using System.Drawing;
using MaxFontEditor.Data;
using System.IO;
using System.Drawing.Imaging;

namespace MaxFontEditor.Converters
{
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    public class GlyphThumbnailConverter : MarkupExtension, IValueConverter
    {
        public GlyphThumbnailConverter()
        {
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType == typeof(System.Windows.Media.ImageSource))
            {
                if (value == null)
                    return null;

                System.Windows.Media.Imaging.BitmapImage bi = new System.Windows.Media.Imaging.BitmapImage();
                
                Bitmap bm = value as Bitmap;
                MemoryStream ms = new MemoryStream();
                bm.Save(ms, ImageFormat.Png);
                ms.Seek(0, SeekOrigin.Begin);

                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();

                return bi;
            }
            else
            {
                return ((string)value);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
