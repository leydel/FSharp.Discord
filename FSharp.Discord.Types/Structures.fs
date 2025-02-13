﻿namespace rec FSharp.Discord.Types

open System
open System.Text.Json
open System.Text.Json.Serialization

// ----- API Reference -----

// https://discord.com/developers/docs/reference#error-messages
type ErrorResponse = {
    [<JsonPropertyName "code">] Code: JsonErrorCode
    [<JsonPropertyName "message">] Message: string
    [<JsonPropertyName "errors">] Errors: (string * string) seq
}

// ----- Interactions: Receiving and Responding -----

// https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-object-interaction-structure
type Interaction = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "application_id">] ApplicationId: string
    [<JsonPropertyName "type">] Type: InteractionType
    [<JsonPropertyName "data">] Data: InteractionData option
    [<JsonPropertyName "guild">] Guild: PartialGuild option
    [<JsonPropertyName "guild_id">] GuildId: string option
    [<JsonPropertyName "channel">] Channel: PartialChannel option
    [<JsonPropertyName "channel_id">] ChannelId: string option
    [<JsonPropertyName "member">] Member: GuildMember option
    [<JsonPropertyName "user">] User: User option
    [<JsonPropertyName "token">] Token: string
    [<JsonPropertyName "version">] Version: int
    [<JsonPropertyName "message">] Message: Message option
    [<JsonPropertyName "app_permissions">] AppPermissions: string
    [<JsonPropertyName "locale">] Locale: string option
    [<JsonPropertyName "guild_locale">] GuildLocale: string option
    [<JsonPropertyName "entitlements">] Entitlements: Entitlement list
    [<JsonPropertyName "authorizing_integration_owners">] AuthorizingIntegrationOwners: (ApplicationIntegrationType * ApplicationIntegrationTypeConfiguration) seq
    [<JsonPropertyName "context">] Context: InteractionContextType option
}

// https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-object-application-command-data-structure
type ApplicationCommandData = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "name">] Name: string
    [<JsonPropertyName "type">] Type: ApplicationCommandType
    [<JsonPropertyName "resolved">] Resolved: ResolvedData option
    [<JsonPropertyName "options">] Options: ApplicationCommandInteractionDataOption list option
    [<JsonPropertyName "guild_id">] GuildId: string option
    [<JsonPropertyName "target_id">] TargetId: string option
}

// TODO: ApplicationCommandInteractionDataOption can be partial when responding to APPLICATION_COMMAND_AUTOCOMPLETE

// https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-object-message-component-data-structure
type MessageComponentData = {
    [<JsonPropertyName "custom_id">] CustomId: string
    [<JsonPropertyName "component_type">] ComponentType: ComponentType
    [<JsonPropertyName "values">] Values: SelectMenuOption list option
    [<JsonPropertyName "esolved">] Resolved: ResolvedData option
}

// https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-object-modal-submit-data-structure
type ModalSubmitData = {
    [<JsonPropertyName "custom_id">] CustomId: string
    [<JsonPropertyName "components">] Components: Component list
}

[<JsonConverter(typeof<InteractionData.Converter>)>]
type InteractionData =
    | APPLICATION_COMMAND of ApplicationCommandData
    | MESSAGE_COMPONENT of MessageComponentData
    | MODAL_SUBMIT of ModalSubmitData

module InteractionData =
    type Converter () =
        inherit JsonConverter<InteractionData> ()

        override _.Read (reader, _, _) =
            let success, document = JsonDocument.TryParseValue &reader
            if not success then JsonException.raise "Failed to parse JSON document"

            let dataType =
                let isApplicationCommand = document.RootElement.TryGetProperty "name" |> fst
                let isMessageComponent = document.RootElement.TryGetProperty "component_type" |> fst
                let isModalSubmit = document.RootElement.TryGetProperty "components" |> fst
                // Using properties that only exist on specific types to check

                match isApplicationCommand, isMessageComponent, isModalSubmit with
                | true, false, false -> InteractionDataType.APPLICATION_COMMAND
                | false, true, false -> InteractionDataType.MESSAGE_COMPONENT
                | false, false, true -> InteractionDataType.MODAL_SUBMIT
                | _ -> JsonException.raise "Unexpected InteractionData provided"

            let json = document.RootElement.GetRawText()

            match dataType with
            | InteractionDataType.APPLICATION_COMMAND -> InteractionData.APPLICATION_COMMAND <| Json.deserializeF<ApplicationCommandData> json
            | InteractionDataType.MESSAGE_COMPONENT -> InteractionData.MESSAGE_COMPONENT <| Json.deserializeF<MessageComponentData> json
            | InteractionDataType.MODAL_SUBMIT -> InteractionData.MODAL_SUBMIT <| Json.deserializeF<ModalSubmitData> json

        override _.Write (writer, value, _) =
            match value with
            | InteractionData.APPLICATION_COMMAND a -> Json.serializeF a
            | InteractionData.MESSAGE_COMPONENT m -> Json.serializeF m
            | InteractionData.MODAL_SUBMIT m -> Json.serializeF m
            |> writer.WriteRawValue

// https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-object-resolved-data-structure
type ResolvedData = {
    [<JsonPropertyName "users">] Users: (string * User) seq
    [<JsonPropertyName "members">] Members: (string * PartialGuildMember) seq // Missing user, deaf, mute
    [<JsonPropertyName "roles">] Roles: (string * Role) seq
    [<JsonPropertyName "channels">] Channels: (string * PartialChannel) seq // Only have id, name, type, permissions (and threads have thread_metadata, parent_id)
    [<JsonPropertyName "messages">] Messages: (string * PartialMessage) seq
    [<JsonPropertyName "attachments">] Attachments: (string * Attachment) seq
}

// https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-object-application-command-interaction-data-option-structure
type ApplicationCommandInteractionDataOption = {
    [<JsonPropertyName "name">] Name: string
    [<JsonPropertyName "type">] Type: ApplicationCommandOptionType
    [<JsonPropertyName "value">] Value: CommandInteractionDataOptionValue option
    [<JsonPropertyName "options">] Options: ApplicationCommandInteractionDataOption list option
    [<JsonPropertyName "focused">] Focused: bool option
}

// https://discord.com/developers/docs/interactions/receiving-and-responding#message-interaction-object-message-interaction-structure
type MessageInteraction = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "type">] Type: InteractionType
    [<JsonPropertyName "name">] Name: string
    [<JsonPropertyName "user">] User: User
    [<JsonPropertyName "member">] Member: PartialGuildMember option
}

// https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-response-object-messages
type MessageInteractionCallbackData = {
    [<JsonPropertyName "tts">] Tts: bool option
    [<JsonPropertyName "content">] Content: string option
    [<JsonPropertyName "embeds">] Embeds: Embed list option
    [<JsonPropertyName "allowed_mentions">] AllowedMentions: AllowedMentions option
    [<JsonPropertyName "flags">] Flags: int option
    [<JsonPropertyName "components">] Components: Component list option
    [<JsonPropertyName "attachments">] Attachments: PartialAttachment list option
    [<JsonPropertyName "poll">] Poll: Poll option
}

// https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-response-object-autocomplete
type AutocompleteInteractionCallbackData = {
    [<JsonPropertyName "choices">] Choices: ApplicationCommandOptionChoice list
}

// https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-response-object-modal
type ModalInteractionCallbackData = {
    [<JsonPropertyName "custom_id">] CustomId: string
    [<JsonPropertyName "title">] Title: string
    [<JsonPropertyName "components">] Components: Component list
}

// https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-response-object-interaction-response-structure
type InteractionResponse =
    | PONG                                    of unit
    | CHANNEL_MESSAGE_WITH_SOURCE             of MessageInteractionCallbackData
    | DEFERRED_CHANNEL_MESSAGE_WITH_SOURCE    of unit
    | DEFERRED_UPDATE_MESSAGE                 of unit
    | UPDATE_MESSAGE                          of MessageInteractionCallbackData
    | APPLICATION_COMMAND_AUTOCOMPLETE_RESULT of AutocompleteInteractionCallbackData
    | MODAL                                   of ModalInteractionCallbackData
    | LAUNCH_ACTIVITY                         of unit

module InteractionResponse =
    // https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-response-object-interaction-response-structure
    type Payload = {
        [<JsonPropertyName "type">] Type: InteractionCallbackType
        [<JsonPropertyName "data">] Data: obj option
    }

    // TODO: Test if serialisation works with data defined as obj (saves having to write an annoying json converter for them)

    type Converter () =
        inherit JsonConverter<InteractionResponse> ()

        override _.Read (reader, _, _) =
            let success, document = JsonDocument.TryParseValue &reader
            if not success then JsonException.raise "Failed to parse JSON document"

            let callbackType = document.RootElement.GetProperty "type" |> _.GetInt32() |> enum<InteractionCallbackType>
            let json = document.RootElement.GetRawText()
            
            match callbackType with
            | InteractionCallbackType.PONG -> InteractionResponse.PONG ()
            | InteractionCallbackType.CHANNEL_MESSAGE_WITH_SOURCE -> InteractionResponse.CHANNEL_MESSAGE_WITH_SOURCE <| Json.deserializeF<MessageInteractionCallbackData> json
            | InteractionCallbackType.DEFERRED_CHANNEL_MESSAGE_WITH_SOURCE -> InteractionResponse.DEFERRED_CHANNEL_MESSAGE_WITH_SOURCE ()
            | InteractionCallbackType.DEFERRED_UPDATE_MESSAGE -> InteractionResponse.DEFERRED_UPDATE_MESSAGE ()
            | InteractionCallbackType.UPDATE_MESSAGE -> InteractionResponse.UPDATE_MESSAGE <| Json.deserializeF<MessageInteractionCallbackData> json
            | InteractionCallbackType.APPLICATION_COMMAND_AUTOCOMPLETE_RESULT -> InteractionResponse.APPLICATION_COMMAND_AUTOCOMPLETE_RESULT <| Json.deserializeF<AutocompleteInteractionCallbackData> json
            | InteractionCallbackType.MODAL -> InteractionResponse.MODAL <| Json.deserializeF<ModalInteractionCallbackData> json
            | InteractionCallbackType.LAUNCH_ACTIVITY -> InteractionResponse.LAUNCH_ACTIVITY ()
            | _ -> JsonException.raise "Unexpected InteractionCallbackType provided"

        override _.Write (writer, value, _) =
            match value with
            | InteractionResponse.PONG _ -> { Type = InteractionCallbackType.PONG; Data = None }
            | InteractionResponse.CHANNEL_MESSAGE_WITH_SOURCE m -> { Type = InteractionCallbackType.CHANNEL_MESSAGE_WITH_SOURCE; Data = Some m }
            | InteractionResponse.DEFERRED_CHANNEL_MESSAGE_WITH_SOURCE _ -> { Type = InteractionCallbackType.DEFERRED_CHANNEL_MESSAGE_WITH_SOURCE; Data = None }
            | InteractionResponse.DEFERRED_UPDATE_MESSAGE _ -> { Type = InteractionCallbackType.DEFERRED_UPDATE_MESSAGE; Data = None }
            | InteractionResponse.UPDATE_MESSAGE m -> { Type = InteractionCallbackType.UPDATE_MESSAGE; Data = Some m }
            | InteractionResponse.APPLICATION_COMMAND_AUTOCOMPLETE_RESULT a -> { Type = InteractionCallbackType.APPLICATION_COMMAND_AUTOCOMPLETE_RESULT; Data = Some a }
            | InteractionResponse.MODAL m -> { Type = InteractionCallbackType.MODAL; Data = Some m }
            | InteractionResponse.LAUNCH_ACTIVITY _ -> { Type = InteractionCallbackType.LAUNCH_ACTIVITY; Data = None }
            |> Json.serializeF
            |> writer.WriteRawValue

// https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-callback-interaction-callback-response-object
type InteractionCallbackResponse = {
    [<JsonPropertyName "interaction">] Interaction: InteractionCallback
    [<JsonPropertyName "resource">] Resource: InteractionCallbackResource option
}

// https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-callback-interaction-callback-object
type InteractionCallback = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "type">] Type: InteractionType
    [<JsonPropertyName "activity_instance_id">] ActivityInstanceId: string option
    [<JsonPropertyName "response_message_id">] ResponseMessageId: string option
    [<JsonPropertyName "response_message_loading">] ResponseMessageLoading: bool option
    [<JsonPropertyName "response_message_ephemeral">] ResponseMessageEphemeral: bool option
}

// https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-callback-interaction-callback-resource-object
type InteractionCallbackResource = {
    [<JsonPropertyName "type">] Type: InteractionCallbackType
    [<JsonPropertyName "activity_instance">] ActivityInstance: InteractionCallbackActivityInstance option
    [<JsonPropertyName "message">] Message: Message option
}

// https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-callback-interaction-callback-activity-instance-resource
type InteractionCallbackActivityInstance = {
    [<JsonPropertyName "id">] Id: string
}

// TODO: Refactor how Structures/* and Events types are implemented and put the structures here (basing off above)

// ----- Interactions: Application Commands -----

