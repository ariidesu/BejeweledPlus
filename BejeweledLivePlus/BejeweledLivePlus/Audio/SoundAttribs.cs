namespace BejeweledLivePlus.Audio
{
	public class SoundAttribs
	{
		public int pan { get; set; }

		public int delay { get; set; }

		public int stagger { get; set; }

		public float fadein { get; set; }

		public float fadeout { get; set; }

		public float pitch { get; set; }

		public float volume { get; set; }

		public SoundAttribs()
		{
			pan = 0;
			pitch = 0f;
			fadein = 1f;
			fadeout = 1f;
			delay = 0;
			stagger = 0;
			volume = 1f;
		}
	}
}
