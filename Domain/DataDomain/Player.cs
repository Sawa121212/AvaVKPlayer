using System;
using System.Linq;
using ManagedBass;
using Player.Domain.ETC;
using VkProvider.Module;

namespace Player.Domain
{
    public class Player
    {
        private int _stream;
        private bool _isNew = false;


        static Player()
        {
            Bass.Configure(Configuration.IncludeDefaultDevice, true);
            Bass.Init();
        }

        public int GetStreamHandler()
        {
            return _stream;
        }


        public int GetPositionSeconds()
        {
            return Convert.ToInt32(Bass.ChannelBytes2Seconds(_stream, Bass.ChannelGetPosition(_stream)));
        }


        public void SetPositon(double val)
        {
            try
            {
                Bass.ChannelSetPosition(_stream, Bass.ChannelSeconds2Bytes(_stream, val));
            }
            catch (Exception)
            {
            }
        }

        public void Update()
        {
            Bass.ChannelUpdate(_stream, 0);
        }

        public void SetStream(AudioModel audioModel)
        {
            string? url = VkApiManager.GetAudioById(new[] {audioModel.GetAudioIdFormatWithAccessKey()})
                .ElementAt(0).Url.AbsoluteUri;

            _stream = Bass.CreateStream(url, 0, BassFlags.Default, null, IntPtr.Zero);

            Errors err = Bass.LastError;

            if (err is Errors.OK) _isNew = false;

            if (_isNew && err == Errors.FileOpen)
                SetStream(audioModel);
        }


        public bool Play(AudioModel model)
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

        public bool Play()
        {
            return Bass.ChannelPlay(_stream);
        }


        public bool Stop()
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

        public bool Pause()
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

        public void SetVolume(double volume)
        {
            Bass.ChannelSetAttribute(_stream, ChannelAttribute.Volume, volume);
        }

        public PlaybackState GetStatus() =>
            Bass.ChannelIsActive(_stream);
    }
}