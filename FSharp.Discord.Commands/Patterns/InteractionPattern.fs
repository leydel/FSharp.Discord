﻿[<AutoOpen>]
module FSharp.Discord.Commands.InteractionPattern

open FSharp.Discord.Types

// TODO: Create active patterns for other necessary interaction properties
// TODO: Figure out appropriate method for handling "focused" options for autocomplete

// Should these only be used internally in a form of "match | interaction when Interaction.matches commandDefinition interaction"?
// Should these only be used internally in a form of "Interaction.tryGetStringOption(name): string option"?
// Maybe both should be created? Might make sense to rework these option patterns to not be on the interaction itself... WILL NOT WORK FOR SUBCOMMANDS BECAUSE OF THIS

let (|SubCommandOption|_|) (name: string) (interaction: Interaction) =
    match interaction.Data with
    | Some (InteractionData.APPLICATION_COMMAND { Options = Some options }) ->
        options |> List.tryPick (fun option ->
            match option with
            | ({
                Type = ApplicationCommandOptionType.SUB_COMMAND
                Options = Some opts
            }) when option.Name = name -> Some opts
            | _ -> None
        )
    | _ -> None

let (|SubCommandGroupOption|_|) (name: string) (interaction: Interaction) =
    match interaction.Data with
    | Some (InteractionData.APPLICATION_COMMAND { Options = Some options }) ->
        options |> List.tryPick (fun option ->
            match option with
            | ({
                Options = Some options
                Type = ApplicationCommandOptionType.SUB_COMMAND_GROUP
            }) when option.Name = name -> Some options
            | _ -> None
        )
    | _ -> None

let (|StringOption|_|) (name: string) (interaction: Interaction) =
    match interaction.Data with
    | Some (InteractionData.APPLICATION_COMMAND { Options = Some options }) ->
        options |> List.tryPick (fun option ->
            match option with
            | ({
                Value = Some (CommandInteractionDataOptionValue.String value)
                Type = ApplicationCommandOptionType.STRING
            }) when option.Name = name -> Some value
            | _ -> None
        )
    | _ -> None

let (|IntegerOption|_|) (name: string) (interaction: Interaction) =
    match interaction.Data with
    | Some (InteractionData.APPLICATION_COMMAND { Options = Some options }) ->
        options |> List.tryPick (fun option ->
            match option with
            | ({
                Value = Some (CommandInteractionDataOptionValue.Int value)
                Type = ApplicationCommandOptionType.INTEGER
            }) when option.Name = name -> Some value
            | _ -> None
        )
    | _ -> None

let (|BooleanOption|_|) (name: string) (interaction: Interaction) =
    match interaction.Data with
    | Some (InteractionData.APPLICATION_COMMAND { Options = Some options }) ->
        options |> List.tryPick (fun option ->
            match option with
            | ({
                Value = Some (CommandInteractionDataOptionValue.Bool value)
                Type = ApplicationCommandOptionType.BOOLEAN
            }) when option.Name = name -> Some value
            | _ -> None
        )
    | _ -> None

let (|UserIdOption|_|) (name: string) (interaction: Interaction) =
    match interaction.Data with
    | Some (InteractionData.APPLICATION_COMMAND { Options = Some options }) ->
        options |> List.tryPick (fun option ->
            match option with
            | ({
                Type = ApplicationCommandOptionType.USER
                Value = Some (CommandInteractionDataOptionValue.String userId)
            }) when option.Name = name -> Some userId
            | _ -> None
        )
    | _ -> None


let (|UserOption|_|) (name: string) (interaction: Interaction) =
    match interaction.Data with
    | Some (InteractionData.APPLICATION_COMMAND { Options = Some options; Resolved = Some { Users = users } }) ->
        options |> List.tryPick (fun option ->
            match option with
            | ({
                Type = ApplicationCommandOptionType.USER
                Value = Some (CommandInteractionDataOptionValue.String userId)
            }) when option.Name = name -> users |> Seq.tryFind (fun (id, _) -> id = userId) |> Option.map snd
            | _ -> None
        )
    | _ -> None

let (|ChannelIdOption|_|) (name: string) (interaction: Interaction) =
    match interaction.Data with
    | Some (InteractionData.APPLICATION_COMMAND { Options = Some options }) ->
        options |> List.tryPick (fun option ->
            match option with
            | ({
                Value = Some (CommandInteractionDataOptionValue.String channelId)
                Type = ApplicationCommandOptionType.CHANNEL
            }) when option.Name = name -> Some channelId
            | _ -> None
        )
    | _ -> None

let (|ChannelOption|_|) (name: string) (interaction: Interaction) =
    match interaction.Data with
    | Some (InteractionData.APPLICATION_COMMAND { Options = Some options; Resolved = Some { Channels = channels } }) ->
        options |> List.tryPick (fun option ->
            match option with
            | ({
                Value = Some (CommandInteractionDataOptionValue.String channelId)
                Type = ApplicationCommandOptionType.CHANNEL
            }) when option.Name = name -> channels |> Seq.tryFind (fun (id, _) -> id = channelId) |> Option.map snd
            | _ -> None
        )
    | _ -> None

let (|RoleIdOption|_|) (name: string) (interaction: Interaction) =
    match interaction.Data with
    | Some (InteractionData.APPLICATION_COMMAND { Options = Some options }) ->
        options |> List.tryPick (fun option ->
            match option with
            | ({
                Value = Some (CommandInteractionDataOptionValue.String roleId)
                Type = ApplicationCommandOptionType.ROLE
            }) when option.Name = name -> Some roleId
            | _ -> None
        )
    | _ -> None

let (|RoleOption|_|) (name: string) (interaction: Interaction) =
    match interaction.Data with
    | Some (InteractionData.APPLICATION_COMMAND { Options = Some options; Resolved = Some { Roles = roles } }) ->
        options |> List.tryPick (fun option ->
            match option with
            | ({
                Value = Some (CommandInteractionDataOptionValue.String roleId)
                Type = ApplicationCommandOptionType.ROLE
            }) when option.Name = name -> roles |> Seq.tryFind (fun (id, _) -> id = roleId) |> Option.map snd
            | _ -> None
        )
    | _ -> None

let (|MentionableIdOption|_|) (name: string) (interaction: Interaction) =
    match interaction.Data with
    | Some (InteractionData.APPLICATION_COMMAND { Options = Some options }) ->
        options |> List.tryPick (fun option ->
            match option with
            | ({
                Value = Some (CommandInteractionDataOptionValue.String mentionableId)
                Type = ApplicationCommandOptionType.MENTIONABLE
            }) when option.Name = name -> Some mentionableId
            | _ -> None
        )
    | _ -> None

type Mentionable =
    | User of User
    | Role of Role

let (|MentionableOption|_|) (name: string) (interaction: Interaction) =
    match interaction.Data with
    | Some (InteractionData.APPLICATION_COMMAND { Options = Some options; Resolved = Some { Users = users; Roles = roles; } }) ->
        options |> List.tryPick (fun option ->
            match option with
            | ({
                Value = Some (CommandInteractionDataOptionValue.String mentionableId)
                Type = ApplicationCommandOptionType.MENTIONABLE
            }) when option.Name = name ->
                let mentionables =
                    let userList = users |> Seq.map (fun (id, v) -> (id, Mentionable.User v)) |> Seq.toList
                    let roleList = roles |> Seq.map (fun (id, v) -> (id, Mentionable.Role v)) |> Seq.toList
                    userList @ roleList

                mentionables |> Seq.tryFind (fun (id, _) -> id = mentionableId) |> Option.map snd
            | _ -> None
        )
    | _ -> None
