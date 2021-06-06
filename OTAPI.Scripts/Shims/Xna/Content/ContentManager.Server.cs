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
using System;

namespace Microsoft.Xna.Framework.Content
{
    public class ContentManager
    {
        public string RootDirectory { get; set; }

        public IServiceProvider ServiceProvider { get; }

        public ContentManager(IServiceProvider serviceProvider) : this(serviceProvider, string.Empty)
        {
        }

        public ContentManager(IServiceProvider serviceProvider, string rootDirectory)
        { }

        public T Load<T>(string path)
        {
            return default(T);
        }
        //public Effect Load<Effect>(string path)
        //{
        //    return default(Effect);
        //}

        public T ReadAsset<T>(string name, Action<IDisposable> _)
        {
            return default(T);
        }
    }
}