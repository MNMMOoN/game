# Architecture

The project deliberately boots from code. `GameBootstrap` creates services and
the prototype UI; `MatchController` owns a match but delegates rules.

| Layer | Responsibility |
|---|---|
| Core | app states, configuration, events, deterministic random source |
| Simulation | trail ring buffer, colony/fusion/crop rules, geometry grid |
| AI | personality-driven utility decisions and legal steering inputs |
| Presentation | Unity views, camera, arena, pooling and effects |
| UI | menu, HUD, leaderboard, results, safe-area layout |
| Persistence | versioned profile/settings, atomic JSON save and recovery |
| Audio | pooled audio/haptic service boundaries |
| Editor | content/scene helpers and Android/iOS build entry points |

Simulation data is plain C# where practical. Followers sample one bounded path
history; they never pathfind independently. Dangerous segments are queried via
a spatial hash. Strategic AI ticks at a lower rate than rendering and supports
near/mid/far frequency bands. Runtime objects come from reusable pools.

The prototype uses Unity primitives and generated materials, making it playable
without imported art. Scriptable tuning assets can override safe defaults.
