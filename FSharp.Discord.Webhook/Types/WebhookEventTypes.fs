﻿namespace FSharp.Discord.Webhook.Types

open FSharp.Discord.Types
open System
open System.Text.Json
open System.Text.Json.Serialization

// https://discord.com/developers/docs/events/webhook-events#event-body-object
type WebhookEventBody<'a> = {
    [<JsonPropertyName "type">] Type: WebhookEventType
    [<JsonPropertyName "timestamp">] [<JsonConverter(typeof<JsonConverter.UnixEpoch>)>] Timestamp: DateTime
    [<JsonPropertyName "data">] Data: 'a
}

// https://discord.com/developers/docs/events/webhook-events#payload-structure
type WebhookEventPayload<'a> = {
    [<JsonPropertyName "version">] Version: int
    [<JsonPropertyName "application_id">] ApplicationId: string
    [<JsonPropertyName "type">] Type: WebhookType
    [<JsonPropertyName "event">] Event: 'a
}

// https://discord.com/developers/docs/events/webhook-events#application-authorized-application-authorized-structure
type ApplicationAuthorizedEvent = {
    [<JsonPropertyName "integration_type">] IntegrationType: ApplicationIntegrationType option
    [<JsonPropertyName "user">] User: User
    [<JsonPropertyName "scopes">] Scopes: OAuth2Scope list
    [<JsonPropertyName "guild">] Guild: Guild option
}

// https://discord.com/developers/docs/events/webhook-events#entitlement-create-entitlement-create-structure
type EntitlementCreateEvent = Entitlement
