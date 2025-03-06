﻿namespace rec FSharp.Discord.Types

open System
open System.Text.Json
open System.Text.Json.Serialization

// ----- API Reference -----

// https://discord.com/developers/docs/reference#error-messages
type ErrorResponse = {
    Code: JsonErrorCode
    Message: string
    Errors: Map<string, string>
}

// ----- Interactions: Receiving and Responding -----

// https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-object-interaction-structure
type Interaction = {
    Id: string
    ApplicationId: string
    Type: InteractionType
    Data: InteractionData option
    Guild: PartialGuild option
    GuildId: string option
    Channel: PartialChannel option
    ChannelId: string option
    Member: GuildMember option
    User: User option
    Token: string
    Version: int
    Message: Message option
    AppPermissions: string
    Locale: string option
    GuildLocale: string option
    Entitlements: Entitlement list
    AuthorizingIntegrationOwners: Map<ApplicationIntegrationType, ApplicationIntegrationTypeConfiguration>
    Context: InteractionContextType option
}

// https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-object-application-command-data-structure
type ApplicationCommandData = {
    Id: string
    Name: string
    Type: ApplicationCommandType
    Resolved: ResolvedData option
    Options: ApplicationCommandInteractionDataOption list option
    GuildId: string option
    TargetId: string option
}

// TODO: ApplicationCommandInteractionDataOption can be partial when responding to APPLICATION_COMMAND_AUTOCOMPLETE

// https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-object-message-component-data-structure
type MessageComponentData = {
    CustomId: string
    ComponentType: ComponentType
    Values: SelectMenuOption list option
    Resolved: ResolvedData option
}

// https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-object-modal-submit-data-structure
type ModalSubmitData = {
    CustomId: string
    Components: Component list
}

type InteractionData =
    | APPLICATION_COMMAND of ApplicationCommandData
    | MESSAGE_COMPONENT   of MessageComponentData
    | MODAL_SUBMIT        of ModalSubmitData

// https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-object-resolved-data-structure
type ResolvedData = {
    Users: Map<string, User> option
    Members: Map<string, PartialGuildMember> option // Missing user, deaf, mute
    Roles: Map<string, Role> option
    Channels: Map<string, PartialChannel> option // Only have id, name, type, permissions (and threads have thread_metadata, parent_id)
    Messages: Map<string, PartialMessage> option
    Attachments: Map<string, Attachment> option
}

// https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-object-application-command-interaction-data-option-structure
type ApplicationCommandInteractionDataOption = {
    Name: string
    Type: ApplicationCommandOptionType
    Value: ApplicationCommandInteractionDataOptionValue option
    Options: ApplicationCommandInteractionDataOption list option
    Focused: bool option
}

[<RequireQualifiedAccess>]
type ApplicationCommandInteractionDataOptionValue =
    | STRING of string
    | INT    of int
    | DOUBLE of double
    | BOOL   of bool

// https://discord.com/developers/docs/interactions/receiving-and-responding#message-interaction-object-message-interaction-structure
type MessageInteraction = {
    Id: string
    Type: InteractionType
    Name: string
    User: User
    Member: PartialGuildMember option
}

// https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-response-object-interaction-response-structure
type InteractionResponse = {
    Type: InteractionCallbackType
    Data: InteractionCallbackData option
}

// https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-response-object-messages
type MessageInteractionCallbackData = {
    Tts: bool option
    Content: string option
    Embeds: Embed list option
    AllowedMentions: AllowedMentions option
    Flags: int option // TODO: Convert to list of flag enums (and check for other instances of `Flag: int` for same change)
    Components: Component list option
    Attachments: PartialAttachment list option
    Poll: Poll option // TODO: Poll REQUEST object, not poll itself
}

// https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-response-object-autocomplete
type AutocompleteInteractionCallbackData = {
    Choices: ApplicationCommandOptionChoice list
}

// https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-response-object-modal
type ModalInteractionCallbackData = {
    CustomId: string
    Title: string
    Components: Component list
}

type InteractionCallbackData =
    | MESSAGE      of MessageInteractionCallbackData
    | AUTOCOMPLETE of AutocompleteInteractionCallbackData
    | MODAL        of ModalInteractionCallbackData

// https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-callback-interaction-callback-response-object
type InteractionCallbackResponse = {
    Interaction: InteractionCallback
    Resource: InteractionCallbackResource option
}

// https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-callback-interaction-callback-object
type InteractionCallback = {
    Id: string
    Type: InteractionType
    ActivityInstanceId: string option
    ResponseMessageId: string option
    ResponseMessageLoading: bool option
    ResponseMessageEphemeral: bool option
}

// https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-callback-interaction-callback-resource-object
type InteractionCallbackResource = {
    Type: InteractionCallbackType
    ActivityInstance: InteractionCallbackActivityInstance option
    Message: Message option
}

// https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-callback-interaction-callback-activity-instance-resource
type InteractionCallbackActivityInstance = {
    Id: string
}

