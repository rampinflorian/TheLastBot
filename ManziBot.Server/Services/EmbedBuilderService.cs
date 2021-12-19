using Discord;
using Discord.WebSocket;

namespace ManziBot.Server.Services;

public class EmbedBuilderService
{
    public static Task<List<EmbedBuilder>> GetEmbedBuilders(List<SocketGuildUser> streamingUsers)
    {
        List<EmbedBuilder> embedBuilders = new();

        foreach (var user in streamingUsers)
        {
            embedBuilders.Add(
                new EmbedBuilder()
                {
                    Author = new EmbedAuthorBuilder()
                    {
                        Name = user.Nickname ?? user.Username,
                        Url = ((StreamingGame)(user.Activity)).Url,
                        IconUrl = "https://cdn0.iconfinder.com/data/icons/social-network-7/50/16-512.png"
                    },
                    Color = Color.Purple,
                    Title = user.Activity.Details,
                    // Description = "",
                    Url = ((StreamingGame)(user.Activity)).Url,
                    ThumbnailUrl = user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl(),
                    ImageUrl =  user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl(),
                    Footer = new EmbedFooterBuilder()
                    {
                        Text = "ManziBot"
                    }
                }
            );
        }
            
        return Task.FromResult(embedBuilders);
    }
}