// https://discord.com/developers/docs/interactions/application-commands#application-command-object-application-command-structure
type ApplicationCommand = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "type">] Type: ApplicationCommandType option
    [<JsonPropertyName "application_id">] ApplicationId: string
    [<JsonPropertyName "guild_id">] GuildId: string option
    [<JsonPropertyName "name">] Name: string
    [<JsonPropertyName "name_localizations">] NameLocalizations: (string * string) seq option
    [<JsonPropertyName "description">] Description: string
    [<JsonPropertyName "description_localizations">] DescriptionLocalizations: (string * string) seq option
    [<JsonPropertyName "options">] Options: ApplicationCommandOption list option
    [<JsonPropertyName "default_member_permissions">] DefaultMemberPermissions: string option
    [<JsonPropertyName "nsfw">] Nsfw: bool option
    [<JsonPropertyName "integration_types">] IntegrationTypes: ApplicationIntegrationType list option
    [<JsonPropertyName "contexts">] Contexts: InteractionContextType list option
    [<JsonPropertyName "version">] Version: string
    [<JsonPropertyName "handler">] Handler: ApplicationCommandHandlerType option

    // Only present under certain conditions: https://discord.com/developers/docs/interactions/application-commands#retrieving-localized-commands
    [<JsonPropertyName "name_localized">] NameLocalized: string option
    [<JsonPropertyName "description_localized">] DescriptionLocalized: string option

    // TODO: Create separate type with these special properties? Like invite metadata?
}

// https://discord.com/developers/docs/interactions/application-commands#application-command-object-application-command-option-structure
type ApplicationCommandOption = {
    [<JsonPropertyName "type">] Type: ApplicationCommandOptionType
    [<JsonPropertyName "name">] Name: string
    [<JsonPropertyName "name_localizations">] NameLocalizations: (string * string) seq option
    [<JsonPropertyName "description">] Description: string
    [<JsonPropertyName "description_localizations">] DescriptionLocalizations: (string * string) seq option
    [<JsonPropertyName "required">] Required: bool option
    [<JsonPropertyName "choices">] Choices: ApplicationCommandOptionChoice list option
    [<JsonPropertyName "options">] Options: ApplicationCommandOption list option
    [<JsonPropertyName "channel_types">] ChannelTypes: ChannelType list option
    [<JsonPropertyName "min_value">] MinValue: ApplicationCommandMinValue option
    [<JsonPropertyName "max_value">] MaxValue: ApplicationCommandMaxValue option
    [<JsonPropertyName "min_length">] MinLength: int option
    [<JsonPropertyName "max_length">] MaxLength: int option
    [<JsonPropertyName "autocomplete">] Autocomplete: bool option
}

// TODO: Create DU for different types of application command option

// https://discord.com/developers/docs/interactions/application-commands#application-command-object-application-command-option-choice-structure
type ApplicationCommandOptionChoice = {
    [<JsonPropertyName "name">] Name: string
    [<JsonPropertyName "name_localizations">] NameLocalizations: (string * string) seq option
    [<JsonPropertyName "value">] Value: ApplicationCommandOptionChoiceValue
}

// TODO: Handle value type based on parent application command option type

// https://discord.com/developers/docs/interactions/application-commands#application-command-permissions-object-guild-application-command-permissions-structure
type GuildApplicationCommandPermissions = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "application_id">] ApplicationId: string
    [<JsonPropertyName "guild_id">] GuildId: string
    [<JsonPropertyName "permissions">] Permissions: ApplicationCommandPermission list
}

// https://discord.com/developers/docs/interactions/application-commands#application-command-permissions-object-application-command-permissions-structure
type ApplicationCommandPermission = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "type">] Type: ApplicationCommandPermissionType
    [<JsonPropertyName "permission">] Permission: bool
}

// TODO: Add application command permission constants somewhere
// TODO: Handle maximum subcommand depth (is this even possible?)
// TODO: Implement locale fallbacks

// ----- Interactions: Message Components -----

// https://discord.com/developers/docs/interactions/message-components#action-rows
type ActionRow = {
    [<JsonPropertyName "type">] Type: ComponentType
    [<JsonPropertyName "components">] Components: Component list
}

// https://discord.com/developers/docs/interactions/message-components#button-object-button-structure
type Button = {
    [<JsonPropertyName "type">] Type: ComponentType
    [<JsonPropertyName "style">] Style: ButtonStyle
    [<JsonPropertyName "label">] Label: string
    [<JsonPropertyName "emoji">] Emoji: Emoji option
    [<JsonPropertyName "custom_id">] CustomId: string option
    [<JsonPropertyName "url">] Url: string option
    [<JsonPropertyName "disabled">] Disabled: bool option
}

// https://discord.com/developers/docs/interactions/message-components#select-menu-object-select-menu-structure
type SelectMenu = {
    [<JsonPropertyName "type">] Type: ComponentType
    [<JsonPropertyName "custom_id">] CustomId: string
    [<JsonPropertyName "options">] Options: SelectMenuOption list option
    [<JsonPropertyName "channel_types">] ChannelTypes: ChannelType list option
    [<JsonPropertyName "placeholder">] Placeholder: string option
    [<JsonPropertyName "default_values">] DefaultValues: SelectMenuDefaultValue option
    [<JsonPropertyName "min_values">] MinValues: int option
    [<JsonPropertyName "max_values">] MaxValues: int option
    [<JsonPropertyName "disabled">] Disabled: bool option
}

// https://discord.com/developers/docs/interactions/message-components#select-menu-object-select-option-structure
type SelectMenuOption = {
    [<JsonPropertyName "label">] Label: string
    [<JsonPropertyName "value">] Value: string
    [<JsonPropertyName "description">] Description: string option
    [<JsonPropertyName "emoji">] Emoji: Emoji option
    [<JsonPropertyName "default">] Default: bool option
}

// https://discord.com/developers/docs/interactions/message-components#select-menu-object-select-default-value-structure
type SelectMenuDefaultValue = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "type">] Type: string
}

// https://discord.com/developers/docs/interactions/message-components#text-input-object-text-input-structure
type TextInput = {
    [<JsonPropertyName "type">] Type: ComponentType
    [<JsonPropertyName "custom_id">] CustomId: string
    [<JsonPropertyName "style">] Style: TextInputStyle
    [<JsonPropertyName "label">] Label: string
    [<JsonPropertyName "min_length">] MinLength: int option
    [<JsonPropertyName "max_length">] MaxLength: int option
    [<JsonPropertyName "required">] Required: bool option
    [<JsonPropertyName "value">] Value: string option
    [<JsonPropertyName "placeholder">] Placeholder: string option
}

[<JsonConverter(typeof<Component.Converter>)>]
type Component =
    | ACTION_ROW of ActionRow
    | BUTTON of Button
    | SELECT_MENU of SelectMenu
    | TEXT_INPUT of TextInput

module Component =
    type Converter () =
        inherit JsonConverter<Component> ()

        override _.Read (reader, _, _) =
            let success, document = JsonDocument.TryParseValue &reader
            if not success then JsonException.raise "Unable to parse JSON document"

            let componentType = document.RootElement.GetProperty "type" |> _.GetInt32() |> enum<ComponentType>
            let json = document.RootElement.GetRawText()

            match componentType with
            | ComponentType.ACTION_ROW -> Component.ACTION_ROW <| Json.deserializeF<ActionRow> json
            | ComponentType.BUTTON -> Component.BUTTON <| Json.deserializeF<Button> json
            | ComponentType.STRING_SELECT
            | ComponentType.USER_SELECT
            | ComponentType.ROLE_SELECT
            | ComponentType.MENTIONABLE_SELECT
            | ComponentType.CHANNEL_SELECT -> Component.SELECT_MENU <| Json.deserializeF<SelectMenu> json
            | ComponentType.TEXT_INPUT -> Component.TEXT_INPUT <| Json.deserializeF<TextInput> json
            | _ -> raise (JsonException "Unexpected ComponentType provided")

        override _.Write (writer, value, _) =
            match value with
            | Component.ACTION_ROW a -> Json.serializeF a
            | Component.BUTTON b -> Json.serializeF b
            | Component.SELECT_MENU s -> Json.serializeF s
            | Component.TEXT_INPUT t -> Json.serializeF t
            |> writer.WriteRawValue

// ----- Events: Using Gateway -----

// https://discord.com/developers/docs/events/gateway#session-start-limit-object-session-start-limit-structure
type SessionStartLimit = {
    [<JsonPropertyName "total">] Total: int
    [<JsonPropertyName "remaining">] Remaining: int
    [<JsonPropertyName "reset_after">] ResetAfter: int
    [<JsonPropertyName "max_concurrency">] MaxConcurrency: int
}

// ----- Events: Gateway Events -----

// https://discord.com/developers/docs/topics/gateway-events#identify-identify-connection-properties
type IdentifyConnectionProperties = {
    [<JsonPropertyName "os">] OperatingSystem: string
    [<JsonPropertyName "browser">] Browser: string
    [<JsonPropertyName "device">] Device: string
}

// https://discord.com/developers/docs/events/gateway-events#client-status-object
[<JsonConverter(typeof<ClientStatus.Converter>)>]
type ClientStatus = {
    [<JsonPropertyName "desktop">] Desktop: Status
    [<JsonPropertyName "mobile">] Mobile: Status
    [<JsonPropertyName "web">] Web: Status
}

module ClientStatus =
    type Payload = {
        [<JsonPropertyName "desktop">] Desktop: string option
        [<JsonPropertyName "mobile">] Mobile: string option
        [<JsonPropertyName "web">] Web: string option
    }

    type Converter () =
        inherit JsonConverter<ClientStatus> ()

        override _.Read (reader, _, _) =
            let success, document = JsonDocument.TryParseValue &reader
            if not success then JsonException.raise "Failed to parse JSON document"

            let statusFromOption (str: string option) =
                match str with
                | None -> Status.OFFLINE
                | Some str ->
                    match Status.fromString str with
                    | Some Status.ONLINE -> Status.ONLINE
                    | Some Status.IDLE -> Status.IDLE
                    | Some Status.DND -> Status.DND
                    | _ -> Status.OFFLINE

            let payload = document.Deserialize<Payload>()

            {
                Desktop = statusFromOption payload.Desktop
                Mobile = statusFromOption payload.Mobile
                Web = statusFromOption payload.Web
            }

        override _.Write (writer, value, _) =
            let statusToOption (status: Status) =
                match status with
                | Status.ONLINE -> Some "online"
                | Status.IDLE -> Some "idle"
                | Status.DND -> Some "dnd"
                | _ -> None

            {
                Desktop = statusToOption value.Desktop
                Mobile = statusToOption value.Mobile
                Web = statusToOption value.Web
            }
            |> Json.serializeF
            |> writer.WriteRawValue

// https://discord.com/developers/docs/topics/gateway-events#activity-object-activity-structure
type Activity = {
    [<JsonPropertyName "name">] Name: string
    [<JsonPropertyName "type">] Type: ActivityType
    [<JsonPropertyName "url">] Url: string option
    [<JsonPropertyName "created_at">] [<JsonConverter(typeof<JsonConverter.UnixEpoch>)>] CreatedAt: DateTime option
    [<JsonPropertyName "timestamps">] Timestamps: ActivityTimestamps option
    [<JsonPropertyName "application_id">] ApplicationId: string option
    [<JsonPropertyName "details">] Details: string option
    [<JsonPropertyName "state">] State: string option
    [<JsonPropertyName "emoji">] Emoji: ActivityEmoji option
    [<JsonPropertyName "party">] Party: ActivityParty option
    [<JsonPropertyName "assets">] Assets: ActivityAssets option
    [<JsonPropertyName "secrets">] Secrets: ActivitySecrets option
    [<JsonPropertyName "instance">] Instance: bool option
    [<JsonPropertyName "flags">] Flags: int option
    [<JsonPropertyName "buttons">] Buttons: ActivityButton list option
}

// https://discord.com/developers/docs/events/gateway-events#activity-object-activity-timestamps
type ActivityTimestamps = {
    [<JsonPropertyName "start">] [<JsonConverter(typeof<JsonConverter.UnixEpoch>)>] Start: DateTime option
    [<JsonPropertyName "end">] [<JsonConverter(typeof<JsonConverter.UnixEpoch>)>] End: DateTime option
}

// https://discord.com/developers/docs/events/gateway-events#activity-object-activity-emoji
type ActivityEmoji = {
    [<JsonPropertyName "name">] Name: string
    [<JsonPropertyName "id">] Id: string option
    [<JsonPropertyName "animated">] Animated: bool option
}

// https://discord.com/developers/docs/events/gateway-events#activity-object-activity-party
type ActivityParty = {
    [<JsonPropertyName "id">] Id: string option
    [<JsonPropertyName "size">] Size: (int * int) option
}

// TODO: Make custom serializer for above to make a record with current and max size properties

// https://discord.com/developers/docs/events/gateway-events#activity-object-activity-assets
type ActivityAssets = {
    [<JsonPropertyName "large_image">] LargeImage: string option
    [<JsonPropertyName "large_text">] LargeText: string option
    [<JsonPropertyName "small_image">] SmallImage: string option
    [<JsonPropertyName "small_text">] SmallText: string option
}

// TODO: Handle activity asset images as documented

// https://discord.com/developers/docs/events/gateway-events#activity-object-activity-secrets
type ActivitySecrets = {
    [<JsonPropertyName "join">] Join: string option
    [<JsonPropertyName "spectate">] Spectate: string option
    [<JsonPropertyName "matcch">] Match: string option
}

// https://discord.com/developers/docs/events/gateway-events#activity-object-activity-buttons
type ActivityButton = {
    [<JsonPropertyName "label">] Label: string
    [<JsonPropertyName "url">] Url: string
}

