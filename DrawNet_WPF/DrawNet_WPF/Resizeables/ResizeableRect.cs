using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace DrawNet_WPF.Resizeables
{
    public class ResizeableRect : Control
    {
        public struct RectInfo
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
        }
        internal static readonly List<ResizeableRect> Selection = new List<ResizeableRect>();
        private bool clicked = false;
        private static Point initialMousePosition;
        private RectInfo initialElementData = new RectInfo() { X = 0, Y = 0, Width = 0, Height = 0 };
        internal ResizeHandle?[,] handles = new ResizeHandle[3, 3];
        private bool modByBound = false;
        private Point prevRelPos = new Point();
        private Point prevBoundSize = new Point();
        private Point prevThisSize = new Point();
        static ResizeableRect()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizeableRect), new FrameworkPropertyMetadata(typeof(ResizeableRect)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            handles[0, 0] = GetTemplateChild("TL") as ResizeHandle;
            handles[0, 1] = GetTemplateChild("TM") as ResizeHandle;
            handles[0, 2] = GetTemplateChild("TR") as ResizeHandle;
            handles[1, 0] = GetTemplateChild("ML") as ResizeHandle;
            handles[1, 1] = null;
            handles[1, 2] = GetTemplateChild("MR") as ResizeHandle;
            handles[2, 0] = GetTemplateChild("BL") as ResizeHandle;
            handles[2, 1] = GetTemplateChild("BM") as ResizeHandle;
            handles[2, 2] = GetTemplateChild("BR") as ResizeHandle;
            this.SizeChanged += ResizeableRect_SizeChanged;
            var window = Window.GetWindow(this);
            if (window != null)
            {
                window.MouseDown += Window_MouseDown;
                window.MouseMove += Window_MouseMove;
                window.MouseUp += Window_MouseUp;
            }
            // Subscribe to BoundTo size and margin change events
            if (BoundTo != null)
            {
                Binding binding = new Binding
                {
                    Source = this.TransformToVisual(BoundTo),
                    Path = new PropertyPath("Transform", new object[] { new Point(0, 0) }),
                    Mode = BindingMode.OneWay
                };
                SetBinding(RelativePositionProperty, binding);
            }
            DependencyPropertyDescriptor.FromProperty(RelativePositionProperty, typeof(ResizeableRect))
                .AddValueChanged(this, RelativePositionChanged);
            BoundTo.SizeChanged += BoundTo_SizeChanged;
            BoundTo.Loaded += BoundTo_Loaded;
        }

        private void ResizeableRect_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!modByBound && BoundTo != null)
            {
                prevRelPos = TransformToVisual(BoundTo).Transform(new Point(0, 0));
                prevBoundSize = new Point(BoundTo.ActualWidth, BoundTo.ActualHeight);
                prevThisSize = new Point(ActualWidth, ActualHeight);
            }
            modByBound = false;
        }
        private bool IsClose(Point point1, Point point2)
        {
            double tolerance = 0.001;
            return Math.Abs(point1.X - point2.X) < tolerance && Math.Abs(point1.Y - point2.Y) < tolerance;
        }

        private void RelativePositionChanged(object sender, EventArgs e)
        {
            if (!modByBound)
            {
                double Check = prevBoundSize.X * prevBoundSize.Y * prevThisSize.X * prevThisSize.Y * BoundTo.ActualHeight * BoundTo.ActualWidth;
                if (Check != 0 && !double.IsNaN(Check))
                {
                    Point vectorDiff = PointExtensions.Multiply(prevRelPos, new Point(ActualWidth / prevThisSize.X, ActualHeight / prevThisSize.Y));
                    Point result = PointExtensions.Subtract(PointExtensions.Add(new Point(Margin.Left, Margin.Top), vectorDiff),
                        TransformToVisual(BoundTo).Transform(new Point(0, 0)));
                    modByBound = true;
                    Margin = new Thickness(result.X, result.Y, 0, 0);
                }
            }
            else
            {
                modByBound = false;
            }
        }

        private void BoundTo_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (BoundTo != null)
            {
                double Check = prevBoundSize.X * prevBoundSize.Y * prevThisSize.X * prevThisSize.Y * BoundTo.ActualHeight * BoundTo.ActualWidth;
                if (Check != 0 && !double.IsNaN(Check))
                {
                    modByBound = true;
                    Width = Width * BoundTo.ActualWidth / prevBoundSize.X;
                    modByBound = true;
                    Height = Height * BoundTo.ActualHeight / prevBoundSize.Y;
                    Point vectorDiff = PointExtensions.Multiply(prevRelPos, new Point(ActualWidth / prevThisSize.X, ActualHeight / prevThisSize.Y));
                    Point result = PointExtensions.Subtract(PointExtensions.Add(new Point(Margin.Left, Margin.Top), vectorDiff),
                        TransformToVisual(BoundTo).Transform(new Point(0, 0)));
                    modByBound = true;
                    Margin = new Thickness(result.X, result.Y, 0, 0);
                }
            }
        }

        private void BoundTo_Loaded(object sender, RoutedEventArgs e)
        {
            prevRelPos = TransformToVisual(BoundTo).Transform(new Point(0, 0));
            prevBoundSize = new Point(BoundTo.ActualWidth, BoundTo.ActualHeight);
            prevThisSize = new Point(ActualWidth, ActualHeight);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            initialElementData.X = Margin.Left;
            initialElementData.Y = Margin.Top;
            if (e.Source == this)
            {
                clicked = true;
                initialMousePosition = e.GetPosition(null);
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (Selection.Contains(this))
                {
                    double deltaX = e.GetPosition(null).X - initialMousePosition.X;
                    double deltaY = e.GetPosition(null).Y - initialMousePosition.Y;
                    this.Margin = new Thickness(initialElementData.X + deltaX, initialElementData.Y + deltaY, 0, 0);
                }
                else if (clicked)
                {
                    DeSelectAll();
                    AddSelect();
                }
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (clicked && e.Source == this)
            {
                if (!Selection.Contains(this))
                {
                    if (Keyboard.IsKeyDown(Key.LeftCtrl))
                        AddSelect();
                    else
                        ChangeSelect();
                }
                else if (Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    DeSelect();
                }
            }
            else if (!(e.Source is ResizeableRect))
            {
                DeSelect();
            }
            clicked = false;
        }


        public static void DeSelectAll()
        {
            foreach (var rect in Selection)
            {
                rect.IsSelected = false;
            }
            Selection.Clear();
        }

        public void AddSelect()
        {
            Selection.Add(this);
            IsSelected = true;
        }

        public void DeSelect()
        {
            Selection.Remove(this);
            IsSelected = false;
        }

        public void ChangeSelect()
        {
            DeSelectAll();
            AddSelect();
        }

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(ResizeableRect), new PropertyMetadata(Brushes.Yellow));

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(ResizeableRect), new PropertyMetadata(Brushes.Red));

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(ResizeableRect), new PropertyMetadata(1.0));

        public static readonly DependencyProperty OpacityProperty =
            DependencyProperty.Register("Opacity", typeof(double), typeof(ResizeableRect), new PropertyMetadata(1.0));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(ResizeableRect), new PropertyMetadata(false));

        public static readonly DependencyProperty BoundToProperty =
            DependencyProperty.Register("BoundTo", typeof(FrameworkElement), typeof(ResizeableRect), new PropertyMetadata(null));

        public static readonly DependencyProperty ConfinedProperty =
            DependencyProperty.Register("Confined", typeof(bool), typeof(ResizeableRect), new PropertyMetadata(false));

        public static readonly DependencyProperty RelativePositionProperty =
            DependencyProperty.Register("RelativePosition", typeof(Point), typeof(ResizeableRect), new PropertyMetadata(default(Point)));

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public double Opacity
        {
            get { return (double)GetValue(OpacityProperty); }
            set { SetValue(OpacityProperty, value); }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set
            {
                if (value)
                {
                    foreach (ResizeHandle? handle in handles)
                    {
                        if (handle != null)
                            handle.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    foreach (ResizeHandle? handle in handles)
                    {
                        if (handle != null)
                            handle.Visibility = Visibility.Hidden;
                    }
                }
                SetValue(IsSelectedProperty, value);
            }
        }
        public FrameworkElement BoundTo
        {
            get { return (FrameworkElement)GetValue(BoundToProperty); }
            set { SetValue(BoundToProperty, value); }
        }

        public bool Confined
        {
            get { return (bool)GetValue(ConfinedProperty); }
            set { SetValue(ConfinedProperty, value); }
        }

        public Point RelativePosition
        {
            get { return (Point)GetValue(RelativePositionProperty); }
            set { SetValue(RelativePositionProperty, value); }
        }
    }
}
