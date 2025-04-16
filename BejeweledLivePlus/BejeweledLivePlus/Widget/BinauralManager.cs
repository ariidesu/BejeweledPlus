using System.Collections.Generic;
using System.Globalization;
using SexyFramework.Misc;
using SexyFramework.Resource;

namespace BejeweledLivePlus.Widget
{
	public class BinauralManager
	{
		public Dictionary<string, BinauralToneSet> mToneSetMap = new Dictionary<string, BinauralToneSet>();

		public List<BinauralTimeSequence> mTimeSequenceVector = new List<BinauralTimeSequence>();

		public int mCurSeqIdx;

		public List<Channel> mChannels = new List<Channel>();

		public double mCurTime;

		public string mDesc;

		public BinauralManager()
		{
			Reset();
		}

		public void Reset()
		{
			mCurTime = 0.0;
			mCurSeqIdx = -1;
			mToneSetMap.Clear();
			mTimeSequenceVector.Clear();
		}

		private static bool IsFloatPart(char ch, bool acceptSign)
		{
			string text = "0123456789.";
			if (acceptSign)
			{
				text += "+-";
			}
			return text.IndexOf(ch) > 0;
		}

		private static bool ExtractParameters(string str, out double carrier, out char sign, out double freq, out double amp)
		{
			char c = '\0';
			bool flag = false;
			bool acceptSign = true;
			bool result = false;
			carrier = 0.0;
			sign = '\0';
			freq = 0.0;
			amp = 0.0;
			foreach (char c2 in str)
			{
				if (!IsFloatPart(c2, acceptSign))
				{
					c = c2;
					flag = true;
					break;
				}
				acceptSign = false;
			}
			if (flag)
			{
				string[] array = str.Split(c, '/');
				if (array.Length == 3 && double.TryParse(array[0], NumberStyles.Any, CultureInfo.InvariantCulture, out carrier) && double.TryParse(array[1], NumberStyles.Any, CultureInfo.InvariantCulture, out freq) && double.TryParse(array[2], NumberStyles.Any, CultureInfo.InvariantCulture, out amp))
				{
					sign = c;
					result = true;
				}
			}
			return result;
		}

		private static int ExtractParameters(string str, out int hour, out int min, out int sec, out string cmd, out string addParam)
		{
			int num = 0;
			hour = 0;
			min = 0;
			sec = 0;
			cmd = string.Empty;
			addParam = string.Empty;
			string[] array = str.Split(' ', ':');
			if (array.Length >= 4 && int.TryParse(array[0], out hour))
			{
				num++;
				if (int.TryParse(array[1], out min))
				{
					num++;
					if (int.TryParse(array[2], out sec))
					{
						num++;
						cmd = array[3];
						num++;
						if (array.Length > 4)
						{
							addParam = array[4];
							num++;
						}
					}
				}
			}
			return num;
		}

		public bool Load(string theFileName)
		{
			Reset();
			Buffer theBuffer = null;
			GlobalMembers.gApp.ReadBufferFromFile(theFileName, theBuffer);
			int num = 0;
			int num2 = 0;
			EncodingParser encodingParser = new EncodingParser();
			if (encodingParser.OpenFile(theFileName))
			{
				string text = null;
				char theChar = '\0';
				while (encodingParser.GetChar(ref theChar) == EncodingParser.GetCharReturnType.SUCCESSFUL)
				{
					if (theChar == '\n' || theChar == '\r')
					{
						text = text.Trim();
						if (text.Substring(0, 3) == "NOW")
						{
							num = 1;
						}
						if (text.Substring(0, 1) == "#")
						{
							if (text.Substring(0, 6).ToUpper() == "#DESC:")
							{
								mDesc = text.Substring(6).Trim();
							}
							continue;
						}
						switch (num)
						{
						case 0:
						{
							int num3 = text.IndexOf(':');
							if (num3 == -1)
							{
								break;
							}
							string key = text.Substring(0, num3);
							text = text.Substring(num3 + 1).Trim();
							BinauralToneSet binauralToneSet = new BinauralToneSet();
							double carrier = 0.0;
							char sign = '\0';
							double freq = 0.0;
							double amp = 0.0;
							if (ExtractParameters(text, out carrier, out sign, out freq, out amp))
							{
								BinauralToneDef binauralToneDef = new BinauralToneDef();
								binauralToneDef.mCarrier = carrier;
								binauralToneDef.mFreq = freq;
								if (sign == '-')
								{
									binauralToneDef.mFreq = 0.0 - binauralToneDef.mFreq;
								}
								binauralToneDef.mVolume = amp / 100.0;
								binauralToneSet.mToneDefVector.Add(binauralToneDef);
							}
							num2 = GlobalMembers.MAX(num2, binauralToneSet.mToneDefVector.Count);
							mToneSetMap[key] = binauralToneSet;
							break;
						}
						case 1:
						{
							BinauralTimeSequence binauralTimeSequence = new BinauralTimeSequence();
							int hour = 0;
							int min = 0;
							int sec = 0;
							string cmd = string.Empty;
							string addParam = string.Empty;
							if (text.Substring(0, 3) == "NOW")
							{
								text = text.Substring(3).Trim();
							}
							else if (4 >= ExtractParameters(text, out hour, out min, out sec, out cmd, out addParam))
							{
								text = cmd.Trim();
							}
							if (string.Compare(addParam, ".") == 0)
							{
								binauralTimeSequence.mCrossfade = true;
							}
							else
							{
								binauralTimeSequence.mCrossfade = false;
							}
							binauralTimeSequence.mTime = hour * 60 * 60 + min * 60 + sec;
							binauralTimeSequence.mToneSet = mToneSetMap[text];
							mTimeSequenceVector.Add(binauralTimeSequence);
							break;
						}
						}
					}
					else
					{
						text += theChar;
					}
				}
			}
			if (mTimeSequenceVector.Count > 0 && !mTimeSequenceVector[0].mCrossfade)
			{
				mTimeSequenceVector[0].mTime = 2;
				BinauralTimeSequence binauralTimeSequence2 = new BinauralTimeSequence();
				binauralTimeSequence2.mTime = 0;
				binauralTimeSequence2.mCrossfade = true;
				binauralTimeSequence2.mToneSet = mToneSetMap["alloff"];
				mTimeSequenceVector.Insert(0, binauralTimeSequence2);
			}
			while (num2 > mChannels.Count)
			{
				bool flag = true;
				Channel channel = null;
				channel.mLeftInstance = GlobalMembers.gApp.mSoundManager.GetSoundInstance(GlobalMembersResourcesWP.SOUND_SIN500);
				if (channel.mLeftInstance != null)
				{
					channel.mLeftInstance.SetMasterVolumeIdx(3);
					channel.mLeftInstance.SetVolume(0.0);
					channel.mLeftInstance.SetPan(-10000);
					channel.mLeftInstance.Play(true, false);
					channel.mRightInstance = GlobalMembers.gApp.mSoundManager.GetSoundInstance(GlobalMembersResourcesWP.SOUND_SIN500);
					if (channel.mRightInstance != null)
					{
						channel.mRightInstance.SetMasterVolumeIdx(3);
						channel.mRightInstance.SetVolume(0.0);
						channel.mRightInstance.SetPan(10000);
						channel.mRightInstance.Play(true, false);
						mChannels.Add(channel);
						flag = false;
					}
					else
					{
						channel.mLeftInstance.Release();
					}
				}
				if (flag)
				{
					break;
				}
			}
			return true;
		}

