using System;
using System.Collections.Generic;

namespace SexyFramework.Graphics
{
	public class PIEmitterBase : IDisposable
	{
		public List<PIParticleDefInstance> mParticleDefInstanceVector = new List<PIParticleDefInstance>();

		public PIParticleGroup mParticleGroup = new PIParticleGroup();

		public virtual void Dispose()
		{
			mParticleGroup = null;
			mParticleDefInstanceVector.Clear();
		}
	}
}
