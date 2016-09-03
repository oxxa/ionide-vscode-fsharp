namespace Ionide.VSCode.FSharp

open System
open Fable.Core
open Fable.Import
open Fable.Import.vscode
open Fable.Import.Node

open DTO
open Ionide.VSCode.Helpers

module Reference =
    let private createProvider () =

        let mapResult (doc : TextDocument) (o : SymbolUseResult) =
            if o |> unbox <> null then
                o.Data.Uses |> Array.map (fun s ->
                    let loc = createEmpty<Location>
                    loc.range <-  Range(float s.StartLine - 1., float s.StartColumn - 1., float s.EndLine - 1., float s.EndColumn - 1.)
                    loc.uri <- Uri.file s.FileName
                    loc  )
                |> ResizeArray
            else
                ResizeArray ()

        { new ReferenceProvider
          with
            member this.provideReferences(doc, pos, _, _) =
                promise {
                    let! res = LanguageService.symbolUseProject (doc.fileName) (int pos.line + 1) (int pos.character + 1)
                    return mapResult doc res
                } |> Case2
        }

    let activate selector (disposables: Disposable[]) =
        languages.registerReferenceProvider(selector, createProvider())
        |> ignore
        ()
