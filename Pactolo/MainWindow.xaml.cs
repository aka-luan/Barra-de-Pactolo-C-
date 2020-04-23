using System;
using System.Runtime.InteropServices;
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
    /// 

    public partial class MainWindow : Window
    {
        //https://stackoverflow.com/questions/586547/how-can-i-block-keyboard-and-mouse-input-in-c testando o BlockInput
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "BlockInput")]
        [return: System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)]
        public static extern bool BlockInput([System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)] bool fBlockIt);
        //criando variáveis que correspondem a biblioteca EventHook
        private readonly EventHookFactory eventHookFactory = new EventHookFactory();
        private readonly MouseWatcher mouseWatcher;
        //variáveis gerais e flags
        public bool dblClick = false;
        public bool mwAtivado, click = true;
        //variável referente a biblioteca do Quellatalo que faz que simula as ações do mouse
        public MouseHandler mouse = new MouseHandler();
        private const int WM_LBUTTONDOWN = 0x0201;


        //Inicializando a janela principal da aplicação
        public MainWindow()
        {
            InitializeComponent();
            System.Windows.Application.Current.Exit += OnApplicationExit;

            //Assim que o aplicativo começa a rodar, o mouseWatcher começa a receber os eventos do mouse
            mouseWatcher = eventHookFactory.GetMouseWatcher();
            mouseWatcher.Start();
        }

        

        private void Grid_Loaded_1(object sender, RoutedEventArgs e)
        {
            //Seta a janela pra fiar ancorada no canto esquerdo da tela (ABEdge.Left). Isso diz respeito a biblioteca WpfAppBar
            AppBarFunctions.SetAppBar(this, ABEdge.Left);
        }

        private void App_Close(object sender, RoutedEventArgs e)
        {
            //Função p normalizar a ancoragem quando a aplicação fechar e não dar bug no Explorer
            AppBarFunctions.SetAppBar(this, ABEdge.None);
            this.Close();
        }

        private void DoubleClick(object sender, RoutedEventArgs e)
        {             
            dblClick = true;
            //Qnd o botão de duplo clique é iniciado, o mouse começa a ser vigiado. Se a flag estiver certinha, a função manda um click pro sistema assim que detectar uma mensagem ''WM_LBUTTONUP''
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

        //função p parar o clique em teste

        private void StopClick(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (click == true)
            {
                Console.WriteLine("false");
                MainWindow.BlockInput(true);
                click = false;
               
            } else if (click == false)
              {
                Console.WriteLine("true");
                click = true;
                MainWindow.BlockInput(false);
                return;
              }                                               
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            //finaliza o mouseWatcher
            mouseWatcher.Stop();
            eventHookFactory.Dispose();
        }
    }
}