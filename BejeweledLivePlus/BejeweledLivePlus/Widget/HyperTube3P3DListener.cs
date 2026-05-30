using SexyFramework;
using SexyFramework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BejeweledLivePlus.Widget
{
    public class HyperTube3P3DListener: Bej3P3DListener
    {
        private Image[] mTextures = new Image[2];

        public void SetTexture(int index, Image texture)
        {
            if (index < 0 || index >= mTextures.Length)
            {
                return;
            }
            mTextures[index] = texture;
        }

        public override void MeshPreDrawSet(Mesh theMesh, string theMeshName, string theSetName, bool hasBump)
        {
            Graphics3D graphics3D = g?.Get3D();
            if (graphics3D != null)
            {
                graphics3D.SetTexture(0, mTextures[0]);
                graphics3D.SetTexture(1, mTextures[1]);
            }
        }

        public override void MeshPostDrawSet(Mesh theMesh, string theMeshName, string theSetName)
        {
            Graphics3D graphics3D = g?.Get3D();
            if (graphics3D != null)
            {
                graphics3D.SetTexture(0, null);
                graphics3D.SetTexture(1, null);
            }
        }
    }
}
