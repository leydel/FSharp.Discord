﻿namespace FSharp.Discord.Rest

open FSharp.Discord.Types
open FSharp.Discord.Types.Serialization
open Thoth.Json.Net

// ----- Interactions: Receiving and Responding -----

type CreateInteractionResponsePayload(response, ?files) =
    member val Response: InteractionResponse = response
    member val Files: Media list = defaultArg files []

    interface IPayload with
        member this.ToHttpContent() =
            HttpContent.createJsonWithFiles InteractionResponse.encoder this.Response this.Files

type CreateInteractionResponseRequest(interactionId, interactionToken, payload) =
    member val InteractionId: string = interactionId
    member val InteractionToken: string = interactionToken

    member val Payload: CreateInteractionResponsePayload = payload

type GetOriginalInteractionResponseRequest(interactionId, interactionToken) =
    member val InteractionId: string = interactionId
    member val InteractionToken: string = interactionToken
    
type EditOriginalInteractionResponsePayload(
    ?content, ?embeds, ?allowedMentions, ?components, ?attachments, ?poll, ?files
) =
    member val Content: string option option = content
    member val Embeds: Embed list option option = embeds
    member val AllowedMentions: AllowedMentions option option = allowedMentions
    member val Components: Component list option option = components
    member val Attachments: Attachment list option option = attachments
    member val Poll: Poll option option = poll
    member val Files: Media list = defaultArg files []

    static member Encoder(v: EditOriginalInteractionResponsePayload) =
        Encode.object ([]
            |> Encode.optinull "content" Encode.string v.Content
            |> Encode.optinull "embeds" (List.map Embed.encoder >> Encode.list) v.Embeds
            |> Encode.optinull "allowed_mentions" AllowedMentions.encoder v.AllowedMentions
            |> Encode.optinull "components" (List.map Component.encoder >> Encode.list) v.Components
            |> Encode.optinull "attachments" (List.map Attachment.encoder >> Encode.list) v.Attachments
            |> Encode.optinull "poll" Poll.encoder v.Poll
        )
    
    interface IPayload with
        member this.ToHttpContent() =
            HttpContent.createJsonWithFiles EditOriginalInteractionResponsePayload.Encoder this this.Files
            
type EditOriginalInteractionResponseRequest(interactionId, interactionToken, payload) =
    member val InteractionId: string = interactionId
    member val InteractionToken: string = interactionToken

    member val Payload: EditOriginalInteractionResponsePayload = payload

type DeleteOriginalInteractionResponseRequest(interactionId, interactionToken) =
    member val InteractionId: string = interactionId
    member val InteractionToken: string = interactionToken

type CreateFollowUpMessagePayload(
    ?content, ?tts, ?embeds, ?allowedMentions, ?components, ?attachments, ?flags, ?poll, ?files
) =
    member val Content: string option = content
    member val Tts: bool option = tts
    member val Embeds: Embed list option = embeds
    member val AllowedMentions: AllowedMentions option = allowedMentions
    member val Components: Component list option = components
    member val Attachments: Attachment list option = attachments
    member val Flags: int option = flags
    member val Poll: Poll option = poll
    member val Files: Media list = defaultArg files []

    static member Encoder(v: CreateFollowUpMessagePayload) =
        Encode.object ([]
            |> Encode.optional "content" Encode.string v.Content
            |> Encode.optional "tts" Encode.bool v.Tts
            |> Encode.optional "embeds" (List.map Embed.encoder >> Encode.list) v.Embeds
            |> Encode.optional "allowed_mentions" AllowedMentions.encoder v.AllowedMentions
            |> Encode.optional "components" (List.map Component.encoder >> Encode.list) v.Components
            |> Encode.optional "attachments" (List.map Attachment.encoder >> Encode.list) v.Attachments
            |> Encode.optional "flags" Encode.int v.Flags
            |> Encode.optional "poll" Poll.encoder v.Poll
        )
    
    interface IPayload with
        member this.ToHttpContent() =
            HttpContent.createJsonWithFiles CreateFollowUpMessagePayload.Encoder this this.Files

    // TODO: Change flags to list of message flags
    // TODO: Should this support thread_name and applied_tags? Not explicitly refused in docs but wouldn't make sense

