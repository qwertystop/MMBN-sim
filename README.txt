Video link:
https://youtu.be/guRGONokrGg

Controls:
Emulates controls of GBA, not including Start and Select
____________________________
|P1 Key : P2 Key : Emulated|
|-------+--------+---------|
|   w   :   up   :    up   |
|   s   :  down  :   down  |
|   a   :  left  :   left  |
|   d   :  right :   right |
|   b   :    [   :     A   |
|   n   :    ]   :     B   |
|   g   :    -   :     L   |
|   h   :    =   :     R   |
----------------------------


Gameplay (controls as written are from the Emulated column above - see equivalent per player):
Gameplay attempts to simulate the GBA game Megaman Battle Network 2. If you already know how to play that, skip to the section that specifies the bugs, necessary modifications, and things I couldn't manage to implement. If not, read the below two sections

Game starts with both players on the Custom Screen. Each player uses directional keys and the A button to select chips from their hand (drawn at random from pre-defined folder). Rules for chip selection: Can select up to 5 chips. Can select multiple chips if they all have the same letter code (* counts as any letter) or all have the same name. After selection, choose OK to use those in the next turn, or ADD to discard them. If you choose ADD, all subsequent turns you will draw more chips (by the amount discarded), until you run out. Once both players have confirmed, the main turn begins. Move with the directions, press A to use a chip (each gets one use, used in the order selected). Press B to shoot the MegaBuster (5 damage to one target in front of user, full-stage range, infinite uses). Hold B to charge the buster (when charged, animation goes purple). Release the charge to do more damage than an uncharged shot (50). Each player's HP is shown in the top corner on their side.

Descriptions of implemented chips:
WideSwrd: Deal 80 damage to enemies in column directly ahead of user.
LongSwrd: Deal 80 damage to enemies in two panels in front of user.
ShotGun: Deal 30 damage to first enemy in row ahead of user, and enemies immediately behind them.
V-Gun: Deal 30 damage to first enemy in row ahead of user, and enemies in V-shape behind them.

------------------------

Known bugs:
-Rarely, firing the uncharged buster causes an exception. Source is unknown beyond that it's thrown from Animation2D.Stop(), bug is difficult to replicate reliably - cannot discern consistent situation to cause it. If exception is swallowed without pausing the game, game functions normally from that point on, other than that the single attempt to fire the buster does not work (bug remains just as inconsistently-appearing from then on).
-Certain sprite-animation-based effects will remain frozen after completing, instead of disappearing, but not always. Cause is uncertain, but seems related to starting more than one of them very close together in time (within a few frames). I have been able to replicate it by rapid-firing an uncharged buster, or by firing the Shotgun or V-Gun chips (which produce several such effects simultaneously). However, other things which start several sprites at once (the LongSwrd and WideSwrd chips, or both players firing busters at each other on the same frame) do not.

Necessary change:
-Custom Screen slightly re-organized from original to be able to fit two of them on-screen at once. Players seeing each others' screens is thus unfortunately possible.

Not yet implemented (with descriptions):
-Most chips
-Functionality for chips which cannot be represented by a single hitscan (non-instant projectiles, summons, other effects over time, etc). The framework should be able to support them with minimal changes, though, once the derived classes are written for each distinct effect
-Displaying small icons of selected chips in a stack over each player's head.
-Displaying name and damage of next chip to use in text at the bottom.
-Original fonts for chip names and codes (aren't in any spritesheets I've found).
-Game automatically ending on one player running out of HP
-Ability to customize chip folder pre-game (not in the Unity editor)
-Regular chips (one chip in folder guaranteed to be drawn first)
-Program Advances (automatic replacement of certain combinations of selected chips with single, extremely powerful chips). The canonical example from the original game, having existed virtually unchanged throughout the series: Sword/WideSwrd/LongSwrd, with no more than one *-code between them, produces the PA LifeSwrd, which does 400 damage in the two columns ahead of the player).
-Styles (alternate forms), which would be chosen immediately prior to folder customization. Each style gives the character a non-null element (meaning some chips do double-damage), changes their charged buster shot to a different attack with range and effects depending on the element (Aqua styles hit as an Aqua-element Shotgun and have a faster charge time, for example). Also, as a second parameter independent of element, a style adds some other effect (Shield styles start with a barrier that blocks all attacks until it has taken 10 damage; Custom styles start off drawing 6 chips per turn, etc).
-Sound
-Hitstun and other status effects
-Some sprites for various effects have poorly-placed pivots and need adjusting - most visibly obvious on the two Sword chips.


Work split:

Game produced by David Heyman with assistance from Dorian Franklin.

Produced by Dorian:
Sprite-sheet trimming by automatic button for 14 sheets (one manually modified in an image editor first). Of these, 3 were overwritten later (purely automatic sprite cutout produced incorrect results), and the other 11 are not used in the final game.

One off-by-one-bug fix in the Player script.

The creation of the RowChip.cs script, which is not used in the final game, and a prefab using it. It applies damage to all targets in an area defined as a rectangle measured from the panel immediately in front of the user. In attempting to fix one potential bug in that script (it would only hit one thing in the target area, which will be relevant when chips that add  obstacles or other summons are added - it seemed reasonably implementable at the time, though not anymore), another bug was accidentally introduced and/or revealed. Looking at the code and trying to figure out how that had happened and how to fix it, I decided that, since the class only provided a small increase in efficiency of setting up chips in the editor, and only for a very specific case, it would be easier to just rewrite the chips that used it to use RelativeAOEChip.cs instead (which can specify any arbitrary collection of panels relative to the user). The file remains in the project for logging purposes only.

Taken from outside sources:
Megaman Battle Network is owned by Capcom (more's the pity...)

Original game's sprite-sheets taken from http://www.sprites-inc.co.uk/sprite.php?local=/EXE/

Parts of some scripts are based on code provided by HeatPhoenix (of mmbn3d.tumblr.com), who is making a similar game as a hobby. The original code which he provided for reference is contained in the folder MMBN3D.

Animation2D.cs is modified from code taken originally from http://gamasutra.com/blogs/DonaldKooiker/20150821/251260/Luckslinger_tech_1__How_to_make_2D_Sprite_Animations_in_Unity.php

Custom-screens redesigned by Maya Heyman (sister of David Heyman). Goal of redesign was to re-organize things to allow the HP box to overlap it without covering anything of note, since the original graphics were not designed for two players to share a screen.

Remainder (most of the project) by David Heyman.