// TODO: Refactor how Structures/* and Events types are implemented and put the structures here (basing off above)

// ----- Interactions: Application Commands -----

// https://discord.com/developers/docs/interactions/application-commands#application-command-object-application-command-structure
type ApplicationCommand = {
    Id: string
    Type: ApplicationCommandType
    ApplicationId: string
    GuildId: string option
    Name: string // TODO: Enforce 1-32 character name with valid chars
    NameLocalizations: Map<string, string> option option
    Description: string
    DescriptionLocalizations: Map<string, string> option option
    Options: ApplicationCommandOption list option
    DefaultMemberPermissions: string option // TODO: Serialize bitfield into permission list
    Nsfw: bool
    IntegrationTypes: ApplicationIntegrationType list
    Contexts: InteractionContextType list option
    Version: string
    Handler: ApplicationCommandHandlerType option
}

// TODO: Create DU for different application command types? A few values only present on one

// https://discord.com/developers/docs/interactions/application-commands#application-command-object-application-command-option-structure
type ApplicationCommandOption = {
    Type: ApplicationCommandOptionType
    Name: string // TODO: Enforce 1-32 character name with valid chars
    NameLocalizations: Map<string, string> option option
    Description: string
    DescriptionLocalizations: Map<string, string> option option
    Required: bool
    Choices: ApplicationCommandOptionChoice list option
    Options: ApplicationCommandOption list option
    ChannelTypes: ChannelType list option
    MinValue: ApplicationCommandOptionMinValue option
    MaxValue: ApplicationCommandOptionMaxValue option
    MinLength: int option
    MaxLength: int option
    Autocomplete: bool option
}

// TODO: Create DU for different types of application command option

[<RequireQualifiedAccess>]
type ApplicationCommandOptionMinValue =
    | INT    of int
    | DOUBLE of double

// TODO: Ensure min 1, max 6000 (create single DU with this requirement)

[<RequireQualifiedAccess>]
type ApplicationCommandOptionMaxValue =
    | INT    of int
    | DOUBLE of double

// TODO: Ensure min 1, max 6000 (create single DU with this requirement)

// https://discord.com/developers/docs/interactions/application-commands#application-command-object-application-command-option-choice-structure
type ApplicationCommandOptionChoice = {
    Name: string
    NameLocalizations: Map<string, string> option option
    Value: ApplicationCommandOptionChoiceValue
}

[<RequireQualifiedAccess>]
type ApplicationCommandOptionChoiceValue =
    | STRING of string
    | INT    of int
    | DOUBLE of double

// TODO: Handle value type based on parent application command option type

// https://discord.com/developers/docs/interactions/application-commands#application-command-permissions-object-guild-application-command-permissions-structure
type GuildApplicationCommandPermissions = {
    Id: string
    ApplicationId: string
    GuildId: string
    Permissions: ApplicationCommandPermission list
}

// https://discord.com/developers/docs/interactions/application-commands#application-command-permissions-object-application-command-permissions-structure
type ApplicationCommandPermission = {
    Id: string
    Type: ApplicationCommandPermissionType
    Permission: bool
}

module ApplicationCommandPermissionConstants =
    /// Sets an application command permission on all members in a guild
    let everyone (guildId: string) =
        guildId

    /// Sets an application command permission on all channels
    let allChannels (guildId: string) =
        (int64 guildId) - 1L |> string // TODO: Return option instead of throwing exception

    // TODO: Should this module be moved into Utils or somewhere else?

// TODO: Handle maximum subcommand depth (is this even possible?)
// TODO: Implement locales and locale fallbacks

// ----- Interactions: Message Components -----

// https://discord.com/developers/docs/interactions/message-components#action-rows
type ActionRow = {
    Type: ComponentType
    Components: Component list
}

// https://discord.com/developers/docs/interactions/message-components#button-object-button-structure
type Button = {
    Type: ComponentType
    Style: ButtonStyle
    Label: string
    Emoji: Emoji option
    CustomId: string option
    Url: string option
    Disabled: bool
}

// TODO: Ensure values meet requirements set in docs for buttons

// https://discord.com/developers/docs/interactions/message-components#select-menu-object-select-menu-structure
type SelectMenu = {
    Type: ComponentType
    CustomId: string
    Options: SelectMenuOption list option
    ChannelTypes: ChannelType list option
    Placeholder: string option
    DefaultValues: SelectMenuDefaultValue list option
    MinValues: int
    MaxValues: int
    Disabled: bool
}

// TODO: DU for different types of select menus
// TODO: Ensure values meet requirements set in docs for select menus

// https://discord.com/developers/docs/interactions/message-components#select-menu-object-select-option-structure
type SelectMenuOption = {
    Label: string
    Value: string
    Description: string option
    Emoji: Emoji option
    Default: bool option
}

// TODO: Ensure values meet requirements set in docs for select menu options

