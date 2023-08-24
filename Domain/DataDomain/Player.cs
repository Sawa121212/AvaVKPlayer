using System;
using System.Linq;
using ManagedBass;
using VkPlayer.Domain.ETC;
using VkProvider.Module;

namespace VkPlayer.Domain
{
    /// <summary>
    /// Плеер
    /// </summary>
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

        /// <summary>
        /// Установить позицию
        /// </summary>
        /// <param name="val"></param>
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

        /// <summary>
        /// Запустить
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Запустить
        /// </summary>
        /// <returns></returns>
        public bool Play()
        {
            return Bass.ChannelPlay(_stream);
        }

        /// <summary>
        /// Остановить
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Пауза
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Установить громкость
        /// </summary>
        /// <param name="volume"></param>
        public void SetVolume(double volume)
        {
            Bass.ChannelSetAttribute(_stream, ChannelAttribute.Volume, volume);
        }

        /// <summary>
        /// Получить статус плеера
        /// </summary>
        /// <returns></returns>
        public PlaybackState GetStatus() =>
            Bass.ChannelIsActive(_stream);
    }
}