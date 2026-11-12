# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.0.0] - 2026-11-12

### Added

- The game starts with no studio logos and no "Made with Unity" logo (`SkipSplash`).
- The black fades at the title screen, around each save load, at a new game, a host or a join, and at a respawn take 0.1 s (`FadeSeconds`). The fades in the world, such as the wake-up after sleep, stay as in the game.
- Save loads take about half the time. Distant terrain loads with a short delay (`FastWorldLoad`). Your View Distance setting does not change.
- The mod writes nothing to the save and changes no game file. Made and tested for single player.
