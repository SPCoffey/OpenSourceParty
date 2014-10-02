﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenSourceParty
{
    public partial class MainMenu : Form
    {
        FileManager fileMan;
        GamepadManager padMan;

        public MainMenu()
        {
            fileMan = new FileManager();   // Instantiate a new file manager.
            padMan = new GamepadManager();
            String background = fileMan.RandomFile(fileMan.BackgroundDir, fileMan.ImageExtension);
            if (background != null)
            {
                this.BackgroundImage = Image.FromFile(background);   // Set the background image.
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }
            InitializeComponent();
            padMan.Init();
            if (padMan[0] != null)
            {
                padMan[0].lJoystickDelegate = new GamepadState.JoystickDelegate(thumbstickManage);
                padMan[0].bDelagate = new GamepadState.GamepadDelegate(button2_Click);
            }   
        }

        /// <summary>
        /// Manages thumbstick input.
        /// </summary>
        /// <param name="sender">The gamepad that sent the delegate.</param>
        /// <param name="j">The joystick arguments.</param>
        private void thumbstickManage(object sender, JoystickArgs j)
        {
            if (j.thumbstick.Position.Y > 0)
            {
                button1_Click(sender, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called when button1 is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            fileMan.RandomGame();
        }

        /// <summary>
        /// Called when button2 is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Games:");
            fileMan.printFileList(fileMan.MinigameDir, fileMan.GameExtension);
            Console.WriteLine("Backgrounds:");
            fileMan.printFileList(fileMan.BackgroundDir, fileMan.ImageExtension);
        }
    }
}
