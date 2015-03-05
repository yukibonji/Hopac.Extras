﻿namespace Hopac.Extras

open Hopac
open Hopac.Infixes
open Hopac.Alt.Infixes
open Hopac.Job.Infixes

[<Sealed>]
type Semaphore(n: int) = 
    do assert (0 <= n)
    let inc = ch()
    let dec = ch()
    do server << Job.iterate n <| fun n -> 
       if 0 < n then (dec >>%? n - 1) <|>? (inc >>%? n + 1)
       else (inc >>%? n + 1)
    member this.Release = inc <-- ()
    member this.Wait = dec <-- ()

[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Semaphore =
    let inline wait (s: Semaphore) = s.Wait
    let inline release (s: Semaphore) = s.Release
    let holding s j = Job.tryFinallyJob (wait s >>. j) s.Release
