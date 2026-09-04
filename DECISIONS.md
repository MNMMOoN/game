# Decisions

- Unity 2022.3 LTS baseline with URP packages; runtime visuals use primitives so
  missing art never blocks play.
- A code bootstrap avoids fragile hand-authored scene YAML in a repository that
  began empty; editor tooling can materialize conventional scenes later.
- Colony mass is an integer and unit tiers are values 1, 3 and 9.
- Collision is deterministic 2D XZ geometry, independent of follower colliders.
- Save files contain no account or remote identifiers and use temp/backup swap.
- Bots submit desired heading/boost to the same mover used by the player.
