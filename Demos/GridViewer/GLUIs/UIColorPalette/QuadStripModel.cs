﻿using CSharpGL;
using System;
using System.Drawing;

namespace GridViewer
{
    /// <summary>
    ///  /|\ y
    ///   |
    ///   |
    ///   |
    ///   ---------------&gt; x
    /// (0, 0)
    /// 0    2    4    6    8    10
    /// --------------------------
    /// |    |    |    |    |    |
    /// |    |    |    |    |    |
    /// |    |    |    |    |    |
    /// |    |    |    |    |    |
    /// |    |    |    |    |    |
    /// |    |    |    |    |    |
    /// |    |    |    |    |    |
    /// --------------------------
    /// 1    3    5    7    9    11
    /// side length is 1.
    /// </summary>
    internal class QuadStripModel : IBufferable
    {
        /// <summary>
        ///
        /// </summary>
        public const string position = "position";

        /// <summary>
        ///
        /// </summary>
        public const string texCoord = "texCoord";

        /// <summary>
        ///
        /// </summary>
        public const string color = "color";

        private VertexAttributeBuffer positionBuffer;
        private VertexAttributeBuffer texCoordBuffer;
        private VertexAttributeBuffer colorBuffer;

        public QuadStripModel(int quadCount, Bitmap bitmap = null)
        {
            this.quadCount = quadCount;
            this.bitmap = bitmap;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="bufferName"></param>
        /// <param name="varNameInShader"></param>
        /// <returns></returns>
        public VertexAttributeBuffer GetVertexAttributeBuffer(string bufferName, string varNameInShader)
        {
            if (bufferName == position)
            {
                if (this.positionBuffer == null)
                {
                    int length = (this.quadCount + 1) * 2;
                    VertexAttributeBuffer buffer = VertexAttributeBuffer.Create(typeof(vec3), length, VertexAttributeConfig.Vec3, BufferUsage.StaticDraw, varNameInShader);
                    unsafe
                    {
                        IntPtr pointer = buffer.MapBuffer(MapBufferAccess.WriteOnly);
                        var array = (vec3*)pointer;
                        for (int i = 0; i < (this.quadCount + 1); i++)
                        {
                            array[i * 2 + 0] = new vec3(-0.5f + (float)i / (float)(this.quadCount), 0.5f, 0);
                            array[i * 2 + 1] = new vec3(-0.5f + (float)i / (float)(this.quadCount), -0.5f, 0);
                        }
                        buffer.UnmapBuffer();
                    }

                    this.positionBuffer = buffer;
                }
                return this.positionBuffer;
            }
            else if (bufferName == texCoord)
            {
                if (this.texCoordBuffer == null)
                {
                    int length = (this.quadCount + 1) * 2;
                    VertexAttributeBuffer buffer = VertexAttributeBuffer.Create(typeof(float), length, VertexAttributeConfig.Float, BufferUsage.StaticDraw, varNameInShader);
                    unsafe
                    {
                        IntPtr pointer = buffer.MapBuffer(MapBufferAccess.WriteOnly);
                        //Random random = new Random();
                        var array = (float*)pointer;
                        for (int i = 0; i < (this.quadCount + 1); i++)
                        {
                            //array[i * 2 + 0] = (float)random.NextDouble();
                            array[i * 2 + 0] = (float)i / (float)this.quadCount;
                            array[i * 2 + 1] = array[i * 2 + 0];
                        }
                        buffer.UnmapBuffer();
                    }

                    this.texCoordBuffer = buffer;
                }
                return this.texCoordBuffer;
            }
            else if (bufferName == color)
            {
                if (this.colorBuffer == null)
                {
                    var buffer = VertexAttributeBuffer.Create(typeof(vec3), (this.quadCount + 1) * 2, VertexAttributeConfig.Vec3, BufferUsage.StaticDraw, varNameInShader);
                    unsafe
                    {
                        var array = (vec3*)buffer.MapBuffer(MapBufferAccess.WriteOnly);
                        for (int i = 0; i < (this.quadCount + 1); i++)
                        {
                            int x = this.bitmap.Width * i / this.quadCount;
                            if (x == this.bitmap.Width) { x = this.bitmap.Width - 1; }
                            vec3 value = this.bitmap.GetPixel(x, 0).ToVec3();
                            array[i * 2 + 0] = value;
                            array[i * 2 + 1] = value;
                        }
                        buffer.UnmapBuffer();
                    }
                    this.colorBuffer = buffer;
                }
                return this.colorBuffer;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public IndexBuffer GetIndexBuffer()
        {
            if (indexBuffer == null)
            {
                ZeroIndexBuffer buffer = ZeroIndexBuffer.Create(DrawMode.QuadStrip, 0, (this.quadCount + 1) * 2);
                this.indexBuffer = buffer;
            }

            return indexBuffer;
        }

        private IndexBuffer indexBuffer = null;
        /// <summary>
        /// Uses <see cref="ZeroIndexBuffer"/> or <see cref="OneIndexBuffer"/>.
        /// </summary>
        /// <returns></returns>
        public bool UsesZeroIndexBuffer() { return true; }

        internal int quadCount;
        private Bitmap bitmap;
    }
}