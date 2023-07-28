using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Avalonia.Controls;
using AvaVKPlayer.ETC;
using AvaVKPlayer.Models;
using AvaVKPlayer.Views;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using VkNet.Model;
using VkNet.Utils;

namespace AvaVKPlayer.ViewModels
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
                if (audioModel != null)
                    Events.AudioRepostEventCall(audioModel);
            });

            AudioAddCommand = ReactiveCommand.Create(async (AudioModel vkModel) =>
            {
                if (vkModel != null)
                {
                    try
                    {
                        long res = await GlobalVars.VkApi.Audio.AddAsync(vkModel.Id,
                            vkModel.OwnerId,
                            vkModel.AccessKey);
                        if (res > 0)
                        {
                            vkModel.Id = res;
                            vkModel.OwnerId = (long) (GlobalVars.VkApi?.UserId ?? 0);

                            Events.AudioAddCall(vkModel);
                            Notify.NotifyManager.Instance.PopMessage(new Notify.NotifyData("Успешно добавлено",
                                vkModel.Title));
                        }
                    }
                    catch (Exception ex)
                    {
                        Notify.NotifyManager.Instance.PopMessage(
                            new Notify.NotifyData("Ошибка добавления", vkModel.Title));
                    }
                }
            });
            AudioAddToAlbumCommand = ReactiveCommand.Create(async (AudioModel vkModel) =>
            {
                Events.AudioAddToAlbumCall(vkModel);
            });

            AudioDownloadCommand = ReactiveCommand.Create(async (AudioModel vkModel) =>
            {
                if (vkModel != null)
                {
                    if (vkModel.IsDownload) return;

                    string? fileName = string.Format("{0}-{1}.mp3", vkModel.Artist, vkModel.Title);
                    SaveFileDialog dialog = new SaveFileDialog();

                    dialog.InitialFileName = fileName;
                    dialog.DefaultExtension = "*.mp3";
                    string? path = await dialog.ShowAsync(MainWindow.Instance);

                    if (path is null) return;

                    vkModel.IsDownload = true;

                    try
                    {
                        IEnumerable<Audio>? res = await GlobalVars.VkApi.Audio.GetByIdAsync(new string[]
                            {vkModel.GetAudioIdFormatWithAccessKey()});

                        using (WebClient webClient = new WebClient())
                        {
                            webClient.DownloadFileAsync(res.ElementAt(0).Url, path);

                            webClient.DownloadFileCompleted += delegate
                            {
                                vkModel.IsDownload = false;
                                Notify.NotifyManager.Instance.PopMessage(
                                    new Notify.NotifyData("Скачивание завершено", fileName));
                            };
                            webClient.DownloadProgressChanged += (object o, DownloadProgressChangedEventArgs e) =>
                                vkModel.DownloadPercent = e.ProgressPercentage;
                        }
                    }
                    catch
                    {
                        Notify.NotifyManager.Instance.PopMessage(
                            new Notify.NotifyData("Ошибка скачивания", vkModel.Title));
                        vkModel.IsDownload = false;
                    }
                }
            });

            AudioRemoveCommand = ReactiveCommand.Create(async (AudioModel vkModel) =>
            {
                if (vkModel != null)
                {
                    if (Album is null)
                    {
                        bool awaiter = await GlobalVars.VkApi.Audio.DeleteAsync(vkModel.Id, vkModel.OwnerId);
                        if (awaiter == true)
                        {
                            Events.AudioRemoveCall(vkModel);
                            Notify.NotifyManager.Instance.PopMessage(
                                new Notify.NotifyData("Аудиозапись удалена", vkModel.Title));
                        }
                    }
                    else
                    {
                        List<string> audios = new List<string>();
                        try
                        {
                            VkCollection<Audio>? audiosres = await GlobalVars.VkApi.Audio.GetAsync(new AudioGetParams()
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

                            bool res = GlobalVars.VkApi.Audio.EditPlaylist(Album.OwnerId, (int) Album.Id, Album.Title,
                                null, audios);

                            if (res)
                                Events.AudioRmoveFromAlbumEventCall(vkModel);

                            Notify.NotifyManager.Instance.PopMessage(
                                new Notify.NotifyData("Успешно удалено",
                                    "Аудиозапись:" + vkModel.Title + "\n" + "удалена из " + Album.Title));
                        }
                        catch (Exception)
                        {
                            Notify.NotifyManager.Instance.PopMessage(
                                new Notify.NotifyData("Ошибка удаления",
                                    "Аудиозапись:" + vkModel.Title + "не была удалена"));
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
        [Reactive] public bool AudioDownloadIsVisible { get; set; }

        [Reactive] public bool AudioAddIsVisible { get; set; }

        [Reactive] public bool AudioRemoveIsVisible { get; set; }

        [Reactive] public bool AudioAddToAlbumIsVisible { get; set; }

        [Reactive] public bool AudioRepostIsVisible { get; set; }


        public IReactiveCommand AudioAddCommand { get; set; }
        public IReactiveCommand AudioDownloadCommand { get; set; }
        public IReactiveCommand AudioRemoveCommand { get; set; }
        public IReactiveCommand AudioAddToAlbumCommand { get; set; }
        public IReactiveCommand AudioRepostCommand { get; set; }

        public IReactiveCommand AudioOpenLyricsCommand { get; set; }
    }
}