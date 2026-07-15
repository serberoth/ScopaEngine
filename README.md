# ScopaEngine

A fully playable implementation of Scope, a traditional Italian card game,
using a Windows Form application.  The application supports one v. the computer;
however, the engine can technically support multiple players up to 4 either as
human or AI.  The AI is an adjusting min-max algorithm accounting for the players
current scoring and tricks taken over time in the round.


## About Scopa

Scopa is a classic Italian card game played with a traditional 40-card
Italian deck. Players score points by capturing cards from the table,
with bonus points awarded for capturing all table cards in a single play
— a "scopa".

## Interfaces

Two interfaces are supported:

- **GUI** — windowed interface for standard interactive play
- **Console** — text-based interface for play without a GUI

## Building

Open `ScopaEngine.sln` in Visual Studio and build the solution.

## How the AI works

The AI opponent uses a minimax search to evaluate positions and select
moves, weighing capture opportunities and scoring potential to play
competitively.

## Status

Feature-complete. Not actively developed.