// https://discord.com/developers/docs/interactions/message-components#select-menu-object-select-default-value-structure
type SelectMenuDefaultValue = {
    Id: string
    Type: SelectMenuDefaultValueType
}

// https://discord.com/developers/docs/interactions/message-components#text-input-object-text-input-structure
type TextInput = {
    Type: ComponentType
    CustomId: string
    Style: TextInputStyle
    Label: string
    MinLength: int option
    MaxLength: int option
    Required: bool
    Value: string option
    Placeholder: string option
}

// TODO: Ensure values meet requirements set in docs for text input

type Component =
    | ACTION_ROW of ActionRow
    | BUTTON of Button
    | SELECT_MENU of SelectMenu
    | TEXT_INPUT of TextInput

// ----- Events: Using Gateway -----

// https://discord.com/developers/docs/events/gateway#session-start-limit-object-session-start-limit-structure
type SessionStartLimit = {
    Total: int
    Remaining: int
    ResetAfter: int
    MaxConcurrency: int
}

// ----- Events: Gateway Events -----

// TODO: Define gateway event data types here

// https://discord.com/developers/docs/topics/gateway-events#identify-identify-connection-properties
type IdentifyConnectionProperties = {
    OperatingSystem: string
    Browser: string
    Device: string
}

// TODO: Make single DU for ensuring the device matches expected format (?)

// https://discord.com/developers/docs/events/gateway-events#client-status-object
//[<JsonConverter(typeof<ClientStatus.Converter>)>]
type ClientStatus = {
    Desktop: ClientDeviceStatus option
    Mobile: ClientDeviceStatus option
    Web: ClientDeviceStatus option
}

// TODO: Should this just define the invis/offline in the type instead of as option (which discord does)

// https://discord.com/developers/docs/topics/gateway-events#activity-object-activity-structure
type Activity = {
    Name: string
    Type: ActivityType
    Url: string option
    CreatedAt: DateTime option
    Timestamps: ActivityTimestamps option
    ApplicationId: string option
    Details: string option
    State: string option
    Emoji: ActivityEmoji option
    Party: ActivityParty option
    Assets: ActivityAssets option
    Secrets: ActivitySecrets option
    Instance: bool option
    Flags: int option
    Buttons: ActivityButton list option
}

// https://discord.com/developers/docs/events/gateway-events#activity-object-activity-timestamps
type ActivityTimestamps = {
    Start: DateTime option
    End: DateTime option
}

// https://discord.com/developers/docs/events/gateway-events#activity-object-activity-emoji
type ActivityEmoji = {
    Name: string
    Id: string option
    Animated: bool option
}

// https://discord.com/developers/docs/events/gateway-events#activity-object-activity-party
type ActivityParty = {
    Id: string option
    Size: ActivityPartySize option
}

type ActivityPartySize = {
    Current: int
    Maximum: int
}

// https://discord.com/developers/docs/events/gateway-events#activity-object-activity-assets
type ActivityAssets = {
    LargeImage: string option
    LargeText: string option
    SmallImage: string option
    SmallText: string option
}

// TODO: Handle activity asset images as documented

// https://discord.com/developers/docs/events/gateway-events#activity-object-activity-secrets
type ActivitySecrets = {
    Join: string option
    Spectate: string option
    Match: string option
}

// https://discord.com/developers/docs/events/gateway-events#activity-object-activity-buttons
type ActivityButton = {
    Label: string
    Url: string
}

// ----- Events: Webhook Events -----

// https://discord.com/developers/docs/events/webhook-events#event-body-object
type WebhookEventPayload<'a> = {
    Version: int
    ApplicationId: string
    Type: WebhookPayloadType
    Event: WebhookEventBody<'a> option
}

// https://discord.com/developers/docs/events/webhook-events#payload-structure
type WebhookEventBody<'a> = {
    Type: WebhookEventType
    Timestamp: DateTime
    Data: 'a option
}

// https://discord.com/developers/docs/events/webhook-events#application-authorized-application-authorized-structure
type ApplicationAuthorizedEvent = {
    IntegrationType: ApplicationIntegrationType option
    User: User
    Scopes: OAuthScope list
    Guild: Guild option
}

// https://discord.com/developers/docs/events/webhook-events#entitlement-create-entitlement-create-structure
type EntitlementCreateEvent = Entitlement

// ----- Resources: Application -----

