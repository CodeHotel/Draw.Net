using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DrawNet_WPF.Handles;
using Vector = DrawNet_WPF.Converters.Vector;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace DrawNet_WPF.Resizeables
{
    public class ResizeWrapper : Canvas
    {
        static ResizeWrapper()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizeWrapper), new FrameworkPropertyMetadata(typeof(ResizeWrapper)));
        }

        public ResizeWrapper()
        {
            #region Throws Exception if Margin is not 0

            var dpd = DependencyPropertyDescriptor.FromProperty(ResizeWrapper.MarginProperty, typeof(ResizeWrapper));
            if (dpd != null)
            {
                dpd.AddValueChanged(this, (s, e) =>
                {
                    if (Margin != new Thickness(0))
                        throw new InvalidOperationException("Margin must be zero!");
                });
            }
            #endregion

            ClipToBounds = false;
            Loaded += ResizeWrapper_Loaded;
        }

        private void ResizeWrapper_Loaded(object sender, RoutedEventArgs e)
        {
            #region Gets closest ancestor that is Panel. Throws exception if not found
            var parent = VisualTreeHelper.GetParent(this);

            Canvas? foundPanel = null;

            while (parent != null)
            {
                if (parent is Canvas panel)
                {
                    foundPanel = panel;
                    break;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }
            if (foundPanel == null) throw new Exception("ResizeWrapper must be inside a Canvas(ResizeWrapper, ResizeCanvas) to function correctly");
            else outerPanel = foundPanel;
            #endregion

            InitializeOuterBorder();

            outerPanel.Children.Remove(this);
            outerPanel.Children.Add(_outerBorder);
            _outerBorder.Child = this;
            _outerBorder.MouseDown += BorderMouseDownHandler;
            InitializeHandles(HandleShape);
            Window? window = Converters.WindowOperations.FindParentWindow(this);
            if (window != null)
            {
                window.MouseMove += WindowsMouseMove;
                window.MouseUp += WindowsMouseUp;
            }

            this.Loaded -= ResizeWrapper_Loaded;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        private void InitializeHandles(ShapeType HandleShape)
        {
            Handles = new ResizeHandle[3, 3];
            Handles[0, 0] = new ResizeHandle { ParentControl = this, outerPanel = outerPanel, MovType = new Vector(-1, -1) };
            Handles[1, 0] = new ResizeHandle { ParentControl = this, outerPanel = outerPanel, MovType = new Vector(0, -1) };
            Handles[2, 0] = new ResizeHandle { ParentControl = this, outerPanel = outerPanel, MovType = new Vector(1, -1) };
            Handles[0, 1] = new ResizeHandle { ParentControl = this, outerPanel = outerPanel, MovType = new Vector(-1, 0) };
            Handles[1, 1] = null;
            Handles[2, 1] = new ResizeHandle { ParentControl = this, outerPanel = outerPanel, MovType = new Vector(1, 0) };
            Handles[0, 2] = new ResizeHandle { ParentControl = this, outerPanel = outerPanel, MovType = new Vector(-1, 1) };
            Handles[1, 2] = new ResizeHandle { ParentControl = this, outerPanel = outerPanel, MovType = new Vector(0, 1) };
            Handles[2, 2] = new ResizeHandle { ParentControl = this, outerPanel = outerPanel, MovType = new Vector(1, 1) };
            RotationHandle = new RotationHandle { ParentControl = this, outerPanel = outerPanel, Shape = HandleShape };

            outerPanel.Children.Add(Handles[0, 0]);
            outerPanel.Children.Add(Handles[1, 0]);
            outerPanel.Children.Add(Handles[2, 0]);
            outerPanel.Children.Add(Handles[0, 1]);
            outerPanel.Children.Add(Handles[2, 1]);
            outerPanel.Children.Add(Handles[0, 2]);
            outerPanel.Children.Add(Handles[1, 2]);
            outerPanel.Children.Add(Handles[2, 2]);
            outerPanel.Children.Add(RotationHandle);

            #pragma warning disable CS8602 // Dereference of a possibly null reference.
            Handles[0, 0].HandleMouseDown += DiagonalMouseDownHandler;
            Handles[1, 0].HandleMouseDown += LinearMouseDownHandler;
            Handles[2, 0].HandleMouseDown += DiagonalMouseDownHandler;
            Handles[0, 1].HandleMouseDown += LinearMouseDownHandler;
            Handles[2, 1].HandleMouseDown += LinearMouseDownHandler;
            Handles[0, 2].HandleMouseDown += DiagonalMouseDownHandler;
            Handles[1, 2].HandleMouseDown += LinearMouseDownHandler;
            Handles[2, 2].HandleMouseDown += DiagonalMouseDownHandler;
            #pragma warning restore CS8602 // Dereference of a possibly null reference.

            InitHandles();
        }

        private void InitHandles()
        {
            #pragma warning disable CS8602 // Dereference of a possibly null reference.
            Handles[0, 0].InitHandle(outerPanel, this, HandleShape, HandleSize, HandleStroke, HandleStrokeThickness, HandleFill);
            Handles[1, 0].InitHandle(outerPanel, this, HandleShape, HandleSize, HandleStroke, HandleStrokeThickness, HandleFill);
            Handles[2, 0].InitHandle(outerPanel, this, HandleShape, HandleSize, HandleStroke, HandleStrokeThickness, HandleFill);
            Handles[0, 1].InitHandle(outerPanel, this, HandleShape, HandleSize, HandleStroke, HandleStrokeThickness, HandleFill);
            Handles[2, 1].InitHandle(outerPanel, this, HandleShape, HandleSize, HandleStroke, HandleStrokeThickness, HandleFill);
            Handles[0, 2].InitHandle(outerPanel, this, HandleShape, HandleSize, HandleStroke, HandleStrokeThickness, HandleFill);
            Handles[1, 2].InitHandle(outerPanel, this, HandleShape, HandleSize, HandleStroke, HandleStrokeThickness, HandleFill);
            Handles[2, 2].InitHandle(outerPanel, this, HandleShape, HandleSize, HandleStroke, HandleStrokeThickness, HandleFill);
            #pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        private void UpdateHandles()
        {
            #pragma warning disable CS8602 // Dereference of a possibly null reference.
            Handles[0, 0].setHandleProperty(this, HandleShape, HandleSize, HandleStroke, HandleStrokeThickness, HandleFill);
            Handles[1, 0].setHandleProperty(this, HandleShape, HandleSize, HandleStroke, HandleStrokeThickness, HandleFill);
            Handles[2, 0].setHandleProperty(this, HandleShape, HandleSize, HandleStroke, HandleStrokeThickness, HandleFill);
            Handles[0, 1].setHandleProperty(this, HandleShape, HandleSize, HandleStroke, HandleStrokeThickness, HandleFill);
            Handles[2, 1].setHandleProperty(this, HandleShape, HandleSize, HandleStroke, HandleStrokeThickness, HandleFill);
            Handles[0, 2].setHandleProperty(this, HandleShape, HandleSize, HandleStroke, HandleStrokeThickness, HandleFill);
            Handles[1, 2].setHandleProperty(this, HandleShape, HandleSize, HandleStroke, HandleStrokeThickness, HandleFill);
            Handles[2, 2].setHandleProperty(this, HandleShape, HandleSize, HandleStroke, HandleStrokeThickness, HandleFill);
            #pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        private void setRotationProperty(bool RotationEnabled, double RotationOffset)
        {
            if (RotationEnabled) RotationHandle.Visibility = Visibility.Visible;
            else RotationHandle.Visibility = Visibility.Collapsed;
            //TODO: declare the line that connects rotational handle and top middle handle at the bottom of class. add logic to manipulate it.
        }

        private void InitializeOuterBorder()
        {
            if (_outerBorder == null) _outerBorder = new Border();

            // Set the border to surround the control
            _outerBorder.BorderBrush = Stroke;
            _outerBorder.BorderThickness = new Thickness(StrokeThickness);
            _outerBorder.Width = Width + 2 * StrokeThickness;
            _outerBorder.Height = Height + 2 * StrokeThickness;
            Position = Position;
        }

        #region Properties
        //The position of the Wrapper, relative to closest parent that is a Panel
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register(nameof(Position), typeof(Vector), typeof(ResizeWrapper), new PropertyMetadata(new Vector(50, 50)));
        /// <summary>
        /// The position("x,y") of the Wrapper, relative to closest parent that is a Panel
        /// </summary>
        public Vector Position
        {
            get => (Vector)GetValue(PositionProperty);
            set
            {
                SetValue(PositionProperty, value);
                if (_outerBorder != null) SetPos(new Vector(value.X, value.Y));
                if (Handles != null) UpdateHandles();
            }
        }
        private void SetPos(Vector v)
        {
            Canvas.SetLeft(_outerBorder, v.X - StrokeThickness);
            Canvas.SetTop(_outerBorder, v.Y - StrokeThickness);
            Canvas.SetLeft(this, v.X);
            Canvas.SetTop(this, v.Y);
            
        }

        // The minimum size to which the wrapper can be shrunk via dragging the handles
        public static readonly DependencyProperty ShrinkLimitProperty =
            DependencyProperty.Register(nameof(ShrinkLimit), typeof(double), typeof(ResizeWrapper), new PropertyMetadata(13.0));
        /// <summary>
        /// The minimum size in which the wrapper can be shrunk via dragging the handles.
        /// </summary>
        public double ShrinkLimit
        {
            get => (double)GetValue(ShrinkLimitProperty);
            set => SetValue(ShrinkLimitProperty, value);
        }


        //The color of the surrounding Border
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register(nameof(Stroke), typeof(Brush), typeof(ResizeWrapper), new PropertyMetadata(Brushes.Black));
        /// <summary>
        /// The color of the surrounding borderline
        /// </summary>
        public Brush Stroke
        {
            get => (Brush)GetValue(StrokeProperty);
            set => SetValue(StrokeProperty, value);
        }

        //The stroke thickness of surrounding Border
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register(nameof(StrokeThickness), typeof(double), typeof(ResizeWrapper), new PropertyMetadata(4.0));
        /// <summary>
        /// The Thickness value of the surrounding borderline
        /// </summary>
        public double StrokeThickness
        {
            get => (double)GetValue(StrokeThicknessProperty);
            set 
            {
                SetValue(StrokeThicknessProperty, value);
                InitializeOuterBorder();
            }
        }

        //Whether to enable rotational handles and rotation
        public static readonly DependencyProperty RotationEnabledProperty =
            DependencyProperty.Register(nameof(RotationEnabled), typeof(bool), typeof(ResizeWrapper), new PropertyMetadata(false));
        /// <summary>
        /// Set true to enable rotational handles and rotation, set false to disable(false by default)
        /// </summary>
        public bool RotationEnabled
        {
            get => (bool)GetValue(RotationEnabledProperty);
            set
            {
                if (value) throw new NotImplementedException("Rotations are not implemented yet!");
                SetValue(RotationEnabledProperty, value);
                setRotationProperty(value, RotationOffset);
            }
        }


        //The shape of the resize/rotational handles(Circle/Square)
        public static readonly DependencyProperty ShapeProperty =
            DependencyProperty.Register(nameof(HandleShape), typeof(ShapeType), typeof(ResizeWrapper), new PropertyMetadata(ShapeType.Circle));
        /// <summary>
        /// Property that describes the shape of the drag-to-resize/rotate handles (Circle / Square)
        /// </summary>
        public ShapeType HandleShape
        {
            get => (ShapeType)GetValue(ShapeProperty);
            set
            {
                SetValue(ShapeProperty, value);
                UpdateHandles();
            }
        }

        //Size of the handle
        public static readonly DependencyProperty HandleSizeProperty =
            DependencyProperty.Register(nameof(HandleSize), typeof(double), typeof(ResizeWrapper), new PropertyMetadata(10.0));
        /// <summary>
        /// The diameter/length/width of the handle, depending on the shape
        /// </summary>
        public double HandleSize
        {
            get => (double)GetValue(HandleSizeProperty);
            set
            {
                SetValue(HandleSizeProperty, value);
                UpdateHandles();
            }
        }

        //The color of the surrounding Border of the Handles
        public static readonly DependencyProperty HandleStrokeProperty =
            DependencyProperty.Register(nameof(HandleStroke), typeof(Brush), typeof(ResizeWrapper), new PropertyMetadata(Brushes.DarkGray));
        /// <summary>
        /// The color of the surrounding borderline of the drag-to-resize/rotate handles
        /// </summary>
        public Brush HandleStroke
        {
            get => (Brush)GetValue(HandleStrokeProperty);
            set
            {
                SetValue(HandleStrokeProperty, value);
                UpdateHandles();
            }
        }

        //The fill color of the Handles
        public static readonly DependencyProperty HandleFillProperty =
            DependencyProperty.Register(nameof(HandleFill), typeof(Brush), typeof(ResizeWrapper), new PropertyMetadata(Brushes.White));
        /// <summary>
        /// The fill color of the drag-to-resize/rotate handles
        /// </summary>
        public Brush HandleFill
        {
            get => (Brush)GetValue(HandleFillProperty);
            set
            {
                SetValue(HandleFillProperty, value);
                UpdateHandles(); // If you need to update handles when Fill is changed
            }
        }

        //The stroke thickness of handles
        public static readonly DependencyProperty HandleStrokeThicknessProperty =
            DependencyProperty.Register(nameof(HandleStrokeThickness), typeof(double), typeof(ResizeWrapper), new PropertyMetadata(1.5));
        /// <summary>
        /// The Thickness value of the surrounding borderline of the drag-to-resize/rotate handles
        /// </summary>
        public double HandleStrokeThickness
        {
            get => (double)GetValue(HandleStrokeThicknessProperty);
            set
            {
                SetValue(HandleStrokeThicknessProperty, value);
                UpdateHandles();
            }
        }

        //Vertical offset of rotational handle
        public static readonly DependencyProperty RotationOffsetProperty =
            DependencyProperty.Register(nameof(RotationOffset), typeof(double), typeof(ResizeWrapper), new PropertyMetadata(30.0));
        /// <summary>
        /// The vertical offset of the rotational handle
        /// </summary>
        public double RotationOffset
        {
            get => (double)GetValue(RotationOffsetProperty);
            set
            {
                SetValue(RotationOffsetProperty, value);
                setRotationProperty(RotationEnabled, value);
            }
        }
        #endregion

        #region Internal Controls
        internal ResizeHandle?[,] Handles { get; private set; }
        internal RotationHandle RotationHandle { get; private set; }
        private Border _outerBorder;
        internal Canvas outerPanel { get; private set; }
        #endregion

        #region DragHandle Control Variables
        private Vector? initialMousePos = null;
        private Vector? FixedPoint = null;
        private Vector? MovePoint = null;
        private Vector? vectorFactor = null;
        private int FixedPointChange = 0;
        #endregion

        private void BorderMouseDownHandler(object sender, MouseEventArgs e)
        {
            vectorFactor = new Vector(1, 1);
            FixedPointChange = 1;
            initialMousePos = new Vector(e.GetPosition(null).X, e.GetPosition(null).Y);
            FixedPoint = Position;
            MovePoint = Position + new Vector(Width, Height);
            Debug.WriteLine($"{Name}: BorderDown");
            e.Handled = true;
        }
        private void DiagonalMouseDownHandler(object sender, MouseEventArgs e)
        {
            ResizeHandle? handle = sender as ResizeHandle;
            if (handle == null) throw new UnauthorizedAccessException("Diagonal Mouse Handler can only be triggered for diagonal mouse clicks!");
            vectorFactor = new Vector(1, 1);
            FixedPointChange = 0;
            initialMousePos = new Vector(e.GetPosition(null).X, e.GetPosition(null).Y);
            Vector MidPoint = Position + new Vector(Width / 2, Height / 2);
            MovePoint = MidPoint + (handle.MovType * new Vector(Width / 2, Height / 2));
            FixedPoint = MidPoint + (handle.MovType * new Vector(Width / 2, Height / 2)) * (-1);
            Debug.WriteLine($"{Name}: DiagonalDown");
            e.Handled = true;
        }
        private void LinearMouseDownHandler(object sender, MouseEventArgs e)
        {
            ResizeHandle? handle = sender as ResizeHandle;
            if (handle == null) throw new UnauthorizedAccessException("Linear Mouse Handler can only be triggered for linear mouse clicks!");
            vectorFactor = new Vector(handle.MovType.X == 0 ? 0 : 1, handle.MovType.Y == 0 ? 0 : 1);
            FixedPointChange = 0;
            initialMousePos = new Vector(e.GetPosition(null).X, e.GetPosition(null).Y);
            Vector MidPoint = Position + new Vector(Width / 2, Height / 2);
            Vector RotateVector = new Vector(handle.MovType.X - handle.MovType.Y, handle.MovType.X + handle.MovType.Y); //rotates as if clockwise+1 rel to original handle
            MovePoint = MidPoint + (RotateVector * new Vector(Width / 2, Height / 2));
            FixedPoint = MidPoint + (RotateVector * new Vector(Width / 2, Height / 2)) * (-1);
            Debug.WriteLine($"{Name}: LinearDown");
            e.Handled = true;
        }

        private void WindowsMouseMove(object sender, MouseEventArgs e)
        {
            if (FixedPoint != null && MovePoint != null && initialMousePos != null && vectorFactor != null)
            {
                Debug.WriteLine($"{Name}: Mov");
                Vector Movement = new Vector(e.GetPosition(null).X, e.GetPosition(null).Y) - initialMousePos;
                Debug.WriteLine($"Movement: {Movement.X},{Movement.Y}");
                Vector a = Movement * vectorFactor + MovePoint;
                Vector b = Movement * FixedPointChange + FixedPoint;
                Debug.WriteLine($"MP: {a.X},{a.Y}");
                Debug.WriteLine($"FP: {b.X},{b.Y}");
                ResizeFromVectors(Movement * vectorFactor + MovePoint, Movement * FixedPointChange + FixedPoint);
                e.Handled = true;
            }
        }
        
        private void WindowsMouseUp(object sender, MouseEventArgs e)
        {
            initialMousePos = null;
            FixedPoint = null;
            MovePoint = null;
            vectorFactor = null;
            FixedPointChange = 0;
            Debug.WriteLine($"{Name}: Up");
        }

        private void ResizeFromVectors(Vector MovePoint, Vector FixPoint)
        {
            double Min = ShrinkLimit;
            double baseX = Math.Min(MovePoint.X, FixPoint.X);
            double baseY = Math.Min(MovePoint.Y, FixPoint.Y);
            double topX = Math.Max(MovePoint.X, FixPoint.X);
            double topY = Math.Max(MovePoint.Y, FixPoint.Y);
            double pWidth = topX - baseX;
            double pHeight = topY - baseY;

            if (pWidth < Min || pHeight < Min)
            {
                Vector newMovePoint = new Vector(0, 0);
                if (pWidth < Min) newMovePoint.X = Math.Sign(MovePoint.X - FixPoint.X) * Min + FixPoint.X;
                else newMovePoint.X = MovePoint.X;
                if (pHeight < Min) newMovePoint.Y = Math.Sign(MovePoint.Y - FixPoint.Y) * Min + FixPoint.Y;
                else newMovePoint.Y = MovePoint.Y;
                Debug.WriteLine($"FIX: MP({newMovePoint.X},{newMovePoint.Y}), FP({FixPoint.X},{FixPoint.Y}");
                baseX = Math.Min(newMovePoint.X, FixPoint.X);
                baseY = Math.Min(newMovePoint.Y, FixPoint.Y);
                topX = Math.Max(newMovePoint.X, FixPoint.X);
                topY = Math.Max(newMovePoint.Y, FixPoint.Y);
                pWidth = topX - baseX;
                pHeight = topY - baseY;
            }
            Position = new Vector(baseX, baseY);
            Width = pWidth;
            Height = pHeight;
            InitializeOuterBorder();
            UpdateHandles();
        }

        internal void ResizeClick()
        {
            // Implement your resize logic here.
        }

        internal void Rotate()
        {
            // Implement your rotate logic here.
        }

    }
}
