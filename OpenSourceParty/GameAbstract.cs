﻿/*  Open Source Party, a platform for making and playing party minigames with your friends
    Copyright (C) 2014  Sean Coffey

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Timers;
using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.DXGI;
using SlimDX.Windows;
using SpriteHandler;
using MenuHandler;
using GamepadHandler;
using FileHandler;
using System.Diagnostics;
using System.IO;

namespace GameAbstracts
{
    /// <summary>
    /// A class that contains all of the essential methods for a menu class to be built off of.
    /// </summary>
    public abstract class GameAbstract : GameState
    {
        // Fields
        protected List<GameObject> gameObjects;
        protected FileManager fileMan;
        protected GameState returnState;
        public GamepadManager padMan;
        public GameManager Manager { get; set; }
        public TimeSpan Elapsed { get; private set; }


        // Properties
        public List<GameObject> GameObjects
        {
            get
            {
                return gameObjects;
            }
        }
        public FileManager FileMan
        {
            get
            {
                return fileMan;
            }
            protected set
            {
                fileMan = value;
            }
        }

        // Constructors and Methods
        public void Run(GameManager iManager, GamepadManager iPadMan, FileManager iFileMan, GameState iReturnState)
        {
            Manager = iManager;
            Manager.CurState = this;
            padMan = iPadMan;
            fileMan = iFileMan;
            returnState = iReturnState;
            Init();
        }

        public GameAbstract() {  }   

        /// <summary>
        /// Initialization method so that common code between the constructors is in one place.
        /// </summary>
        public virtual void Init()
        {
            gameObjects = new List<GameObject>();
            AssignMouseDelegates();
            for (int i = 0; i < padMan.Devices.Count; i++)
            {
                if (padMan.Devices[i] != null)
                {
                    AssignGamepadDelegates(padMan.Devices[i], i);
                }
            }
            Manager.Invalidate();
        }

        public override void Restart()
        {
            AssignMouseDelegates();
            for (int i = 0; i < padMan.Devices.Count; i++)
            {
                if (padMan.Devices[i] != null)
                {
                    AssignGamepadDelegates(padMan.Devices[i], i);
                }
            }
            Manager.Invalidate();
        }

        public abstract void AssignGamepadDelegates(GamepadState gamepad, int index);

        public abstract void DestroyGamepadDelegates(GamepadState gamepad, int index);

        public abstract void AssignMouseDelegates();

        public abstract void DestroyMouseDelegates();

        /// <summary>
        /// Remove any outstanding menu pieces. Used when switching menus.
        /// </summary>
        public virtual void Destroy()
        {
            DestroyMouseDelegates();
            for (int i = 0; i < padMan.Devices.Count; i++)
            {
                if (padMan.Devices[i] != null)
                {
                    DestroyGamepadDelegates(padMan.Devices[i], i);
                }
            }
            Manager.CurState = returnState;
            gameObjects.Clear();
            returnState.Restart();
        }

        /// <summary>
        /// Updates this menu and all of it's controls.
        /// </summary>
        /// <param name="elapsedTime">Milliseconds since last update.</param>
        public override void Update(TimeSpan elapsedTime)
        {
            padMan.Update();
            Elapsed = elapsedTime;
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Update(Elapsed.TotalMilliseconds);
            }
        }

        /// <summary>
        /// Draws all menu controls.
        /// </summary>
        public override void Draw(Graphics graphics, List<Rectangle> clipRectangles)
        {
            Graphics = graphics;
            foreach (GameObject gameObject in gameObjects)
            {
                foreach (Rectangle rect in clipRectangles)
                {
                    if (rect.IntersectsWith(gameObject.BoundingRect))
                    {
                        gameObject.Draw(Graphics);
                    }
                }
            }
        }

        /// <summary>
        /// Draws all of the menu objects.
        /// </summary>
        public override void DrawAll()
        {
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.AutoInvalidate();
            }
        }
    }
}