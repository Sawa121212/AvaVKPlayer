﻿using System;
using System.Linq;
using AvaVKPlayer.ETC;
using AvaVKPlayer.Models;
using ManagedBass;

namespace AvaVKPlayer.ViewModels
{
    public partial class PlayerControlViewModel
    {
        public static class Player
        {
            private static int _stream;
            private static bool _isNew = false;
          

            static Player()
            {
                Bass.Configure(Configuration.IncludeDefaultDevice,true);
                Bass.Init();
            }

            public static int GetStreamHandler()
            {
                return _stream;
            }
            

            public static int GetPositionSeconds()
            {
                return Convert.ToInt32(Bass.ChannelBytes2Seconds(_stream, Bass.ChannelGetPosition(_stream)));
            }


            public static void SetPositon(double val)
            {
                try
                {
                    Bass.ChannelSetPosition(_stream, Bass.ChannelSeconds2Bytes(_stream, val));
                }
                catch (Exception)
                {
                }
            }

            public static void Update()
            {
               Bass.ChannelUpdate(_stream,0);
            }

            public static void SetStream(AudioModel audioModel)
            {
                
                string? url = GlobalVars.VkApi?.Audio.GetById(new[] { audioModel.GetAudioIdFormatWithAccessKey() })
                    .ElementAt(0).Url.AbsoluteUri;
               
                _stream = Bass.CreateStream(url, 0,BassFlags.Default,null, IntPtr.Zero);    

                Errors err = Bass.LastError;

                if (err is Errors.OK) _isNew = false;
                
                if (_isNew && err == Errors.FileOpen)
                    SetStream(audioModel);
            }
           

            public static bool Play(AudioModel model)
            {
                try
                {

                    Stop();
                    _isNew = true;
                    SetStream(model);
                    return Play();
                }
                catch (Exception)
                {
                    return false;
                }
            }

            public static bool Play()
            {
                
                return Bass.ChannelPlay(_stream);
            }


            public static bool Stop()
            {
                try
                {
                    if (Bass.ChannelStop(_stream))
                    {
                        Bass.StreamFree(_stream);
                    }

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            public static bool Pause()
            {
                try
                {
                    return Bass.ChannelPause(_stream);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            public static void SetVolume(double volume)
            {
                Bass.ChannelSetAttribute(_stream, ChannelAttribute.Volume, volume);
            }

            public static PlaybackState GetStatus()=>
                Bass.ChannelIsActive(_stream);
                
        }
    }
}