// https://discord.com/developers/docs/resources/application#application-object-application-structure
type Application = {
    Id: string
    Name: string
    Icon: string option
    Description: string
    RpcOrigins: string list option
    BotPublic: bool
    BotRequireCodeGrant: bool
    Bot: PartialUser option
    TermsOfServiceUrl: string option
    PrivacyPolicyUrl: string option
    Owner: PartialUser option
    VerifyKey: string
    Team: Team option
    GuildId: string option
    Guild: PartialGuild option
    PrimarySkuId: string option
    Slug: string option
    CoverImage: string option
    Flags: int option
    ApproximateGuildCount: int option
    ApproximateUserInstallCount: int option
    RedirectUris: string list option
    InteractionsEndpointUrl: string option option
    RoleConnectionsVerificationUrl: string option option
    EventWebhooksUrl: string option option
    EventWebhooksStatus: WebhookEventStatus
    EventWebhooksTypes: WebhookEventType list option
    Tags: string list option
    InstallParams: InstallParams option
    IntegrationTypesConfig: Map<ApplicationIntegrationType, ApplicationIntegrationTypeConfiguration> option
    CustomInstallUrl: string option
}

and PartialApplication = {
    Id: string
    Name: string option
    Icon: string option option
    Description: string option
    RpcOrigins: string list option
    BotPublic: bool option
    BotRequireCodeGrant: bool option
    Bot: PartialUser option
    TermsOfServiceUrl: string option
    PrivacyPolicyUrl: string option
    Owner: PartialUser option
    VerifyKey: string option
    Team: Team option option
    GuildId: string option
    Guild: PartialGuild option
    PrimarySkuId: string option
    Slug: string option
    CoverImage: string option
    Flags: int option
    ApproximateGuildCount: int option
    ApproximateUserInstallCount: int option
    RedirectUris: string list option
    InteractionsEndpointUrl: string option option
    RoleConnectionsVerificationUrl: string option option
    EventWebhooksUrl: string option option
    EventWebhooksStatus: WebhookEventStatus option
    EventWebhooksTypes: WebhookEventType list option
    Tags: string list option
    InstallParams: InstallParams option
    IntegrationTypesConfig: Map<ApplicationIntegrationType, ApplicationIntegrationTypeConfiguration> option
    CustomInstallUrl: string option
}

// https://discord.com/developers/docs/resources/application#application-object-application-integration-type-configuration-object
type ApplicationIntegrationTypeConfiguration = {
    OAuth2InstallParams: InstallParams option
}

// https://discord.com/developers/docs/resources/application#install-params-object-install-params-structure
type InstallParams = {
    Scopes: OAuthScope list
    Permissions: string // TODO: Serialize bitfield into permission list
}

// https://discord.com/developers/docs/resources/application#get-application-activity-instance-activity-instance-object
type ActivityInstance = {
    ApplicationId: string
    InstanceId: string
    LaunchId: string
    Location: ActivityLocation
    Users: string list
}

// https://discord.com/developers/docs/resources/application#get-application-activity-instance-activity-location-object
type ActivityLocation = {
    Id: string
    Kind: ActivityLocationKind
    ChannelId: string
    GuildId: string option option
}

// TODO: Should above optional + nullable properties be stored as `option option`? What alternative would there be?

// ----- Resources: Application Role Connection Metadata -----

// https://discord.com/developers/docs/resources/application-role-connection-metadata#application-role-connection-metadata-object-application-role-connection-metadata-structure
type ApplicationRoleConnectionMetadata = {
    Type: ApplicationRoleConnectionMetadataType
    Key: string
    Name: string
    NameLocalizations: Map<string, string> option
    Description: string
    DescriptionLocalizations: Map<string, string> option
}

// ----- Resources: Audit Log -----

// https://discord.com/developers/docs/resources/audit-log#audit-log-object-audit-log-structure
type AuditLog = {
    ApplicationCommands: ApplicationCommand list
    AuditLogEntries: AuditLogEntry list
    AutoModerationRules: AutoModerationRule list
    GuildScheduledEvents: GuildScheduledEvent list
    Integrations: PartialIntegration list
    Threads: Channel list
    Users: User list
    Webhooks: Webhook list
}

// https://discord.com/developers/docs/resources/audit-log#audit-log-entry-object-audit-log-entry-structure
type AuditLogEntry = {
    TargetId: string option
    Changes: AuditLogChange list option
    UserId: string option
    Id: string
    ActionType: AuditLogEventType
    Options: AuditLogEntryOptionalInfo option
    Reason: string option
}

// https://discord.com/developers/docs/resources/audit-log#audit-log-entry-object-optional-audit-entry-info
type AuditLogEntryOptionalInfo = {
    ApplicationId: string option
    AutoModerationRuleName: string option
    AutoModerationRuleTriggerType: string option
    ChannelId: string option
    Count: string option
    DeleteMemberDays: string option
    Id: string option
    MembersRemoved: string option
    MessageId: string option
    RoleName: string option
    Type: string option
    IntegrationType: string option
}

// TODO: Create DU for above for specific event types

// https://discord.com/developers/docs/resources/audit-log#audit-log-change-object
type AuditLogChange = {
    NewValue: obj option
    OldValue: obj option
    Key: string
}

// TODO: Handle exceptions for keys as documented, and consider if obj is most appropriate for old and new value (DU for all json types?)

// ----- Resources: Auto Moderation -----

