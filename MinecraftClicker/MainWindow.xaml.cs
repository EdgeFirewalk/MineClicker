using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using Utilities;
using System.Windows.Resources;
using System.Windows.Media;
using System.Media;

namespace MinecraftClicker
{
    public partial class MainWindow : Window
    {
        bool clickerActivated = true;

        bool breakingBlocks = false;
        bool buildingBlocks = false;

        int breakInterval = 500;
        int buildInterval = 500;
        int replaceInterval = 100;

        System.Windows.Forms.Keys quickBreakKey = Keys.P;
        System.Windows.Forms.Keys quickBuildKey = Keys.B;
        System.Windows.Forms.Keys quickReplaceKey = Keys.F;

        Timer breakerTimer;
        Timer builderTimer;

        GlobalKeyboardHook gkh = new GlobalKeyboardHook();
        void gkh_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            // Breaking code
            if (e.KeyCode == quickBreakKey)
            {
                if (!breakingBlocks)
                {
                    breakerTimer.Start();
                    breakingBlocks = !breakingBlocks;
                }
                else if (breakingBlocks)
                {
                    breakerTimer.Stop();
                    breakingBlocks = !breakingBlocks;
                }
            }
            // Building code
            else if(e.KeyCode == quickBuildKey)
            {
                if (!buildingBlocks)
                {
                    builderTimer.Start();
                    buildingBlocks = !buildingBlocks;
                }
                else if (buildingBlocks)
                {
                    builderTimer.Stop();
                    buildingBlocks = !buildingBlocks;
                }
            }
            // Replacing code
            else if(e.KeyCode == quickReplaceKey)
            {
                ReplaceBlock();
            }
            // Self destruct code
            else if (e.KeyCode == Keys.X)
            {
                Process.GetCurrentProcess().Kill();
            }
        }

        private void BreakBlocks(object sender, EventArgs e)
        {
            MouseOperations.PressLeftMouse();
        }

        private void BuildBlocks(object sender, EventArgs e)
        {
            MouseOperations.PressRightMouse();
        }

        private void ReplaceBlock()
        {
            MouseOperations.PressLeftMouse();
            System.Threading.Thread.Sleep(replaceInterval);
            MouseOperations.PressRightMouse();
        }

        public MainWindow()
        {
            InitializeComponent();
            gkh.HookedKeys.Add(quickBreakKey);
            gkh.HookedKeys.Add(quickBuildKey);
            gkh.HookedKeys.Add(quickReplaceKey);
            gkh.KeyDown += new System.Windows.Forms.KeyEventHandler(gkh_KeyDown);

            breakerTimer = new Timer();
            breakerTimer.Interval = breakInterval;
            breakerTimer.Tick += new EventHandler(BreakBlocks);

            builderTimer = new Timer();
            builderTimer.Interval = buildInterval;
            builderTimer.Tick += new EventHandler(BuildBlocks);
        }

        private void quickBreakTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            quickBreakTextBox.Text = Convert.ToString(e.Key);
            Keys formsKey = quickBreakKey;                        // Временная переменная
            formsKey = (Keys)KeyInterop.VirtualKeyFromKey(e.Key); // Получает нажатую кнопку
            gkh.HookedKeys.Remove(quickBreakKey);                 // Удаляется старое значение для хука
            quickBreakKey = formsKey;                             // На действие назначается новая кнопка
            gkh.HookedKeys.Add(quickBreakKey);                    // Хук отслеживает эту новую кнопку
            Keyboard.ClearFocus();
        }

        private void quickBuildTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            quickBuildTextBox.Text = Convert.ToString(e.Key);
            Keys formsKey = quickBuildKey;                        // Временная переменная
            formsKey = (Keys)KeyInterop.VirtualKeyFromKey(e.Key); // Получает нажатую кнопку
            gkh.HookedKeys.Remove(quickBuildKey);                 // Удаляется старое значение для хука
            quickBuildKey = formsKey;                             // На действие назначается новая кнопка
            gkh.HookedKeys.Add(quickBuildKey);                    // Хук отслеживает эту новую кнопку
            Keyboard.ClearFocus();
        }

        private void replaceTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            quickReplaceTextBox.Text = Convert.ToString(e.Key);
            Keys formsKey = quickReplaceKey;                        // Временная переменная
            formsKey = (Keys)KeyInterop.VirtualKeyFromKey(e.Key); // Получает нажатую кнопку
            gkh.HookedKeys.Remove(quickReplaceKey);                 // Удаляется старое значение для хука
            quickReplaceKey = formsKey;                             // На действие назначается новая кнопка
            gkh.HookedKeys.Add(quickReplaceKey);                    // Хук отслеживает эту новую кнопку
            Keyboard.ClearFocus();
        }

        private void disableClickerButton_Click(object sender, RoutedEventArgs e)
        {
            if (clickerActivated)
            {
                gkh.UnHook();

                breakerTimer.Stop();
                builderTimer.Stop();

                clickerActivated = false;

                disableClickerButton.Content = "Activate";
                Title = "MineClicker (Deactivated)";

                quickBreakTextBox.IsEnabled = true;
                quickBreakIntervalTextBox.IsEnabled = true;

                quickBuildTextBox.IsEnabled = true;
                quickBuildIntervalTextBox.IsEnabled = true;

                quickReplaceTextBox.IsEnabled = true;
                quickReplaceIntervalTextBox.IsEnabled = true;
            }
            else
            {
                gkh.Hook();

                try
                {
                    breakInterval = Convert.ToInt32(quickBreakIntervalTextBox.Text);
                    buildInterval = Convert.ToInt32(quickBuildIntervalTextBox.Text);
                    replaceInterval = Convert.ToInt32(quickReplaceIntervalTextBox.Text);
                    if ((breakInterval <= 0 || breakInterval > 99999) || 
                        (buildInterval <= 0 || buildInterval > 99999) ||
                        (replaceInterval <= 0 || replaceInterval > 99999))
                    {
                        System.Windows.MessageBox.Show($"Interval value is TOO LOW or TOO HIGHT!", "Value error");
                    }
                    else 
                    {
                        clickerActivated = true;

                        breakerTimer.Interval = breakInterval;
                        builderTimer.Interval = buildInterval;

                        disableClickerButton.Content = "Deactivate";
                        Title = "MineClicker (Activated)";

                        quickBreakTextBox.IsEnabled = false;
                        quickBreakIntervalTextBox.IsEnabled = false;

                        quickBuildTextBox.IsEnabled = false;
                        quickBuildIntervalTextBox.IsEnabled = false;

                        quickReplaceTextBox.IsEnabled = false;
                        quickReplaceIntervalTextBox.IsEnabled = false;
                    }
                }
                catch (FormatException)
                {
                    System.Windows.MessageBox.Show($"Interval fields must contain ONLY NUMBERS!", "Convert error");
                }
            }
        }
    }
}
