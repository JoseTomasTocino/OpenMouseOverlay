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

            Dispatcher.Invoke(() => ToggleHighlight(e.Button, Visibility.Visible));            
        }

        private void OnMouseUpExt(object? s, MouseEventExtArgs e)
        {
            Dispatcher.Invoke(() => ToggleHighlight(e.Button, Visibility.Collapsed));
        }

        private void ToggleHighlight(MouseButtons button, Visibility visible)
        {
            if (button == MouseButtons.Left)
            {
                LeftButtonHighlight.Visibility = visible; // oculta el botón izquierdo
            }
            else if (button == MouseButtons.Right)
            {
                RightButtonHighlight.Visibility = visible; // oculta el botón derecho
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _globalMouse?.Dispose();    // libera el hook
            base.OnClosed(e);
        }
    }
}