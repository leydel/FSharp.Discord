﻿module FSharp.Discord.Gateway.GatewayState

open Elmish
open FSharp.Discord.Types
open System
open System.Threading
open System.Threading.Tasks

type QueuedSendEvent =
    | Pending of GatewaySendEvent
    | Processing

type Model = {
    // Args
    GatewayUrl: string
    IdentifyEvent: IdentifySendEvent
    Handler: GatewayHandler

    // Lifecycle state
    Interval: int option
    ResumeGatewayUrl: string option
    SessionId: string option
    SequenceId: int option
    Heartbeat: DateTime option
    HeartbeatAcked: bool

    // Child Models
    SendQueue: QueuedSendEvent list

    // Other state
    Socket: ISocket option
}

type Msg =
    | Receive of GatewayReceiveEvent * string

    // TODO: Create message to initialise with connect (and run as cmd on init?)
    // TODO: Figure out how to handle disconnect and termination (probably needs a msg here)

    | Connect of forceReconnect: bool
    | Ready
    | Heartbeat
    
    | Send of GatewaySendEvent
    | StartProcessNext
    | EndProcessNext

    | Ignore

let init (gatewayUrl, identifyEvent, handler) =
    {
        GatewayUrl = gatewayUrl
        IdentifyEvent = identifyEvent
        Handler = handler
        Interval = None
        ResumeGatewayUrl = None
        SessionId = None
        SequenceId = None
        Heartbeat = None
        HeartbeatAcked = true
        SendQueue = []
        Socket = None
    },
    Cmd.ofMsg (Msg.Connect true)

let private receive model (ev: GatewayReceiveEvent) (raw: string) =
    match ev, raw with
    | GatewayReceiveEvent.HELLO ev, _ ->
        let event =
            match model.SessionId, model.SequenceId with
            | Some sessionId, Some sequenceId ->
                let payload = ResumeSendEvent.create(model.IdentifyEvent.Token, sessionId, sequenceId)
                GatewayEventPayload.create(GatewayOpcode.RESUME, payload) |> GatewaySendEvent.RESUME

            | _ ->
                GatewayEventPayload.create(GatewayOpcode.IDENTIFY, model.IdentifyEvent) |> GatewaySendEvent.IDENTIFY

        { model with Interval = Some ev.Data.HeartbeatInterval }, Cmd.ofMsg (Msg.Send event)

    | GatewayReceiveEvent.HEARTBEAT _, _ ->
        model, Cmd.ofMsg Msg.Heartbeat

    | GatewayReceiveEvent.HEARTBEAT_ACK ev, _ ->
        { model with HeartbeatAcked = true }, Cmd.none

    | GatewayReceiveEvent.READY ev, _ ->
        { model with ResumeGatewayUrl = Some ev.Data.ResumeGatewayUrl; SessionId = Some ev.Data.SessionId },
        Cmd.none

    | GatewayReceiveEvent.RESUMED _, _ ->
        model, Cmd.none

    | GatewayReceiveEvent.RECONNECT _, _ ->
        model, Cmd.ofMsg (Msg.Connect false)

    | GatewayReceiveEvent.INVALID_SESSION ev, _ ->
        model, Cmd.ofMsg (Msg.Connect (not ev.Data))

    | _, raw ->
        model, Cmd.OfAsync.perform (fun r -> model.Handler r |> Async.AwaitTask) raw (fun _ -> Msg.Ignore) // TODO: Can Msg.Ignore be removed? Cmd.ofEffect?

let ready model =
    let ev =
        GatewayReceiveEvent.UNKNOWN (GatewayEventPayload.create (CustomGatewayOpcode.cast CustomGatewayOpcode.ACTIVE, ())),
        $"""{{"op": {CustomGatewayOpcode.toString CustomGatewayOpcode.ACTIVE}}}"""
    
    // TODO: Clean up this (should this even be sending in the same way as a custom event?)

    model, Cmd.ofMsg (Msg.Receive ev)

