using System;
using TwitchLib.Api.V5;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Zack.TuyaNet;
using Zack.TuyaNet.Devices;
using Zack.TuyaNet.Services;
using System.Drawing;
using System.Collections.Generic;


namespace Twitch_Bot
{

    public class Bot
    {
        ConnectionCredentials creds = new ConnectionCredentials(TwitchInfo.ChannelName, TwitchInfo.BotToken);
        TwitchClient client;

        char nl = Convert.ToChar(11);

        public string[] _bannedWords = new string[4] { "mongoloide", "mongolóide", "mongol", "gado" };


        ServiceCollection services = new ServiceCollection();
        TuyaApiClient _tuyaApiClient;

        public void Connect(bool isLogging)
        {
            client = new TwitchClient();
            client.Initialize(creds, TwitchInfo.ChannelName);
            client.OnConnected += Client_OnConnected;

            Console.WriteLine("[Bot]: Conectando...");

            if (isLogging)
                client.OnLog += Client_OnLog;

            client.OnError += Client_OnError;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnChatCommandReceived += Client_OnChatCommandReceived;
            client.AddChatCommandIdentifier('$');

            client.Connect();

        }

        public void Client_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {


            switch (e.Command.CommandText.ToLower())
            {
                case "social":
                    client.SendMessage(TwitchInfo.ChannelName, "Aqui está minhas redes sociais! Instagram: https://www.instagram.com/lucas_tadeu_salomao/ Discord: https://discord.gg/K4rbKR5.");
                    break;
                case "comandos":
                    client.SendMessage(TwitchInfo.ChannelName, "Há um painel com todos os comandos disponíveis e um link para o meu Github com o bot utilizado neste canal, que foi feito em C#");
                    break;
                case "ligar":
                    ligar();
                    break;
                case "desligar":
                    desligar();
                    break;
                case "brilho":
                    brilho(Convert.ToUInt16(e.Command.ArgumentsAsString));
                    break;
                case "temperatura":
                    temperatura(Convert.ToUInt16(e.Command.ArgumentsAsString));
                    break;
                case "ncor":
                    ncor(e.Command.ArgumentsAsString);
                    break;
                case "wm":
                    wm();
                    break;
                case "cor":
                    List<string> argumentos = new List<string>();
                    argumentos=(e.Command.ArgumentsAsList);
                    cor(Convert.ToInt16(argumentos[0]), Convert.ToInt16(argumentos[1]), Convert.ToInt16(argumentos[2]));
                    break;
            }


            if (e.Command.ChatMessage.DisplayName == TwitchInfo.ChannelName)
            {
                switch (e.Command.CommandText.ToLower())
                {
                    case "hi":
                        client.SendMessage(TwitchInfo.ChannelName, "Olá chefe!");
                        break;
                }
            }
        }

        public void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            for (int i = 0; i < _bannedWords.Length; i++)
            {
                if (e.ChatMessage.Message.ToLower().Contains(_bannedWords[i]))
                {
                    client.BanUser(TwitchInfo.ChannelName, e.ChatMessage.Username);
                }
            }

            Console.WriteLine($"[{e.ChatMessage.DisplayName}]: {e.ChatMessage.Message}");
        }

        public void Client_OnLog(object sender, OnLogArgs e)
        {
            Console.WriteLine(e.Data);
        }

        public void Client_OnError(object sender, OnErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine("[Bot]: Conectado!");
        }

        public void Disconnect()
        {
            Console.WriteLine("[Bot]: Desconectando e fechando a aplicação");

            client.Disconnect();
        }

        public async Task ligar()
        {
            services.AddHttpClient();
            services.AddOptions().Configure<TuyaConfig>(opt => {
                opt.Region = Zack.TuyaNet.Region.WesternAmerica;
                opt.AccessId = tuya.Access_ID;
                opt.ApiSecret = tuya.Access_Secret;
            });
            services.AddScoped<TuyaApiClient>();
            services.AddScoped<LedLightDevice>();
            var sp = services.BuildServiceProvider();
            _tuyaApiClient = sp.GetRequiredService<TuyaApiClient>();
            await _tuyaApiClient.RefreshAccessTokenAsync();
            LedLightDevice lamp = sp.GetRequiredService<LedLightDevice>();
            await lamp.SwitchOnAsync(tuya.lampada_quarto_ID);
            client.SendMessage(TwitchInfo.ChannelName, "A lâmpada foi ligada com sucesso!");
        }

        public async Task desligar()
        {
            services.AddHttpClient();
            services.AddOptions().Configure<TuyaConfig>(opt => {
                opt.Region = Zack.TuyaNet.Region.WesternAmerica;
                opt.AccessId = tuya.Access_ID;
                opt.ApiSecret = tuya.Access_Secret;
            });
            services.AddScoped<TuyaApiClient>();
            services.AddScoped<LedLightDevice>();
            var sp = services.BuildServiceProvider();
            _tuyaApiClient = sp.GetRequiredService<TuyaApiClient>();
            await _tuyaApiClient.RefreshAccessTokenAsync();
            LedLightDevice lamp = sp.GetRequiredService<LedLightDevice>();
            await lamp.SwitchOffAsync(tuya.lampada_quarto_ID);
            client.SendMessage(TwitchInfo.ChannelName, "A lâmpada foi desligada com sucesso!");
        }