// https://discord.com/developers/docs/resources/auto-moderation#auto-moderation-rule-object-auto-moderation-rule-structure
type AutoModerationRule = {
    Id: string
    GuildId: string
    Name: string
    CreatorId: string
    EventType: AutoModerationEventType
    TriggerType: AutoModerationTriggerType
    TriggerMetadata: AutoModerationTriggerMetadata
    Actions: AutoModerationAction list
    Enabled: bool
    ExemptRoles: string list
    ExemptChannels: string list
}

// https://discord.com/developers/docs/resources/auto-moderation#auto-moderation-rule-object-trigger-metadata
type AutoModerationTriggerMetadata = {
    KeywordFilter: string list option
    RegexPatterns: string list option
    Presets: AutoModerationKeywordPreset list option
    AllowList: string list option
    MentionTotalLimit: int option
    MentionRaidProtectionEnabled: bool option
}

// TODO: DU for trigger metadata types
// TODO: Handle trigger metadata field limits?

// https://discord.com/developers/docs/resources/auto-moderation#auto-moderation-action-object
type AutoModerationAction = {
    Type: AutoModerationActionType
    Metadata: AutoModerationActionMetadata option
}

// https://discord.com/developers/docs/resources/auto-moderation#auto-moderation-action-object-action-metadata
type AutoModerationActionMetadata = {
    ChannelId: string option
    DurationSeconds: int option
    CustomMessage: string option
}

// ----- Resources: Channel -----

// https://discord.com/developers/docs/resources/channel#channel-object-channel-structure
type Channel = {
    Id: string
    Type: ChannelType
    GuildId: string option
    Position: int option
    PermissionOverwrites: PermissionOverwrite list option
    Name: string option option
    Topic: string option option
    Nsfw: bool option
    LastMessageId: string option option
    Bitrate: int option
    UserLimit: int option
    RateLimitPerUser: int option
    Recipients: User list option
    Icon: string option option
    OwnerId: string option
    ApplicationId: string option
    Managed: bool option
    ParentId: string option option
    LastPinTimestamp: DateTime option option
    RtcRegion: string option option
    VideoQualityMode: VideoQualityMode option
    MessageCount: int option
    MemberCount: int option
    ThreadMetadata: ThreadMetadata option
    Member: ThreadMember option // only in certain endpoints (likely should be an ExtraField)
    DefaultAutoArchiveDuration: AutoArchiveDuration option
    Permissions: string option // TODO: Convert bitfield into permission list
    Flags: int option
    TotalMessagesSent: int option
    AvailableTags: ForumTag list option
    AppliedTags: string list option
    DefaultReactionEmoji: DefaultReaction option option
    DefaultThreadRateLimitPerUser: int option
    DefaultSortOrder: ChannelSortOrder option option
    DefaultForumLayout: ForumLayout option
}

and PartialChannel = {
    Id: string
    Type: ChannelType option
    GuildId: string option
    Position: int option
    PermissionOverwrites: PermissionOverwrite list option
    Name: string option option
    Topic: string option option
    Nsfw: bool option
    LastMessageId: string option option
    Bitrate: int option
    UserLimit: int option
    RateLimitPerUser: int option
    Recipients: User list option
    Icon: string option option
    OwnerId: string option
    ApplicationId: string option
    Managed: bool option
    ParentId: string option option
    LastPinTimestamp: DateTime option option
    RtcRegion: string option option
    VideoQualityMode: VideoQualityMode option
    MessageCount: int option
    MemberCount: int option
    ThreadMetadata: ThreadMetadata option
    Member: ThreadMember option // only in certain endpoints (likely should be an ExtraField)
    DefaultAutoArchiveDuration: AutoArchiveDuration option
    Permissions: string option // TODO: Convert bitfield into permission list
    Flags: int option
    TotalMessagesSent: int option
    AvailableTags: ForumTag list option
    AppliedTags: string list option
    DefaultReactionEmoji: DefaultReaction option option
    DefaultThreadRateLimitPerUser: int option
    DefaultSortOrder: ChannelSortOrder option option
    DefaultForumLayout: ForumLayout option
}

//type GuildTextChannel = BaseChannel
//type DmChannel = BaseChannel
//type GuildVoiceChannel = BaseChannel
//type GroupDmChannel = BaseChannel
//type GuildCategoryChannel = BaseChannel
//type GuildAnnouncementChannel = BaseChannel
//type AnnouncementThreadChannel = BaseChannel
//type PublicThreadChannel = BaseChannel
//type PrivateThreadChannel = BaseChannel
//type GuildStageVoiceChannel = BaseChannel
//type GuildDirectoryChannel = BaseChannel
//type GuildForumChannel = BaseChannel
//type GuildMediaChannel = BaseChannel