// TODO: Move gateway events here from Structures/GatewayEvents.fs

// ----- Events: Webhook Events -----

// TODO: Move webhook events here from Structures/WebhookEvents.fs

// ----- Resources: Application -----

// https://discord.com/developers/docs/resources/application#application-object-application-structure
type Application = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "name">] Name: string
    [<JsonPropertyName "icon">] Icon: string option
    [<JsonPropertyName "description">] Description: string
    [<JsonPropertyName "rpc_origins">] RpcOrigins: string list option
    [<JsonPropertyName "bot_public">] BotPublic: bool
    [<JsonPropertyName "bot_require_code_grant">] BotRequireCodeGrant: bool
    [<JsonPropertyName "bot">] Bot: PartialUser option
    [<JsonPropertyName "terms_of_service_url">] TermsOfServiceUrl: string option
    [<JsonPropertyName "privacy_policy_url">] PrivacyPolicyUrl: string option
    [<JsonPropertyName "owner">] Owner: PartialUser option
    [<JsonPropertyName "verify_key">] VerifyKey: string
    [<JsonPropertyName "team">] Team: Team option
    [<JsonPropertyName "guild_id">] GuildId: string option
    [<JsonPropertyName "guild">] Guild: PartialGuild option
    [<JsonPropertyName "primary_sku_id">] PrimarySkuId: string option
    [<JsonPropertyName "slug">] Slug: string option
    [<JsonPropertyName "cover_image">] CoverImage: string option
    [<JsonPropertyName "flags">] Flags: int option
    [<JsonPropertyName "approximate_guild_count">] ApproximateGuildCount: int option
    [<JsonPropertyName "approximate_user_install_count">] ApproximateUserInstallCount: int option
    [<JsonPropertyName "redirect_uris">] RedirectUris: string list option
    [<JsonPropertyName "interactions_endpoint_url">] InteractionsEndpointUrl: string option
    [<JsonPropertyName "role_connections_verification_url">] RoleConnectionsVerificationUrl: string option
    [<JsonPropertyName "event_webhooks_url">] EventWebhooksUrl: string option
    [<JsonPropertyName "event_webhooks_status">] EventWebhooksStatus: WebhookEventStatus
    [<JsonPropertyName "event_webhooks_types">] EventWebhooksTypes: WebhookEventType list option
    [<JsonPropertyName "tags">] Tags: string list option
    [<JsonPropertyName "install_params">] InstallParams: InstallParams option
    [<JsonPropertyName "integration_types_config">] IntegrationTypesConfig: Map<ApplicationIntegrationType, ApplicationIntegrationTypeConfiguration> option
    [<JsonPropertyName "custom_install_url">] CustomInstallUrl: string option
}

and PartialApplication = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "name">] Name: string option
    [<JsonPropertyName "icon">] Icon: string option
    [<JsonPropertyName "description">] Description: string option
    [<JsonPropertyName "rpc_origins">] RpcOrigins: string list option
    [<JsonPropertyName "bot_public">] BotPublic: bool option
    [<JsonPropertyName "bot_require_code_grant">] BotRequireCodeGrant: bool option
    [<JsonPropertyName "bot">] Bot: PartialUser option
    [<JsonPropertyName "terms_of_Service_url">] TermsOfServiceUrl: string option
    [<JsonPropertyName "privacy_policy_url">] PrivacyPolicyUrl: string option
    [<JsonPropertyName "owner">] Owner: PartialUser option
    [<JsonPropertyName "verify_key">] VerifyKey: string option
    [<JsonPropertyName "team">] Team: Team option
    [<JsonPropertyName "guild_id">] GuildId: string option
    [<JsonPropertyName "guild">] Guild: PartialGuild option
    [<JsonPropertyName "primary_sku_id">] PrimarySkuId: string option
    [<JsonPropertyName "slug">] Slug: string option
    [<JsonPropertyName "cover_image">] CoverImage: string option
    [<JsonPropertyName "flags">] Flags: int option
    [<JsonPropertyName "approximate_guild_count">] ApproximateGuildCount: int option
    [<JsonPropertyName "redirect_uris">] RedirectUris: string list option
    [<JsonPropertyName "interactions_endpoint_url">] InteractionsEndpointUrl: string option
    [<JsonPropertyName "role_connections_verification_url">] RoleConnectionsVerificationUrl: string option
    [<JsonPropertyName "tags">] Tags: string list option
    [<JsonPropertyName "install_params">] InstallParams: InstallParams option
    [<JsonPropertyName "integration_types_config">] IntegrationTypesConfig: Map<ApplicationIntegrationType, ApplicationIntegrationTypeConfiguration> option
    [<JsonPropertyName "custom_install_url">] CustomInstallUrl: string option
}

// https://discord.com/developers/docs/resources/application#application-object-application-integration-type-configuration-object
type ApplicationIntegrationTypeConfiguration = {
    [<JsonPropertyName "oauth2_install_params">] OAuth2InstallParams: InstallParams option
}

// https://discord.com/developers/docs/resources/application#install-params-object-install-params-structure
type InstallParams = {
    [<JsonPropertyName "scopes">] Scopes: string list
    [<JsonPropertyName "permissions">] Permissions: string
}

// https://discord.com/developers/docs/resources/application#get-application-activity-instance-activity-instance-object
type ActivityInstance = {
    [<JsonPropertyName "application_id">] ApplicationId: string
    [<JsonPropertyName "instance_id">] InstanceId: string
    [<JsonPropertyName "launch_id">] LaunchId: string
    [<JsonPropertyName "location">] Location: ActivityLocation
    [<JsonPropertyName "users">] Users: string list
}

// https://discord.com/developers/docs/resources/application#get-application-activity-instance-activity-location-object
type ActivityLocation = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "kind">] Kind: ActivityLocationKind
    [<JsonPropertyName "scopes">] ChannelId: string
    [<JsonPropertyName "guild_id">] GuildId: string option
}

// ----- Resources: Application Role Connection Metadata -----

// https://discord.com/developers/docs/resources/application-role-connection-metadata#application-role-connection-metadata-object-application-role-connection-metadata-structure
type ApplicationRoleConnectionMetadata = {
    [<JsonPropertyName "type">] Type: ApplicationRoleConnectionMetadataType
    [<JsonPropertyName "key">] Key: string
    [<JsonPropertyName "name">] Name: string
    [<JsonPropertyName "name_localizations">] NameLocalizations: (string * string) seq option
    [<JsonPropertyName "description">] Description: string
    [<JsonPropertyName "description_localizations">] DescriptionLocalizations: (string * string) option
}

// ----- Resources: Audit Log -----

// https://discord.com/developers/docs/resources/audit-log#audit-log-object-audit-log-structure
type AuditLog = {
    [<JsonPropertyName "application_commands">] ApplicationCommands: ApplicationCommand list
    [<JsonPropertyName "audit_log_entries">] AuditLogEntries: AuditLogEntry list
    [<JsonPropertyName "auto_moderation_rules">] AutoModerationRules: AutoModerationRule list
    [<JsonPropertyName "guild_scheduled_events">] GuildScheduledEvents: GuildScheduledEvent list
    [<JsonPropertyName "integrations">] Integrations: PartialIntegration list
    [<JsonPropertyName "threads">] Threads: Channel list
    [<JsonPropertyName "users">] Users: User list
    [<JsonPropertyName "webhooks">] Webhooks: Webhook list
}

// https://discord.com/developers/docs/resources/audit-log#audit-log-entry-object-audit-log-entry-structure
type AuditLogEntry = {
    [<JsonPropertyName "target_id">] TargetId: string option
    [<JsonPropertyName "changes">] Changes: AuditLogChange list option
    [<JsonPropertyName "user_id">] UserId: string option
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "action_type">] ActionType: AuditLogEventType
    [<JsonPropertyName "options">] Options: AuditLogEntryOptionalInfo option
    [<JsonPropertyName "reason">] Reason: string option
}

// https://discord.com/developers/docs/resources/audit-log#audit-log-entry-object-optional-audit-entry-info
type AuditLogEntryOptionalInfo = {
    [<JsonPropertyName "application_id">] ApplicationId: string option
    [<JsonPropertyName "auto_moderation_rule_name">] AutoModerationRuleName: string option
    [<JsonPropertyName "auto_moderation_rule_trigger_type">] AutoModerationRuleTriggerType: string option
    [<JsonPropertyName "channel_id">] ChannelId: string option
    [<JsonPropertyName "count">] Count: string option
    [<JsonPropertyName "delete_member_days">] DeleteMemberDays: string option
    [<JsonPropertyName "id">] Id: string option
    [<JsonPropertyName "members_removed">] MembersRemoved: string option
    [<JsonPropertyName "message_id">] MessageId: string option
    [<JsonPropertyName "role_name">] RoleName: string option
    [<JsonPropertyName "type">] Type: string option
    [<JsonPropertyName "integration_type">] IntegrationType: string option
}

// TODO: Create DU for above for specific event types

// https://discord.com/developers/docs/resources/audit-log#audit-log-change-object
type AuditLogChange = {
    [<JsonPropertyName "new_value">] NewValue: obj option
    [<JsonPropertyName "old_value">] OldValue: obj option
    [<JsonPropertyName "key">] Key: string
}

// TODO: Handle exceptions for keys as documented, and consider if obj is most appropriate for old and new value

// ----- Resources: Auto Moderation -----

// https://discord.com/developers/docs/resources/auto-moderation#auto-moderation-rule-object-auto-moderation-rule-structure
type AutoModerationRule = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "guild_id">] GuildId: string
    [<JsonPropertyName "name">] Name: string
    [<JsonPropertyName "creator_id">] CreatorId: string
    [<JsonPropertyName "event_type">] EventType: AutoModerationEventType
    [<JsonPropertyName "trigger_type">] TriggerType: AutoModerationTriggerType
    [<JsonPropertyName "trigger_metadata">] TriggerMetadata: TriggerMetadata
    [<JsonPropertyName "actions">] Actions: AutoModerationAction list
    [<JsonPropertyName "enabled">] nabled: bool
    [<JsonPropertyName "exempt_roles">] ExemptRoles: string list
    [<JsonPropertyName "exempt_channels">] ExemptChannels: string list
}

// https://discord.com/developers/docs/resources/auto-moderation#auto-moderation-rule-object-trigger-metadata
type TriggerMetadata = {
    [<JsonPropertyName "keyword_filter">] KeywordFilter: string list option
    [<JsonPropertyName "regex_patterns">] RegexPatterns: string list option
    [<JsonPropertyName "presets">] Presets: AutoModerationKeywordPreset option
    [<JsonPropertyName "allow_list">] AllowList: string list option
    [<JsonPropertyName "mention_total_limit">] MentionTotalLimit: int option
    [<JsonPropertyName "mention_raid_protection_enabled">] MentionRaidProtectionEnabled: bool option
}

// TODO: Handle trigger metadata field limits?

// https://discord.com/developers/docs/resources/auto-moderation#auto-moderation-action-object
type AutoModerationAction = {
    [<JsonPropertyName "type">] Type: AutoModerationActionType
    [<JsonPropertyName "metadata">] Metadata: ActionMetadata option
}

// https://discord.com/developers/docs/resources/auto-moderation#auto-moderation-action-object-action-metadata
type ActionMetadata = {
    [<JsonPropertyName "channel_id">] ChannelId: string option
    [<JsonPropertyName "duration_seconds">] DurationSeconds: int option
    [<JsonPropertyName "custom_message">] CustomMessage: string option
}

// ----- Resources: Channel -----

// https://discord.com/developers/docs/resources/channel#channel-object-channel-structure
type BaseChannel = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "type">] Type: ChannelType
    [<JsonPropertyName "guild_id">] GuildId: string option
    [<JsonPropertyName "position">] Position: int option
    [<JsonPropertyName "permission_overwrites">] PermissionOverwrites: PermissionOverwrite list option
    [<JsonPropertyName "name">] Name: string option
    [<JsonPropertyName "topic">] Topic: string option
    [<JsonPropertyName "nsfw">] Nsfw: bool option
    [<JsonPropertyName "last_message_id">] LastMessageId: string option
    [<JsonPropertyName "bitrate">] Bitrate: int option
    [<JsonPropertyName "user_limit">] UserLimit: int option
    [<JsonPropertyName "rate_limit_per_user">] RateLimitPerUser: int option
    [<JsonPropertyName "recipients">] Recipients: User list option
    [<JsonPropertyName "icon">] Icon: string option
    [<JsonPropertyName "owner_id">] OwnerId: string option
    [<JsonPropertyName "application_id">] ApplicationId: string option
    [<JsonPropertyName "managed">] Managed: bool option
    [<JsonPropertyName "parent_id">] ParentId: string option
    [<JsonPropertyName "last_pin_timestamp">] LastPinTimestamp: DateTime option
    [<JsonPropertyName "rtc_region">] RtcRegion: string option
    [<JsonPropertyName "video_quality_mode">] VideoQualityMode: VideoQualityMode option
    [<JsonPropertyName "message_count">] MessageCount: int option
    [<JsonPropertyName "member_count">] MemberCount: int option
    [<JsonPropertyName "thread_metadata">] ThreadMetadata: ThreadMetadata option
    [<JsonPropertyName "member">] Member: ThreadMember option
    [<JsonPropertyName "default_auto_archive_duration">] DefaultAutoArchiveDuration: AutoArchiveDuration option
    [<JsonPropertyName "permissions">] Permissions: string option
    [<JsonPropertyName "flags">] Flags: int option
    [<JsonPropertyName "total_messages_sent">] TotalMessagesSent: int option
    [<JsonPropertyName "available_tags">] AvailableTags: ForumTag list option
    [<JsonPropertyName "applied_tags">] AppliedTags: int list option
    [<JsonPropertyName "default_reaction_emoji">] DefaultReactionEmoji: DefaultReaction option
    [<JsonPropertyName "default_thread_rate_limit_per_user">] DefaultThreadRateLimitPerUser: int option
    [<JsonPropertyName "default_sort_order">] DefaultSortOrder: ChannelSortOrder option
    [<JsonPropertyName "default_forum_layout">] DefaultForumLayout: ForumLayout option
}

