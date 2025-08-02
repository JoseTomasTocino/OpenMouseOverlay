using Gma.System.MouseKeyHook;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation; // <-- Añade esto
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OpenMouseOverlay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IKeyboardMouseEvents _globalMouse;

        public MainWindow()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;
        }

        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch
            {
                // Ignora excepciones si el drag no es posible
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            _globalMouse = Hook.GlobalEvents();          // hook global
            _globalMouse.MouseDownExt += OnMouseDownExt; // botón pulsado
            _globalMouse.MouseUpExt += OnMouseUpExt;   // botón soltado
        }

        private void OnMouseDownExt(object? s, MouseEventExtArgs e)
        {
            e.Handled = false;          // no consumas el evento → sigue hasta la app en primer plano

            Dispatcher.Invoke(() => ShowHighlight(e.Button));
        }

        private void OnMouseUpExt(object? s, MouseEventExtArgs e)
        {
            Dispatcher.Invoke(() => FadeOutHighlight(e.Button));
        }

        private void ShowHighlight(MouseButtons button)
        {
            if (button == MouseButtons.Left)
            {
                LeftButtonHighlight.Visibility = Visibility.Visible;
                LeftButtonTint.Opacity = 1.0;
            }
            else if (button == MouseButtons.Right)
            {
                RightButtonHighlight.Visibility = Visibility.Visible;
                RightButtonTint.Opacity = 1.0;
            }
        }

        private void FadeOutHighlight(MouseButtons button)
        {
            Rectangle? targetTint = null;
            Image? targetHighlight = null;

            if (button == MouseButtons.Left)
            {
                targetTint = LeftButtonTint;
                targetHighlight = LeftButtonHighlight;
            }             
            
            else if (button == MouseButtons.Right)
            {
                targetTint = RightButtonTint;
                targetHighlight = RightButtonHighlight;
            }

            else
            {
                return;
            }

            var fade = new DoubleAnimation(1.0, 0.0, new Duration(TimeSpan.FromMilliseconds(300)));
            fade.Completed += (s, e) =>
            {
                targetHighlight.Visibility = Visibility.Collapsed;
            };
            targetTint.BeginAnimation(UIElement.OpacityProperty, fade);
        }

        protected override void OnClosed(EventArgs e)
        {
            _globalMouse?.Dispose();    // libera el hook
            base.OnClosed(e);
        }
    }
}