//type Channel =
//    | GUILD_TEXT          of GuildTextChannel
//    | DM                  of DmChannel
//    | GUILD_VOICE         of GuildVoiceChannel
//    | GROUP_DM            of GroupDmChannel
//    | GUILD_CATEGORY      of GuildCategoryChannel
//    | GUILD_ANNOUNCEMENT  of GuildAnnouncementChannel
//    | ANNOUNCEMENT_THREAD of AnnouncementThreadChannel
//    | PUBLIC_THREAD       of PublicThreadChannel
//    | PRIVATE_THREAD      of PrivateThreadChannel
//    | GUILD_STAGE_VOICE   of GuildStageVoiceChannel
//    | GUILD_DIRECTORY     of GuildDirectoryChannel
//    | GUILD_FORUM         of GuildForumChannel
//    | GUILD_MEDIA         of GuildMediaChannel

// https://discord.com/developers/docs/resources/channel#followed-channel-object-followed-channel-structure
type FollowedChannel = {
    ChannelId: string
    WebhookId: string
}

// https://discord.com/developers/docs/resources/channel#overwrite-object-overwrite-structure
type PermissionOverwrite = {
    Id: string
    Type: PermissionOverwriteType
    Allow: string
    Deny: string
}

and PartialPermissionOverwrite = {
    Id: string
    Type: PermissionOverwriteType option
    Allow: string option
    Deny: string option
}

// https://discord.com/developers/docs/resources/channel#thread-metadata-object-thread-metadata-structure
type ThreadMetadata = {
    Archived: bool
    AutoArchiveDuration: AutoArchiveDuration
    ArchiveTimestamp: DateTime
    Locked: bool
    Invitable: bool option
    CreateTimestamp: DateTime option option
}

// https://discord.com/developers/docs/resources/channel#thread-member-object-thread-member-structure
type ThreadMember = {
    Id: string option
    UserId: string option
    JoinTimestamp: DateTime
    Flags: int
    Member: GuildMember option
}

// https://discord.com/developers/docs/resources/channel#default-reaction-object-default-reaction-structure
type DefaultReaction = {
    EmojiId: string option
    EmojiName: string option
}

// https://discord.com/developers/docs/resources/channel#forum-tag-object-forum-tag-structure
type ForumTag = {
    Id: string
    Name: string
    Moderated: bool
    EmojiId: string option
    EmojiName: string option
}

// TODO: "When updating a GUILD_FORUM or a GUILD_MEDIA channel, tag objects in available_tags only require the name field."

// ----- Resources: Emoji -----

// https://discord.com/developers/docs/resources/emoji#emoji-object-emoji-structure
type Emoji = {
    Id: string option
    Name: string option
    Roles: string list option
    User: User option
    RequireColons: bool option
    Managed: bool option
    Animated: bool option
    Available: bool option
}

and PartialEmoji = Emoji // All emoji properties are already optional

// ----- Resources: Entitlement -----

// https://discord.com/developers/docs/resources/entitlement#entitlement-object-entitlement-structure
type Entitlement = {
    Id: string
    SkuId: string
    ApplicationId: string
    UserId: string option
    Type: EntitlementType
    Deleted: bool
    StartsAt: DateTime option
    EndsAt: DateTime option
    GuildId: string option
    Consumed: bool option
}

// ----- Resources: Guild -----

// https://discord.com/developers/docs/resources/guild#guild-object-guild-structure
type Guild = {
    Id: string
    Name: string
    Icon: string option
    IconHash: string option option
    Splash: string option
    DiscoverySplash: string option
    Owner: bool option
    OwnerId: string
    Permissions: string option
    AfkChannelId: string option
    AfkTimeout: int
    WidgetEnabled: bool option
    WidgetChannelId: string option option
    VerificationLevel: VerificationLevel
    DefaultMessageNotifications: MessageNotificationLevel
    ExplicitContentFilter: ExplicitContentFilterLevel
    Roles: Role list
    Emojis: Emoji list
    Features: GuildFeature list
    MfaLevel: MfaLevel
    ApplicationId: string option
    SystemChannelId: string option
    SystemChannelFlags: int
    RulesChannelId: string option
    MaxPresences: int option option
    MaxMembers: int option
    VanityUrlCode: string option
    Description: string option
    Banner: string option
    PremiumTier: GuildPremiumTier
    PremiumSubscriptionCount: int option
    PreferredLocale: string
    PublicUpdatesChannelId: string option
    MaxVideoChannelUsers: int option
    MaxStageVideoChannelUsers: int option
    ApproximateMemberCount: int option
    ApproximatePresenceCount: int option
    WelcomeScreen: WelcomeScreen option
    NsfwLevel: NsfwLevel
    Stickers: Sticker list option
    PremiumProgressBarEnabled: bool
    SafetyAlertsChannelId: string option
    IncidentsData: IncidentsData option
}

