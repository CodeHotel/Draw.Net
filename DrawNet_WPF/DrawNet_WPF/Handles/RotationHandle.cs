﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using DrawNet_WPF.Resizeables;
using Vector = DrawNet_WPF.Converters.Vector;
namespace DrawNet_WPF.Handles
{
    
    public class RotationHandle : Control
    {
        private Rectangle _rectangle;
        private Ellipse _ellipse;

        public RotationHandle()
        {
            _rectangle = new Rectangle();
            _ellipse = new Ellipse();
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateShape();
        }

        private void UpdateShape()
        {
            if (Shape == ShapeType.Square)
            {
                _ellipse.Visibility = Visibility.Collapsed;
                _rectangle.Visibility = Visibility.Visible;
            }
            else
            {
                _rectangle.Visibility = Visibility.Collapsed;
                _ellipse.Visibility = Visibility.Visible;
            }
        }

        // Trigger UpdateShape when the DependencyProperty changes.
        private static void OnShapeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RotationHandle handle)
            {
                handle.UpdateShape();
            }
        }

        static RotationHandle()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RotationHandle), new FrameworkPropertyMetadata(typeof(RotationHandle)));
        }
        public void SetPos(Vector v)
        {
            Canvas.SetLeft(this, v.X);
            Canvas.SetTop(this, v.Y);
        }

        internal ResizeWrapper ParentControl { get; set; }
        internal Panel outerPanel { get; set; }


        public static readonly DependencyProperty ShapeProperty =
            DependencyProperty.Register(nameof(Shape), typeof(ShapeType), typeof(RotationHandle), new PropertyMetadata(ShapeType.Square));

        public ShapeType Shape
        {
            get => (ShapeType)GetValue(ShapeProperty);
            set => SetValue(ShapeProperty, value);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            ParentControl?.Rotate();
        }

    }

}