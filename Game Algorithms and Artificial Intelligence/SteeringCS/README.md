# Zombie Sound Simulator

## Overview

Zombie Sound Simulator is a simple top-down 2D game where the player must reach a safe zone while avoiding zombies. The unique mechanic of the game is that zombies react to **sound** instead of only seeing the player. The player can move around the map and create sound (for example by throwing a rock) to distract zombies and escape.

## Player Objective

The goal of the player is to safely reach the **safe zone** without being caught by zombies. If a zombie reaches the player, the game ends.

## Controls

* **WASD** – Move the player
* **Space** – Throw a rock to create a sound that attracts zombies

## AI Techniques

### Steering Behaviours

Zombies use several steering behaviours to move around the map:

* **Wander** – Zombies move randomly when idle
* **Seek** – Zombies move toward a sound or the player
* **Obstacle Avoidance** – Zombies avoid walls and buildings

### Pathfinding

Zombies use **A*** pathfinding with a navigation graph to move around obstacles when traveling to a sound location or chasing the player.

### Behaviour (State Machine)

Each zombie uses a simple state machine with three states:

* **Idle** – Wander around randomly
* **Investigate Sound** – Move toward a sound source
* **Chase Player** – Chase the player when detected

### Fuzzy Logic

Fuzzy logic is used to determine how strongly zombies react to sounds.

Inputs:

* Sound volume
* Distance to the sound

Output:

* Zombie interest level (how strongly they react)

## Debug Visualization

The game includes several debug views to visualize AI behaviour:

* **F1** – Show steering vectors
* **F2** – Show navigation graph
* **F3** – Show A* pathfinding
* **F4** – Show zombie state
* **F5** – Show sound radius

These visualizations help demonstrate how the AI systems work in the game.

## Goal of the Project

This project demonstrates the use of several **Game AI techniques**, including steering behaviours, pathfinding, state machines, and fuzzy logic to create believable character behaviour in a simple 2D environment.
