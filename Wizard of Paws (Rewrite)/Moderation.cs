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
        public class MuteModule : ModuleBase<SocketCommandContext>
        {
            [Command("mute")]
            [Alias("muzzle")]
            [Summary("Mutes the user.")]
            [RequireBotPermission(GuildPermission.ManageRoles)]
            [RequireUserPermission(GuildPermission.Administrator)]
            public async Task MuteTask(SocketGuildUser socketGuildUser, string forTime = null)
            {
                //Checks if user context is owner
                if (socketGuildUser == Context.Guild.GetUser(174768008529575936))
                {
                    await ReplyAsync("I cannot mute my creator.");
                    return;
                }

                //Admin Check
                if (PermissionHelper.HasPermission(socketGuildUser, GuildPermission.Administrator))
                {
                    await ReplyAsync($"{socketGuildUser.Mention} is untameable, put your muzzle away.");
                    return;
                }

                //Fetching the Muted Role
                SocketRole mutedRole =
                    (from r in socketGuildUser.Guild.Roles where r.Name == "Muted" select r).FirstOrDefault();
                //Create the role if it doesn't exist.
                if (mutedRole == null)
                {
                    await socketGuildUser.Guild.CreateRoleAsync("Muted", GuildPermissions.None, Color.DarkRed, true);
                    mutedRole = (from r in socketGuildUser.Guild.Roles where r.Name == "Muted" select r).FirstOrDefault();
                }

                //Add the role to user
                await socketGuildUser.AddRoleAsync(mutedRole);

                //Add role permissions to the channel
                var perms = new OverwritePermissions(sendMessages: PermValue.Deny);
                await socketGuildUser.Guild.GetTextChannel(Context.Channel.Id).AddPermissionOverwriteAsync(mutedRole, perms);
                await ReplyAsync($"{socketGuildUser.Mention} has been muzzled.");
            }
        }

        public class UnmuteModule : ModuleBase<SocketCommandContext>
        { 
            [Command("unmute")]
            [Alias("unmuzzle")]
            [Summary("Unmutes a user.")]
            [RequireBotPermission(GuildPermission.ManageRoles)]
            [RequireUserPermission(GuildPermission.Administrator)]
            public async Task UnmuteTask(SocketGuildUser socketGuildUser, string forTime = null)
            {
                //Checks if user context is owner
                if (socketGuildUser == Context.Guild.GetUser(174768008529575936))
                {
                    await ReplyAsync("I will not disobey my master.");
                    return;
                }

                //Admin role check
                if (PermissionHelper.HasPermission(socketGuildUser, GuildPermission.Administrator))
                {
                    await ReplyAsync($"{socketGuildUser.Mention} is untameable, put your muzzle away.");
                    return;
                }

                //Fetches role
                SocketRole mutedRole =
                    (from r in socketGuildUser.Guild.Roles where r.Name == "Muted" select r).FirstOrDefault();

                //Checks user for role and unmutes them
                if (PermissionHelper.IsUserRoleHolder(socketGuildUser, mutedRole.Name))
                {
                    await socketGuildUser.RemoveRoleAsync(mutedRole);
                    await ReplyAsync($"{socketGuildUser.Mention}, {Context.User.Username} has removed your muzzle.");
                    return;
                }

                await ReplyAsync($"{socketGuildUser.Nickname} has not been muzzled.");
                return;
                
            }
        }
    }
}