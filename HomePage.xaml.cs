using System;
using System.IO;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static LlOOyBeats.HomePage;
using ZPF.Media;
using Firebase.Database;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System.Drawing;
using Email.Net;
using static QuestPDF.Helpers.Colors;
using CSharpShellCore;
using Newtonsoft.Json;
using System.Net.Mime;
using Rave;
using Rave.NET.Models.Card;
using Rave.NET.Models.Charge;

namespace LlOOyBeats
{
    public partial class HomePage : ContentPage
    {
        public class Beat
        {
            public string Title_ { get; set; }
            public string Genre { get; set; }
            public string Emotion { get; set; }
            public string Style { get; set; }
            public string Audio { get; set; }
            public string Lyric { get; set; }
            public string Art { get; set; }
            public string BPM { get; set; }
            public string ID { get; set; }
        }
        public class Licence
        {
            public string Name { get; set; }
            public string Audio_streams { get; set; }
            public string Video_streams { get; set; }
            public int Song { get; set; }
            public int Music_videos { get; set; }
            public string Radio_stations { get; set; }
            public string Performance { get; set; }
            public string File_received { get; set; }
            public decimal Price { get; set; }
        }
        public class DictionaryTwo : Dictionary<string, string>
        {
        }
        public class ListTwo : ObservableCollection<DictionaryTwo>
        {
        }
        public HomePage()
        {
            InitializeComponent();
            ////Vars
            var beat_list = new ObservableCollection<Beat>();
            var beat = new Beat();
            var beat_player = ZeMediaPlayer.Current;
            var license = new Licence();
            var options = new FirebaseOptions();
            options.SyncPeriod = TimeSpan.FromSeconds(05);
            options.JsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            options.JsonSerializerSettings.FloatFormatHandling = FloatFormatHandling.DefaultValue;
            options.JsonSerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
            options.JsonSerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            var general__firebase = new FirebaseClient("https://llooybeat-default-rtdb.europe-west1.firebasedatabase.app/", options).Child("Beats");
            var beat__firebase = general__firebase.Client.Child("Beat");
            var rave_config = new RaveConfig(true);
            rave_config.SecretKey = "FLWSECK-902dcf9ae00496332c0ba2a16edebbc5-192680b11d3vt-X";
            rave_config.PbfPubKey = "902dcf9ae00498bcad334aa2";
            var email = EmailMessage.Compose();
            email.From("yotechnigeria@gmail.com");
            email.ReplyTo("producerlooy@gmail.com");
            email.WithSubject("Beat Purchase Receipt");
            email.WithNormalPriority();
            ////
            var licence_list = new ObservableCollection<Licence>();
            license = new Licence();
            license.Name = "Basic";
            license.Song = 1;
            license.Performance = "false";
            license.Music_videos = 1;
            license.Audio_streams = "20000";
            license.File_received = "WAV file";
            license.Video_streams = "10500";
            license.Radio_stations = "true";
            license.Price = 5;
            licence_list.Add(license);
            license = new Licence();
            license.Name = "Gold";
            license.Song = 1;
            license.Performance = "false";
            license.Music_videos = 1;
            license.Audio_streams = "100000";
            license.File_received = "WAV file";
            license.Video_streams = "74000";
            license.Radio_stations = "true";
            license.Price = 10;
            licence_list.Add(license);
            license = new Licence();
            license.Name = "Premium";
            license.Song = 1;
            license.Price = 50;
            license.Performance = "true";
            license.Music_videos = 1;
            license.Audio_streams = "2000000";
            license.File_received = "WAV file, Stems";
            license.Video_streams = "1000000";
            license.Radio_stations = "true";
            licence_list.Add(license);
            license = new Licence();
            license.Name = "Premium Plus";
            license.Song = 1;
            license.Price = 300;
            license.Performance = "true";
            license.Music_videos = 5;
            license.Audio_streams = "Unlimited";
            license.File_received = "WAV, Stems";
            license.Video_streams = "Unlimited";
            license.Radio_stations = "true";
            licence_list.Add(license);
            License_list.ItemsSource = licence_list;
            ////Other Events
            CVV.Keyboard = Keyboard.Numeric;
            Pin.Keyboard = Keyboard.Numeric;
            Email__Entry.Keyboard = Keyboard.Email;
            beat_player.PositionChanged += (s, e) =>
            {
                var a = beat_player.Position.Milliseconds / beat_player.Duration.Milliseconds;
                var ab = JsonConvert.SerializeObject(a);
                var pos = JsonConvert.DeserializeObject<double>(ab);
                Beat_Progressbar.Progress = pos;
            };
            Beat_Listview.ItemSelected += (s, e) =>
            {
                if (Bottom_Player.IsVisible == false)
                {
                    Bottom_Player.IsVisible = true;
                }
                if (beat_player.State == MediaPlayerState.Playing)
                {
                    beat_player.Pause();
                    beat_player.Stop();
                }
                beat_player.Play(beat.Audio);
                Title.Text = beat.Title_;
                Art.Source = beat.Art;
                Duration.Text = beat_player.Position.Minutes + "/" + beat_player.Position.Seconds;
            };
            Art.Clicked += (s, e) =>
            {
                if (beat_player.State == MediaPlayerState.Playing)
                {
                    beat_player.Pause();
                }
                else
                {
                    beat_player.Play();
                }
            };
            Art.Pressed += (s, e) =>
            {
                if (Card_Plate.IsVisible == false)
                {
                    Card_Plate.IsVisible = true;
                }
                var user_card = new Card(Card_No.Text, Expiration_Month.Text, Expiration_Year.Text, CVV.Text);
                var payload = new CardParams("FLWSECK-902dcf9ae00496332c0ba2a16edebbc5-192680b11d3vt-X", "902dcf9ae00498bcad334aa2", Name.Text, "customer", Email__Entry.Text, license.Price, "NGN", user_card);
                var user_card_charge = new ChargeCard(rave_config);
                var result = user_card_charge.Charge(payload, false).Result;
                if (result.Message == "AUTH_SUGGESTION" && result.Data.SuggestedAuth == "PIN")
                {
                    if (Pin.IsVisible == false)
                    {
                        Pin.IsVisible = true;
                    }
                    if (Pin.Placeholder == "Pin")
                    {
                        Pin.Completed += (s, e) =>
                        {

                            payload.Pin = Pin.Text;
                            payload.SuggestedAuth = "PIN";
                            Pin.Placeholder = "Enter OTP (Message you Receive from your Bank Concerning This Transaction).";
                        };
                    }
                    if (Pin.Placeholder == "Enter OTP (Message you Receive from your Bank Concerning This Transaction).")
                    {
                        payload.Otp = Pin.Text;
                        Pin.Completed += (s, e) =>
                        {
                            payload.Otp = Pin.Text;
                            result = user_card_charge.Charge(payload).Result;
                            if (result.Data.Amount == license.Price)
                            {
                                var receipt = Document.Create(document =>
                                {
                                    document.Page(page =>
                                    {
                                        page.Size(PageSizes.A4);
                                        page.PageColor(Colors.Blue.Lighten5);
                                        page.Foreground().Text("Receipt from Yo!Beats").Bold();
                                        page.Content().Column(column =>
                                        {
                                            column.Item().Text("Tnks for Purchasing from Yo!Beats. I'm to hear the heat you make with this");
                                            column.Item().Table(table =>
                                            {
                                                table.ColumnsDefinition(columndefinition =>
                                                {
                                                    columndefinition.RelativeColumn();
                                                });
                                                table.Cell().Row(1).Column(1).Text("Title").Bold();
                                                table.Cell().Row(1).Column(2).Text(beat.Title_);
                                                table.Cell().Row(2).Column(1).Text("Genre").Bold();
                                                table.Cell().Row(2).Column(2).Text(beat.Genre);
                                                table.Cell().Row(3).Column(1).Text("BPM").Bold();
                                                table.Cell().Row(3).Column(2).Text(beat.BPM);
                                                table.Cell().Row(4).Column(1).Text("Emotion").Bold();
                                                table.Cell().Row(4).Column(2).Text(beat.Emotion);
                                                table.Cell().Row(5).Column(1).Text("Price").Bold();
                                                table.Cell().Row(5).Column(2).Text(license.Price.ToString());
                                                table.Cell().Row(6).Column(1).Text("Style");
                                                table.Cell().Row(6).Column(2).Text(beat.Style);
                                                table.Cell().Row(7).Column(1).Text("Licence Type");
                                                table.Cell().Row(7).Column(2).Text(license.Name);
                                                table.Cell().Row(8).Column(1).Text("Audio Streams");
                                                table.Cell().Row(8).Column(2).Text(license.Audio_streams);
                                                table.Cell().Row(9).Column(1).Text("Video Streams");
                                                table.Cell().Row(9).Column(2).Text(license.Video_streams);
                                                table.Cell().Row(10).Column(1).Text("Songs Allowed to be made");
                                                table.Cell().Row(10).Column(2).Text(license.Song.ToString());
                                                table.Cell().Row(11).Column(1).Text("Allowed to Perform");
                                                table.Cell().Row(11).Column(2).Text(license.Performance);
                                                table.Cell().Row(12).Column(1).Text("Music Videos Allowed to be made");
                                                table.Cell().Row(12).Column(2).Text(license.Music_videos.ToString());
                                                table.Cell().Row(13).Column(1).Text("Files");
                                                table.Cell().Row(13).Column(2).Text(license.File_received);
                                                table.Cell().Row(14).Column(1).Text("Play on Radio Stations");
                                                table.Cell().Row(14).Column(2).Text(license.Radio_stations);
                                            });
                                        });
                                    });
                                });
                                receipt.GeneratePdf("Receipt from Yo.pdf");
                                email.WithPlainTextContent("This is your receipt from Yo for the purchase of " + beat.Title_ + ".");
                                email.Build();
                                Toast.MakeText("Success", CSharpShellCore.Toast.ToastLength.Short).Show();
                            }
                        };
                    }
                }
            };

            Purchase_Button.Clicked += (s, e) =>
            {
                if (Card_Plate.IsVisible == false)
                {
                    Card_Plate.IsVisible = true;
                }
            };
            Done_Button.Clicked += (s, e) =>
            {
                var user_card = new Card(Card_No.Text, Expiration_Month.Text, Expiration_Year.Text, CVV.Text);
                var payload = new CardParams("FLWSECK-902dcf9ae00496332c0ba2a16edebbc5-192680b11d3vt-X", "902dcf9ae00498bcad334aa2", Name.Text, "customer", Email__Entry.Text, license.Price, "NGN", user_card);
                var user_card_charge = new ChargeCard(rave_config);
                var result = user_card_charge.Charge(payload, false).Result;
                if (result.Message == "AUTH_SUGGESTION" && result.Data.SuggestedAuth == "PIN")
                {
                    if (Pin.IsVisible == false)
                    {
                        Pin.IsVisible = true;
                    }
                    if (Pin.Placeholder == "Pin")
                    {
                        Pin.Completed += (s, e) =>
                        {

                            payload.Pin = Pin.Text;
                            payload.SuggestedAuth = "PIN";
                            Pin.Placeholder = "Enter OTP (Message you Receive from your Bank Concerning This Transaction).";
                        };
                    }
                    if (Pin.Placeholder == "Enter OTP (Message you Receive from your Bank Concerning This Transaction).")
                    {
                        payload.Otp = Pin.Text;
                        Pin.Completed += (s, e) =>
                        {
                            payload.Otp = Pin.Text;
                            result = user_card_charge.Charge(payload).Result;
                            if (result.Data.Amount == license.Price)
                            {
                                var receipt = Document.Create(document =>
                                {
                                    document.Page(page =>
                                    {
                                        page.Size(PageSizes.A4);
                                        page.PageColor(Colors.Blue.Lighten5);
                                        page.Foreground().Text("Receipt from Yo!Beats").Bold();
                                        page.Content().Column(column =>
                                        {
                                            column.Item().Text("Tnks for Purchasing from Yo!Beats. I'm to hear the heat you make with this");
                                            column.Item().Table(table =>
                                            {
                                                table.ColumnsDefinition(columndefinition =>
                                                {
                                                    columndefinition.RelativeColumn();
                                                });
                                                table.Cell().Row(1).Column(1).Text("Title").Bold();
                                                table.Cell().Row(1).Column(2).Text(beat.Title_);
                                                table.Cell().Row(2).Column(1).Text("Genre").Bold();
                                                table.Cell().Row(2).Column(2).Text(beat.Genre);
                                                table.Cell().Row(3).Column(1).Text("BPM").Bold();
                                                table.Cell().Row(3).Column(2).Text(beat.BPM);
                                                table.Cell().Row(4).Column(1).Text("Emotion").Bold();
                                                table.Cell().Row(4).Column(2).Text(beat.Emotion);
                                                table.Cell().Row(5).Column(1).Text("Price").Bold();
                                                table.Cell().Row(5).Column(2).Text(license.Price.ToString());
                                                table.Cell().Row(6).Column(1).Text("Style");
                                                table.Cell().Row(6).Column(2).Text(beat.Style);
                                                table.Cell().Row(7).Column(1).Text("Licence Type");
                                                table.Cell().Row(7).Column(2).Text(license.Name);
                                                table.Cell().Row(8).Column(1).Text("Audio Streams");
                                                table.Cell().Row(8).Column(2).Text(license.Audio_streams);
                                                table.Cell().Row(9).Column(1).Text("Video Streams");
                                                table.Cell().Row(9).Column(2).Text(license.Video_streams);
                                                table.Cell().Row(10).Column(1).Text("Songs Allowed to be made");
                                                table.Cell().Row(10).Column(2).Text(license.Song.ToString());
                                                table.Cell().Row(11).Column(1).Text("Allowed to Perform");
                                                table.Cell().Row(11).Column(2).Text(license.Performance);
                                                table.Cell().Row(12).Column(1).Text("Music Videos Allowed to be made");
                                                table.Cell().Row(12).Column(2).Text(license.Music_videos.ToString());
                                                table.Cell().Row(13).Column(1).Text("Files");
                                                table.Cell().Row(13).Column(2).Text(license.File_received);
                                                table.Cell().Row(14).Column(1).Text("Play on Radio Stations");
                                                table.Cell().Row(14).Column(2).Text(license.Radio_stations);
                                            });
                                        });
                                    });
                                });
                                receipt.GeneratePdf("Receipt from Yo.pdf");
                                email.WithPlainTextContent("This is your receipt from Yo for the purchase of " + beat.Title_ + ".");
                                email.Build();
                                Toast.MakeText("Success", CSharpShellCore.Toast.ToastLength.Short).Show();
                            }
                        };
                    }
                }
            };
            ////
            //// Windows
            Window.Activated += (s, e) =>
            {
                try
                {
                    beat__firebase.AsObservable<Beat>().Subscribe(data =>
                    {
                        if (data.EventType == Firebase.Database.Streaming.FirebaseEventType.InsertOrUpdate)
                        {

                            if (beat_list.Contains(beat))
                            {
                                var index_ = 0;
                                index_ = beat_list.IndexOf(beat);
                                beat_list.RemoveAt(index_);
                                beat_list.Insert(index_, beat);
                            }
                        }
                        if (data.EventType == Firebase.Database.Streaming.FirebaseEventType.Delete)
                        {
                            beat_list.Remove(data.Object);
                        }
                    });
                }
                catch
                {
                    Toast.MakeText("An error Occured with Windows On start", CSharpShellCore.Toast.ToastLength.Long).Show();
                }
            };
            Window.Stopped += (s, e) =>
            {
                if (beat_player.State == MediaPlayerState.Playing)
                {
                    beat_player.Pause();
                    beat_player.Stop();
                }
            };
        }
    }
}