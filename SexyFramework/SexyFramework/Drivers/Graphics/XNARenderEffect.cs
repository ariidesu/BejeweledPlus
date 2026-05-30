using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SexyFramework.Graphics;
using SexyFramework.Drivers.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SexyFramework.Drivers.Graphics
{
    [Flags]
    public enum XNAMatrixSemanticFlags
    {
        None = 0,
        World = 1,
        View = 2,
        Projection = 4,
        Transpose = 8
    }

    public struct XNAMatrixSemanticBinding
    {
        public EffectParameter Param;
        public XNAMatrixSemanticFlags Flags;

        public XNAMatrixSemanticBinding(EffectParameter param, XNAMatrixSemanticFlags flags)
        {
            Param = param;
            Flags = flags;
        }
    }

    public class XNARenderEffect : RenderEffect
    {
        public RenderEffectDefinition mDefinition;
        public BaseXNARenderDevice mRenderDevice;

        public int mCurrentPass = -1;
        public Dictionary<string, EffectParameter> mParams = new Dictionary<string, EffectParameter>();
        public Dictionary<string, EffectTechnique> mTechniques = new Dictionary<string, EffectTechnique>();

        public EffectParameter mParamWVP;
        public EffectParameter mParamWorld;
        public EffectParameter mParamView;
        public EffectParameter mParamProjection;
        public EffectParameter mParamTex0;
        public EffectParameter mParamTex1;
        public EffectParameter mParamTex2;
        public List<XNAMatrixSemanticBinding> mMatrixSemantics = new List<XNAMatrixSemanticBinding>();
        private Dictionary<string, XNAMatrixSemanticFlags> mMatrixSemanticByName = new Dictionary<string, XNAMatrixSemanticFlags>();
        private static readonly Regex sMatrixSemanticRegex = new Regex(
            @"\b(?:(?:float|half|double)\d*x\d+|matrix)\s+(\w+)\s*:\s*([A-Za-z0-9_]+)\s*;",
            RegexOptions.Compiled);

        public XNARenderEffect(RenderEffectDefinition theDefinition, BaseXNARenderDevice theRenderDevice)
        {
            mDefinition = theDefinition;
            mRenderDevice = theRenderDevice;

            if (mDefinition.mEffect != null)
            {
                foreach (var param in mDefinition.mEffect.Parameters)
                    mParams[param.Name] = param;

                foreach (var tech in mDefinition.mEffect.Techniques)
                    mTechniques[tech.Name] = tech;

                if (mTechniques.TryGetValue("Default", out var defaultTech))
                {
                    mDefinition.mEffect.CurrentTechnique = defaultTech;
                }
                else if (mTechniques.Count > 0)
                {
                    mDefinition.mEffect.CurrentTechnique = mDefinition.mEffect.Techniques[0];
                }

                mParams.TryGetValue("worldViewProj", out mParamWVP);
                if (mParamWVP == null)
                {
                    mParams.TryGetValue("WorldViewProj", out mParamWVP);
                }
                mParams.TryGetValue("world", out mParamWorld);
                mParams.TryGetValue("view", out mParamView);
                mParams.TryGetValue("projection", out mParamProjection);
                mParams.TryGetValue("Tex0", out mParamTex0);
                mParams.TryGetValue("Tex1", out mParamTex1);
                mParams.TryGetValue("Tex2", out mParamTex2);

                LoadMatrixSemantics();
            }
        }

        private void LoadMatrixSemantics()
        {
            mMatrixSemantics.Clear();
            mMatrixSemanticByName.Clear();

            string source = DecodeEffectData(mDefinition.mData);
            if (!string.IsNullOrEmpty(source))
            {
                foreach (Match match in sMatrixSemanticRegex.Matches(source))
                {
                    string paramName = match.Groups[1].Value;
                    string semanticName = match.Groups[2].Value;
                    if (!mParams.TryGetValue(paramName, out var param))
                    {
                        continue;
                    }
                    if (!TryParseMatrixSemantic(semanticName, out var flags))
                    {
                        continue;
                    }

                    AddMatrixSemantic(paramName, param, flags);
                }
            }

            if (mMatrixSemantics.Count == 0)
            {
                AddFallbackMatrixSemantics();
            }
        }

        private void AddFallbackMatrixSemantics()
        {
            if (mParams.TryGetValue("world", out var world))
            {
                AddMatrixSemantic("world", world, XNAMatrixSemanticFlags.World | XNAMatrixSemanticFlags.Transpose);
            }
            if (mParams.TryGetValue("view", out var view))
            {
                AddMatrixSemantic("view", view, XNAMatrixSemanticFlags.View | XNAMatrixSemanticFlags.Transpose);
            }
            if (mParams.TryGetValue("projection", out var projection))
            {
                AddMatrixSemantic("projection", projection, XNAMatrixSemanticFlags.Projection | XNAMatrixSemanticFlags.Transpose);
            }
            if (mParams.TryGetValue("worldViewProj", out var wvp))
            {
                AddMatrixSemantic("worldViewProj", wvp, XNAMatrixSemanticFlags.World | XNAMatrixSemanticFlags.View | XNAMatrixSemanticFlags.Projection | XNAMatrixSemanticFlags.Transpose);
            }
            else if (mParams.TryGetValue("WorldViewProj", out wvp))
            {
                AddMatrixSemantic("WorldViewProj", wvp, XNAMatrixSemanticFlags.World | XNAMatrixSemanticFlags.View | XNAMatrixSemanticFlags.Projection);
            }
        }

        private void AddMatrixSemantic(string paramName, EffectParameter param, XNAMatrixSemanticFlags flags)
        {
            if (mMatrixSemanticByName.ContainsKey(paramName))
            {
                return;
            }

            mMatrixSemanticByName[paramName] = flags;
            mMatrixSemantics.Add(new XNAMatrixSemanticBinding(param, flags));
        }

        private static bool TryParseMatrixSemantic(string semanticName, out XNAMatrixSemanticFlags flags)
        {
            flags = XNAMatrixSemanticFlags.None;
            if (string.IsNullOrEmpty(semanticName))
            {
                return false;
            }

            string semantic = semanticName.ToUpperInvariant();
            bool transpose = false;
            if (semantic.EndsWith("_TRANSPOSE", StringComparison.Ordinal))
            {
                transpose = true;
                semantic = semantic.Substring(0, semantic.Length - "_TRANSPOSE".Length);
            }

            switch (semantic)
            {
                case "WORLD":
                    flags = XNAMatrixSemanticFlags.World;
                    break;
                case "VIEW":
                    flags = XNAMatrixSemanticFlags.View;
                    break;
                case "PROJ":
                case "PROJECTION":
                    flags = XNAMatrixSemanticFlags.Projection;
                    break;
                case "WORLDVIEW":
                    flags = XNAMatrixSemanticFlags.World | XNAMatrixSemanticFlags.View;
                    break;
                case "VIEWPROJ":
                case "VIEWPROJECTION":
                    flags = XNAMatrixSemanticFlags.View | XNAMatrixSemanticFlags.Projection;
                    break;
                case "WORLDVIEWPROJ":
                case "WORLDVIEWPROJECTION":
                    flags = XNAMatrixSemanticFlags.World | XNAMatrixSemanticFlags.View | XNAMatrixSemanticFlags.Projection;
                    break;
                default:
                    return false;
            }

            if (transpose)
            {
                flags |= XNAMatrixSemanticFlags.Transpose;
            }
            return true;
        }

        private static string DecodeEffectData(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return null;
            }

            int bytesToProbe = Math.Min(data.Length, 256);
            for (int i = 0; i < bytesToProbe; i++)
            {
                if (data[i] == 0)
                {
                    return null;
                }
            }

            try
            {
                string source = Encoding.UTF8.GetString(data);
                return source.IndexOf(':') >= 0 ? source : null;
            }
            catch
            {
                return null;
            }
        }

        public override RenderDevice3D GetDevice()
        {
            return mRenderDevice;
        }

        public override RenderEffectDefinition GetDefinition()
        {
            return mDefinition;
        }

        private Dictionary<string, float[]> mLastParamValues = new Dictionary<string, float[]>();

        private static bool FloatsEqual(float[] a, float[] b, uint count)
        {
            if (a == null || b == null || a.Length < count || b.Length < count) return false;
            for (uint i = 0; i < count; i++)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }

        public override void SetParameter(string inParamName, float[] inFloatData, uint inFloatCount)
        {
            if (mDefinition.mEffect == null)
            {
                return;
            }

            if (!mParams.TryGetValue(inParamName, out var param))
            {
                return;
            }

            if (mLastParamValues.TryGetValue(inParamName, out var prev) && FloatsEqual(prev, inFloatData, inFloatCount))
            {
                return;
            }

            if (mRenderDevice != null && mRenderDevice.mStateMgr != null)
            {
                var actives = mRenderDevice.mStateMgr.mActiveEffects;
                if (actives.Count > 0 && actives[actives.Count - 1].mDefinition.mEffect == mDefinition.mEffect)
                {
                    mRenderDevice.FlushBatchBeforeStateChange();
                }
            }

            switch (inFloatCount)
            {
                case 4:
                    param.SetValue(new Vector4(
                        inFloatData[0],
                        inFloatData[1],
                        inFloatData[2],
                        inFloatData[3]
                    ));
                    break;

                case 16:
                    Matrix matrix = new Matrix(
                        inFloatData[0], inFloatData[1], inFloatData[2], inFloatData[3],
                        inFloatData[4], inFloatData[5], inFloatData[6], inFloatData[7],
                        inFloatData[8], inFloatData[9], inFloatData[10], inFloatData[11],
                        inFloatData[12], inFloatData[13], inFloatData[14], inFloatData[15]
                    );
                    if (mMatrixSemanticByName.TryGetValue(inParamName, out var flags) && (flags & XNAMatrixSemanticFlags.Transpose) != 0)
                    {
                        param.SetValueTranspose(matrix);
                    }
                    else
                    {
                        param.SetValue(matrix);
                    }
                    break;

                default:
                    param.SetValue(inFloatData);
                    break;
            }

            float[] copy = new float[inFloatCount];
            for (uint i = 0; i < inFloatCount; i++) copy[i] = inFloatData[i];
            mLastParamValues[inParamName] = copy;
        }

        public override void SetParameter(string inParamName, float inFloatData)
        {
            if (mDefinition.mEffect == null)
            {
                System.Diagnostics.Debug.WriteLine("Attempting to set parameter on null effect");
                return;
            }
            if (mParams.TryGetValue(inParamName, out var param))
            {
                param.SetValue(inFloatData);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Parameter '" + inParamName + "' not found in effect " + mDefinition.mEffect.Name);
            }
        }

        public override void GetParameterBySemantic(uint inSemantic, float[] outFloatData, uint inMaxFloatCount)
        {
            for (int i = 0; i < inMaxFloatCount; i++)
            {
                outFloatData[i] = 0f;
            }

            if (mRenderDevice == null || mRenderDevice.mStateMgr == null || inMaxFloatCount == 0)
            {
                return;
            }

            XNAMatrixSemanticFlags flags = (XNAMatrixSemanticFlags)(inSemantic & 0xFu);
            if ((flags & (XNAMatrixSemanticFlags.World | XNAMatrixSemanticFlags.View | XNAMatrixSemanticFlags.Projection)) == 0)
            {
                return;
            }

            Matrix matrix = ComposeSemanticMatrix(
                flags,
                mRenderDevice.mStateMgr.mXNAWorldMatrix,
                mRenderDevice.mStateMgr.mXNAViewMatrix,
                mRenderDevice.mStateMgr.mXNAProjectionMatrix);
            CopyMatrix(matrix, (flags & XNAMatrixSemanticFlags.Transpose) != 0, outFloatData, inMaxFloatCount);
        }

        private static Matrix ComposeSemanticMatrix(XNAMatrixSemanticFlags flags, Matrix world, Matrix view, Matrix projection)
        {
            Matrix result = Matrix.Identity;
            bool hasMatrix = false;

            if ((flags & XNAMatrixSemanticFlags.World) != 0)
            {
                result = world;
                hasMatrix = true;
            }
            if ((flags & XNAMatrixSemanticFlags.View) != 0)
            {
                result = hasMatrix ? result * view : view;
                hasMatrix = true;
            }
            if ((flags & XNAMatrixSemanticFlags.Projection) != 0)
            {
                result = hasMatrix ? result * projection : projection;
                hasMatrix = true;
            }

            return hasMatrix ? result : Matrix.Identity;
        }

        private static void CopyMatrix(Matrix matrix, bool raw, float[] outFloatData, uint inMaxFloatCount)
        {
            float[] values;
            if (raw)
            {
                values = new float[]
                {
                    matrix.M11, matrix.M12, matrix.M13, matrix.M14,
                    matrix.M21, matrix.M22, matrix.M23, matrix.M24,
                    matrix.M31, matrix.M32, matrix.M33, matrix.M34,
                    matrix.M41, matrix.M42, matrix.M43, matrix.M44
                };
            }
            else
            {
                values = new float[]
                {
                    matrix.M11, matrix.M21, matrix.M31, matrix.M41,
                    matrix.M12, matrix.M22, matrix.M32, matrix.M42,
                    matrix.M13, matrix.M23, matrix.M33, matrix.M43,
                    matrix.M14, matrix.M24, matrix.M34, matrix.M44
                };
            }

            int count = Math.Min(values.Length, (int)inMaxFloatCount);
            for (int i = 0; i < count; i++)
            {
                outFloatData[i] = values[i];
            }
        }

        public override void SetCurrentTechnique(string inName, bool inCheckValid)
        {
            if (mTechniques.TryGetValue(inName, out var tech))
            {
                mDefinition.mEffect.CurrentTechnique = tech;
            }
        }

        public override string GetCurrentTechniqueName()
        {
            return mDefinition.mEffect.CurrentTechnique?.Name ?? "";
        }

        public override int Begin(out object outRunHandle, HRenderContext inRenderContext)
        {
            outRunHandle = 0;
            mRenderDevice.mStateMgr.PushActiveEffect(this);
            return mDefinition.mEffect.CurrentTechnique?.Passes.Count ?? 0;
        }

        public override void BeginPass(object inRunHandle, int inPass)
        {
            mCurrentPass = inPass;
        }

        public bool MG_ApplyPass()
        {
            if (mCurrentPass == -1) return false;
            mDefinition.mEffect.CurrentTechnique?.Passes[mCurrentPass].Apply();
            return true;
        }

        public override void EndPass(object inRunHandle, int inPass)
        {
            mCurrentPass = -1;
        }

        public override void End(object inRunHandle)
        {
            mRenderDevice.mStateMgr.RemoveActiveEffect(this);
        }

        public override bool PassUsesVertexShader(int inPass)
        {
            return true;
        }

        public override bool PassUsesPixelShader(int inPass)
        {
            return true;
        }
    }
}
