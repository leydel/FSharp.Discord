﻿namespace FSharp.Discord

open FSharp.Discord.Rest

type OAuthGetGuildsParams =
    | Origin of PaginationOrigin
    | Limit of int
    | WithCounts of bool

type OAuthUpdateRoleConnection = 
    | PlatformName of string
    | PlatformUsername of string
    | Metadata of (string * string) seq

module OAuth =
    let getUser client = task {
        let! res = Rest.getCurrentUser (DiscordClient.OAuth client)
        return DiscordResponse.toOption res
    }

    let getConnections client = task {
        let! res = Rest.getCurrentUserConnections client
        return DiscordResponse.toOption res
    }

    let getGuilds (parameters: OAuthGetGuildsParams list) client = task {
        let before = parameters |> List.tryPick (function | OAuthGetGuildsParams.Origin (PaginationOrigin.Before v) -> Some v | _ -> None)
        let after = parameters |> List.tryPick (function | OAuthGetGuildsParams.Origin (PaginationOrigin.After v) -> Some v | _ -> None)
        let limit = parameters |> List.tryPick (function | OAuthGetGuildsParams.Limit v -> Some v | _ -> None)
        let withCounts = parameters |> List.tryPick (function | OAuthGetGuildsParams.WithCounts v -> Some v | _ -> None)

        let! res = Rest.getCurrentUserGuilds before after limit withCounts (DiscordClient.OAuth client)
        return DiscordResponse.toOption res
    }

    let getGuildMember guildId client = task {
        let! res = Rest.getCurrentUserGuildMember guildId (DiscordClient.OAuth client)
        return DiscordResponse.toOption res
    }

    let getRoleConnection applicationId client = task {
        let! res = Rest.getCurrentUserApplicationRoleConnection applicationId client
        return DiscordResponse.toOption res
    }

    let updateRoleConnection applicationId (optionals: OAuthUpdateRoleConnection list) client = task {
        let platformName = optionals |> List.tryPick (function | OAuthUpdateRoleConnection.PlatformName v -> Some v | _ -> None)
        let platformUsername = optionals |> List.tryPick (function | OAuthUpdateRoleConnection.PlatformUsername v -> Some v | _ -> None)
        let metadata = optionals |> List.tryPick (function | OAuthUpdateRoleConnection.Metadata v -> dict v |> Some | _ -> None)

        let payload = UpdateCurrentUserApplicationRoleConnectionPayload(
            ?platform_name = platformName,
            ?platform_username = platformUsername,
            ?metadata = metadata
        )

        let! res = Rest.updateCurrentUserApplicationRoleConnection applicationId payload client
        return DiscordResponse.toOption res
    }

    let getApplication client = task {
        let! res = Rest.getCurrentBotApplicationInformation client
        return DiscordResponse.toOption res
    }

    let getAuthorizationInformation client = task {
        let! res = Rest.getCurrentAuthorizationInformation client
        return DiscordResponse.toOption res
    }