		public void Update()
		{
			mCurTime += 0.01;
			if (mTimeSequenceVector.Count > 0)
			{
				if (mCurSeqIdx == -1)
				{
					mCurSeqIdx = 0;
				}
				BinauralTimeSequence binauralTimeSequence = mTimeSequenceVector[mCurSeqIdx];
				while (mCurSeqIdx < mTimeSequenceVector.Count - 1)
				{
					BinauralTimeSequence binauralTimeSequence2 = mTimeSequenceVector[mCurSeqIdx + 1];
					if (mCurTime < (double)binauralTimeSequence2.mTime)
					{
						break;
					}
					mCurSeqIdx++;
					binauralTimeSequence = mTimeSequenceVector[mCurSeqIdx];
				}
				if (!binauralTimeSequence.mCrossfade)
				{
					return;
				}
				BinauralTimeSequence binauralTimeSequence3 = mTimeSequenceVector[mCurSeqIdx + 1];
				float num = (float)((mCurTime - (double)binauralTimeSequence.mTime) / (double)(binauralTimeSequence3.mTime - binauralTimeSequence.mTime));
				int num2 = GlobalMembers.MAX(binauralTimeSequence.mToneSet.mToneDefVector.Count, binauralTimeSequence3.mToneSet.mToneDefVector.Count);
				for (int i = 0; i < num2; i++)
				{
					double num3 = 0.0;
					double num4 = 0.0;
					double num5 = 0.0;
					BinauralToneDef binauralToneDef = null;
					if (i < binauralTimeSequence.mToneSet.mToneDefVector.Count)
					{
						binauralToneDef = binauralTimeSequence.mToneSet.mToneDefVector[i];
						num5 = binauralToneDef.mVolume;
						num3 = binauralToneDef.mCarrier;
						num4 = binauralToneDef.mFreq;
					}
					double num6 = num3;
					double num7 = num4;
					double num8 = 0.0;
					if (i < binauralTimeSequence3.mToneSet.mToneDefVector.Count)
					{
						BinauralToneDef binauralToneDef2 = binauralTimeSequence3.mToneSet.mToneDefVector[i];
						num6 = binauralToneDef2.mCarrier;
						num7 = binauralToneDef2.mFreq;
						num8 = binauralToneDef2.mVolume;
						if (binauralToneDef == null)
						{
							num3 = num6;
							num4 = num7;
						}
					}
					double num9 = num3 * (1.0 - (double)num) + num6 * (double)num;
					double num10 = num4 * (1.0 - (double)num) + num7 * (double)num;
					double volume = num5 * (1.0 - (double)num) + num8 * (double)num;
					if (mChannels.Count > i)
					{
						Channel channel = mChannels[i];
						channel.mLeftInstance.SetBaseRate(num9 / 500.0);
						channel.mRightInstance.SetBaseRate((num9 + num10) / 500.0);
						if ((int)GlobalMembers.gApp.mUpdateCount % 25 == 0)
						{
							channel.mLeftInstance.AdjustPitch(0.0);
							channel.mLeftInstance.SetVolume(volume);
							channel.mRightInstance.AdjustPitch(0.0);
							channel.mRightInstance.SetVolume(volume);
						}
					}
				}
				return;
			}
			for (int j = 0; j < mChannels.Count; j++)
			{
				Channel channel2 = mChannels[j];
				if (channel2.mLeftInstance.GetVolume() > 0.0)
				{
					channel2.mLeftInstance.SetVolume(channel2.mLeftInstance.GetVolume() - (double)GlobalMembers.M(0.01f));
				}
				if (channel2.mRightInstance.GetVolume() > 0.0)
				{
					channel2.mRightInstance.SetVolume(channel2.mRightInstance.GetVolume() - (double)GlobalMembers.M(0.01f));
				}
			}
		}
	}
}
