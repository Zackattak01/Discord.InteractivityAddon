﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Interactivity.Pagination;
using Interactivity.Selection;

namespace Interactivity
{
    /// <summary>
    /// A service containing methods to make your discordbot more interactive.
    /// </summary>
    public sealed class InteractivityService
    {
        private BaseSocketClient Client { get; }

        public TimeSpan DefaultTimeout { get; }
        public DateTime BotStartTime { get; }

        /// <summary>
        /// Creates a new instance of <see cref="InteractivityService"/>.
        /// </summary>
        /// <param name="client">Your instance of <see cref="BaseSocketClient"/>.</param>
        /// <param name="defaultTimeout">The default timeout for this <see cref="InteractivityService"/>.</param>
        public InteractivityService(BaseSocketClient client, TimeSpan? defaultTimeout = null)
        {
            Client = client;
            BotStartTime = DateTime.Now;

            DefaultTimeout = defaultTimeout ?? TimeSpan.FromSeconds(45);
            if (DefaultTimeout <= TimeSpan.Zero)
            {
                throw new Exception("Timespan cannot be negative or zero");
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="InteractivityService"/>.
        /// </summary>
        /// <param name="client">Your instance of <see cref="DiscordSocketClient"/>.</param>
        /// <param name="defaultTimeout">The default timeout for this <see cref="InteractivityService"/>.</param>
        public InteractivityService(DiscordSocketClient client, TimeSpan? defaultTimeout = null)
            : this((BaseSocketClient) client, defaultTimeout)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="InteractivityService"/>.
        /// </summary>
        /// <param name="client">Your instance of <see cref="DiscordShardedClient"/>.</param>
        /// <param name="defaultTimeout">The default timeout for this <see cref="InteractivityService"/>.</param>
        public InteractivityService(DiscordShardedClient client, TimeSpan? defaultTimeout = null)
            : this((BaseSocketClient) client, defaultTimeout)
        {
        }

        /// <summary>
        /// Get the time passed since the bot started
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetUptime() => DateTime.Now - BotStartTime;

        /// <summary>
        /// Sends a message to a channel delayed and deletes it after another delay.
        /// </summary>
        /// <param name="channel">The target channel.</param>
        /// <param name="sendDelay">The time to wait before sending the message.</param>
        /// <param name="deleteDelay">The time to wait between sending and deleting the message.</param>
        /// <param name="text">The message to be sent.</param>
        /// <param name="isTTS">Determines whether the message should be read aloud by Discord or not.</param>
        /// <param name="embed">The <see cref="Embed"/> to be sent.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns></returns>
        public void DelayedSendMessageAndDeleteAsync(IMessageChannel channel, TimeSpan? sendDelay = null, TimeSpan? deleteDelay = null,
            string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null) => _ = Task.Run(async () =>
            {
                await Task.Delay(sendDelay ?? TimeSpan.Zero).ConfigureAwait(false);
                var msg = await channel.SendMessageAsync(text, isTTS, embed, options).ConfigureAwait(false);
                await Task.Delay(deleteDelay ?? DefaultTimeout).ConfigureAwait(false);
                await msg.DeleteAsync().ConfigureAwait(false);
            });

        /// <summary>
        /// Sends a file to a channel delayed and deletes it after another delay.
        /// </summary>
        /// <param name="channel">The target channel.</param>
        /// <param name="sendDelay">The time to wait before sending the file.</param>
        /// <param name="deleteDelay">The time to wait between sending and deleting the file.</param>
        /// <param name="filepath">The file path of the file.</param>
        /// <param name="text">The message to be sent.</param>
        /// <param name="isTTS">Whether the message should be read aloud by Discord or not.</param>
        /// <param name="embed">The <see cref="Embed"/> to be sent.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns></returns>
        public void DelayedSendFileAndDeleteAsync(IMessageChannel channel, TimeSpan? sendDelay = null, TimeSpan? deleteDelay = null,
            string filepath = null,
            string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null) => _ = Task.Run(async () =>
            {
                await Task.Delay(sendDelay ?? TimeSpan.Zero).ConfigureAwait(false);
                var msg = await channel.SendFileAsync(filepath, text, isTTS, embed, options).ConfigureAwait(false);
                await Task.Delay(deleteDelay ?? DefaultTimeout).ConfigureAwait(false);
                await msg.DeleteAsync().ConfigureAwait(false);
            });

        /// <summary>
        /// Sends a file to a channel delayed and deletes it after another delay.
        /// </summary>
        /// <param name="channel">The target Channel.</param>
        /// <param name="sendDelay">The time to wait before sending the file.</param>
        /// <param name="deleteDelay">The time to wait between sending and deleting the file.</param>
        /// <param name="filestream">The <see cref="Stream"/> of the file to be sent.</param>
        /// <param name="filename">The name of the attachment.</param>
        /// <param name="text">The message to be sent.</param>
        /// <param name="isTTS">Whether the message should be read aloud by Discord or not.</param>
        /// <param name="embed">The <see cref="Embed"/> to be sent.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns></returns>
        public void DelayedSendFileAndDeleteAsync(IMessageChannel channel, TimeSpan? sendDelay = null, TimeSpan? deleteDelay = null,
            Stream filestream = null, string filename = null,
            string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null) => _ = Task.Run(async () =>
            {
                await Task.Delay(sendDelay ?? TimeSpan.Zero).ConfigureAwait(false);
                var msg = await channel.SendFileAsync(filestream, filename, text, isTTS, embed, options).ConfigureAwait(false);
                await Task.Delay(deleteDelay ?? DefaultTimeout).ConfigureAwait(false);
                await msg.DeleteAsync().ConfigureAwait(false);
            });

        /// <summary>
        /// Deletes a message after a delay
        /// </summary>
        /// <param name="msg">The message to delete</param>
        /// <param name="deleteDelay">The time to wait before deleting the message</param>
        /// <returns></returns>
        public void DelayedDeleteMessageAsync(IMessage msg, TimeSpan? deleteDelay = null) => _ = Task.Run(async () =>
        {
            await Task.Delay(deleteDelay ?? DefaultTimeout).ConfigureAwait(false);
            await msg.DeleteAsync().ConfigureAwait(false);
        });

        /// <summary>
        /// Retrieves the next incoming reaction that passes the <paramref name="filter"/>.
        /// </summary>
        /// <param name="filter">The <see cref="Predicate{SocketReaction}"/> which the reaction has to pass.</param>
        /// <param name="actions">The <see cref="ActionCollection{SocketReaction}"/> which gets executed to incoming reactions.</param>
        /// <param name="timeout">The time to wait before the methods retuns a timeout result.</param>
        /// <param name="token">The <see cref="CancellationToken"/> to cancel the request.</param>
        /// <returns></returns>
        public async Task<InteractivityResult<SocketReaction>> NextReactionAsync(Predicate<SocketReaction> filter = null, Func<SocketReaction, bool, Task> actions = null,
            TimeSpan? timeout = null, CancellationToken token = default)
        {
            var startTime = DateTime.UtcNow;

            filter ??= (s => true);
            actions ??= ((s, v) => Task.CompletedTask);

            var reactionSource = new TaskCompletionSource<InteractivityResult<SocketReaction>>();
            var cancelSource = new TaskCompletionSource<bool>();

            token.Register(() => cancelSource.SetResult(true));

            var reactionTask = reactionSource.Task;
            var cancelTask = cancelSource.Task;
            var timeoutTask = Task.Delay(timeout ?? DefaultTimeout);

            async Task CheckReactionAsync(Cacheable<IUserMessage, ulong> m, ISocketMessageChannel c, SocketReaction reaction)
            {
                if (reaction.UserId == Client.CurrentUser.Id)
                { //Ignore own reactions
                    await actions.Invoke(reaction, true).ConfigureAwait(false);
                    return;
                }

                if (filter.Invoke(reaction) == false)
                {
                    await actions.Invoke(reaction, true).ConfigureAwait(false);
                    return;
                }

                await actions.Invoke(reaction, false).ConfigureAwait(false);
                reactionSource.SetResult(new InteractivityResult<SocketReaction>(reaction, DateTime.UtcNow - startTime, false, false));
            }

            Client.ReactionAdded += CheckReactionAsync;

            var result = await Task.WhenAny(reactionTask, cancelTask, timeoutTask).ConfigureAwait(false);

            Client.ReactionAdded -= CheckReactionAsync;

            return result == reactionTask
                ? await reactionTask.ConfigureAwait(false)
                : result == timeoutTask
                    ? new InteractivityResult<SocketReaction>(default, timeout ?? DefaultTimeout, true, false)
                    : new InteractivityResult<SocketReaction>(default, DateTime.UtcNow - startTime, false, true);
        }

        /// <summary>
        /// Retrieves the next incoming message that passes the <paramref name="filter"/>.
        /// </summary>
        /// <param name="filter">The <see cref="Criteria{SocketMessage}"/> which the message has to pass.</param>
        /// <param name="actions">The <see cref="ActionCollection{SocketMessage}"/> which gets executed to incoming messages.</param>
        /// <param name="timeout">The time to wait before the methods retuns a timeout result.</param>
        /// <param name="token">The <see cref="CancellationToken"/> to cancel the request.</param>
        /// <returns></returns>
        public async Task<InteractivityResult<SocketMessage>> NextMessageAsync(Predicate<SocketMessage> filter = null, Func<SocketMessage, bool, Task> actions = null,
            TimeSpan? timeout = null, CancellationToken token = default)
        {
            var startTime = DateTime.UtcNow;

            actions ??= ((s, v) => Task.CompletedTask);
            filter ??= (s => true);

            var messageSource = new TaskCompletionSource<InteractivityResult<SocketMessage>>();
            var cancelSource = new TaskCompletionSource<bool>();

            token.Register(() => cancelSource.SetResult(true));

            var messageTask = messageSource.Task;
            var cancelTask = cancelSource.Task;
            var timeoutTask = Task.Delay(timeout ?? DefaultTimeout);

            async Task CheckMessageAsync(SocketMessage s)
            {
                if (s.Author.Id == Client.CurrentUser.Id)
                { //Ignore own messages
                    return;
                }

                if (filter.Invoke(s) == false)
                {
                    await actions.Invoke(s, true).ConfigureAwait(false);
                    return;
                }

                await actions.Invoke(s, false).ConfigureAwait(false);
                messageSource.SetResult(new InteractivityResult<SocketMessage>(s, s.Timestamp - BotStartTime, false, false));
            }

            Client.MessageReceived += CheckMessageAsync;

            var result = await Task.WhenAny(messageTask, timeoutTask, cancelTask).ConfigureAwait(false);

            Client.MessageReceived -= CheckMessageAsync;

            return result == messageTask
                ? await messageTask.ConfigureAwait(false)
                : result == timeoutTask
                    ? new InteractivityResult<SocketMessage>(default, timeout ?? DefaultTimeout, true, false)
                    : new InteractivityResult<SocketMessage>(default, DateTime.UtcNow - startTime, false, true);
        }

        /// <summary>
        /// Waits for a user/users to confirm something.
        /// </summary>
        /// <param name="confirmation">The <see cref="Confirmation.Confirmation"/> containing required informations about the confirmation.</param>
        /// <param name="timeout">The time before the confirmation returns a timeout result.</param>
        /// <param name="token">The <see cref="CancellationToken"/> to cancel the confirmation.</param>
        /// <returns></returns>
        public async Task<InteractivityResult<bool>> SendConfirmationAsync(Confirmation.Confirmation confirmation, IMessageChannel channel,
            TimeSpan? timeout = null, CancellationToken token = default)
        {
            var startTime = DateTime.UtcNow;
            var confirmationSource = new TaskCompletionSource<InteractivityResult<bool>>();
            var cancelSource = new TaskCompletionSource<bool>();

            token.Register(() => cancelSource.SetResult(true));

            var confirmationTask = confirmationSource.Task;
            var cancelTask = cancelSource.Task;
            var timeoutTask = Task.Delay(timeout ?? DefaultTimeout);

            var msg = await channel.SendMessageAsync(confirmation.Content.Text, embed: confirmation.Content.Embed).ConfigureAwait(false);

            async Task CheckReactionAsync(Cacheable<IUserMessage, ulong> m, ISocketMessageChannel c, SocketReaction reaction)
            {
                if (reaction.UserId == Client.CurrentUser.Id)
                {
                    return;
                }

                if (reaction.MessageId != msg.Id)
                {
                    return;
                }

                if (!confirmation.GetFilter().Invoke(reaction))
                {
                    await confirmation.GetActions().Invoke(reaction, true).ConfigureAwait(false);
                    return;
                }

                await confirmation.GetActions().Invoke(reaction, false).ConfigureAwait(false);

                if (reaction.Emote.Equals(confirmation.ConfirmEmote))
                {
                    confirmationSource.SetResult(new InteractivityResult<bool>(true, DateTime.UtcNow - startTime, false, false));
                    return;
                }
                else
                {
                    confirmationSource.SetResult(new InteractivityResult<bool>(false, DateTime.UtcNow - startTime, false, true));
                    return;
                }
            }

            Client.ReactionAdded += CheckReactionAsync;

            await msg.AddReactionsAsync(confirmation.Emotes).ConfigureAwait(false);
            var task_result = await Task.WhenAny(confirmationTask, cancelTask, timeoutTask).ConfigureAwait(false);

            Client.ReactionAdded -= CheckReactionAsync;

            var result = task_result == confirmationTask
                    ? await confirmationTask.ConfigureAwait(false)
                    : task_result == timeoutTask
                        ? new InteractivityResult<bool>(default, timeout ?? DefaultTimeout, true, false)
                        : new InteractivityResult<bool>(default, DateTime.UtcNow - startTime, false, true);

            if (confirmation.Deletion.HasFlag(DeletionOption.AfterCapturedContext))
            {
                await msg.DeleteAsync().ConfigureAwait(false);
            }
            else if (result.IsCancelled == true)
            {
                await msg.ModifyAsync(x => x.Embed = confirmation.CancelledEmbed).ConfigureAwait(false);
            }

            else if (result.IsTimeouted == true)
            {
                await msg.ModifyAsync(x => x.Embed = confirmation.TimeoutedEmbed).ConfigureAwait(false);
            }

            return result;
        }

        /// <summary>
        /// Sends a user selection into a discord channel.
        /// </summary>
        /// <typeparam name="T">The type of values the user can select from.</typeparam>
        /// <typeparam name="TAppearance">The appearance class of the selection</typeparam>
        /// <param name="selection">The selection to send in the channel.</param>
        /// <param name="channel">The <see cref="IMessageChannel"/> to send the selection to.</param>
        /// <param name="timeout">The time until the selection times out.</param>
        /// <param name="token">The <see cref="CancellationToken"/> to cancel the selection.</param>
        /// <returns></returns>
        public async Task<InteractivityResult<T>> SendSelectionAsync<T>(Selection<T, SocketMessage> selection, IMessageChannel channel,
            TimeSpan? timeout = null, CancellationToken token = default)
        {
            var selectionSource = new TaskCompletionSource<InteractivityResult<T>>();
            var cancelSource = new TaskCompletionSource<bool>();

            token.Register(() => cancelSource.SetResult(true));

            var selectionTask = selectionSource.Task;
            var cancelTask = cancelSource.Task;
            var timeoutTask = Task.Delay(timeout ?? DefaultTimeout);

            var msg = await channel.SendMessageAsync(embed: selection.SelectionEmbed).ConfigureAwait(false);
            var startTime = msg.Timestamp.UtcDateTime;

            async Task CheckMessageAsync(SocketMessage s)
            {
                if (s.Channel != channel)
                {
                    return;
                }
                if (s.Author.Id == Client.CurrentUser.Id)
                {
                    return;
                }
                if (await selection.HandleResponseAsync(Client, s).ConfigureAwait(false) == false)
                {
                    return;
                }

                var sResult = await selection.ParseAsync(s, startTime).ConfigureAwait(false);
                if (sResult.IsSpecified == false)
                {
                    return;
                }

                selectionSource.SetResult(sResult.Value);
            }

            Client.MessageReceived += CheckMessageAsync;
            await selection.InitializeMessageAsync(msg);
            var task_result = await Task.WhenAny(selectionTask, timeoutTask, cancelTask).ConfigureAwait(false);
            Client.MessageReceived -= CheckMessageAsync;

            var result = task_result == selectionTask
                                ? await selectionTask.ConfigureAwait(false)
                                : task_result == timeoutTask
                                    ? new InteractivityResult<T>(default, timeout ?? DefaultTimeout, true, false)
                                    : new InteractivityResult<T>(default, DateTime.UtcNow - startTime, false, true);

            if (selection.Deletion.HasFlag(DeletionOption.AfterCapturedContext) == true)
            {
                await msg.DeleteAsync().ConfigureAwait(false);
            }
            else if (result.IsCancelled == true)
            {
                await msg.ModifyAsync(x => x.Embed = selection.CancelledEmbed).ConfigureAwait(false);
            }
            else if (result.IsTimeouted == true)
            {
                await msg.ModifyAsync(x => x.Embed = selection.TimeoutedEmbed).ConfigureAwait(false);
            }

            return result;
        }

        /// <summary>
        /// Sends a user selection into a discord channel.
        /// </summary>
        /// <typeparam name="T">The type of values the user can select from.</typeparam>
        /// <typeparam name="TAppearance">The appearance class of the selection</typeparam>
        /// <param name="selection">The selection to send in the channel.</param>
        /// <param name="channel">The <see cref="IMessageChannel"/> to send the selection to.</param>
        /// <param name="timeout">The time until the selection times out.</param>
        /// <param name="token">The <see cref="CancellationToken"/> to cancel the selection.</param>
        /// <returns></returns>
        public async Task<InteractivityResult<T>> SendSelectionAsync<T>(Selection<T, SocketReaction> selection, IMessageChannel channel,
            TimeSpan? timeout = null, CancellationToken token = default)
        {
            var startTime = DateTime.UtcNow;

            var selectionSource = new TaskCompletionSource<InteractivityResult<T>>();
            var cancelSource = new TaskCompletionSource<bool>();

            token.Register(() => cancelSource.SetResult(true));

            var selectionTask = selectionSource.Task;
            var cancelTask = cancelSource.Task;
            var timeoutTask = Task.Delay(timeout ?? DefaultTimeout);

            var msg = await channel.SendMessageAsync(embed: selection.SelectionEmbed).ConfigureAwait(false);

            async Task CheckReactionAsync(Cacheable<IUserMessage, ulong> m, ISocketMessageChannel c, SocketReaction r)
            {
                if (m.Id != msg.Id)
                {
                    return;
                }
                if (r.UserId == Client.CurrentUser.Id)
                {
                    return;
                }
                if (await selection.HandleResponseAsync(Client, r).ConfigureAwait(false) == false)
                {
                    return;
                }

                var sResult = await selection.ParseAsync(r, startTime).ConfigureAwait(false);
                if (sResult.IsSpecified == false)
                {
                    return;
                }

                selectionSource.SetResult(sResult.Value);
            }

            Client.ReactionAdded += CheckReactionAsync;
            await selection.InitializeMessageAsync(msg);
            var task_result = await Task.WhenAny(selectionTask, timeoutTask, cancelTask).ConfigureAwait(false);
            Client.ReactionAdded -= CheckReactionAsync;

            var result = task_result == selectionTask
                                ? await selectionTask.ConfigureAwait(false)
                                : task_result == timeoutTask
                                    ? new InteractivityResult<T>(default, timeout ?? DefaultTimeout, true, false)
                                    : new InteractivityResult<T>(default, DateTime.UtcNow - startTime, false, true);

            if (selection.Deletion.HasFlag(DeletionOption.AfterCapturedContext) == true)
            {
                await msg.DeleteAsync().ConfigureAwait(false);
            }
            else if (result.IsCancelled == true)
            {
                await msg.ModifyAsync(x => x.Embed = selection.CancelledEmbed).ConfigureAwait(false);
            }
            else if (result.IsTimeouted == true)
            {
                await msg.ModifyAsync(x => x.Embed = selection.TimeoutedEmbed).ConfigureAwait(false);
            }

            return result;
        }

        /// <summary>
        /// Sends a page with multiple pages which the user can move through via reactions.
        /// </summary>
        /// <param name="paginator">The paginator to send.</param>
        /// <param name="channel">The <see cref="IMessageChannel"/> to send the paginator to.</param>
        /// <param name="timeout">The time until the paginator times out.</param>
        /// <param name="token">The <see cref="CancellationToken"/> to cancel the paginator.</param>
        /// <returns></returns>
        public async Task<InteractivityResult<object>> SendPaginatorAsync(Paginator paginator, IMessageChannel channel,
            TimeSpan? timeout = null, CancellationToken token = default)
        {
            var startTime = DateTime.UtcNow;

            var resultSource = new TaskCompletionSource<InteractivityResult<object>>();
            var cancelSource = new TaskCompletionSource<bool>();

            token.Register(() => cancelSource.SetResult(true));

            var resultTask = resultSource.Task;
            var cancelTask = cancelSource.Task;
            var timeoutTask = Task.Delay(timeout ?? DefaultTimeout);

            var msg = await channel.SendMessageAsync(paginator.CurrentPage.Text, embed: paginator.CurrentPage.Embed).ConfigureAwait(false);

            async Task CheckReactionAsync(Cacheable<IUserMessage, ulong> cachedMessage, ISocketMessageChannel c, SocketReaction reaction)
            {
                if (reaction.UserId == Client.CurrentUser.Id)
                {
                    return;
                }

                if (reaction.MessageId != msg.Id)
                {
                    return;
                }

                if (!paginator.GetReactionFilter().Invoke(reaction))
                {
                    await paginator.GetActions().Invoke(reaction, true).ConfigureAwait(false);
                    return;
                }

                await paginator.GetActions().Invoke(reaction, false).ConfigureAwait(false);

                var action = paginator.Appearance.ParseAction(reaction.Emote);

                if (action == PaginatorAction.Exit)
                {
                    resultSource.SetResult(new InteractivityResult<object>(default, DateTime.UtcNow - startTime, false, true));
                    return;
                }
                if (action != PaginatorAction.None)
                {
                    bool pageChanged = paginator.ApplyAction(action);

                    if (pageChanged)
                    {
                        await msg.ModifyAsync(x =>
                        {
                            x.Embed = paginator.CurrentPage.Embed;
                            x.Content = paginator.CurrentPage.Text;
                        })
                        .ConfigureAwait(false);
                    }
                }
            }

            Client.ReactionAdded += CheckReactionAsync;

            await msg.AddReactionsAsync(paginator.Appearance.Emotes).ConfigureAwait(false);
            var task_result = await Task.WhenAny(resultTask, cancelTask, timeoutTask).ConfigureAwait(false);

            Client.ReactionAdded -= CheckReactionAsync;

            await msg.RemoveAllReactionsAsync().ConfigureAwait(false);

            var result = task_result == resultTask
                                                ? await resultTask.ConfigureAwait(false)
                                                : task_result == timeoutTask
                                                    ? new InteractivityResult<object>(default, timeout ?? DefaultTimeout, true, false)
                                                    : new InteractivityResult<object>(default, DateTime.UtcNow - startTime, false, true);

            if (paginator.Appearance.Deletion.HasFlag(DeletionOption.AfterCapturedContext))
            {
                await msg.DeleteAsync();
            }
            else if (result.IsCancelled == true)
            {
                await msg.ModifyAsync(x => x.Embed = paginator.Appearance.CancelledEmbed).ConfigureAwait(false);
            }
            else if (result.IsTimeouted == true)
            {
                await msg.ModifyAsync(x => x.Embed = paginator.Appearance.TimeoutedEmbed).ConfigureAwait(false);
            }

            return result;
        }
    }
}