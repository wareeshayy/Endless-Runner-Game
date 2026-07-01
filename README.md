# Endless Runner Game

A 3D endless runner with two versions: an original **C++ console game** and a **Unity edition** with enhanced gameplay, visuals, and mobile support.

## Repository contents

```
Endless-Runner-Game/
├── FinalGame.zip          # Archived project bundle
└── FinalGame/
    ├── FileName.cpp       # Original C++ console game
    └── Unity/             # Unity port (ZILL Runner)
```

## Unity edition (recommended)

The Unity version extends the original mechanics with smooth lane switching, obstacle pooling, coins, power-ups, combo scoring, and touch controls.

| Feature | C++ Console | Unity Edition |
|---------|-------------|---------------|
| Movement | A/D lane switch, W jump, S slide | Same + smooth lane tween, touch/mobile |
| Obstacles | Jump/slide box, jump-only box | Same + pooled prefabs, visual meshes |
| Lives | 3 hearts | 3 hearts + invincibility frames |
| Score | +10/frame, +50/spawn | Distance + coins + combo multiplier |
| Extras | — | Coins, shield, magnet, speed ramp, high score |

### Requirements

- **Unity 2022.3 LTS** or newer (2021.3+ also works)
- **TextMeshPro** (import TMP Essentials on first open)

### Quick start

1. Open **Unity Hub** → **Add** → select the `FinalGame/Unity` folder.
2. Create or open scene `Assets/Scenes/GameScene.unity`.
3. Menu: **ZILL Runner → Setup Game Scene** (auto-builds track, player, managers).
4. Press **Play**.

For manual setup, controls, and project structure, see [FinalGame/Unity/README.md](FinalGame/Unity/README.md).

### Controls

| Action | Keyboard | Touch |
|--------|----------|-------|
| Left lane | A / Left Arrow | Swipe left |
| Right lane | D / Right Arrow | Swipe right |
| Jump | W / Space / Up | Swipe up |
| Slide | S / Down Arrow | Swipe down |
| Pause | Escape | Pause button |

## C++ console edition

The original game lives in `FinalGame/FileName.cpp`. Compile and run it with any C++ compiler that supports standard console I/O.

## Credits

Original C++ game by Mahrukh (22F-3422), Sania (22F-3279), Wareesha (22F-3441).

Unity port extends gameplay while preserving core mechanics.

## License

This project is provided as-is for educational purposes.