and PartialChannel = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "type">] Type: ChannelType option
    [<JsonPropertyName "guild_id">] GuildId: string option
    [<JsonPropertyName "position">] Position: int option
    [<JsonPropertyName "permission_overwrites">] PermissionOverwrites: PermissionOverwrite list option
    [<JsonPropertyName "name">] Name: string option
    [<JsonPropertyName "topic">] Topic: string option
    [<JsonPropertyName "nsfw">] Nsfw: bool option
    [<JsonPropertyName "last_message_id">] LastMessageId: string option
    [<JsonPropertyName "bitrate">] Bitrate: int option
    [<JsonPropertyName "user_limit">] UserLimit: int option
    [<JsonPropertyName "rate_limit_per_user">] RateLimitPerUser: int option
    [<JsonPropertyName "recipients">] Recipients: User list option
    [<JsonPropertyName "icon">] Icon: string option
    [<JsonPropertyName "owner_id">] OwnerId: string option
    [<JsonPropertyName "application_id">] ApplicationId: string option
    [<JsonPropertyName "managed">] Managed: bool option
    [<JsonPropertyName "parent_id">] ParentId: string option
    [<JsonPropertyName "last_pin_timestamp">] LastPinTimestamp: DateTime option
    [<JsonPropertyName "rtc_region">] RtcRegion: string option
    [<JsonPropertyName "video_quality_mode">] VideoQualityMode: VideoQualityMode option
    [<JsonPropertyName "message_count">] MessageCount: int option
    [<JsonPropertyName "member_count">] MemberCount: int option
    [<JsonPropertyName "thread_metadata">] ThreadMetadata: ThreadMetadata option
    [<JsonPropertyName "member">] Member: ThreadMember option
    [<JsonPropertyName "default_auto_archive_duration">] DefaultAutoArchiveDuration: AutoArchiveDuration option
    [<JsonPropertyName "permissions">] Permissions: string option
    [<JsonPropertyName "flags">] Flags: int option
    [<JsonPropertyName "total_messages_sent">] TotalMessagesSent: int option
    [<JsonPropertyName "available_tags">] AvailableTags: ForumTag list option
    [<JsonPropertyName "applied_tags">] AppliedTags: int list option
    [<JsonPropertyName "default_reaction_emoji">] DefaultReactionEmoji: DefaultReaction option
    [<JsonPropertyName "default_thread_rate_limit_per_user">] DefaultThreadRateLimitPerUser: int option
    [<JsonPropertyName "default_sort_order">] DefaultSortOrder: ChannelSortOrder option
    [<JsonPropertyName "default_forum_layout">] DefaultForumLayout: ForumLayout option
}

type GuildTextChannel = BaseChannel
type DmChannel = BaseChannel
type GuildVoiceChannel = BaseChannel
type GroupDmChannel = BaseChannel
type GuildCategoryChannel = BaseChannel
type GuildAnnouncementChannel = BaseChannel
type AnnouncementThreadChannel = BaseChannel
type PublicThreadChannel = BaseChannel
type PrivateThreadChannel = BaseChannel
type GuildStageVoiceChannel = BaseChannel
type GuildDirectoryChannel = BaseChannel
type GuildForumChannel = BaseChannel
type GuildMediaChannel = BaseChannel

// TODO: Define actual values available for these channel types rather than using the base

[<JsonConverter(typeof<ChannelConverter>)>]
type Channel =
    | GUILD_TEXT          of GuildTextChannel
    | DM                  of DmChannel
    | GUILD_VOICE         of GuildVoiceChannel
    | GROUP_DM            of GroupDmChannel
    | GUILD_CATEGORY      of GuildCategoryChannel
    | GUILD_ANNOUNCEMENT  of GuildAnnouncementChannel
    | ANNOUNCEMENT_THREAD of AnnouncementThreadChannel
    | PUBLIC_THREAD       of PublicThreadChannel
    | PRIVATE_THREAD      of PrivateThreadChannel
    | GUILD_STAGE_VOICE   of GuildStageVoiceChannel
    | GUILD_DIRECTORY     of GuildDirectoryChannel
    | GUILD_FORUM         of GuildForumChannel
    | GUILD_MEDIA         of GuildMediaChannel

type ChannelConverter () =
    inherit JsonConverter<Channel> ()

    override _.Read (reader, _, _) =
        let success, document = JsonDocument.TryParseValue &reader
        if not success then JsonException.raise "Failed to parse JSON document"

        let channelType = document.RootElement.GetProperty "type" |> _.GetInt32() |> enum<ChannelType>
        let json = document.RootElement.GetRawText()

        match channelType with
        | ChannelType.GUILD_TEXT -> Channel.GUILD_TEXT <| Json.deserializeF<GuildTextChannel> json
        | ChannelType.DM -> Channel.DM <| Json.deserializeF<DmChannel> json
        | ChannelType.GUILD_VOICE -> Channel.GUILD_VOICE <| Json.deserializeF<GuildVoiceChannel> json
        | ChannelType.GROUP_DM -> Channel.GROUP_DM <| Json.deserializeF<GroupDmChannel> json
        | ChannelType.GUILD_CATEGORY -> Channel.GUILD_CATEGORY <| Json.deserializeF<GuildCategoryChannel> json
        | ChannelType.GUILD_ANNOUNCEMENT -> Channel.GUILD_ANNOUNCEMENT <| Json.deserializeF<GuildAnnouncementChannel> json
        | ChannelType.ANNOUNCEMENT_THREAD -> Channel.ANNOUNCEMENT_THREAD <| Json.deserializeF<AnnouncementThreadChannel> json
        | ChannelType.PUBLIC_THREAD -> Channel.PUBLIC_THREAD <| Json.deserializeF<PublicThreadChannel> json
        | ChannelType.PRIVATE_THREAD -> Channel.PRIVATE_THREAD <| Json.deserializeF<PrivateThreadChannel> json
        | ChannelType.GUILD_STAGE_VOICE -> Channel.GUILD_STAGE_VOICE <| Json.deserializeF<GuildStageVoiceChannel> json
        | ChannelType.GUILD_DIRECTORY -> Channel.GUILD_DIRECTORY <| Json.deserializeF<GuildDirectoryChannel> json
        | ChannelType.GUILD_FORUM -> Channel.GUILD_FORUM <| Json.deserializeF<GuildForumChannel> json
        | ChannelType.GUILD_MEDIA -> Channel.GUILD_MEDIA <| Json.deserializeF<GuildMediaChannel> json
        | _ -> JsonException.raise "Unexpected ChannelType provided"

    override _.Write (writer, value, _) =
        match value with
        | Channel.GUILD_TEXT channel -> Json.serializeF channel
        | Channel.DM channel -> Json.serializeF channel
        | Channel.GUILD_VOICE channel -> Json.serializeF channel
        | Channel.GROUP_DM channel -> Json.serializeF channel
        | Channel.GUILD_CATEGORY channel -> Json.serializeF channel
        | Channel.GUILD_ANNOUNCEMENT channel -> Json.serializeF channel
        | Channel.ANNOUNCEMENT_THREAD channel -> Json.serializeF channel
        | Channel.PUBLIC_THREAD channel -> Json.serializeF channel
        | Channel.PRIVATE_THREAD channel -> Json.serializeF channel
        | Channel.GUILD_STAGE_VOICE channel -> Json.serializeF channel
        | Channel.GUILD_DIRECTORY channel -> Json.serializeF channel
        | Channel.GUILD_FORUM channel -> Json.serializeF channel
        | Channel.GUILD_MEDIA channel -> Json.serializeF channel
        |> writer.WriteRawValue

// https://discord.com/developers/docs/resources/channel#followed-channel-object-followed-channel-structure
type FollowedChannel = {
    [<JsonPropertyName "channel_id">] ChannelId: string
    [<JsonPropertyName "webhook_id">] WebhookId: string
}

// https://discord.com/developers/docs/resources/channel#overwrite-object-overwrite-structure
type PermissionOverwrite = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "type">] Type: PermissionOverwriteType
    [<JsonPropertyName "allow">] Allow: string
    [<JsonPropertyName "deny">] Deny: string
}

and PartialPermissionOverwrite = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "type">] Type: PermissionOverwriteType
    [<JsonPropertyName "allow">] Allow: string option
    [<JsonPropertyName "deny">] Deny: string option
}

// https://discord.com/developers/docs/resources/channel#thread-metadata-object-thread-metadata-structure
type ThreadMetadata = {
    [<JsonPropertyName "archived">] Archived: bool
    [<JsonPropertyName "auto_archive_duration">] AutoArchiveDuration: int
    [<JsonPropertyName "archive_timestamp">] ArchiveTimestamp: DateTime
    [<JsonPropertyName "locked">] Locked: bool
    [<JsonPropertyName "invitable">] Invitable: bool option
    [<JsonPropertyName "create_timestamp">] CreateTimestamp: DateTime option
}

// https://discord.com/developers/docs/resources/channel#thread-member-object-thread-member-structure
type ThreadMember = {
    [<JsonPropertyName "id">] Id: string option
    [<JsonPropertyName "user_id">] UserId: string option
    [<JsonPropertyName "join_timestamp">] JoinTimestamp: DateTime
    [<JsonPropertyName "flags">] Flags: int
    [<JsonPropertyName "member">] Member: GuildMember option
}

// https://discord.com/developers/docs/resources/channel#default-reaction-object-default-reaction-structure
type DefaultReaction = {
    [<JsonPropertyName "emoji_id">] EmojiId: string option
    [<JsonPropertyName "emoji_name">] EmojiName: string option
}

// https://discord.com/developers/docs/resources/channel#forum-tag-object-forum-tag-structure
type ForumTag = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "name">] Name: string
    [<JsonPropertyName "moderated">] Moderated: bool
    [<JsonPropertyName "emoji_id">] EmojiId: string option
    [<JsonPropertyName "emoji_name">] EmojiName: string option
}

// TODO: "When updating a GUILD_FORUM or a GUILD_MEDIA channel, tag objects in available_tags only require the name field."

// ----- Resources: Emoji -----

// https://discord.com/developers/docs/resources/emoji#emoji-object-emoji-structure
type Emoji = {
    [<JsonPropertyName "id">] Id: string option
    [<JsonPropertyName "name">] Name: string option
    [<JsonPropertyName "roles">] Roles: string list option
    [<JsonPropertyName "user">] User: User option
    [<JsonPropertyName "require_colons">] RequireColons: bool option
    [<JsonPropertyName "managed">] Managed: bool option
    [<JsonPropertyName "animated">] Animated: bool option
    [<JsonPropertyName "available">] Available: bool option
}

and PartialEmoji = Emoji // All emoji properties are already optional

// ----- Resources: Entitlement -----

// https://discord.com/developers/docs/resources/entitlement#entitlement-object-entitlement-structure
type Entitlement = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "sku_id">] SkuId: string
    [<JsonPropertyName "application_id">] ApplicationId: string
    [<JsonPropertyName "user_id">] UserId: string option
    [<JsonPropertyName "type">] Type: EntitlementType
    [<JsonPropertyName "deleted">] Deleted: bool
    [<JsonPropertyName "starts_at">] StartsAt: DateTime option
    [<JsonPropertyName "ends_at">] EndsAt: DateTime option
    [<JsonPropertyName "guild_id">] GuildId: string option
    [<JsonPropertyName "consumed">] Consumed: bool option
}

// ----- Resources: Guild -----

