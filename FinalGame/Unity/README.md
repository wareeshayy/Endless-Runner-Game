# ZILL Runner — Unity Edition

An advanced 3D endless runner converted from the original C++ console game (`FileName.cpp`). The C++ source is unchanged in the parent folder.

## Original vs Unity

| Feature | C++ Console | Unity Edition |
|---------|-------------|---------------|
| Movement | A/D lane switch, W jump, S slide | Same + smooth lane tween, touch/mobile |
| Obstacles | Jump/slide box, jump-only box | Same + pooled prefabs, visual meshes |
| Lives | 3 hearts | 3 hearts + invincibility frames |
| Score | +10/frame, +50/spawn | Distance + coins + combo multiplier |
| Extras | — | Coins, shield, magnet, speed ramp, high score |

## Requirements

- **Unity 2022.3 LTS** or newer (2021.3+ also works)
- **TextMeshPro** (included with Unity; import TMP Essentials on first open)

## Quick Setup (5 minutes)

1. Open **Unity Hub** → **Add** → select this `Unity` folder.
2. Create/open scene `Assets/Scenes/GameScene.unity`.
3. Menu: **ZILL Runner → Setup Game Scene** (auto-builds track, player, managers).
4. Assign obstacle prefabs on `ObstacleSpawner` if you create custom meshes (optional — primitives work).
5. Press **Play**.

### Manual setup (if you skip the menu item)

1. Create empty `GameBootstrap` → add `GameBootstrap` component.
2. Create `Player` (Capsule) at `(0, 1, 0)` → tag `Player` → add `PlayerController`, `CharacterController`, `PlayerCollision`.
3. Create empty `Managers` with: `GameManager`, `ObstacleSpawner`, `UIManager`, `AudioManager`, `InputManager`.
4. Create `Canvas` (Screen Space Overlay) with HUD panels wired in `UIManager`.
5. Add `Main Camera` with `CameraFollow` targeting Player.

## Controls

| Action | Keyboard | Touch |
|--------|----------|-------|
| Left lane | A / Left Arrow | Swipe left |
| Right lane | D / Right Arrow | Swipe right |
| Jump | W / Space / Up | Swipe up |
| Slide | S / Down Arrow | Swipe down |
| Pause | Escape | Pause button |

## Obstacle types (from original)

- **FullBarrier** — jump **or** slide (empty box in C++).
- **JumpBarrier** — jump only (starred box in C++).

## Project structure

```
Assets/
  Scripts/
    Core/          GameManager, GameBootstrap, Constants
    Player/        PlayerController, PlayerCollision
    Obstacles/     Obstacle, ObstacleSpawner, ObstaclePool
    Collectibles/  Coin, PowerUp
    UI/            UIManager, MainMenuUI, GameOverUI
    Audio/         AudioManager
    Input/         InputManager
    Camera/        CameraFollow
    Editor/        GameSetupEditor
  Scenes/
    GameScene.unity (create in Unity)
```

## Credits

Original C++ game by Mahrukh (22F-3422), Sania (22F-3279), Wareesha (22F-3441).  
Unity port extends gameplay while preserving core mechanics.
