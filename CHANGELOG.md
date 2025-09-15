# Changelog
All notable changes to this project will be documented here.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
### Added
- Sweeping magic weapon (planned)
- Basic enemy AI + collisions (planned)
- Sprite integration from art branch (planned)

### Changed
- UI polish and crosshair visuals (planned)

## [0.1.0] - 2025-09-14
### Added
- `WeaponDefinition` ScriptableObject for bullet stats and firing settings
- Bullet.Configure() to override prefab defaults at runtime
- Per-weapon auto-fire toggle
- DamageFlash system for hit feedback
- Hitstop system to pause briefly on hit impact
- TestWeapon asset for iteration

### Changed
- WeaponShooter updated to apply stats from `WeaponDefinition`
- Bullet prefab cleaned up to rely on `Configure()`

### Removed
- Reliance on prefab-only bullet stats (now owned by WeaponDefinition)