type CreateFollowupMessageRequest(applicationId, interactionToken, payload) =
    member val ApplicationId: string = applicationId
    member val InteractionToken: string = interactionToken

    member val Payload: CreateFollowUpMessagePayload = payload

type GetFollowupMessageRequest(applicationId, interactionToken, messageId) =
    member val ApplicationId: string = applicationId
    member val InteractionToken: string = interactionToken
    member val MessageId: string = messageId
    
type EditFollowupMessagePayload(?content, ?embeds, ?allowedMentions, ?components, ?attachments, ?poll, ?files) =
    member val Content: string option option = content
    member val Embeds: Embed list option option = embeds
    member val AllowedMentions: AllowedMentions option option = allowedMentions
    member val Components: Component list option option = components
    member val Attachments: Attachment list option option = attachments
    member val Poll: Poll option option = poll
    member val Files: Media list = defaultArg files []

    static member Encoder(v: EditFollowupMessagePayload) =
        Encode.object ([]
            |> Encode.optinull "content" Encode.string v.Content
            |> Encode.optinull "embeds" (List.map Embed.encoder >> Encode.list) v.Embeds
            |> Encode.optinull "allowed_mentions" AllowedMentions.encoder v.AllowedMentions
            |> Encode.optinull "components" (List.map Component.encoder >> Encode.list) v.Components
            |> Encode.optinull "attachments" (List.map Attachment.encoder >> Encode.list) v.Attachments
            |> Encode.optinull "poll" Poll.encoder v.Poll
        )
    
    interface IPayload with
        member this.ToHttpContent() =
            HttpContent.createJsonWithFiles EditFollowupMessagePayload.Encoder this this.Files

type EditFollowupMessageRequest(applicationId, interactionToken, messageId, payload) =
    member val ApplicationId: string = applicationId
    member val InteractionToken: string = interactionToken
    member val MessageId: string = messageId

    member val Payload: EditFollowupMessagePayload = payload

type DeleteFollowupMessageRequest(applicationId, interactionToken, messageId) =
    member val ApplicationId: string = applicationId
    member val InteractionToken: string = interactionToken
    member val MessageId: string = messageId

// ----- Interactions: Application Commands -----

type GetGlobalApplicationCommandsRequest(applicationId, ?withLocalizations) =
    member val ApplicationId: string = applicationId

    member val withLocalizations: bool option = withLocalizations
    
type CreateGlobalApplicationCommandPayload(
    name, ?nameLocalizations, ?description, ?descriptionLocalizations, ?options, ?defaultMemberPermissions,
    ?integrationTypes, ?contexts, ?type', ?nsfw
) =
    member val Name: string = name
    member val NameLocalizations: Map<string, string> option option = nameLocalizations
    member val Description: string option = description
    member val DescriptionLocalizations: Map<string, string> option option = descriptionLocalizations
    member val Options: ApplicationCommandOption list option = options
    member val DefaultMemberPermissions: string option option = defaultMemberPermissions
    member val IntegrationTypes: ApplicationIntegrationType list option = integrationTypes
    member val Contexts: InteractionContextType list option = contexts
    member val Type: ApplicationCommandType option = type'
    member val Nsfw: bool option = nsfw
    
    static member Encoder(v: CreateGlobalApplicationCommandPayload) =
        Encode.object ([]
            |> Encode.required "name" Encode.string v.Name
            |> Encode.optinull "name_localizations" (Encode.mapv Encode.string) v.NameLocalizations
            |> Encode.optional "description" Encode.string v.Description
            |> Encode.optinull "description_localizations" (Encode.mapv Encode.string) v.DescriptionLocalizations
            |> Encode.optional "options" (List.map ApplicationCommandOption.encoder >> Encode.list) v.Options
            |> Encode.optinull "default_member_permissions" Encode.string v.DefaultMemberPermissions
            |> Encode.optional "integration_types" (List.map Encode.Enum.int<ApplicationIntegrationType> >> Encode.list) v.IntegrationTypes
            |> Encode.optional "contexts" (List.map Encode.Enum.int<InteractionContextType> >> Encode.list) v.Contexts
            |> Encode.optional "type" Encode.Enum.int<ApplicationCommandType> v.Type
            |> Encode.optional "nsfw" Encode.bool v.Nsfw
        )
    
    interface IPayload with
        member this.ToHttpContent() =
            StringContent.createJson CreateGlobalApplicationCommandPayload.Encoder this

