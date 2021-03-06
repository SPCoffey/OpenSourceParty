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
using System.Drawing;
using GamepadHandler;

namespace MinigameLibrary
{
    /// <summary>
    /// A class that contains all of the essential methods for a menu class to be built off of.
    /// </summary>
    public abstract class GameAbstract : GameState
    {
        // Fields
        public GamepadManager padMan;


        // Properties
        public GameWindow Window { get; set; }
        public TimeSpan Elapsed { get; private set; }
        public List<GameObject> GameObjects { get; protected set; }


        // Constructors and Methods
        public void Run(GameWindow iWindow)
        {
            Window = iWindow;
            padMan = Window.PadMan;
            Init();
        }

        public GameAbstract() {  }   

        /// <summary>
        /// Initialization method so that common code between the constructors is in one place.
        /// </summary>
        public virtual void Init()
        {
            GameObjects = new List<GameObject>();
            AssignMouseDelegates();
            for (int i = 0; i < padMan.Devices.Count; i++)
            {
                if (padMan.Devices[i] != null)
                {
                    AssignGamepadDelegates(padMan.Devices[i], i);
                }
            }
            Window.Invalidate();
        }

        /// <summary>
        /// Restarts the game.
        /// </summary>
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
            Window.Invalidate();
        }

        /// <summary>
        /// Assigns delagates to each of the gamepads.
        /// </summary>
        /// <param name="gamepad"></param>
        /// <param name="index"></param>
        public abstract void AssignGamepadDelegates(GamepadStateHandler gamepad, int index);

        /// <summary>
        /// Removes delegates from each of the gamepads.
        /// </summary>
        /// <param name="gamepad"></param>
        /// <param name="index"></param>
        public abstract void DestroyGamepadDelegates(GamepadStateHandler gamepad, int index);

        /// <summary>
        /// Assigns delegates to the mouse.
        /// </summary>
        public abstract void AssignMouseDelegates();

        /// <summary>
        /// Removes delegates from the mouse.
        /// </summary>
        public abstract void DestroyMouseDelegates();

        /// <summary>
        /// Remove any outstanding menu pieces. Used when switching menus.
        /// </summary>
        public override void Destroy()
        {
            DestroyMouseDelegates();
            for (int i = 0; i < padMan.Devices.Count; i++)
            {
                if (padMan.Devices[i] != null)
                {
                    DestroyGamepadDelegates(padMan.Devices[i], i);
                }
            }
            GameObjects.Clear();
        }

        /// <summary>
        /// Updates this menu and all of it's controls.
        /// </summary>
        /// <param name="elapsedTime">Milliseconds since last update.</param>
        public override void Update(TimeSpan elapsedTime)
        {
            padMan.Update();
            Elapsed = elapsedTime;
            foreach (GameObject gameObject in GameObjects)
            {
                gameObject.Update(Elapsed.TotalMilliseconds);
            }
        }

        /// <summary>
        /// Draws all menu controls.
        /// </summary>
        public override void Draw(Graphics graphics, List<Rectangle> clipRectangles)
        {
            List<GameObject> drawObjects = new List<GameObject>();
            if (Window.linux)
            {
                foreach (GameObject gameObject in GameObjects)
                {
                    gameObject.Draw(graphics);
                }
            }
            else
            {
                foreach (GameObject gameObject in GameObjects)
                {
                    foreach (Rectangle rect in clipRectangles)
                    {
                        if (rect.IntersectsWith(gameObject.BoundingRect))
                        {
                            if (!drawObjects.Contains(gameObject))
                            {
                                drawObjects.Add(gameObject);
                                continue;
                            }
                        }
                    }
                }
                if (drawObjects.Count > 0)
                {
                    foreach (GameObject drawObject in drawObjects)
                    {
                        drawObject.Draw(graphics);
                    }
                }
            }
        }

        /// <summary>
        /// Draws all of the menu objects.
        /// </summary>
        public override void DrawAll()
        {
            foreach (GameObject gameObject in GameObjects)
            {
                gameObject.AutoInvalidate();
            }
        }
    }
}