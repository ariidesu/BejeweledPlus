// Based on BejBlitz's CustomBassMusicInterface

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ManagedBass;
using Microsoft.Xna.Framework.Content;
using SexyFramework.Drivers.App;
using SexyFramework.Resource;

namespace SexyFramework
{
    public class QueuedSongCommand
    {
        public string mSongName;
        public string mEventName;
        public bool mForceRestart;
    }

    public class SongEventInfo
    {
        public string mName;
        public string mVolumeData;
        public string mTempoData;
        public string mOffsetData;

        public List<int> mTracks = new();
        public List<int> mRowSync = new();
        public bool mMultVolume = false;

        public SongEventInfo()
        {
        }

        public SongEventInfo(SongEventInfo rhs)
        {
            mName = rhs.mName;
            mVolumeData = rhs.mVolumeData;
            mTempoData = rhs.mTempoData;
            mOffsetData = rhs.mOffsetData;
            mTracks = new List<int>(rhs.mTracks);
            mRowSync = new List<int>(rhs.mRowSync);
            mMultVolume = rhs.mMultVolume;
        }
    }

    public class TrackInfo
    {
        public float mOrigVolume = 1.0f;
        public CurvedVal mVolume = new();
    }

    public class SongInfo
    {
        public int mStartPos;
        public List<TrackInfo> mTracks = new();
        public int mMusicId;
        public string mName = "";
        public Dictionary<string, SongEventInfo> mSongEventInfoMap = new();
        public bool mLoop = true;
        public float mMainVolume = 1.0f;
        public CurvedVal mVolume = new();
        public int mOrigTempo = 0;
        public CurvedVal mTempo = new();

        public SongInfo()
        {
            mVolume.SetConstant(1f);
        }
    }

    public class CustomBassMusicInterface : BassMusicInterface
    {
        private GCHandle _mGcHandle;

        public Dictionary<string, int> mLoadedMusicFiles = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, SongInfo> mSongs = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, SongEventInfo> mDefaultEventsMap = new(StringComparer.OrdinalIgnoreCase);
        public List<QueuedSongCommand> mQueuedSongCommandVector = new();
        public string mSongName = "";
        public double mMusicVolumeScalar = 1.0;
        public bool mHasFailed = false;
        public bool mForceParamUpdate = false;
        public bool mCurCommandDone = true;
        public bool mStopAll = false;
        public XMLParser mXMLParser;

        public CustomBassMusicInterface()
        {
            _mGcHandle = GCHandle.Alloc(this);
        }

        private static void ProcessDataString(string dataString, ref CurvedVal theCurvedVal)
        {
            if (!string.IsNullOrEmpty(dataString))
            {
                char firstChar = dataString[0];
                if (firstChar < 'a' || firstChar > 'z')
                {
                    if (int.TryParse(dataString, out int val))
                    {
                        theCurvedVal.SetConstant((double)val);
                    }
                }
                else
                {
                    theCurvedVal.SetCurve(dataString);
                }
            }
        }

        public void QueueEvent(string theEventName, string theSongName, bool theForceRestart)
        {
            mQueuedSongCommandVector.Add(new QueuedSongCommand
            {
                mEventName = theEventName,
                mSongName = theSongName,
                mForceRestart = theForceRestart
            });
        }

        public override void Update()
        {
            base.Update();
            CheckQueue();

            if (mStopAll)
            {
                mStopAll = false;
                StopAllMusic();
            }
        }

        public override void StopAllMusic()
        {
            mSongName = string.Empty;
            base.StopAllMusic();
        }

        public void MusicSyncCallbackStub(int handle, int channel, int data, IntPtr user)
        {
            CustomBassMusicInterface aPtr = GCHandle.FromIntPtr(user).Target as CustomBassMusicInterface;
            aPtr.CheckQueue();
        }