type CreateGlobalApplicationCommandRequest(applicationId, payload) =
    member val ApplicationId: string = applicationId

    member val Payload: CreateGlobalApplicationCommandPayload = payload

type GetGlobalApplicationCommandRequest(applicationId, commandId) =
    member val ApplicationId: string = applicationId
    member val CommandId: string = commandId

type EditGlobalApplicationCommandPayload(
    ?name, ?nameLocalizations, ?description, ?descriptionLocalizations, ?options, ?defaultMemberPermissions,
    ?integrationTypes, ?contexts, ?type', ?nsfw
) =
    member val Name: string option = name
    member val NameLocalizations: Map<string, string> option option = nameLocalizations
    member val Description: string option = description
    member val DescriptionLocalizations: Map<string, string> option option = descriptionLocalizations
    member val Options: ApplicationCommandOption list option = options
    member val DefaultMemberPermissions: bool option option = defaultMemberPermissions
    member val IntegrationTypes: ApplicationIntegrationType list option = integrationTypes
    member val Contexts: InteractionContextType list option = contexts
    member val Nsfw: bool option = nsfw
    
    static member Encoder(v: EditGlobalApplicationCommandPayload) =
        Encode.object ([]
            |> Encode.optional "name" Encode.string v.Name
            |> Encode.optinull "name_localizations" (Encode.mapv Encode.string) v.NameLocalizations
            |> Encode.optional "description" Encode.string v.Description
            |> Encode.optinull "description_localizations" (Encode.mapv Encode.string) v.DescriptionLocalizations
            |> Encode.optional "options" (List.map ApplicationCommandOption.encoder >> Encode.list) v.Options
            |> Encode.optinull "default_member_permissions" Encode.bool v.DefaultMemberPermissions
            |> Encode.optional "integration_types" (List.map Encode.Enum.int<ApplicationIntegrationType> >> Encode.list) v.IntegrationTypes
            |> Encode.optional "contexts" (List.map Encode.Enum.int<InteractionContextType> >> Encode.list) v.Contexts
            |> Encode.optional "nsfw" Encode.bool v.Nsfw
        )
    
    interface IPayload with
        member this.ToHttpContent() =
            StringContent.createJson EditGlobalApplicationCommandPayload.Encoder this

type EditGlobalApplicationCommandRequest(applicationId, commandId, payload) =
    member val ApplicationId: string = applicationId
    member val CommandId: string = commandId

    member val Payload: EditGlobalApplicationCommandPayload = payload

type DeleteGlobalApplicationCommandRequest(applicationId, commandId) =
    member val ApplicationId: string = applicationId
    member val CommandId: string = commandId

type BulkOverwriteGlobalApplicationCommandsPayload(commands) =
    member val Commands: CreateGlobalApplicationCommandPayload list = commands
    
    static member Encoder(v: BulkOverwriteGlobalApplicationCommandsPayload) =
        (List.map CreateGlobalApplicationCommandPayload.Encoder >> Encode.list) v.Commands
    
    interface IPayload with
        member this.ToHttpContent() =
            StringContent.createJson BulkOverwriteGlobalApplicationCommandsPayload.Encoder this

    // TODO: Docs appear incorrect currently, likely also contains `id` like bulk guild commands, which also looks partially incorrectly documented

type BulkOverwriteGlobalApplicationCommandsRequest(applicationId, payload) =
    member val ApplicationId: string = applicationId

    member val Payload: BulkOverwriteGlobalApplicationCommandsPayload = payload

