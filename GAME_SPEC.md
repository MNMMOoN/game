# WaddleRush game specification

WaddleRush is a 2–5 minute, one-thumb, entirely offline arena game. The player
steers a lead penguin and a growing waddle against local AI. Fish add mass,
three equal-tier penguins fuse into a compact stronger tier, and crossing an
enemy trail crops the vulnerable tail rather than causing routine instant death.

## Play loop

Menu → play → steer and boost → collect → fuse → cut or evade → rank → results
→ instant rematch. A match starts with a leader and three mass-one chicks.

## Rules

- Small/medium/golden fish are worth 1/3/8 mass.
- Three chicks fuse to a mass-3 Scout; three Scouts fuse to a mass-9 Emperor.
- Fusion conserves mass while shortening the physical trail.
- A cut removes units after the hit index; at least one follower is protected.
- A cropped colony receives 1.25 seconds of immunity. Lost mass becomes a
  bounded set of equivalent-value fish.
- Head/trail hits remove 40% of exposed mass, clamped to survivor protection.
- Boost increases speed and reduces turn authority while energy remains.
- Scores and the leaderboard are local. No online claim is made.

## Presentation

Aurora Ice Bay is a clean turquoise 120 m square arena with snow banks, ocean,
simple rocks, fish, readable colored scarves, a tilted follow camera, compact
HUD, boost button, leaderboard, mission hint, results, and immediate rematch.

## Accessibility and ethics

Music/SFX/haptics, reduced motion, reduced effects, and color-friendly markers
are local settings. There are no ads, accounts, timers, paid power, telemetry,
or network calls.
