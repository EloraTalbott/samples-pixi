module App

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Pixi
open Fable.Import.Pixi.Sound
open Fable.Import.Browser
open Fable.Import.Pixi.Particles
open Fable.Pixi
open Elmish
open Hink
open Types

// our view
let options = jsOptions<PIXI.ApplicationOptions> (fun o ->
  o.backgroundColor <- Some 0x000000
  o.antialias <- Some true
)
let app = PIXI.Application(800., 600., options)
Browser.document.body.appendChild(app.view) |> ignore

let renderer : PIXI.WebGLRenderer = !!app.renderer


let startGame() = 

  let mutable screen = Title None
  
  // our render loop  
  app.ticker.add (fun delta -> 

    screen <- 
      match screen with 

      | NextScreen nextScreen -> nextScreen

      | GameOver -> screen
      
      | Title model -> 

        let model, moveToNextScreen = ScreenIntroduction.Update model app.stage !!app.renderer delta
        if not moveToNextScreen then  
          ScreenKind.Title (Some model)
        else
          // do some cleanup
          ScreenIntroduction.Clean model          
          // move to next screen
          NextScreen (ScreenKind.GameOfCogs None)
      
      | GameOfCogs model ->  
        
        let model, moveToNextScreen = GameOfCogs.Update model app.stage !!app.renderer delta

        if not moveToNextScreen then  
          ScreenKind.GameOfCogs (Some model)
        else
          // do some cleanup
          GameOfCogs.Clean model          
          // move to next screen
          NextScreen (ScreenKind.Title None)

    ) |> ignore 

// start our main loop
let init() = 

  // We start by loading our assets 
  let loader = PIXI.loaders.Loader()
  let path = "../img/draggor"
  [
    // particle confi files
    ("rightConfig",sprintf "%s/right.json" path)
    ("leftConfig",sprintf "%s/left.json" path)

    // pictures
    ("help1",sprintf "%s/help1.png" path)
    ("help2",sprintf "%s/help2.png" path)
    ("particle",sprintf "%s/particle.png" path)
    ("cog",sprintf "%s/cog.png" path)
    ("great",sprintf "%s/great.png" path)
    ("target",sprintf "%s/target.png" path)
    ("title",sprintf "%s/Title.png" path)
    ("subtitle",sprintf "%s/subtitle.png" path)
    
    // sounds
    ("goodMove",sprintf "%s/goodMove.mp3" path)
  ] 
  |> Seq.iter( fun (name,path) -> loader.add(name,path) |> ignore  )

  loader.load( fun (loader:PIXI.loaders.Loader) (res:PIXI.loaders.Resource) ->
    
    // fill our Asset store 
    Assets.addTexture "help1" !!res?help1?texture 
    Assets.addTexture "help2" !!res?help2?texture 
    Assets.addTexture "cog" !!res?cog?texture 
    Assets.addTexture "target" !!res?target?texture 
    Assets.addTexture "particle" !!res?particle?texture 
    Assets.addTexture "great" !!res?great?texture 
    Assets.addTexture "title" !!res?title?texture 
    Assets.addTexture "subtitle" !!res?subtitle?texture 

    // our particle configuration file 
    Assets.addObj "rightConfig" !!res?rightConfig?data
    Assets.addObj "leftConfig" !!res?leftConfig?data
    
    Assets.addSound "goodMove" !!res?goodMove?data

    // Let's have some fun now!    
    startGame()

  ) |> ignore

init() // it all begins there

(*
let play error (sound:PIXI.sound.Sound) ok = 
  printfn "in"
  sound.play() |> ignore 

let path = "../img/draggor"
let o = jsOptions<PIXI.sound.Options> (fun o ->
  o.url <- Some (sprintf "%s/goodMove.mp3" path)
  o.preload <- Some true
  o.loaded <-  Some play
)
PIXI.sound.Sound.from !!o |> ignore
*)
(*
let sound = PIXI.sound.Globals.add("goodMove",!!o)
sound.play() |> ignore
*)