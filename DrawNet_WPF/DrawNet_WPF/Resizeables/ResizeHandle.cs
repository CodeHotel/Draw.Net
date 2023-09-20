using DrawNet_WPF.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using DrawNet_WPF.Converters;

namespace DrawNet_WPF.Resizeables
{
    public class ResizeHandle : Control
    {
        public struct RectInfo
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
        }
        Random random = new Random();

        private static List<ResizeHandle> DraggingHandles = new List<ResizeHandle>();
        private static Point initialMousePosition;
        private static Tuple<int, int> DragType;
        private ResizeableRect? dragElement;
        private RectInfo initialElementData = new RectInfo() { X = 0, Y = 0, Width = 0, Height = 0 };
        Rectangle? HandleRect;

        static ResizeHandle()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizeHandle), new FrameworkPropertyMetadata(typeof(ResizeHandle)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            HandleRect = GetTemplateChild("HandleRect") as Rectangle;
            var window = Window.GetWindow(this);
            if (window != null)
            {
                this.MouseDown += this.Handle_MouseDown;
                window.MouseUp += this.Window_MouseUp;
            }
        }

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(ResizeHandle), new PropertyMetadata(Brushes.White));

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(ResizeHandle), new PropertyMetadata(Brushes.Black));

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(ResizeHandle), new PropertyMetadata(1.0));

        public static readonly DependencyProperty HandleSizeProperty =
            DependencyProperty.Register("HandleSize", typeof(double), typeof(ResizeHandle), new PropertyMetadata(8.0));

        public static readonly DependencyProperty HandleTypeProperty =
            DependencyProperty.Register("HandleType", typeof(Tuple<int, int>), typeof(ResizeHandle), new PropertyMetadata(null, HandleTypePropertyChanged));

        public static readonly DependencyProperty VisibilityProperty =
            DependencyProperty.Register("Visibility", typeof(System.Windows.Visibility), typeof(ResizeHandle), new PropertyMetadata(System.Windows.Visibility.Hidden, OnVisibilityChanged));

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

        public double HandleSize
        {
            get { return (double)GetValue(HandleSizeProperty); }
            set { SetValue(HandleSizeProperty, value); }
        }

        [TypeConverter(typeof(TupleTypeConverter))] // Apply the custom TypeConverter
        public Tuple<int, int> HandleType
        {
            get { return (Tuple<int, int>)GetValue(HandleTypeProperty); }
            set { SetValue(HandleTypeProperty, value); }
        }
        public System.Windows.Visibility Visibility
        {
            get { return (System.Windows.Visibility)GetValue(VisibilityProperty); }
            set { HandleRect.Visibility = value; SetValue(VisibilityProperty, value); }
        }

        private static void HandleTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var handleType = (Tuple<int, int>)e.NewValue;

            var allowedValues = new HashSet<int> { -1, 0, 1 };

            if (!allowedValues.Contains(handleType.Item1) || !allowedValues.Contains(handleType.Item2))
            {
                throw new ArgumentException("HandleType values must be either -1, 0, or 1.");
            }

            if (handleType.Item1 == 0 && handleType.Item2 == 0)
            {
                throw new ArgumentException("HandleType values cannot be 0,0.");
            }
        }

        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var handle = (ResizeHandle)d;

            if ((System.Windows.Visibility)e.NewValue == System.Windows.Visibility.Visible)
            {
                SubscribeToEvents(handle);
            }
            else
            {
                UnsubscribeFromEvents(handle);
            }
        }

        private static void SubscribeToEvents(ResizeHandle handle)
        {
            var window = Window.GetWindow(handle);
            if (window != null)
            {
                window.MouseMove += handle.Window_MouseMove;
            }
        }

        private static void UnsubscribeFromEvents(ResizeHandle handle)
        {
            var window = Window.GetWindow(handle);
            if (window != null)
            {
                window.MouseMove -= handle.Window_MouseMove;
            }
        }

        private void Handle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source == this)
            {
                DraggingHandles.Add(this);
                initialMousePosition = e.GetPosition(null);
                DragType = this.HandleType;
                dragElement = FindParentElement<ResizeableRect>(this);
                if (dragElement != null)
                {
                    initialElementData = new RectInfo
                    {
                        X = dragElement.Margin.Left,
                        Y = dragElement.Margin.Top,
                        Width = dragElement.Width,
                        Height = dragElement.Height
                    };
                }
                foreach (var rect in ResizeableRect.Selection)
                {
                    ResizeHandle? isotope = rect.handles[HandleType.Item1 + 1, HandleType.Item2 + 1];
                    if (isotope == null) continue;
                    DraggingHandles.Add(isotope);
                    isotope.dragElement = FindParentElement<ResizeableRect>(isotope);
                    if (isotope.dragElement == null) continue;
                    isotope.initialElementData = new RectInfo
                    {
                        X = isotope.dragElement.Margin.Left,
                        Y = isotope.dragElement.Margin.Top,
                        Width = isotope.dragElement.Width,
                        Height = isotope.dragElement.Height
                    };
                }
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (DraggingHandles.Contains(this))
            {
                Point currentPoint = e.GetPosition(null);
                Point vectorDiff = new Point(currentPoint.X - initialMousePosition.X, currentPoint.Y - initialMousePosition.Y);
                Thickness newPos = new Thickness(
                    initialElementData.X + ((DragType.Item1 == -1) ? 1 : 0) * vectorDiff.X,
                    initialElementData.Y + ((DragType.Item2 == -1) ? 1 : 0) * vectorDiff.Y, 0, 0);
                ResizeableRect? resizableRect = dragElement;
                if (resizableRect != null)
                {
                    // Update the text from another class
                    /*Application.Current.Dispatcher.Invoke(() =>
                    {
                        MainWindow.MyTextBlock.Text = $"VectorDiff: {vectorDiff.X}, {vectorDiff.Y}\n" +
                                                      $"newPos: {newPos.Left}, {newPos.Right}\n"+
                                                      $"newSize: {initialElementData.Width + DragType.Item1 * vectorDiff.X}, {initialElementData.Height + DragType.Item2 * vectorDiff.Y}\n" +
                                                      $"initPos: {initialElementData.X},{initialElementData.Y}\n" +
                                                      $"initSize: {initialElementData.Width}, {initialElementData.Height}\n" +
                                                      $"Updating: {random.Next()}";

                    });*/

                    resizableRect.Margin = newPos;
                    resizableRect.Width = initialElementData.Width + DragType.Item1 * vectorDiff.X;
                    resizableRect.Height = initialElementData.Height + DragType.Item2 * vectorDiff.Y;
                }
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DraggingHandles.Clear();
            DragType = null;
            dragElement = null;
            initialMousePosition = new Point();
            initialElementData = new RectInfo();
        }

        private static T? FindParentElement<T>(DependencyObject child) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(child);
            while (parent != null && !(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as T;
        }
    }
}
