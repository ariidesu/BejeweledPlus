using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework.Content;
using ManagedBass;
using SexyFramework.Resource;

namespace SexyFramework
{
    public class BassMusicInfo
    {
        public int mHMusic;
        public int mHStream;
        public double mVolume;
        public double mVolumeAdd;
        public double mVolumeCap;
        public bool mStopOnFade;

        public int GetHandle() => mHMusic != 0 ? mHMusic : mHStream;
    }

    public class BassMusicInterface : MusicInterface
    {
        public Dictionary<int, BassMusicInfo> mMusicMap;
        public int mMaxMusicVolume;
        public BassFlags mMusicLoadFlags;
        private double _globalVolume;

        public BassMusicInterface()
        {
            Bass.Init();
            Bass.Configure(Configuration.PlaybackBufferLength, 2000);
            Bass.Start();

            mMaxMusicVolume = 40;
            mMusicLoadFlags = BassFlags.Loop | BassFlags.MusicRamp | BassFlags.Prescan;
            mMusicMap = new Dictionary<int, BassMusicInfo>();
        }

        public static bool Bass_MusicPlayEx(int handle, int pos, BassFlags flags, bool reset, long startPos = 0)
        {
            long offset;
            PositionFlags mode;

            if (startPos != 0)
            {
                if ((startPos & 0x80000000L) == 0)
                {
                    offset = startPos & 0x7FFFFFFFL;
                    mode = PositionFlags.MusicOrders;
                }
                else
                {
                    offset = startPos;
                    mode = 0;
                }
            }
            else
            {
                offset = pos;
                mode = PositionFlags.MusicOrders;
            }

            Bass.ChannelStop(handle);
            Bass.ChannelFlags(handle, flags, unchecked((BassFlags)(-1)));
            if (!Bass.ChannelSetPosition(handle, offset, mode))
                Bass.ChannelSetPosition(handle, 0, mode);

            return Bass.ChannelPlay(handle, false);
        }

        public static bool Bass_StreamPlay(int handle, bool flush, BassFlags flags)
        {
            Bass.ChannelFlags(handle, flags, unchecked((BassFlags)(-1)));
            return Bass.ChannelPlay(handle, flush);
        }

        public static bool Bass_ChannelSetPosition(int theHandle, int thePos, PositionFlags theFlags)
        {
            if (thePos >= 0)
                return Bass.ChannelSetPosition(theHandle, thePos, theFlags);
            else
                return Bass.ChannelSetPosition(theHandle, thePos & 0x7FFFFFFF, theFlags);
        }

        public static double Bass_MusicGetBPM(int theHandle)
        {
            return Bass.ChannelGetAttribute(theHandle, ChannelAttribute.MusicBPM);
        }

        public static bool Bass_MusicSetBPM(int theHandle, double theBpm)
        {
            return Bass.ChannelSetAttribute(theHandle, ChannelAttribute.MusicBPM, theBpm);
        }
        
        public static double Bass_MusicGetChannelVolumeFloat(int theHandle, int theChannel)
        {
            double aResult = Bass.ChannelGetAttribute(theHandle, ChannelAttribute.MusicVolumeChannel + theChannel);
            if (Bass.LastError == Errors.OK)
                return aResult;
            else
                return -1.0;
        }
        
        public static int Bass_MusicGetChannelVolumeInt(int theHandle, int theChannel)
        {
            double aVolume = Bass_MusicGetChannelVolumeFloat(theHandle, theChannel);
            if (aVolume >= 0.0)
                return (int)(aVolume * 100.0);
            else
                return -1;
        }
        
        public static bool Bass_MusicSetChannelVolumeFloat(int theHandle, int theChannel, double theVolume)
        {
            return Bass.ChannelSetAttribute(theHandle, ChannelAttribute.MusicVolumeChannel + theChannel, theVolume);
        }
        
        public static bool Bass_MusicSetChannelVolumeInt(int theHandle, int theChannel, int theVolume)
        {
            double aVolume = theVolume / 100.0;
            return Bass_MusicSetChannelVolumeFloat(theHandle, theChannel, aVolume);
        }

