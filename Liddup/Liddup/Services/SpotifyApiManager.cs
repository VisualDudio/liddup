using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Liddup.Models;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Models;
using Xamarin.Forms;
using SpotifyAPI.Web.Enums;

namespace Liddup.Services
{
    public static class SpotifyApiManager
    {
        private static readonly SpotifyWebAPI Spotify;
        private static readonly PrivateProfile Profile;

        static SpotifyApiManager()
        {
            Spotify = new SpotifyWebAPI
            {
                UseAuth = true,
                TokenType = "Bearer",
                AccessToken = DependencyService.Get<ISpotifyApi>()?.AccessToken
            };
            Profile = Spotify.GetPrivateProfile();
            Profile.Id = Uri.EscapeDataString(Profile.Id);
        }

        public static async Task<List<SimplePlaylist>> GetUserPlaylistsAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var playlists = await Spotify.GetUserPlaylistsAsync(Profile.Id);
            token.ThrowIfCancellationRequested();
            var list = playlists.Items.ToList();

            while (playlists.Next != null)
            {
                token.ThrowIfCancellationRequested();
                playlists = await Spotify.GetUserPlaylistsAsync(Profile.Id, 20, playlists.Offset + playlists.Limit);
                token.ThrowIfCancellationRequested();
                list.AddRange(playlists.Items);
            }

            return list;
        }

        public static async Task<List<FullTrack>> GetUserPlaylistSongsAsync(string profileId, string playlistId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var savedTracks = await Spotify.GetPlaylistTracksAsync(profileId, playlistId);
            token.ThrowIfCancellationRequested();
            var list = savedTracks.Items.Select(track => track.Track).ToList();

            while (savedTracks.Next != null)
            {
                token.ThrowIfCancellationRequested();
                savedTracks = await Spotify.GetPlaylistTracksAsync(profileId, playlistId, "", 20, savedTracks.Offset + savedTracks.Limit);
                token.ThrowIfCancellationRequested();
                list.AddRange(savedTracks.Items.Select(track => track.Track));
            }

            return list;
        }

        public static async Task<List<FullTrack>> GetSavedTracks(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var savedTracks = await Spotify.GetSavedTracksAsync();
            token.ThrowIfCancellationRequested();
            var list = savedTracks.Items.Select(track => track.Track).ToList();

            while (savedTracks.Next != null)
            {
                token.ThrowIfCancellationRequested();
                savedTracks = await Spotify.GetSavedTracksAsync(20, savedTracks.Offset + savedTracks.Limit);
                token.ThrowIfCancellationRequested();
                list.AddRange(savedTracks.Items.Select(track => track.Track));
            }
            
            return list;
        }

        public static async Task<List<FullTrack>> GetSearchResults(string query, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var searchResults = (await Spotify.SearchItemsAsync(query, SearchType.Track)).Tracks;
            token.ThrowIfCancellationRequested();

            //while (searchResults.Next != null)
            //{
            //    token.ThrowIfCancellationRequested();
            //    searchResults = (await Spotify.SearchItemsAsync(query, SearchType.Track, 20, searchResults.Offset + searchResults.Limit)).Tracks;
            //    token.ThrowIfCancellationRequested();
            //    searchResults.Items.AddRange(searchResults.Items);
            //}

            return searchResults.Items;
        }

        public static void AddSongToQueue(Song song, ISongProvider sender)
        {
            //var song = new Song
            //{
            //    Uri = ((FullTrack)item).Uri,
            //    Source = "Spotify",
            //    Title = ((FullTrack)item).Name,
            //    Votes = 0
            //};

            try
            {
                song.AddToQueueCommand.Execute(null);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }

        public static void PlayTrack(string uri)
        {
            DependencyService.Get<ISpotifyApi>().PlayTrack(uri);
        }

        public static void ResumeTrack()
        {
            DependencyService.Get<ISpotifyApi>().ResumeTrack();
        }

        public static void PauseTrack()
        {
            DependencyService.Get<ISpotifyApi>().PauseTrack();
        }
    }
}
