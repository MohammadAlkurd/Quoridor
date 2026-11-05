# Quoridor

A Godot 4 implementation of the board game **Quoridor**, written in C#. Written entirely by hand.

## About the game

Quoridor is an abstract strategy game for two players. Each turn you either move your pawn one square or place a wall. Walls can't fully block a path — the goal is to reach the opposite side of the board before your opponent does.

## Project layout

- `godot/` — the playable game: board, pieces, wall placement, menus, local play
- `backend/` — an unfinished experiment toward an online multiplayer server (.NET console app, raw TCP). Online play was never completed.

## Tech

Godot 4, C#

## Running

Open `godot/` in the Godot 4 editor (with .NET support) and run.
