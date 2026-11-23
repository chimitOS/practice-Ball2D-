using Ball2D;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace billiards_wpf_
{
    public partial class MainWindow : Window
    {
        private readonly Simulation _simulation = new Simulation();
        private readonly Dictionary<Ball, Ellipse> _ballVisuals =
                        new Dictionary<Ball, Ellipse>();
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private System.Windows.Shapes.Line _aimLine;
        private double _previousFrameTime;

        private bool _isCharging = false; //показывает, удерживает ли игрок кнопку для накопления силы удара
        private double _chargeStartTime; //момент, когда игрок начал удерживать кнопку
        private float _shotPower; //текущая сила удара
        private const float MaxShotPower = 1000f;
        private const float ChargeDuration = 2f;
        private CustomVector2 _shotDirection;

        private void FrictionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _simulation.Settings.Friction = (float)e.NewValue;
        }

        private void RestitutionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _simulation.Settings.Restitution = (float)e.NewValue;
        }
        public MainWindow()
        {
            InitializeComponent();
            CreateVisuals();

            FrictionSlider.Value = _simulation.Settings.Friction;
            RestitutionSlider.Value = _simulation.Settings.Restitution;

            FrictionSlider.ValueChanged += FrictionSlider_ValueChanged;
            RestitutionSlider.ValueChanged += RestitutionSlider_ValueChanged;

            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
        }

        private void CreateVisuals()
        {
            var tableVisual = new Rectangle
            {
                Width = _simulation.billiardTable.Width,
                Height = _simulation.billiardTable.Height,
                Fill = Brushes.DarkGreen
            };

            Canvas.SetLeft(tableVisual, _simulation.billiardTable.Left);
            Canvas.SetTop(tableVisual, _simulation.billiardTable.Top);

            WorldCanvas.Children.Add(tableVisual);

            foreach (var pocketsPos in _simulation.billiardTable.Pockets)
            {
                var pockets = new Ellipse
                {
                    Width = _simulation.billiardTable.PocketRadius * 2,
                    Height = _simulation.billiardTable.PocketRadius * 2,
                    Stroke = Brushes.Black,
                    StrokeThickness = 3,
                    Fill = Brushes.Black
                };

                Canvas.SetLeft(pockets, pocketsPos.X - _simulation.billiardTable.PocketRadius);
                Canvas.SetTop(pockets, pocketsPos.Y - _simulation.billiardTable.PocketRadius);

                WorldCanvas.Children.Add(pockets);
            }

            foreach (var line in _simulation.Lines)
            {
                var visual = new System.Windows.Shapes.Line
                {
                    X1 = line.Start.X,
                    Y1 = line.Start.Y,
                    X2 = line.End.X,
                    Y2 = line.End.Y,
                    Stroke = Brushes.Black,
                    StrokeThickness = 4
                };
                WorldCanvas.Children.Add(visual);
            }

            foreach (var ball in _simulation.Balls)
            {
                var visual = new Ellipse
                {
                    Width = ball.Radius * 2,
                    Height = ball.Radius * 2,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2,
                    Fill = ball == _simulation.CueBall
                        ? Brushes.White
                        : Brushes.Red
                };

                _ballVisuals.Add(ball, visual);
                WorldCanvas.Children.Add(visual);
            }

            _aimLine = new System.Windows.Shapes.Line
            {
                Stroke = Brushes.White,
                StrokeThickness = 2,
                StrokeDashArray = new DoubleCollection { 5, 5 },
                Visibility = Visibility.Collapsed,
                IsHitTestVisible = false
            };

            WorldCanvas.Children.Add(_aimLine);

            UpdateBallVisuals();
        }

        private void WorldCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isCharging)
            {
                return;
            }

            if (!_simulation.AreAllBallsStopped())
            {
                return;
            }

            Point mousePosition = e.GetPosition(WorldCanvas);

            CustomVector2 mouseVector = new CustomVector2(
                (float)mousePosition.X,
                (float)mousePosition.Y
            );

            CustomVector2 direction = (mouseVector - _simulation.CueBall.Position).Normalized();

            _aimLine.X1 = _simulation.CueBall.Position.X;
            _aimLine.Y1 = _simulation.CueBall.Position.Y;

            CustomVector2 endPoint = _simulation.CueBall.Position + direction * 150f;

            _aimLine.X2 = endPoint.X;
            _aimLine.Y2 = endPoint.Y;

            _aimLine.Visibility = Visibility.Visible;
        }

        private void WorldCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_isCharging)
            {
                return;
            }

            if (!_simulation.AreAllBallsStopped())
            {
                return;
            }

            if (!_ballVisuals.ContainsKey(_simulation.CueBall))
            {
                return;
            }

            Point clickPosition = e.GetPosition(WorldCanvas);

            CustomVector2 clickVector = new CustomVector2(
                (float)clickPosition.X,
                (float)clickPosition.Y
            );

            CustomVector2 direction = (clickVector - _simulation.CueBall.Position).Normalized();

            if (direction.LengthSquared() < 0.00000001f)
            {
                return;
            }

            _shotDirection = direction;

            _isCharging = true;

            _chargeStartTime = _stopwatch.Elapsed.TotalSeconds;
            _shotPower = 0;
            WorldCanvas.CaptureMouse();

            _aimLine.Visibility = Visibility.Collapsed;
        }

        private void WorldCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isCharging)
            {
                return;
            }

            _isCharging = false;

            _simulation.CueBall.Velocity = _shotDirection * _shotPower;

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _stopwatch.Start();
            _previousFrameTime = _stopwatch.Elapsed.TotalSeconds;
            CompositionTarget.Rendering += OnRendering;
        }

        private void OnRendering(object sender, EventArgs e)
        {
            double now = _stopwatch.Elapsed.TotalSeconds;

            if (_isCharging)
            {
                double elapsed = now - _chargeStartTime;
                _shotPower = (float)Math.Min(MaxShotPower, elapsed / ChargeDuration * MaxShotPower);
            }

            float deltaTime = (float)Math.Min(
                Math.Max(now - _previousFrameTime, 0.0),
                1.0 / 30.0
            );
            _previousFrameTime = now;

            _simulation.Update(deltaTime);

            if (!_simulation.AreAllBallsStopped())
            {
                _aimLine.Visibility = Visibility.Collapsed;
            }

            UpdateBallVisuals();
        }

        private void UpdateBallVisuals()
        {
            var activeBalls = new HashSet<Ball>(_simulation.Balls);

            foreach (var ball in _simulation.Balls)
            { 
                var visual = _ballVisuals[ball];

                Canvas.SetLeft(visual, ball.Position.X - ball.Radius);
                Canvas.SetTop(visual, ball.Position.Y - ball.Radius);
            }

            var ballsToRemove = new List<Ball>();

            foreach (var ball in _ballVisuals.Keys)
            {
                if (!activeBalls.Contains(ball))
                {
                    ballsToRemove.Add(ball);
                }
            }

            foreach (var ball in ballsToRemove)
            {
                var visual = _ballVisuals[ball];

                WorldCanvas.Children.Remove(visual);

                _ballVisuals.Remove(ball);
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            CompositionTarget.Rendering -= OnRendering;
            _stopwatch.Stop();
        }
    }
}