        public static bool Bass_MusicPlay(int theHandle)
        {
            return Bass.ChannelPlay(theHandle, true);
        }

        public static bool Bass_ChannelResume(int theHandle)
        {
            return Bass.ChannelPlay(theHandle, false);
        }

        public static bool Bass_MusicSetAmplify(int theHandle, int value)
        {
            return Bass.ChannelSetAttribute(theHandle, ChannelAttribute.MusicAmplify, value);
        }

        public static double Bass_MusicGetAmplify(int theHandle)
        {
            return Bass.ChannelGetAttribute(theHandle, ChannelAttribute.MusicAmplify);
        }

        public static bool Bass_MusicSetPanSep(int theHandle, int value)
        {
            return Bass.ChannelSetAttribute(theHandle, ChannelAttribute.MusicPanSeparation, value);
        }

        public static double Bass_MusicGetPanSep(int theHandle)
        {
            return Bass.ChannelGetAttribute(theHandle, ChannelAttribute.MusicPanSeparation);
        }

        public static bool Bass_MusicSetPScaler(int theHandle, int value)
        {
            return Bass.ChannelSetAttribute(theHandle, ChannelAttribute.MusicPositionScaler, value);
        }

        public static double Bass_MusicGetPScaler(int theHandle)
        {
            return Bass.ChannelGetAttribute(theHandle, ChannelAttribute.MusicPositionScaler);
        }

        public static bool Bass_MusicSetSpeed(int theHandle, int value)
        {
            return Bass.ChannelSetAttribute(theHandle, ChannelAttribute.MusicSpeed, value);
        }

        public static double Bass_MusicGetSpeed(int theHandle)
        {
            return Bass.ChannelGetAttribute(theHandle, ChannelAttribute.MusicSpeed);
        }

        public static bool Bass_MusicSetGlobalVolume(int theHandle, int value)
        {
            return Bass.ChannelSetAttribute(theHandle, ChannelAttribute.MusicVolumeGlobal, value);
        }

        public static double Bass_MusicGetGlobalVolume(int theHandle)
        {
            return Bass.ChannelGetAttribute(theHandle, ChannelAttribute.MusicVolumeGlobal);
        }

        public static bool Bass_MusicSetInstrumentVolumeFloat(int theHandle, int inst, double value)
        {
            return Bass.ChannelSetAttribute(theHandle, ChannelAttribute.MusicVolumeInstrument + inst, value);
        }

        public static double Bass_MusicGetInstrumentVolumeFloat(int theHandle, int inst)
        {
            return Bass.ChannelGetAttribute(theHandle, ChannelAttribute.MusicVolumeInstrument + inst);
        }

        public static bool Bass_MusicSetInstrumentVolumeInt(int theHandle, int inst, int value)
        {
            return Bass_MusicSetInstrumentVolumeFloat(theHandle, inst, value / 100.0);
        }

        public static int Bass_MusicGetInstrumentVolumeInt(int theHandle, int inst)
        {
            return (int)(Bass_MusicGetInstrumentVolumeFloat(theHandle, inst) * 100.0);
        }

        public static long Bass_ChannelGetPosition(int theHandle)
        {
            return Bass.ChannelGetPosition(theHandle, PositionFlags.Bytes);
        }

        public static long Bass_ChannelGetLength(int theHandle)
        {
            return Bass.ChannelGetLength(theHandle, PositionFlags.Bytes);
        }

        public static int Bass_MusicGetOrders(int theHandle)
        {
            return (int)Bass.ChannelGetLength(theHandle, PositionFlags.MusicOrders);
        }

        public static int Bass_MusicGetOrderPosition(int theHandle)
        {
            return (int)Bass.ChannelGetPosition(theHandle, PositionFlags.MusicOrders);
        }

        public static bool Bass_ChannelSetAttributes(int theHandle, int theVolume)
        {
            if (theVolume >= 0)
            {
                return Bass.ChannelSetAttribute(theHandle, ChannelAttribute.Volume, theVolume);
            }

            return true;
        }
        
        public void Dispose()
        {
            Bass.Stop();
            Bass.Free();
        }

