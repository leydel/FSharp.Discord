﻿namespace rec FSharp.Discord.Types.Serialization

open System
open Thoth.Json.Net

module Decode =
    /// Decode a map with a custom key value mapping from the provided string.
    let mapkv (keyMapper: string -> 'a option) (valueDecoder: Decoder<'b>) path v =
        let decoded = Decode.dict valueDecoder path v

        match decoded with
        | Error err -> Error err
        | Ok d ->
            d
            |> Map.toSeq
            |> Seq.fold
                (fun acc cur -> acc |> Result.bind (fun acc ->
                    match keyMapper (fst cur) with
                    | None -> Error (path, BadField("an invalid key", v))
                    | Some k -> Ok (acc |> Seq.append (seq { k, snd cur }))
                ))
                (Ok [])
            |> Result.map (Map.ofSeq)

module Encode =
    /// Append an encoding that is required.
    let required key decoder v list =
        list @ [key, decoder v]

    /// Append an encoding that is optional.
    let optional key decoder v list =
        match v with
        | Some s -> list @ [key, decoder s]
        | None -> list

    /// Append an encoding that is nullable.
    let nullable key decoder v list =
        list @ [key, Encode.option decoder v]

    /// Append an encoding that is optional and nullable.
    let optinull key decoder v list =
        match v with
        | Some s -> list @ [key, Encode.option decoder s]
        | None -> list

    /// Encode a map to an object with a value encoder.
    let mapv (encoder: Encoder<'a>) value =
        Map.map (fun _ v -> encoder v) value
        |> Encode.dict

    /// Encode a map to an object with a key mapper and value encoder.
    let mapkv (mapper: 'a -> string) (encoder: Encoder<'b>) value =
        value
        |> Map.toSeq
        |> Seq.map (fun (k, v) -> mapper k, encoder v)
        |> Map.ofSeq
        |> Encode.dict
        
module Get =
    /// Get a required decoded value.
    let required key decoder (get: Decode.IGetters) =
        get.Required.Field key decoder

    /// Get an optional decoded value.
    let optional key decoder (get: Decode.IGetters) =
        get.Optional.Field key decoder

    /// Get a nullable decoded value.
    let nullable key decoder (get: Decode.IGetters) =
        get.Required.Field key (Decode.option decoder)

    /// Get an optional and nullable decoded value.
    let optinull key decoder (get: Decode.IGetters) =
        get.Optional.Raw (Decode.field key (Decode.option decoder))
        
module UnixTimestamp =
    let decoder path v =
        Decode.map (DateTimeOffset.FromUnixTimeMilliseconds >> _.DateTime) Decode.int64 path v

    let encoder (v: DateTime) =
        DateTimeOffset v |> _.ToUnixTimeMilliseconds() |> Encode.int64
