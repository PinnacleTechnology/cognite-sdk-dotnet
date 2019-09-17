﻿// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Assets

open System.IO
open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading

open Oryx
open Thoth.Json.Net

open CogniteSdk
open System.Threading.Tasks
open FSharp.Control.Tasks.V2.ContextInsensitive


type AssetQuery =
    private
    | CaseLimit of int
    | CaseCursor of string

    /// Max number of results to return
    static member Limit limit =
        if limit > MaxLimitSize || limit < 1 then
            failwith "Limit must be set to 1000 or less"
        CaseLimit limit
    /// Cursor return from previous request
    static member Cursor cursor = CaseCursor cursor

    static member Render (this: AssetQuery) =
        match this with
        | CaseLimit limit -> "limit", Encode.int limit
        | CaseCursor cursor -> "cursor", Encode.string cursor

[<RequireQualifiedAccess>]
module Items =
    [<Literal>]
    let Url = "/assets/list"

    type Request = {
        Filters : AssetFilter seq
        Options : AssetQuery seq
    } with
        member this.Encoder =
            Encode.object [
                yield "filter", Encode.object [
                    yield! this.Filters |> Seq.map AssetFilter.Render
                ]
                yield! this.Options |> Seq.map AssetQuery.Render
            ]

    let listCore (options: AssetQuery seq) (filters: AssetFilter seq) (fetch: HttpHandler<HttpResponseMessage, HttpResponseMessage, 'a>) =
        let decodeResponse = Decode.decodeContent AssetItemsReadDto.Decoder id
        let request : Request = {
            Filters = filters
            Options = options
        }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> Decode.decodeError
        >=> decodeResponse

    /// <summary>
    /// Retrieves list of assets matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <param name="filters">Search filters</param>
    /// <param name="next">Async handler to use</param>
    /// <returns>List of assets matching given filters and optional cursor</returns>
    let list (options: AssetQuery seq) (filters: AssetFilter seq) (next: NextFunc<AssetItemsReadDto,'a>) : HttpContext -> Task<Context<'a>> =
        listCore options filters fetch next

    /// <summary>
    /// Retrieves list of assets matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <param name="filters">Search filters</param>
    /// <returns>List of assets matching given filters and optional cursor</returns>
    let listAsync (options: AssetQuery seq) (filters: AssetFilter seq) : HttpContext -> Task<Context<AssetItemsReadDto>> =
        listCore options filters fetch Task.FromResult


[<Extension>]
type ListAssetsExtensions =
    /// <summary>
    /// Retrieves list of assets matching query, filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <param name="filters">Search filters</param>
    /// <returns>List of assets matching given filters and optional cursor</returns>
    [<Extension>]
    static member ListAsync (this: ClientExtension, options: AssetQuery seq, filters: AssetFilter seq, [<Optional>] token: CancellationToken) : Task<AssetItems> =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! ctx' = Items.listAsync options filters ctx
            match ctx'.Result with
            | Ok assets ->
                let cursor = if assets.NextCursor.IsSome then assets.NextCursor.Value else Unchecked.defaultof<string>
                let items : AssetItems = {
                    NextCursor = cursor
                    Items = assets.Items |> Seq.map (fun asset -> asset.ToAssetEntity ())
                }
                return items
            | Error error ->
                return raise (error.ToException ())
        }

    /// <summary>
    /// Retrieves list of assets matching filter.
    /// </summary>
    /// <param name="filters">Search filters</param>
    /// <returns>List of assets matching given filters and optional cursor</returns>
    [<Extension>]
    static member ListAsync (this: ClientExtension, filters: AssetFilter seq, [<Optional>] token: CancellationToken) : Task<AssetItems> =
        let query = ResizeArray<AssetQuery>()
        this.ListAsync(query, filters, token)

    /// <summary>
    /// Retrieves list of assets with a cursor if given limit is exceeded.
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <returns>List of assets matching given filters and optional cursor</returns>
    [<Extension>]
    static member ListAsync (this: ClientExtension, options: AssetQuery seq, [<Optional>] token: CancellationToken) : Task<AssetItems> =
        let filter = ResizeArray<AssetFilter>()
        this.ListAsync(options, filter, token)