type GetGuildApplicationCommandsRequest(applicationId, guildId, ?withLocalizations) =
    member val ApplicationId: string = applicationId
    member val GuildId: string = guildId

    member val withLocalizations: bool option = withLocalizations

type CreateGuildApplicationCommandPayload(
    name, ?nameLocalizations, ?description, ?descriptionLocalizations, ?options, ?defaultMemberPermissions,
    ?type', ?nsfw
) =
    member val Name: string = name
    member val NameLocalizations: Map<string, string> option option = nameLocalizations
    member val Description: string option = description
    member val DescriptionLocalizations: Map<string, string> option option = descriptionLocalizations
    member val Options: ApplicationCommandOption list option = options
    member val DefaultMemberPermissions: string option option = defaultMemberPermissions
    member val Type: ApplicationCommandType option = type'
    member val Nsfw: bool option = nsfw
    
    static member Encoder(v: CreateGuildApplicationCommandPayload) =
        Encode.object ([]
            |> Encode.required "name" Encode.string v.Name
            |> Encode.optinull "name_localizations" (Encode.mapv Encode.string) v.NameLocalizations
            |> Encode.optional "description" Encode.string v.Description
            |> Encode.optinull "description_localizations" (Encode.mapv Encode.string) v.DescriptionLocalizations
            |> Encode.optional "options" (List.map ApplicationCommandOption.encoder >> Encode.list) v.Options
            |> Encode.optinull "default_member_permissions" Encode.string v.DefaultMemberPermissions
            |> Encode.optional "type" Encode.Enum.int<ApplicationCommandType> v.Type
            |> Encode.optional "nsfw" Encode.bool v.Nsfw
        )
    
    interface IPayload with
        member this.ToHttpContent() =
            StringContent.createJson CreateGuildApplicationCommandPayload.Encoder this

type CreateGuildApplicationCommandRequest(applicationId, guildId, payload) =
    member val ApplicationId: string = applicationId
    member val GuildId: string = guildId

    member val Payload: CreateGuildApplicationCommandPayload = payload
    
type GetGuildApplicationCommandRequest(applicationId, guildId, commandId) =
    member val ApplicationId: string = applicationId
    member val GuildId: string = guildId
    member val CommandId: string = commandId
    
type EditGuildApplicationCommandPayload(
    ?name, ?nameLocalizations, ?description, ?descriptionLocalizations, ?options, ?defaultMemberPermissions, ?nsfw
) =
    member val Name: string option = name
    member val NameLocalizations: Map<string, string> option option = nameLocalizations
    member val Description: string option = description
    member val DescriptionLocalizations: Map<string, string> option option = descriptionLocalizations
    member val Options: ApplicationCommandOption list option = options
    member val DefaultMemberPermissions: bool option option = defaultMemberPermissions
    member val Nsfw: bool option = nsfw
    
    static member Encoder(v: EditGuildApplicationCommandPayload) =
        Encode.object ([]
            |> Encode.optional "name" Encode.string v.Name
            |> Encode.optinull "name_localizations" (Encode.mapv Encode.string) v.NameLocalizations
            |> Encode.optional "description" Encode.string v.Description
            |> Encode.optinull "description_localizations" (Encode.mapv Encode.string) v.DescriptionLocalizations
            |> Encode.optional "options" (List.map ApplicationCommandOption.encoder >> Encode.list) v.Options
            |> Encode.optinull "default_member_permissions" Encode.bool v.DefaultMemberPermissions
            |> Encode.optional "nsfw" Encode.bool v.Nsfw
        )
    
    interface IPayload with
        member this.ToHttpContent() =
            StringContent.createJson EditGuildApplicationCommandPayload.Encoder this
            
type EditGuildApplicationCommandRequest(applicationId, guildId, commandId, payload) =
    member val ApplicationId: string = applicationId
    member val GuildId: string = guildId
    member val CommandId: string = commandId

    member val Payload: EditGuildApplicationCommandPayload = payload
    
type DeleteGuildApplicationCommandRequest(applicationId, guildId, commandId) =
    member val ApplicationId: string = applicationId
    member val GuildId: string = guildId
    member val CommandId: string = commandId