        public void CheckQueue()
        {
            if (mCurCommandDone && mQueuedSongCommandVector.Count > 0)
            {
                QueuedSongCommand command = mQueuedSongCommandVector[0];
                mQueuedSongCommandVector.RemoveAt(0);
                bool changed = true;
                string newSongName = !string.IsNullOrEmpty(command.mSongName) ? command.mSongName : mSongName;
                bool restartSong = newSongName != mSongName || command.mForceRestart;
                mSongName = newSongName;

                SongInfo songInfo = FindSong(mSongName);
                if (songInfo != null)
                {
                    if (restartSong)
                    {
                        if (mMusicMap.TryGetValue(songInfo.mMusicId, out BassMusicInfo musicInfo))
                        {
                            for (int i = 0; i < songInfo.mTracks.Count; i++)
                            {
                                TrackInfo track = songInfo.mTracks[i];
                                track.mVolume.SetConstant(track.mOrigVolume);
                                Bass_MusicSetChannelVolumeInt(musicInfo.mHMusic, i, 0);
                            }

                            songInfo.mTempo.SetConstant(songInfo.mOrigTempo);
                            double tempo = songInfo.mTempo.GetOutVal();
                            Bass_MusicSetBPM(musicInfo.mHMusic, tempo);

                            int startPos = songInfo.mStartPos;
                            songInfo.mVolume.SetConstant(1.0);

                            double volume = Math.Min(songInfo.mVolume.GetOutVal() * songInfo.mMainVolume,
                                musicInfo.mVolumeCap);
                            musicInfo.mVolume = volume;
                            musicInfo.mVolumeAdd = 0.0;
                            musicInfo.mStopOnFade = !songInfo.mLoop;

                            int handle = musicInfo.mHMusic != 0 ? musicInfo.mHMusic : musicInfo.mHStream;
                            Bass_ChannelSetAttributes(handle, (int)(volume * 100.0));

                            if (musicInfo.mHMusic != 0)
                            {
                                Bass_MusicPlayEx(handle, startPos,
                                    (BassFlags)((songInfo.mLoop ? 4 : 0) | 0x400200), false);
                            }
                            else
                            {
                                Bass_StreamPlay(handle, startPos != -1,
                                    (BassFlags)(songInfo.mLoop ? 4 : 0));
                                if (startPos > 0)
                                {
                                    Bass_ChannelSetPosition(handle, startPos, (PositionFlags)
                                        (startPos >> 31));
                                }
                            }
                        }
                    }

                    string upperEventName = command.mEventName.ToUpper();

                    if (mDefaultEventsMap.TryGetValue(upperEventName, out SongEventInfo defaultEvent))
                    {
                        if (!HandleEvent(songInfo, defaultEvent))
                            changed = false;
                    }

                    if (songInfo.mSongEventInfoMap.TryGetValue(upperEventName, out SongEventInfo songEvent))
                    {
                        if (!HandleEvent(songInfo, songEvent))
                            changed = false;
                    }

                    mForceParamUpdate = true;
                }
                else
                {
                    mStopAll = true;
                }
            }
            
            mCurCommandDone = true;

            SongInfo curSong = FindSong(mSongName);
            if (curSong != null && mMusicMap.TryGetValue(curSong.mMusicId, out BassMusicInfo curMusic))
            {
                bool updateTempo = mForceParamUpdate ||
                                   (curSong.mTempo.mRamp == 6 && !curSong.mTempo.HasBeenTriggered());
                mCurCommandDone &= !curSong.mTempo.IncInVal();

                if (updateTempo)
                {
                    double tempo = curSong.mTempo.GetOutVal();
                    Bass_MusicSetBPM(curMusic.mHMusic, tempo);
                }

                bool updateVolume = mForceParamUpdate ||
                                    (curSong.mVolume.mRamp == 6 && !curSong.mVolume.HasBeenTriggered());
                mCurCommandDone &= !curSong.mVolume.IncInVal();

                if (updateVolume)
                {
                    double volume = Math.Min(curSong.mVolume.GetOutVal() * curSong.mMainVolume, curMusic.mVolumeCap);
                    curMusic.mVolume = volume;

                    int hStream = curMusic.mHMusic != 0 ? curMusic.mHMusic : curMusic.mHStream;
                    Bass_ChannelSetAttributes(hStream, (int)(volume * 100.0));
                }

                for (int i = 0; i < curSong.mTracks.Count; i++)
                {
                    TrackInfo track = curSong.mTracks[i];
                    bool updateTrack = mForceParamUpdate ||
                                       (track.mVolume.mRamp == 6 && !track.mVolume.HasBeenTriggered());
                    mCurCommandDone &= !track.mVolume.IncInVal();

                    if (updateTrack)
                    {
                        double volume = track.mVolume.GetOutVal() * 100.0;
                        Bass_MusicSetChannelVolumeInt(curMusic.mHMusic, i, (int)volume);
                    }
                }

                mForceParamUpdate = false;
            }
        }

