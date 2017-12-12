using PropertyChanged;
using Xamarin.Forms;
using Liddup.Models;
using Liddup.Pages;
using Liddup.Services;
using FreshMvvm;
using System.Collections.ObjectModel;

namespace Liddup.PageModels
{
    [AddINotifyPropertyChangedInterface]
    public class AppContainerPageModel : FreshBasePageModel
    {
        public int Position { get; set; } = 1;
        public ObservableCollection<DataTemplate> Pages { get; private set; }
        protected static Room Room { get; set; }
        protected static User User { get; set; }

        private void ChangePosition(object sender, int newPosition) => Position = newPosition;

        public override void Init(object initData)
        {
            base.Init(initData);

            MessagingCenter.Subscribe<FreshBasePageModel, int>(this, "PositionChanged", ChangePosition);
            User = (initData as object[])[0] as User;

            if (User.IsHost)
            {
                Room = (initData as object[])[1] as Room;
                DatabaseManager.SaveRoom(Room);
            }
            else
                Room = DatabaseManager.GetRoom();

            Pages = new ObservableCollection<DataTemplate>
            {
                new DataTemplate(() => new RoomInformationPage()),
                new DataTemplate(() => new MasterPlaylistPage()),
                new DataTemplate(() => new AddSongsPage())
            };
        }
    }
}
