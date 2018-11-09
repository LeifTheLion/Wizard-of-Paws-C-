using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ModuleHelper;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WizardBot
{
    public class Moderation : ModuleBase<SocketCommandContext>
    {
        [Group("mute")]
        public class MuteModule : ModuleBase<SocketCommandContext>
        {
            [Command("channelmute")]
            [Alias("channel", "c")]
            [Summary("Creates a new role.")]
            [RequireBotPermission(GuildPermission.ManageRoles)]
            [RequireUserPermission(GuildPermission.Administrator)]
            public async Task MuteTask(SocketGuildUser socketGuildUser, string forTime = null)
            {
                //Checks if message is from me
                if (socketGuildUser == Context.Guild.GetUser(174768008529575936))
                {
                    await ReplyAsync("I cannot mute my creator.");
                    return;
                }

                //Admin Check
                if (PermissionHelper.HasPermission(socketGuildUser, GuildPermission.Administrator))
                {
                    await ReplyAsync($"{socketGuildUser.Username} is an Administrator and cannot be muted.");
                    return;
                }

                //Fetching the Muted Role
                SocketRole mutedRole =
                    (from r in socketGuildUser.Guild.Roles where r.Name == "Muted" select r)
                    .FirstOrDefault();
                //Create the role if it doesn't exist.
                if (mutedRole == null)
                {
                    await socketGuildUser.Guild.CreateRoleAsync("Muted", GuildPermissions.None, Color.DarkRed, true);
                    mutedRole = (from r in socketGuildUser.Guild.Roles where r.Name == "Muted" select r)
                        .FirstOrDefault();
                }

                //Add the role to user
                await socketGuildUser.AddRoleAsync(mutedRole);

                //Add role permissions to the channel
                var perms = new OverwritePermissions(sendMessages: PermValue.Deny);
                await socketGuildUser.Guild.GetTextChannel(Context.Channel.Id).AddPermissionOverwriteAsync(mutedRole, perms);
                await ReplyAsync($"{socketGuildUser.Mention} has been muzzled in this channel.");
            }
        }
    }
}