using System;
using System.IO;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using Utilities;
using Newtonsoft.Json;

namespace MinecraftClicker
{
    public partial class MainWindow : Window
    {
        bool clickerActivated = true;

        bool breakingBlocks = false;
        bool buildingBlocks = false;

        int breakInterval = 500;   // Default value
        int buildInterval = 500;   // Default value
        int replaceInterval = 100; // Default value

        Keys quickBreakKey = Keys.P;   // Default value
        Keys quickBuildKey = Keys.B;   // Default value
        Keys quickReplaceKey = Keys.F; // Default value

        Timer breakerTimer;
        Timer builderTimer;

        GlobalKeyboardHook gkh = new GlobalKeyboardHook();

        ClickerConfigManager changedClickerConfig; // Used for applying changes
        void gkh_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            // Breaking block
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
            // Building block
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
            // Replacing block
            else if(e.KeyCode == quickReplaceKey)
            {
                ReplaceBlock();
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

            // If file exists -> changing default values
            // In worst case clicker will simply work with default values [No exceptions =)]
            if (File.Exists(Environment.CurrentDirectory + "\\saved_config.json"))
            {
                StreamReader sr = new StreamReader(Environment.CurrentDirectory + "\\saved_config.json");
                string savedJsonConfig = sr.ReadToEnd();
                sr.Close();

                ClickerConfigManager savedClickerConfig = JsonConvert.DeserializeObject<ClickerConfigManager>(savedJsonConfig);

                quickBreakKey = savedClickerConfig.savedQuickBreakKey;      //
                quickBuildKey = savedClickerConfig.savedQuickBuildKey;       //
                quickReplaceKey = savedClickerConfig.savedQuickReplaceKey;    //
                //==============================================================> Changing default values
                breakInterval = savedClickerConfig.savedBreakInterval;       //
                buildInterval = savedClickerConfig.savedBuildInterval;      //
                replaceInterval = savedClickerConfig.savedReplaceInterval; //
            }

            gkh.HookedKeys.Add(quickBreakKey);   // Now we're looking after this button
            gkh.HookedKeys.Add(quickBuildKey);   // And this button
            gkh.HookedKeys.Add(quickReplaceKey); // And this one too
            gkh.KeyDown += new System.Windows.Forms.KeyEventHandler(gkh_KeyDown); // And will use this method if any of them will be pressed

            //=========================Setting timers up===========================
            breakerTimer = new Timer();
            breakerTimer.Interval = breakInterval;
            breakerTimer.Tick += new EventHandler(BreakBlocks);

            builderTimer = new Timer();
            builderTimer.Interval = buildInterval;
            builderTimer.Tick += new EventHandler(BuildBlocks);
            //=====================================================================

            //==========================Applying values============================
            quickBreakTextBox.Text = Convert.ToString(quickBreakKey);
            quickBuildTextBox.Text = Convert.ToString(quickBuildKey);
            quickReplaceTextBox.Text = Convert.ToString(quickReplaceKey);

            quickBreakIntervalTextBox.Text = Convert.ToString(breakInterval);
            quickBuildIntervalTextBox.Text = Convert.ToString(buildInterval);
            quickReplaceIntervalTextBox.Text = Convert.ToString(replaceInterval);
            //=====================================================================
        }

        // If you click on any textbox with key in it and press any button, it will automatically apply pressed key
        private void quickBreakTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            quickBreakTextBox.Text = Convert.ToString(e.Key);
            Keys formsKey = (Keys)KeyInterop.VirtualKeyFromKey(e.Key); // formsKey gets a new value (new button)
            gkh.HookedKeys.Remove(quickBreakKey);                      // Then we delete previous value from our global hook (we won't look after this button)
            quickBreakKey = formsKey;                                  // Now a new button will do this
            gkh.HookedKeys.Add(quickBreakKey);                         // And we're looking after this button
            Keyboard.ClearFocus();
        }