// https://discord.com/developers/docs/resources/guild#guild-object-guild-structure
type Guild = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "name">] Name: string
    [<JsonPropertyName "icon">] Icon: string option
    [<JsonPropertyName "icon_hash">] IconHash: string option
    [<JsonPropertyName "splash">] Splash: string option
    [<JsonPropertyName "discovery_splash">] DiscoverySplash: string option
    [<JsonPropertyName "owner">] Owner: bool option
    [<JsonPropertyName "owner_id">] OwnerId: string
    [<JsonPropertyName "permissions">] Permissions: string option
    [<JsonPropertyName "afk_channel_id">] AfkChannelId: string option
    [<JsonPropertyName "afk_timeout">] AfkTimeout: int
    [<JsonPropertyName "widget_enabled">] WidgetEnabled: bool option
    [<JsonPropertyName "widget_channel_id">] WidgetChannelId: string option
    [<JsonPropertyName "verification_level">] VerificationLevel: VerificationLevel
    [<JsonPropertyName "default_message_notifications">] DefaultMessageNotifications: MessageNotificationLevel
    [<JsonPropertyName "explicit_content_filter">] ExplicitContentFilter: ExplicitContentFilterLevel
    [<JsonPropertyName "roles">] Roles: Role list
    [<JsonPropertyName "emojis">] Emojis: Emoji list
    [<JsonPropertyName "features">] Features: GuildFeature list
    [<JsonPropertyName "mfa_level">] MfaLevel: MfaLevel
    [<JsonPropertyName "application_id">] ApplicationId: string option
    [<JsonPropertyName "system_channel_id">] SystemChannelId: string option
    [<JsonPropertyName "system_channel_flags">] SystemChannelFlags: int
    [<JsonPropertyName "rules_channel_id">] RulesChannelId: string option
    [<JsonPropertyName "max_presences">] MaxPresences: int option
    [<JsonPropertyName "max_members">] MaxMembers: int option
    [<JsonPropertyName "vanity_url_code">] VanityUrlCode: string option
    [<JsonPropertyName "description">] Description: string option
    [<JsonPropertyName "banner">] Banner: string option
    [<JsonPropertyName "premium_tier">] PremiumTier: GuildPremiumTier
    [<JsonPropertyName "premium_subscription_count">] PremiumSubscriptionCount: int option
    [<JsonPropertyName "preferred_locale">] PreferredLocale: string
    [<JsonPropertyName "public_updates_channel_id">] PublicUpdatesChannelId: string option
    [<JsonPropertyName "max_video_channel_users">] MaxVideoChannelUsers: int option
    [<JsonPropertyName "max_stage_video_channel_users">] MaxStageVideoChannelUsers: int option
    [<JsonPropertyName "approximate_member_count">] ApproximateMemberCount: int option
    [<JsonPropertyName "approximate_presence_count">] ApproximatePresenceCount: int option
    [<JsonPropertyName "welcome_screen">] WelcomeScreen: WelcomeScreen option
    [<JsonPropertyName "nsfw_level">] NsfwLevel: NsfwLevel
    [<JsonPropertyName "stickers">] Stickers: Sticker list option
    [<JsonPropertyName "premium_progress_bar_enabled">] PremiumProgressBarEnabled: bool
    [<JsonPropertyName "safety_alerts_channel_id">] SafetyAlertsChannelId: string option
    [<JsonPropertyName "incidents_data">] IncidentsData: IncidentsData option
}

and PartialGuild = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "name">] Name: string option
    [<JsonPropertyName "icon">] Icon: string option
    [<JsonPropertyName "icon_hash">] IconHash: string option
    [<JsonPropertyName "splash">] Splash: string option
    [<JsonPropertyName "discovery_splash">] DiscoverySplash: string option
    [<JsonPropertyName "owner">] Owner: bool option
    [<JsonPropertyName "owner_id">] OwnerId: string option
    [<JsonPropertyName "permissions">] Permissions: string option
    [<JsonPropertyName "afk_channel_id">] AfkChannelId: string option
    [<JsonPropertyName "afk_timeout">] AfkTimeout: int option
    [<JsonPropertyName "widget_enabled">] WidgetEnabled: bool option
    [<JsonPropertyName "widget_channel_id">] WidgetChannelId: string option
    [<JsonPropertyName "verification_level">] VerificationLevel: VerificationLevel option
    [<JsonPropertyName "default_message_notifications">] DefaultMessageNotifications: MessageNotificationLevel option
    [<JsonPropertyName "explicit_content_filter">] ExplicitContentFilter: ExplicitContentFilterLevel option
    [<JsonPropertyName "roles">] Roles: Role list option
    [<JsonPropertyName "emojis">] Emojis: Emoji list option
    [<JsonPropertyName "features">] Features: GuildFeature list option
    [<JsonPropertyName "mfa_level">] MfaLevel: MfaLevel option
    [<JsonPropertyName "application_id">] ApplicationId: string option
    [<JsonPropertyName "system_channel_id">] SystemChannelId: string option
    [<JsonPropertyName "system_channel_flags">] SystemChannelFlags: int option
    [<JsonPropertyName "rules_channel_id">] RulesChannelId: string option
    [<JsonPropertyName "max_presences">] MaxPresences: int option
    [<JsonPropertyName "max_members">] MaxMembers: int option
    [<JsonPropertyName "vanity_url_code">] VanityUrlCode: string option
    [<JsonPropertyName "description">] Description: string option
    [<JsonPropertyName "banner">] Banner: string option
    [<JsonPropertyName "premium_tier">] PremiumTier: GuildPremiumTier option
    [<JsonPropertyName "premium_subscription_count">] PremiumSubscriptionCount: int option
    [<JsonPropertyName "preferred_locale">] PreferredLocale: string option
    [<JsonPropertyName "public_updates_channel_id">] PublicUpdatesChannelId: string option
    [<JsonPropertyName "max_video_channel_users">] MaxVideoChannelUsers: int option
    [<JsonPropertyName "max_stage_video_channel_users">] MaxStageVideoChannelUsers: int option
    [<JsonPropertyName "approximate_member_count">] ApproximateMemberCount: int option
    [<JsonPropertyName "approximate_presence_count">] ApproximatePresenceCount: int option
    [<JsonPropertyName "welcome_screen">] WelcomeScreen: WelcomeScreen option
    [<JsonPropertyName "nsfw_level">] NsfwLevel: NsfwLevel option
    [<JsonPropertyName "stickers">] Stickers: Sticker list option
    [<JsonPropertyName "premium_progress_bar_enabled">] PremiumProgressBarEnabled: bool option
    [<JsonPropertyName "safety_alerts_channel_id">] SafetyAlertsChannelId: string option
}

// https://discord.com/developers/docs/resources/guild#unavailable-guild-object
type UnavailableGuild = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "unavailable">] Unavailable: bool
}

// https://discord.com/developers/docs/resources/guild#guild-preview-object-guild-preview-structure
type GuildPreview = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "name">] Name: string
    [<JsonPropertyName "icon">] Icon: string option
    [<JsonPropertyName "splash">] Splash: string option
    [<JsonPropertyName "discovery_splash">] DiscoverySplash: string option
    [<JsonPropertyName "emojis">] Emojis: Emoji list
    [<JsonPropertyName "features">] Features: GuildFeature list
    [<JsonPropertyName "approximate_member_count">] ApproximateMemberCount: int
    [<JsonPropertyName "approximate_presence_count">] ApproximatePresenceCount: int
    [<JsonPropertyName "description">] Description: string option
    [<JsonPropertyName "stickers">] Stickers: Sticker list
}

// https://discord.com/developers/docs/resources/guild#guild-widget-settings-object-guild-widget-settings-structure
type GuildWidgetSettings = {
    [<JsonPropertyName "enabled">] Enabled: bool
    [<JsonPropertyName "channel_id">] ChannelId: string option
}

// https://discord.com/developers/docs/resources/guild#guild-widget-object-guild-widget-structure
type GuildWidget = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "name">] Name: string
    [<JsonPropertyName "instant_invite">] InstantInvite: string option
    [<JsonPropertyName "channels">] Channels: PartialChannel list
    [<JsonPropertyName "members">] Members: PartialUser list
    [<JsonPropertyName "presence_count">] PresenceCount: int
}

// https://discord.com/developers/docs/resources/guild#guild-member-object-guild-member-structure
type GuildMember = {
    [<JsonPropertyName "user">] User: User option
    [<JsonPropertyName "nick">] Nick: string option
    [<JsonPropertyName "avatar">] Avatar: string option
    [<JsonPropertyName "banner">] Banner: string option
    [<JsonPropertyName "roles">] Roles: string list
    [<JsonPropertyName "joined_at">] JoinedAt: DateTime option
    [<JsonPropertyName "premium_since">] PremiumSince: DateTime option
    [<JsonPropertyName "deaf">] Deaf: bool
    [<JsonPropertyName "mute">] Mute: bool
    [<JsonPropertyName "flags">] Flags: int
    [<JsonPropertyName "pending">] Pending: bool option
    [<JsonPropertyName "permissions">] Permissions: string option
    [<JsonPropertyName "communication_disabled_until">] CommunicationDisabledUntil: DateTime option
    [<JsonPropertyName "avatar_decoration_metadata">] AvatarDecorationData: AvatarDecorationData option
}

and PartialGuildMember = {
    [<JsonPropertyName "user">] User: User option
    [<JsonPropertyName "nick">] Nick: string option
    [<JsonPropertyName "avatar">] Avatar: string option
    [<JsonPropertyName "banner">] Banner: string option
    [<JsonPropertyName "roles">] Roles: string list option
    [<JsonPropertyName "joined_at">] JoinedAt: DateTime option
    [<JsonPropertyName "premium_since">] PremiumSince: DateTime option
    [<JsonPropertyName "deaf">] Deaf: bool option
    [<JsonPropertyName "mute">] Mute: bool option
    [<JsonPropertyName "flags">] Flags: int option
    [<JsonPropertyName "pending">] Pending: bool option
    [<JsonPropertyName "permissions">] Permissions: string option
    [<JsonPropertyName "communication_disabled_until">] CommunicationDisabledUntil: DateTime option
    [<JsonPropertyName "avatar_decoration_metadata">] AvatarDecorationData: AvatarDecorationData option
}

// https://discord.com/developers/docs/resources/guild#integration-object-integration-structure
type Integration = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "name">] Name: string
    [<JsonPropertyName "type">] Type: GuildIntegrationType
    [<JsonPropertyName "enabled">] Enabled: bool
    [<JsonPropertyName "syncing">] Syncing: bool option
    [<JsonPropertyName "role_id">] RoleId: string option
    [<JsonPropertyName "enable_emoticons">] EnableEmoticons: bool option
    [<JsonPropertyName "expire_behavior">] ExpireBehavior: IntegrationExpireBehavior option
    [<JsonPropertyName "expire_grace_period">] ExpireGracePeriod: int option
    [<JsonPropertyName "user">] User: User option
    [<JsonPropertyName "account">] Account: IntegrationAccount
    [<JsonPropertyName "synced_at">] SyncedAt: DateTime option
    [<JsonPropertyName "subscriber_count">] SubscriberCount: int option
    [<JsonPropertyName "revoked">] Revoked: bool option
    [<JsonPropertyName "application">] Application: IntegrationApplication option
    [<JsonPropertyName "scopes">] Scopes: OAuth2Scope list option
}

and PartialIntegration = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "name">] Name: string option
    [<JsonPropertyName "type">] Type: GuildIntegrationType option
    [<JsonPropertyName "enabled">] Enabled: bool option
    [<JsonPropertyName "syncing">] Syncing: bool option
    [<JsonPropertyName "role_id">] RoleId: string option
    [<JsonPropertyName "enable_emoticons">] EnableEmoticons: bool option
    [<JsonPropertyName "expire_behavior">] ExpireBehavior: IntegrationExpireBehavior option
    [<JsonPropertyName "expire_grace_period">] ExpireGracePeriod: int option
    [<JsonPropertyName "user">] User: User option
    [<JsonPropertyName "account">] Account: IntegrationAccount option
    [<JsonPropertyName "synced_at">] SyncedAt: DateTime option
    [<JsonPropertyName "subscriber_count">] SubscriberCount: int option
    [<JsonPropertyName "revoked">] Revoked: bool option
    [<JsonPropertyName "application">] Application: IntegrationApplication option
    [<JsonPropertyName "scopes">] Scopes: OAuth2Scope list option
}

// https://discord.com/developers/docs/resources/guild#integration-account-object-integration-account-structure
type IntegrationAccount = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "name">] Name: string
}

// https://discord.com/developers/docs/resources/guild#integration-application-object-integration-application-structure
type IntegrationApplication = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "name">] Name: string
    [<JsonPropertyName "icon">] Icon: string option
    [<JsonPropertyName "description">] Description: string
    [<JsonPropertyName "bot">] Bot: User option
}

// https://discord.com/developers/docs/resources/guild#ban-object-ban-structure
type Ban = {
    [<JsonPropertyName "reason">] Reason: string option
    [<JsonPropertyName "user">] User: User
}

// https://discord.com/developers/docs/resources/guild#welcome-screen-object-welcome-screen-structure
type WelcomeScreen = {
    [<JsonPropertyName "description">] Description: string option
    [<JsonPropertyName "welcome_channels">] WelcomeChannels: WelcomeScreenChannel list
}

// https://discord.com/developers/docs/resources/guild#welcome-screen-object-welcome-screen-channel-structure
type WelcomeScreenChannel = {
    [<JsonPropertyName "channel_id">] ChannelId: string
    [<JsonPropertyName "description">] Description: string
    [<JsonPropertyName "emoji_id">] EmojiId: string option
    [<JsonPropertyName "emoji_name">] EmojiName: string option
}

// https://discord.com/developers/docs/resources/guild#guild-onboarding-object-guild-onboarding-structure
type GuildOnboarding = {
    [<JsonPropertyName "guild_id">] GuildId: string
    [<JsonPropertyName "prompts">] Prompts: GuildOnboardingPrompt list
    [<JsonPropertyName "default_channel_ids">] DefaultChannelIds: string list
    [<JsonPropertyName "enabled">] Enabled: bool
    [<JsonPropertyName "mode">] Mode: OnboardingMode
}

// https://discord.com/developers/docs/resources/guild#guild-onboarding-object-onboarding-prompt-structure
type GuildOnboardingPrompt = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "type">] Type: OnboardingPromptType
    [<JsonPropertyName "options">] Options: GuildOnboardingPromptOption list
    [<JsonPropertyName "title">] Title: string
    [<JsonPropertyName "single_select">] SingleSelect: bool
    [<JsonPropertyName "required">] Required: bool
    [<JsonPropertyName "in_onboarding">] InOnboarding: bool
}

