﻿namespace FSharp.Discord.Webhook

open FSharp.Discord.Types
open FSharp.Discord.Types.Serialization
open System.Text.Json
open System.Text.Json.Serialization

[<JsonConverter(typeof<WebhookReceiveEventConverter>)>]
type WebhookReceiveEvent =
    | PING                   of WebhookReceiveEventPayload<unit>
    | ENTITLEMENT_CREATE     of WebhookReceiveEventPayload<WebhookReceiveEventBody<EntitlementCreateEvent>>
    | APPLICATION_AUTHORIZED of WebhookReceiveEventPayload<WebhookReceiveEventBody<ApplicationAuthorizedEvent>>
    | UNKNOWN                of WebhookReceiveEventPayload<obj>

and WebhookReceiveEventConverter () =
    inherit JsonConverter<WebhookReceiveEvent> ()

    override __.Read (reader, _, _) =
        let success, document = JsonDocument.TryParseValue &reader
        JsonException.raiseIf (not success)

        let webhookType =
            document.RootElement.GetProperty "type"
            |> _.GetInt32()
            |> enum<WebhookPayloadType>

        let webhookEventType =
            try
                document.RootElement.GetProperty "data"
                |> _.GetProperty("type")
                |> _.GetString()
                |> WebhookEventType.fromString
            with | _ ->
                None

        let json = document.RootElement.GetRawText()

        match webhookType, webhookEventType with
        | WebhookPayloadType.PING, _ -> PING <| Json.deserializeF json
        | WebhookPayloadType.EVENT, Some WebhookEventType.ENTITLEMENT_CREATE -> ENTITLEMENT_CREATE <| Json.deserializeF json
        | WebhookPayloadType.EVENT, Some WebhookEventType.APPLICATION_AUTHORIZED -> APPLICATION_AUTHORIZED <| Json.deserializeF json
        | _ -> UNKNOWN <| Json.deserializeF json
                
    override __.Write (writer, value, _) =
        match value with
        | PING p -> Json.serializeF p |> writer.WriteRawValue
        | ENTITLEMENT_CREATE e -> Json.serializeF e |> writer.WriteRawValue
        | APPLICATION_AUTHORIZED a -> Json.serializeF a |> writer.WriteRawValue
        | UNKNOWN u -> Json.serializeF u |> writer.WriteRawValue
        
// TODO: Rewrite serializer with thoth separately
