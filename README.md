# Quoridor

A networked implementation of the board game **Quoridor** — a Godot client (C#) with a dedicated .NET game server for online matches, plus an offline mode.

## About the game

Quoridor is an abstract strategy game for two players. Each turn you either move your pawn one square or place a wall. Walls can't fully block a path — the goal is to reach the opposite side of the board before your opponent does.

## Project layout

- `godot/` — Godot 4 client written in C# (game board, pieces, walls, menus, networking code)
- `backend/` — standalone .NET server (`net7.0`, published as a single self-contained binary) that hosts matches over raw TCP and relays game state between the two connected players

## How it works

- The server listens on a configured IP/port and accepts exactly two clients per match (white and black), alternating turns.
- Clients connect using a shareable room code derived from the server address.
- The Godot client can also run an `Offline` mode for local play without the server.

## Tech

- **Client:** Godot 4, C#, GDScript
- **Server:** .NET 7 console app, `TcpListener`, `Newtonsoft.Json`

## Running

```bash
# server
cd backend && dotnet run

# client: open godot/ in the Godot 4 editor (with .NET support) and run
```
