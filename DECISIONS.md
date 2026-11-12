# DECISIONS.md - Assignment 10: Three Enemy Types


2026-11-11  Asked the agent for enemy scripts as state machines. Patroller: Patrol -> Chase -> WindUp -> Cooldown. Shooter: Idle -> Aim -> Reload. Charger: Idle -> WindUp -> Dash -> Recover. I checked that the player is cached in Start (no Find in Update) and that sight uses a Raycast with the Walls layer so walls block it.

2026-11-11  Charger drifted away like in space after a small push. Cause: Rigidbody2D had no damping. Fix: raised Linear Damping and Mass, froze Rotation Z.

2026-11-12  Patroller hit the player only when the player stood still: the hit check ran at the end of the wind-up and the player had already walked away. Added a separate hitRange so dodging needs more distance.

2026-11-12  Phone build failed: Samsung A02 is ARMv7 and the APK was ARM64 only. Enabled ARMv7 + ARM64 in Player Settings. Active Input Handling left on Both, tested on the phone.

2026-11-12  Playtest on phone, two testers who had not seen the game. Arman (original version): 1 try, the level was too easy, enemies were easy to dodge. Munisa (changed version): 2 tries.

2026-11-12  Changed after the first playtest: Patroller windUpTime 0.6 -> 0.4, cooldownTime 1.2 -> 0.7, attackRange 1 -> 1.3, chaseSpeed 3.5 -> 4.2. Map: moved the passage a bit higher and slightly changed the zones so the player can dodge the enemies.
