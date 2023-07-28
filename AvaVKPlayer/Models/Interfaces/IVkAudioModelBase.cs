namespace AvaVKPlayer.Models.Interfaces
{
    public interface IVkAudiOrAlbumModelBase : IVkModelBase
    {
        public long OwnerId { get; set; }
        public string Artist { get; set; }
        public string AccessKey { get; set; }
        public string Subtitle { get; set; }
    }
}