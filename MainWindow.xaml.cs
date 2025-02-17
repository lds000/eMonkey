using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace eMonkey
{
    public partial class MainWindow : Window
    {
        // ViewModel instance for data binding
        private readonly PatientVisitViewModel _viewModel;

        ChromeWindowM _chromeWindow;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Get handle from a window with Title starting with "eCW"
            var handle = WindowHelper.GetWindowHandle("eCW (");

            _chromeWindow = new ChromeWindowM(handle);
            _viewModel = new PatientVisitViewModel(_chromeWindow);

            // Set DataContext after initializing the ViewModel
            DataContext = _viewModel;

            // Load patient visits using the obtained window handle
            _viewModel.LoadPatientVisits(handle); // Example handle
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

        /// <summary>
        /// Gets the handle of a window whose title starts with the specified string.
        /// </summary>
        /// <param name="titleStartsWith">The starting string of the window title.</param>
        /// <returns>The handle of the window if found; otherwise, IntPtr.Zero.</returns>
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

