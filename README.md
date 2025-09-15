# 2D Roguelite

A cooperative roguelite prototype built in **Unity**.

## 🎮 Features (so far)
- Top-down player movement
- Shooting system with pooled bullets
- `WeaponDefinition` ScriptableObjects
- Per-weapon auto-fire toggle
- Combat feedback: **Damage Flash** + **Hitstop**

## 🛠️ Getting Started
1. **Clone the repo**
   ```bash
   git clone https://github.com/nrtrinid/2D-Roguelite.git
   cd 2D-Roguelite
   ```
2. **Open in Unity Hub** and use version **6000.2.3f1** (or your project’s current version).
3. **Open the main scene:** `Assets/Scenes/Main.unity`
4. **Press Play** to run.

## 📦 Project Conventions
- **Serialization:** *Force Text*
- **Version Control:** *Visible Meta Files*
- **Ignored:** `Library/`, `Temp/`, `Obj/`, `Build/`, `Logs/` (see `.gitignore`)

## 🔀 Branch Workflow
- `main` → stable, tested build only (protected)
- `feature/*` → gameplay features (e.g., `feature/combat`, `feature/sweeper`)
- `art/*` → sprites, tiles, animations (e.g., `art/tileset-floor`)
- Use Pull Requests to merge into `main`

**Commit style:** Conventional Commits  
Examples:
- `feat(combat): add hitstop and damage flash`
- `fix(weapon): apply Configure() before Init()`
- `chore(ci): update .gitignore`

## 🧪 Test Checklist (before merging to main)
- Bullets spawn via pool and despawn correctly
- `WeaponDefinition` overrides prefab stats (`Bullet.Configure()`)
- Auto-fire toggle works per weapon
- Damage flash + hitstop trigger on enemy hit
- Scene opens cleanly on a fresh clone

## 📋 Roadmap / Next Steps
- Sweeping magic weapon (arc emission, alternating sweep direction)
- Basic enemy AI + collisions
- Sprite integration from art branch
- UI polish (crosshair, damage numbers)

## 📜 Changelog
See `CHANGELOG.md`. Keep entries grouped under **[Unreleased]** and cut a version when merging to `main`.

---

> Tip: If cloning on a new machine, ensure **Git LFS** is installed if you add large assets.