        public override bool LoadMusic(int theSongId, string theFileName, ContentManager content)
        {
            var pFile = new PFILE(theFileName, "");
            if (!pFile.Open())
            {
                Console.WriteLine("LoadMusic Error: Failed to open file");
                return false;
            }
            
            var data = pFile.GetData();
            int music = 0, stream = 0;
            var ext = System.IO.Path.GetExtension(theFileName).ToLowerInvariant();
            if (ext == ".wav" || ext == ".ogg" || ext == ".mp3")
            {
                stream = Bass.CreateStream(data, 0, data.Length, BassFlags.Default);
            }
            else
            {
                music = Bass.MusicLoad(data, 0, data.Length, mMusicLoadFlags, 44100);
            }

            if (music == 0 && stream == 0)
            {
                Console.WriteLine($"LoadMusic Error: {Bass.LastError}");
                return false;
            }

            var info = new BassMusicInfo
            {
                mHMusic   = music,
                mHStream  = stream,
                mVolume        = 1.0,
                mVolumeAdd     = 0.0,
                mVolumeCap     = 1.0,
                mStopOnFade    = false
            };
            mMusicMap[theSongId] = info;
            return true;
        }

        public override void PlayMusic(int theSongId, int theOffset, bool noLoop, long theStartPos)
        {
            if (!mMusicMap.TryGetValue(theSongId, out var info)) return;
            info.mVolume = info.mVolumeCap;
            info.mVolumeAdd = 0.0;
            info.mStopOnFade = noLoop;
            var handle = info.GetHandle();

            Bass_ChannelSetAttributes(handle, (int)(info.mVolume * 100));
            Bass.ChannelStop(handle);

            if (info.mHMusic != 0)
            {
                Bass_MusicPlayEx(handle, theOffset, BassFlags.MusicPositionResetEx | BassFlags.MusicRamp | (noLoop ? 0 : BassFlags.Loop), true, theStartPos);
            }
            else
            {
                Bass_StreamPlay(handle, theOffset != -1, noLoop ? 0 : BassFlags.Loop);
                if (theOffset > 0)
                    Bass.ChannelSetPosition(handle, theOffset);
            }
        }

        public override void StopMusic(int theSongId)
        {
            if (mMusicMap.TryGetValue(theSongId, out var info))
                Bass.ChannelStop(info.GetHandle());
        }

        public override void StopAllMusic()
        {
            foreach (var info in mMusicMap.Values)
            {
                info.mVolume = 0.0;
                Bass.ChannelStop(info.GetHandle());
            }
        }

        public override void UnloadMusic(int theSongId)
        {
            StopMusic(theSongId);
            if (mMusicMap.TryGetValue(theSongId, out var info))
            {
                if (info.mHStream != 0)
                    Bass.StreamFree(info.mHStream);
                else if (info.mHMusic != 0)
                    Bass.MusicFree(info.mHMusic);

                mMusicMap.Remove(theSongId);
            }
        }

        public override void UnloadAllMusic()
        {
            StopAllMusic();
            foreach (var info in mMusicMap.Values)
            {
                if (info.mHStream != 0)
                    Bass.StreamFree(info.mHStream);
                else if (info.mHMusic != 0)
                    Bass.MusicFree(info.mHMusic);
            }
            mMusicMap.Clear();
        }

        public override void PauseMusic(int theSongId)
        {
            if (mMusicMap.TryGetValue(theSongId, out var info))
                Bass.ChannelPause(info.GetHandle());
        }

        public override void PauseAllMusic()
        {
            foreach (var info in mMusicMap.Values)
            {
                var h = info.GetHandle();
                if (Bass.ChannelIsActive(h) == PlaybackState.Playing)
                    Bass.ChannelPause(h);
            }
        }

        public override void ResumeMusic(int theSongId)
        {
            if (mMusicMap.TryGetValue(theSongId, out var info))
                Bass.ChannelPlay(info.GetHandle(), false);
        }

        public override void ResumeAllMusic()
        {
            foreach (var info in mMusicMap.Values)
            {
                var h = info.GetHandle();
                if (Bass.ChannelIsActive(h) == PlaybackState.Paused)
                    Bass.ChannelPlay(h, false);
            }
        }

