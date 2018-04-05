using PropertyChanged;

namespace Liddup.PageModels
{
    [AddINotifyPropertyChangedInterface]
    public class RoomInformationPageModel : AppContainerPageModel
    {
        public string RoomName { get; set; }
        public string RoomCode { get; set; }
        public bool AllowsExplicitSongs { get; set; }
        public int SongRequestLimit { get; set; }

        public RoomInformationPageModel()
        {
            RoomName = Room.Name;
            RoomCode = Room.Code;
            AllowsExplicitSongs = Room.ExplicitSongsAllowed;
            SongRequestLimit = Room.SongRequestLimit;
        }
    }
}
