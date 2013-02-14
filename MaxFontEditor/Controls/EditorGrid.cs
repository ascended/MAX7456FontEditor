using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using MaxFontEditor.Data;
using System.Collections.ObjectModel;
using System.Windows;
using System.Collections.Specialized;
using Caliburn.Micro;
using System.Windows.Media;

namespace MaxFontEditor.Controls
{
    class EditorGlyph : PropertyChangedBase
    {
        Glyph editingGlyph;
        public Glyph EditingGlyph
        {
            get { return editingGlyph; }
            set
            {
                if (editingGlyph == value)
                    return;
                editingGlyph = value;
                NotifyOfPropertyChange(() => EditingGlyph);
            }
        }

        int row;
        public int Row
        {
            get { return row; }
            set
            {
                if (row == value)
                    return;
                row = value;
                NotifyOfPropertyChange(() => Row);
            }
        }

        int column;
        public int Column
        {
            get { return column; }
            set
            {
                if (column == value)
                    return;
                column = value;
                NotifyOfPropertyChange(() => Column);
            }
        }

        public EditorGlyph(Glyph glyph, int row, int column)
        {
            EditingGlyph = glyph;
            Row = row;
            Column = column;
        }
    }

    public class EditorGrid : Grid
    {
        readonly NotifyCollectionChangedEventHandler glyphsChanged = null;

        readonly ObservableCollection<EditorGlyph> items;

        public EditorGrid()
        {
            items = new ObservableCollection<EditorGlyph>();
            items.CollectionChanged += (sender, e) =>
                {
                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems)
                        {
                            foreach (var child in Children)
                            {
                                if (((ContentControl)child).Content == ((EditorGlyph)item).EditingGlyph)
                                {
                                    Children.Remove(child as ContentControl);
                                    break;
                                }
                            }
                        }
                    }

                    if (e.NewItems != null)
                    {
                        foreach (var item in e.NewItems)
                        {
                            EditorGlyph eg = item as EditorGlyph;
                            
                            if (eg == null)
                                continue;

                            ContentControl control = new ContentControl();
                            control.Content = eg.EditingGlyph;
                            
                            control.SetValue(Grid.RowProperty, eg.Row);
                            control.SetValue(Grid.ColumnProperty, eg.Column);

                            Children.Add(control);
                        }
                    }
                };

            glyphsChanged = (sender, e) =>
                {
                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems)
                        {
                            Glyph g = item as Glyph;

                            var match = items.Where(gi => gi.EditingGlyph == g).FirstOrDefault();
                            if (match == null)
                                continue;

                            items.Remove(match);
                        }
                    }

                    // cleanup positions
                    if (items.Count > 0)
                    {
                        var sort = items.OrderBy(i => i.Row);

                        for (int i = 0; i <= (items.Count / ColumnCount); i++)
                        {
                            var row = sort.Skip(i * ColumnCount).Take(ColumnCount);
                            int c = 0;

                            row.Where(gi => gi.Row == i).OrderBy(gi => gi.Column).ForEach(gi => { gi.Column = c++; });
                            row.Where(gi => gi.Row != i).ForEach(gi => { gi.Column = c++; gi.Row = i; });
                        }
                    }

                    if (e.NewItems != null)
                    {
                        foreach (var item in e.NewItems)
                        {
                            Glyph g = item as Glyph;
                            items.Add(new EditorGlyph(g, (items.Count / ColumnCount), items.Count % ColumnCount));
                        }
                    }

                    CorrectRowCount();
                };

            CorrectColumnCount();
        }

        private void CorrectRowCount()
        {
            int maxRow = items.Count == 0 ? 1 : items.Max(gi => gi.Row) + 1;

            if (RowDefinitions.Count == maxRow)
                return;

            // do not care if there are more row definitions than rows...
            for (int i = RowDefinitions.Count; i < maxRow; i++)
            {
                RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            }
        }

        private void CorrectColumnCount()
        {
            if (ColumnDefinitions.Count == ColumnCount)
                return;

            // do not care if there are more column definitions than columns...
            for (int i = ColumnDefinitions.Count; i < ColumnCount + 1; i++)
            {
                ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        public ObservableCollection<Glyph> GlyphsSource
        {
            get { return (ObservableCollection<Glyph>)GetValue(GlyphsSourceProperty); }
            set { SetValue(GlyphsSourceProperty, value); }
        }
        public static readonly DependencyProperty GlyphsSourceProperty = DependencyProperty.Register("GlyphsSource", typeof(ObservableCollection<Glyph>), typeof(EditorGrid), new FrameworkPropertyMetadata(OnGlyphsSourceChanged));

        public int ColumnCount
        {
            get { return (int)GetValue(ColumnCountProperty); }
            set { SetValue(ColumnCountProperty, value); }
        }
        public static readonly DependencyProperty ColumnCountProperty = DependencyProperty.Register("ColumnCount", typeof(int), typeof(EditorGrid), new FrameworkPropertyMetadata(3, OnColumnCountChanged));

        public ActiveBrush Brush
        {
            get { return (ActiveBrush)GetValue(BrushProperty); }
            set { SetValue(BrushProperty, value); }
        }
        public static readonly DependencyProperty BrushProperty = DependencyProperty.Register("Brush", typeof(ActiveBrush), typeof(EditorGrid));

        private static void OnColumnCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EditorGrid grid = d as EditorGrid;

            grid.CorrectColumnCount();
        }

        private static void OnGlyphsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EditorGrid grid = d as EditorGrid;

            grid.items.Clear();

            if (e.OldValue != null)
                ((ObservableCollection<Glyph>)e.OldValue).CollectionChanged -= grid.glyphsChanged;
            if (e.NewValue != null)
                ((ObservableCollection<Glyph>)e.NewValue).CollectionChanged += grid.glyphsChanged;

            int col = 0;
            int row = 0;
            foreach (var item in ((ObservableCollection<Glyph>)e.NewValue))
            {
                grid.items.Add(new EditorGlyph(item, row, col));
                
                col++;

                if (col >= grid.ColumnCount)
                {
                    col = 0;
                    row++;
                }
            }
        }
    }
}
