# About
On the surface, it's just another of my countless Unity projects... But in reality, it's **THE CHOSEN ONE** - chosen to get publicly released instead of privately shelved ;) Wish me luck! ([project board](https://github.com/users/Vheos/projects/2/views/2))

# Game design

### Inspirations
- [Darkest Dungeon](https://www.gog.com/game/darkest_dungeon) - core gameplay loop and phases
- [For the King](https://store.steampowered.com/app/527230/For_The_King/) - focus points and chance rolls
- [Neuroshima (tabletop RPG)](https://en.wikipedia.org/wiki/Neuroshima) - wounds instead of HP
- [Griftlands](https://store.steampowered.com/app/601840/Griftlands/) - combat system
- [Res Arcana (board game)](https://boardgamegeek.com/boardgame/262712/res-arcana) - minimalistic action cards design

### Gameplay loop
- the main inspiration for the core gameplay loop is [Darkest Dungeon](https://www.gog.com/game/darkest_dungeon), in which you first explore a dungeon and fight enemies, then manage your party in a city to prepare for another expedition. While exploring, it's also possible to camp, which allows your party members to use camping-only abilities. 

- in *ActionPoints*, the city and camping phases will be combined into one, so the gameplay loop will become *Explore -> Camp*, with *Combat* occuring mostly during exploration, but also possibly when resting at an unguarded camp.

### Action system
- characters will interact with the world by using their *Actions*. Each action will be usable only during certain game phase - for example, a character might be able to use *Disarm trap* while exploring and *Patrol* while camping, but won't have any special combat actions.
- characters will gain actions from equipment, progression and events. For example, wielding an axe may allow you to use *Chop* combat action and *Chop firewood* camping action, or a magic ritual may permanently grant you *Fireball* combat action and *Light bonfire* camping action.
- actions will cost resources - most commonly the easily-available action points, but may also require the rare focus points or even some consumable items.

### Action resources
- characters will have a number of *Maximum points* that represent both their health and stamina at the same time.
- points can be *charged*, usually by resting. A fully charged point becomes an *Action point* - the most common resource used to perform actions.
- under certain conditions, action points may get *focused* (over-charged). A fully focused action point becomes a *Focus point* - rare, extra resource used to perform unique actions. Focus points also count as normal action points, and can be used as such in a pinch, but that would waste their potential.
- when a character gets wounded, they will receive a *Wound point*. Wound points cannot be charged or focused, effectively reducing the character's maximum available points. Moreover, if all of character's points become wound points, that character will die.

### Single resource bar
- many games feature separate bars for each of character-related resources - like health, stamina or mana. In *ActionPoints*, all resources will be combined into a single bar consisting of multiple *points*, each of which usually represents only one resource (or lack thereof) at any given time. This way, the UI won't be cluttered with multiple bars, and all vital information will be visible at first glance.
- *(image of action points bar with all types of resource points)*
<br/>from left to right:
  - focus point (also counts as an action point)
  - action point, partially focused
  - action point
  - partially charged point
  - wound (cannot be charged)

### Combat system
- WIP
- actions may be used even if the character doesn't have enough action points, but doing so will put the character in an *exhausted* state, unable to perform anything else until all exhaust points are recharged.
- when a character uses up more action points then they have, they will receive *Exhaust points* - essentialy negative action points that prevent the character from using any more actions until recharged.
- *(image of action points bar with all types of resource points)*
<br/>from left to right:
  - exhaust point
  - partialy exhausted point
  - empty point
  - wound (cannot be charged or exhausted)
  - wound (cannot be charged or exhausted)

### Damage types
- the game will feature 2 mitigatable damage/armor types (blunt and sharp) and 1 unmitigatable damage type (pure).
- blunt armor mitigates blunt damage additively, following the formula `Damage - Armor`. Dealing 80 blunt damage against character with 60 blunt armor will result in `80 - 60 = 20` damage. This encourages single high-damage attacks instead of multiple weaker attacks.
- sharp armor mitigates sharp damage multiplicatively and is displayed as percentage. It follows the formula `Damage x (1 - Armor)`. Dealing 80 sharp damage against 60% sharp armor will result in `80 x (1 - 60%) = 80 x 40% = 32` damage. This type of damage doesn't encourage either big or quick attacks.
- pure damage is unmitigatable, so it doesn't have a corresponding armor type. It will be used for special effects, like poison or magic.

## No stalling
- in Darkest Dungeon, there is an abusable tactic called *stalling*, which can used to fully heal up after most combats and thus reducing the survival factor. It's performed by killing the most dangerous enemies, then using 1-3 characters to stun-lock the remaining enemies while the remaining party members heal up everyone.
- this tactic trivializes the dungeon-exploration phase so much that there exists an *anti-stalling* mechanic - when there are few enemies left and the player characters use too many non-damaging actions, the enemies will call for reinforcements.
- both stalling and anti-stalling exist because there are actions that replenish a valubale resource (health) practically for free, but only in combat. If either the heals *required another valuable resource*, were *usable outside of combat* or there were *no combat heals* at all, then there would be no benefit in stalling.
- in *ActionPoints* I have decided to go for the most survival solution - the only healing combat actions will be consumables, which will be sparse and valuable. In effect, there won't be any cheap way to remove a wound after it's been inflicted, but there will be to prevent a wound from being inflicted.
- essentially, *reactive* heals will be replaced by *proactive* wards (or barriers). Wards will simply tank a certain amount of unmitigated damage before they break and expose the character. This will make the supports' role much more tactical than the usual *"if anyone's missing HP, spam heal"*.

### Chance rolls
- damage and healing are actually percentage chances of inflicting or removing a single wound, so dealing 99 damage might not actually have any effect (if you're unlucky).
- for every 100 damage or healing, there will be guaranteed success. The remaining amount (below 100) will be rolled as usual, so 150 damage is guaranteed to inflict 1 wound, with 50% chance of inflicting 2 wounds instead.

### Luck
- there will be an attribute called *Luck* that will secretly cause rerolls. I haven't decided yet whether this attribute will be per-character or party wide.
- if luck is positive, every failed positive roll (such as healing) and every successful negative roll (such as taking damage) against/by the character will be rolled again, with chance equal to the luck's value. This essentially increases chances of positive rolls being successful, and negative rolls being failed.
- analogcially, if luck is negative, every successful positive roll and failed negative roll will be rerolled with chance equal to the luck's absolute value.
- there will be visual feedback (like a four-leaf clover pop-up) when a roll has been successful or failed because of the luck reroll.

### Simple math
- most of the game math will use whole numbers, usualy below 100 (for percentage chances) and even below 10 (for action costs).
- there will be very little inflation throughout the game - end-game characters shouldn't be more than twice as powerful as a starting characters.



# Code design

### Composition over inheritance
- this time around, I'm actively trying **NOT** to get tangled up in inheritance spaghetti. Yep, I'm finally yielding to the Unity-suggested pseudo-ECS pattern by creating a lot of small, specialized components instead of huge, god-like ones.

### Event-driven communication
- instead of one component calling methods of another to change its state, it will merely invoke some event - whether any other component decides to react to it, that's none of its concern. This way the event handler (at the very least) won't be coupled with its listeners.

### Subject-observer pattern
- managed components (observers) merely observe their manager (subject) - which, in best case scenario, doesn't even have to know about their existence. For example, the `ActionBar` component instantiates and initalizes `ActionButton`s, which subscribe to the `ActionBar`'s events. After the initialization, the `ActionBar` doesn't really have to remember what components are observing it (although it might want to).

### Disabled domain reloading
- WIP

### New input system package
- WIP

### No asset store
- Mostly to learn the far limits of Unity, but also to develop my own tools along the way, which I'll move to my [game-dev core package](https://github.com/Vheos/Games.Core) over time.



# Progress
- (soon)
