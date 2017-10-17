using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Couchbase.Lite;

using Liddup.Models;
using Xamarin.Forms;

namespace Liddup
{
    public static class SongManager
    {
        public delegate void ReloadDataDelegate();

        private static Database Database;
        private static Manager Manager;
        private const ushort Port = 5432;
        private const string Scheme = "http";
        private const string Host = "172.16.29.132";
        private const string DatabaseName = "liddupsongs";

        static SongManager()
        {
           
        }

        public static void InitManager()
        {
            Manager = Manager.SharedInstance;
            Database = Manager.GetDatabase(DatabaseName);
            Database.Changed += Database_Changed;
        }

        private static void Database_Changed(object sender, DatabaseChangeEventArgs e)
        {
            var changes = e.Changes;

            foreach (var change in changes)
            {
                System.Diagnostics.Debug.WriteLine(GetSong(change.DocumentId).Uri);
            }
        }

        public static Song GetSong(string id)
        {
            var doc = Database.GetDocument(id);
            var props = doc.UserProperties;
            var song = new Song
            {
                Id = id,
                Title = props["title"].ToString(),
                Description = props["description"].ToString(),
                Uri = props["uri"].ToString()
            };

            return song;
        }

        public static ObservableCollection<Song> GetSongs()
        {
            var query = Database.CreateAllDocumentsQuery();
            var results = query.Run();
            var songs = new ObservableCollection<Song>();

            foreach (var row in results)
            {
                var song = new Song
                {
                    Id = row.DocumentId,
                    Title = row.Document.UserProperties["title"].ToString(),
                    Description = row.Document.UserProperties["description"].ToString(),
                    Uri = row.Document.UserProperties["uri"].ToString()
                };
                songs.Add(song);
            }

            return songs;
        }

        public static string SaveSong(Song song)
        {
            Document doc;

            if (song.Id == null)
            {
                doc = Database.CreateDocument();
                doc.PutProperties(song.ToDictionary());
                song.Id = doc.Id;
            }
            else
            {
                doc = Database.GetDocument(song.Id);
                try
                {
                    doc.Update(newRevision =>
                    {
                        var props = newRevision.Properties;
                        props["title"] = song.Title;
                        props["description"] = song.Description;
                        props["uri"] = song.Uri;
                        return true;
                    });
                }
                catch (Exception ex)
                {
                }
            }
            return doc.Id;
        }

        public static void DeleteSong(Song song)
        {
            var doc = Database.GetExistingDocument(song.Id);
            doc?.Delete();
        }

        public static void StartListener()
        {
            DependencyService.Get<IListener>().Start(Manager, Port);
        }

        public static void StartReplications(ReloadDataDelegate refresher)
        {
            var pull = Database.CreatePullReplication(CreateSyncUri());
            var push = Database.CreatePushReplication(CreateSyncUri());

            pull.Continuous = true;
            push.Start();

            pull.Changed += (sender, e) =>
            {
                refresher?.Invoke();
            };

            push.Changed += (sender, e) =>
            {
                refresher?.Invoke();
            };
        }

        private static Uri CreateSyncUri()
        {
            Uri syncUri = null;

            try
            {
                var uriBuilder = new UriBuilder(Scheme, Host, Port, DatabaseName);
                syncUri = uriBuilder.Uri;
            }
            catch
            {

            }

            return syncUri;
        }
    }
}
