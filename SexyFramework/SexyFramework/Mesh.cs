using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using SexyFramework.Graphics;

namespace SexyFramework
{
	public class Mesh : IDisposable
	{
		public string mFileName;

		public MeshListener mListener;

		public object mUserData;

		public List<MeshPiece> mPieces = new List<MeshPiece>();

		public Mesh()
		{
			mListener = null;
			mUserData = null;
			GlobalMembers.gSexyAppBase.mGraphicsDriver.AddMesh(this);
		}

		public void Dispose()
		{
			if (mListener != null)
			{
				mListener.MeshPreDeleted(this);
			}
			Cleanup();
		}

		public virtual void Cleanup()
		{
			List<MeshPiece>.Enumerator enumerator = mPieces.GetEnumerator();
			while (enumerator.MoveNext())
			{
				enumerator.Current?.Dispose();
			}
			mPieces.Clear();
		}

		public virtual void SetListener(MeshListener theListener)
		{
			mListener = theListener;
		}

		public bool LoadFromFile(string thePath)
		{
			byte[] bytes;
			try
			{
				using (Stream s = TitleContainer.OpenStream("Content/" + thePath.Replace("\\", "/")))
				{
					if (s == null) { Console.WriteLine($"[Mesh] {thePath}: stream null"); return false; }
					using (var ms = new MemoryStream())
					{
						s.CopyTo(ms);
						bytes = ms.ToArray();
					}
				}
			}
			catch (Exception ex)
			{
                return false;
            }

			try
			{
				using (var br = new BinaryReader(new MemoryStream(bytes)))
				{
					int magic = br.ReadInt32();
					if (magic != 0x3DBEEF00) { Console.WriteLine($"[Mesh] {thePath}: bad magic 0x{magic:X}"); return false; }
					int aVersion = br.ReadInt32();
					if (aVersion > 2) { Console.WriteLine($"[Mesh] {thePath}: bad version {aVersion}"); return false; }

					Cleanup();
					mListener?.MeshPreLoad(this);

					int anObjectCount = br.ReadInt16();
					for (int anObjIdx = 0; anObjIdx < anObjectCount; anObjIdx++)
					{
						string anObjectName = ReadAsciiString(br);
						int aSetCount = br.ReadInt16();
						int aSetIdx;
						for (aSetIdx = 0; aSetIdx < aSetCount; aSetIdx++)
						{
							if (aVersion > 1)
							{
								byte aFlags = br.ReadByte();
								if (aFlags == 0) continue;
							}
							MeshPiece aPiece = new MeshPiece();
							mPieces.Add(aPiece);
							string aSetName = ReadAsciiString(br);
							string aTexFileName = null;
							string aBumpFileName = null;
							aPiece.mObjectName = anObjectName;
							aPiece.mSetName = aSetName;
							int aPropCount = br.ReadInt16();
							for (int i = 0; i < aPropCount; i++)
							{
								string aPropName = ReadAsciiString(br);
								string aPropValue = ReadAsciiString(br);
								mListener?.MeshHandleProperty(this, anObjectName, aSetName, aPropName, aPropValue);
								if (aPropName == "texture0.fileName") aTexFileName = aPropValue;
								if (aPropName == "bump.fileName") aBumpFileName = aPropValue;
							}
							if (!string.IsNullOrEmpty(aTexFileName))
							{
								if (mListener != null)
									aPiece.mTexture = mListener.MeshLoadTex(this, anObjectName, aSetName, "texture0.fileName", aTexFileName);
								else
									aPiece.mTexture = ResolveTextureRef(aTexFileName);
							}
							if (!string.IsNullOrEmpty(aBumpFileName))
							{
								if (mListener != null)
									aPiece.mBumpTexture = mListener.MeshLoadTex(this, anObjectName, aSetName, "bump.fileName", aBumpFileName);
								else
									aPiece.mBumpTexture = ResolveTextureRef(aBumpFileName);
							}
							short aType = br.ReadInt16();
							int aFVF = br.ReadInt32();
							int aVertexSize = 4 * (3 + 3 + 2) + 4; // pos3 + normal3 + uv2 + diffuseColor4 = 36
							aPiece.mSexyVF = aFVF;
							aPiece.mVertexSize = aVertexSize;
							aPiece.mVertexBufferCount = br.ReadInt16();
							aPiece.mVertexData = br.ReadBytes(aPiece.mVertexBufferCount * aVertexSize);
							aPiece.mIndexBufferCount = br.ReadInt16() * 3;
							aPiece.mIndexData = br.ReadBytes(aPiece.mIndexBufferCount * 2);
						}
						if (aSetIdx < aSetCount) { Console.WriteLine($"[Mesh] {thePath}: aborted set loop at {aSetIdx}/{aSetCount}"); return false; }
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[Mesh] {thePath}: parse exception: {ex.GetType().Name}: {ex.Message}");
				return false;
			}
			return true;
		}
		private static string ReadAsciiString(BinaryReader br)
		{
			int len = br.ReadUInt16();
			if (len == 0) return string.Empty;
			byte[] data = br.ReadBytes(len);
			return System.Text.Encoding.ASCII.GetString(data);
		}

		private static SharedImageRef ResolveTextureRef(string theTexFileName)
		{
			string fname = theTexFileName.Replace('\\', '/');
			int dot = fname.IndexOf('.');
			if (dot >= 0) fname = fname.Substring(0, dot);
			int slash = fname.LastIndexOf('/');
			if (slash >= 0) fname = fname.Substring(slash + 1);
			bool isNew = false;
			SharedImageRef r = GlobalMembers.gSexyAppBase.GetSharedImage("images/960/tex/" + fname, "", ref isNew, true, false);
			return r;
		}
	}
}
