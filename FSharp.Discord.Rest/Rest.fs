﻿module FSharp.Discord.Rest.Rest

open FSharp.Discord.Types.Serialization
open System.Net.Http
open Thoth.Json.Net

let [<Literal>] API_BASE_URL = "https://discord.com/api/v10"
let [<Literal>] OAUTH_BASE_URL = "https://discord.com/api/oauth2"

// ----- Interactions: Receiving and Responding -----

// https://discord.com/developers/docs/interactions/receiving-and-responding#create-interaction-response
let createInteractionResponse (req: CreateInteractionResponseRequest) (client: IBotClient) =
    Uri.create [API_BASE_URL; "interactions"; req.InteractionId; req.InteractionToken; "callback"]
    |> Uri.toRequest HttpMethod.Post
    |> HttpRequestMessage.withPayload req.Payload
    |> client.SendAsync
    |> Task.bind DiscordResponse.unit
    
// https://discord.com/developers/docs/interactions/receiving-and-responding#create-interaction-response
let createInteractionResponseWithCallback (req: CreateInteractionResponseRequest) (client: IBotClient) =
    Uri.create [API_BASE_URL; "interactions"; req.InteractionId; req.InteractionToken; "callback"]
    |> Uri.withRequiredQuery "with_response" "true"
    |> Uri.toRequest HttpMethod.Post
    |> HttpRequestMessage.withPayload req.Payload
    |> client.SendAsync
    |> Task.bind (DiscordResponse.decode InteractionCallbackResponse.decoder)

// https://discord.com/developers/docs/interactions/receiving-and-responding#get-original-interaction-response
let getOriginalInteractionResponse (req: GetOriginalInteractionResponseRequest) (client: IBotClient) =
    Uri.create [API_BASE_URL; "webhooks"; req.InteractionId; req.InteractionToken; "messages"; "@original"]
    |> Uri.toRequest HttpMethod.Get
    |> client.SendAsync
    |> Task.bind (DiscordResponse.decode Message.decoder)
    
// https://discord.com/developers/docs/interactions/receiving-and-responding#edit-original-interaction-response
let editOriginalInteractionResponse (req: EditOriginalInteractionResponseRequest) (client: IBotClient) =
    Uri.create [API_BASE_URL; "webhooks"; req.InteractionId; req.InteractionToken; "messages"; "@original"]
    |> Uri.toRequest HttpMethod.Patch
    |> HttpRequestMessage.withPayload req.Payload
    |> client.SendAsync
    |> Task.bind (DiscordResponse.decode Message.decoder)

// https://discord.com/developers/docs/interactions/receiving-and-responding#delete-original-interaction-response
let deleteOriginalInteractionResponse (req: DeleteOriginalInteractionResponseRequest) (client: IBotClient) =
    Uri.create [API_BASE_URL; "webhooks"; req.InteractionId; req.InteractionToken; "messages"; "@original"]
    |> Uri.toRequest HttpMethod.Delete
    |> client.SendAsync
    |> Task.bind DiscordResponse.unit

// https://discord.com/developers/docs/interactions/receiving-and-responding#create-followup-message
let createFollowupMessage (req: CreateFollowupMessageRequest) (client: IBotClient) =
    Uri.create [API_BASE_URL; "webhooks"; req.ApplicationId; req.InteractionToken]
    |> Uri.toRequest HttpMethod.Post
    |> HttpRequestMessage.withPayload req.Payload
    |> client.SendAsync
    |> Task.bind (DiscordResponse.decode Message.decoder)

// https://discord.com/developers/docs/interactions/receiving-and-responding#get-followup-message
let getFollowupMessage (req: GetFollowupMessageRequest) (client: IBotClient) =
    Uri.create [API_BASE_URL; "webhooks"; req.ApplicationId; req.InteractionToken; "messages"; req.MessageId]
    |> Uri.toRequest HttpMethod.Get
    |> client.SendAsync
    |> Task.bind (DiscordResponse.decode Message.decoder)

