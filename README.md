# Table of contents
- [About](https://github.com/Vheos/Games.ActionPoints#About)
- [Game design](https://github.com/Vheos/Games.ActionPoints#Hame-design)
  - [Gameplay loop](https://github.com/Vheos/Games.ActionPoints#Gameplay-loop)
  - [Action system](https://github.com/Vheos/Games.ActionPoints#Action-system)
  - [Single resource bar](https://github.com/Vheos/Games.ActionPoints#Single-resource-bar)
  - [Damage types](https://github.com/Vheos/Games.ActionPoints#Damage-types)
  - [Chance rolls](https://github.com/Vheos/Games.ActionPoints#Chance-rolls)
  - [Simple math](https://github.com/Vheos/Games.ActionPoints#Simple-math)
- [Code design](https://github.com/Vheos/Games.ActionPoints#Code-design)
  - [Composition over inheritance](https://github.com/Vheos/Games.ActionPoints#Composition-over-inheritance)
  - [Event-driven communication](https://github.com/Vheos/Games.ActionPoints#Event-driven-communication)
  - [Subject-observer pattern](https://github.com/Vheos/Games.ActionPoints#Subject-observer-pattern)
  - [No asset store](https://github.com/Vheos/Games.ActionPoints#No-asset-store)
- [Progress](https://github.com/Vheos/Games.ActionPoints#Progress)

# About
On the surface, it's just another of my countless Unity projects. But in reality it's **THE CHOSEN ONE** - chosen to get publicly released instead of privately shelved ;) So yeah, that's basically it - `ActionPoints` is my first attempt at actual finished product. Wish me luck!

# Game design
### Gameplay loop
- the main inspiration for the core gameplay loop is [Darkest Dungeon](https://www.gog.com/game/darkest_dungeon), in which you first explore a dungeon and fight enemies, then manage your party in a city to prepare for another expedition. While exploring, it's also possible to camp, which allows your party members to use camping-only skills. In `ActionPoints`, I'll skip the city phase, but expand upon the camping phase, so the gameplay loop will become `Explore & Fight -> Camp`.
### Action system
- each character will have a number of `Action points` that can be used to perform actions during exploration, combat and camping. Action points recharge at various times during the game - for example, slowly during combat or instantly during camping.
- action points may get *over-charged* under certain conditions, becoming `Focus points` - rare, extra resource used to perform powerful actions.
- when a character uses up more action points then they have, they will receive `Exhaust points` - essentialy negative action points that prevent the character from using any more actions until recharged.
- when a character gets successfully damaged (since damage will chance-based), they will receive a `Wound point`. Wound points cannot be recharged, effectively reducing the character's maximum available action points. Moreover, if all of character's action points become wound points, that character will die.
### Single resource bar
- many games feature separate bars for each of character-related resources - like health, mana and stamina. In `ActionPoints`, all resources will be combined into a single bar consisting of multiple *points*, each of which usually represents only one resource (or lack thereof) at any given time. This way, the UI won't be cluttered with multiple bars, and all vital information will be visible at first glance.
- example resoure bar, from left to right;
<br/>*(image of action points bar with all types of resource points)*
  - fully charged focus point
  - fully charged action point with partially charged (unusable) focus
  - fully charged action point
  - partially charged (unusable) action point
  - wound (unchargeable action point)
### Damage types
- the game will feature 2 mitigatable damage/armor types (blunt and sharp) and 1 unmitigatable damage type (pure).
- blunt armor mitigates blunt damage additively, following the formula `Damage - Armor`. Dealing 80 blunt damage against character with 60 blunt armor will result in `80 - 60 = 20` damage. This encourages single high-damage attacks instead of multiple weaker attacks.
- sharp armor mitigates sharp damage multiplicatively and is displayed as percentage. It follows the formula `Damage x (1 - Armor)`. Dealing 80 sharp damage against 60% sharp armor will result in `80 x (1 - 60%) = 80 x 40% = 32` damage. This type of damage doesn't encourage either big or quick attacks.
- pure damage is unmitigatable, so it doesn't have a corresponding armor type. It will be used for special effects, like poison or magic.
### Chance rolls
- damage and healing are actually percentage chances of inflicting or removing a single wound, so dealing 99 damage might not actually have any effect.
- for every 100 damage or healing, there will be guaranteed success. The remaining amount (below 100) will be rolled as usual, so 150 damage is guaranteed to inflict 1 wound, and 50% of the time will inflict 2 wounds.
### Simple math
- most of the game math will use whole numbers, usualy below 100 (for percentage chances) and even below 10 (for action costs).
- there will be very little inflation throughout the game - end-game characters shouldn't be more than twice as powerful as a starting characters.

# Code design
### Composition over inheritance
This time around, I'm actively trying **NOT** to get tangled up in inheritance spaghetti. Yep, I'm finally yielding to the Unity-suggested pseudo-ECS pattern by creating a lot of small, specialized components instead of huge, god-like ones.
### Event-driven communication
Instead of one component calling methods of another to change its state, it will merely invoke some event - whether any other component decides to react to it, that's none of its concern. This way the event handler (at the very least) won't be coupled with its listeners.
### Subject-observer pattern
Managed components (observers) merely observe their manager (subject) - which, in best case scenario, doesn't even have to know about their existence. For example, the `SkillBar` component instantiates and initalizes `SkillButton`s, which subscribe to the `SkillBar`'s events. After the initialization, the `SkillBar` doesn't really have to remember what components are observing it (although it might want to).
### No asset store
Mostly to learn the far limits of Unity, but also to develop my own tools along the way, which I'll move to my [game-dev core package](https://github.com/Vheos/Games.Core) over time.

# Progress
(soon)
