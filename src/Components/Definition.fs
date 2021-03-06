namespace Ionide.VSCode.FSharp

open System
open Fable.Core
open Fable.Import
open Fable.Import.vscode
open Fable.Import.Node
open Ionide.VSCode.Helpers
open DTO

module Definition =
    let private createProvider () =

        let mapResult (doc : TextDocument) (pos : Position) (o : FindDeclarationResult) : Definition =
            if o |> unbox <> null then
                let loc = createEmpty<Location>
                let range = doc.getWordRangeAtPosition pos
                let length = range.``end``.character - range.start.character
                loc.uri <- Uri.file o.Data.File
                loc.range <- Range(float o.Data.Line - 1., float o.Data.Column - 1., float o.Data.Line - 1., float o.Data.Column + length  - 1.)
                loc |> Case1
            else
                createEmpty<Location> |> Case1


        { new DefinitionProvider
          with
            member this.provideDefinition(doc, pos, ct) =
                promise {
                    let! res = LanguageService.findDeclaration (doc.fileName) (int pos.line + 1) (int pos.character + 1)
                    return mapResult doc pos res
                } |> Case2

        }


    let activate selector (disposables: Disposable[]) =
        languages.registerDefinitionProvider(selector, createProvider())
        |> ignore

        ()