        public bool HandleEvent(SongInfo theSongInfo, SongEventInfo theSongEventInfo)
        {
            if (theSongEventInfo.mRowSync != null && theSongEventInfo.mRowSync.Count > 0)
            {
                if (!mMusicMap.TryGetValue((int)mMusicLoadFlags, out var musicInfo))
                    return false;

                int currentRow = (int)(Bass.ChannelGetPosition(musicInfo.mHMusic, PositionFlags.MusicOrders) >> 16);
                bool matched = false;
                for (int i = 0; i < theSongEventInfo.mRowSync.Count; i++)
                {
                    if (theSongEventInfo.mRowSync[i] == currentRow)
                    {
                        matched = true;
                        break;
                    }
                }

                if (!matched)
                    return false;
            }

            ProcessDataString(theSongEventInfo.mTempoData, ref theSongInfo.mTempo);

            if (theSongEventInfo.mTracks.Count > 0)
            {
                foreach (int trackId in theSongEventInfo.mTracks)
                {
                    int theRealTrackId = trackId - 1;
                    if (theRealTrackId >= 0 && theRealTrackId < theSongInfo.mTracks.Count)
                        theSongInfo.mTracks[theRealTrackId].mVolume.SetConstant(theSongInfo.mTracks[theRealTrackId].mOrigVolume);
                }
            }
            else
            {
                ProcessDataString(theSongEventInfo.mVolumeData, ref theSongInfo.mVolume);
            }

            foreach (int trackId in theSongEventInfo.mTracks)
            {
                if (!mMusicMap.TryGetValue(theSongInfo.mMusicId, out var music))
                    continue;

                double prevVolume = music.mVolume;
                int theRealTrackId = trackId - 1;
                if (theRealTrackId >= 0 && theRealTrackId < theSongInfo.mTracks.Count)
                    ProcessDataString(theSongEventInfo.mVolumeData, ref theSongInfo.mTracks[theRealTrackId].mVolume);

                if (theSongEventInfo.mMultVolume)
                {
                    music.mVolume = prevVolume;
                }
            }

            if (!string.IsNullOrEmpty(theSongEventInfo.mOffsetData))
            {
                if (mMusicMap.TryGetValue(theSongInfo.mMusicId, out var music))
                {
                    if (int.TryParse(theSongEventInfo.mOffsetData, out int offset))
                    {
                        Bass.ChannelSetPosition(music.mHMusic, BitHelper.MakeLong((short)offset, 0), PositionFlags.MusicOrders);
                    }
                }
            }

            return true;
        }

        public SongInfo FindSong(string theSongName)
        {
            mSongs.TryGetValue(theSongName.ToUpper(), out var songInfo);
            return songInfo;
        }

        public bool ReadMusicXML()
        {
            mXMLParser = new XMLParser();

            if (!mXMLParser.OpenFile("properties/music.xml"))
                return false;

            XMLElement aElement = new XMLElement();
            if (!mXMLParser.NextElement(aElement))
                return false;

            while (aElement.mType == XMLElement.XMLElementType.TYPE_START)
            {
                if (!aElement.mValue.Equals("Music"))
                {
                    if (!mHasFailed)
                    {
                        mHasFailed = true;
                        Console.WriteLine($"Invalid Section '{aElement.mValue}'");
                    }

                    break;
                }

                if (!ParseMusic())
                    break;

                if (!mXMLParser.NextElement(aElement))
                    break;
            }

            if (aElement.mType != XMLElement.XMLElementType.TYPE_END &&
                aElement.mType != XMLElement.XMLElementType.TYPE_NONE)
            {
                if (!mHasFailed)
                {
                    mHasFailed = true;
                    Console.WriteLine($"Element Not Expected '{aElement.mValue}'");
                }
            }

            if (mXMLParser.HasFailed())
            {
                if (!mHasFailed)
                {
                    mHasFailed = true;
                    Console.WriteLine(mXMLParser.GetErrorText());
                }
            }

            mXMLParser.Dispose();
            mXMLParser = null;

            return !mHasFailed;
        }