let connect (env: #ISocketFactory) model forceReconnect =
    // TODO: Check if socket already present, if so, close it then reconenct/resume based on its state and `forceReconnect`
    // TODO: Detect if reconnecting, and send custom INACTIVE event if doing reconnect

    let uri, event =
        match forceReconnect, model.ResumeGatewayUrl, model.SessionId, model.SequenceId with
        | false, Some resumeGatewayUrl, Some sessionId, Some sequenceId ->
            let payload = ResumeSendEvent.create (model.IdentifyEvent.Token, sessionId, sequenceId)
            let event = GatewaySendEvent.RESUME (GatewayEventPayload.create (GatewayOpcode.RESUME, payload))
            Uri resumeGatewayUrl, event

        | _ ->
            let event = GatewaySendEvent.IDENTIFY (GatewayEventPayload.create (GatewayOpcode.IDENTIFY, model.IdentifyEvent))
            Uri model.GatewayUrl, event

    let socket = env.CreateSocket()

    // TODO: Connect socket (do async in effect cmd?) then send event from above

    { model with Socket = Some socket }, Cmd.ofMsg (Msg.Send event)

let heartbeat (env: #IGetCurrentTime) model =
    let event = GatewayEventPayload.create(GatewayOpcode.HEARTBEAT, model.SequenceId) |> GatewaySendEvent.HEARTBEAT

    { model with
        HeartbeatAcked = false
        Heartbeat = model.Interval |> Option.map (env.GetCurrentTime().AddMilliseconds) },
    Cmd.ofMsg (Msg.Send event)

let private send model (ev: GatewaySendEvent) =
    { model with SendQueue = model.SendQueue @ [QueuedSendEvent.Pending ev] },
    Cmd.ofMsg Msg.StartProcessNext

let private startProcessNext env model =
    match model.SendQueue |> List.tryHead with
    | Some (QueuedSendEvent.Pending ev) ->
        { model with SendQueue = model.SendQueue |> List.skip 1 |> List.append [QueuedSendEvent.Processing] },
        Cmd.ofEffect (fun dispatch -> (async {
            // TODO: Send event `ev` here (where does the dependency to send gateway events come from?)

            dispatch (Msg.EndProcessNext)
        } |> Async.StartImmediate)) // TODO: Test if this is blocking or not

    | _ -> model, Cmd.none

let private endProcessNext model =
    { model with SendQueue = model.SendQueue |> List.skip 1 },
    Cmd.ofMsg Msg.StartProcessNext

    // TODO: Should this somehow filter for `QueuedSendEvent.Processing` in some way to ensure it doesn't get stuck?

let update env msg model =
    match msg with
    | Msg.Receive (ev, raw) -> receive model ev raw

    | Msg.Connect forceReconnect -> connect env model forceReconnect
    | Msg.Ready -> ready model
    | Msg.Heartbeat -> heartbeat env model

    | Msg.Send ev -> send model ev
    | Msg.StartProcessNext -> startProcessNext env model
    | Msg.EndProcessNext -> endProcessNext model

    | Msg.Ignore -> model, Cmd.none

let view model dispatch =
    ()

let subscribe (env: #IGetCurrentTime) model =
    let socket =
        match model.Socket with
        | Some socket ->
            [
                ["websocket"; socket.Id.ToString()],
                fun dispatch -> socket.Subscribe(fun (ev, raw) -> dispatch (Msg.Receive (ev, raw)));
            ]
        | _ -> []

    let delay (timespan: TimeSpan) (callback: unit -> unit) =
        use cts = new CancellationTokenSource()
        Task.Delay(timespan, cts.Token).ContinueWith(fun _ -> callback()) |> ignore
        { new IDisposable with member _.Dispose () = cts.Cancel() }

        // TODO: Test if ignore and the cts work here

    let heartbeat =
        match model.HeartbeatAcked, model.Heartbeat with
        | false, Some due ->
            [
                ["heartbeat"; "notacked"; Guid.NewGuid().ToString()],
                fun dispatch -> delay (due.Subtract (env.GetCurrentTime())) (fun () -> dispatch (Msg.Connect false))
            ]
        | true, Some due ->
            [
                ["heartbeat"; "acked"; Guid.NewGuid().ToString()],
                fun dispatch -> delay (due.Subtract (env.GetCurrentTime())) (fun () -> dispatch Msg.Heartbeat)
            ]
        | _ -> []

    socket @ heartbeat

let program env gatewayUrl identify handler =
    Task.Run(fun () ->
        Program.mkProgram init (update env) view
        |> Program.withSubscription (subscribe env)
        |> Program.runWith (gatewayUrl, identify, handler)
    )

    // TODO: Check if this resolves when program ends, otherwise figure out `withSubscription`/`withTermination` type logic

// TODO: Figure out how to notify that the gateway disconencted irrecoverably (likely cant use `Program.runWith` as it starts single threaded loop)
