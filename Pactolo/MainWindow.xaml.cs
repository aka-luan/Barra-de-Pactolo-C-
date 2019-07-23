using System;
using System.ComponentModel;
using System.Windows;
using WpfAppBar;
using EventHook;
using Quellatalo.Nin.TheHands;
using System.Windows.Interop;

namespace Pactolo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly EventHookFactory eventHookFactory = new EventHookFactory();
        private readonly MouseWatcher mouseWatcher;
        public bool dblClick = false;
        public bool mwAtivado, noClick = true;
        public MouseHandler mouse = new MouseHandler();
        private const int WM_LBUTTONDOWN = 0x0201;

        public MainWindow()
        {
            InitializeComponent();
            System.Windows.Application.Current.Exit += OnApplicationExit;

            mouseWatcher = eventHookFactory.GetMouseWatcher();
            mouseWatcher.Start();
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_LBUTTONDOWN:
                    // MouseMove?.Invoke();
                    break;
            }

            return IntPtr.Zero;
        }

        private void Grid_Loaded_1(object sender, RoutedEventArgs e)
        {
            AppBarFunctions.SetAppBar(this, ABEdge.Left);
        }

        private void App_Close(object sender, RoutedEventArgs e)
        {
            AppBarFunctions.SetAppBar(this, ABEdge.None);
            this.Close();
        }

        private void DoubleClick(object sender, RoutedEventArgs e)
        {             
            dblClick = true;
            mouseWatcher.OnMouseInput += (s, r) => {
                //Console.WriteLine(r.Message.ToString());
                if (dblClick == true)
                {
                    if (r.Message.ToString() == "WM_LBUTTONUP")
                    {
                        mouse.Click();
                        Console.WriteLine("duplo clicoui porra!!");
                        dblClick = false;
                    }
                }
                else {
                    return;
                }
                
            };
        }
        private void StopClick(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (noClick == true)
            {
                if (mwAtivado == false) {
                    mouseWatcher.Start();
                } 
                Console.WriteLine("entrou");
                noClick = false;
                mouseWatcher.OnMouseInput += (s, r) =>
                {
                    if (noClick == true) {
                        return;
                    }
                    if (r.Message.ToString() == "WM_LBUTTONDOWN")
                    {
                        HwndSource.FromHwnd(new WindowInteropHelper(this).Handle)?.AddHook(this.WndProc);
                        Console.WriteLine("impediu o clique");
                    }
                };
            } else if (noClick == false)
              {
                  noClick = true;
                  Console.WriteLine("saiu");
                  return;
              }                                               
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            mouseWatcher.Stop();
            eventHookFactory.Dispose();
        }

    }

}