// https://discord.com/developers/docs/resources/guild#guild-onboarding-object-prompt-option-structure
type GuildOnboardingPromptOption = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "channel_ids">] ChannelIds: string list
    [<JsonPropertyName "role_ids">] RoleIds: string list
    [<JsonPropertyName "emoji">] Emoji: Emoji option
    [<JsonPropertyName "emoji_id">] EmojiId: string option
    [<JsonPropertyName "emoji_name">] EmojiName: string option
    [<JsonPropertyName "emoji_animated">] EmojiAnimated: bool option
    [<JsonPropertyName "title">] Title: string
    [<JsonPropertyName "description">] Description: string
}

// https://discord.com/developers/docs/resources/guild#incidents-data-object-incidents-data-structure
type IncidentsData = {
    [<JsonPropertyName "invites_disabled_until">] InvitesDisabledUntil: DateTime option
    [<JsonPropertyName "dms_disabled_until">] DmsDisabledUntil: DateTime option
    [<JsonPropertyName "dm_spam_detected_at">] DmSpamDetectedAt: DateTime option
    [<JsonPropertyName "raid_detected_at">] RaidDetectedAt: DateTime option
}

// ----- Resources: Guild Scheduled Event -----

// https://discord.com/developers/docs/resources/guild-scheduled-event#guild-scheduled-event-object-guild-scheduled-event-structure
type GuildScheduledEvent = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "guild_id">] GuildId: string
    [<JsonPropertyName "channel_id">] ChannelId: string option
    [<JsonPropertyName "creator_id">] CreatorId: string option
    [<JsonPropertyName "name">] Name: string
    [<JsonPropertyName "description">] Description: string option
    [<JsonPropertyName "scheduled_start_time">] ScheduledStartTime: DateTime option
    [<JsonPropertyName "scheduled_end_time">] ScheduledEndTime: DateTime option
    [<JsonPropertyName "privacy_level">] PrivacyLevel: PrivacyLevel
    [<JsonPropertyName "event_status">] EventStatus: EventStatus
    [<JsonPropertyName "entity_type">] EntityType: ScheduledEntityType
    [<JsonPropertyName "entity_id">] EntityId: string option
    [<JsonPropertyName "entity_metadata">] EntityMetadata: EntityMetadata option
    [<JsonPropertyName "creator">] Creator: User option
    [<JsonPropertyName "user_count">] UserCount: int option
    [<JsonPropertyName "image">] Image: string option
    [<JsonPropertyName "recurrence_rule">] RecurrenceRule: RecurrenceRule option
}

// https://discord.com/developers/docs/resources/guild-scheduled-event#guild-scheduled-event-object-guild-scheduled-event-entity-metadata
type EntityMetadata = {
    [<JsonPropertyName "location">] Location: string option
}

// https://discord.com/developers/docs/resources/guild-scheduled-event#guild-scheduled-event-user-object-guild-scheduled-event-user-structure
type GuildScheduledEventUser = {
    [<JsonPropertyName "guild_scheduled_event_id">] GuildScheduledEventId: string
    [<JsonPropertyName "user">] User: User
    [<JsonPropertyName "member">] Member: GuildMember option
}

// https://discord.com/developers/docs/resources/guild-scheduled-event#guild-scheduled-event-recurrence-rule-object
type RecurrenceRule = {
    [<JsonPropertyName "start">] Start: string
    [<JsonPropertyName "end">] End: string option
    [<JsonPropertyName "frequency">] Frequency: RecurrenceRuleFrequency
    [<JsonPropertyName "interval">] Interval: int
    [<JsonPropertyName "by_weekday">] ByWeekday: RecurrenceRuleWeekday list option
    [<JsonPropertyName "by_weekend">] ByWeekend: RecurrenceRuleNWeekday list option
    [<JsonPropertyName "by_month">] ByMonth: RecurrenceRuleMonth list option
    [<JsonPropertyName "by_month_day">] ByMonthDay: int list option
    [<JsonPropertyName "by_year_day">] ByYearDay: int list option
    [<JsonPropertyName "count">] Count: int option
}

// TODO: Handle documented limitations?

// https://discord.com/developers/docs/resources/guild-scheduled-event#guild-scheduled-event-recurrence-rule-object-guild-scheduled-event-recurrence-rule-nweekday-structure
type RecurrenceRuleNWeekday = {
    [<JsonPropertyName "n">] N: int
    [<JsonPropertyName "day">] Day: RecurrenceRuleWeekday
}

// ----- Resources: Guild Template -----

// https://discord.com/developers/docs/resources/guild-template#guild-template-object-guild-template-structure
type GuildTemplate = {
    [<JsonPropertyName "code">] Code: string
    [<JsonPropertyName "name">] Name: string
    [<JsonPropertyName "description">] Description: string option
    [<JsonPropertyName "usage_count">] UsageCount: int
    [<JsonPropertyName "creator_id">] CreatorId: string
    [<JsonPropertyName "creator">] Creator: User
    [<JsonPropertyName "created_at">] CreatedAt: DateTime
    [<JsonPropertyName "updated_at">] UpdatedAt: DateTime
    [<JsonPropertyName "source_guild_id">] SourceGuildId: string
    [<JsonPropertyName "serialized_source_guild">] SerializedSourceGuild: PartialGuild
    [<JsonPropertyName "is_dirty">] IsDirty: bool option
}

// ----- Resources: Invite -----

// https://discord.com/developers/docs/resources/invite#invite-object-invite-structure
type Invite = {
    [<JsonPropertyName "type">] Type: InviteType
    [<JsonPropertyName "code">] Code: string
    [<JsonPropertyName "guild">] Guild: Guild option
    [<JsonPropertyName "channel">] Channel: PartialChannel option
    [<JsonPropertyName "inviter">] Inviter: PartialUser option
    [<JsonPropertyName "target_type">] TargetType: InviteTargetType option
    [<JsonPropertyName "target_user">] TargetUser: User option
    [<JsonPropertyName "target_application">] TargetApplication: PartialApplication option
    [<JsonPropertyName "approximate_presence_count">] ApproximatePresenceCount: int option
    [<JsonPropertyName "approximate_member_count">] ApproximateMemberCount: int option
    [<JsonPropertyName "expires_at">] ExpiresAt: DateTime
    [<JsonPropertyName "guild_scheduled_event">] GuildScheduledEvent: GuildScheduledEvent option
}

// https://discord.com/developers/docs/resources/invite#invite-metadata-object-invite-metadata-structure
type InviteMetadata = {
    [<JsonPropertyName "uses">] Uses: int
    [<JsonPropertyName "max_uses">] MaxUses: int
    [<JsonPropertyName "max_age">] MaxAge: int
    [<JsonPropertyName "temporary">] Temporary: bool
    [<JsonPropertyName "created_at">] CreatedAt: DateTime
}

[<JsonConverter(typeof<InviteWithmetadataConverter>)>]
type InviteWithMetadata = {
    Invite: Invite
    Metadata: InviteMetadata
}

and InviteWithmetadataConverter () =
    inherit JsonConverter<InviteWithMetadata> ()
    
    override _.Read (reader, typeToConvert, options) =
        let success, document = JsonDocument.TryParseValue &reader
        if not success then raise (JsonException())

        let json = document.RootElement.GetRawText()

        {
            Invite = Json.deserializeF json;
            Metadata = Json.deserializeF json;
        }

    override _.Write (writer, value, options) =
        let invite = Json.serializeF value.Invite
        let metadata = Json.serializeF value.Metadata

        writer.WriteRawValue (Json.merge invite metadata)

// https://discord.com/developers/docs/resources/invite#invite-stage-instance-object-invite-stage-instance-structure
type InviteStageInstance = {
    [<JsonPropertyName "members">] Members: PartialGuildMember list
    [<JsonPropertyName "participant_count">] ParticipantCount: int
    [<JsonPropertyName "speaker_count">] SpeakerCount: int
    [<JsonPropertyName "topic">] Topic: string
}

// ----- Resources: Message -----

// https://discord.com/developers/docs/resources/message#message-object-message-structure
type Message = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "channel_id">] ChannelId: string
    [<JsonPropertyName "author">] Author: User
    [<JsonPropertyName "content">] Content: string option
    [<JsonPropertyName "timestamp">] Timestamp: DateTime
    [<JsonPropertyName "edited_timestamp">] EditedTimestamp: DateTime option
    [<JsonPropertyName "tts">] Tts: bool
    [<JsonPropertyName "mention_everyone">] MentionEveryone: bool
    [<JsonPropertyName "mentions">] Mentions: User list
    [<JsonPropertyName "mention_roles">] MentionRoles: string list
    [<JsonPropertyName "mention_channels">] MentionChannels: ChannelMention list
    [<JsonPropertyName "attachments">] Attachments: Attachment list
    [<JsonPropertyName "embeds">] Embeds: Embed list
    [<JsonPropertyName "reactions">] Reactions: Reaction list
    [<JsonPropertyName "nonce">] Nonce: MessageNonce option
    [<JsonPropertyName "pinned">] Pinned: bool
    [<JsonPropertyName "webhook_id">] WebhookId: string option
    [<JsonPropertyName "type">] Type: MessageType
    [<JsonPropertyName "activity">] Activity: MessageActivity option
    [<JsonPropertyName "application">] Application: PartialApplication option
    [<JsonPropertyName "flags">] Flags: int option
    [<JsonPropertyName "message_reference">] MessageReference: MessageReference option
    [<JsonPropertyName "message_snapshots">] MessageSnapshots: MessageSnapshot list option
    [<JsonPropertyName "referenced_message">] ReferencedMessage: Message option
    [<JsonPropertyName "interaction_metadata">] InteractionMetadata: MessageInteractionMetadata option
    [<JsonPropertyName "interaction">] Interaction: MessageInteraction option
    [<JsonPropertyName "thread">] Thread: Channel option
    [<JsonPropertyName "components">] Components: Component list option
    [<JsonPropertyName "sticker_items">] StickerItems: StickerItem list option
    [<JsonPropertyName "position">] Position: int option
    [<JsonPropertyName "role_subscription_data">] RoleSubscriptionData: RoleSubscriptionData option
    [<JsonPropertyName "resolved">] Resolved: ResolvedData option
    [<JsonPropertyName "poll">] Poll: Poll option
    [<JsonPropertyName "call">] Call: MessageCall option
}

and PartialMessage = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "channel_id">] ChannelId: string option
    [<JsonPropertyName "author">] Author: User option
    [<JsonPropertyName "content">] Content: string option
    [<JsonPropertyName "timestamp">] Timestamp: DateTime option
    [<JsonPropertyName "edited_timestamp">] EditedTimestamp: DateTime option
    [<JsonPropertyName "tts">] Tts: bool option
    [<JsonPropertyName "mention_everyone">] MentionEveryone: bool option
    [<JsonPropertyName "mentions">] Mentions: User list option
    [<JsonPropertyName "mention_roles">] MentionRoles: string list option
    [<JsonPropertyName "mention_channels">] MentionChannels: ChannelMention list option
    [<JsonPropertyName "attachments">] Attachments: Attachment list option
    [<JsonPropertyName "embeds">] Embeds: Embed list option
    [<JsonPropertyName "reactions">] Reactions: Reaction list option
    [<JsonPropertyName "nonce">] Nonce: MessageNonce option
    [<JsonPropertyName "pinned">] Pinned: bool option
    [<JsonPropertyName "webhook_id">] WebhookId: string option
    [<JsonPropertyName "type">] Type: MessageType option
    [<JsonPropertyName "activity">] Activity: MessageActivity option
    [<JsonPropertyName "application">] Application: PartialApplication option
    [<JsonPropertyName "flags">] Flags: int option
    [<JsonPropertyName "message_reference">] MessageReference: MessageReference option
    [<JsonPropertyName "message_snapshots">] MessageSnapshots: MessageSnapshot list option
    [<JsonPropertyName "referenced_message">] ReferencedMessage: Message option
    [<JsonPropertyName "interaction_metadata">] InteractionMetadata: MessageInteractionMetadata option
    [<JsonPropertyName "interaction">] Interaction: MessageInteraction option
    [<JsonPropertyName "thread">] Thread: Channel option
    [<JsonPropertyName "components">] Components: Component list option
    [<JsonPropertyName "sticker_items">] StickerItems: StickerItem list option
    [<JsonPropertyName "position">] Position: int option
    [<JsonPropertyName "role_subscription_data">] RoleSubscriptionData: RoleSubscriptionData option
    [<JsonPropertyName "resolved">] Resolved: ResolvedData option
    [<JsonPropertyName "poll">] Poll: Poll option
    [<JsonPropertyName "call">] Call: MessageCall option
}

/// A partial message specifically for message snapshots
and SnapshotPartialMessage = {
    [<JsonPropertyName "content">] Content: string option
    [<JsonPropertyName "timestamp">] Timestamp: DateTime
    [<JsonPropertyName "edited_timestamp">] EditedTimestamp: DateTime option
    [<JsonPropertyName "mentions">] Mentions: User list
    [<JsonPropertyName "mention_roles">] MentionRoles: string list
    [<JsonPropertyName "attachments">] Attachments: Attachment list
    [<JsonPropertyName "embeds">] Embeds: Embed list
    [<JsonPropertyName "type">] Type: MessageType
    [<JsonPropertyName "flags">] Flags: int option
    [<JsonPropertyName "components">] Components: Component list option
    [<JsonPropertyName "sticker_items">] StickerItems: StickerItem list option
}

