﻿module FSharp.Discord.Rest.DiscordRequest

open System.Net.Http
open System.Web

let withAuditLogReason (auditLogReason: string option) (req: HttpRequestMessage) =
    match auditLogReason with
    | None -> ()
    | Some auditLogReason ->
        let urlencoded = HttpUtility.UrlEncode auditLogReason
        req.Headers.Add("X-Audit-Log-Reason", urlencoded)

    req