        public override void FadeIn(int theSongId, int theOffset, double theSpeed, bool noLoop)
        {
            if (!mMusicMap.TryGetValue(theSongId, out var info)) return;
            info.mVolumeAdd  = theSpeed;
            info.mStopOnFade = noLoop;
            var handle      = info.GetHandle();

            Bass.ChannelStop(handle);
            Bass_ChannelSetAttributes(handle, (int)(info.mVolume * 100));

            if (info.mHMusic != 0)
            {
                if (theOffset == -1)
                    Bass.ChannelPlay(info.mHMusic, true);
                else
                    Bass_MusicPlayEx(info.mHMusic, theOffset, BassFlags.MusicRamp | (noLoop ? 0 : BassFlags.Loop), true);
            }
            else
            {
                Bass_StreamPlay(handle, theOffset != -1, noLoop ? 0 : BassFlags.Loop);
                if (theOffset > 0)
                    Bass_ChannelSetPosition(handle, theOffset, (PositionFlags)(theOffset >> 31));
            }
        }

        public override void FadeOut(int theSongId, bool stopSong, double theSpeed)
        {
            if (!mMusicMap.TryGetValue(theSongId, out var info)) return;
            if (info.mVolume != 0.0)
                info.mVolumeAdd = -theSpeed;
            info.mStopOnFade = stopSong;
        }

        public override void FadeOutAll(bool stopSong, double theSpeed)
        {
            foreach (var info in mMusicMap.Values)
            {
                info.mVolumeAdd  = -theSpeed;
                info.mStopOnFade = stopSong;
            }
        }

        public override void SetVolume(double theVolume)
        {
            Bass.Configure(Configuration.GlobalMusicVolume, (int)(theVolume * 100));
            Bass.Configure(Configuration.GlobalStreamVolume, (int)(theVolume * 100));
        }

        public override void SetSongVolume(int theSongId, double theVolume)
        {
            if (mMusicMap.TryGetValue(theSongId, out var info))
            {
                info.mVolume = theVolume;
                Bass_ChannelSetAttributes(info.GetHandle(), (int)(info.mVolume * 100));
            }
        }

        public override void SetSongMaxVolume(int theSongId, double theMaxVolume)
        {
            if (mMusicMap.TryGetValue(theSongId, out var info))
            {
                info.mVolumeCap = theMaxVolume;
                info.mVolume    = Math.Min(info.mVolume, theMaxVolume);
                Bass_ChannelSetAttributes(info.GetHandle(), (int)(info.mVolume * 100));
            }
        }

        public override bool IsPlaying(int theSongId)
        {
            if (mMusicMap.TryGetValue(theSongId, out var info))
                return Bass.ChannelIsActive(info.GetHandle()) == PlaybackState.Playing;
            return false;
        }

        public override void SetMusicAmplify(int theSongId, double theAmp)
        {
            
            if (mMusicMap.TryGetValue(theSongId, out var info))
                Bass.ChannelSetAttribute(info.GetHandle(), ChannelAttribute.MusicAmplify, (int)(theAmp * 100));
        }

        public override void Update()
        {
            foreach (var info in mMusicMap.Values)
            {
                if (info.mVolumeAdd != 0.0)
                {
                    info.mVolume += info.mVolumeAdd;
                    if (info.mVolume > info.mVolumeCap)
                    {
                        info.mVolume    = info.mVolumeCap;
                        info.mVolumeAdd = 0.0;
                    }
                    else if (info.mVolume < 0.0)
                    {
                        info.mVolume    = 0.0;
                        info.mVolumeAdd = 0.0;
                        if (info.mStopOnFade)
                            Bass.ChannelStop(info.GetHandle());
                    }
                    Bass_ChannelSetAttributes(info.GetHandle(), (int)(info.mVolume * 100));
                }
            }
        }

        public override int GetMusicOrder(int theSongId)
        {
            if (mMusicMap.TryGetValue(theSongId, out var info))
                return Bass_MusicGetOrderPosition(info.GetHandle());
            return -1;
        }
    }
}
