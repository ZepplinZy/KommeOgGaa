using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KommeOgGaa.Controls
{
    public class PlaceholderBox : TextBox
    {



        public bool Error
        {
            get { return (bool)GetValue(ErrorProperty); }
            set { SetValue(ErrorProperty, value); }
        }



        public Visibility IsTagVisiable
        {
            get { return (Visibility)GetValue(IsTagVisiableProperty); }
            set { SetValue(IsTagVisiableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsTagVisiable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsTagVisiableProperty =
            DependencyProperty.Register("IsTagVisiable", typeof(Visibility), typeof(PlaceholderBox), new PropertyMetadata(Visibility.Collapsed));



        // Using a DependencyProperty as the backing store for Error.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ErrorProperty =
            DependencyProperty.Register("Error", typeof(bool), typeof(PlaceholderBox), new PropertyMetadata(false));

        public Brush PlaceholderBackground
        {
            get { return (Brush)GetValue(PlaceholderBackgroundProperty); }
            set { SetValue(PlaceholderBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlaceholderBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaceholderBackgroundProperty =
            DependencyProperty.Register("PlaceholderBackground", typeof(Brush), typeof(PlaceholderBox), new PropertyMetadata(Brushes.Transparent));



        public Brush PlaceholderForeground
        {
            get { return (Brush)GetValue(PlaceholderForegroundProperty); }
            set { SetValue(PlaceholderForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlaceholderForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaceholderForegroundProperty =
            DependencyProperty.Register("PlaceholderForeground", typeof(Brush), typeof(PlaceholderBox), new PropertyMetadata(Brushes.Transparent));



        public Brush PlaceholderBorderBrush
        {
            get { return (Brush)GetValue(PlaceholderBorderBrushProperty); }
            set { SetValue(PlaceholderBorderBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlaceholderBorderBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaceholderBorderBrushProperty =
            DependencyProperty.Register("PlaceholderBorderBrush", typeof(Brush), typeof(PlaceholderBox), new PropertyMetadata(Brushes.Transparent));



        public FontWeight PlaceholderFontWeight
        {
            get { return (FontWeight)GetValue(PlaceholderFontWeightProperty); }
            set { SetValue(PlaceholderFontWeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlaceholderFontWeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaceholderFontWeightProperty =
            DependencyProperty.Register("PlaceholderFontWeight", typeof(FontWeight), typeof(PlaceholderBox), new PropertyMetadata(FontWeights.Normal));




        public FontStyle PlaceholderFontStyle
        {
            get { return (FontStyle)GetValue(PlaceholderFontStyleProperty); }
            set { SetValue(PlaceholderFontStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlaceholderFontStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaceholderFontStyleProperty =
            DependencyProperty.Register("PlaceholderFontStyle", typeof(FontStyle), typeof(PlaceholderBox), new PropertyMetadata(FontStyles.Italic));







        public bool IsPasswordBox
        {
            get { return (bool)GetValue(IsPasswordBoxProperty); }
            set { SetValue(IsPasswordBoxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPasswordBox.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPasswordBoxProperty =
            DependencyProperty.Register("IsPasswordBox", typeof(bool), typeof(PlaceholderBox), new PropertyMetadata(false));




        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Placeholder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register("Placeholder", typeof(string), typeof(PlaceholderBox), new PropertyMetadata(""));



        public bool IsPlaceholderVisible
        {
            get { return (bool)GetValue(IsPlaceholderVisibleProperty); }
            private set { SetValue(IsPlaceholderVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPlaceholderVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPlaceholderVisibleProperty =
            DependencyProperty.Register("IsPlaceholderVisible", typeof(bool), typeof(PlaceholderBox), new PropertyMetadata(false));




        static PlaceholderBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PlaceholderBox), new FrameworkPropertyMetadata(typeof(PlaceholderBox)));
        }


        public PlaceholderBox()
        {
            TextChanged += (o, e) => IsPlaceholderVisible = string.IsNullOrEmpty(Text);
            Loaded += (o, e) => IsPlaceholderVisible = string.IsNullOrEmpty(Text);
        }
    }
}