        public async Task brilho(ushort brilho)
        {
            services.AddHttpClient();
            services.AddOptions().Configure<TuyaConfig>(opt => {
                opt.Region = Zack.TuyaNet.Region.WesternAmerica;
                opt.AccessId = tuya.Access_ID;
                opt.ApiSecret = tuya.Access_Secret;
            });
            services.AddScoped<TuyaApiClient>();
            services.AddScoped<LedLightDevice>();
            var sp = services.BuildServiceProvider();
            _tuyaApiClient = sp.GetRequiredService<TuyaApiClient>();
            await _tuyaApiClient.RefreshAccessTokenAsync();
            LedLightDevice lamp = sp.GetRequiredService<LedLightDevice>();
            await lamp.ChangeBrightnessAsync(tuya.lampada_quarto_ID, brilho);
            client.SendMessage(TwitchInfo.ChannelName, "Brilho atualizado com sucesso!");
        }

        public async Task temperatura(ushort temperatura)
        {
            services.AddHttpClient();
            services.AddOptions().Configure<TuyaConfig>(opt => {
                opt.Region = Zack.TuyaNet.Region.WesternAmerica;
                opt.AccessId = tuya.Access_ID;
                opt.ApiSecret = tuya.Access_Secret;
            });
            services.AddScoped<TuyaApiClient>();
            services.AddScoped<LedLightDevice>();
            var sp = services.BuildServiceProvider();
            _tuyaApiClient = sp.GetRequiredService<TuyaApiClient>();
            await _tuyaApiClient.RefreshAccessTokenAsync();
            LedLightDevice lamp = sp.GetRequiredService<LedLightDevice>();
            await lamp.ChangeTemperatureAsync(tuya.lampada_quarto_ID, temperatura);
            client.SendMessage(TwitchInfo.ChannelName, "Temperatura de cor atualizada com sucesso!");
        }

        public HSV GetHSV(Color color)
        {
            
            double _h, _s, _v;

            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            _h = Math.Round(color.GetHue(), 2);
            _s = ((max == 0) ? 0 : 1d - (1d * min / max)) * 1000;
            _s = Math.Round(_s, 2);
            _v = Math.Round(((max / 255d) * 1000), 2);

            HSV toReturn = new HSV((ushort)_h,(ushort)_s ,(ushort)_v );
            return toReturn;
        }

        public async Task ncor(string cor)
        {
            services.AddHttpClient();
            services.AddOptions().Configure<TuyaConfig>(opt => {
                opt.Region = Zack.TuyaNet.Region.WesternAmerica;
                opt.AccessId = tuya.Access_ID;
                opt.ApiSecret = tuya.Access_Secret;
            });
            services.AddScoped<TuyaApiClient>();
            services.AddScoped<LedLightDevice>();
            var sp = services.BuildServiceProvider();
            _tuyaApiClient = sp.GetRequiredService<TuyaApiClient>();
            await _tuyaApiClient.RefreshAccessTokenAsync();
            LedLightDevice lamp = sp.GetRequiredService<LedLightDevice>();
            await lamp.ChangeColorAsync(tuya.lampada_quarto_ID, GetHSV(Color.FromName(cor)));
            client.SendMessage(TwitchInfo.ChannelName, "Cor atualizada com sucesso!");
        }

        public async Task cor(int r, int g, int b)
        {
            services.AddHttpClient();
            services.AddOptions().Configure<TuyaConfig>(opt => {
                opt.Region = Zack.TuyaNet.Region.WesternAmerica;
                opt.AccessId = tuya.Access_ID;
                opt.ApiSecret = tuya.Access_Secret;
            });
            services.AddScoped<TuyaApiClient>();
            services.AddScoped<LedLightDevice>();
            var sp = services.BuildServiceProvider();
            _tuyaApiClient = sp.GetRequiredService<TuyaApiClient>();
            await _tuyaApiClient.RefreshAccessTokenAsync();
            LedLightDevice lamp = sp.GetRequiredService<LedLightDevice>();
            await lamp.ChangeColorAsync(tuya.lampada_quarto_ID, GetHSV(Color.FromArgb(r,g,b)));
            client.SendMessage(TwitchInfo.ChannelName, "Cor atualizada com sucesso!");
        }

        public async Task wm()
        {
            services.AddHttpClient();
            services.AddOptions().Configure<TuyaConfig>(opt => {
                opt.Region = Zack.TuyaNet.Region.WesternAmerica;
                opt.AccessId = tuya.Access_ID;
                opt.ApiSecret = tuya.Access_Secret;
            });
            services.AddScoped<TuyaApiClient>();
            services.AddScoped<LedLightDevice>();
            var sp = services.BuildServiceProvider();
            _tuyaApiClient = sp.GetRequiredService<TuyaApiClient>();
            await _tuyaApiClient.RefreshAccessTokenAsync();
            LedLightDevice lamp = sp.GetRequiredService<LedLightDevice>();
            await lamp.ChangeWorkModeAsync(tuya.lampada_quarto_ID, "white" );
            client.SendMessage(TwitchInfo.ChannelName, "Cor atualizada com sucesso!");
        }
    }
}