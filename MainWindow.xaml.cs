using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace eMonkey
{
    public partial class MainWindow : Window
    {
        private readonly PatientVisitViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new PatientVisitViewModel();
            DataContext = _viewModel;

            //get handle from a window with Title starting with "eCW"
            var handle = WindowHelper.GetWindowHandle("eCW");

            _viewModel.LoadPatientVisits(new IntPtr(handle)); // Example handle
        }
    }

    public static class WindowHelper
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        private const uint GW_HWNDNEXT = 2;

        public static IntPtr GetWindowHandle(string titleStartsWith)
        {
            IntPtr hWnd = FindWindow(null, null);
            while (hWnd != IntPtr.Zero)
            {
                int length = GetWindowTextLength(hWnd);
                StringBuilder sb = new StringBuilder(length + 1);
                GetWindowText(hWnd, sb, sb.Capacity);

                if (sb.ToString().StartsWith(titleStartsWith))
                {
                    return hWnd;
                }

                hWnd = GetWindow(hWnd, GW_HWNDNEXT);
            }
            return IntPtr.Zero;
        }
    }
}