// TODO: Handle documented conditions?

// https://discord.com/developers/docs/resources/message#message-object-message-activity-structure
type MessageActivity = {
    [<JsonPropertyName "type">] Type: MessageActivityType
    [<JsonPropertyName "party_id">] PartyId: string option
}

// https://discord.com/developers/docs/resources/message#message-interaction-metadata-object-application-command-interaction-metadata-structure
type ApplicationCommandInteractionMetadata = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "type">] Type: InteractionType
    [<JsonPropertyName "user">] User: User
    [<JsonPropertyName "authorizing_integration_owners">] AuthorizingIntegrationOwners: Map<ApplicationIntegrationType, string>
    [<JsonPropertyName "original_response_message_id">] OriginalResponseMessageId: string option
    [<JsonPropertyName "target_user">] TargetUser: User option
    [<JsonPropertyName "target_message_id">] TargetMessageId: string option
}

// https://discord.com/developers/docs/resources/message#message-interaction-metadata-object-message-component-interaction-metadata-structure
type MessageComponentInteractionMetadata = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "type">] Type: InteractionType
    [<JsonPropertyName "user">] User: User
    [<JsonPropertyName "authorizing_integration_owners">] AuthorizingIntegrationOwners: Map<ApplicationIntegrationType, string>
    [<JsonPropertyName "original_response_message_id">] OriginalResponseMessageId: string option
    [<JsonPropertyName "interacted_message_id">] InteractedMessageId: string
}

// https://discord.com/developers/docs/resources/message#message-interaction-metadata-object-modal-submit-interaction-metadata-structure
type ModalSubmitInteractionMetadata = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "type">] Type: InteractionType
    [<JsonPropertyName "user">] User: User
    [<JsonPropertyName "authorizing_integration_owners">] AuthorizingIntegrationOwners: Map<ApplicationIntegrationType, string>
    [<JsonPropertyName "original_response_message_id">] OriginalResponseMessageId: string option
    [<JsonPropertyName "triggering_interaction_metadata">] TriggeringInteractionMetadata: MessageInteractionMetadata // TODO: Make this not allowed to be a ModalSubmitInteractionMetadata
}

[<JsonConverter(typeof<MessageInteractionMetadataConverter>)>]
type MessageInteractionMetadata =
    | APPLICATION_COMMAND of ApplicationCommandInteractionMetadata
    | MESSAGE_COMPONENT of MessageComponentInteractionMetadata
    | MODAL_SUBMIT of ModalSubmitInteractionMetadata

type MessageInteractionMetadataConverter () =
    inherit JsonConverter<MessageInteractionMetadata> ()

    override _.Read (reader, _, _) =
        let success, document = JsonDocument.TryParseValue &reader
        if not success then JsonException.raise "Failed to parse JSON document"

        let interactionType =
            document.RootElement.GetProperty "type"
            |> _.GetInt32()
            |> enum<InteractionType>

        let json = document.RootElement.GetRawText()

        match interactionType with
        | InteractionType.APPLICATION_COMMAND -> MessageInteractionMetadata.APPLICATION_COMMAND <| Json.deserializeF<ApplicationCommandInteractionMetadata> json
        | InteractionType.MESSAGE_COMPONENT -> MessageInteractionMetadata.MESSAGE_COMPONENT <| Json.deserializeF<MessageComponentInteractionMetadata> json
        | InteractionType.MODAL_SUBMIT -> MessageInteractionMetadata.MODAL_SUBMIT <| Json.deserializeF<ModalSubmitInteractionMetadata> json
        | _ -> JsonException.raise "Unexpected InteractionType provided"

    override _.Write (writer, value, _) =
        match value with
        | MessageInteractionMetadata.APPLICATION_COMMAND a -> Json.serializeF a
        | MessageInteractionMetadata.MESSAGE_COMPONENT m -> Json.serializeF m
        | MessageInteractionMetadata.MODAL_SUBMIT m -> Json.serializeF m
        |> writer.WriteRawValue

// https://discord.com/developers/docs/resources/message#message-call-object-message-call-object-structure
type MessageCall = {
    [<JsonPropertyName "participants">] Participants: string list
    [<JsonPropertyName "ended_timestamp">] EndedTimestamp: DateTime option
}

// https://discord.com/developers/docs/resources/message#message-reference-structure
type MessageReference = {
    [<JsonPropertyName "type">] Type: MessageReferenceType option
    [<JsonPropertyName "message_id">] MessageId: string option
    [<JsonPropertyName "channel_id">] ChannelId: string option
    [<JsonPropertyName "guild_id">] GuildId: string option
    [<JsonPropertyName "fail_if_not_exists">] FailIfNotExists: bool option
}

// TODO: Handle documented conditions?

// https://discord.com/developers/docs/resources/message#message-snapshot-structure
type MessageSnapshot = {
    [<JsonPropertyName "message">] Message: SnapshotPartialMessage
}

// https://discord.com/developers/docs/resources/message#reaction-object-reaction-structure
type Reaction = {
    [<JsonPropertyName "count">] Count: int
    [<JsonPropertyName "count_details">] CountDetails: ReactionCountDetails
    [<JsonPropertyName "me">] Me: bool
    [<JsonPropertyName "me_burst">] MeBurst: bool
    [<JsonPropertyName "emoji">] Emoji: PartialEmoji
    [<JsonPropertyName "burst_colors">] BurstColors: int list
}

// https://discord.com/developers/docs/resources/message#reaction-count-details-object-reaction-count-details-structure
type ReactionCountDetails = {
    [<JsonPropertyName "burst">] Burst: int
    [<JsonPropertyName "normal">] Normal: int
}

// https://discord.com/developers/docs/resources/message#embed-object-embed-structure
type Embed = {
    [<JsonPropertyName "title">] Title: string option
    [<JsonPropertyName "type">] Type: EmbedType option
    [<JsonPropertyName "description">] Description: string option
    [<JsonPropertyName "url">] Url: string option
    [<JsonPropertyName "timestamp">] Timestamp: DateTime option
    [<JsonPropertyName "color">] Color: int option
    [<JsonPropertyName "footer">] Footer: EmbedFooter option
    [<JsonPropertyName "image">] Image: EmbedImage option
    [<JsonPropertyName "thumbnail">] Thumbnail: EmbedThumbnail option
    [<JsonPropertyName "video">] Video: EmbedVideo option
    [<JsonPropertyName "provider">] Provider: EmbedProvider option
    [<JsonPropertyName "author">] Author: EmbedAuthor option
    [<JsonPropertyName "fields">] Fields: EmbedField list option
}

// https://discord.com/developers/docs/resources/message#embed-object-embed-thumbnail-structure
type EmbedThumbnail = {
    [<JsonPropertyName "url">] Url: string
    [<JsonPropertyName "proxy_url">] ProxyUrl: string option
    [<JsonPropertyName "height">] Height: int option
    [<JsonPropertyName "width">] Width: int option
}

// https://discord.com/developers/docs/resources/message#embed-object-embed-video-structure
type EmbedVideo = {
    [<JsonPropertyName "url">] Url: string option
    [<JsonPropertyName "proxy_url">] ProxyUrl: string option
    [<JsonPropertyName "height">] Height: int option
    [<JsonPropertyName "width">] Width: int option
}

// https://discord.com/developers/docs/resources/message#embed-object-embed-image-structure
type EmbedImage = {
    [<JsonPropertyName "url">] Url: string
    [<JsonPropertyName "proxy_url">] ProxyUrl: string option
    [<JsonPropertyName "height">] Height: int option
    [<JsonPropertyName "width">] Width: int option
}

// https://discord.com/developers/docs/resources/message#embed-object-embed-provider-structure
type EmbedProvider = {
    [<JsonPropertyName "name">] Name: string option
    [<JsonPropertyName "url">] Url: string option
}

// https://discord.com/developers/docs/resources/message#embed-object-embed-author-structure
type EmbedAuthor = {
    [<JsonPropertyName "name">] Name: string
    [<JsonPropertyName "url">] Url: string option
    [<JsonPropertyName "icon_url">] IconUrl: string option
    [<JsonPropertyName "proxy_icon_url">] ProxyIconUrl: string option
}

// https://discord.com/developers/docs/resources/message#embed-object-embed-footer-structure
type EmbedFooter = {
    [<JsonPropertyName "text">] Text: string
    [<JsonPropertyName "icon_url">] IconUrl: string option
    [<JsonPropertyName "proxy_icon_url">] ProxyIconUrl: string option
}

// https://discord.com/developers/docs/resources/message#embed-object-embed-field-structure
type EmbedField = {
    [<JsonPropertyName "name">] Name: string
    [<JsonPropertyName "value">] Value: string
    [<JsonPropertyName "inline">] Inline: bool option
}

// TODO: Handle documented embed limits
// TODO: Add poll result embed fields from https://discord.com/developers/docs/resources/message#embed-fields-by-embed-type-poll-result-embed-fields

// https://discord.com/developers/docs/resources/message#attachment-object-attachment-structure
type Attachment = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "filename">] Filename: string
    [<JsonPropertyName "description">] Description: string
    [<JsonPropertyName "content_type">] ContentType: string option
    [<JsonPropertyName "size">] Size: int
    [<JsonPropertyName "url">] Url: string
    [<JsonPropertyName "proxy_url">] ProxyUrl: string
    [<JsonPropertyName "height">] Height: int option
    [<JsonPropertyName "width">] Width: int option
    [<JsonPropertyName "ephemeral">] Ephemeral: bool option
    [<JsonPropertyName "duration_secs">] DurationSecs: float option
    [<JsonPropertyName "waveform">] Waveform: string option
    [<JsonPropertyName "flags">] Flags: int option
}

and PartialAttachment = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "filename">] Filename: string option
    [<JsonPropertyName "description">] Description: string option
    [<JsonPropertyName "content_type">] ContentType: string option
    [<JsonPropertyName "size">] Size: int option
    [<JsonPropertyName "url">] Url: string option
    [<JsonPropertyName "proxy_url">] ProxyUrl: string option
    [<JsonPropertyName "height">] Height: int option
    [<JsonPropertyName "width">] Width: int option
    [<JsonPropertyName "ephemeral">] Ephemeral: bool option
    [<JsonPropertyName "duration_secs">] DurationSecs: float option
    [<JsonPropertyName "waveform">] Waveform: string option
    [<JsonPropertyName "flags">] Flags: int option
}

// https://discord.com/developers/docs/resources/message#channel-mention-object-channel-mention-structure
type ChannelMention = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "guild_id">] GuildId: string
    [<JsonPropertyName "type">] Type: ChannelType
    [<JsonPropertyName "name">] Name: string
}

// https://discord.com/developers/docs/resources/message#allowed-mentions-object-allowed-mentions-structure
type AllowedMentions = {
    [<JsonPropertyName "parse">] Parse: AllowedMentionsParseType list
    [<JsonPropertyName "roles">] Roles: string list option
    [<JsonPropertyName "users">] Users: string list option
    [<JsonPropertyName "replied_user">] RepliedUser: bool option
}

// https://discord.com/developers/docs/resources/message#role-subscription-data-object-role-subscription-data-object-structure
type RoleSubscriptionData = {
    [<JsonPropertyName "role_subscription_listing_id">] RoleSubscriptionListingId: string
    [<JsonPropertyName "tier_name">] TierName: string
    [<JsonPropertyName "total_months_subscribed">] TotalMonthsSubscribed: int
    [<JsonPropertyName "is_renewal">] IsRenewal: bool
}

// ----- Resources: Poll -----

// https://discord.com/developers/docs/resources/poll#poll-object-poll-object-structure
type Poll = {
    [<JsonPropertyName "question">] Question: PollMedia
    [<JsonPropertyName "answers">] Answers: PollAnswer list
    [<JsonPropertyName "expiry">] Expiry: DateTime option
    [<JsonPropertyName "allow_multiselect">] AllowMultiselect: bool
    [<JsonPropertyName "layout_type">] LayoutType: PollLayout
    [<JsonPropertyName "results">] Results: PollResults option
}

// TODO: Is "Poll Create Request Object Structure" needed?

// https://discord.com/developers/docs/resources/poll#poll-media-object-poll-media-object-structure
type PollMedia = {
    [<JsonPropertyName "text">] Text: string option
    [<JsonPropertyName "emoji">] Emoji: PartialEmoji option
}

// https://discord.com/developers/docs/resources/poll#poll-answer-object
type PollAnswer = {
    [<JsonPropertyName "answer_id">] AnswerId: int
    [<JsonPropertyName "poll_media">] PollMedia: PollMedia
}

// https://discord.com/developers/docs/resources/poll#poll-results-object-poll-results-object-structure
type PollResults = {
    [<JsonPropertyName "is_finalized">] IsFinalized: bool
    [<JsonPropertyName "answer_counts">] AnswerCounts: PollAnswerCount list
}

// https://discord.com/developers/docs/resources/poll#poll-results-object-poll-answer-count-object-structure
type PollAnswerCount = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "count">] Count: int
    [<JsonPropertyName "me_voted">] MeVoted: bool
}

// ----- Resources: SKU -----

