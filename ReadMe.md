# Hollow Knight: GodhomeQoL

This mod is useful for challenge runners who don't want their runs to be accidentally affected by QoL mods.

GodhomeQoL provides a carefully selected set of features **built upon** and inspired by the following mods:

*   **HollowKnight.QoL** by fifty-six: [https://github.com/fifty-six/HollowKnight.QoL](https://github.com/fifty-six/HollowKnight.QoL)
*   **HollowKnight.GodSeekerPlus** by Clazex: [https://github.com/Clazex/HollowKnight.GodSeekerPlus](https://github.com/Clazex/HollowKnight.GodSeekerPlus)
*   **ShowHPOnDeath** by FIN: [https://github.com/F1NS3N/ShowHPOnDeath](https://github.com/F1NS3N)

This mod emphasizes stability and predictability.

Every feature is opt-in: on a fresh install (and once after updating to this version) all modules are disabled, so the mod does not change the game until you enable something yourself.

See [INSTALL.md](INSTALL.md) for installation and build instructions.

## Known issues

*   **Freeze Hitboxes:** freezing on death (with "Any Hits" turned off) currently does not trigger; only the "Any Hits" mode freezes. It is not yet known whether this predates the opt-in rework (where the module's hooks started being attached only when the feature is switched on) or was introduced by it.
