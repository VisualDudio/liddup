using System;
using System.Collections.ObjectModel;
using System.Linq;
using Couchbase.Lite;

using Liddup.Models;
using Xamarin.Forms;

namespace Liddup.Services
{
    public static class SongManager
    {
        public delegate void DatabaseChangedDelegate(object sender, DatabaseChangeEventArgs e);

        private static readonly Database _database;
        private static readonly Manager _manager;
        private const ushort Port = 5431;
        private const string Scheme = "http";
        public static string Host;
        private const string DatabaseName = "liddupsongs003";

        static SongManager()
        {
            _manager = Manager.SharedInstance;
            _database = _manager.GetDatabase(DatabaseName);
        }

        public static Song GetSong(string id)
        {
            var doc = _database.GetDocument(id);
            var props = doc.UserProperties;
            var song = new Song
            {
                Id = id,
                Title = props["title"].ToString(),
                Uri = props["uri"].ToString(),
                Votes = Convert.ToInt32(props["votes"].ToString()),
                SongSource = props["songsource"].ToString()
            };

            return song;
        }

        public static byte[] GetSongContents(Song song)
        {
            return _database.GetDocument(song.Id).CurrentRevision.GetAttachment("contents").Content.ToArray();
        }

        public static ObservableCollection<Song> GetSongs()
        {
            var query = _database.CreateAllDocumentsQuery();
            var results = query.Run().OrderBy(x => x.Document.Properties["votes"]);

            var songs = new ObservableCollection<Song>();

            foreach (var row in results)
            {
                try
                {
                    var song = new Song
                    {
                        Id = row.Document.Id,
                        Title = row.Document.UserProperties["title"].ToString(),
                        Uri = row.Document.UserProperties["uri"].ToString(),
                        Votes = Convert.ToInt32(row.Document.UserProperties["votes"].ToString()),
                        SongSource = row.Document.UserProperties["songsource"].ToString()
                    };
                    songs.Add(song);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            return songs;
        }

        public static string SaveSong(Song song)
        {
            Document doc;

            if (song.Id == null)
            {
                doc = _database.CreateDocument();
                doc.PutProperties(song.ToDictionary());
                var newRevision = doc.CurrentRevision.CreateRevision();
                newRevision.SetAttachment("contents", "audio/mpeg", song.Contents);
                newRevision.Save();
                song.Id = doc.Id;
            }
            else
            {
                doc = _database.GetDocument(song.Id);
                try
                {
                    doc.Update(newRevision =>
                    {
                        var props = newRevision.UserProperties;
                        props["votes"] = song.Votes;
                        song.Id = newRevision.Document.Id;
                        newRevision.SetUserProperties(props);
                        return true;
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            }
            return doc.Id;
        }

        public static void DeleteSong(Song song)
        {
            var doc = _database.GetExistingDocument(song.Id);
            doc?.Delete();
        }

        public static void StartListener()
        {
            DependencyService.Get<INetworkManager>().Start(_manager, Port);
        }

        public static void StartReplications(DatabaseChangedDelegate refresher)
        {
            var pull = _database.CreatePullReplication(CreateSyncUri());
            var push = _database.CreatePushReplication(CreateSyncUri());

            pull.Continuous = true;
            push.Continuous = true;

            _database.Changed += (sender, e) =>
            {
                refresher?.Invoke(sender, e);
            };

            pull.Start();
            push.Start();
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
                System.Diagnostics.Debug.WriteLine("Error in creating sync uri!");
            }

            return syncUri;
        }

        public static void DeleteDatabase()
        {
            _database?.Delete();
        }

        public static void DeleteDatabases()
        {
            var database = _manager.GetExistingDatabase(DatabaseName);
            foreach (var doc in database.CreateAllDocumentsQuery().Run())
                doc.Document.Delete();
        }
    }
}