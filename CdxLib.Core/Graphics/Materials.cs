using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace CdxLib.Core.Graphics
{
    public static class Materials
    {
        private static readonly Dictionary<string, Texture2D> _materials = new Dictionary<string, Texture2D>();

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="texture"></param>
        public static void AddMaterial(string name, Texture2D texture)
        {
            if (!_materials.ContainsKey(name.ToLower()))
                _materials.Add(name.ToLower(), texture);
        }

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Texture2D GetMaterial(string name)
        {
            return _materials[name.ToLower()];
        }
    }
}