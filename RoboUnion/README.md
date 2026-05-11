# Robo Union *(More Players)*

Protecting Semibot Rights Since 2025

*(Updated to works with REPO v0.4.0)*

## About

The Robo Union empowers YOU 🫵 to safely increase (or decrease) the maximum number of workers on a R.E.P.O. team. Leave no Semibot behind!

**Features:**

- Increase or decrease player count (MorePlayers)
- Works with both public and private lobbies.
- Only the Host needs to have RoboUnion installed
- ~~Fallback handling when MaxPlayers is set higher than permitted by the game servers.~~ (Semiwork built this into the base game now. W devs.)

**Limitations:**

By default, this mod increases the lobby size to 10 players which is stable on most setups. This can be configured to any number, but REPO's photon servers will not allow lobbies with more than 20 players.

## Supported Versions

| Mod Version | Game Version    |
| ----------- | --------------- |
| v0.6.0+     | v0.4.x          |
| v0.5.0      | v0.1.x - v0.3.x |

## Installation

1. Use a mod manager
    - I **highly** recommend using [Gale Mod Manager](https://thunderstore.io/c/repo/p/Kesomannen/GaleModManager/).
    It's faster than other mod managers, has no ads, and doesn't require any third-party apps.
    - Other mod managers work as well.
2. Select the game R.E.P.O.
3. Add the RoboUnion mod.
4. Launch the game!

## Configuration (Changing Max Players)

RoboUnion can be configured to change the max number of players.

**Method 1 - Configure With Mod Manager:**

1. Run the game with the mod installed at least once.
2. In the mod manager, go to the 'mod config editor' tab.
3. Find and click on 'RoboUnion.cfg' or 'Linkoid.Repo.RoboUnion.cfg'
4. Change the settings under the 'General' header.

**Method 2 - Configure in-game with REPOConfig:**

1. Install the [REPOConfig mod](https://thunderstore.io/c/repo/p/nickklmao/REPOConfig/).
2. Launch the game.
3. In the 'Mods' setting menu, find RoboUnion.
4. Change the settings under the 'General' header.

**Method 3 - Configure directly via config file (not recommended):**

1. Run the game with the mod installed at least once.
2. In file explorer, navigate to `{GAME_OR_PROFILE_FOLDER}/BepInEx/config/Linkoid.Repo.RoboUnion.cfg`.
3. Open the config file in a text editor and change the values under the 'General' header directly.

## Troubleshooting and Debugging

If the configured player limit is greater than the max players permitted by the servers,
an error message will be displayed and the player cap will automatically be adjusted for that session.

You may encounter stability issues with a large number of players.
Some debug tools for viewing network traffic are provided in the config.
Technical users may use these tools to test for and remove mods that may cause increased traffic, leading to instability.

## Bugs / Contact

The fastest way to contact me is via Discord.
You can find me on the [R.E.P.O. Modding Discord](https://discord.gg/vPJtKhYAFe) in the [RoboUnion Thread](https://discord.com/channels/1344557689979670578/1368823239643758682),
or in my [Deja Drift Discord Server](https://discord.gg/yKwt2AWcGF).

## Credits

- Icon by @Lordfirespeed and @linkoid on Discord
- @1A3 for drafting part of the RoboUnion v0.6 update
- Shoutout to Zelofi who made the original [MorePlayers](https://github.com/zelofi/MorePlayers) mod which lifted the 6 player cap in the early REPO days.
