using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using MaxFontEditor.Data;
using System.Windows.Shapes;
using System.Windows.Media;
using MaxFontEditor.Framework;

namespace MaxFontEditor.Controls
{
    public enum ActiveBrush
    {
        Transparent,
        White,
        Black
    }

    public class GlyphEditor : UserControl
    {
        private Canvas editorCanvas;
        private Border editorBorder;

        private DateTime timeout;

        public GlyphEditor()
        {
            editorCanvas = new Canvas();
            editorBorder = new Border();

            editorBorder.BorderThickness = new Thickness(0, 0, 1, 1);
            editorBorder.BorderBrush = BorderBrush;
            editorBorder.Background = TransparentBrush;
            editorBorder.Width = 12 * PixelSize + 1;
            editorBorder.Height = 18 * PixelSize + 1;

            editorBorder.Child = editorCanvas;
            this.AddChild(editorBorder);

            editorCanvas.MouseLeftButtonDown += editorCanvas_MouseLeftButtonDown;
            timeout = DateTime.Now.Subtract(new TimeSpan(10000));
        }

        void editorCanvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Item == null)
                return;

            if (IsReadOnly)
                return;

            Point cursor = e.GetPosition(editorCanvas);
            
            bool? value = null;
            if (Brush == ActiveBrush.Black)
                value = false;
            else if (Brush == ActiveBrush.White)
                value = true;

            Item.SetPixel((int)(cursor.X / PixelSize), (int)(cursor.Y / PixelSize), value);

            e.Handled = true;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();


            timeout = DateTime.Now.Subtract(new TimeSpan(10000));

            RedrawEditor();

            timeout = DateTime.Now.AddMilliseconds(500);
        }

        private void UpdateEditor()
        {
            if (Item == null)
                return;

            int r = 0;
            int c = 0;
            foreach (var row in Item.Rows)
            {
                c = 0;

                foreach (var col in row)
                {
                    Border b = UI.FindChild<Border>(editorCanvas, String.Format("b{0}x{1}", r, c));
                    if (b == null)
                        continue;

                    if (col == null)
                        b.Background = TransparentBrush;
                    else if (col == false)
                        b.Background = BlackBrush;
                    else if (col == true)
                        b.Background = WhiteBrush;
                    c++;
                }
                r++;
            }

        }

        private void RedrawEditor()
        {
            if (DateTime.Now < timeout)
                return;

            if (editorCanvas.Children.Count == 216)
            {
                UpdateEditor();
                return;
            }

            if (Item == null)
            {
                for (int r = 0; r < 18; r++)
                {
                    for (int c = 0; c < 12; c++)
                    {
                        Border b = new Border();
                        b.Name = String.Format("b{0}x{1}", r, c);
                        b.Width = PixelSize;
                        b.Height = PixelSize;
                        b.BorderThickness = new Thickness(1, 1, 0, 0);
                        b.BorderBrush = BorderBrush;
                        b.Background = TransparentBrush;
                        b.SetValue(Canvas.LeftProperty, c * PixelSize);
                        b.SetValue(Canvas.TopProperty, r * PixelSize);

                        editorCanvas.Children.Add(b);
                    }
                }
            }
            else
            {
                int r = 0;
                int c = 0;
                foreach (var row in Item.Rows)
                {
                    c = 0;

                    foreach (var col in row)
                    {
                        Border b = new Border();
                        b.Name = String.Format("b{0}x{1}", r, c);
                        b.Width = PixelSize;
                        b.Height = PixelSize;
                        b.BorderThickness = new Thickness(1, 1, 0, 0);
                        b.BorderBrush = BorderBrush;
                        b.SetValue(Canvas.LeftProperty, c * PixelSize);
                        b.SetValue(Canvas.TopProperty, r * PixelSize);

                        if (col == null)
                            b.Background = TransparentBrush;
                        else if (col == false)
                            b.Background = BlackBrush;
                        else if (col == true)
                            b.Background = WhiteBrush;

                        editorCanvas.Children.Add(b);

                        c++;
                    }
                    r++;
                }
            }
            
        }

        public ActiveBrush Brush
        {
            get { return (ActiveBrush)GetValue(BrushProperty); }
            set { SetValue(BrushProperty, value); }
        }
        public static readonly DependencyProperty BrushProperty = DependencyProperty.Register("Brush", typeof(ActiveBrush), typeof(GlyphEditor));

        public Glyph Item
        {
            get { return (Glyph)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }
        public static readonly DependencyProperty ItemProperty = DependencyProperty.Register("Item", typeof(Glyph), typeof(GlyphEditor), new FrameworkPropertyMetadata(OnGlyphChanged));

        public double PixelSize
        {
            get { return (double)GetValue(PixelSizeProperty); }
            set { SetValue(PixelSizeProperty, value); }
        }
        public static readonly DependencyProperty PixelSizeProperty = DependencyProperty.Register("PixelSize", typeof(double), typeof(GlyphEditor), new FrameworkPropertyMetadata(30d, OnPixelSizeChanged));

        public new Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }
        public static readonly new DependencyProperty BorderBrushProperty = DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(GlyphEditor), new FrameworkPropertyMetadata(Brushes.Black, OnBrushChanged));

        public Brush TransparentBrush
        {
            get { return (Brush)GetValue(TransparentBrushProperty); }
            set { SetValue(TransparentBrushProperty, value); }
        }
        public static readonly DependencyProperty TransparentBrushProperty = DependencyProperty.Register("TransparentBrush", typeof(Brush), typeof(GlyphEditor), new FrameworkPropertyMetadata(Brushes.Blue, OnBrushChanged));

        public Brush WhiteBrush
        {
            get { return (Brush)GetValue(WhiteBrushProperty); }
            set { SetValue(WhiteBrushProperty, value); }
        }
        public static readonly DependencyProperty WhiteBrushProperty = DependencyProperty.Register("WhiteBrush", typeof(Brush), typeof(GlyphEditor), new FrameworkPropertyMetadata(Brushes.White, OnBrushChanged));

        public Brush BlackBrush
        {
            get { return (Brush)GetValue(BlackBrushProperty); }
            set { SetValue(BlackBrushProperty, value); }
        }
        public static readonly DependencyProperty BlackBrushProperty = DependencyProperty.Register("BlackBrush", typeof(Brush), typeof(GlyphEditor), new FrameworkPropertyMetadata(Brushes.Black, OnBrushChanged));

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(GlyphEditor), new FrameworkPropertyMetadata(false));

        private static void OnPixelSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GlyphEditor c = (GlyphEditor)d;
            c.RedrawEditor();
        }

        private static void OnBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GlyphEditor c = (GlyphEditor)d;
            c.RedrawEditor();
        }

        private static void OnGlyphChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GlyphEditor c = (GlyphEditor)d;
            c.UpdateEditor();
            c.timeout = DateTime.Now.AddMilliseconds(500);

            Glyph newGlyph = e.NewValue as Glyph;
            newGlyph.PropertyChanged += (sender, ex) =>
                {
                    if (ex.PropertyName == "GlyphData")
                        c.UpdateEditor();
                };
        }
    }
}