and PartialGuild = {
    Id: string
    Name: string option
    Icon: string option option
    IconHash: string option option
    Splash: string option option
    DiscoverySplash: string option option
    Owner: bool option
    OwnerId: string option
    Permissions: string option
    AfkChannelId: string option option
    AfkTimeout: int option
    WidgetEnabled: bool option
    WidgetChannelId: string option option
    VerificationLevel: VerificationLevel option
    DefaultMessageNotifications: MessageNotificationLevel option
    ExplicitContentFilter: ExplicitContentFilterLevel option
    Roles: Role list option
    Emojis: Emoji list option
    Features: GuildFeature list option
    MfaLevel: MfaLevel option
    ApplicationId: string option option
    SystemChannelId: string option option
    SystemChannelFlags: int option
    RulesChannelId: string option option
    MaxPresences: int option option
    MaxMembers: int option
    VanityUrlCode: string option option
    Description: string option option
    Banner: string option option
    PremiumTier: GuildPremiumTier option
    PremiumSubscriptionCount: int option
    PreferredLocale: string option
    PublicUpdatesChannelId: string option option
    MaxVideoChannelUsers: int option
    MaxStageVideoChannelUsers: int option
    ApproximateMemberCount: int option
    ApproximatePresenceCount: int option
    WelcomeScreen: WelcomeScreen option
    NsfwLevel: NsfwLevel option
    Stickers: Sticker list option
    PremiumProgressBarEnabled: bool option
    SafetyAlertsChannelId: string option option
    IncidentsData: IncidentsData option option
}

// https://discord.com/developers/docs/resources/guild#unavailable-guild-object
type UnavailableGuild = {
    Id: string
    Unavailable: bool
}

// https://discord.com/developers/docs/resources/guild#guild-preview-object-guild-preview-structure
type GuildPreview = {
    Id: string
    Name: string
    Icon: string option
    Splash: string option
    DiscoverySplash: string option
    Emojis: Emoji list
    Features: GuildFeature list
    ApproximateMemberCount: int
    ApproximatePresenceCount: int
    Description: string option
    Stickers: Sticker list
}

// https://discord.com/developers/docs/resources/guild#guild-widget-settings-object-guild-widget-settings-structure
type GuildWidgetSettings = {
    Enabled: bool
    ChannelId: string option
}

// https://discord.com/developers/docs/resources/guild#guild-widget-object-guild-widget-structure
type GuildWidget = {
    Id: string
    Name: string
    InstantInvite: string option
    Channels: PartialChannel list
    Members: PartialUser list
    PresenceCount: int
}

// https://discord.com/developers/docs/resources/guild#guild-member-object-guild-member-structure
type GuildMember = {
    User: User option // TODO: Not included in MESSAGE_CREATE and MESSAGE_UPDATE (how do handle this?)
    Nick: string option option
    Avatar: string option option
    Banner: string option option
    Roles: string list
    JoinedAt: DateTime
    PremiumSince: DateTime option option
    Deaf: bool
    Mute: bool
    Flags: int
    Pending: bool option // TODO: Only in GUILD_ events (should it be in ExtraFields?)
    Permissions: string option
    CommunicationDisabledUntil: DateTime option option
    AvatarDecorationData: AvatarDecorationData option option
}

and PartialGuildMember = {
    User: User option // TODO: Not included in MESSAGE_CREATE and MESSAGE_UPDATE (how do handle this?)
    Nick: string option option
    Avatar: string option option
    Banner: string option option
    Roles: string list option
    JoinedAt: DateTime option
    PremiumSince: DateTime option option
    Deaf: bool option
    Mute: bool option
    Flags: int option
    Pending: bool option // TODO: Only in GUILD_ events (should it be in ExtraFields?)
    Permissions: string option
    CommunicationDisabledUntil: DateTime option option
    AvatarDecorationData: AvatarDecorationData option option
}

// https://discord.com/developers/docs/resources/guild#integration-object-integration-structure
type Integration = {
    Id: string
    Name: string
    Type: GuildIntegrationType
    Enabled: bool
    Syncing: bool option
    RoleId: string option
    EnableEmoticons: bool option
    ExpireBehavior: IntegrationExpireBehavior option
    ExpireGracePeriod: int option
    User: User option
    Account: IntegrationAccount
    SyncedAt: DateTime option
    SubscriberCount: int option
    Revoked: bool option
    Application: IntegrationApplication option
    Scopes: OAuthScope list option
}

and PartialIntegration = {
    Id: string
    Name: string option
    Type: GuildIntegrationType option
    Enabled: bool option
    Syncing: bool option
    RoleId: string option
    EnableEmoticons: bool option
    ExpireBehavior: IntegrationExpireBehavior option
    ExpireGracePeriod: int option
    User: User option
    Account: IntegrationAccount option
    SyncedAt: DateTime option
    SubscriberCount: int option
    Revoked: bool option
    Application: IntegrationApplication option
    Scopes: OAuthScope list option
}

// https://discord.com/developers/docs/resources/guild#integration-account-object-integration-account-structure
type IntegrationAccount = {
    Id: string
    Name: string
}

