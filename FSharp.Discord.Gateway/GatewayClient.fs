﻿namespace FSharp.Discord.Gateway

open FSharp.Discord.Types
open System
open System.Net.WebSockets
open System.Threading
open System.Threading.Tasks

type IGatewayClient =
    abstract member RunAsync:
        gatewayUrl: string ->
        identifyEvent: IdentifySendEvent ->
        handler: (string -> Task<unit>) ->
        ct: CancellationToken ->
        Task<GatewayCloseEventCode option>

    abstract member RequestGuildMembers:
        RequestGuildMembersSendEvent ->
        Task<bool>

    abstract member RequestSoundboardSounds:
        RequestSoundboardSoundsSendEvent ->
        Task<bool>

    abstract member UpdateVoiceState:
        UpdateVoiceStateSendEvent ->
        Task<bool>

    abstract member UpdatePresence:
        UpdatePresenceSendEvent ->
        Task<bool>

type GatewayClient () =
    let _ws: ClientWebSocket option ref = ref None

    interface IGatewayClient with
        member _.RunAsync gatewayUrl identifyEvent handler ct =
            let state = ConnectState.zero gatewayUrl identifyEvent
            Gateway.connect state handler _ws ct

        member _.RequestGuildMembers payload = task {
            let event =
                GatewayEventPayload.create(GatewayOpcode.REQUEST_GUILD_MEMBERS, payload)
                |> GatewaySendEvent.REQUEST_GUILD_MEMBERS

            return!
                _ws.Value
                |> Option.map (fun ws -> ws |> Gateway.send event |> Task.map (fun _ -> true))
                |> Option.defaultValue (Task.FromResult false)
        }

        member _.RequestSoundboardSounds payload = task {
            let event =
                GatewayEventPayload.create(GatewayOpcode.REQUEST_SOUNDBOARD_SOUNDS, payload)
                |> GatewaySendEvent.REQUEST_SOUNDBOARD_SOUNDS

            return!
                _ws.Value
                |> Option.map (fun ws -> ws |> Gateway.send event |> Task.map (fun _ -> true))
                |> Option.defaultValue (Task.FromResult false)
        }

        member _.UpdateVoiceState payload = task {
            let event =
                GatewayEventPayload.create(GatewayOpcode.VOICE_STATE_UPDATE, payload)
                |> GatewaySendEvent.UPDATE_VOICE_STATE

            return!
                _ws.Value
                |> Option.map (fun ws -> ws |> Gateway.send event |> Task.map (fun _ -> true))
                |> Option.defaultValue (Task.FromResult false)
        }

        member _.UpdatePresence payload = task {
            let event =
                GatewayEventPayload.create(GatewayOpcode.PRESENCE_UPDATE, payload)
                |> GatewaySendEvent.UPDATE_PRESENCE

            return!
                _ws.Value
                |> Option.map (fun ws -> ws |> Gateway.send event |> Task.map (fun _ -> true))
                |> Option.defaultValue (Task.FromResult false)
        }

    interface IDisposable with
        member _.Dispose () =
            match _ws.Value with
            | Some ws -> ws.Dispose()
            | None -> ()