type BulkOverwriteGuildApplicationCommandsPayload(commands) =
    member val Commands: CreateGuildApplicationCommandPayload list = commands
    
    static member Encoder(v: BulkOverwriteGuildApplicationCommandsPayload) =
        (List.map CreateGuildApplicationCommandPayload.Encoder >> Encode.list) v.Commands
    
    interface IPayload with
        member this.ToHttpContent() =
            StringContent.createJson BulkOverwriteGuildApplicationCommandsPayload.Encoder this

    // TODO: Also contains optional `id` property "ID of the command, if known"
    // TODO: Docs include `integration_types` and `contexts` but these should be global only afaik

type BulkOverwriteGuildApplicationCommandsRequest(applicationId, guildId, payload) =
    member val ApplicationId: string = applicationId
    member val GuildId: string = guildId

    member val Payload: BulkOverwriteGuildApplicationCommandsPayload = payload

type GetGuildApplicationCommandPermissionsRequest(applicationId, guildId) =
    member val ApplicationId: string = applicationId
    member val GuildId: string = guildId

type GetApplicationCommandPermissionsRequest(applicationId, guildId, commandId) =
    member val ApplicationId: string = applicationId
    member val GuildId: string = guildId
    member val CommandId: string = commandId

type EditApplicationCommandPermissionsPayload(permissions) =
    member val Permissions: ApplicationCommandPermission list = permissions
    
    static member Encoder(v: EditApplicationCommandPermissionsPayload) =
        Encode.object ([]
            |> Encode.required "permissions" (List.map ApplicationCommandPermission.encoder >> Encode.list) v.Permissions
        )
    
    interface IPayload with
        member this.ToHttpContent() =
            StringContent.createJson EditApplicationCommandPermissionsPayload.Encoder this

type EditApplicationCommandPermissionsRequest(applicationId, guildId, commandId, payload) =
    member val ApplicationId: string = applicationId
    member val GuildId: string = guildId
    member val CommandId: string = commandId

    member val Payload: EditApplicationCommandPermissionsPayload = payload

// ----- Events: Using Gateway -----

type GetGatewayRequest(version, encoding, compression) =
    member val Version: string = version
    member val Encoding: GatewayEncoding = encoding
    member val Compression: GatewayCompression option = compression

type GetGatewayResponse = {
    Url: string
}

module GetGatewayResponse =
    let decoder: Decoder<GetGatewayResponse> =
        Decode.object (fun get -> {
            Url = get |> Get.required "url" Decode.string
        })
        
type GetGatewayBotRequest(version, encoding, compression) =
    member val Version: string = version
    member val Encoding: GatewayEncoding = encoding
    member val Compression: GatewayCompression option = compression

type GetGatewayBotResponse = {
    Url: string
    Shards: int
    SessionStartLimit: SessionStartLimit
}

module GetGatewayBotResponse =
    let decoder: Decoder<GetGatewayBotResponse> =
        Decode.object (fun get -> {
            Url = get |> Get.required "url" Decode.string
            Shards = get |> Get.required "shards" Decode.int
            SessionStartLimit = get |> Get.required "session_start_limit" SessionStartLimit.decoder
        })
        
// ----- Resources: Application -----

// ----- Resources: Application Role Connection Metadata -----

// ----- Resources: Audit Log -----

// ----- Resources: Auto Moderation -----

// ----- Resources: Channel -----

// ----- Resources: Emoji -----

// ----- Resources: Entitlement -----

// ----- Resources: Guild -----

// ----- Resources: Guild Scheduled Event -----

// ----- Resources: Guild Template -----

// ----- Resources: Invite -----

// ----- Resources: Lobby -----

// ----- Resources: Message -----

// ----- Resources: Poll -----

// ----- Resources: SKU -----

// ----- Resources: Soundboard -----

// ----- Resources: Stage Instance -----

// ----- Resources: Sticker -----

// ----- Resources: Subscription -----

// ----- Resources: User -----

// ----- Resources: Voice -----

// ----- Resources: Webhook -----

// ----- Topics: OAuth2 -----
