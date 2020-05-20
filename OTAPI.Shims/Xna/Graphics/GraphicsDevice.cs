using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace Microsoft.Xna.Framework.Graphics
{
	public class GraphicsDevice : IDisposable
	{
		public event EventHandler<EventArgs> Disposing;

		public event EventHandler<ResourceDestroyedEventArgs> ResourceDestroyed;

		public event EventHandler<ResourceCreatedEventArgs> ResourceCreated;

		public event EventHandler<EventArgs> DeviceLost;

		public event EventHandler<EventArgs> DeviceReset;

		public event EventHandler<EventArgs> DeviceResetting;

		public bool IsDisposed
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get;
		}

		public unsafe Rectangle ScissorRectangle { get; set; }

		public unsafe IndexBuffer Indices { get; set; }

		public unsafe Viewport Viewport { get; set; }

		public unsafe DisplayMode DisplayMode { get; }

		public unsafe GraphicsDeviceStatus GraphicsDeviceStatus { get; }

		public GraphicsProfile GraphicsProfile { get; }

		public GraphicsAdapter Adapter { get; }

		public PresentationParameters PresentationParameters { get; }

		public RasterizerState RasterizerState { get; set; }

		public unsafe int ReferenceStencil { get; set; }

		public DepthStencilState DepthStencilState { get; set; }

		public unsafe int MultiSampleMask { get; set; }

		public unsafe Color BlendFactor { get; set; }

		public BlendState BlendState { get; set; }

		public TextureCollection VertexTextures { get; }

		public TextureCollection Textures { get; }

		public SamplerStateCollection VertexSamplerStates { get; }

		public SamplerStateCollection SamplerStates { get; }

		public unsafe void Present(Rectangle? sourceRectangle, Rectangle? destinationRectangle, IntPtr overrideWindowHandle)
		{ }

		public void Present()
		{ }

		protected void raise_DeviceResetting(object value0, EventArgs value1)
		{ }

		protected void raise_DeviceReset(object value0, EventArgs value1)
		{ }

		protected void raise_DeviceLost(object value0, EventArgs value1)
		{ }

		protected void raise_ResourceCreated(object value0, ResourceCreatedEventArgs value1)
		{ }

		protected void raise_ResourceDestroyed(object value0, ResourceDestroyedEventArgs value1)
		{ }

		public GraphicsDevice(GraphicsAdapter adapter, GraphicsProfile graphicsProfile, PresentationParameters presentationParameters)
		{ }

		public void Reset()
		{ }

		public void Reset(PresentationParameters presentationParameters)
		{ }

		public unsafe void Reset(PresentationParameters presentationParameters, GraphicsAdapter graphicsAdapter)
		{ }

		public unsafe void DrawPrimitives(PrimitiveType primitiveType, int startVertex, int primitiveCount)
		{ }

		public unsafe void DrawIndexedPrimitives(PrimitiveType primitiveType, int baseVertex, int minVertexIndex, int numVertices, int startIndex, int primitiveCount)
		{ }

		public unsafe void DrawInstancedPrimitives(PrimitiveType primitiveType, int baseVertex, int minVertexIndex, int numVertices, int startIndex, int primitiveCount, int instanceCount)
		{ }

		public void DrawUserIndexedPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int numVertices, int[] indexData, int indexOffset, int primitiveCount) where T : struct, IVertexType
		{ }

		public void DrawUserIndexedPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int numVertices, short[] indexData, int indexOffset, int primitiveCount) where T : struct, IVertexType
		{ }

		public void DrawUserIndexedPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int numVertices, int[] indexData, int indexOffset, int primitiveCount, VertexDeclaration vertexDeclaration) where T : struct
		{ }

		public void DrawUserIndexedPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int numVertices, short[] indexData, int indexOffset, int primitiveCount, VertexDeclaration vertexDeclaration) where T : struct { }

		public void DrawUserPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int primitiveCount) where T : struct, IVertexType { }

		public unsafe void DrawUserPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int primitiveCount, VertexDeclaration vertexDeclaration) where T : struct { }

		public void Clear(ClearOptions options, Vector4 color, float depth, int stencil) { }

		public unsafe void Clear(ClearOptions options, Color color, float depth, int stencil) { }

		public void Clear(Color color) { }

		public unsafe void SetRenderTargets(params RenderTargetBinding[] renderTargets) { }

		public unsafe void SetRenderTarget(RenderTargetCube renderTarget, CubeMapFace cubeMapFace) { }

		public unsafe void SetRenderTarget(RenderTarget2D renderTarget) { }

		public RenderTargetBinding[] GetRenderTargets() { return null; }

		public unsafe void GetBackBufferData<T>(Rectangle? rect, T[] data, int startIndex, int elementCount) where T : struct { }

		public void GetBackBufferData<T>(T[] data, int startIndex, int elementCount) where T : struct { }

		public void GetBackBufferData<T>(T[] data) where T : struct { }

		public VertexBufferBinding[] GetVertexBuffers() { return null; }

		public unsafe void SetVertexBuffer(VertexBuffer vertexBuffer, int vertexOffset) { }

		public unsafe void SetVertexBuffer(VertexBuffer vertexBuffer) { }

		public unsafe void SetVertexBuffers(params VertexBufferBinding[] vertexBuffers) { }

		internal void raise_DrawGuide(object sender, EventArgs e) { }

		[HandleProcessCorruptedStateExceptions]
		protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool flag) { }

		public void Dispose() { }
	}

	public enum GraphicsDeviceStatus
	{
		Normal,
		Lost,
		NotReset
	}

	public enum GraphicsProfile
	{
		Reach,
		HiDef
	}

	[Flags]
	public enum ClearOptions
	{
		Target = 1,
		DepthBuffer = 2,
		Stencil = 4
	}

	public enum CubeMapFace
	{
		PositiveX,
		NegativeX,
		PositiveY,
		NegativeY,
		PositiveZ,
		NegativeZ
	}


	public class TextureCube : Texture //, IGraphicsResource
	{
		public int Size { get; }

		public TextureCube(GraphicsDevice graphicsDevice, int size, [MarshalAs(UnmanagedType.U1)] bool mipMap, SurfaceFormat format)
		{ }

		public void SetData<T>(CubeMapFace cubeMapFace, T[] data) where T : struct
		{ }

		public void SetData<T>(CubeMapFace cubeMapFace, T[] data, int startIndex, int elementCount) where T : struct
		{ }

		public void SetData<T>(CubeMapFace cubeMapFace, int level, Rectangle? rect, T[] data, int startIndex, int elementCount) where T : struct
		{ }

		public void GetData<T>(CubeMapFace cubeMapFace, T[] data) where T : struct
		{ }

		public void GetData<T>(CubeMapFace cubeMapFace, T[] data, int startIndex, int elementCount) where T : struct
		{ }

		public void GetData<T>(CubeMapFace cubeMapFace, int level, Rectangle? rect, T[] data, int startIndex, int elementCount) where T : struct
		{ }
	}

	public class RenderTargetCube : TextureCube //, IDynamicGraphicsResource
	{
		public virtual event EventHandler<EventArgs> ContentLost;

		public bool IsContentLost
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get;
		}

		public RenderTargetUsage RenderTargetUsage { get; }

		public int MultiSampleCount { get; }

		public DepthFormat DepthStencilFormat { get; }

		public RenderTargetCube(GraphicsDevice graphicsDevice, int size, [MarshalAs(UnmanagedType.U1)] bool mipMap, SurfaceFormat preferredFormat, DepthFormat preferredDepthFormat, int preferredMultiSampleCount, RenderTargetUsage usage)
			: base(graphicsDevice, size, mipMap, preferredFormat)
		{ }

		public RenderTargetCube(GraphicsDevice graphicsDevice, int size, [MarshalAs(UnmanagedType.U1)] bool mipMap, SurfaceFormat preferredFormat, DepthFormat preferredDepthFormat)
			: base(graphicsDevice, size, mipMap, preferredFormat)
		{ }

		protected virtual void raise_ContentLost(object value0, EventArgs value1)
		{ }
	}

	public struct RenderTargetBinding
	{
		public CubeMapFace CubeMapFace { get; }

		public Texture RenderTarget { get; }

		public RenderTargetBinding(RenderTargetCube renderTarget, CubeMapFace cubeMapFace)
		{
			this.RenderTarget = null;
			this.CubeMapFace = cubeMapFace;
		}

		public RenderTargetBinding(RenderTarget2D renderTarget)
			: this(null, CubeMapFace.PositiveX)
		{ }

		public static implicit operator RenderTargetBinding(RenderTarget2D renderTarget)
		{
			return default(RenderTargetBinding);
		}
	}

	public struct VertexBufferBinding
	{
		public int InstanceFrequency { get; }

		public int VertexOffset { get; }

		public VertexBuffer VertexBuffer { get; }

		public VertexBufferBinding(VertexBuffer vertexBuffer, int vertexOffset, int instanceFrequency)
		{
			this.InstanceFrequency = 0;
			this.VertexOffset = 0;
			this.VertexBuffer = default(VertexBuffer);
		}

		public VertexBufferBinding(VertexBuffer vertexBuffer, int vertexOffset)
			: this(vertexBuffer, vertexOffset, 0)
		{ }

		public VertexBufferBinding(VertexBuffer vertexBuffer)
			: this(vertexBuffer, 0)
		{ }

		public static implicit operator VertexBufferBinding(VertexBuffer vertexBuffer)
		{
			return default(VertexBufferBinding);
		}
	}

	public enum PrimitiveType
	{

	}

	public class VertexBuffer : GraphicsResource
	{
	}

	public class TextureCollection
	{
		public Texture this[int index]
		{
			get
			{
				return null;
			}
			set { }
		}

	}

	public class SamplerStateCollection
	{
		public SamplerState this[int index]
		{
			get
			{
				return null;
			}
			set { }
		}
	}
	public interface IGraphicsDeviceService
	{
		GraphicsDevice GraphicsDevice
		{
			get;
		}

		event EventHandler<EventArgs> DeviceDisposing;

		event EventHandler<EventArgs> DeviceReset;

		event EventHandler<EventArgs> DeviceResetting;

		event EventHandler<EventArgs> DeviceCreated;
	}
}