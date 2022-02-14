using Discord;
using Discord.WebSocket;

namespace TheLastBot.Server.Services;

public class EmbedBuilderService
{
    public static Task<List<EmbedBuilder>> GetEmbedBuilders(List<SocketGuildUser> streamingUsers)
    {
        var embedBuilders = new List<EmbedBuilder>();
        
        foreach (var streamingUser in streamingUsers)
        {
            var activity = (StreamingGame)streamingUser.Activities.First(m => m.Name == "Twitch");
            
            embedBuilders.Add(new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder {
                    Name = streamingUser.Nickname ?? streamingUser.Username, 
                    Url = activity.Url, 
                    IconUrl = "https://cdn0.iconfinder.com/data/icons/social-network-7/50/16-512.png" 
                },
                Color = Color.Purple,
                Title = activity.Details,
                // Description = "",
                Url = activity.Url,
                ThumbnailUrl = streamingUser.GetAvatarUrl() ?? streamingUser.GetDefaultAvatarUrl(),
                ImageUrl = streamingUser.GetAvatarUrl() ?? streamingUser.GetDefaultAvatarUrl(),
                Footer = new EmbedFooterBuilder { Text = "TheLastBot" }
            });
        }
        return Task.FromResult(embedBuilders);
    }
}