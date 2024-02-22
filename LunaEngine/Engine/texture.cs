using System;
using System.IO;
using Silk.NET.OpenGL;
using StbImageSharp;
namespace Engine;

public class texture : IDisposable
    {
        private uint handle;
        private GL gl;

        public unsafe texture(GL gl, string path)
        {
            this.gl = gl;

            handle = this.gl.GenTexture();
            Bind();
            
            ImageResult result = ImageResult.FromMemory(File.ReadAllBytes(path.MakeAbsolute()), ColorComponents.RedGreenBlueAlpha);
            
            fixed (byte* ptr = result.Data)
            {
                gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint) result.Width, 
                    (uint) result.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ptr);
            }

            SetParameters();
        }
        
        private void SetParameters()
        {
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) GLEnum.ClampToEdge);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) GLEnum.ClampToEdge);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) GLEnum.LinearMipmapLinear);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) GLEnum.Linear);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 8);
            gl.GenerateMipmap(TextureTarget.Texture2D);
        }

        public void Bind(TextureUnit textureSlot = TextureUnit.Texture0)
        {
            gl.ActiveTexture(textureSlot);
            gl.BindTexture(TextureTarget.Texture2D, handle);
        }

        public void Dispose()
        {
            gl.DeleteTexture(handle);
        }
    }