        private void quickBuildTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            quickBuildTextBox.Text = Convert.ToString(e.Key);
            Keys formsKey = (Keys)KeyInterop.VirtualKeyFromKey(e.Key); // formsKey gets a new value (new button)
            gkh.HookedKeys.Remove(quickBuildKey);                      // Then we delete previous value from our global hook (we won't look after this button)
            quickBuildKey = formsKey;                                  // Now a new button will do this
            gkh.HookedKeys.Add(quickBuildKey);                         // And we're looking after this button
            Keyboard.ClearFocus();
        }

        private void replaceTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            quickReplaceTextBox.Text = Convert.ToString(e.Key);
            Keys formsKey = (Keys)KeyInterop.VirtualKeyFromKey(e.Key); // formsKey gets a new value (new button)
            gkh.HookedKeys.Remove(quickReplaceKey);                    // Then we delete previous value from our global hook (we won't look after this button)
            quickReplaceKey = formsKey;                                // Now a new button will do this
            gkh.HookedKeys.Add(quickReplaceKey);                       // And we're looking after this button
            Keyboard.ClearFocus();
        }

        private void disableClickerButton_Click(object sender, RoutedEventArgs e)
        {
            if (clickerActivated)
            {
                gkh.UnHook(); // Now we're not looking after buttons

                // If user's clicker was working -> it will stop it 
                breakerTimer.Stop();
                builderTimer.Stop();
                //===================================================

                clickerActivated = false;

                disableClickerButton.Content = "Activate";
                Title = "MineClicker (Deactivated)";

                //============Now user can change clicker settings====
                quickBreakTextBox.IsEnabled = true;
                quickBreakIntervalTextBox.IsEnabled = true;

                quickBuildTextBox.IsEnabled = true;
                quickBuildIntervalTextBox.IsEnabled = true;

                quickReplaceTextBox.IsEnabled = true;
                quickReplaceIntervalTextBox.IsEnabled = true;
                //====================================================
            }
            else
            {
                gkh.Hook(); // We're looking after buttons again

                // Checking if user input is OK
                try
                {
                    breakInterval = Convert.ToInt32(quickBreakIntervalTextBox.Text);
                    buildInterval = Convert.ToInt32(quickBuildIntervalTextBox.Text);
                    replaceInterval = Convert.ToInt32(quickReplaceIntervalTextBox.Text);
                    if ((breakInterval <= 0 || breakInterval > 99999) || 
                        (buildInterval <= 0 || buildInterval > 99999) ||
                        (replaceInterval <= 0 || replaceInterval > 99999)) // Same here
                    {
                        System.Windows.MessageBox.Show($"Interval value is TOO LOW or TOO HIGHT!", "Value error");
                    }
                    else // If everything is alright
                    {
                        breakerTimer.Interval = breakInterval; // Applying new value
                        builderTimer.Interval = buildInterval; // Applying new value

                        disableClickerButton.Content = "Deactivate";
                        Title = "MineClicker (Activated)";

                        //============Now user can't change clicker settings====
                        quickBreakTextBox.IsEnabled = false;
                        quickBreakIntervalTextBox.IsEnabled = false;

                        quickBuildTextBox.IsEnabled = false;
                        quickBuildIntervalTextBox.IsEnabled = false;

                        quickReplaceTextBox.IsEnabled = false;
                        quickReplaceIntervalTextBox.IsEnabled = false;
                        //======================================================

                        // When we activate clicker again that means we changed some settings 
                        // (Sometimes not actually, but that's not the point)
                        // Settings were changed -> we have to save new info
                        changedClickerConfig = new ClickerConfigManager(breakInterval, buildInterval, replaceInterval,
                                                                        quickBreakKey, quickBuildKey, quickReplaceKey);
                        string savedConfig = JsonConvert.SerializeObject(changedClickerConfig);
                        StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + "\\saved_config.json");
                        sw.Write(savedConfig);
                        sw.Close();

                        clickerActivated = true;
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