// https://discord.com/developers/docs/interactions/receiving-and-responding#edit-followup-message
let editFollowupMessage (req: EditFollowupMessageRequest) (client: IBotClient) =
    Uri.create [API_BASE_URL; "webhooks"; req.ApplicationId; req.InteractionToken; "messages"; req.MessageId]
    |> Uri.toRequest HttpMethod.Patch
    |> HttpRequestMessage.withPayload req.Payload
    |> client.SendAsync
    |> Task.bind (DiscordResponse.decode Message.decoder)

// https://discord.com/developers/docs/resources/webhook#delete-webhook-message
let deleteFollowupMessage (req: DeleteFollowupMessageRequest) (client: IBotClient) =
    Uri.create [API_BASE_URL; "webhooks"; req.ApplicationId; req.InteractionToken; "messages"; req.MessageId]
    |> Uri.toRequest HttpMethod.Delete
    |> client.SendAsync
    |> Task.bind DiscordResponse.unit

// ----- Interactions: Application Commands -----

// https://discord.com/developers/docs/interactions/application-commands#get-global-application-commands
let getGlobalApplicationCommands (req: GetGlobalApplicationCommandsRequest) (client: IBotClient) =
    Uri.create [API_BASE_URL; "applications"; req.ApplicationId; "commands"]
    |> Uri.withOptionalQuery "with_localizations" (Option.map string req.withLocalizations)
    |> Uri.toRequest HttpMethod.Get
    |> client.SendAsync
    |> Task.bind (DiscordResponse.decode (Decode.list ApplicationCommand.decoder))

// https://discord.com/developers/docs/interactions/application-commands#create-global-application-command
let createGlobalApplicationCommand (req: CreateGlobalApplicationCommandRequest) (client: IBotClient) =
    Uri.create [API_BASE_URL; "applications"; req.ApplicationId; "commands"]
    |> Uri.toRequest HttpMethod.Post
    |> HttpRequestMessage.withPayload req.Payload
    |> client.SendAsync
    |> Task.bind (DiscordResponse.decode ApplicationCommand.decoder)

// https://discord.com/developers/docs/interactions/application-commands#get-global-application-command
let getGlobalApplicationCommand (req: GetGlobalApplicationCommandRequest) (client: IBotClient) =
    Uri.create [API_BASE_URL; "applications"; req.ApplicationId; "commands"; req.CommandId]
    |> Uri.toRequest HttpMethod.Get
    |> client.SendAsync
    |> Task.bind (DiscordResponse.decode ApplicationCommand.decoder)

// https://discord.com/developers/docs/interactions/application-commands#edit-global-application-command
let editGlobalApplicationCommand (req: EditGlobalApplicationCommandRequest) (client: IBotClient) =
    Uri.create [API_BASE_URL; "applications"; req.ApplicationId; "commands"; req.CommandId]
    |> Uri.toRequest HttpMethod.Patch
    |> HttpRequestMessage.withPayload req.Payload
    |> client.SendAsync
    |> Task.bind (DiscordResponse.decode ApplicationCommand.decoder)

// https://discord.com/developers/docs/interactions/application-commands#delete-global-application-command
let deleteGlobalApplicationCommand (req: DeleteGlobalApplicationCommandRequest) (client: IBotClient) =
    Uri.create [API_BASE_URL; "applications"; req.ApplicationId; "commands"; req.CommandId]
    |> Uri.toRequest HttpMethod.Delete
    |> client.SendAsync
    |> Task.bind DiscordResponse.unit

// https://discord.com/developers/docs/interactions/application-commands#bulk-overwrite-global-application-commands
let bulkOverwriteGlobalApplicationCommands (req: BulkOverwriteGlobalApplicationCommandsRequest) (client: IBotClient) =
    Uri.create [API_BASE_URL; "applications"; req.ApplicationId; "commands"]
    |> Uri.toRequest HttpMethod.Put
    |> HttpRequestMessage.withPayload req.Payload
    |> client.SendAsync
    |> Task.bind (DiscordResponse.decode (Decode.list ApplicationCommand.decoder))

// TODO: Guild commands
// TODO: Guild command permissions
