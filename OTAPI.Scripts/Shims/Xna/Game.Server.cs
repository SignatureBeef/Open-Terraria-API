/*
Copyright (C) 2020 DeathCradle

This file is part of Open Terraria API v3 (OTAPI)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <http://www.gnu.org/licenses/>.
*/
#pragma warning disable CS0436 // Type conflicts with imported type

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Microsoft.Xna.Framework
{
    public class Game : IDisposable
    {
        public ContentManager Content { get; set; }

        public GameWindow Window { get; set; }

        public GraphicsDevice GraphicsDevice { get; set; }

        public bool IsActive { get; set; }

        public bool IsFixedTimeStep { get; set; }

        public bool IsMouseVisible { get; set; }

        public Game()
        {
            Content = new ContentManager(null);
            Window = new GameWindow();
        }

        protected virtual void Initialize()
        {
        }

        protected virtual void LoadContent()
        {
        }

        protected virtual void Draw(GameTime gameTime)
        {
        }

        protected virtual void Update(GameTime gameTime)
        {
        }

        protected virtual void UnloadContent()
        {
        }

        public void Exit()
        {
        }

        public void Dispose()
        {
        }
    }

    public sealed class GameComponentCollection : System.Collections.ObjectModel.Collection<IGameComponent>
    {

    }

    public interface IGameComponent
    {

    }

    public class LaunchParameters : System.Collections.Generic.Dictionary<string, string>
    {

    }

    public class GameServiceContainer : IServiceProvider
    {
        public void AddService(Type type, object provider) { }

        public void RemoveService(Type type) { }

        public object GetService(Type type) => null;
    }
}