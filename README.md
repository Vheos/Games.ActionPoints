# Table of contents
- [About](https://github.com/Vheos/Games.ActionPoints#about)
- [Game design](https://github.com/Vheos/Games.ActionPoints#game-design)
- [Code design](https://github.com/Vheos/Games.ActionPoints#code-design)
- [Progress](https://github.com/Vheos/Games.ActionPoints#progress)

# About
On the surface, it's just another of my countless Unity projects. But in reality it's **THE CHOSEN ONE** - chosen to get publicly released instead of privately shelved ;) So yeah, that's basically it - `ActionPoints` is my first attempt at actual finished product. Wish me luck!

# Game design
### Gameplay loop
The main inspiration for the core gameplay loop is [Darkest Dungeon](https://www.gog.com/game/darkest_dungeon), in which you first explore a dungeon and fight enemies, then manage your party in a city to prepare for another expedition. While exploring, it's also possible to camp, which allows your party members to use camping-only skills. In `ActionPoints`, I'lll skip the city phase, but expand upon the camping phase, so the gameplay loop will become `Explore & Fight -> Camp -> ...`
### Action points
(soon)
### Wounds
(soon)
### Chance-based damage
(soon)
### Damage mitigation formulas
The game will feature a different damage formula for each of its mitigatable armor types: `Blunt` armor will be applied `additively`, so 80 blunt damage against 40 blunt armor will result in `80 - 40 = 30` damage. `Sharp` armor will be applied `multiplicatively`, so 80 sharp damage against 40 sharp armor will result in `80 x 60% = 48` damage. There's also an unmitigatable damage type (called `Pure`) which will be used for special effects - like poison or magic.
### Simple math
Most of the game math will use whole numbers, usualy below 100 (for percentage chances) and even below 10 (for action costs). 

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
