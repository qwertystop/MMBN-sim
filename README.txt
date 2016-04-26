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


Gameplay:
~~~~~~~~~~~~~~~~~~   TODO


Work split:
"I" here refers to David Heyman.
Game produced by David Heyman and Dorian Franklin.
~~~~~~~~~~~~~~~~~ TODO
One script made by Dorian, RowChip.cs, is not used in the final product. In attempting to fix one potential bug (it would only hit one thing in the target area, which will be relevant when chips that add  obstacles or other summons are added), I accidentally introduced an infinite loop. Looking at the code and trying to figure out how that had happened and how to fix it, I decided that, since the class only provided a small increase in efficiency of setting up chips in the editor, and only for a very specific case, it would be easier to just rewrite the chips that used it to use RelativeAOEChip.cs instead. The file remains in the project for logging purposes.


Outside Sources:
Megaman Battle Network is owned by Capcom (more's the pity...)

Original game's sprite-sheets taken from http://www.sprites-inc.co.uk/sprite.php?local=/EXE/

Parts of scripts based on code provided by HeatPhoenix (of mmbn3d.tumblr.com), who is making a similar game as a hobby. The original code which he provided to me for reference is contained in the folder MMBN3D.

Sprite animation system originally from http://gamasutra.com/blogs/DonaldKooiker/20150821/251260/Luckslinger_tech_1__How_to_make_2D_Sprite_Animations_in_Unity.php

Custom-screens redesigned by my sister, Maya Heyman. Goal of redesign was to re-organize things to allow the HP box to overlap it without covering anything of note, since the original graphics were not designed for two players to share a screen.