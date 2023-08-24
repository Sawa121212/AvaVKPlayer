using System;
using System.Collections.Generic;
using System.Windows.Input;
using ReactiveUI;
using VkNet.Model;
using VkNet.Utils;
using VkPlayer.Domain;
using VkPlayer.Domain.ETC;
using VkProvider.Module;

namespace VkPlayer.Module.ViewModels
{
    public class AudioListButtonsViewModel : ReactiveObject
    {
        public AudioListButtonsViewModel()
        {
            AudioAddIsVisible = true;
            AudioAddToAlbumIsVisible = true;
            AudioDownloadIsVisible = true;
            AudioRemoveIsVisible = true;
            AudioRepostIsVisible = true;


            AudioOpenLyricsCommand = ReactiveCommand.Create(async (AudioModel audioModel) =>
            {
                if (audioModel.LyricsViewModel != null)
                {
                    if (audioModel.LyricsViewModel.Text is null || audioModel.LyricsViewModel.Text.Length > 0)
                    {
                        audioModel.LyricsViewModel.StartLoad();
                    }

                    audioModel.LyricsViewModel.IsVisible = !audioModel.LyricsViewModel.IsVisible;
                }
            });
            AudioRepostCommand = ReactiveCommand.Create(async (AudioModel audioModel) =>
            {
                if (audioModel != null) ;
                // ToDo Events.AudioRepostEventCall(audioModel);
            });

            AudioAddCommand = ReactiveCommand.Create(async (AudioModel vkModel) =>
            {
                if (vkModel != null)
                {
                    try
                    {
                        long res = await VkApiManager.AddAudioAsync(vkModel.Id, vkModel.OwnerId, vkModel.AccessKey);
                        if (res > 0)
                        {
                            vkModel.Id = res;
                            vkModel.OwnerId = (long) (VkApiManager.VkApi.UserId ?? 0);

                            // ToDo
                            //Events.AudioAddCall(vkModel);
                            /*Notify.NotifyManager.Instance.PopMessage(new Notify.NotifyData("Успешно добавлено",
                                vkModel.Title));*/
                        }
                    }
                    catch (Exception ex)
                    {
                        /*Notify.NotifyManager.Instance.PopMessage(
                            new Notify.NotifyData("Ошибка добавления", vkModel.Title));*/
                    }
                }
            });
            AudioAddToAlbumCommand = ReactiveCommand.Create(async (AudioModel vkModel) =>
            {
                // ToDo Events.AudioAddToAlbumCall(vkModel);
            });

            // ToDo
            /*AudioDownloadCommand = ReactiveCommand.Create(async (AudioModel vkModel) =>
            {
                if (vkModel != null)
                {
                    if (vkModel.IsDownload) return;

                    string? fileName = string.Format("{0}-{1}.mp3", vkModel.Artist, vkModel.Title);
                    SaveFileDialog dialog = new SaveFileDialog();

                    dialog.InitialFileName = fileName;
                    dialog.DefaultExtension = "*.mp3";
                    string? path = await dialog.ShowAsync(MainView.Instance);

                    if (path is null) return;

                    vkModel.IsDownload = true;

                    try
                    {
                        IEnumerable<Audio>? res = await VkApiManager.GetAudioByIdAsync(new string[]
                            {vkModel.GetAudioIdFormatWithAccessKey()});

                        using (WebClient webClient = new WebClient())
                        {
                            webClient.DownloadFileAsync(res.ElementAt(0).Url, path);

                            webClient.DownloadFileCompleted += delegate
                            {
                                vkModel.IsDownload = false;
                                /*Notify.NotifyManager.Instance.PopMessage(
                                    new Notify.NotifyData("Скачивание завершено", fileName));#1#
                            };
                            webClient.DownloadProgressChanged += (object o, DownloadProgressChangedEventArgs e) =>
                                vkModel.DownloadPercent = e.ProgressPercentage;
                        }
                    }
                    catch
                    {
                        /*Notify.NotifyManager.Instance.PopMessage(
                            new Notify.NotifyData("Ошибка скачивания", vkModel.Title));#1#
                        vkModel.IsDownload = false;
                    }
                }
            });*/

            AudioRemoveCommand = ReactiveCommand.Create(async (AudioModel vkModel) =>
            {
                if (vkModel != null)
                {
                    if (Album is null)
                    {
                        bool awaiter = await VkApiManager.DeleteAudioAsync(vkModel.Id, vkModel.OwnerId);
                        if (awaiter == true)
                        {
                            // ToDo Events.AudioRemoveCall(vkModel);
                            /*Notify.NotifyManager.Instance.PopMessage(
                                new Notify.NotifyData("Аудиозапись удалена", vkModel.Title));*/
                        }
                    }
                    else
                    {
                        List<string> audios = new List<string>();
                        try
                        {
                            VkCollection<Audio>? audiosres = await VkApiManager.GetAudioAsync(new AudioGetParams()
                            {
                                OwnerId = Album.OwnerId,
                                PlaylistId = Album.Id,
                                Count = 6000
                            });

                            for (int i = 0; i < audiosres.Count; i++)
                            {
                                if (audiosres[i].Id == vkModel.Id)
                                    continue;

                                audios.Add(audiosres[i].GetAudioIdFormatWithAccessKey());
                            }

                            bool res = VkApiManager.EditAudioPlaylist(Album.OwnerId, (int) Album.Id, Album.Title, null,
                                audios);

                            if (res) ;
                            // ToDo Events.AudioRmoveFromAlbumEventCall(vkModel);

                            /*Notify.NotifyManager.Instance.PopMessage(
                                new Notify.NotifyData("Успешно удалено",
                                    "Аудиозапись:" + vkModel.Title + "\n" + "удалена из " + Album.Title));*/
                        }
                        catch (Exception)
                        {
                            /*Notify.NotifyManager.Instance.PopMessage(
                                 new Notify.NotifyData("Ошибка удаления",
                                     "Аудиозапись:" + vkModel.Title + "не была удалена"));*/
                        }
                        finally
                        {
                            audios.Clear();
                        }
                    }
                }
            });
        }

        public AudioAlbumModel Album { get; set; } = null;
        public bool AudioDownloadIsVisible { get; set; }

        public bool AudioAddIsVisible { get; set; }

        public bool AudioRemoveIsVisible { get; set; }

        public bool AudioAddToAlbumIsVisible { get; set; }

        public bool AudioRepostIsVisible { get; set; }


        public ICommand AudioAddCommand { get; set; }
        public ICommand AudioDownloadCommand { get; set; }
        public ICommand AudioRemoveCommand { get; set; }
        public ICommand AudioAddToAlbumCommand { get; set; }
        public ICommand AudioRepostCommand { get; set; }

        public ICommand AudioOpenLyricsCommand { get; set; }
    }
}