﻿namespace AvaVKPlayer.Models.Interfaces
{
    public interface IVkModelBase
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public IImageBase Image { get; set; }
    }
}
