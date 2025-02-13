﻿namespace FSharp.Discord.Gateway

open FSharp.Discord.Types
open System
open System.Net.WebSockets
open System.Threading
open System.Threading.Tasks

type IGatewayClient =
    abstract Connect:
        gatewayUrl: string ->
        identify: IdentifySendEvent ->
        handler: GatewayHandler ->
        cancellationToken: CancellationToken ->
        Task<GatewayCloseEventCode option>

    abstract RequestGuildMembers:
        guildId: string ->
        query: string option ->
        limit: int ->
        presences: bool option ->
        userIds: string list option ->
        nonce: string option ->
        cancellationToken: CancellationToken ->
        Task<Result<unit, WebsocketWriteError>>

    abstract RequestSoundboardSounds:
        guildIds: string list ->
        cancellationToken: CancellationToken ->
        Task<Result<unit, WebsocketWriteError>>

    abstract UpdateVoiceState:
        guildId: string ->
        channelId: string option ->
        selfMute: bool ->
        selfDeaf: bool ->
        cancellationToken: CancellationToken ->
        Task<Result<unit, WebsocketWriteError>>

    abstract UpdatePresence:
        since: int option ->
        activities: Activity list option ->
        status: Status ->
        afk: bool option ->
        cancellationToken: CancellationToken ->
        Task<Result<unit, WebsocketWriteError>>

type GatewayClient () =
    member val private _ws: ClientWebSocket ref = ref (new ClientWebSocket()) with get, set

    interface IGatewayClient with
        member this.Connect gatewayUrl identify handler ct = task {
            let mutable disconnectReason = ReconnectableGatewayDisconnect.Reconnect
            let mutable closeCode: GatewayCloseEventCode option option = None

            while Option.isNone closeCode do
                this._ws.Value <- new ClientWebSocket()
            
                let! disconnect =
                    match disconnectReason with
                    | ReconnectableGatewayDisconnect.Reconnect -> Gateway.connect identify handler gatewayUrl None ct this._ws.Value
                    | ReconnectableGatewayDisconnect.Resume resumeData -> Gateway.connect identify handler gatewayUrl (Some resumeData) ct this._ws.Value

                match disconnect with
                | Error code -> closeCode <- Some code
                | Ok reason -> disconnectReason <- reason

            return closeCode |> Option.bind id
        }

        member this.RequestGuildMembers guildId query limit presences userIds nonce ct = task {
            return! Gateway.requestGuildMembers guildId query limit presences userIds nonce ct this._ws.Value
        }
        
        member this.RequestSoundboardSounds guildIds ct = task {
            return! Gateway.requestSoundboardSounds guildIds ct this._ws.Value
        }

        member this.UpdateVoiceState guildId channelId selfMute selfDeaf ct = task {
            return! Gateway.updateVoiceState guildId channelId selfMute selfDeaf ct this._ws.Value
        }

        member this.UpdatePresence since activities status afk ct = task {
            return! Gateway.updatePresence since activities status afk ct this._ws.Value
        }

    interface IDisposable with
        member this.Dispose () =
            this._ws.Value.Dispose() // TODO: Figure out proper disposal to gracefully disconnect (probably needs to be IAsyncDisposable)

    // TODO: Figure out what should be getting injected or partially applied so that the gateway client can be reasonably testable
