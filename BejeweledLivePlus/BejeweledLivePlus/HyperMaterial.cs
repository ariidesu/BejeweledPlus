namespace BejeweledLivePlus
{
	public class HyperMaterial
	{
		public float[] ambient;

		public float[] diffuse;

		public float[] specular;

		public float power;

		public HyperMaterial(float[] inAmbient, float[] inDiffuse, float[] inSpecular, float inPower)
		{
			ambient = inAmbient;
			diffuse = inDiffuse;
			specular = inSpecular;
			power = inPower;
		}
	}
}
