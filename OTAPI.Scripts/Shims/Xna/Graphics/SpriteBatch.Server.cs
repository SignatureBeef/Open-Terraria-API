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

using System.Text;

namespace Microsoft.Xna.Framework.Graphics
{
    public class SpriteBatch
    {
        public SpriteBatch(GraphicsDevice graphicsDevice)
        {
        }

        public GraphicsDevice GraphicsDevice { get; set; }

        public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, float layerDepth)
        {
        }

        public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
        }
        //public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth) { }
        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
        }

        public void Draw(Texture2D texture, Vector2 position, Color color)
        {
        }

        public void Draw(Texture2D texture, Color color)
        {
        }

        public void Draw(Texture2D texture, Rectangle destinationRectangle, Color color)
        {
        }

        public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
        {
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
        {
        }

        public void Draw(Texture2D a, Rectangle b, Rectangle? c, Color d, float e, Vector2 f, SpriteEffects g, float h)
        {
        }

        public void Begin(SpriteSortMode sortMode, BlendState blendState)
        {
        }

        public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix transformMatrix)
        {
        }

        public void Begin()
        {
        }

        public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState)
        {
        }

        public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect)
        {
        }

        public void End()
        {
        }


        public void DrawString(
            SpriteFont spriteFont,
            string text,
            Vector2 position,
            Color color
        )
        {
        }

        public void DrawString(
            SpriteFont spriteFont,
            string text,
            Vector2 position,
            Color color,
            float rotation,
            Vector2 origin,
            Vector2 scale,
            SpriteEffects effects,
            float layerDepth
        )
        {
        }

        public void DrawString(
            SpriteFont spriteFont,
            StringBuilder text,
            Vector2 position,
            Color color
        )
        {
        }

        public void DrawString(
            SpriteFont spriteFont,
            StringBuilder text,
            Vector2 position,
            Color color,
            float rotation,
            Vector2 origin,
            float scale,
            SpriteEffects effects,
            float layerDepth
        )
        {
        }

        public void DrawString(
            SpriteFont spriteFont,
            StringBuilder text,
            Vector2 position,
            Color color,
            float rotation,
            Vector2 origin,
            Vector2 scale,
            SpriteEffects effects,
            float layerDepth
        )
        {
        }
    }
}