// https://discord.com/developers/docs/resources/guild#integration-application-object-integration-application-structure
type IntegrationApplication = {
    Id: string
    Name: string
    Icon: string option
    Description: string
    Bot: User option
}

// https://discord.com/developers/docs/resources/guild#ban-object-ban-structure
type Ban = {
    Reason: string option
    User: User
}

// https://discord.com/developers/docs/resources/guild#welcome-screen-object-welcome-screen-structure
type WelcomeScreen = {
    Description: string option
    WelcomeChannels: WelcomeScreenChannel list
}

// https://discord.com/developers/docs/resources/guild#welcome-screen-object-welcome-screen-channel-structure
type WelcomeScreenChannel = {
    ChannelId: string
    Description: string
    EmojiId: string option
    EmojiName: string option
}

// https://discord.com/developers/docs/resources/guild#guild-onboarding-object-guild-onboarding-structure
type GuildOnboarding = {
    GuildId: string
    Prompts: GuildOnboardingPrompt list
    DefaultChannelIds: string list
    Enabled: bool
    Mode: OnboardingMode
}

// https://discord.com/developers/docs/resources/guild#guild-onboarding-object-onboarding-prompt-structure
type GuildOnboardingPrompt = {
    Id: string
    Type: OnboardingPromptType
    Options: GuildOnboardingPromptOption list
    Title: string
    SingleSelect: bool
    Required: bool
    InOnboarding: bool
}

// https://discord.com/developers/docs/resources/guild#guild-onboarding-object-prompt-option-structure
type GuildOnboardingPromptOption = {
    Id: string
    ChannelIds: string list
    RoleIds: string list
    Emoji: Emoji option
    EmojiId: string option
    EmojiName: string option
    EmojiAnimated: bool option
    Title: string
    Description: string option
}

// https://discord.com/developers/docs/resources/guild#incidents-data-object-incidents-data-structure
type IncidentsData = {
    InvitesDisabledUntil: DateTime option
    DmsDisabledUntil: DateTime option
    DmSpamDetectedAt: DateTime option option
    RaidDetectedAt: DateTime option option
}

// ----- Resources: Guild Scheduled Event -----

// https://discord.com/developers/docs/resources/guild-scheduled-event#guild-scheduled-event-object-guild-scheduled-event-structure
type GuildScheduledEvent = {
    Id: string
    GuildId: string
    ChannelId: string option
    CreatorId: string option option
    Name: string
    Description: string option option
    ScheduledStartTime: DateTime
    ScheduledEndTime: DateTime option
    PrivacyLevel: PrivacyLevel
    Status: EventStatus
    EntityType: ScheduledEntityType
    EntityId: string option
    EntityMetadata: EntityMetadata option
    Creator: User option
    UserCount: int option
    Image: string option option
    RecurrenceRule: RecurrenceRule option
}

// https://discord.com/developers/docs/resources/guild-scheduled-event#guild-scheduled-event-object-guild-scheduled-event-entity-metadata
type EntityMetadata = {
    [<JsonPropertyName "location">] Location: string option
}

// https://discord.com/developers/docs/resources/guild-scheduled-event#guild-scheduled-event-user-object-guild-scheduled-event-user-structure
type GuildScheduledEventUser = {
    GuildScheduledEventId: string
    User: User
    Member: GuildMember option
}

// https://discord.com/developers/docs/resources/guild-scheduled-event#guild-scheduled-event-recurrence-rule-object
type RecurrenceRule = {
    Start: DateTime
    End: DateTime option
    Frequency: RecurrenceRuleFrequency
    Interval: int
    ByWeekday: RecurrenceRuleWeekday list option
    ByWeekend: RecurrenceRuleNWeekday list option
    ByMonth: RecurrenceRuleMonth list option
    ByMonthDay: int list option
    ByYearDay: int list option
    Count: int option
}

// TODO: Handle documented limitations?

// https://discord.com/developers/docs/resources/guild-scheduled-event#guild-scheduled-event-recurrence-rule-object-guild-scheduled-event-recurrence-rule-nweekday-structure
type RecurrenceRuleNWeekday = {
    N: int
    Day: RecurrenceRuleWeekday
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

[<JsonConverter(typeof<MessageNonceConverter>)>]
[<RequireQualifiedAccess>]
type MessageNonce =
    | Number of int
    | String of string

and MessageNonceConverter () =
    inherit JsonConverter<MessageNonce> ()

    override _.Read (reader, _, _) =
        match reader.TokenType with
        | JsonTokenType.Number -> MessageNonce.Number (reader.GetInt32())
        | JsonTokenType.String -> MessageNonce.String (reader.GetString())
        | _ -> raise (JsonException "Unexpected MessageNonce value")

    override _.Write (writer, value, _) =
        match value with
        | MessageNonce.Number v -> writer.WriteNumberValue v
        | MessageNonce.String v -> writer.WriteStringValue v

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