        public bool ParseMusic()
        {
            XMLElement aElement = new XMLElement();

            if (!mXMLParser.NextElement(aElement))
                return false;

            while (aElement.mType == XMLElement.XMLElementType.TYPE_START)
            {
                if (aElement.mValue.ToString() != "DefaultEvents" && aElement.mValue.ToString() != "Song")
                {
                    if (!mHasFailed)
                    {
                        mHasFailed = true;
                        Console.WriteLine($"Invalid Section '{aElement.mValue}'");
                    }

                    return false;
                }

                if (aElement.mValue.ToString() == "DefaultEvents")
                {
                    if (!ParseDefaultEvents(aElement))
                        return false;
                }
                else if (aElement.mValue.ToString() == "Song")
                {
                    SongInfo song = new SongInfo();
                    if (!ParseSong(song, aElement))
                    {
                        return false;
                    }
                }

                if (!mXMLParser.NextElement(aElement))
                    return false;
            }

            if (aElement.mType == XMLElement.XMLElementType.TYPE_END)
                return true;

            if (aElement.mType == XMLElement.XMLElementType.TYPE_ELEMENT)
            {
                if (!mHasFailed)
                {
                    mHasFailed = true;
                    Console.WriteLine($"Element Not Expected '{aElement.mValue}'");
                }

                return false;
            }

            return true;
        }

        public bool ParseSong(SongInfo theSongInfo, XMLElement theXMLElement)
        {
            theSongInfo.mName = theXMLElement.GetAttribute("Name");
            if (string.IsNullOrEmpty(theSongInfo.mName))
            {
                mHasFailed = true;
                return false;
            }

            theSongInfo.mLoop = !theXMLElement.GetAttributeBool("NoLoop", false);
            if (!int.TryParse(theXMLElement.GetAttribute("Offset"), out theSongInfo.mStartPos))
            {
                theSongInfo.mStartPos = 0;
            }

            if (!float.TryParse(theXMLElement.GetAttribute("Volume"), out theSongInfo.mMainVolume))
            {
                theSongInfo.mMainVolume = 1f;
            }

            string aFilename = theXMLElement.GetAttribute("File").ToUpper();
            if (string.IsNullOrEmpty(aFilename))
            {
                mHasFailed = true;
                return false;
            }

            string aUpperName = theSongInfo.mName.ToUpper();
            int foundMusicId;
            int musicId;

            if (!mLoadedMusicFiles.TryGetValue(aUpperName, out foundMusicId))
            {
                musicId = mMusicMap.Count;
                if (!LoadMusic(musicId, aFilename, WP7AppDriver.sWP7AppDriverInstance.mContentManager))
                {
                    if (!mHasFailed)
                        mHasFailed = true;
                    return false;
                }

                mLoadedMusicFiles[aUpperName] = musicId;

                BassMusicInfo bassMusicInfo = mMusicMap[musicId];
                Bass.ChannelSetSync(bassMusicInfo.mHMusic, (SyncFlags)1073741834, -1, MusicSyncCallbackStub,
                    GCHandle.ToIntPtr(_mGcHandle));
            }
            else
            {
                musicId = foundMusicId;
            }

            theSongInfo.mMusicId = musicId;

            int aTempo;
            if (!int.TryParse(theXMLElement.GetAttribute("Tempo"), out aTempo))
            {
                aTempo = (int)Bass_MusicGetBPM(mMusicMap[musicId].mHMusic);
            }

            theSongInfo.mOrigTempo = aTempo;

            int channelIndex = 0;
            while (true)
            {
                double trackVol = Bass.ChannelGetAttribute(mMusicMap[musicId].mHMusic,
                    (ChannelAttribute.MusicVolumeChannel + channelIndex));
                if (trackVol == 0f)
                    break;
                TrackInfo trackInfo = new TrackInfo();
                trackInfo.mVolume.SetConstant(trackVol);
                theSongInfo.mTracks.Add(trackInfo);
                channelIndex++;
            }

            if (FindSong(mSongName) != null)
            {
                if (!mHasFailed)
                    mHasFailed = true;
                return false;
            }

            XMLElement aElement = new XMLElement();
            if (!mXMLParser.NextElement(aElement))
            {
                return false;
            }

            while (aElement.mType == XMLElement.XMLElementType.TYPE_START)
            {
                if (aElement.mValue.ToString() != "Event")
                {
                    if (!mHasFailed)
                        mHasFailed = true;
                    return false;
                }

                SongEventInfo aEventInfo = new SongEventInfo();
                if (!ParseEvent(aEventInfo, aElement))
                {
                    return false;
                }

                theSongInfo.mSongEventInfoMap[aEventInfo.mName] = aEventInfo;

                if (!mXMLParser.NextElement(aElement))
                {
                    return false;
                }
            }

            if (aElement.mType == XMLElement.XMLElementType.TYPE_ELEMENT)
            {
                if (!mHasFailed)
                    mHasFailed = true;
                return false;
            }

            mSongs[aUpperName] = theSongInfo;
            return true;
        }