// https://discord.com/developers/docs/resources/sku#sku-object-sku-structure
type Sku = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "type">] Type: SkuType
    [<JsonPropertyName "application_id">] ApplicationId: string
    [<JsonPropertyName "name">] Name: string
    [<JsonPropertyName "slug">] Slug: string
    [<JsonPropertyName "flags">] Flags: int
}

// ----- Resources: Soundboard -----

// https://discord.com/developers/docs/resources/soundboard#soundboard-sound-object-soundboard-sound-structure
type SoundboardSound = {
    [<JsonPropertyName "name">] Name: string
    [<JsonPropertyName "sound_id">] SoundId: string
    [<JsonPropertyName "volume">] Volume: double
    [<JsonPropertyName "emoji_id">] EmojiId: string option
    [<JsonPropertyName "emoji_name">] EmojiName: string option
    [<JsonPropertyName "guild_id">] GuildId: string option
    [<JsonPropertyName "available">] Available: bool
    [<JsonPropertyName "user">] User: User
}

// ----- Resources: Stage Instance -----

// https://discord.com/developers/docs/resources/stage-instance#stage-instance-object-stage-instance-structure
type StageInstance = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "guild_id">] GuildId: string
    [<JsonPropertyName "channel_id">] ChannelId: string
    [<JsonPropertyName "topic">] Topic: string
    [<JsonPropertyName "privacy_level">] PrivacyLevel: PrivacyLevel
    [<JsonPropertyName "discoverable_enabled">] DiscoverableEnabled: bool
    [<JsonPropertyName "guild_scheduled_event_id">] GuildScheduledEventId: string option
}

// ----- Resources: Sticker -----

// https://discord.com/developers/docs/resources/sticker#sticker-object-sticker-structure
type Sticker = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "pack_id">] PackId: string option
    [<JsonPropertyName "name">] Name: string
    [<JsonPropertyName "description">] Description: string option
    [<JsonPropertyName "tags">] Tags: string
    [<JsonPropertyName "type">] Type: StickerType
    [<JsonPropertyName "format_type">] FormatType: StickerFormat
    [<JsonPropertyName "available">] Available: bool option
    [<JsonPropertyName "guild_id">] GuildId: string option
    [<JsonPropertyName "user">] User: User option
    [<JsonPropertyName "sort_value">] SortValue: int option
}

// https://discord.com/developers/docs/resources/sticker#sticker-item-object-sticker-item-structure
type StickerItem = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "name">] Name: string
    [<JsonPropertyName "format_type">] FormatType: StickerFormat
}

// https://discord.com/developers/docs/resources/sticker#sticker-pack-object
type StickerPack = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "stickers">] Stickers: Sticker list
    [<JsonPropertyName "name">] Name: string
    [<JsonPropertyName "sku_id">] SkuId: string
    [<JsonPropertyName "cover_sticker_id">] CoverStickerId: string option
    [<JsonPropertyName "description">] Description: string
    [<JsonPropertyName "banner_asset_id">] BannerAssetId: string option
}

// ----- Resources: Subscription -----

// https://discord.com/developers/docs/resources/subscription#subscription-object
type Subscription = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "user_id">] UserId: string
    [<JsonPropertyName "sku_ids">] SkuIds: string list
    [<JsonPropertyName "entitlement_ids">] EntitlmentIds: string list
    [<JsonPropertyName "renewal_sku_ids">] RenewalSkuIds: string list option
    [<JsonPropertyName "current_period_start">] CurrentPeriodStart: DateTime
    [<JsonPropertyName "current_period_end">] CurrentPeriodEnd: DateTime
    [<JsonPropertyName "status">] Status: SubscriptionStatus
    [<JsonPropertyName "created_at">] CanceledAt: DateTime option
    [<JsonPropertyName "country">] Country: string option
}

// ----- Resources: User -----

// https://discord.com/developers/docs/resources/user#user-object-user-structure
type User = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "username">] Username: string
    [<JsonPropertyName "discriminator">] Discriminator: string
    [<JsonPropertyName "global_name">] GlobalName: string option
    [<JsonPropertyName "avatar">] Avatar: string option
    [<JsonPropertyName "bot">] Bot: bool option
    [<JsonPropertyName "system">] System: bool option
    [<JsonPropertyName "mfa_enabled">] MfaEnabled: bool option
    [<JsonPropertyName "banner">] Banner: string option
    [<JsonPropertyName "accent_color">] AccentColor: int option
    [<JsonPropertyName "locale">] Locale: string option
    [<JsonPropertyName "verified">] Verified: bool option
    [<JsonPropertyName "email">] Email: string option
    [<JsonPropertyName "flags">] Flags: int option
    [<JsonPropertyName "premium_type">] PremiumType: UserPremiumTier option
    [<JsonPropertyName "public_flags">] PublicFlags: int option
    [<JsonPropertyName "avatar_decoration_data">] AvatarDecorationData: AvatarDecorationData option
}

and PartialUser = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "username">] Username: string option
    [<JsonPropertyName "discriminator">] Discriminator: string option
    [<JsonPropertyName "global_name">] GlobalName: string option
    [<JsonPropertyName "avatar">] Avatar: string option
    [<JsonPropertyName "bot">] Bot: bool option
    [<JsonPropertyName "system">] System: bool option
    [<JsonPropertyName "mfa_enabled">] MfaEnabled: bool option
    [<JsonPropertyName "banner">] Banner: string option
    [<JsonPropertyName "accent_color">] AccentColor: int option
    [<JsonPropertyName "locale">] Locale: string option
    [<JsonPropertyName "verified">] Verified: bool option
    [<JsonPropertyName "email">] Email: string option
    [<JsonPropertyName "flags">] Flags: int option
    [<JsonPropertyName "premium_type">] PremiumType: UserPremiumTier option
    [<JsonPropertyName "public_flags">] PublicFlags: int option
    [<JsonPropertyName "avatar_decoration_data">] AvatarDecorationData: AvatarDecorationData option
}

module User =
    /// Get the avatar index for the user to fetch their default avatar if no custom avatar is set
    let getAvatarIndex (user: User) =
        match int user.Discriminator, Int64.Parse user.Id with
        | 0, id -> (int (id >>> 22)) % 6
        | discriminator, _ -> discriminator % 5

// https://discord.com/developers/docs/resources/user#avatar-decoration-data-object-avatar-decoration-data-structure
type AvatarDecorationData = {
    [<JsonPropertyName "asset">] Asset: string
    [<JsonPropertyName "sku_id">] SkuId: string
}

// https://discord.com/developers/docs/resources/user#connection-object-connection-structure
type Connection = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "name">] Name: string
    [<JsonPropertyName "type">] Type: ConnectionServiceType
    [<JsonPropertyName "revoked">] Revoked: bool option
    [<JsonPropertyName "integrations">] Integrations: PartialIntegration list option
    [<JsonPropertyName "verified">] Verified: bool
    [<JsonPropertyName "friend_sync">] FriendSync: bool
    [<JsonPropertyName "show_activity">] ShowActivity: bool
    [<JsonPropertyName "two_way_link">] TwoWayLink: bool
    [<JsonPropertyName "visibility">] Visibility: ConnectionVisibility
}

// https://discord.com/developers/docs/resources/user#application-role-connection-object-application-role-connection-structure
type ApplicationRoleConnection = {
    [<JsonPropertyName "platform_name">] PlatformName: string option
    [<JsonPropertyName "platform_username">] PlatformUsername: string option
    [<JsonPropertyName "metadata">] Metadata: (string * string) seq // value is the "stringified value" of the metadata
}

// ----- Resources: Voice -----

// https://discord.com/developers/docs/resources/voice#voice-state-object-voice-state-structure
type VoiceState = {
    [<JsonPropertyName "guild_id">] GuildId: string option
    [<JsonPropertyName "channel_id">] ChannelId: string option
    [<JsonPropertyName "user_id">] UserId: string option
    [<JsonPropertyName "member">] Member: GuildMember option
    [<JsonPropertyName "session_id">] SessionId: string
    [<JsonPropertyName "deaf">] Deaf: bool
    [<JsonPropertyName "mute">] Mute: bool
    [<JsonPropertyName "self_deaf">] SelfDeaf: bool
    [<JsonPropertyName "self_mute">] SelfMute: bool
    [<JsonPropertyName "self_stream">] SelfStream: bool option
    [<JsonPropertyName "self_video">] SelfVideo: bool
    [<JsonPropertyName "suppress">] Suppress: bool
    [<JsonPropertyName "request_to_speak_timestamp">] RequestToSpeakTimestamp: DateTime option
}

and PartialVoiceState = {
    [<JsonPropertyName "guild_id">] GuildId: string option
    [<JsonPropertyName "channel_id">] ChannelId: string option
    [<JsonPropertyName "user_id">] UserId: string option
    [<JsonPropertyName "member">] Member: GuildMember option
    [<JsonPropertyName "session_id">] SessionId: string option
    [<JsonPropertyName "deaf">] Deaf: bool option
    [<JsonPropertyName "mute">] Mute: bool option
    [<JsonPropertyName "self_deaf">] SelfDeaf: bool option
    [<JsonPropertyName "self_mute">] SelfMute: bool option
    [<JsonPropertyName "self_stream">] SelfStream: bool option
    [<JsonPropertyName "self_video">] SelfVideo: bool option
    [<JsonPropertyName "suppress">] Suppress: bool option
    [<JsonPropertyName "request_to_speak_timestamp">] RequestToSpeakTimestamp: DateTime option
}

// https://discord.com/developers/docs/resources/voice#voice-region-object-voice-region-structure
type VoiceRegion = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "name">] Name: string
    [<JsonPropertyName "optimal">] Optimal: bool
    [<JsonPropertyName "deprecated">] Deprecated: bool
    [<JsonPropertyName "custom">] Custom: bool
}

// ----- Resources: Webhook -----

// https://discord.com/developers/docs/resources/webhook#webhook-object-webhook-structure
type Webhook = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "webhook_type">] Type: WebhookType
    [<JsonPropertyName "guild_id">] GuildId: string option
    [<JsonPropertyName "channel_id">] ChannelId: string option
    [<JsonPropertyName "user">] User: User option
    [<JsonPropertyName "name">] Name: string option
    [<JsonPropertyName "avatar">] Avatar: string option
    [<JsonPropertyName "token">] Token: string option
    [<JsonPropertyName "application_id">] ApplicationId: string option
    [<JsonPropertyName "source_guild">] SourceGuild: PartialGuild option
    [<JsonPropertyName "source_channel">] SourceChannel: PartialChannel option
    [<JsonPropertyName "url">] Url: string option
}

// ----- Topics: Permissions -----

// TODO: Create modules for computing permissions as documented

// https://discord.com/developers/docs/topics/permissions#role-object-role-structure
type Role = {
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "name">] Name: string
    [<JsonPropertyName "color">] Color: int
    [<JsonPropertyName "hoist">] Hoist: bool
    [<JsonPropertyName "icon">] Icon: string option
    [<JsonPropertyName "unicode_emoji">] UnicodeEmoji: string option
    [<JsonPropertyName "position">] Position: int
    [<JsonPropertyName "permissions">] Permissions: string
    [<JsonPropertyName "managed">] Managed: bool
    [<JsonPropertyName "mentionable">] Mentionable: bool
    [<JsonPropertyName "tags">] Tags: RoleTags option
    [<JsonPropertyName "flags">] Flags: int
}

// https://discord.com/developers/docs/topics/permissions#role-object-role-tags-structure
type RoleTags = {
    [<JsonPropertyName "bot_id">] BotId: string option
    [<JsonPropertyName "integration_id">] IntegrationId: string option
    [<JsonPropertyName "premium_subscriber">] [<JsonConverter(typeof<JsonConverter.NullUndefinedAsBool>)>] PremiumSubscriber: bool
    [<JsonPropertyName "subscription_listing_id">] SubscriptionListingId: string option
    [<JsonPropertyName "available_for_purchase">] [<JsonConverter(typeof<JsonConverter.NullUndefinedAsBool>)>] AvailableForPurchase: bool
    [<JsonPropertyName "guild_connections">] [<JsonConverter(typeof<JsonConverter.NullUndefinedAsBool>)>] GuildConnections: bool
}

// ----- Topics: Rate Limits -----

// TODO: Create modules for handling rate limit headers and etc

// https://discord.com/developers/docs/topics/rate-limits#exceeding-a-rate-limit-rate-limit-response-structure
type RateLimitResponse = {
    [<JsonPropertyName "message">] Message: string
    [<JsonPropertyName "retry_after">] RetryAfter: float
    [<JsonPropertyName "global">] Global: bool
    [<JsonPropertyName "code">] Code: JsonErrorCode option
}

// ----- Topics: Teams -----

// https://discord.com/developers/docs/topics/teams#data-models-team-object
type Team = {
    [<JsonPropertyName "icon">] Icon: string option
    [<JsonPropertyName "id">] Id: string
    [<JsonPropertyName "members">] Members: TeamMember list
    [<JsonPropertyName "name">] Name: string
    [<JsonPropertyName "owner_user_id">] OwnerUserId: string
}

// https://discord.com/developers/docs/topics/teams#data-models-team-member-object
type TeamMember = {
    [<JsonPropertyName "membership_state">] MembershipState: MembershipState
    [<JsonPropertyName "team_id">] TeamId: string
    [<JsonPropertyName "user">] User: PartialUser // avatar, discriminator, id, username
    [<JsonPropertyName "role">] Role: string
}