        public bool ParseDefaultEvents(XMLElement theXMLElement)
        {
            if (!mXMLParser.NextElement(theXMLElement))
                return false;

            while (theXMLElement.mType == XMLElement.XMLElementType.TYPE_START)
            {
                if (theXMLElement.mValue.ToString() != "Event")
                {
                    Console.WriteLine($"Invalid Section '{theXMLElement.mSection}'");
                    mHasFailed = true;
                    return false;
                }

                SongEventInfo aEventInfo = new SongEventInfo();
                if (!ParseEvent(aEventInfo, theXMLElement))
                    return false;

                mDefaultEventsMap[aEventInfo.mName] = aEventInfo;

                if (!mXMLParser.NextElement(theXMLElement))
                {
                    return false;
                }

                if (theXMLElement.mType == XMLElement.XMLElementType.TYPE_ELEMENT)
                {
                    mHasFailed = true;
                    Console.WriteLine($"Element Not Expected '{theXMLElement.mSection}'");
                    return false;
                }
            }

            if (theXMLElement.mType == XMLElement.XMLElementType.TYPE_END)
                return true;
            else
                Console.WriteLine($"Element Not Expected '{theXMLElement.mSection}'");

            return false;
        }

        public bool ParseEvent(SongEventInfo theEvent, XMLElement theEventElement)
        {
            theEvent.mName = theEventElement.GetAttribute("Name")?.ToUpper();
            theEvent.mVolumeData = theEventElement.GetAttribute("Volume");
            theEvent.mTempoData = theEventElement.GetAttribute("Tempo");
            theEvent.mOffsetData = theEventElement.GetAttribute("Offset");
            theEvent.mMultVolume = theEventElement.HasAttribute("MultVolume");

            if (theEventElement.HasAttribute("Tracks"))
                ParseIntVector(theEvent.mTracks, theEventElement.GetAttribute("Tracks"));

            if (theEventElement.HasAttribute("RowSync"))
                ParseIntVector(theEvent.mRowSync, theEventElement.GetAttribute("RowSync"));

            XMLElement aElement = new XMLElement();

            if (!mXMLParser.NextElement(aElement))
            {
                mHasFailed = true;
                return false;
            }

            while (true)
            {
                switch (aElement.mType)
                {
                    case XMLElement.XMLElementType.TYPE_INSTRUCTION:
                    case XMLElement.XMLElementType.TYPE_COMMENT:
                        break;

                    case XMLElement.XMLElementType.TYPE_ELEMENT:
                        mHasFailed = true;
                        Console.WriteLine($"Element Not Expected '{aElement.mValue}'");
                        return false;

                    case XMLElement.XMLElementType.TYPE_START:
                        mHasFailed = true;
                        Console.WriteLine($"Invalid Section '{aElement.mValue}'");
                        return false;

                    case XMLElement.XMLElementType.TYPE_END:
                        return true;
                }

                aElement = new XMLElement();
                if (!mXMLParser.NextElement(aElement))
                {
                    mHasFailed = true;
                    return false;
                }
            }
        }

        public void ParseIntVector(List<int> theVector, string theString)
        {
            var parts = theString.Split(',');
            foreach (var part in parts)
            {
                if (int.TryParse(part.Trim(), out var value))
                    theVector.Add(value);
            }
        }
        
        public void SetTempo(string theSongName, double theTempo)
        {
            SongInfo aSongInfo = FindSong(theSongName);
            if (aSongInfo != null)
            {
                if (mMusicMap.TryGetValue(aSongInfo.mMusicId, out BassMusicInfo info))
                {
                    double tempo = theTempo switch
                    {
                        > 255 => 255,
                        < 0 => 0,
                        _ => theTempo
                    };
                    aSongInfo.mTempo.SetConstant(theTempo);
                    Bass_MusicSetBPM(info.mHMusic, tempo);
                }
            }
        }

        public new void Dispose()
        {
            base.Dispose();
            mXMLParser?.Dispose();
            mLoadedMusicFiles.Clear();
            mSongs.Clear();
            mDefaultEventsMap.Clear();
            mQueuedSongCommandVector.Clear();

            if (_mGcHandle.IsAllocated)
                _mGcHandle.Free();
        }
    }
}